using BattleDB;
using protocol.game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class BattleAchieve
{
    public static List<int> completeAchieve = new List<int>();
    public static Dictionary<int, int> battleAchieveList = new Dictionary<int, int>();
    public static Dictionary<int, int> taskAchieveList = new Dictionary<int, int>();
    public static Dictionary<int, int> dailyAchieveList = new Dictionary<int, int>();

    public static void Load(ArrayList arryList)
    {
        completeAchieve.Clear();
        for (int i = 0; i < arryList.Count; i++)
        {
            int achieve_id = Convert.ToInt32(arryList[i]);
            BattleAchieve.completeAchieve.Add(achieve_id);
        }
    }

    public static ArrayList PushResultToLua()
    {
        ArrayList list = new ArrayList();
        for (int i = 0; i < BattleAchieve.completeAchieve.Count; i++)
            list.Add(BattleAchieve.completeAchieve[i]);

        return list;
    }

    public static void Init()
    {
        BattleAchieve.battleAchieveList.Clear();
        BattleAchieve.taskAchieveList.Clear();
        BattleAchieve.dailyAchieveList.Clear();
    }

    public static void RegisterMessage()
    {
        CSharpNetMessage.AddCSharpNetEvent(opclient_t.SMSG_BATTLE_ACHIEVE, BattleAchieve.SMSG_BATTLE_ACHIEVE);
        CSharpNetMessage.AddCSharpNetEvent(opclient_t.SMSG_BATTLE_TASK, BattleAchieve.SMSG_BATTLE_TASK);
        CSharpNetMessage.AddCSharpNetEvent(opclient_t.SMSG_BATTLE_DAILY, BattleAchieve.SMSG_BATTLE_DAILY);
    } 
    public static void RemoveMessage()
    {
        CSharpNetMessage.RemoveCSharpNetEvent(opclient_t.SMSG_BATTLE_ACHIEVE, BattleAchieve.SMSG_BATTLE_ACHIEVE);
        CSharpNetMessage.RemoveCSharpNetEvent(opclient_t.SMSG_BATTLE_TASK, BattleAchieve.SMSG_BATTLE_TASK);
        CSharpNetMessage.RemoveCSharpNetEvent(opclient_t.SMSG_BATTLE_DAILY, BattleAchieve.SMSG_BATTLE_DAILY);
    }


    public static bool IsFirstComplete(long id)
    {
        for (int i = 0; i < completeAchieve.Count; i++)
        {
            if (BattleAchieve.completeAchieve[i] == id)
                return false;
        }

        for (int i = 0; i < self.player.achieve_reward.Count; i++)
        {
            if (self.player.achieve_reward[i] == id)
                return false;
        }
        return true;
    }

    public static void AddBattleTaskNumber(int id, int value)
    {
        if (BattleAchieve.taskAchieveList.ContainsKey(id))
            BattleAchieve.taskAchieveList[id] = BattleAchieve.taskAchieveList[id] + value;
        else
            BattleAchieve.taskAchieveList.Add(id, value);
    }

    public static void SetBattleTask(int id, int value)
    {
        if (BattleAchieve.taskAchieveList.ContainsKey(id))
            BattleAchieve.taskAchieveList[id] = value;
        else
            BattleAchieve.taskAchieveList.Add(id, value);
    }

    public static void AddDailyTask(int id, int value)
    {
        if (BattleAchieve.dailyAchieveList.ContainsKey(id))
            BattleAchieve.dailyAchieveList[id] = BattleAchieve.dailyAchieveList[id] + value;
        else
            BattleAchieve.dailyAchieveList[id] = value;
    }

    public static void SetDailyTask(int id, int value)
    {
        if (BattleAchieve.dailyAchieveList.ContainsKey(id))
            BattleAchieve.dailyAchieveList[id] = value;
        else
            BattleAchieve.dailyAchieveList.Add(id, value);
    }
    
    public static void AddBattleAchieveNumber(int id, int value)
    {
        if (BattleAchieve.battleAchieveList.ContainsKey(id)) 
            BattleAchieve.battleAchieveList[id] = BattleAchieve.battleAchieveList[id] + value;
        else
            BattleAchieve.battleAchieveList.Add(id, value);

        int out_num = self.get_achieve_num(id);
        var data = Config.get_t_achievement(id);

        out_num = out_num + BattleAchieve.battleAchieveList[id];
        if (out_num >= data.target_num)
        {
            if (BattleAchieve.IsFirstComplete(data.id))
            {
                string s = AchieveToString(data);
                LuaInterface.LuaTable tt = AppFacade._instance.LuaManager.GetLuaTable(s, "client_achieve");
                Util.CallMethod("AchieveAnimation", "AddAnimation", tt);
                BattleAchieve.completeAchieve.Add(data.id);
            }
        }
    }

    public static string AchieveToString(t_achievement t_achieve)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("client_achieve = {};");
        sb.Append("client_achieve.id = " + t_achieve.id+";");
        sb.Append("client_achieve.name = '" + t_achieve.name + "';");
        sb.Append("client_achieve.desc = '" + t_achieve.desc + "';");
        sb.Append("client_achieve.dtype = " + t_achieve.dtype + ";");
        sb.Append("client_achieve.target_num = " + t_achieve.target_num + ";");
        sb.Append("client_achieve.type_id = " + t_achieve.type_id + ";");
        sb.Append("client_achieve.param1 = " + t_achieve.param1 + ";");
        sb.Append("client_achieve.param2 = " + t_achieve.param2 + ";");
        sb.Append("client_achieve.param3 = " + t_achieve.param3 + ";");
        sb.Append("client_achieve.param4 = " + t_achieve.param4 + ";");
        sb.Append("client_achieve.point = " + t_achieve.point + ";");
        sb.Append("client_achieve.max_star = " + t_achieve.max_star + ";");
        sb.Append("client_achieve.star = " + t_achieve.star + ";");
        sb.Append("client_achieve.icon = '" + t_achieve.icon + "';");
        return sb.ToString();
    }

    public static void SetBattleAchieve(int id,int value)
    {
        if (BattleAchieve.battleAchieveList.ContainsKey(id))
            BattleAchieve.battleAchieveList[id] = value;
        else
            BattleAchieve.battleAchieveList.Add(id, value);

        int out_num = self.get_achieve_num(id);
        var data = Config.get_t_achievement(id);

        out_num = out_num + BattleAchieve.battleAchieveList[id];
        if (out_num >= data.target_num)
        {
            if (BattleAchieve.IsFirstComplete(data.id))
            {
                string s = AchieveToString(data);
                LuaInterface.LuaTable tt = AppFacade._instance.LuaManager.GetLuaTable(s, "client_achieve");
                Util.CallMethod("AchieveAnimation", "AddAnimation", tt);
                BattleAchieve.completeAchieve.Add(data.id);
            }
        }
    }

    public static bool IsComplete(int achieve_id)
    {
        for (int i = 0; i < self.player.achieve_reward.Count; i++)
        {
            if (self.player.achieve_reward[i] == achieve_id)
                return true;
        }
        return false;
    }

    public static void OnlyBattleSkillRelationForMan(int skillID, Dictionary<string, int> effect_hums)
    {
        if (!Battle.is_online)
            return;
        int dies = 0, totals = 0;
        foreach (var item in effect_hums)
        {
            if (item.Value == 0)
                dies = dies + 1;
            totals = totals + 1;
        }

        for (int i = 0; i < Config.t_achieve_type_ids[11].Count; i++)
        {
            var list = Config.t_achieve_type_ids[11][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    if (data.param1 == skillID && data.param2 == 1)
                    {
                        if (data.param3 == 1 && dies >= data.param4)
                            BattleAchieve.SetBattleAchieve(data.id, 1);
                        else if (data.param3 != 1 && totals >= data.param4)
                            BattleAchieve.SetBattleAchieve(data.id, 1);
                    }
                    break;
                }
            }
        }

        for (int i = 0; i < Config.t_achieve_type_ids[32].Count; i++)
        {
            var list = Config.t_achieve_type_ids[32][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    if (dies >= data.param1 && BattlePlayers.me.is_die)
                        BattleAchieve.AddBattleAchieveNumber(data.id, 1);
                    break;
                }
            }
        }
    }

    public static void OnlyBattleSkillRelationForSkill(int skillID, int otherSkillID, int skillCount)
    {
        if (!Battle.is_online)
            return;

        for (int i = 0; i < Config.t_achieve_type_ids[11].Count; i++)
        {
            var list = Config.t_achieve_type_ids[11][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    if ((data.param1 == skillID) && (data.param2 == 2) && data.param3 == otherSkillID)
                    {
                        if (skillCount >= data.param4)
                            BattleAchieve.SetBattleAchieve(data.id, 1);
                    }
                    break;
                }
            }
        }

        for (int i = 0; i < Config.t_achieve_type_ids[14].Count; i++)
        {
            var list = Config.t_achieve_type_ids[14][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    if ((data.param1 == skillID) && (data.param2 == 2) && data.param3 == otherSkillID)
                        BattleAchieve.AddBattleAchieveNumber(data.id, skillCount);
                    break;
                }
            }
        }
    }

    public static void OnlyBattleKillRelation(BattleAnimalPlayer re_bp, BattleAnimalPlayer bp)
    {
        if (!Battle.is_online)
            return;
        
        for (int i = 0; i < Config.t_achieve_type_ids[5].Count; i++)
        {
            var list = Config.t_achieve_type_ids[5][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    BattleAchieve.AddBattleAchieveNumber(data.id, 1);
                    break;
                }
            }
        }

        for (int i = 0; i < Config.t_task_types[5].Count; i++)
        {
            var data = Config.t_task_types[5][i];
            BattleAchieve.AddBattleTaskNumber(data.id, 1);
        }

        for (int i = 0; i < Config.t_daily_types[5].Count; i++)
        {
            var data = Config.t_daily_types[5][i];
            BattleAchieve.AddDailyTask(data.id, 1);
        }

        for (int i = 0; i < Config.t_achieve_type_ids[7].Count; i++)
        {
            var list = Config.t_achieve_type_ids[7][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    if (re_bp.player.lsha >= data.param1)
                        BattleAchieve.SetBattleAchieve(data.id, 1);
                    break;
                }
            }
        }

        for (int i = 0; i < Config.t_task_types[7].Count; i++)
        {
            var data = Config.t_task_types[7][i];
            if (re_bp.player.lsha >= data.param1)
                BattleAchieve.SetBattleTask(data.id, 1);
        }

        for (int i = 0; i < Config.t_task_types[39].Count; i++)
        {
            var data = Config.t_task_types[39][i];
            if (bp.player.sex >= data.param1)
                BattleAchieve.AddBattleTaskNumber(data.id, 1);
        }

        for (int i = 0; i < Config.t_achieve_type_ids[20].Count; i++)
        {
            var list = Config.t_achieve_type_ids[20][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    int sex = data.param2;
                    int role = data.param1;
                    if (re_bp.player.role_id == role && bp.player.sex == sex)
                        BattleAchieve.AddBattleAchieveNumber(data.id, 1);
                    break;
                }
            }
        }

        for (int i = 0; i < Config.t_achieve_type_ids[22].Count; i++)
        {
            var list = Config.t_achieve_type_ids[22][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    var id = data.id;
                    if (bp.player.is_jf)
                        BattleAchieve.AddBattleAchieveNumber(id, 1);
                    break; 
                }
            }
        }

        for (int i = 0; i < Config.t_achieve_type_ids[19].Count; i++)
        {
            var list = Config.t_achieve_type_ids[19][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    var firstRole = data.param1;
                    var secRole = data.param2;
                    if (re_bp.player.role_id == firstRole && bp.player.role_id == secRole)
                        BattleAchieve.AddBattleAchieveNumber(data.id, 1);
                    break;
                }
            }
        }

    }

    public static void KillArmyInLevelLimit(BattleAnimalPlayer re_bp, BattleAnimalPlayer bp)
    {
        if (!Battle.is_online)
            return;
    }

    public static void OnlyBattleFinish()
    {
        if (!Battle.is_online || BattlePlayers.me == null)
            return;

        var bp = BattlePlayers.me;
        for (int i = 0; i < Config.t_achieve_type_ids[6].Count; i++)
        {
            var list = Config.t_achieve_type_ids[6][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    if (bp.player.sha >= data.param1)
                        BattleAchieve.SetBattleAchieve(data.id, 1);
                    break;
                }
            }
        }

        for (int i = 0; i < Config.t_task_types[6].Count; i++)
        {
            var data = Config.t_task_types[6][i];
            if (bp.player.sha >= data.param1)
                BattleAchieve.SetBattleTask(data.id, 1);
        }

        for (int i = 0; i < Config.t_daily_types[6].Count; i++)
        {
            var data = Config.t_daily_types[6][i];
            if (bp.player.sha >= data.param1)
                BattleAchieve.SetDailyTask(data.id, 1);
        }

        for (int i = 0; i < Config.t_achieve_type_ids[8].Count; i++)
        {
            var list = Config.t_achieve_type_ids[8][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    if (Battle.is_online && BattlePlayers.battle_type == 0)
                    {
                        if (bp.player.die <= 0)
                            BattleAchieve.SetBattleAchieve(data.id, 1);
                    }
                    break;
                }
            }
        }

        for (int i = 0; i < Config.t_achieve_type_ids[9].Count; i++)
        {
            var list = Config.t_achieve_type_ids[9][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    var rank = BattleAchieve.GetFinalRank(BattlePlayers.me.player.guid);
                    if (Battle.is_online && BattlePlayers.battle_type == 0)
                    {
                        if (rank == 1)
                            BattleAchieve.AddBattleAchieveNumber(data.id, 1);
                    }
                    break;
                }
            }
        }

        for (int i = 0; i < Config.t_task_types[9].Count; i++)
        {
            var rank = BattleAchieve.GetFinalRank(BattlePlayers.me.player.guid);
            var data = Config.t_task_types[9][i];
            if (Battle.is_online && BattlePlayers.battle_type == 0)
            {
                if (rank == 1)
                    BattleAchieve.AddBattleTaskNumber(data.id, 1);
            }
        }

        for (int i = 0; i < Config.t_daily_types[9].Count; i++)
        {
            var rank = BattleAchieve.GetFinalRank(BattlePlayers.me.player.guid);
            var data = Config.t_daily_types[9][i];
            if (Battle.is_online && BattlePlayers.battle_type == 0)
            {
                if (rank == 1)
                    BattleAchieve.AddDailyTask(data.id, 1);
            }
        }

        for (int i = 0; i < Config.t_achieve_type_ids[12].Count; i++)
        {
            var list = Config.t_achieve_type_ids[12][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    if (!bp.achieveRecords.IsUseSkill)
                        BattleAchieve.SetBattleAchieve(data.id, 1);
                }
                break;
            }
        }

        for (int i = 0; i < Config.t_achieve_type_ids[13].Count; i++)
        {
            var list = Config.t_achieve_type_ids[13][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    if (!bp.achieveRecords.IsUseNormalToKill && bp.achieveRecords.IsUseSkillToKill)
                        BattleAchieve.SetBattleAchieve(data.id, 1);
                    break;
                }
            }
        }

        for (int i = 0; i < Config.t_achieve_type_ids[17].Count; i++)
        {
            var list = Config.t_achieve_type_ids[17][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    var sex = data.param1;
                    if (sex == 0 && bp.achieveRecords.killMale >= data.param2)
                        BattleAchieve.SetBattleAchieve(data.id, 1);

                    if (sex == 1 && data.param2 <= bp.achieveRecords.killFemale)
                        BattleAchieve.SetBattleAchieve(data.id, 1);
                    break;
                }
            }
        }

        for (int i = 0; i < Config.t_achieve_type_ids[18].Count; i++)
        {
            var list = Config.t_achieve_type_ids[18][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    var sex = data.param1;
                    if (sex == 0 && bp.achieveRecords.killFemale == 0 && bp.achieveRecords.killMale >= data.param2)
                    {
                        BattleAchieve.SetBattleAchieve(data.id, 1);
                        break;
                    }

                    if (sex == 1 && bp.achieveRecords.killMale == 0 && bp.achieveRecords.killFemale >= data.param2)
                    {
                        BattleAchieve.SetBattleAchieve(data.id, 1);
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < Config.t_achieve_type_ids[21].Count; i++)
        {
            var list = Config.t_achieve_type_ids[21][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    if (bp.achieveRecords.kills != null)
                    {
                        int maxKills = 0;
                        foreach (var item in bp.achieveRecords.kills)
                        {
                            if (item.Value > maxKills)
                                maxKills = item.Value;
                        }
                        if (data.param1 <= maxKills)
                            BattleAchieve.SetBattleAchieve(data.id, 1);
                    }
                    break;
                }
            }
        }

        for (int i = 0; i < Config.t_achieve_type_ids[25].Count; i++)
        {
            var list = Config.t_achieve_type_ids[25][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    if (bp.achieveRecords.blood_ten_pert >= data.param2)
                        BattleAchieve.SetBattleAchieve(data.id, 1);
                    break;
                }
            }
        }

        for (int i = 0; i < Config.t_achieve_type_ids[27].Count; i++)
        {
            var list = Config.t_achieve_type_ids[27][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    if (bp.achieveRecords.kills != null)
                    {
                        var kills = 0;
                        foreach (var item in bp.achieveRecords.kills)
                            kills = kills + 1;
                        if (data.param1 <= kills)
                            BattleAchieve.SetBattleAchieve(data.id, 1);
                    }
                    break;
                }
            }
        }

        for (int i = 0; i < Config.t_achieve_type_ids[28].Count; i++)
        {
            var list = Config.t_achieve_type_ids[28][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    if (Battle.is_online && BattlePlayers.battle_type == 0)
                    {
                        if (bp.achieveRecords.killHeader >= data.param2)
                            BattleAchieve.SetBattleAchieve(data.id, 1);
                    }
                    break;
                }
            }
        }

        for (int i = 0; i < Config.t_achieve_type_ids[29].Count; i++)
        {
            var list = Config.t_achieve_type_ids[29][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    if (Battle.is_online && BattlePlayers.battle_type == 0)
                    {
                        if (bp.achieveRecords.killMale <= 0)
                        {
                            var rank = BattleAchieve.GetFinalRank(BattlePlayers.me.player.guid);
                            if (rank == 1)
                                BattleAchieve.SetBattleAchieve(data.id, 1);
                        }
                    }
                    break;
                }
            }
        }

        for (int i = 0; i < Config.t_achieve_type_ids[33].Count; i++)
        {
            var list = Config.t_achieve_type_ids[33][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    if (Battle.is_online && BattlePlayers.battle_type == 0)
                    {
                        if (!bp.achieveRecords.useXueren)
                        {
                            var rank = BattleAchieve.GetFinalRank(self.guid);
                            if (rank == 1)
                                BattleAchieve.SetBattleAchieve(data.id, 1);
                        }
                    }
                    break;
                }
            }
        }

        for (int i = 0; i < Config.t_achieve_type_ids[34].Count; i++)
        {
            var list = Config.t_achieve_type_ids[34][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    if (bp.achieveRecords.killCaos > data.param1)
                        BattleAchieve.SetBattleAchieve(data.id, 1);
                    break;
                }
            }
        }

        for (int i = 0; i < Config.t_achieve_type_ids[26].Count; i++)
        {
            var list = Config.t_achieve_type_ids[26][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    if (bp.achieveRecords.quietKills >= data.param2)
                        BattleAchieve.SetBattleAchieve(data.id, 1);
                    break;
                }
            }
        }

        for (int i = 0; i < Config.t_task_types[38].Count; i++)
        {
            var data = Config.t_task_types[38][i];

            if (Battle.is_online && BattlePlayers.battle_type == 0)
            {
                if (bp.player.score >= data.param1)
                    BattleAchieve.SetBattleTask(data.id, 1);
            }
        }

        for (int i = 0; i < Config.t_task_types[138].Count; i++)
        {
            var data = Config.t_task_types[138][i];
            if(Battle.is_online && BattlePlayers.battle_type == 1)
            {
                var score = bp.player.score;
                if (score >= data.param1)
                    BattleAchieve.SetBattleTask(data.id, 1);
            } 
        }
        BattleAchieve.CompleteCup();
    }

    public static void BattleChangeSkill()
    {
        if (!Battle.is_online)
            return;

        var bp = BattlePlayers.me;
        if (bp == null)
            return;

        for (int i = 0; i < Config.t_achieve_type_ids[31].Count; i++)
        {
            var list = Config.t_achieve_type_ids[31][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    if (bp.achieveRecords.changeSkill != null)
                        if (data.param1 <= bp.achieveRecords.changeSkill.Count)
                            BattleAchieve.SetBattleAchieve(data.id, 1);
                    break;
                }
            }
        }
    }

    public static void BattleExpChange()
    {
        if (!Battle.is_online)
            return;
    }

    public static void BattleBuffer(int buffer_id)
    {
        if (!Battle.is_online)
            return;

        if (BattlePlayers.me.achieveRecords.bufferKill == null)
            return;

        for (int i = 0; i < Config.t_achieve_type_ids[23].Count; i++)
        {
            var list = Config.t_achieve_type_ids[23][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    var state_id = data.param1;
                    var attrList = Config.get_t_battle_buff(buffer_id).attr;
                    for (int m = 0; m < attrList.Count; m++)
                    {
                        int effect_id = attrList[m].param1;
                        if (effect_id == state_id)
                        {
                            if (BattlePlayers.me.achieveRecords.bufferKill.ContainsKey(buffer_id))
                            {
                                if (BattlePlayers.me.achieveRecords.bufferKill[buffer_id] >= data.param2)
                                    BattleAchieve.AddBattleAchieveNumber(data.id, 1);
                            }
                        }
                    }
                    break;
                }
            }
        }
    }

    public static void AttackXueRen()
    {
        if (!Battle.is_online)
            return;

        for (int i = 0; i < Config.t_achieve_type_ids[16].Count; i++)
        {
            var list = Config.t_achieve_type_ids[16][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    BattleAchieve.AddBattleAchieveNumber(data.id, 1);
                    break;
                }
            }
        }
    }
    public static void BattleRoleLevelCheck(BattleAnimalPlayer bp)
    {
        for (int i = 0; i < Config.t_task_types[10].Count; i++)
        {
            var data = Config.t_task_types[10][i];

            if (Battle.is_online)
            {
                if (bp.player.level >= data.param1)
                    BattleAchieve.SetBattleTask(data.id, 1);
            }
        }
    }

    public static void CompleteCup()
    {
        if (!Battle.is_online)
            return;
        for (int i = 0; i < Config.t_achieve_type_ids[35].Count; i++)
        {
            var list = Config.t_achieve_type_ids[35][i];
            for (int j = 0; j < list.Count; j++)
            {
                var data = list[j];
                if (!BattleAchieve.IsComplete(data.id))
                {
                    var out_num = self.get_achieve_num(data.id);
                    if (out_num >= data.target_num)
                    {
                        Util.CallMethod("AchieveAnimation", "AddHallAchieve", data.id, true);
                        BattleAchieve.completeAchieve.Add(data.id);
                    }
                }
            }
        }
    }

    public static int GetFinalRank(string ids)
    {
        ulong id = Convert.ToUInt64(ids);
        List<BattleAnimalPlayer> players = new List<BattleAnimalPlayer>();
        for (int ll = 0; ll < BattlePlayers.players_list.Count; ll++)
            players.Add(BattlePlayers.players_list[ll]);

        players.Sort((a, b) =>
        {
            return b.player.score - a.player.score;
        });

        int rank = 30;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].player.guid == id.ToString())
            {
                rank = i + 1;
                break;
            }
        }
        return rank;
    }

    public static void BattleFinish()
    {
        BattleAchieve.OnlyBattleFinish();
        CMSG_BATTLE_ACHIEVE();
        CMSG_BATTLE_TASK();
        CMSG_BATTLE_DAILY();
        ArrayList arr = BattleAchieve.PushResultToLua();
        Util.CallMethod("LuaAchieve", "LoadFromClient", arr);
    }

    public static void CMSG_BATTLE_ACHIEVE()
    {
        var msg =  new cmsg_battle_achieve();
        foreach (var item in BattleAchieve.battleAchieveList)
        {
            var t_achievement = Config.get_t_achievement(item.Key);
            if (t_achievement.dtype == 2)
            {
                msg.id.Add(item.Key);
                msg.num.Add(item.Value);
            }
        }

        GameTcp.Send<cmsg_battle_achieve>(opclient_t.CMSG_BATTLE_ACHIEVE, msg);
    }

    public static void CMSG_BATTLE_TASK()
    {
        var msg = new cmsg_battle_task();
        foreach (var item in BattleAchieve.taskAchieveList)
        {
            var t_task = Config.get_t_task(item.Key);
            if (t_task.dtype == 2)
            {
                msg.id.Add(item.Key);
                msg.num.Add(item.Value);
            }
        }

        GameTcp.Send<cmsg_battle_task>(opclient_t.CMSG_BATTLE_TASK, msg);
    }

    public static void CMSG_BATTLE_DAILY()
    {
        var msg = new cmsg_battle_daily();
        int count = 0;
        foreach (var item in BattleAchieve.dailyAchieveList)
            count += 1;

        foreach (var item in BattleAchieve.dailyAchieveList)
        {
            var t_daily = Config.get_t_daily(item.Key);
            if (t_daily.dtype == 2)
            {
                msg.id.Add(item.Key);
                msg.num.Add(item.Value);
            }
        }
        GameTcp.Send<cmsg_battle_daily>(opclient_t.CMSG_BATTLE_DAILY, msg);
    }
    /////////////////////////////////////////////////////////////////////////////////////////
    public static void SMSG_BATTLE_ACHIEVE(s_net_message msg)
    {
        foreach (var item in BattleAchieve.battleAchieveList)
        {
            self.add_achieve_num(item.Key,item.Value, true);
            Util.CallMethod("PlayerData", "add_achieve_num", item.Key, item.Value, true);
        }

        //判断是否有成就达成
        for (int i = 0; i < self.player.achieve_id.Count; i++)
        {
            var data = Config.get_t_achievement(self.player.achieve_id[i]);
            if (data != null)
            {
                if (data.dtype == 2 && self.player.achieve_num[i] >= data.target_num)
                    Util.CallMethod("AchieveAnimation", "AddHallAchieve", data.id);
            }
        }
    }

    public static void SMSG_BATTLE_TASK(s_net_message msg)
    {
        foreach (var item in taskAchieveList)
        {
            self.add_task_num(item.Key,item.Value, true);
            Util.CallMethod("PlayerData", "add_task_num", item.Key, item.Value, true);
        }

        bool sign = false;
        for (int i = 0; i < self.player.task_id.Count; i++)
        {
            var data = Config.get_t_task(self.player.task_id[i]);
            if (data.dtype == 2 && self.player.task_num[i] >= data.target_num)
                sign = true;
        }

        if (sign)
        {
            //BattleAchieve.SendTaskReach()
        }
    }

    public static void SMSG_BATTLE_DAILY(s_net_message msg)
    {
        foreach (var item in dailyAchieveList)
        {
            self.add_daily_num(item.Key, item.Value, true);
            Util.CallMethod("PlayerData", "add_daily_num", item.Key, item.Value, true);
        }


        bool sign = false;
        for (int i = 0; i < self.player.daily_id.Count; i++)
        {
            var data = Config.get_t_daily(self.player.daily_id[i]);
            if (data.dtype == 2 && self.player.daily_num[i] >= data.target_num)
                sign = true;
        }

        if (sign)
        {
            //BattleAchieve.SendDailyTaskReach()
        }
    }
}
