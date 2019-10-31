using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;

public class NetworkManager : MonoBehaviour
{
    Dictionary<string, SocketClient> m_sockets = new Dictionary<string, SocketClient>();
    Dictionary<string, UdpSocketClient> m_udp_sockets = new Dictionary<string, UdpSocketClient>();
    static readonly object m_lockObject = new object();
    static Queue<KeyValuePair<string, s_net_message>> mEvents = new Queue<KeyValuePair<string, s_net_message>>();
    private static int COMPRESS_SIZE = 1024;

    /// <summary>
    /// 发送链接请求
    /// </summary>
    public void Connect(string name, string addr, int port) {
        SocketClient c = new SocketClient();
        c.SendConnect(name, addr, port);
        m_sockets[name] = c;
    }

    public void UdpConnect(string name, string addr, int port)
    {
        UdpSocketClient c = new UdpSocketClient();
        c.SendConnect(name, addr, port);
        m_udp_sockets[name] = c;
    }

    public void Disconnect(string name)
    {
        if (m_sockets.ContainsKey(name))
        {
            m_sockets[name].Close();
            m_sockets.Remove(name);
        }
        else if (m_udp_sockets.ContainsKey(name))
        {
            m_udp_sockets[name].Close();
            m_udp_sockets.Remove(name);
        }
    }
    public bool Isconnect(string name)
    {
        if (m_sockets.ContainsKey(name))
        {
            return true;
        }
        else if (m_udp_sockets.ContainsKey(name))
        {
            return true;
        }
        return false;
    }

    public int GetPing(string name)
    {
        if (m_udp_sockets.ContainsKey(name))
        {
            return m_udp_sockets[name].GetPing();
        }
        return 0;
    }

    /// <summary>
    /// 发送SOCKET消息
    /// </summary>
	public void SendMessage(string name, int opcode, byte[] buffer)
    {
        bool ys = false;
        if (buffer.Length >= COMPRESS_SIZE)
        {
            buffer = utils.Compress(buffer);
            ys = true;
        }
        byte[] _data = new byte[Packet.size + buffer.Length];
        System.BitConverter.GetBytes(ys).CopyTo(_data, 0);
        System.BitConverter.GetBytes((ushort)opcode).CopyTo(_data, 2);
        System.BitConverter.GetBytes(buffer.Length).CopyTo(_data, 4);
        buffer.CopyTo(_data, 8);
        if (m_sockets.ContainsKey(name))
        {
            m_sockets[name].SendMessage(_data);
        }
        else if (m_udp_sockets.ContainsKey(name))
        {
            m_udp_sockets[name].SendMessage(_data);
        }

        if (!BattleUtils.send_sockets.ContainsKey(name))
            BattleUtils.send_sockets.Add(name, new Dictionary<opclient_t, long>());

        if (BattleUtils.send_sockets[name].ContainsKey((opclient_t)opcode))
            BattleUtils.send_sockets[name][(opclient_t)opcode] += _data.Length;
        else
            BattleUtils.send_sockets[name].Add((opclient_t)opcode, _data.Length);

    }

	public void SendMessageNull(string name, int opcode)
    {
        byte[] _data = new byte[Packet.size];
        System.BitConverter.GetBytes(false).CopyTo(_data, 0);
        System.BitConverter.GetBytes((ushort)opcode).CopyTo(_data, 2);
        System.BitConverter.GetBytes(0).CopyTo(_data, 4);
        if (m_sockets.ContainsKey(name))
        {
            m_sockets[name].SendMessage(_data);
        }
        else if (m_udp_sockets.ContainsKey(name))
        {
            m_udp_sockets[name].SendMessage(_data);
        }

        if (!BattleUtils.send_sockets.ContainsKey(name))
            BattleUtils.send_sockets.Add(name, new Dictionary<opclient_t, long>());

        if (BattleUtils.send_sockets[name].ContainsKey((opclient_t)opcode))
            BattleUtils.send_sockets[name][(opclient_t)opcode] += _data.Length;
        else
            BattleUtils.send_sockets[name].Add((opclient_t)opcode, _data.Length);
    }

    void OnDestroy()
    {
        foreach (SocketClient c in m_sockets.Values)
        {
            c.Close();
        }
        foreach (UdpSocketClient c in m_udp_sockets.Values)
        {
            c.Close();
        }
    }

    public static void AddEvent(string name, s_net_message message)
    {
        lock (m_lockObject)
        {
            mEvents.Enqueue(new KeyValuePair<string, s_net_message>(name, message));
        }
    }

    void Update()
    {
        List<string> names = new List<string>();
        foreach (string name in m_sockets.Keys)
        {
            SocketClient sc = m_sockets[name];
            if (sc.is_close())
            {
                names.Add(name);
            }
        }
        for (int i = 0; i < names.Count; ++i)
        {
            m_sockets.Remove(names[i]);
        }
        names.Clear();
        foreach (string name in m_udp_sockets.Keys)
        {
            UdpSocketClient sc = m_udp_sockets[name];
            if (sc.is_close())
            {
                names.Add(name);
            }
            else
            {
                sc.Update();
            }
        }
        for (int i = 0; i < names.Count; ++i)
        {
            m_udp_sockets.Remove(names[i]);
        }
        while (true)
        {
            int num = 0;
            lock (m_lockObject)
            {
                num = mEvents.Count;
            }
            if (num > 0)
            {
                KeyValuePair<string, s_net_message> _event;
                lock (m_lockObject)
                {
                    _event = mEvents.Dequeue();
                }

                if (_event.Value.opcode == -1)
                {
                    Debug.Log("OnConnect");
                    Util.CallMethod(_event.Key, "OnConnect");
                }
                else if (_event.Value.opcode == -2)
                {
                    Util.CallMethod(_event.Key, "OnDisconnect");
                }
                else if (_event.Value.opcode == -3)
                {
                    Util.CallMethod(_event.Key, "OnConnectFail");
                }
                else if (CSharpNetMessage.csharpNetList.Contains((opclient_t)(_event.Value.opcode)))
                {
                    AppFacade._instance.MessageManager.AddCSharpNetMessage(_event.Value);
                }
                else
                {
                    AppFacade._instance.MessageManager.AddNetMessage(_event.Value);
                    if (CSharpNetMessage.client_wait_lua_Net_list.Contains((opclient_t)(_event.Value.opcode)))
                        AppFacade._instance.MessageManager.AddCSharpNetMessage(_event.Value);
                }
            }
            else
            {
                break;
            }
        }
    }
}