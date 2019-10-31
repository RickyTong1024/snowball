using BattleDB;
using System;
using System.Collections.Generic;

public class Config
{
    public static Dictionary<int,t_role> t_roles = null;
    public static List<int> t_role_blue_ids = null;
    public static void parse_t_role()
    {
        var dbc_role = new dbc();
        dbc_role.load_txt("t_role");
        t_roles = new Dictionary<int, t_role>();
        t_role_blue_ids = new List<int>();
        for (int i = 0; i < dbc_role.get_y(); i++)
        {
            t_role tr = new t_role();
            tr.id = dbc_role.get_int(0, i);
            tr.name = dbc_role.get_string(2, i);
            tr.name = Config.get_t_lang(tr.name);
            tr.color = dbc_role.get_int(3, i);
            tr.sex = dbc_role.get_int(4, i);
            tr.icon = dbc_role.get_string(5, i);
            tr.res = dbc_role.get_string(6, i);
            tr.desc = dbc_role.get_string(7, i);
            tr.desc = Config.get_t_lang(tr.desc);
            tr.yy = new string[4];
            for (int j = 0; j < 4; j++)
                tr.yy[j] = dbc_role.get_string(8 + j, i);

            tr.hp = dbc_role.get_int(12, i);
            tr.hp_add = dbc_role.get_int(13, i);
            tr.atk = dbc_role.get_int(14, i);
            tr.atk_add = dbc_role.get_int(15, i);
            tr.def = dbc_role.get_int(16, i);
            tr.def_add = dbc_role.get_int(17, i);
            tr.range = dbc_role.get_int(18, i);
            tr.aspeed = dbc_role.get_int(19, i);
            tr.speed = dbc_role.get_int(20, i);

            tr.gskills = new List<int>();
            for (int k = 0; k <= 2; k++)
            {
                int bs = dbc_role.get_int(21 + k, i);
                if (bs > 0)
                    tr.gskills.Add(bs);
            }

            tr.bskills = new List<int>();
            for (int k = 0; k <= 2; k++)
            {
                int bs = dbc_role.get_int(24 + k, i);
                if (bs > 0)
                    tr.bskills.Add(bs);
            }
            tr.suipian_id = dbc_role.get_int(27, i);
            tr.suipian_cost = dbc_role.get_int(28, i);
            tr.type = dbc_role.get_int(29, i);
            tr.get_type = dbc_role.get_int(30, i);
            tr.pf_x = dbc_role.get_int(31, i);
            tr.pf_y = dbc_role.get_int(32, i);
            tr.pf_z = dbc_role.get_int(33, i);

            Config.t_roles.Add(tr.id, tr);
            if (tr.color == 1)
                t_role_blue_ids.Add(tr.id);
        }
    }

    public static t_role get_t_role(int id)
    {
        if (Config.t_roles.ContainsKey(id))
            return t_roles[id];
        else
            return null;
    }
    /////////////////////////////////////////////////////////
    public static Dictionary<int, t_role_buff> t_role_buffs = null;
    public static void parse_t_role_buff()
    {
        var dbc_role_buff = new dbc();
        dbc_role_buff.load_txt("t_role_buff");
        t_role_buffs = new Dictionary<int, t_role_buff>();
        for (int i = 0; i < dbc_role_buff.get_y(); i++)
        {
            var tf = new t_role_buff();
            tf.id = dbc_role_buff.get_int(0, i);
            tf.name = dbc_role_buff.get_string(2, i);
            tf.name = Config.get_t_lang(tf.name);
            tf.desc = dbc_role_buff.get_string(4, i);
            tf.desc = Config.get_t_lang(tf.desc);
            tf.type = dbc_role_buff.get_int(5, i);
            tf.param1 = dbc_role_buff.get_int(6, i);
            tf.param2 = dbc_role_buff.get_int(7, i);
            tf.param3 = dbc_role_buff.get_int(8, i);
            tf.param4 = dbc_role_buff.get_int(9, i);
            Config.t_role_buffs.Add(tf.id, tf);
        }
    }
    public static t_role_buff get_t_role_buff(int id)
    {
        if (Config.t_role_buffs.ContainsKey(id))
            return Config.t_role_buffs[id];
        else
            return null;
    }
    /////////////////////////////////////////////////////////
    public static Dictionary<string, t_lang> t_langs = null;
    public static void parse_t_lang()
    {
        var dbc_lang = new dbc();
        dbc_lang.load_txt("t_lang");
        t_langs = new Dictionary<string, t_lang>();
        for (int i = 0; i < dbc_lang.get_y(); i++)
        {
            t_lang tl = new t_lang();
            tl.id = dbc_lang.get_string(0, i);
            tl.lang = new string[dbc_lang.get_x() - 1];
            for (int j = 1; j < dbc_lang.get_x(); j++)
                tl.lang[j - 1] = dbc_lang.get_string(j,i).Replace("##", "\n");
            if (!t_langs.ContainsKey(tl.id))
                t_langs.Add(tl.id, tl);
        }
    }

    public static string get_t_lang(string id)
    {
        if (Config.t_langs.ContainsKey(id))
        {
            if (t_langs[id].lang.Length > platform_config_common.languageType)
            {
                return t_langs[id].lang[platform_config_common.languageType];
            }
            else
            {
                return "";
            }
        }
        return "";
    }

    //////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_boss_attr> t_boss_attrs = null;
    public static List<t_boss_attr> t_boss_list = null;
    public static void parse_t_boss_attr()
    {
        var dbc_boss_attr = new dbc();
        dbc_boss_attr.load_txt("t_boss_attr");
        t_boss_attrs = new Dictionary<int, t_boss_attr>();
        t_boss_list = new List<t_boss_attr>();
        for (int i = 0; i < dbc_boss_attr.get_y(); i++)
        {
            var dba = new t_boss_attr();
            dba.id = dbc_boss_attr.get_int(0, i);
            dba.name = dbc_boss_attr.get_string(2, i);
            dba.name = Config.get_t_lang(dba.name);
            dba.role_id = dbc_boss_attr.get_int(3, i);
            dba.refresh_t = dbc_boss_attr.get_int(4, i);
            dba.life_t = dbc_boss_attr.get_int(5, i);
            dba.body_scale = dbc_boss_attr.get_int(6, i);
            dba.birth_x = dbc_boss_attr.get_int(7, i);
            dba.birth_y = dbc_boss_attr.get_int(8, i);
            dba.def = dbc_boss_attr.get_int(9, i);
            dba.skill1 = dbc_boss_attr.get_int(10, i);
            dba.skill2 = dbc_boss_attr.get_int(11, i);
            dba.patk_power = dbc_boss_attr.get_int(12, i);
            dba.xl_power = dbc_boss_attr.get_int(13, i);
            dba.bexp = dbc_boss_attr.get_int(14, i);
            dba.bscore = dbc_boss_attr.get_int(15, i);
            dba.maxb = dbc_boss_attr.get_int(16, i);
            dba.max_dis = dbc_boss_attr.get_int(17, i);
            dba.toukuang_id = dbc_boss_attr.get_int(18, i);
            dba.buff_id = dbc_boss_attr.get_int(19, i);
            dba.wt = dbc_boss_attr.get_int(20, i);
            dba.b_amount = dbc_boss_attr.get_int(21, i);
            dba.b_pro = dbc_boss_attr.get_int(22, i);

            t_boss_attrs.Add(dba.id, dba);
            t_boss_list.Add(dba);
        }
    }

    public static t_boss_attr get_t_boss_attr(int id)
    {
        if (Config.t_boss_attrs.ContainsKey(id))
            return Config.t_boss_attrs[id];
        else
            return null;
    }
    //////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, List<string>> t_names = null;
    public static void parse_t_name()
    {
        var db_name = new dbc();
        db_name.load_txt("t_name");
        t_names = new Dictionary<int, List<string>>();
        List<string> firstName = new List<string>();
        List<string> secName = new List<string>();
        List<string> thirdName = new List<string>();

        for (int i = 0; i < db_name.get_y(); i++)
        {
            string fir = db_name.get_string(0, i);
            string last = db_name.get_string(1, i);
            string engName = db_name.get_string(2, i);
            if (!String.IsNullOrEmpty(fir))
                firstName.Add(fir);
            if (!String.IsNullOrEmpty(last))
                secName.Add(last);
            if (!String.IsNullOrEmpty(engName))
                thirdName.Add(engName);
        }
        t_names.Add(1, firstName);
        t_names.Add(2, secName);
        t_names.Add(3, thirdName);
    }

