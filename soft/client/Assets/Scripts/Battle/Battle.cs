using BattleDB;
using dhc;
using protocol.game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;


public class find_player
{
    public BattleAnimal tbp;
    public int r;
}

public class Battle
{
    public const int BL = 10000;
    public static string name = "";
    public static bool is_end = false;
    public static bool is_online = false;
    public static bool is_newplayer_guide = false;
    public static bool isPause = false;
    public static bool ch_pause = false;
    public static double turnTime = 8;
    public static int exTime = 0;
    public static int is_new = 0;
    public static List<int> self_skills = null;
    //zdb  是否是第一次的技能拾取
    public static bool is_frist_Skill_Hold = false;

    public static int move_hold_r_ = -1;
    public static int attack_hold_r_ = -1;
    public static Dictionary<string, bool> is_hold_ = new Dictionary<string, bool>() { { "move", false }, { "attack",false }, { "jskill",false }};
    public static List<string> holds_ = new List<string>(){"move","attack", "jskill"};
    public static int key_hold_r_ = -1;

    private static double MAX_READY_TIME = 30;
    private static bool ready_to_start_ = false;
    private static double offlinet_ = 0;
    private static double ready_time_ = 0;
    public static BattlePanel battle_panel;
    // 这个方法是lua 传过来的数据 ()
    public static void UpdateSelfData(player_t player,string guid,string battle_code,List<role_t> roles,int quality,string share_url,int eff_quatity)
    {
        self.player = player;
        self.guid = guid;
        self.battle_code = battle_code;
        self.roles = roles;
        self.quality = quality;
        self.share_url = share_url;
        self.eff_quatity = eff_quatity;
    }
    public static void Awake(string _name, bool _is_online)
    {
        Awake(_name, _is_online, false);
    }
    // lua 调用过来的方法 (两个重载)
    public static void Awake(string _name, bool _is_online, bool is_guide)
    {
        name = _name;
        isPause = false;
        ch_pause = false;
        Battle.is_end = false;
        Battle.is_online = _is_online;
	    move_hold_r_ = -1;
	    attack_hold_r_ = -1;
        is_hold_ = new Dictionary<string, bool>() { { "move", false }, { "attack", false }, { "jskill", false } };
        need_send_attackr = false;
        need_send_attackr_r = 0;
        need_send_move_stop = false;
        need_send_move_stop_action = false;
        need_send_move_stop_r = 0;

        if (!is_guide)
        {
            is_newplayer_guide = false;
            turnTime = 8;
            exTime = 0;
        }
        else
        {
            is_newplayer_guide = true;
            turnTime = 2.5;
            exTime = 0;
        }
        offlinet_ = 0;
        Util.CallMethod("GUIRoot", "HideGUI", "HallPanel");
        GUIRoot.ShowGUI<BattlePanel>("BattlePanel");
        GUIRoot.HideGUI("BattlePanel", false);
        battle_panel = GUIRoot.GetPanel("BattlePanel").GetComponent<BattlePanel>();

        AppFacade._instance.BattleTask.AddUpdate("Battle", Update);
        BattleOperation.set_random_seed(0, 50);
        Battle.RegisterMessage();
        Start();
    }

    public static void Start()
    {
        ready_to_start_ = true;
        ready_time_ = 0;
        Battle.send_link();
    }

    public static void OnDestroy()
    {
        AppFacade._instance.BattleTask.RemoveUpdate("Battle");
        Battle.RemoveRegisterMessage();
        GUIRoot.HideGUI("BattlePanel");
        BattlePlayers.Fini();
        battle_panel = null;
        Util.CallMethod("AchieveAnimation", "ClearFightQueue");     //清理 战斗成就 播放队列
    }

    public static string GetTimeStr()
    {
        char[] t = DateTime.Now.Ticks.ToString().ToCharArray();
        StringBuilder s = new StringBuilder();
        for (int i = t.Length - 1; i >= (t.Length - 6); i--)
        {
            s.Append(t[i]);
        };
        return s.ToString();
    }

