using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Lidgren.Network;

namespace LidgrenClient
{
    public class PlayerPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
    public class Client
    {
        public NetClient client { get; set; }
        public Client(int port, string server, string serverName)
        {
            var config = new NetPeerConfiguration(serverName);
            config.AutoFlushSendQueue = false;

            client = new NetClient(config);
            client.RegisterReceivedCallback(new SendOrPostCallback(ReceiveMessage));

            client.Start();
            client.Connect(server, port);
        }

        public void ReceiveMessage(object peer)
        {
            NetIncomingMessage message;

            while((message = client.ReadMessage()) != null)
            {
                Debug.Log("Message received from server");

                switch(message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        // Get Packet Type
                        byte packetType = message.ReadByte();

                        // Create Packet
                        Packet packet;

                        switch (packetType)
                        {
                            case (byte)PacketTypes.LocalPlayerPacket:
                                packet = new LocalPlayerPacket();
                                packet.NetIncomingMessageToPacket(message);
                                ExtractLocalPlayerInformation((LocalPlayerPacket)packet);
                                break;
                            case (byte)PacketTypes.PlayerDisconnectsPacket:
                                packet = new PlayerDisconnetsPacket();
                                packet.NetIncomingMessageToPacket(message);
                                DisconnectPlayer((PlayerDisconnetsPacket)packet);
                                break;
                            case (byte)PacketTypes.PositionPacket:
                                packet = new PlayerPositionPacket();
                                packet.NetIncomingMessageToPacket(message);
                                UpdatePlayerPosition((PlayerPositionPacket)packet);
                                break;
                            case (byte)PacketTypes.SpawnPacket:
                                packet = new PlayerSpawnPacket();
                                packet.NetIncomingMessageToPacket(message);
                                SpawnPlayer((PlayerSpawnPacket)packet);
                                break;
                            default:
                                Debug.Log("Unhandled pacet type");
                                break;
                        }

                        break;
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = message.ReadString();
                        Debug.Log(text);
                        break;
                    default:
                        Debug.Log("Unhandled message type");
                        break;
                }

                client.Recycle(message);
            }
        }

        public void SendPosition(float X, float Y)
        {
            Debug.Log("Sending Position");

            NetOutgoingMessage message = client.CreateMessage();
            new PlayerPositionPacket() { Player = StaticManager.LocalPlayerID, X = X, Y = Y }.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
        }

        public void SendDisconnect()
        {
            Debug.Log("Disconnecting from server.");
            NetOutgoingMessage message = client.CreateMessage();
            new PlayerDisconnetsPacket() { Player = StaticManager.LocalPlayerID}.PacketToNetOutGoingMessage(message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
        }

        public void ExtractLocalPlayerInformation(LocalPlayerPacket packet)
        {
            Debug.Log($"Local ID is : {packet.ID}");

            StaticManager.LocalPlayerID = packet.ID;
        }

        public void SpawnPlayer(PlayerSpawnPacket packet)
        {
            Debug.Log($"Spawning player {packet.Player}");

            GameObject playerPrefab = (GameObject)Resources.Load("Player");
            Vector3 position = new Vector3(packet.X, packet.Y);

            GameObject player = MonoBehaviour.Instantiate(playerPrefab, position, Quaternion.identity);

            // If the client is out local player, add controls!
            if(packet.Player == StaticManager.LocalPlayerID)
            {
                player.AddComponent<Controller>();
                player.transform.name = "Local Player";
            }
            else
            {
                player.transform.name = packet.Player;
            }

            StaticManager.Players.Add(packet.Player, player);
        }

        public void UpdatePlayerPosition(PlayerPositionPacket packet)
        {
            Debug.Log($"Moving player: {packet.Player}");

            StaticManager.Players[packet.Player].gameObject.GetComponent<Movement>().SetNextPosition(new Vector3(packet.X, packet.Y));
        }

        public void DisconnectPlayer(PlayerDisconnetsPacket packet)
        {
            Debug.Log($"Removing player {packet.Player}");

            MonoBehaviour.Destroy(StaticManager.Players[packet.Player]);
            StaticManager.Players.Remove(packet.Player);
        }
    }
}

