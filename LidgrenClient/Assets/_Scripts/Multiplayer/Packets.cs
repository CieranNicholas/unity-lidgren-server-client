using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

namespace LidgrenClient
{
    public enum PacketTypes
    {
        LocalPlayerPacket,
        PlayerDisconnectsPacket,
        PositionPacket,
        SpawnPacket
    }

    public interface IPacket
    {
        void PacketToNetOutGoingMessage(NetOutgoingMessage message);
        void NetIncomingMessageToPacket(NetIncomingMessage message);
    }

    public abstract class Packet : IPacket
    {
        public abstract void PacketToNetOutGoingMessage(NetOutgoingMessage message);
        public abstract void NetIncomingMessageToPacket(NetIncomingMessage message);
    }

    public class LocalPlayerPacket : Packet
    {
        public string ID { get; set; }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.LocalPlayerPacket);
            message.Write(ID);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            ID = message.ReadString();
        }
    }

    public class PlayerDisconnetsPacket : Packet
    {
        public string Player { get; set; }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.PlayerDisconnectsPacket);
            message.Write(Player);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            Player = message.ReadString();
        }
    }

    public class PlayerPositionPacket : Packet
    {
        public float X { get; set; }
        public float Y { get; set; }
        public string Player { get; set; }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.PositionPacket);
            message.Write(X);
            message.Write(Y);
            message.Write(Player);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            X = message.ReadFloat();
            Y = message.ReadFloat();
            Player = message.ReadString();
        }
    }

    public class PlayerSpawnPacket : Packet
    {
        public float X { get; set; }
        public float Y { get; set; }
        public string Player { get; set; }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.SpawnPacket);
            message.Write(X);
            message.Write(Y);
            message.Write(Player);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            X = message.ReadFloat();
            Y = message.ReadFloat();
            Player = message.ReadString();
        }
    }
}