    public static string get_battle_random_name()
    {
        string fir = Config.t_names[1][BattleOperation.random(0, Config.t_names[1].Count)];
        string last = Config.t_names[2][BattleOperation.random(0, Config.t_names[2].Count)];
        return String.Concat(fir, last);
        //return Config.t_names[3][BattleOperation.random(0, Config.t_names[3].Count)];
    }

    ///////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_toukuang> t_toukuangs = null;
    public static List<int> t_toukuang_ids = null;
    public static void parse_t_toukuang()
    {
        var dbc_toukuang = new dbc();
        dbc_toukuang.load_txt("t_toukuang");

        t_toukuangs = new Dictionary<int, t_toukuang>();
        t_toukuang_ids = new List<int>();

        for (int i = 0; i < dbc_toukuang.get_y(); i++)
        {
            var tk = new t_toukuang();
            tk.id = dbc_toukuang.get_int(0, i);
            tk.name = dbc_toukuang.get_string(2, i);
            tk.name = Config.get_t_lang(tk.name);
            tk.big_icon = dbc_toukuang.get_string(3, i);
            tk.color = dbc_toukuang.get_int(4, i);
            tk.desc1 = dbc_toukuang.get_string(5, i);
            tk.desc1 = Config.get_t_lang(tk.desc1);
            tk.desc2 = dbc_toukuang.get_string(6, i);
            tk.desc2 = Config.get_t_lang(tk.desc2);
            tk.icon = dbc_toukuang.get_string(8, i);
            tk.gskills = new List<fQua>();
            for (int j = 1; j < 4; j++)
            {
                if (dbc_toukuang.get_int(9 + (j - 1) * 4, i) != 0)
                {
                    fQua fq = new fQua();
                    fq.type = dbc_toukuang.get_int(9 + (j - 1) * 4, i);
                    fq.param1 = dbc_toukuang.get_int(10 + (j - 1) * 4, i);
                    fq.param2 = dbc_toukuang.get_int(11 + (j - 1) * 4, i);
                    fq.param3 = dbc_toukuang.get_int(12 + (j - 1) * 4, i);
                    tk.gskills.Add(fq);
                }
            }
            tk.desc = tk.desc1 + "\n" + tk.desc2;
            t_toukuangs.Add(tk.id, tk);
            t_toukuang_ids.Add(tk.id);
        }
    }

    public static t_toukuang get_t_toukuang(int id)
    {
        if (t_toukuangs.ContainsKey(id))
            return t_toukuangs[id];
        else
            return null;
    }

    public static int get_battle_random_toukuang()
    {
        return Config.t_toukuang_ids[BattleOperation.random(0, 3)];
    }

    //////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_fashion> t_fashions = null;
    public static List<int> t_fashion_ids = null;
    public static void parse_t_fashion()
    {
        var dbc_fashion = new dbc();
        dbc_fashion.load_txt("t_fashion");
        t_fashions = new Dictionary<int, t_fashion>();
        t_fashion_ids = new List<int>();

        for (int i = 0; i < dbc_fashion.get_y(); i++)
        {
            var tf = new t_fashion();
            tf.id = dbc_fashion.get_int(0, i);
            tf.name = dbc_fashion.get_string(2, i);
            tf.type = dbc_fashion.get_int(3, i);
            tf.icon = dbc_fashion.get_string(4, i);
            tf.color = dbc_fashion.get_int(5, i);
            tf.name = Config.get_t_lang(tf.name);
            tf.model = dbc_fashion.get_string(6, i);
            tf.hit_effect = dbc_fashion.get_string(7, i);
            tf.down_effect = dbc_fashion.get_string(8, i);
            tf.show = dbc_fashion.get_string(9, i);
            tf.desc1 = dbc_fashion.get_string(11, i);
            tf.desc1 = Config.get_t_lang(tf.desc1);
            tf.desc2 = dbc_fashion.get_string(13, i);
            tf.desc2 = Config.get_t_lang(tf.desc2);
            tf.desc = tf.desc1 + "\n" + tf.desc2;
            tf.buchang = dbc_fashion.get_int(14, i);
            tf.param_type = dbc_fashion.get_int(15, i);
            tf.param1 = dbc_fashion.get_int(16, i);
            tf.param2 = dbc_fashion.get_int(17, i);
            tf.param3 = dbc_fashion.get_int(18, i);
            Config.t_fashions.Add(tf.id, tf);
            Config.t_fashion_ids.Add(tf.id);
        }
    }

    public static t_fashion get_t_fashion(int id)
    {
        if (Config.t_fashions.ContainsKey(id))
            return Config.t_fashions[id];
        else
            return null;
    }
    //////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_bodysize> t_bodysizes = new Dictionary<int, t_bodysize>();
    public static int max_score_level = 0;
    public static void parse_t_bodysize()
    {
        var dbc_body = new dbc();
        dbc_body.load_txt("t_bodysize");
        t_bodysizes = new Dictionary<int, t_bodysize>();
        max_score_level = 0;
        for (int i = 0;i < dbc_body.get_y();i++)
        {
            var t_body = new t_bodysize();
            t_body.id = dbc_body.get_int(0, i);
            t_body.score = dbc_body.get_int(1, i);
            t_body.scale = dbc_body.get_int(2, i);
            t_body.dis = dbc_body.get_int(3, i);
            t_body.snowsize = dbc_body.get_int(4, i);
            t_bodysizes.Add(t_body.id, t_body);
            if (t_body.id > Config.max_score_level)
                Config.max_score_level = t_body.id;
        }
    }

    public static t_bodysize get_t_bodysize(int id)
    {
        if (!Config.t_bodysizes.ContainsKey(id))
            return null;
        else
            return Config.t_bodysizes[id];
    }
    //////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_battle_exp> t_battle_exps = new Dictionary<int, t_battle_exp>();
    public static int max_battle_level = 1;

    public static void parse_t_battle_exp()
    {
        var dbc_battle_exp = new dbc();
        dbc_battle_exp.load_txt("t_battle_exp");
        max_battle_level = 1;
        t_battle_exps = new Dictionary<int, t_battle_exp>();
        for (int i = 0; i < dbc_battle_exp.get_y(); i++)
        {
            var tb = new t_battle_exp();
            tb.level = dbc_battle_exp.get_int(0, i);
            tb.exp = dbc_battle_exp.get_int(1, i);
            tb.gexp = dbc_battle_exp.get_int(2, i);
            tb.bexp = dbc_battle_exp.get_int(3, i);
            tb.bscore = dbc_battle_exp.get_int(4, i);
            tb.max_bl = dbc_battle_exp.get_int(5, i);
            tb.max_dis = dbc_battle_exp.get_int(6, i);
            tb.huifu = dbc_battle_exp.get_int(7, i);
            tb.item_num2 = dbc_battle_exp.get_int(8, i);
            tb.item_rate2 = dbc_battle_exp.get_int(9, i);
            t_battle_exps.Add(tb.level, tb);
            if (tb.level > max_battle_level)
                max_battle_level = tb.level;
        }
    }

    public static t_battle_exp get_t_battle_exp(int level)
    {
        if (Config.t_battle_exps.ContainsKey(level))
            return Config.t_battle_exps[level];
        else
            return null;
    }
    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_battle_item> t_battle_items = null;
    public static List<t_battle_item> t_battle_ability_ball = null;

    public static void parse_t_battle_item()
    {
        var dbc_battle_item = new dbc();
        dbc_battle_item.load_txt("t_battle_item");

        t_battle_items = new Dictionary<int, t_battle_item>();
        t_battle_ability_ball = new List<t_battle_item>();

        for (int i = 0; i < dbc_battle_item.get_y(); i++)
        {
            var bi = new t_battle_item();
            bi.id = dbc_battle_item.get_int(0, i);
            bi.name = dbc_battle_item.get_string(1, i);
            bi.type = dbc_battle_item.get_int(2, i);
            bi.effect = dbc_battle_item.get_string(3, i);
            bi.geffect = dbc_battle_item.get_string(4, i);
            bi.skill = dbc_battle_item.get_int(5, i);
            bi.max = dbc_battle_item.get_int(6, i);
            bi.desc = dbc_battle_item.get_string(8, i);
            bi.desc = Config.get_t_lang(bi.desc);
            bi.icon = dbc_battle_item.get_int(9, i);
            bi.param1 = dbc_battle_item.get_int(10, i);
            bi.param2 = dbc_battle_item.get_int(11, i);
            Config.t_battle_items.Add(bi.id, bi);
            if (bi.type == 3)
                Config.t_battle_ability_ball.Add(bi);
        }
    }

