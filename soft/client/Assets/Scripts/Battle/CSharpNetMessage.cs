using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public delegate void NetMessage(s_net_message s);
public class CSharpNetMessage
{
    public static Dictionary<opclient_t, List<NetMessage>> csharpNetEvents = new Dictionary<opclient_t, List<NetMessage>>();
    public static List<opclient_t> csharpNetList = new List<opclient_t>()
    {
        opclient_t.SMSG_BATTLE_LINK,
        opclient_t.SMSG_BATTLE_OP,
        opclient_t.SMSG_BATTLE_ZHEN,
        opclient_t.SMSG_BATTLE_FINISH,
        opclient_t.SMSG_GUIDE
    };

    public static List<opclient_t> client_wait_lua_Net_list = new List<opclient_t>()
    {

    };

    public static void AddCSharpNetEvent(opclient_t opcode, NetMessage m,bool exp = false)
    {
        if (!csharpNetEvents.ContainsKey(opcode))
        {
            if (!exp)
            {
                if (!csharpNetList.Contains(opcode))
                    csharpNetList.Add(opcode);
            }
            else
            {
                if (!client_wait_lua_Net_list.Contains(opcode))
                    client_wait_lua_Net_list.Add(opcode);
            }
            csharpNetEvents.Add(opcode, new List<NetMessage>());
        }
        csharpNetEvents[opcode].Add(m);
    }

    public static void RemoveCSharpNetEvent(opclient_t opcode, NetMessage m)
    {
        if (csharpNetEvents.ContainsKey(opcode) && csharpNetEvents[opcode].Contains(m))
            csharpNetEvents[opcode].Remove(m);
    }

    public static void Deal(s_net_message s)
    {
        opclient_t m = (opclient_t)s.opcode;
        if (csharpNetEvents.ContainsKey(m))
        {
            for (int i = 0; i < csharpNetEvents[m].Count; i++)
            {
                csharpNetEvents[m][i](s);
            }
        }
    }
}

