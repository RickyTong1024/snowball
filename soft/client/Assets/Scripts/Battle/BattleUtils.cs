using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class BattleUtils
{
    public static Dictionary<string, Dictionary<opclient_t, long>> send_sockets = new Dictionary<string, Dictionary<opclient_t, long>>();
    public static Dictionary<opclient_t, long> recive_sockets = new Dictionary<opclient_t, long>();

    public static void add(int opcode, s_net_message message)
    {
        if (BattleUtils.recive_sockets.ContainsKey((opclient_t)message.opcode))
        {
            if (message.buffer != null)
                BattleUtils.recive_sockets[(opclient_t)message.opcode] += (message.buffer.Length + 8);
            else
                BattleUtils.recive_sockets[(opclient_t)message.opcode] = 8;
        } 
        else
        {
            if(message.buffer != null)
                BattleUtils.recive_sockets.Add((opclient_t)message.opcode, message.buffer.Length + 8);
            else
                BattleUtils.recive_sockets.Add((opclient_t)message.opcode,8);
        }
            
    }
}
