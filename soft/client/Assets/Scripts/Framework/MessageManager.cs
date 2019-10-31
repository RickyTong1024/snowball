using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class s_message
{
    public string name;
    public float time = 0.0f;
	public ArrayList m_object = new ArrayList ();
}

public class s_net_message
{
    public int opcode;
    public float time = 0.0f;
    public byte[] buffer;
    public LuaInterface.LuaByteBuffer luabuff;
}

public interface IHandle
{
    void message(s_message message);
    void net_message(s_net_message message);
}

public class MessageManager : MonoBehaviour
{
    protected HashSet<IHandle> m_handles = new HashSet<IHandle>();
    protected List<s_message> m_messages = new List<s_message>();
    protected List<s_net_message> m_net_messages = new List<s_net_message>();
    protected List<s_net_message> m_csharp_message = new List<s_net_message>();

    public void RegisterHandle(IHandle handle)
    {
        if (!m_handles.Contains(handle))
        {
            m_handles.Add(handle);
        }
    }

    public void RemoveHandle(IHandle handle)
    {
        if (m_handles.Contains(handle))
        {
            m_handles.Remove(handle);
        }
    }

    public void AddMessage(s_message message)
    {
        m_messages.Add(message);
    }

    public void AddNetMessage(s_net_message message)
    {
        m_net_messages.Add(message);
    }

    public void AddCSharpNetMessage(s_net_message message)
    {
        m_csharp_message.Add(message);
    }

    void Update()
	{
        for (int c = 0; c < m_messages.Count; )
        {
            s_message message = m_messages[c] as s_message;
            if (message.time <= 0.0f)
            {
                m_messages.RemoveAt(c);
                foreach (IHandle handle in m_handles)
                {
                    if (handle != null)
                    {
                        handle.message(message);
                    }
                }
                Util.CallMethod("Message", "OnMessage", message);
            }
            else
            {
                message.time -= Time.deltaTime;
                c++;
            }
        }

        for (int c = 0; c < m_net_messages.Count; )
        {
            s_net_message message = m_net_messages[c] as s_net_message;
            if (message.time <= 0.0f)
            {
                m_net_messages.RemoveAt(c);
                foreach (IHandle handle in m_handles)
                {
                    if (handle != null)
                    {
                        handle.net_message(message);
                    }
                }
                message.luabuff = new LuaInterface.LuaByteBuffer(message.buffer);
                Util.CallMethod("Message", "OnNetMessage", message);
            }
            else
            {
                message.time -= Time.deltaTime;
                c++;
            }
		}

        for (int c = 0; c < m_csharp_message.Count;)
        {
            s_net_message message = m_csharp_message[c] as s_net_message;
            if (message.time <= 0.0f)
            {
                m_csharp_message.RemoveAt(c);
                //µ÷ÓÃ·ÖÅä
                CSharpNetMessage.Deal(message);
            }
            else
            {
                message.time -= Time.deltaTime;
                c++;
            }
        }
    }
}