using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UdpSave
{
    public byte[] buff;
    public int len;
    public ulong time;
}


public class UdpRecv
{
    public byte[] buff;
    public int len;
}

public class UdpSocketClient
{
    private string m_name;

    private const int BUFF_SIZE = 512;
    private const int MAX_PCK_SIZE = 512;

    private RakNet.RakPeerInterface client;
    private bool is_connect_ = false;
    private MemoryStream memStream;
    private BinaryReader reader;
    private Packet m_packet = new Packet();
    private RakNet.SystemAddress m_addr;

    private readonly object m_lockObject = new object();

    // Use this for initialization
    public UdpSocketClient()
    {
        memStream = new MemoryStream();
        reader = new BinaryReader(memStream);
    }

    public void SendConnect(string name, string addr, int port)
    {
        m_name = name;
        client = RakNet.RakPeerInterface.GetInstance();
        RakNet.SocketDescriptor socketDescriptor = new RakNet.SocketDescriptor();

        System.Net.IPAddress[] address = System.Net.Dns.GetHostAddresses(addr);
        if (address.Length == 0)
        {
            Debug.Log("udp Ipaddress length 0");
            return;
        }

        if (address[0].AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            socketDescriptor.socketFamily = (short)System.Net.Sockets.AddressFamily.InterNetworkV6;
        }
        else
        {
            socketDescriptor.socketFamily = (short)System.Net.Sockets.AddressFamily.InterNetwork;
        }
            

        client.Startup(1, socketDescriptor, 1, 1);
        client.Connect(addr, (ushort)port, "", 0);
        client.SetTimeoutTime(3000, RakNet.RakNet.UNASSIGNED_SYSTEM_ADDRESS);
        m_addr = new RakNet.SystemAddress(addr, (ushort)port);
    }

    public void SendMessage(byte[] buffer)
    {
        if (client == null)
        {
            Debug.Log("SendMessage client == null");
            return;
        }
        int pos = 0;
        int len = buffer.Length;
        while (len > 0)
        {
            int num = 500;
            if (num > len)
            {
                num = len;
            }
            byte[] buffer1 = new byte[num];
            Array.Copy(buffer, pos, buffer1, 0, num);
            RakNet.BitStream rakStringTestSendBitStream = new RakNet.BitStream();
            rakStringTestSendBitStream.Write((byte)RakNet.DefaultMessageIDTypes.ID_USER_PACKET_ENUM);
            rakStringTestSendBitStream.Write(buffer1, (uint)buffer1.Length);
            uint r = client.Send(rakStringTestSendBitStream, RakNet.PacketPriority.HIGH_PRIORITY, RakNet.PacketReliability.RELIABLE_ORDERED, (char)0, m_addr, false);
            if (r == 0)
            {
                Debug.Log("SendMessage fail");
            }
            len = len - num;
            pos = pos + num;
        }
    }

    public int GetPing()
    {
        if (client == null)
        {
            return 0;
        }
        int a = client.GetLastPing(m_addr);
        if (a < 0)
        {
            a = 0;
        }
        return a;
    }

    void Dispose()
    {
        if (client != null)
        {
            RakNet.RakPeerInterface.DestroyInstance(client);
            client = null;
        }
        is_connect_ = false;
        reader.Close();
        memStream.Close();
    }

    public void Close(bool is_send = true)
    {
        if (!is_connect_)
        {
            return;
        }
        client.CloseConnection(m_addr, is_send);
        s_net_message mes = new s_net_message();
        mes.opcode = -2;
        NetworkManager.AddEvent(m_name, mes);
        Dispose();
    }

    public void Update()
    {
        if (client == null)
        {
            return;
        }
        for (RakNet.Packet p = client.Receive(); p != null; client.DeallocatePacket(p), p = client.Receive())
        {
            switch ((RakNet.DefaultMessageIDTypes)p.data[0])
            {
                case RakNet.DefaultMessageIDTypes.ID_DISCONNECTION_NOTIFICATION:
                case RakNet.DefaultMessageIDTypes.ID_CONNECTION_LOST:
                    {
                        Close(false);
                        break;
                    }
                case RakNet.DefaultMessageIDTypes.ID_CONNECTION_REQUEST_ACCEPTED:
                    {
                        is_connect_ = true;
                        s_net_message mes = new s_net_message();
                        mes.opcode = -1;
                        NetworkManager.AddEvent(m_name, mes);
                        break;
                    }
                case RakNet.DefaultMessageIDTypes.ID_INVALID_PASSWORD:
                case RakNet.DefaultMessageIDTypes.ID_NO_FREE_INCOMING_CONNECTIONS:
                case RakNet.DefaultMessageIDTypes.ID_CONNECTION_ATTEMPT_FAILED:
                case RakNet.DefaultMessageIDTypes.ID_CONNECTION_BANNED:
                case RakNet.DefaultMessageIDTypes.ID_IP_RECENTLY_CONNECTED:
                case RakNet.DefaultMessageIDTypes.ID_INCOMPATIBLE_PROTOCOL_VERSION:
                    {
                        s_net_message mes = new s_net_message();
                        mes.opcode = -3;
                        NetworkManager.AddEvent(m_name, mes);
                        Close(false);
                        break;
                    }
                case RakNet.DefaultMessageIDTypes.ID_SND_RECEIPT_LOSS:
                    {
                        Debug.Log("ID_SND_RECEIPT_LOSS");
                        break;
                    }
                case RakNet.DefaultMessageIDTypes.ID_USER_PACKET_ENUM:
                    {
                        RakNet.BitStream receiveBitStream = new RakNet.BitStream();
                        OnReceive(p.data, (int)p.length);
                        break;
                    }
            }
            if (client == null)
            {
                break;
            }
        }
    }

    private long RemainingBytes()
    {
        return memStream.Length - memStream.Position;
    }

    void OnReceive(byte[] bytes, int length)
    {
        memStream.Seek(0, SeekOrigin.End);
        memStream.Write(bytes, 1, length - 1);
        //Reset to beginning
        memStream.Seek(0, SeekOrigin.Begin);
        while (RemainingBytes() >= Packet.size)
        {
            byte[] _lenbyte = new byte[Packet.size];
            reader.Read(_lenbyte, 0, Packet.size);
            m_packet.m_compress = _lenbyte[0];
            m_packet.m_opcode = System.BitConverter.ToUInt16(_lenbyte, 2);
            m_packet.m_size = System.BitConverter.ToInt32(_lenbyte, 4);
            if (RemainingBytes() >= m_packet.m_size)
            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(ms);
                writer.Write(reader.ReadBytes(m_packet.m_size));
                ms.Seek(0, SeekOrigin.Begin);
                OnReceivedMessage(ms);
            }
            else
            {
                //Back up the position two bytes
                memStream.Position = memStream.Position - Packet.size;
                break;
            }
        }
        //Create a new stream with any leftover bytes
        byte[] leftover = reader.ReadBytes((int)RemainingBytes());
        memStream.SetLength(0);     //Clear
        memStream.Write(leftover, 0, leftover.Length);
    }

    void OnReceivedMessage(MemoryStream ms)
    {
        BinaryReader r = new BinaryReader(ms);
        byte[] bf = r.ReadBytes((int)(ms.Length - ms.Position));
        if (m_packet.m_compress != 0)
        {
            bf = utils.Decompress(bf);
        }
        s_net_message mes = new s_net_message();
        mes.opcode = m_packet.m_opcode;
        mes.buffer = bf;
        NetworkManager.AddEvent(m_name, mes); 
    }

    public bool is_close()
    {
        return client == null;
    }
}