    public static t_battle_item get_t_battle_item(int id)
    {
        if (Config.t_battle_items.ContainsKey(id))
            return Config.t_battle_items[id];
        else
            return null;
    }
    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_battle_buff> t_battle_buffs = new Dictionary<int, t_battle_buff>();
    public static void parse_t_battle_buff()
    {
        var dbc_battle_buff = new dbc();
        dbc_battle_buff.load_txt("t_battle_buff");
        t_battle_buffs = new Dictionary<int, t_battle_buff>();
        for (int i = 0; i < dbc_battle_buff.get_y(); i++)
        {
            var tbb = new t_battle_buff();
            tbb.id = dbc_battle_buff.get_int(0, i);
            tbb.name = dbc_battle_buff.get_string(1, i);
            tbb.effect_type = dbc_battle_buff.get_int(2, i);
            tbb.effect = dbc_battle_buff.get_string(3, i);
            tbb.effect_end = dbc_battle_buff.get_string(4, i);
            tbb.effect_bone = dbc_battle_buff.get_string(5, i);
            tbb.attr = new List<pattr>();
            for (int j = 0; j < 4; j++)
            {
                var attr = new pattr();
                attr.ptype = dbc_battle_buff.get_int(6 + j * 3, i);
                attr.param1 = dbc_battle_buff.get_int(7 + j * 3, i);
                attr.param2 = dbc_battle_buff.get_int(8 + j * 3, i);
                if (attr.ptype > 0)
                    tbb.attr.Add(attr);
            }
            tbb.time = dbc_battle_buff.get_int(18, i);
            Config.t_battle_buffs.Add(tbb.id, tbb);
        }
    }

    public static t_battle_buff get_t_battle_buff(int id)
    {
        if (Config.t_battle_buffs.ContainsKey(id))
            return Config.t_battle_buffs[id];
        else
            return null;
    }
    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_ai_attr> t_ai_attrs = new Dictionary<int, t_ai_attr>();
    public static void parse_t_ai_attr()
    {
        var dbc_ai_attr = new dbc();
        dbc_ai_attr.load_txt("t_ai_attr");
        t_ai_attrs = new Dictionary<int, t_ai_attr>();
        for (int i = 0; i < dbc_ai_attr.get_y(); i++)
        {
            var ta = new t_ai_attr();
            ta.ai_type = dbc_ai_attr.get_int(0, i);
            ta.attack_p = dbc_ai_attr.get_int(2, i);
            ta.attack_t = dbc_ai_attr.get_int(3, i);
            ta.damage_p = dbc_ai_attr.get_int(4, i);
            ta.snow_p = dbc_ai_attr.get_int(5, i);
            ta.fire_p = dbc_ai_attr.get_int(6, i);
            ta.exp_t = dbc_ai_attr.get_int(7, i);
            ta.talent_p = dbc_ai_attr.get_int(8, i);
            ta.stand_p = dbc_ai_attr.get_int(9, i);
            ta.stand_min = dbc_ai_attr.get_int(10, i);
            ta.stand_max = dbc_ai_attr.get_int(11, i);
            ta.charge_attack_p = dbc_ai_attr.get_int(12, i);
            ta.attack_player_p = dbc_ai_attr.get_int(13, i);
            Config.t_ai_attrs.Add(ta.ai_type,ta);
        }
    }

    public static t_ai_attr get_t_ai_attr(int ai_type)
    {
        if (Config.t_ai_attrs.ContainsKey(ai_type))
            return Config.t_ai_attrs[ai_type];
        else
            return null;
    }
    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, Dictionary<int, t_skill>> t_skills = null;
    public static void parse_t_skill()
    {
        var dbc_t_skill = new dbc();
        dbc_t_skill.load_txt("t_skill");
        t_skills = new Dictionary<int, Dictionary<int, t_skill>>();
        for (int i = 0; i < dbc_t_skill.get_y(); i++)
        {
            var tdic = new t_skill();
            tdic.id = dbc_t_skill.get_int(0, i);
            tdic.name = dbc_t_skill.get_string(1, i);
            tdic.level = dbc_t_skill.get_int(2, i);
            tdic.type = dbc_t_skill.get_int(3, i);
            tdic.cd = dbc_t_skill.get_int(4, i);
            tdic.release_type = dbc_t_skill.get_int(5, i);
            tdic.range = dbc_t_skill.get_int(6, i);
            tdic.range_param = dbc_t_skill.get_int(7, i);
            tdic.action = dbc_t_skill.get_string(8, i);
            tdic.qy_action_time = dbc_t_skill.get_int(9, i);
            tdic.link_effect = dbc_t_skill.get_int(10, i);
            tdic.wy_speed = dbc_t_skill.get_int(11, i);
            tdic.wy_time = dbc_t_skill.get_int(12, i);
            tdic.hy_td_time = dbc_t_skill.get_int(13, i);
            tdic.icon = dbc_t_skill.get_string(14, i);
            tdic.cost = dbc_t_skill.get_int(15, i);

            if (!Config.t_skills.ContainsKey(tdic.id))
                Config.t_skills.Add(tdic.id, new Dictionary<int, t_skill>());
            if (!Config.t_skills[tdic.id].ContainsKey(tdic.level))
                Config.t_skills[tdic.id].Add(tdic.level, tdic);
        }
    }

    public static t_skill get_t_skill(int id, int level = 1,int add = 0)
    {
        level = level + add;
        if (!Config.t_skills.ContainsKey(id))
            return null;
        else
        {
            if (!Config.t_skills[id].ContainsKey(level))
                return null;
            else
                return Config.t_skills[id][level];
        }
    }

    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_role_skill> t_role_skills = null;

    public static void parse_t_role_skill()
    {
        var dbc_skill_role = new dbc();
        dbc_skill_role.load_txt("t_role_skill");
        t_role_skills = new Dictionary<int, t_role_skill>();
        for (int i = 0; i < dbc_skill_role.get_y(); i++)
        {
            var tdic = new t_role_skill();
            tdic.id = dbc_skill_role.get_int(0, i);
            tdic.name = dbc_skill_role.get_string(2, i);
            tdic.name = Config.get_t_lang(tdic.name);
            tdic.desc = dbc_skill_role.get_string(4, i);
            tdic.desc = Config.get_t_lang(tdic.desc);
            tdic.attrs = new List<SAttr>();
            for (int j = 0; j <= 1; j++)
            {
                SAttr sa = new SAttr();
                sa.type = dbc_skill_role.get_int(5 + j * 5, i);
                sa.param1 = dbc_skill_role.get_int(6 + j * 5, i);
                sa.param2 = dbc_skill_role.get_int(7 + j * 5, i);
                sa.param3 = dbc_skill_role.get_int(8 + j * 5, i);
                sa.param4 = dbc_skill_role.get_int(9 + j * 5, i);
                if (sa.type > 0)
                    tdic.attrs.Add(sa);
            }

            Config.t_role_skills.Add(tdic.id, tdic);
        }
    }