    private static void Update()
    {
        if (ready_to_start_)
        {
            if (ready_time_ < Battle.MAX_READY_TIME)
            {
                ready_time_ = ready_time_ + Time.deltaTime / Time.timeScale;
                bool isconnect = LuaHelper.GetNetManager().Isconnect("BattleTcp");
                if (Battle.is_online && (ready_time_ >= Battle.MAX_READY_TIME || !isconnect))
                {
                    Util.CallMethod("BattleTcp", "Disconnect3");
                    return;
                }
                else if (!Battle.is_online && ready_time_ >= Battle.MAX_READY_TIME)
                {
                    Util.CallMethod("BattleTcp", "Disconnect3");
                    return;
                }
            }

            if (BattlePlayers.start && !BattlePlayers.jiasu)
            {
                ready_to_start_ = false;   
                Util.CallMethod("LoadPanel", "csharpclose");
            }
        }

        if (!Battle.is_online)
        {
            offlinet_ = offlinet_ + Time.deltaTime / Time.timeScale;
            while (offlinet_ >= BattlePlayers.TICK / 1000.0f)
            {
                offlinet_ = offlinet_ - BattlePlayers.TICK / 1000.0f;
                if (!Battle.is_newplayer_guide && BattlePlayers.max_zhen >= BattleOperation.toInt(BattlePlayers.TNUM * Battle.turnTime * 60) + Battle.exTime)
                {
                    if (!Battle.is_end)
                    {
                        battle_panel.show_end();
                        Battle.is_end = true;
                        send_offline_state(0);
                    }
                    battle_panel.UpdateBattlePanel();
                    return;
                }

                if (!Battle.isPause)
                {
                    BattlePlayers.max_zhen = BattlePlayers.max_zhen + 1;
                }
            }
        }

        if (Battle.is_online)
        {
            if (LuaHelper.GetNetManager().Isconnect("BattleTcp"))
            {
                if (!LuaHelper.GetNetManager().Isconnect("BattleStateTcp"))
                    Util.CallMethod("BattleStateTcp", "Connect");
            }
        }

        BattlePlayers.Update();
        battle_panel.UpdateBattlePanel();
        Util.CallMethod("AchieveAnimation", "UpdateFightAchieve");
        if (BattlePlayers.me != null)
        {
            var bp = BattlePlayers.me;
            bool joy_mode = false;
            var joy = Joy.Get("move");
            if (joy != null)
            {
                if ((Battle.is_newplayer_guide && !Battle.ch_pause) || (!Battle.is_newplayer_guide))
                {
                    if (joy.IsHolding() && !joy.IsStart())
                    {
                        joy_mode = true;
                        float d = joy.Axis2Angle(true);
                        int r = BattleOperation.toInt((d + 5) / 10) * 10;
                        var flag = false;
                        if (!is_hold_["move"])
                        {
                            is_hold_["move"] = true;
                            move_hold_r_ = r;
                            flag = true;
                        }
                        else if (r != move_hold_r_)
                        {
                            move_hold_r_ = r;
                            flag = true;
                        }

                        if (flag)
                            Battle.send_move(move_hold_r_);
                    }
                    else
                    {
                        if (is_hold_["move"])
                        {
                            joy_mode = true;
                            is_hold_["move"] = false;
                            Battle.send_stop(move_hold_r_);
                        }
                    }
                }
            }

            if ((Battle.is_newplayer_guide && !Battle.ch_pause) || (!Battle.is_newplayer_guide))
            {
                if (!joy_mode)
                {
                    var key = -1;
                    if (Input.GetKey(KeyCode.I) && Input.GetKey(KeyCode.L))
                        key = 45;
                    else if (Input.GetKey(KeyCode.J) && Input.GetKey(KeyCode.I))
                        key = 135;
                    else if (Input.GetKey(KeyCode.K) && Input.GetKey(KeyCode.J))
                        key = 225;
                    else if (Input.GetKey(KeyCode.K) && Input.GetKey(KeyCode.L))
                        key = 315;
                    else if (Input.GetKey(KeyCode.I))
                        key = 90;
                    else if (Input.GetKey(KeyCode.K))
                        key = 270;
                    else if (Input.GetKey(KeyCode.J))
                        key = 180;
                    else if (Input.GetKey(KeyCode.L))
                        key = 0;

                    if (key != key_hold_r_)
                    { 
                        if (key == -1)
                            Battle.send_stop(key_hold_r_);
                        else
                            Battle.send_move(key);
                        key_hold_r_ = key;
                    }
                }
            }

            for (int i = 1; i < holds_.Count; i++)
            {
                string xm = holds_[i];
                int skill = 100101;
                int skill_level = 1;
                if (i == 2)
                {
                    skill = bp.player.skill_id;
                    if (skill <= 0)
                        skill_level = 0;
                    else
                        skill_level = bp.get_skill_level(skill);
                }

                if (skill == 0 && BattleSkillRange.GetName() == xm)
                {
                    BattleSkillRange.Destroy();
                    is_hold_[xm] = false;
                }

                if (skill > 0)
                {
                    joy = Joy.Get(xm);
                    if (joy != null && skill > 0)
                    {
                        int mode = joy.Mode();
                        if (joy.IsHolding())
                        {
                            var ts = Config.get_t_skill(skill, skill_level, BattlePlayers.me.get_skill_level_add(skill));
                            if ((ts == null) || (bp.player.cost < ts.cost * BattlePlayers.POWERUP))
                            {
                                string s = @"client_msg = {'"+Config.get_t_script_str("Battle_001") +"'}";
                                var msg = AppFacade._instance.LuaManager.GetLuaTable(s, "client_msg");
                                Util.CallMethod("GUIRoot", "ShowGUI", "MessagePanel", msg);
                                return;
                            }
                            if (ts != null)
                            {
                                bool first_hold = false;
                                if (!is_hold_[xm])
                                {
                                    is_hold_[xm] = true;
                                    first_hold = true;
                                    if (mode != 2 && i > 1)
                                        BattleSkillRange.Create(xm, ts.release_type, ts.get_range(bp), ts.get_range_param(bp));
                                    if (i == 1)
                                        Battle.send_prerelease(1);
                                }

                                if (mode != 2)
                                {
                                    if (i > 1 && BattleSkillRange.GetName() == xm)
                                    {
                                        int arr = 0;
                                        if (joy.IsStart() || ts.release_type > 3)
                                        {
                                            var near = Battle.GetNear(ts.get_range(bp));
                                            BattleSkillRange.SetPosition1(bp, near.r, near.tbp);
                                            arr = near.r;
                                        }
                                        else
                                        {
                                            int r = BattleOperation.toInt(joy.Axis2Angle(true));
                                            var pos = joy.GetPos();
                                            BattleSkillRange.SetPosition(bp, r, pos.x, pos.y);
                                            arr = r;
                                        }
                                        arr = BattleOperation.toInt((arr + 5) / 10) * 10;

                                        if (i == 1)
                                            BattleSkillRange.SetRange(ts.get_range(bp), ts.get_range_param(bp));


                                        BattleSkillRange.SetC(joy.IsCancel());

                                        if (first_hold || arr != attack_hold_r_)
                                        {
                                            attack_hold_r_ = arr;
                                            Battle.send_attackr(arr);
                                        }
                                    }
                                    else if (i == 1)
                                    {
                                        if (bp.player.re_pre_zhen * BattlePlayers.TICK > 300 && BattleSkillRange.GetName() == "")
                                            BattleSkillRange.Create(xm, ts.release_type, ts.get_range(bp), ts.get_range_param(bp));

                                        var ar = 0;
                                        if (joy.IsStart() || ts.release_type > 3)
                                        {
                                            var near = Battle.GetNear(ts.get_range(bp));
                                            if (BattleSkillRange.GetName() != "")
                                                BattleSkillRange.SetPosition1(bp, near.r, near.tbp);
                                            ar = near.r;
                                        }
                                        else
                                        {
                                            int r = BattleOperation.toInt(joy.Axis2Angle(true));

                                            Vector2 pos = joy.GetPos();
                                            if (BattleSkillRange.GetName() != "")
                                                BattleSkillRange.SetPosition(bp, r, pos.x, pos.y);
                                            ar = r;
                                        }
                                        ar = BattleOperation.toInt((ar + 5) / 10) * 10;
                                        if (BattleSkillRange.GetName() != "")
                                        {
                                            BattleSkillRange.SetRange(ts.get_range(bp), ts.get_range_param(bp));
                                            BattleSkillRange.SetC(joy.IsCancel());
                                        }

                                        if (first_hold || ar != attack_hold_r_)
                                        {
                                            attack_hold_r_ = ar;
                                            Battle.send_attackr(ar);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (is_hold_[xm])
                            {
                                is_hold_[xm] = false;
                                if (mode != 2)
                                {
                                    if (BattleSkillRange.GetName() == "")
                                    {
                                        var t_skill = Config.get_t_skill(skill, skill_level, BattlePlayers.me.get_skill_level_add(skill));
                                        if (t_skill != null)
                                        {
                                            if ((Battle.is_newplayer_guide && !Battle.isPause) || !Battle.ch_pause)
                                            {
                                                var near = Battle.GetNear(t_skill.get_range(bp));
                                                if (near.tbp == null)
                                                    near.tbp = bp;
                                                Battle.send_release(t_skill.id, near.tbp.animal.x, near.tbp.animal.y, near.r, bp.player.re_pre_zhen);
                                            }
                                        }
                                    }
                                    else if (BattleSkillRange.GetName() == xm)
                                    {
                                        var t_skill = Config.get_t_skill(skill, skill_level, BattlePlayers.me.get_skill_level_add(skill));
                                        if (t_skill != null)
                                        {
                                            var cs = BattleSkillRange.GetPosition();
                                            BattleSkillRange.Destroy();
                                            if (!joy.IsCancel())
                                            {
                                                if ((Battle.is_newplayer_guide && !Battle.isPause) || !Battle.ch_pause)
                                                {
                                                    Battle.send_release(t_skill.id, cs[0], cs[1], cs[2], bp.player.re_pre_zhen);
                                                }                                            
                                            }
                                        }

                                    }
                                }
                                //蓄力结束
                                if (i == 1)
                                    Battle.send_prerelease(0);
                            }
                        }
                            
                    }
                }
            }

        }
    }

    private static void send_link()
    {
        BattlePlayers.Fini();
        if (Battle.is_online)
        {
            var msg = new cmsg_battle_link();
            msg.guid = Convert.ToUInt64(self.guid);
            msg.code = self.battle_code;
            Battle.SendBattleUdp<cmsg_battle_link>(msg, opclient_t.CMSG_BATTLE_LINK);
        }
        else
        {
            BattlePlayers.Init(null);
            BattlePlayers.max_zhen = -1;
            BattlePlayers.zhen = 0;
            BattlePlayers.tid = 0;
            BattlePlayers.init_item = false;
            BattlePlayers.dtime = 0;
            BattlePlayers.jiasu = true;
            BattlePlayers.seed = UnityEngine.Random.Range(0, 99999999);
            BattlePlayers.seed_add = UnityEngine.Random.Range(0, 999);
            BattlePlayers.team_num = 6;
            BattlePlayers.member_num = 1;
            if (!Battle.is_newplayer_guide)
            {
                Battle.is_new = 1;
                Battle.self_skills = Battle.RandomForeBattleMsg();
            }
            else
            {
                Battle.is_new = 0;
                Battle.self_skills = null;
            }
            BattlePlayerAI.Init();
            send_offline_state(8);
        }
    }
    public static void send_offline_state(int state)
    {
        cmsg_offline_battle msg = new cmsg_offline_battle();
        msg.fight = state;
        GameTcp.Send<cmsg_offline_battle>(opclient_t.CMSG_OFFLINE_BATTLE,msg);
    }
    //////////////////////////////////////////////////static////////////////////////////////////////////////////////
    public static void RegisterMessage()
    {
        CSharpNetMessage.AddCSharpNetEvent(opclient_t.SMSG_BATTLE_LINK, Battle.SMSG_BATTLE_LINK);
        CSharpNetMessage.AddCSharpNetEvent(opclient_t.SMSG_BATTLE_OP, Battle.SMSG_BATTLE_OP);
        CSharpNetMessage.AddCSharpNetEvent(opclient_t.SMSG_BATTLE_ZHEN, Battle.SMSG_BATTLE_ZHEN);
        CSharpNetMessage.AddCSharpNetEvent(opclient_t.SMSG_BATTLE_FINISH, Battle.SMSG_BATTLE_FINISH);
        CSharpNetMessage.AddCSharpNetEvent(opclient_t.SMSG_GUIDE, Battle.SMSG_GUIDE);
    }

    public static void RemoveRegisterMessage()
    {
        CSharpNetMessage.RemoveCSharpNetEvent(opclient_t.SMSG_BATTLE_LINK, Battle.SMSG_BATTLE_LINK);
        CSharpNetMessage.RemoveCSharpNetEvent(opclient_t.SMSG_BATTLE_OP, Battle.SMSG_BATTLE_OP);
        CSharpNetMessage.RemoveCSharpNetEvent(opclient_t.SMSG_BATTLE_ZHEN, Battle.SMSG_BATTLE_ZHEN);
        CSharpNetMessage.RemoveCSharpNetEvent(opclient_t.SMSG_BATTLE_FINISH, Battle.SMSG_BATTLE_FINISH);
        CSharpNetMessage.RemoveCSharpNetEvent(opclient_t.SMSG_GUIDE, Battle.SMSG_GUIDE);
    }
    public static void send_state()
    {
        if (!Battle.is_online)
           return;

        if (BattlePlayers.me == null)
            return;
        var msg = new cmsg_battle_state();
        msg.guid = Convert.ToUInt64(BattlePlayers.me.player.guid);
        msg.state = new msg_battle_state();
        msg.state.zhen = BattlePlayers.zhen;
        msg.state.tid = BattlePlayers.tid;

        msg.state.boss.Add(TypeTransform.toMsgBattleBoss(BattlePlayers.Boss.player));
        for (int i = 0; i < BattlePlayers.penguin_list.Count; i++)
            msg.state.monsters.Add(TypeTransform.toMsgBattleMonster(BattlePlayers.penguin_list[i].player));

        for (int i = 0; i < BattlePlayers.players_list.Count; i++)
            msg.state.players.Add(TypeTransform.toMsgBattlePlayer(BattlePlayers.players_list[i].player));

        var effectDics = from obj in BattlePlayers.effects orderby obj.Key select obj;
        foreach (KeyValuePair<int, battle_effect> kvp in effectDics)
            msg.state.effects.Add(kvp.Value.effect);

        var itemDics = from obj in BattlePlayers.items orderby obj.Key select obj;
        var its = new List<battle_item_base>();
        foreach (KeyValuePair<int, battle_item> kvp in itemDics)
            its.Add(kvp.Value.item);

        List<msg_battle_item> items = null;
        List<msg_battle_item_base> bases = null;
        TypeTransform.toBattleItem(its, out items, out bases);
        foreach (var item in items)
            msg.state.items.Add(item);
        foreach (var item in bases)
            msg.state.bases.Add(item);

        msg.state.init_item = BattlePlayers.init_item;
        Battle.SendBattleTcp<cmsg_battle_state>(msg,opclient_t.CMSG_BATTLE_STATE);
    }

    public static void send_code()
    {
        if (!Battle.is_online)
            return;
        var msg = new msg_battle_code();
        msg.zhen = BattlePlayers.zhen;
        msg.code = 1;
        int code = 0;
        for (int ll = 0; ll < BattlePlayers.players_list.Count; ll++)
        {
            var bp = BattlePlayers.players_list[ll];
            code = code + BattleOperation.string2int(bp.player.name);
            code = code + bp.player.sex + bp.player.role_level + bp.player.camp;
            for (int i = 0; i < bp.player.attr_type.Count; i++)
                code = code + bp.player.attr_type[i];
            for (int i = 0; i < bp.player.attr_param1.Count; i++)
                code = code + bp.player.attr_param1[i];
            for (int i = 0; i < bp.player.attr_param2.Count; i++)
                code = code + bp.player.attr_param2[i];
            for (int i = 0; i < bp.player.attr_param3.Count; i++)
                code = code + bp.player.attr_param3[i];
            code = code + bp.player.avatar;
            code = code + bp.player.cup;
            code = code + bp.player.toukuang;
            for (int i = 0; i < bp.player.fashion.Count; i++)
                code = code + bp.player.fashion[i];
            //code = code + bp.player.region;
            code = code + bp.player.name_color + bp.player.level + bp.player.exp + bp.player.score + bp.player.score_level;
            for (int i = 0; i < bp.player.lattr_value.Count; i++)
                code = code + bp.player.lattr_value[i];
            for (int i = 0; i < bp.player.talent_id.Count; i++)
                code = code + bp.player.talent_id[i];
            for (int i = 0; i < bp.player.talent_level.Count; i++)
                code = code + bp.player.talent_level[i];
            code = code + bp.player.talent_point + bp.player.die + bp.player.sha + bp.player.lsha + bp.player.max_lsha;
            code = code + bp.player.skill_id + bp.player.skill_level + bp.player.re_pre + bp.player.re_pre_zhen + bp.player.re_pre_zhen1;
            code = code + bp.player.xueren_zhen + bp.player.cost;
            if (bp.player.is_xueren)
                code = code + 1;
            code = code + bp.player.attack_state + bp.player.attackZhen + bp.player.birth_x + bp.player.birth_y;
            code = code + BattleOperation.string2int(bp.player.attack_guid);
            //unit
            code = code + BattleOperation.string2int(bp.animal.guid);
            code = code + bp.animal.role_id + bp.animal.x + bp.animal.y + bp.animal.r + bp.animal.r_py;
            if (bp.player.is_move)
                code = code + 1;
            code = code + bp.animal.re_tid + bp.animal.re_state + bp.animal.re_id + bp.animal.re_level + bp.animal.re_time;
            code = code + bp.animal.re_x + bp.animal.re_y + bp.animal.re_r;
            for (int i = 0; i < bp.player.save_re_id.Count; i++)
                code = code + bp.player.save_re_id[i];
            for (int i = 0; i < bp.player.save_re_zhen.Count; i++)
                code = code + bp.player.save_re_zhen[i];
            if (bp.player.is_jf)
                code = code + 1;

            code = code + bp.animal.jf_sx + bp.animal.jf_sy + bp.animal.jf_xx + bp.animal.jf_yy + bp.animal.jf_speed;
            code = code + bp.animal.jf_r + bp.animal.death_time + bp.animal.rtime + bp.animal.attackr;
            for (int i = 0; i < bp.player.bhit_tids.Count; i++)
                code = code + bp.player.bhit_tids[i];

            code = code + bp.animal.hp + bp.animal.is_ai + bp.animal.eyeRange + bp.animal.ai_state + bp.animal.nextPoint_x;
            code = code + bp.player.nextPoint_y + bp.player.totalZhen;
            for (int i = 0; i < bp.player.buffs.Count; i++)
                code = code + bp.player.buffs[i];
            for (int i = 0; i < bp.player.buffs_time.Count; i++)
                code = code + bp.player.buffs_time[i];
        }

        for (int ll = 0; ll < BattlePlayers.penguin_list.Count; ll++)
        {
            var bp = BattlePlayers.penguin_list[ll];
            code = code + bp.player.birth_x + bp.player.birth_y + BattleOperation.string2int(bp.player.attack_guid);
            //unit
            code = code + BattleOperation.string2int(bp.animal.guid);
            code = code + bp.animal.role_id + bp.animal.x + bp.animal.y + bp.animal.r + bp.animal.r_py;
            if (bp.player.is_move)
                code = code + 1;
            code = code + bp.animal.re_tid + bp.animal.re_state + bp.animal.re_id + bp.animal.re_level + bp.animal.re_time;
            code = code + bp.animal.re_x + bp.animal.re_y + bp.animal.re_r;
            for (int i = 0; i < bp.player.save_re_id.Count; i++)
                code = code + bp.player.save_re_id[i];
            for (int i = 0; i < bp.player.save_re_zhen.Count; i++)
                code = code + bp.player.save_re_zhen[i];
            if (bp.player.is_jf)
                code = code + 1;

            code = code + bp.animal.jf_sx + bp.animal.jf_sy + bp.animal.jf_xx + bp.animal.jf_yy + bp.animal.jf_speed;
            code = code + bp.animal.jf_r + bp.animal.death_time + bp.animal.rtime + bp.animal.attackr;
            for (int i = 0; i < bp.player.bhit_tids.Count; i++)
                code = code + bp.player.bhit_tids[i];

            code = code + bp.animal.hp + bp.animal.is_ai + bp.animal.eyeRange + bp.animal.ai_state + bp.animal.nextPoint_x;
            code = code + bp.player.nextPoint_y + bp.player.totalZhen;
            for (int i = 0; i < bp.player.buffs.Count; i++)
                code = code + bp.player.buffs[i];
            for (int i = 0; i < bp.player.buffs_time.Count; i++)
                code = code + bp.player.buffs_time[i];
        }

        {//Boss
            var bp = BattlePlayers.Boss;
            code = code + bp.player.attack_state + bp.player.attackZhen + bp.player.boss_birth_time;
            //unit
            code = code + BattleOperation.string2int(bp.animal.guid);
            code = code + bp.animal.role_id + bp.animal.x + bp.animal.y + bp.animal.r + bp.animal.r_py;
            if (bp.player.is_move)
                code = code + 1;
            code = code + bp.animal.re_tid + bp.animal.re_state + bp.animal.re_id + bp.animal.re_level + bp.animal.re_time;
            code = code + bp.animal.re_x + bp.animal.re_y + bp.animal.re_r;
            for (int i = 0; i < bp.player.save_re_id.Count; i++)
                code = code + bp.player.save_re_id[i];
            for (int i = 0; i < bp.player.save_re_zhen.Count; i++)
                code = code + bp.player.save_re_zhen[i];
            if (bp.player.is_jf)
                code = code + 1;

            code = code + bp.animal.jf_sx + bp.animal.jf_sy + bp.animal.jf_xx + bp.animal.jf_yy + bp.animal.jf_speed;
            code = code + bp.animal.jf_r + bp.animal.death_time + bp.animal.rtime + bp.animal.attackr;
            for (int i = 0; i < bp.player.bhit_tids.Count; i++)
                code = code + bp.player.bhit_tids[i];

            code = code + bp.animal.hp + bp.animal.is_ai + bp.animal.eyeRange + bp.animal.ai_state + bp.animal.nextPoint_x;
            code = code + bp.player.nextPoint_y + bp.player.totalZhen;
            for (int i = 0; i < bp.player.buffs.Count; i++)
                code = code + bp.player.buffs[i];
            for (int i = 0; i < bp.player.buffs_time.Count; i++)
                code = code + bp.player.buffs_time[i];
        }

        for (int ll = 0; ll < BattlePlayers.effects_list.Count; ll++)
        {
            var be = BattlePlayers.effects_list[ll];
            code = code + be.effect.tid;
            code = code + be.effect.re_tid + be.effect.id + be.effect.sx + be.effect.sy + be.effect.x + be.effect.y;
            code = code + be.effect.xx + be.effect.yy + be.effect.r + BattleOperation.string2int(be.effect.re_guid);
            code = code + be.effect.re_ur;
            for (int i = 0; i < be.effect.hit_guids.Count; i++)
                code = code + BattleOperation.string2int(be.effect.hit_guids[i]);
            code = code + be.effect.time + be.effect.dd_time + be.effect.state + be.effect.len + be.effect.pre_zhen + be.effect.camp + BattleOperation.string2int(be.effect.follow_guid);
        }

        var itemDics = from obj in BattlePlayers.items orderby obj.Key select obj;
        foreach (KeyValuePair<int, battle_item> kvp in itemDics)
            code = code + kvp.Value.item.birth_pos + kvp.Value.item.tid + kvp.Value.item.id + kvp.Value.item.x + kvp.Value.item.y + kvp.Value.item.param + kvp.Value.item.zhen;

        if (code == 0)
            code = 1;
        msg.code = code;
        SendBattleUdp<msg_battle_code>(msg,opclient_t.CMSG_BATTLE_CODE);
    }

    public static byte[] get_string()
    {
        msg_battle_state msg = new msg_battle_state();
        msg.zhen = BattlePlayers.zhen;
        msg.tid = BattlePlayers.tid;
        msg.boss.Add(TypeTransform.toMsgBattleBoss(BattlePlayers.Boss.player));
        for (int i = 0; i < BattlePlayers.penguin_list.Count; i++)
            msg.monsters.Add(TypeTransform.toMsgBattleMonster(BattlePlayers.penguin_list[i].player));

        for (int i = 0; i < BattlePlayers.players_list.Count; i++)
            msg.players.Add(TypeTransform.toMsgBattlePlayer(BattlePlayers.players_list[i].player));

        for (int ll = 0; ll < BattlePlayers.effects_list.Count; ll++)
        {
            var bp = BattlePlayers.effects_list[ll];
            msg.effects.Add(bp.effect);
        }

        var itemDics = from obj in BattlePlayers.items orderby obj.Key select obj;
        var its = new List<battle_item_base>();
        foreach (KeyValuePair<int, battle_item> kvp in itemDics)
            its.Add(kvp.Value.item);

        List<msg_battle_item> items = null;
        List<msg_battle_item_base> bases = null;
        TypeTransform.toBattleItem(its, out items, out bases);
        foreach (var item in items)
            msg.items.Add(item);
        foreach (var item in bases)
            msg.bases.Add(item);

        msg.init_item = BattlePlayers.init_item;
        byte[] hash = null;
        using (MemoryStream ms = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize<msg_battle_state>(ms, msg);
            byte[] bs = new byte[ms.Length];
            ms.Position = 0L;
            ms.Read(bs, 0, bs.Length);
            var md5 = new MD5CryptoServiceProvider();
            hash = md5.ComputeHash(bs);
        }
        return hash;
    }

    public static void send_talent(int talent_id)
    {
        if (Battle.is_online)
        {
            var msg = new msg_battle_op();
            msg.opcode = (int)e_battle_msg.MSG_BATTLE_TALENT;
            msg.guid = Convert.ToUInt64(self.guid);
            msg.param_ints.Add(talent_id);
            Battle.SendBattleUdp<msg_battle_op>(msg, opclient_t.CMSG_BATTLE_OP);
        }
        else
            BattlePlayers.Talent1(BattlePlayers.me, talent_id);
    }

    public static void send_battle_inside_msg(int battle_inside_msg_id)
    {
        if (Battle.is_online)
        {
            var msg = new msg_battle_op();
            msg.opcode = (int)e_battle_msg.MSG_BATTLE_INSIDE_MSG;
            msg.guid = Convert.ToUInt64(self.guid);
            msg.param_ints.Add(battle_inside_msg_id);
            Battle.SendBattleUdp<msg_battle_op>(msg, opclient_t.CMSG_BATTLE_OP);
        }
        else
            BattlePlayers.BattleInsideMsg1(self.guid,battle_inside_msg_id);     
    }

    public static void send_release(int id, float x, float y, float r, int pzhen)
    {
        int _x = BattleOperation.toExactInt(x);
        int _y = BattleOperation.toExactInt(y);
        int _r = BattleOperation.toExactInt(r);
        if (Battle.is_online)
        {
            var msg = new msg_battle_op();
            msg.opcode = (int)e_battle_msg.MSG_BATTLE_RELEASE;
            msg.guid = Convert.ToUInt64(self.guid);
            msg.param_ints.Add(id);
            msg.param_ints.Add(_x);
            msg.param_ints.Add(_y);
            msg.param_ints.Add(_r);
            msg.param_ints.Add(pzhen);
            Battle.SendBattleUdp<msg_battle_op>(msg, opclient_t.CMSG_BATTLE_OP);
        }
        else
            BattlePlayers.Release1(BattlePlayers.me, id, _x,_y,_r);
    }

    public static void send_prerelease(int state)
    {
        if (Battle.is_online)
        {
            var msg = new msg_battle_op();
            msg.opcode = (int)e_battle_msg.MSG_BATTLE_PRERELEASE;
            msg.guid = Convert.ToUInt64(self.guid);
            msg.param_ints.Add(state);
            Battle.SendBattleUdp<msg_battle_op>(msg, opclient_t.CMSG_BATTLE_OP);
        }
        else
            BattlePlayers.PreRelease1(BattlePlayers.me, state);
    }

    private static bool need_send_attackr = false;
    private static int need_send_attackr_r = 0;
    public static void send_attackr(int r)
    {
        need_send_attackr = true;
        need_send_attackr_r = r;
    }

    public static void send_attackr_true()
    {
        if (!need_send_attackr)
        {
            return;
        }
        need_send_attackr = false;
        if (Battle.is_online)
        {
            var msg = new msg_battle_op();
            msg.opcode = (int)e_battle_msg.MSG_BATTLE_ATTACKR;
            msg.guid = Convert.ToUInt64(self.guid);
            msg.param_ints.Add(need_send_attackr_r);
            Battle.SendBattleUdp<msg_battle_op>(msg, opclient_t.CMSG_BATTLE_OP);
        }
        else
            BattlePlayers.Attackr1(BattlePlayers.me, need_send_attackr_r);
    }

    private static bool need_send_move_stop = false;
    private static bool need_send_move_stop_action = false;
    private static int need_send_move_stop_r = 0;

    public static void send_move_stop_true()
    {
        if (!need_send_move_stop)
        {
            return;
        }
        need_send_move_stop = false;
        if (!need_send_move_stop_action)
        {
            send_stop1();
        }
        else
        {
            send_move1();
        }
    }

    public static void send_stop(int r)
    {
        need_send_move_stop = true;
        need_send_move_stop_action = false;
        need_send_move_stop_r = r;
    }

    public static void send_stop1()
    {
        if (Battle.is_online)
        {
            var msg = new msg_battle_op();
            msg.opcode = (int)e_battle_msg.MSG_BATTLE_STOP;
            msg.guid = Convert.ToUInt64(self.guid);
            msg.param_ints.Add(need_send_move_stop_r);
            Battle.SendBattleUdp<msg_battle_op>(msg, opclient_t.CMSG_BATTLE_OP);
        }
        else
            BattlePlayers.Stop1(BattlePlayers.me, need_send_move_stop_r);
    }

    public static void send_move(int r)
    {
        need_send_move_stop = true;
        need_send_move_stop_action = true;
        need_send_move_stop_r = r;
    }

    public static void send_move1()
    {
        if (Battle.is_online)
        {
            var msg = new msg_battle_op();
            msg.opcode = (int)e_battle_msg.MSG_BATTLE_MOVE;
            msg.guid = Convert.ToUInt64(self.guid);
            msg.param_ints.Add(need_send_move_stop_r);
            Battle.SendBattleUdp<msg_battle_op>(msg, opclient_t.CMSG_BATTLE_OP);
        }
        else
            BattlePlayers.Move1(BattlePlayers.me, need_send_move_stop_r);
    }

    public static void send_reset()
    {
        BattlePlayers.Fini();
        if (Battle.is_online)
            Battle.SendBattleNullUdp(opclient_t.CMSG_BATTLE_RESET);
    }

    public static void send_in()
    {
        if (Battle.is_online)
            Battle.SendBattleNullUdp(opclient_t.CMSG_BATTLE_IN);
        else
        {
            var msg = new msg_battle_op();
            msg.zhen = BattlePlayers.zhen + 1;
            msg.opcode = (int)e_battle_msg.MSG_BATTLE_IN;
            msg.guid = Convert.ToUInt64(self.guid);
            msg.param_strings.Add(self.player.name);
            var role = self.get_role(self.player.role_on);
            msg.param_ints.Add(0);
            msg.param_ints.Add(role.template_id);
            msg.param_ints.Add(role.level);
            msg.param_ints.Add(self.player.sex);
            msg.param_ints.Add(self.player.avatar_on);
            msg.param_ints.Add(self.player.cup);
            msg.param_ints.Add(self.player.toukuang_on);
            msg.param_ints.Add(self.player.region_id);
            //if (self.player.region == "")
            //    msg.param_strings.Add("");
            //else
            //{
            //    string[] l = self.player.region.Split(' ');
            //    msg.param_strings.Add(l[0]);
            //}

            msg.param_ints.Add(10000000);
            msg.param_ints.Add(self.get_name_color());
            var index = msg.param_ints.Count;
            msg.param_ints.Add(0);
            int num = 0;
            for (int i = 0; i < self.roles.Count; i++)
            {
                var t_role = Config.get_t_role(self.roles[i].template_id);
                for (int j = 0; j < t_role.gskills.Count; j++)
                {
                    var t_role_buff = Config.get_t_role_buff(t_role.gskills[j]);
                    if (t_role_buff != null && t_role_buff.type < 3)
                    {
                        num = num + 1;  
                        msg.param_ints.Add(t_role_buff.type);
                        msg.param_ints.Add(t_role_buff.param1);
                        msg.param_ints.Add(t_role_buff.param2);
                        msg.param_ints.Add(t_role_buff.param3 + t_role_buff.param4 * (self.roles[i].level - 1));
                    }

                }
            }

            for (int i = 0; i < self.player.toukuang.Count; i++)
            {
                var t_toukuang = Config.get_t_toukuang(self.player.toukuang[i]);
                for (int j = 0; j < t_toukuang.gskills.Count; j++)
                {
                    if (t_toukuang.gskills[j].type < 3)
                    {
                        num = num + 1;
                        msg.param_ints.Add(t_toukuang.gskills[j].type);
                        msg.param_ints.Add(t_toukuang.gskills[j].param1);
                        msg.param_ints.Add(t_toukuang.gskills[j].param2);
                        msg.param_ints.Add(t_toukuang.gskills[j].param3);
                    }
                }
            }

            msg.param_ints[index] = num;
            msg.param_ints.Add(self.player.fashion_on.Count);
            for(int i = 0;i < self.player.fashion_on.Count;i++)
                msg.param_ints.Add(self.player.fashion_on[i]);


            if (Battle.self_skills != null)
            {
                for (int i = 0; i < Battle.self_skills.Count; i++)
                    msg.param_ints.Add(Battle.self_skills[i]);
            }

            BattlePlayers.AddOperation(msg);
        }
    }

    public static void LanGan()
    {
        //if (Battle.is_new == 1)
        //{
        //    GUIRoot.ShowGUI<ForeBattle>("ForeBattle", Battle.self_skills);
        //}
        //else
        {
            Battle.send_in();
            GUIRoot.ShowGUI<BattlePanel>("BattlePanel");
        }
    }

    public static void ShowBattlePanel()
    {
        GUIRoot.ShowGUI<BattlePanel>("BattlePanel");
    }

    public static void send_change_skill()
    {
        if (Battle.is_online)
        {
            var msg = new msg_battle_op();
            msg.opcode = (int)e_battle_msg.MSG_BATTLE_CHANGE_SKILL;
            msg.guid = Convert.ToUInt64(self.guid);
            Battle.SendBattleUdp<msg_battle_op>(msg, opclient_t.CMSG_BATTLE_OP);
        }
        //else
        //    BattlePlayers.ChangeSkill1(BattlePlayers.me);
    }
    public static List<int> RandomForeBattleMsg()
    {
        List<int> talents = new List<int>();
        List<int> skills = new List<int>();
        List<int> result = new List<int>();
        foreach (var item in Config.t_talents)
            talents.Add(item.Value.id);
        System.Random r = new System.Random((int)DateTime.Now.Ticks);
        foreach (var item in Config.t_skills)
        {
            var t_skill = Config.get_t_skill(item.Key);
            if (t_skill.type == 2 && t_skill.id != 400201 && t_skill.id != 400301)
                skills.Add(t_skill.id);
        }

        int p = r.Next(0, talents.Count);
        result.Add(talents[p]);
        for (int i = 1; i <= 2; i++)
        {
            p = r.Next(0, skills.Count);
            result.Add(skills[p]);
        }

        while (result[1] == result[2])
        {
            result[2] = skills[r.Next(0, skills.Count)];
        }

        return result;
    }
   
    public static void SMSG_BATTLE_LINK(s_net_message message)
    {
        var msg = new smsg_battle_link();
        using (MemoryStream ms = new MemoryStream(message.buffer))
        {
            msg = ProtoBuf.Serializer.Deserialize<smsg_battle_link>(ms);
        }
        BattlePlayers.Init(msg.battle_guid.ToString());
        BattlePlayers.max_zhen = msg.zhen - 1;
        BattlePlayers.seed = msg.seed;
        BattlePlayers.seed_add = msg.seed_add;
        BattlePlayers.battle_type = msg.type;
        BattlePlayers.self_camp = msg.self_camp;
        BattlePlayers.jiasu = true;
        BattlePlayers.team_num = msg.team_num;
        BattlePlayers.member_num = msg.member_num;

        if (BattlePlayers.battle_guid != null)
        {
            if (BattlePlayers.battle_type == 0)
                BattlePlayers.channel = BattlePlayers.battle_guid;
            else
                BattlePlayers.channel = BattlePlayers.battle_guid + "_" + BattlePlayers.self_camp;
        }
        var state = msg.state;
        if (msg.is_state == 2)
        {
            BattlePlayers.zhen = state.zhen;
            for (int i = 0; i < state.players.Count; i++)
                BattlePlayers.addplayer(TypeTransform.toBattlePlayer(state.players[i]));
            for (int i = 0; i < state.monsters.Count; i++)
                BattlePlayers.addmonster(TypeTransform.toBattleMonster(state.monsters[i]));
            for (int i = 0; i < state.boss.Count; i++)
                BattlePlayers.addboss(TypeTransform.toBattleBoss(state.boss[i]));

            for (int i = 0; i < state.effects.Count; i++)
                BattlePlayers.add_effect2(state.effects[i]);

            List<battle_item_base> bib = TypeTransform.toBattleItem(state.items, state.bases);
            for (int i = 0; i < bib.Count; i++)
                BattlePlayers.add_item2(bib[i]);

            for (int i = 0; i < msg.ops.Count; i++)
                BattlePlayers.AddOperation(msg.ops[i]);

            BattlePlayers.change_player_list();
            BattlePlayers.change_effect_list();
        }
        else if (msg.is_state == 1)
        {
            BattlePlayers.zhen = state.zhen;
            for (int i = 0; i < msg.ops.Count; i++)
                BattlePlayers.AddOperation(msg.ops[i]);
        }
        else
            BattlePlayers.zhen = state.zhen;

        BattlePlayers.tid = state.tid;
        BattlePlayers.init_item = state.init_item;
        BattlePlayers.dtime = 0;
        Battle.is_new = msg.is_new;
        BattleAchieve.Init();
        BattlePlayerAI.Init();
    }

    public static void SMSG_BATTLE_OP(s_net_message message)
    {
        var msg = new msg_battle_op();
        using (MemoryStream ms = new MemoryStream(message.buffer))
        {
            msg = ProtoBuf.Serializer.Deserialize<msg_battle_op>(ms);
        }
        BattlePlayers.AddOperation(msg);
    }

    public static void SMSG_BATTLE_ZHEN(s_net_message message)
    {
        BattlePlayers.max_zhen = BattlePlayers.max_zhen + 1;
    }

    public static void SMSG_BATTLE_FINISH(s_net_message message)
    {
        battle_panel.show_end();
        Battle.send_result();
        Battle.is_end = true;
        Time.timeScale = 1;
        BattleAchieve.BattleFinish();
    }

    public static find_player GetNear(long range)
    {    
        var bp = BattlePlayers.me;
        bool flag = false;
        long d = 0;
        BattleAnimal tbp = null;
       
        if (bp.player.re_pre_zhen <= 10)
        {
            for (int ll = 0; ll < BattlePlayers.players_list.Count; ll++)
            {
                var bp1 = BattlePlayers.players_list[ll];
                if (bp1.animal.guid != self.guid && bp.player.camp != bp1.animal.camp && BattleOperation.can_see(bp, bp1) && !bp.attr.is_hunluan() && !bp.attr.is_zhimang() && !bp1.is_die && !bp1.animal.is_xueren)
                {
                    var dd = BattleOperation.get_distance2(bp.player.x, bp.player.y, bp1.animal.x, bp1.animal.y);
                    if (dd <= range * range)
                    {
                        if (!flag)
                        {
                            flag = true;
                            d = dd;
                            tbp = bp1;
                        }
                        else if (dd < d)
                        {
                            d = dd;
                            tbp = bp1;
                        }
                    }
                }
            }

            if (tbp == null && BattlePlayers.Boss != null)
            {
                var dis = BattleOperation.get_distance2(bp.player.x, bp.player.y, BattlePlayers.Boss.player.x, BattlePlayers.Boss.player.y);
                if (dis <= range * range)
                {
                    d = dis;
                    flag = true;
                    tbp = BattlePlayers.Boss;
                }
            }

            if (tbp == null)
            {
                for (int ll = 0; ll < BattlePlayers.penguin_list.Count; ll++)
                {
                    var bp1 = BattlePlayers.penguin_list[ll];
                    if (BattleOperation.can_see(bp, bp1) && !bp.attr.is_hunluan() && !bp.attr.is_zhimang() && !bp1.is_die && !bp1.animal.is_xueren)
                    {
                        var dd = BattleOperation.get_distance2(bp.player.x, bp.player.y, bp1.animal.x, bp1.animal.y);
                        if (dd <= range * range)
                        {
                            if (!flag)
                            {
                                flag = true;
                                d = dd;
                                tbp = bp1;
                            }
                            else if (dd < d)
                            {
                                d = dd;
                                tbp = bp1;
                            }
                        }
                    }
                }
            }
        }
        
        find_player near = new find_player();
        if (flag)
        {
            var dx = tbp.animal.x - bp.animal.x;
            var dy = tbp.animal.y - bp.animal.y;
            near.r = BattleOperation.toInt(Math.Atan2(dy, dx) * 180 / Math.PI);
            near.tbp = tbp;
        }
        else
        {
            near.r = bp.player.r;
            near.tbp = null;
        }
        return near;
    }

    public static void send_result()
    {
        if (Battle.is_online)
        {
            battle_result_t msg = new battle_result_t();
            msg.guid = 0;
            msg.type = BattlePlayers.battle_type;

            List<BattleAnimalPlayer> players = new List<BattleAnimalPlayer>();
            if (BattlePlayers.battle_type == 1)
                players = battle_panel.TeamListRank;
            else
            {
                for (int i = 0; i < BattlePlayers.players_list.Count; i++)
                {
                    var bp = BattlePlayers.players_list[i];
                    players.Add(bp);
                }
                players.Sort((a, b) =>
                {
                    return b.player.score - a.player.score;
                });
            }

            for (int i = 0; i < players.Count; i++)
            {
                var bp = players[i];
                msg.player_guids.Add(Convert.ToUInt64(bp.player.guid));
                msg.names.Add(bp.player.name);
                msg.role_ids.Add(bp.player.role_id);
                msg.sexs.Add(bp.player.sex);
                msg.avatars.Add(bp.player.avatar);
                msg.ranks.Add(i + 1);
                msg.shas.Add(bp.player.sha);
                msg.lshas.Add(bp.player.max_lsha);
                msg.dies.Add(bp.player.die);
                msg.scores.Add(bp.player.score);
                msg.cups.Add(bp.player.cup);
                msg.cup_adds.Add(1);

                int value = 0;
                if (battle_panel.mvpList.Contains(bp.player.guid))
                    value = value + 1;

                if (battle_panel.cgList.Contains(bp.player.guid))
                    value = value + 2;

                if (battle_panel.max_killList.Contains(bp.player.guid))
                    value = value + 4;

                msg.achieves.Add(value);
            }

            SendBattleUdp<battle_result_t>(msg, opclient_t.CMSG_BATTLE_END);
        }
    }

    public static void SMSG_GUIDE(s_net_message s)
    {
        NewPlayerGuide.SMSG_GUIDE();
        Battle.exTime = BattlePlayers.max_zhen;
        Battle.is_newplayer_guide = false;
        //BattlePlayers.Boss.player.boss_birth_time = BattlePlayers.zhen;
        self.player.is_guide = 0;
        Util.CallMethod("State", "set_playerguide", 0);
    }

    public static void ChangeSkill(int pos)
    {
        if (pos == 1)
        {
            if (is_hold_["jskill"])
            {
                if (BattlePlayers.me != null)
                {
                    var bp = BattlePlayers.me;
                    var t_skill = Config.get_t_skill(bp.player.skill_id, bp.player.skill_level, bp.get_skill_level_add(bp.player.skill_id));
                    if (t_skill != null)
                        Debug.Log(" skill.release_type " + t_skill.release_type + "t_skill.get_range(bp)" + t_skill.get_range(bp) + "t_skill.get_range_param(bp)" + t_skill.get_range_param(bp));
                        BattleSkillRange.Create("jskill", t_skill.release_type, t_skill.get_range(bp), t_skill.get_range_param(bp));
                }
            }
        }
    }
    public static void hreturn()
    {
        if (Battle.is_newplayer_guide)
        {
            NewPlayerGuide.CMSG_GUIDE(true);
            GUIRoot.GetPanel("BattlePanel").GetComponent<BattlePanel>().guide_skip_panel_.gameObject.SetActive(false);
        }
        else
        {
            if (Battle.is_online)
                Util.CallMethod("BattleTcp", "Disconnect2");
            Util.CallMethod("State", "ChangeState", 2);
        }
    }
    public static void SendBattleTcp<T>(T t, opclient_t opcode)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize<T>(ms, t);
            byte[] bs = new byte[ms.Length];
            ms.Position = 0L;
            ms.Read(bs, 0, bs.Length);
            LuaHelper.GetNetManager().SendMessage("BattleStateTcp", (int)opcode, bs);
        }
    }
    public static void SendBattleUdp<T>(T t, opclient_t opcode)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize<T>(ms, t);
            byte[] bs = new byte[ms.Length];
            ms.Position = 0L;
            ms.Read(bs, 0, bs.Length);
            LuaHelper.GetNetManager().SendMessage("BattleTcp", (int)opcode, bs);
        }
    }

