using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using System.Threading;


namespace LidgrenServer
{
    public class PlayerPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
    class Server
    {
        private NetServer server;
        private Thread thread;
        private List<string> players;
        private Dictionary<string, PlayerPosition> playerPositions;
        public Server() 
        {
            players = new List<string>();
            playerPositions = new Dictionary<string, PlayerPosition>();

            NetPeerConfiguration config = new NetPeerConfiguration("game");
            config.MaximumConnections = 20;
            config.Port = 1337;

            server = new NetServer(config);
            server.Start();

            thread = new Thread(Listen);
            thread.Start();
        } 

        public void Listen()
        {
            Logging.Info("Listening for clients ...");

            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
            {
                NetIncomingMessage message;

                while((message = server.ReadMessage()) != null)
                {
                    Logging.Info("Message received");

                    // Get List of users
                    List<NetConnection> all = server.Connections;
                    
                    switch(message.MessageType)
                    {
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)message.ReadByte();

                            string reason = message.ReadString();
                            
                            if(status == NetConnectionStatus.Connected)
                            {
                                var player = NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier);

                                // Add our player to the dictionary
                                players.Add(player);

                                // Send Player ID
                                SendLocalPlayerPacket(message.SenderConnection, player);

                                // Send Spawn Info
                                SpawnPlayers(all, message.SenderConnection, player);
                            }

                            break;
                        case NetIncomingMessageType.Data:
                            byte type = message.ReadByte();
                            Packet packet;
                            switch(type)
                            {
                                case (byte)PacketTypes.PositionPacket:
                                    packet = new PlayerPositionPacket();
                                    packet.NetIncomingMessageToPacket(message);
                                    SendPositionPacket(all, (PlayerPositionPacket)packet);
                                    break;
                                case (byte)PacketTypes.PlayerDisconnectsPacket: 
                                    packet = new PlayerDisconnetsPacket();
                                    packet.NetIncomingMessageToPacket(message);
                                    SendPlayerDisconnectsPacket(all, (PlayerDisconnetsPacket)packet);
                                    break;
                                default:
                                    Logging.Error("Unhandled Data / Packet type");
                                    break;
                            }
                            break;
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                            string text = message.ReadString();
                            Logging.Debug(text);
                            break;
                        default:
                            Logging.Error($"Unhandled type: {message.MessageType} {message.LengthBytes} Bytes {message.DeliveryMethod} | {message.SequenceChannel}");
                            break;
                    }

                    server.Recycle(message);
                }
            }
        }
        public void SpawnPlayers(List<NetConnection> all, NetConnection local, string player)
        {
            // Spawn all clients on the local player
            all.ForEach (p =>
            {
                string _player = NetUtility.ToHexString(p.RemoteUniqueIdentifier);

                if(player != _player)
                    SendSpawnPacketToLocal(local, _player, playerPositions[_player].X, playerPositions[_player].Y);

            });

            // Spawn the local player on all the clients
            Random random = new Random();
            SendSpawnPacketToAll(all, player, random.Next(-3, 3), random.Next(-3, 3));

        }

        public void SendLocalPlayerPacket(NetConnection local, string player)
        {
            Logging.Info($"Sending player their user ID: {player}");

            NetOutgoingMessage message = server.CreateMessage();
            new LocalPlayerPacket() { ID = player}.PacketToNetOutGoingMessage(message);
            server.SendMessage(message, local, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void SendSpawnPacketToLocal(NetConnection local, string player, float X, float Y)
        {
            Logging.Info($"Sending user spawn info for player: {player}");

            playerPositions[player] = new PlayerPosition() { X = X, Y = Y };

            NetOutgoingMessage message = server.CreateMessage();
            new PlayerSpawnPacket() { Player = player, X = X, Y = Y }.PacketToNetOutGoingMessage(message);
            server.SendMessage(message, local, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void SendSpawnPacketToAll(List<NetConnection> all, string player, float X, float Y)
        {
            Logging.Info($"Sending user spawn info for player: {player}");

            playerPositions[player] = new PlayerPosition() { X = X, Y = Y };

            NetOutgoingMessage message = server.CreateMessage();
            new PlayerSpawnPacket() { Player = player, X = X, Y = Y }.PacketToNetOutGoingMessage(message);
            server.SendMessage(message, all, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void SendPositionPacket(List<NetConnection> all, PlayerPositionPacket packet) 
        {
            Logging.Info($"Sending position for {packet.Player}");

            playerPositions[packet.Player] = new PlayerPosition { X = packet.X, Y = packet.Y };

            NetOutgoingMessage message = server.CreateMessage();
            packet.PacketToNetOutGoingMessage(message);
            server.SendMessage(message, all, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void SendPlayerDisconnectsPacket(List<NetConnection> all, PlayerDisconnetsPacket packet)
        {
            Logging.Info($"Player disconnected {packet.Player}");

            playerPositions.Remove(packet.Player);
            players.Remove(packet.Player);

            NetOutgoingMessage message = server.CreateMessage();
            packet.PacketToNetOutGoingMessage(message);
            server.SendMessage(message, all, NetDeliveryMethod.ReliableOrdered, 0);
        }
    }
}