    public static t_role_skill get_t_role_skill(int id)
    {
        if (Config.t_role_skills.ContainsKey(id))
            return Config.t_role_skills[id];
        else
            return null;
    }
    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_talent> t_talents = null;
    public static void parse_t_talent()
    {
        var db_talent = new dbc();
        db_talent.load_txt("t_talent");
        t_talents = new Dictionary<int, t_talent>();
        for (int i = 0; i < db_talent.get_y(); i++)
        {
            var tdic = new t_talent();
            tdic.id = db_talent.get_int(0, i);
            tdic.desc1 = db_talent.get_string(2, i);
            tdic.desc1 = Config.get_t_lang(tdic.desc1);
            tdic.desc2 = db_talent.get_string(4, i);
            tdic.desc2 = Config.get_t_lang(tdic.desc2);
            tdic.max_level = db_talent.get_int(5, i);
            tdic.icon = db_talent.get_string(6, i);
            tdic.type = db_talent.get_int(7, i);
            tdic.param1 = db_talent.get_int(8, i);
            tdic.param2 = db_talent.get_int(9, i);
            tdic.param3 = db_talent.get_int(10, i);
            Config.t_talents.Add(tdic.id, tdic);
       }
    }
    public static t_talent get_t_talent(int id)
    {
        if (Config.t_talents.ContainsKey(id))
            return Config.t_talents[id];
        else
            return null;
    }
    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_skill_effect> t_skill_effects = null;
    public static void parse_t_skill_effect()
    {
        var dbc_skill_effect = new dbc();
        dbc_skill_effect.load_txt("t_skill_effect");
        t_skill_effects = new Dictionary<int, t_skill_effect>();
        for (int i = 0; i < dbc_skill_effect.get_y(); i++)
        {
            t_skill_effect tk = new t_skill_effect();
            tk.id = dbc_skill_effect.get_int(0, i);
            tk.name = dbc_skill_effect.get_string(1, i);
            tk.skill_id = dbc_skill_effect.get_int(2, i);
            tk.skill_type = dbc_skill_effect.get_int(3, i);
            tk.type = dbc_skill_effect.get_int(4, i);
            tk.range = dbc_skill_effect.get_int(5, i);
            tk.range_param = dbc_skill_effect.get_int(6, i);
            tk.release_pos = dbc_skill_effect.get_int(7, i);
            tk.effect = dbc_skill_effect.get_string(8, i);
            tk.xiaoshi_effect = dbc_skill_effect.get_string(9, i);
            tk.is_zaxs = dbc_skill_effect.get_int(10, i);
            tk.xiaoshi_effect1 = dbc_skill_effect.get_string(11, i);
            tk.is_jzxs = dbc_skill_effect.get_int(12, i);
            tk.xiaoshi_effect2 = dbc_skill_effect.get_string(13, i);
            tk.effect_scale = dbc_skill_effect.get_int(14, i) / 100.0f;
            tk.fx_speed = dbc_skill_effect.get_int(15, i);
            tk.fx_hight = dbc_skill_effect.get_int(16, i);
            tk.fx_hight1 = dbc_skill_effect.get_int(17, i);
            tk.fx_hight2 = dbc_skill_effect.get_int(18, i);
            tk.sh_time = dbc_skill_effect.get_int(19, i);
            tk.time = dbc_skill_effect.get_int(20, i);
            tk.target_type = dbc_skill_effect.get_int(21, i);
            tk.dd_type = dbc_skill_effect.get_int(22, i);
            tk.dd_jg = dbc_skill_effect.get_double(23, i);
            tk.is_zp = dbc_skill_effect.get_int(24, i);
            tk.fl_num = dbc_skill_effect.get_int(25, i);
            tk.fl_r = dbc_skill_effect.get_int(26, i);
            tk.link_xiaoguo = dbc_skill_effect.get_int(27, i);
            tk.link_effect = dbc_skill_effect.get_int(28, i);
            tk.link_effect1 = dbc_skill_effect.get_int(29, i);
            tk.togather_effect = dbc_skill_effect.get_int(30, i);
            tk.hit_effect = dbc_skill_effect.get_int(31, i);
            Config.t_skill_effects.Add(tk.id, tk);
        }
    }

    public static t_skill_effect get_t_skill_effect(int id)
    {
        if (Config.t_skill_effects.ContainsKey(id))
            return Config.t_skill_effects[id];
        else
            return null;
    }
    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_ai_setting> t_ai_settings = null;
    public static void parse_t_ai_setting()
    {
        var dbc_ai_setting = new dbc();
        dbc_ai_setting.load_txt("t_ai_setting");
        t_ai_settings = new Dictionary<int, t_ai_setting>();
        for (int i = 0; i < dbc_ai_setting.get_y(); i++)
        {
            t_ai_setting tas = new t_ai_setting();
            tas.level = dbc_ai_setting.get_int(0, i);
            tas.low_ai_amount = dbc_ai_setting.get_int(1, i);
            tas.mid_ai_amount = dbc_ai_setting.get_int(2, i);
            tas.high_ai_amount = dbc_ai_setting.get_int(3, i);
            Config.t_ai_settings.Add(tas.level, tas);
        }
    }

    public static t_ai_setting get_t_ai_setting(int level)
    {
        if (!Config.t_ai_settings.ContainsKey(level))
            return null;
        else
            return Config.t_ai_settings[level];
    }
    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_skill_xiaoguo> t_skill_xiaoguos = null;
    public static void parse_t_skill_xiaoguo()
    {
        var dbc_skill_xiaoguo = new dbc();
        dbc_skill_xiaoguo.load_txt("t_skill_xiaoguo");
        t_skill_xiaoguos = new Dictionary<int, t_skill_xiaoguo>();
        for (int i = 0; i < dbc_skill_xiaoguo.get_y(); i++)
        {
            t_skill_xiaoguo tkx = new t_skill_xiaoguo();
            tkx.id = dbc_skill_xiaoguo.get_int(0, i);
            tkx.name = dbc_skill_xiaoguo.get_string(1, i);
            tkx.sj_effect = dbc_skill_xiaoguo.get_string(2, i);
            tkx.dmg_type = dbc_skill_xiaoguo.get_int(3, i);
            tkx.dmg_per = dbc_skill_xiaoguo.get_int(4, i);
            tkx.dmg_gd = dbc_skill_xiaoguo.get_int(5, i);
            tkx.jf_type = dbc_skill_xiaoguo.get_int(6, i);
            tkx.jf_dis = dbc_skill_xiaoguo.get_int(7, i);
            tkx.jf_speed = dbc_skill_xiaoguo.get_int(8, i);
            tkx.link_buff = dbc_skill_xiaoguo.get_int(9, i);
            Config.t_skill_xiaoguos.Add(tkx.id, tkx);
        }
    }

    public static t_skill_xiaoguo get_t_skill_xiaoguo(int id)
    {
        if (Config.t_skill_xiaoguos.ContainsKey(id))
            return Config.t_skill_xiaoguos[id];
        else      
            return null;
    }
    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_achievement> t_achievements = null;
    public static Dictionary<int, List<t_achievement>> t_achieve_types = null;
    public static Dictionary<int, List<List<t_achievement>>> t_achieve_type_ids = null;
    public static Dictionary<int, List<t_achievement>> t_achieve_trees = null;
    public static void parse_t_achievement()
    {
        var dbc_achieve = new dbc();
        dbc_achieve.load_txt("t_achievement");
        t_achievements = new Dictionary<int, t_achievement>();
        t_achieve_types = new Dictionary<int, List<t_achievement>>();
        t_achieve_trees = new Dictionary<int, List<t_achievement>>();
        t_achieve_type_ids = new Dictionary<int, List<List<t_achievement>>>();

        for (int i = 0; i < dbc_achieve.get_y(); i++)
        {
            t_achievement tdic = new t_achievement();
            tdic.id = dbc_achieve.get_int(0, i);
            tdic.name = dbc_achieve.get_string(2, i);
            tdic.name = Config.get_t_lang(tdic.name);    
            tdic.desc = dbc_achieve.get_string(4, i);
            tdic.desc = Config.get_t_lang(tdic.desc);
            tdic.dtype = dbc_achieve.get_int(5, i);
            tdic.pre = dbc_achieve.get_int(6, i);
            tdic.target_num = dbc_achieve.get_int(7, i);
            tdic.type_id = dbc_achieve.get_int(8, i);
            tdic.param1 = dbc_achieve.get_int(9, i);
            tdic.param2 = dbc_achieve.get_int(10, i);
            tdic.param3 = dbc_achieve.get_int(11, i);
            tdic.param4 = dbc_achieve.get_int(12, i);
            tdic.point = dbc_achieve.get_int(13, i);
            tdic.max_star = dbc_achieve.get_int(14, i);
            tdic.star = dbc_achieve.get_int(15, i);
            tdic.icon = dbc_achieve.get_string(16, i);

            if (!t_achieve_types.ContainsKey(tdic.type_id))
                t_achieve_types.Add(tdic.type_id, new List<t_achievement>());
            t_achieve_types[tdic.type_id].Add(tdic);
            t_achievements.Add(tdic.id, tdic);
        }

        Dictionary<int, bool> searchState = new Dictionary<int, bool>();
        foreach (var item in Config.t_achievements)
        {
            if (!searchState.ContainsKey(item.Key))
            {
                List<t_achievement> list = new List<t_achievement>();
                var tmp = item.Value;
                while (tmp.pre > 0)
                {
                    list.Insert(0, tmp);
                    if (!searchState.ContainsKey(tmp.id))
                        searchState.Add(tmp.id, true);
                    tmp = Config.t_achievements[tmp.pre];
                }
                list.Insert(0, tmp);

                if (t_achieve_trees.ContainsKey(tmp.id))
                {
                    if (t_achieve_trees[tmp.id].Count < list.Count)
                        t_achieve_trees[tmp.id] = list;
                }
                else
                    t_achieve_trees.Add(tmp.id, list);
            }
        }

        foreach (var item in Config.t_achieve_trees)
        {
            var t_achievement = Config.get_t_achievement(item.Key);
            if (!Config.t_achieve_type_ids.ContainsKey(t_achievement.type_id))
                Config.t_achieve_type_ids.Add(t_achievement.type_id, new List<List<t_achievement>>());

            Config.t_achieve_type_ids[t_achievement.type_id].Add(item.Value);
        }

        foreach (var item in Config.t_achieve_trees)
        {
            if (item.Value.Count == 0)
                Config.get_t_achievement(item.Key).next_achieve_id = null;
            else
                Config.get_t_achievement(item.Key).next_achieve_id = item.Value[0].id;

            for (int i = 0; i < item.Value.Count; i++)
            {
                if (i != item.Value.Count - 1)
                    item.Value[i].next_achieve_id = item.Value[i + 1].id;
                else
                    item.Value[i].next_achieve_id = null;
            }
        }
    }