    public static void SendBattleNullUdp(opclient_t opcode)
    {
        LuaHelper.GetNetManager().SendMessageNull("BattleTcp", (int)opcode);
    }
    public static void SendCircle<T>(opclient_t opcode,T t,List<string> smsgs = null, string text = "", double time = 10)
    {
        if (smsgs != null && smsgs.Count > 0)
        {
            StringBuilder s = new StringBuilder();
            s.Append("client_msg_param = {");
            for (int i = 0; i < smsgs.Count; i++)
            {
                s.Append(smsgs[i]);

                if (i != smsgs.Count - 1)
                    s.Append(",");
            }
            s.Append("}");
            LuaInterface.LuaTable tt = AppFacade._instance.LuaManager.GetLuaTable(s.ToString(), "client_msg_param");
            Util.CallMethod("Message", "OnMask", tt, text, time);
        }
        if (t == null)
            SendBattleNullUdp(opcode);
        else
            SendBattleUdp<T>(t, opcode);
    }
}

public static class GameTcp
{
    public static void Send<T>(opclient_t opcode,T t)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize<T>(ms, t);
            byte[] bs = new byte[ms.Length];
            ms.Position = 0L;
            ms.Read(bs, 0, bs.Length);
            LuaHelper.GetNetManager().SendMessage("GameTcp", (int)opcode, bs);
        }   
    }

    public static void SendNull(opclient_t opcode)
    {
        LuaHelper.GetNetManager().SendMessageNull("GameTcp", (int)opcode);
    }

    public static void SendCircle<T>(opclient_t opcode,T t,List<string> smsgs = null,string text = "",double time = 10)
    {
        if (smsgs != null && smsgs.Count > 0)
        {
            StringBuilder s = new StringBuilder();
            s.Append("circle_param = {");
            for (int i = 0; i < smsgs.Count; i++)
            {
                s.Append(smsgs[i]);

                if (i != smsgs.Count - 1)
                    s.Append(",");
            }
            s.Append("}");
            LuaInterface.LuaTable tt = AppFacade._instance.LuaManager.GetLuaTable(s.ToString(), "circle_param");
            Util.CallMethod("Message", "OnMask", tt, text, time);
        }

        if (t == null)
            SendNull(opcode);
        else
            Send<T>(opcode,t);   
    }
}

