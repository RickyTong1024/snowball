using dhc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class self
{
    public static Dictionary<int, string> font_color = new Dictionary<int, string>() { { 1, "[3defff]" }, { 2, "[ff41da]" }, { 3, "[ff41da]" }, { 4, "[ffcd00]" }, { 5, "[f01c1c]" }, { 6, "[c2e5ed]" }, { 7, "[57FC5B]" } };
    public static string guid;
    public static string battle_code;
    public static player_t player;
    public static List<role_t> roles;
    public static int pre_battle_num;
    public static int quality;
    public static int eff_quatity;
    public static string share_url;

    private static List<int> out_attr = new List<int>();
    public static role_t get_role(ulong guid)
    {
        for (int i = 0; i < self.roles.Count; i++)
        {
            if (self.roles[i].guid == guid)
                return self.roles[i];
        }
        return null;
    }

    public static int get_name_color()
    {
        if (Convert.ToUInt64(self.player.nian_time) > Convert.ToUInt64(LuaHelper.GetTimerManager().now_string()))
            return 2;
        else if (Convert.ToUInt64(self.player.yue_time) > Convert.ToUInt64(LuaHelper.GetTimerManager().now_string()))
            return 1;
        return 0;
    }
    public static role_t get_role_id(int id)
    {
        for (int i = 0; i < self.roles.Count; i++)
        {
            if (self.roles[i].template_id == id)
                return self.roles[i];
        }
        return null;
    }
    private static void calc_out_attr()
    {
        out_attr.Clear();
        for (int i = 0; i <= 100; i++)
            out_attr.Add(0);

        for (int i = 0; i < self.roles.Count; i++)
        {
            var role = self.roles[i];
            var t_role = Config.get_t_role(role.template_id);
            for (int j = 0; j < t_role.gskills.Count; j++)
            {
                var t_role_buff = Config.get_t_role_buff(t_role.gskills[j]);
                if (t_role_buff.type == 3)
                    out_attr[t_role_buff.param1] = out_attr[t_role_buff.param1] + t_role_buff.param_value(role.level);
            }
        }

        var t_exp = Config.get_t_exp(self.player.level);
        for (int i = 0; i < t_exp.level_add.Count; i++)
        {
            if (t_exp.level_add[i].type == 3)
                out_attr[t_exp.level_add[i].param1] = out_attr[t_exp.level_add[i].param1] + t_exp.level_add[i].param3;
        }

        for (int i = 0; i < self.player.toukuang.Count; i++)
        {
            var id = self.player.toukuang[i];
            var t_toukuang = Config.get_t_toukuang(id);
            for (int j = 0; j < t_toukuang.gskills.Count; j++)
            {
                if (t_toukuang.gskills[j].type == 3)
                    out_attr[t_toukuang.gskills[j].param1] = out_attr[t_toukuang.gskills[j].param1] + t_toukuang.gskills[j].param3;
            }
        }
    }

    public static int get_out_attr(int id)
    {
        if (out_attr.Count <= 0)
            calc_out_attr();

        var a = out_attr[id];
        long now_t = Convert.ToInt64(LuaHelper.GetTimerManager().now_string());
        long yue_t = Convert.ToInt64(self.player.yue_time);
        long nian_t = Convert.ToInt64(self.player.nian_time);
        if (now_t < yue_t)
        {
            var t_vip_attr = Config.get_t_vip_attr(1);
            for (int i = 0; i < t_vip_attr.attrs.Count; i++)
            {
                if (t_vip_attr.attrs[i].param1 == id)
                    a = a + t_vip_attr.attrs[i].param3;
            }
        }
        else if (now_t < nian_t)
        {
            var t_vip_attr = Config.get_t_vip_attr(2);
            for (int i = 0; i < t_vip_attr.attrs.Count; i++)
            {
                if (t_vip_attr.attrs[i].param1 == id)
                    a = a + t_vip_attr.attrs[i].param3;
            }
        }
        return a;
    }
    public static int GetVipNameColor()
    {
        int color = 0;
        long now_t = Convert.ToInt64(LuaHelper.GetTimerManager().now_string());
        long yue_t = Convert.ToInt64(self.player.yue_time);
        long nian_t = Convert.ToInt64(self.player.nian_time);
        if (yue_t > now_t)
            color = 1;

        if (nian_t > now_t)
            color = 2;
        return color;
    }

    public static void save_set()
    {
        PlayerPrefs.SetString("quality", self.quality.ToString());
        PlayerPrefs.SetString("eff_quality", self.eff_quatity.ToString());
        PlayerPrefs.Save();
        Util.CallMethod("PlayerData", "load_set");
    }

    public static int get_achieve_num(int id)
    {
        var t_achievement = Config.get_t_achievement(id);
        if (t_achievement == null)
            return 0;

        if (t_achievement.type_id == 1)
        {
            int num = 0;
            for (int i = 0; i < self.roles.Count; i++)
            {
                var flag = true;
                var role = self.roles[i];
                var t_role = Config.get_t_role(role.template_id);
                if (t_achievement.param1 != 2 && t_achievement.param1 != t_role.sex)
                    flag = false;
                if (t_achievement.param2 != 0 && t_achievement.param2 != t_role.color)
                    flag = false;
                if (flag)
                    num = num + 1;
            }
            return num;
        }
        else if (t_achievement.type_id == 2)
            return self.player.level;
        else if (t_achievement.type_id == 3)
        {
            int num = 0;
            for (int i = 0; i < self.roles.Count; i++)
            {
                var role = self.roles[i];
                if (role.template_id == t_achievement.param1)
                    return role.level;
            }
            return num;
        }
        else if (t_achievement.type_id == 35)
        {
            if (self.player.max_cup >= t_achievement.param1)
                return 1;
            else
                return 0;
        }
        else
        {
            for (int i = 0; i < self.player.achieve_id.Count; i++)
            {
                if (self.player.achieve_id[i] == id)
                    return self.player.achieve_num[i];
            }
        }
        return 0;
    }
    
    public static void add_achieve_num(int id, int num, bool check = false)
    {
        var t_achievement = Config.get_t_achievement(id);
        if (t_achievement == null)
            return;

        if (check && t_achievement.dtype != 2)
            return;

        for (int i = 0; i < self.player.achieve_reward.Count; i++)
        {
            if (self.player.achieve_reward[i] == id)
                return;
        }

        for (int i = 0; i < self.player.achieve_id.Count; i++)
        {
            if (self.player.achieve_id[i] == id)
            {
                num = self.player.achieve_num[i] + num;
                if (num > t_achievement.target_num)
                    num = t_achievement.target_num;
                self.player.achieve_num[i] = num;
                return;
            }
        }

        if (num > t_achievement.target_num)
            num = t_achievement.target_num;
        self.player.achieve_id.Add(id);
        self.player.achieve_num.Add(num);
    }

    public static void add_task_num(int id, int num, bool check = false)
    {
        var t_task = Config.get_t_task(id);
        if (t_task == null)
            return;

        if (check && t_task.dtype != 2)
            return;

        if (self.player.level < t_task.level)
            return;

        for (int i = 0; i < self.player.task_reward.Count; i++)
        {
            if (self.player.task_reward[i] == id)
                return;
        }

        for (int i = 0; i < self.player.task_id.Count; i++)
        {
            if (self.player.task_id[i] == id)
            {
                num = self.player.task_num[i] + num;
                if (num > t_task.target_num)
                    num = t_task.target_num;
                self.player.task_num[i] = num;
                return;
            }
        }

        if (num > t_task.target_num)
            num = t_task.target_num;

        self.player.task_id.Add(id);
        self.player.task_num.Add(num);
    }

    public static void add_daily_num(int id, int num, bool check = false)
    {
        var t_daily = Config.get_t_daily(id);
        if (t_daily == null)
            return;

        if (check && t_daily.dtype != 2)
            return;

        for (int i = 0; i < self.player.daily_reward.Count; i++)
        {
            if (self.player.daily_reward[i] == id)
                return;
        }

        for (int i = 0; i < self.player.daily_id.Count; i++)
        {
            if (self.player.daily_id[i] == id)
            {
                num = self.player.daily_num[i] + num;
                if (num > t_daily.target_num)
                    num = t_daily.target_num;
                self.player.daily_num[i] = num;
                return;
            }
        }

        if (num > t_daily.target_num)
            num = t_daily.target_num;

        self.player.daily_id.Add(id);
        self.player.daily_num.Add(num);
    }
}