    public static t_achievement get_t_achievement(int id)
    {
        if (Config.t_achievements.ContainsKey(id))
            return Config.t_achievements[id];
        else
            return null;
    }
    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_task> t_tasks = null;
    public static Dictionary<int, List<t_task>> t_task_types = null;

    public static void parse_t_task()
    {
        var dbc_task = new dbc();
        dbc_task.load_txt("t_task");
        t_tasks = new Dictionary<int, t_task>();
        t_task_types = new Dictionary<int, List<t_task>>();
        for (int i = 0; i < dbc_task.get_y(); i++)
        {
            var tdic = new t_task();
            tdic.id = dbc_task.get_int(0, i);
            tdic.desc = dbc_task.get_string(2, i);
            tdic.desc = Config.get_t_lang(tdic.desc);
            tdic.level = dbc_task.get_int(3, i);
            tdic.dtype = dbc_task.get_int(4, i);
            tdic.target_num = dbc_task.get_int(5, i);
            tdic.type = dbc_task.get_int(6, i);
            tdic.param1 = dbc_task.get_int(7, i);
            tdic.param2 = dbc_task.get_int(8, i);
            tdic.param3 = dbc_task.get_int(9, i);
            tdic.param4 = dbc_task.get_int(10, i);
            tdic.rewards = new List<Rds>();
            for (int j = 1; j <= 2; j++)
            {
                var reward = new Rds();
                reward.type = dbc_task.get_int(11 + (j - 1) * 4, i);
                if (reward.type != 0)
                {
                    reward.value1 = dbc_task.get_int(12 + (j - 1) * 4, i);
                    reward.value2 = dbc_task.get_int(13 + (j - 1) * 4, i);
                    reward.value3 = dbc_task.get_int(14 + (j - 1) * 4, i);
                    tdic.rewards.Add(reward);
                }
            }

            if (!Config.t_task_types.ContainsKey(tdic.type))
                Config.t_task_types.Add(tdic.type, new List<t_task>());
            Config.t_task_types[tdic.type].Add(tdic);
            Config.t_tasks.Add(tdic.id, tdic);
        }
    }

    public static t_task get_t_task(int id)
    {
        if (Config.t_tasks.ContainsKey(id))
            return Config.t_tasks[id];
        else
            return null;
    }
    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_daily> t_dailys = null;
    public static Dictionary<int, List<t_daily>> t_daily_types = null;

    public static void parse_t_daily()
    {
        var dbc_daily = new dbc();
        dbc_daily.load_txt("t_daily");
        t_dailys = new Dictionary<int, t_daily>();
        t_daily_types = new Dictionary<int, List<t_daily>>();
        for (int i = 0; i < dbc_daily.get_y(); i++)
        {
            var tdic = new t_daily();
            tdic.id = dbc_daily.get_int(0, i);
            tdic.name = dbc_daily.get_string(2, i);
            tdic.name = Config.get_t_lang(tdic.name);
            tdic.desc = dbc_daily.get_string(4, i);
            tdic.desc = Config.get_t_lang(tdic.desc);
            tdic.dtype = dbc_daily.get_int(5, i);
            tdic.target_num = dbc_daily.get_int(6, i);
            tdic.type = dbc_daily.get_int(7, i);
            tdic.param1 = dbc_daily.get_int(8, i);
            tdic.param2 = dbc_daily.get_int(9, i);
            tdic.param3 = dbc_daily.get_int(10, i);
            tdic.param4 = dbc_daily.get_int(11, i);
            tdic.rewards = new List<Rds>();
            for (int j = 1; j <= 3; j++)
            {
                Rds reward = new Rds();
                reward.type = dbc_daily.get_int(12 + (j - 1) * 4, i);
                if (reward.type != 0)
                {
                    reward.value1 = dbc_daily.get_int(13 + (j - 1) * 4, i);
                    reward.value2 = dbc_daily.get_int(14 + (j - 1) * 4, i);
                    reward.value3 = dbc_daily.get_int(15 + (j - 1) * 4, i);
                    tdic.rewards.Add(reward);
                }
            }

            if (!Config.t_daily_types.ContainsKey(tdic.type))
                Config.t_daily_types.Add(tdic.type, new List<t_daily>());
            Config.t_daily_types[tdic.type].Add(tdic);
            Config.t_dailys.Add(tdic.id, tdic);
        }
    }

    public static t_daily get_t_daily(int id)
    {
        if (Config.t_dailys.ContainsKey(id))
            return Config.t_dailys[id];
        else
            return null;
    }
    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, List<t_preguide>> t_preguides = null;
    public static void parse_t_preguide()
    {
        var dbc_preguide = new dbc();
        dbc_preguide.load_txt("t_preguide");
        t_preguides = new Dictionary<int, List<t_preguide>>();
        for (int i = 0; i < dbc_preguide.get_y(); i++)
        {
            var tdic = new t_preguide();
            tdic.id = dbc_preguide.get_int(0, i);
            tdic.guide_id = dbc_preguide.get_int(1, i);
            tdic.type = dbc_preguide.get_int(2, i);
            tdic.offset_x = dbc_preguide.get_int(3, i);
            tdic.offset_y = dbc_preguide.get_int(4, i);
            tdic.param_int = dbc_preguide.get_int(5, i);
            tdic.param1 = dbc_preguide.get_int(6, i);
            tdic.param2 = dbc_preguide.get_int(7, i);
            tdic.param3 = dbc_preguide.get_int(8, i);
            if (!Config.t_preguides.ContainsKey(tdic.guide_id))
                Config.t_preguides.Add(tdic.guide_id,new List<t_preguide>());
            t_preguides[tdic.guide_id].Add(tdic);
        }
    }

    public static List<t_preguide> get_t_preguide(int guide_id)
    {
        if (!Config.t_preguides.ContainsKey(guide_id))
            return null;
        else
            return Config.t_preguides[guide_id];
    }
    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_battle_attr> t_battle_attrs = null;
    public static void parse_t_battle_attr()
    {
        var dbc_battle_attr = new dbc();
        dbc_battle_attr.load_txt("t_battle_attr");
        Config.t_battle_attrs = new Dictionary<int, t_battle_attr>();
        for (int i = 0; i < dbc_battle_attr.get_y(); i++)
        {
            var tdic = new t_battle_attr();
            tdic.id = dbc_battle_attr.get_int(0, i);
            tdic.isBadState = dbc_battle_attr.get_int(4, i);
            tdic.icon = dbc_battle_attr.get_string(5, i);
            tdic.icon_bg = dbc_battle_attr.get_string(6, i);
            Config.t_battle_attrs.Add(tdic.id, tdic);
        }
    }

    public static t_battle_attr get_t_battle_attr(int id)
    {
        if (Config.t_battle_attrs.ContainsKey(id))
            return Config.t_battle_attrs[id];
        else
            return null;
    }
    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_teamid> t_teamids = null;
    public static void parse_t_teamid()
    {
        var dbc_teamid = new dbc();
        dbc_teamid.load_txt("t_teamid");
        t_teamids = new Dictionary<int, t_teamid>();
        for (int i = 0; i < dbc_teamid.get_y(); i++)
        {
            var tdic = new t_teamid();
            tdic.id = dbc_teamid.get_int(0, i);
            tdic.name = dbc_teamid.get_string(2, i);
            tdic.name = Config.get_t_lang(tdic.name);
            Config.t_teamids.Add(tdic.id, tdic);
        }
    }

    public static t_teamid get_t_teamid(int id)
    {
        if (Config.t_teamids.ContainsKey(id))
            return Config.t_teamids[id];
        else
            return null;
    }
    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_avatar> t_avatars = null;
    public static List<int> t_avatar_ids = null;
    public static void parse_t_avatar()
    {
        var dbc_avatar = new dbc();
        dbc_avatar.load_txt("t_avatar");
        t_avatars = new Dictionary<int, t_avatar>();
        t_avatar_ids = new List<int>();

        for (int i = 0; i < dbc_avatar.get_y(); i++)
        {
            t_avatar tdic = new t_avatar();
            tdic.id = dbc_avatar.get_int(0, i);
            tdic.name = dbc_avatar.get_string(2, i);
            tdic.name = Config.get_t_lang(tdic.name);
            tdic.role_id = dbc_avatar.get_int(3, i);
            tdic.icon = dbc_avatar.get_string(4, i);
            tdic.desc1 = dbc_avatar.get_string(5, i);
            tdic.desc1 = Config.get_t_lang(tdic.desc1);
            tdic.desc2 = dbc_avatar.get_string(6, i);
            tdic.desc2 = Config.get_t_lang(tdic.desc2);
            tdic.color = dbc_avatar.get_int(7, i);
            Config.t_avatar_ids.Add(tdic.id);
            Config.t_avatars.Add(tdic.id, tdic);
        }
    }

    public static t_avatar get_t_avatar(int id)
    {
        if (Config.t_avatars.ContainsKey(id))
            return Config.t_avatars[id];
        else
            return null;
    }
    public static t_avatar get_t_avatar_id(int role_id)
    {
        foreach (var item in Config.t_avatars)
        {
            if (role_id == item.Value.role_id)
                return item.Value;
        }
        return null;
    }
    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_exp> t_exps = null;
    public static int max_level = 0;
    public static void parse_t_exp()
    {
        var dbc_exp = new dbc();
        dbc_exp.load_txt("t_exp");
        t_exps = new Dictionary<int, t_exp>();
        for (int i = 0; i < dbc_exp.get_y(); i++)
        {
            var tdic = new t_exp();
            tdic.level = dbc_exp.get_int(0, i);
            tdic.exp = dbc_exp.get_int(1, i);
            tdic.rewards = new List<Rds>();
            for (int j = 1; j <= 3; j++)
            {
                var reward = new Rds();
                reward.type = dbc_exp.get_int(2 + (j - 1) * 4, i);
                if (reward.type > 0)
                {
                    reward.value1 = dbc_exp.get_int(3 + (j - 1) * 4, i);
                    reward.value2 = dbc_exp.get_int(4 + (j - 1) * 4, i);
                    reward.value3 = dbc_exp.get_int(5 + (j - 1) * 4, i);
                    tdic.rewards.Add(reward);
                }
            }

            tdic.tasks = new List<int>();
            for (int j = 1; j <= 3; j++)
            {
                var task_id = dbc_exp.get_int(14 + j - 1, i);
                if (task_id > 0)
                    tdic.tasks.Add(task_id);
            }

            tdic.level_add = new List<fQua>();
            for (int j = 0; j <= 6; j++)
            {
                var add = new fQua();
                add.type = dbc_exp.get_int(j * 4 + 17, i);
                if (add.type != 0)
                {
                    add.param1 = dbc_exp.get_int(j * 4 + 18, i);
                    add.param2 = dbc_exp.get_int(j * 4 + 19, i);
                    add.param3 = dbc_exp.get_int(j * 4 + 20, i);
                    tdic.level_add.Add(add);
                }
            }
            Config.t_exps.Add(tdic.level, tdic);
            if (tdic.level > Config.max_level)
                Config.max_level = tdic.level;
        }
    }

    public static t_exp get_t_exp(int level)
    {
        if (Config.t_exps.ContainsKey(level))
            return Config.t_exps[level];
        else
            return null;
    }
    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_vip_attr> t_vip_attrs = null;
    public static void parse_t_vip_attr()
    {
        var dbc_vip_attr = new dbc();
        dbc_vip_attr.load_txt("t_vip_attr");
        t_vip_attrs = new Dictionary<int, t_vip_attr>();
        for (int i = 0; i < dbc_vip_attr.get_y(); i++)
        {
            var tdic = new t_vip_attr();
            tdic.id = dbc_vip_attr.get_int(0, i);
            tdic.color = dbc_vip_attr.get_string(2, i);
            tdic.first_id = dbc_vip_attr.get_int(5, i);
            tdic.day_id = dbc_vip_attr.get_int(6, i);
            tdic.attrs = new List<fQua>();
            for (int j = 0; j <= 4; j++)
            {
                fQua t_attr = new fQua();
                t_attr.type = dbc_vip_attr.get_int(7 + j * 4, i);
                if (t_attr.type > 0)
                {
                    t_attr.param1 = dbc_vip_attr.get_int(8 + j * 4, i);
                    t_attr.param2 = dbc_vip_attr.get_int(9 + j * 4, i);
                    t_attr.param3 = dbc_vip_attr.get_int(10 + j * 4, i);
                    tdic.attrs.Add(t_attr);
                }

            }
            Config.t_vip_attrs.Add(tdic.id, tdic);
        }
    }

    public static t_vip_attr get_t_vip_attr(int id)
    {
        if (Config.t_vip_attrs.ContainsKey(id))
            return Config.t_vip_attrs[id];
        else
            return null;
    }
    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_cup> t_cups = null;
    public static void parse_t_cup()
    {
        var dbc_cup = new dbc();
        dbc_cup.load_txt("t_cup");
        t_cups = new Dictionary<int, t_cup>();
        for (int i = 0; i < dbc_cup.get_y(); i++)
        {
            var tdic = new t_cup();
            tdic.id = dbc_cup.get_int(0, i);
            tdic.name = dbc_cup.get_string(2, i);
            tdic.name = Config.get_t_lang(tdic.name);
            tdic.star = dbc_cup.get_int(3, i);
            tdic.max_star = dbc_cup.get_int(4, i);
            tdic.icon = dbc_cup.get_string(5, i);
            tdic.small_icon = dbc_cup.get_string(6, i);
            tdic.dai_icon = dbc_cup.get_string(7, i);
            tdic.duan = dbc_cup.get_int(8, i);
            tdic.down = dbc_cup.get_int(9, i);
            tdic.sb = dbc_cup.get_int(10, i);
            tdic.jb = dbc_cup.get_int(11, i);
            tdic.tsb = dbc_cup.get_int(12, i);
            tdic.tjb = dbc_cup.get_int(13, i);     
            tdic.tsbnum = dbc_cup.get_int(14, i);
            tdic.rewards = new List<Rds>();
            for (int j = 0; j <= 2; j++)
            {
                Rds reward = new Rds();
                reward.type = dbc_cup.get_int(15 + j * 4, i);
                reward.value1 = dbc_cup.get_int(16 + j * 4, i);
                reward.value2 = dbc_cup.get_int(17 + j * 4, i);
                reward.value3 = dbc_cup.get_int(18 + j * 4, i);
                if (reward.type != 0)
                    tdic.rewards.Add(reward);
            }
            Config.t_cups.Add(tdic.id, tdic);
        }
    }

    public static t_cup get_t_cup(int id)
    {
        if (id < 0)
            return null;
        else if (id >= Config.t_cups.Count)
            id = Config.t_cups.Count - 1;

        return Config.t_cups[id];
    }

    /////////////////////////////////////////////////////////////////////////////////
    private static Dictionary<int, List<t_penguin>> penguinTypeDict = new Dictionary<int, List<t_penguin>>();
    private static Dictionary<int, t_penguin> penguins = new Dictionary<int, t_penguin>();

    public static void parse_t_penguin()
    {
        var dbc_penguin = new dbc();
        dbc_penguin.load_txt("t_penguin");
        penguinTypeDict = new Dictionary<int, List<t_penguin>>();
        penguins = new Dictionary<int, t_penguin>();
        for (int i = 0; i < dbc_penguin.get_y(); i++)
        {
            var tdic = new t_penguin();
            tdic.id = dbc_penguin.get_int(0, i);
            tdic.name = dbc_penguin.get_string(2, i);
            tdic.name = Config.get_t_lang(tdic.name);
            tdic.type = dbc_penguin.get_int(3, i);
            tdic.role_id = dbc_penguin.get_int(4, i);
            tdic.scale = dbc_penguin.get_int(5, i);
            tdic.view_radius = dbc_penguin.get_int(6, i);
            tdic.exp = dbc_penguin.get_int(7, i);
            tdic.max_bl = dbc_penguin.get_int(8, i);
            tdic.max_dis = dbc_penguin.get_int(9, i);
            tdic.skill_id = dbc_penguin.get_int(10, i);
            tdic.standp = dbc_penguin.get_int(11, i);
            tdic.mint = dbc_penguin.get_int(12, i);
            tdic.maxt = dbc_penguin.get_int(13, i);
            tdic.b_amount = dbc_penguin.get_int(14, i);
            tdic.b_pro = dbc_penguin.get_int(15, i);

            if (!penguinTypeDict.ContainsKey(tdic.type))
                penguinTypeDict.Add(tdic.type, new List<t_penguin>());
            penguinTypeDict[tdic.type].Add(tdic);
            penguins.Add(tdic.id, tdic);
        }
    }

    public static t_penguin get_t_penguin(int id)
    {
        if (penguins.ContainsKey(id))
            return penguins[id];
        else
            return null;
    }

    public static t_penguin get_random_t_penguin(int typeid)
    {
        if (!penguinTypeDict.ContainsKey(typeid))
            return null;

        int index = BattleOperation.random(0, penguinTypeDict[typeid].Count);
        return penguinTypeDict[typeid][index];
    }

    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_penguin_num> t_penguin_nums = new Dictionary<int, t_penguin_num>();

    private static void parse_t_penguin_num()
    {
        var dbc_penguin = new dbc();
        dbc_penguin.load_txt("t_penguin_num");
        t_penguin_nums = new Dictionary<int, t_penguin_num>();
        for (int i = 0; i < dbc_penguin.get_y(); i++)
        {
            var tdic = new t_penguin_num();
            tdic.type_id = dbc_penguin.get_int(0, i);
            tdic.num = dbc_penguin.get_int(1, i);
            t_penguin_nums.Add(tdic.type_id, tdic);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_battle_item_pos> t_battle_refresh_items = new Dictionary<int, t_battle_item_pos>();
    private static void parse_t_battle_item_pos()
    {
        var dbc_refresh_battle = new dbc();
        dbc_refresh_battle.load_txt("t_battle_item_pos");
        t_battle_refresh_items = new Dictionary<int, t_battle_item_pos>();
        for (int i = 0; i < dbc_refresh_battle.get_y(); i++)
        {
            var tdic = new t_battle_item_pos();
            tdic.id = dbc_refresh_battle.get_int(0, i);
            tdic.name = dbc_refresh_battle.get_string(1, i);
            tdic.pos_x = dbc_refresh_battle.get_int(2, i);
            tdic.pos_y = dbc_refresh_battle.get_int(3, i);
            tdic.max_dis = dbc_refresh_battle.get_int(4, i);
            tdic.max_amount = dbc_refresh_battle.get_int(5, i);
            tdic.bts = new List<t_battle_refresh_item>();
            for (int m = 0; m < 12; m++)
            {
                t_battle_refresh_item tbr = new t_battle_refresh_item();
                tbr.id = dbc_refresh_battle.get_int(6 + m * 4, i);
                tbr.name = dbc_refresh_battle.get_string(7 + m * 4, i);
                tbr.amount = dbc_refresh_battle.get_int(8 + m * 4, i);
                tbr.wt = dbc_refresh_battle.get_int(9 + m * 4, i);
                tdic.bts.Add(tbr);
            }
            t_battle_refresh_items.Add(tdic.id, tdic);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////// 
    public static Dictionary<int, t_scarf> t_scarfs = new Dictionary<int, t_scarf>();
    public static int max_pf_level = 1;
    private static void parse_t_scarf()
    {
        var dbc_scarf = new dbc();
        dbc_scarf.load_txt("t_scarf");
        t_scarfs = new Dictionary<int, t_scarf>();
        for (int i = 0; i < dbc_scarf.get_y(); i++)
        {
            var tdic = new t_scarf();
            tdic.id = dbc_scarf.get_int(0, i);
            tdic.min = dbc_scarf.get_int(1, i);
            tdic.max = dbc_scarf.get_int(2, i);
            tdic.pf_name = dbc_scarf.get_string(3, i);
            t_scarfs.Add(tdic.id, tdic);
            if (tdic.id > max_pf_level)
                max_pf_level = tdic.id;
        }
    }

    public static t_scarf get_t_scarf(int id)
    {
        if (t_scarfs.ContainsKey(id))
            return t_scarfs[id];
        else
            return null;
    }
    ///////////////////////////////////////////////////////////////////////////////// 
    private static Dictionary<int, t_battle_msg> t_battle_msgs = new Dictionary<int, t_battle_msg>();
    public static List<t_battle_msg> l_battle_msg = new List<t_battle_msg>();
    private static void parse_t_battle_msg()
    {
        var dbc_bt_msg = new dbc();
        dbc_bt_msg.load_txt("t_battle_msg");
        t_battle_msgs = new Dictionary<int, t_battle_msg>();
        l_battle_msg = new List<t_battle_msg>();
        for (int i = 0; i < dbc_bt_msg.get_y(); i++)
        {
            var tdic = new t_battle_msg();
            tdic.id = dbc_bt_msg.get_int(0, i);
            tdic.msg = dbc_bt_msg.get_string(2, i);
            tdic.msg = Config.get_t_lang(tdic.msg);
            t_battle_msgs.Add(tdic.id, tdic);
            l_battle_msg.Add(tdic);
        }
    }

    public static t_battle_msg get_t_battle_msg(int id)
    {
        if (t_battle_msgs.ContainsKey(id))
            return t_battle_msgs[id];
        else
            return null;
    }

    /////////////////////////////////////////////////////////////////////////////////
    private static Dictionary<int, t_hp_recover> t_hp_recovers = new Dictionary<int, t_hp_recover>();
    public static int max_score_buff_id = 0;
    private static void parse_t_hp_recover()
    {
        var db_hp_recover = new dbc();
        db_hp_recover.load_txt("t_hp_recover");
        t_hp_recovers = new Dictionary<int, t_hp_recover>();
        max_score_buff_id = int.MinValue;
        for (int i = 0; i < db_hp_recover.get_y(); i++)
        {
            var tdic = new t_hp_recover();
            tdic.id = db_hp_recover.get_int(0, i);
            tdic.min_score = db_hp_recover.get_int(1, i);
            tdic.max_score = db_hp_recover.get_int(2, i);
            tdic.buff_id = db_hp_recover.get_int(3, i);
            t_hp_recovers.Add(tdic.id, tdic);
            if (tdic.id > max_score_buff_id)
                max_score_buff_id = tdic.id;
        }
    }

    public static t_hp_recover get_t_hp_recover(int id)
    {
        if (t_hp_recovers.ContainsKey(id))
            return t_hp_recovers[id];
        else
            return null;
    }
    ///////////////////////////////////////////////////////////////////////////////// 
    private static Dictionary<string, t_script_str> t_scripts = new Dictionary<string, t_script_str>();
    public static void parse_t_script_str()
    {
        var db_script_lang = new dbc();
        db_script_lang.load_txt("t_script_str");
        t_scripts = new Dictionary<string, t_script_str>();

        for (int i = 0; i < db_script_lang.get_y(); i++)
        {
            var tdic = new t_script_str();
            tdic.id = db_script_lang.get_string(0, i);
            tdic.lang = new List<string>();

            for (int j = 1; j < db_script_lang.get_x(); j++)
            {
                var lang_str = db_script_lang.get_string(j, i).Replace("##", "\n");
                tdic.lang.Add(lang_str);
            }
            t_scripts.Add(tdic.id,tdic);
        }
    }

    public static string get_t_script_str(string key)
    {
        if (t_scripts.ContainsKey(key))
        {
            if (t_scripts[key].lang.Count > platform_config_common.languageType)
                return t_scripts[key].lang[platform_config_common.languageType];
            else
                return "";
        }
        else
            return ""; 
    }

    /////////////////////////////////////////////////////////////////////////////////
    private static Dictionary<int, t_guide_btn_path> t_guide_btn_paths = new Dictionary<int, t_guide_btn_path>();
    public static void parse_t_guide_btn_path()
    {
        var db_guide_btn = new dbc();
        db_guide_btn.load_txt("t_guide_btn_path");
        t_guide_btn_paths = new Dictionary<int, t_guide_btn_path>();

        for (int i = 0; i < db_guide_btn.get_y(); i++)
        {
            var tdic = new t_guide_btn_path();
            tdic.id = db_guide_btn.get_int(0, i);
            tdic.path = db_guide_btn.get_string(1, i);
            t_guide_btn_paths.Add(tdic.id, tdic);
        }
    }

    public static string get_t_guide_btn_path(int id)
    {
        if (t_guide_btn_paths.ContainsKey(id))
            return t_guide_btn_paths[id].path;
        else
            return null;
    }
    ///////////////////////////////////////////////////////////////////////////////// 
    private static Dictionary<int, List<t_guide_init>> t_guide_inits = new Dictionary<int, List<t_guide_init>>();
    public static void parse_t_guide_init()
    {
        var db_guide_init = new dbc();
        db_guide_init.load_txt("t_guide_init");
        t_guide_inits = new Dictionary<int, List<t_guide_init>>();

        for (int i = 0; i < db_guide_init.get_y(); i++)
        {
            var tdic = new t_guide_init();
            tdic.id = db_guide_init.get_int(0, i);
            tdic.guide_id = db_guide_init.get_int(1, i);
            tdic.type = db_guide_init.get_int(2, i);
            tdic.param_int = db_guide_init.get_int(3, i);
            tdic.param1 = db_guide_init.get_int(4, i);
            tdic.param2 = db_guide_init.get_int(5, i);
            tdic.param3 = db_guide_init.get_int(6, i);
            tdic.param_string = db_guide_init.get_string(7, i);
            tdic.offset_x = db_guide_init.get_int(8, i);
            tdic.offset_y = db_guide_init.get_int(9, i);
            if (!t_guide_inits.ContainsKey(tdic.guide_id))
                t_guide_inits.Add(tdic.guide_id, new List<t_guide_init>());
            t_guide_inits[tdic.guide_id].Add(tdic);
        }
    }

    public static List<t_guide_init> get_t_guide_init(int id)
    {
        //t_guide_inits 这个字典中的 key 是表中的.guide_id ;value 是这个table
        if (t_guide_inits.ContainsKey(id))
            return t_guide_inits[id];
        else
            return null;
    }
    /////////////////////////////////////////////////////////////////////////////////
    private static Dictionary<int, List<t_guide_condition>> t_guide_conditions = new Dictionary<int, List<t_guide_condition>>();
    public static void parse_t_guide_condition()
    {
        var db_guide_condition = new dbc();
        db_guide_condition.load_txt("t_guide_condition");
        t_guide_conditions = new Dictionary<int, List<t_guide_condition>>();

        for (int i = 0; i < db_guide_condition.get_y(); i++)
        {
            var tdic = new t_guide_condition();
            tdic.id = db_guide_condition.get_int(0, i);
            tdic.guide_id = db_guide_condition.get_int(1, i);
            tdic.type = db_guide_condition.get_int(2, i);
            tdic.type_child_id = db_guide_condition.get_int(3, i);
            tdic.param_int = db_guide_condition.get_int(4, i);
            tdic.param1 = db_guide_condition.get_int(5, i);
            tdic.param2 = db_guide_condition.get_int(6, i);
            tdic.param3 = db_guide_condition.get_int(7, i);
            tdic.param_string = db_guide_condition.get_string(8, i);
            if (!t_guide_conditions.ContainsKey(tdic.guide_id))
                t_guide_conditions.Add(tdic.guide_id, new List<t_guide_condition>());
            t_guide_conditions[tdic.guide_id].Add(tdic);
        }
    }

    public static List<t_guide_condition> get_t_guide_condition(int id)
    {
        if (t_guide_conditions.ContainsKey(id))
            return t_guide_conditions[id];
        else
            return null;
    }
    /////////////////////////////////////////////////////////////////////////////////
    public static Dictionary<int, t_guide> t_guides = null;
    public static int max_guide_step = 0;
    public static void parse_t_guide()
    {
        var dbc_guide = new dbc();
        dbc_guide.load_txt("t_guide");
        t_guides = new Dictionary<int, t_guide>();
        for (int i = 0; i < dbc_guide.get_y(); i++)
        {
            var tdic = new t_guide();
            tdic.id = dbc_guide.get_int(0, i);
            tdic.step_id = dbc_guide.get_int(1, i);
            tdic.step_child_id = dbc_guide.get_int(2, i);
            if (tdic.step_id > max_guide_step)
                max_guide_step = tdic.step_id;
            Config.t_guides.Add(tdic.id, tdic); 
        }
    }

    ///////////////////////////////////////////////////////////////////////////////// 
    private static Dictionary<string, t_ui_label> t_ui_labels = new Dictionary<string, t_ui_label>();

    public static void parse_t_ui_label()
    {
        var db_ui_label = new dbc();
        db_ui_label.load_txt("t_ui_label");
        t_ui_labels = new Dictionary<string, t_ui_label>();
        for (int i = 0; i < db_ui_label.get_y(); i++)
        {
            var tdic = new t_ui_label();
            tdic.id = db_ui_label.get_string(0, i);

            tdic.lang = new string[db_ui_label.get_x() - 1];
            for (int j = 1; j < db_ui_label.get_x(); j++)
                tdic.lang[j - 1] = db_ui_label.get_string(j, i).Replace("##", "\n");
                     
            if (!t_ui_labels.ContainsKey(tdic.id))
                t_ui_labels.Add(tdic.id, tdic);
        }
    }

    public static string get_t_ui_string(string k)
    {
        if (Config.t_ui_labels.ContainsKey(k))
        {
            if (Config.t_ui_labels[k].lang.Length > platform_config_common.languageType)
                return Config.t_ui_labels[k].lang[platform_config_common.languageType];
            else
                return "";
        }

        return "";
    }
    /////////////////////////////////////////////////////////////////////////////////
    private static Dictionary<int, t_region> t_regions = new Dictionary<int, t_region>();
    private static List<t_region> t_region_list = new List<t_region>();
    private static void parse_t_foregion()
    {
        var db_region = new dbc();
        db_region.load_txt("t_region");
        t_regions = new Dictionary<int, t_region>();
        t_region_list = new List<t_region>();
        for (int i = 0; i < db_region.get_y(); i++)
        {
            var tdic = new t_region();
            tdic.id = db_region.get_int(0, i);
            tdic.icon = db_region.get_string(1, i);
            t_regions.Add(tdic.id, tdic);
            t_region_list.Add(tdic);
        }
    }

    public static t_region get_t_foregion(int id)
    {
        if (t_regions.ContainsKey(id))
            return t_regions[id];
        else
            return null;
    }

    public static t_region get_random_region()
    {
        int num = BattleOperation.random(0,Config.t_region_list.Count);
        return Config.t_region_list[num];
    }
    ///////////////////////////////////////////////////////////////////////////////// 
    public static void Init()
    {
        Config.parse_t_lang();
        Config.parse_t_role();
        Config.parse_t_boss_attr();
        Config.parse_t_name();
        Config.parse_t_toukuang();
        Config.parse_t_fashion();
        Config.parse_t_bodysize();
        Config.parse_t_battle_exp();
        Config.parse_t_battle_item();
        Config.parse_t_battle_buff();
        Config.parse_t_ai_attr();
        Config.parse_t_skill();
        Config.parse_t_role_skill();
        Config.parse_t_talent();
        Config.parse_t_skill_effect();
        Config.parse_t_ai_setting();
        Config.parse_t_skill_xiaoguo();
        Config.parse_t_role_buff();
        Config.parse_t_achievement();
        Config.parse_t_task();
        Config.parse_t_daily();
        Config.parse_t_guide();
        Config.parse_t_preguide();
        Config.parse_t_battle_attr();
        Config.parse_t_teamid();
        Config.parse_t_avatar();
        Config.parse_t_exp();
        Config.parse_t_vip_attr();
        Config.parse_t_cup();
        Config.parse_t_penguin();
        Config.parse_t_penguin_num();
        Config.parse_t_battle_item_pos();
        Config.parse_t_scarf();
        Config.parse_t_battle_msg();
        Config.parse_t_hp_recover();
        Config.parse_t_script_str();
        Config.parse_t_guide_btn_path();
        Config.parse_t_guide_init();
        Config.parse_t_guide_condition();
        Config.parse_t_ui_label();
        Config.parse_t_foregion();
    }
}

