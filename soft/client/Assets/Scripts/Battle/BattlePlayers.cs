using BattleDB;
using ProtoBuf;
using protocol.game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public enum e_battle_msg
{
    MSG_BATTLE_IN = 0,
    MSG_BATTLE_OUT,
    MSG_BATTLE_STATE,
    MSG_BATTLE_CODE,

    MSG_BATTLE_MOVE = 100,
    MSG_BATTLE_STOP,
    MSG_BATTLE_PRERELEASE,
    MSG_BATTLE_RELEASE,
    MSG_BATTLE_FUHUO,
    MSG_BATTLE_TALENT,
    MSG_BATTLE_CHANGE_SKILL,
    MSG_BATTLE_ATTACKR,
    MSG_BATTLE_INSIDE_MSG
}

public class BattlePlayers
{
    public static List<GameObject> ItemMarksList = new List<GameObject>();
    public static Dictionary<string, BattleAnimal> players = new Dictionary<string, BattleAnimal>();
    public static List<BattleAnimalPlayer> players_list = new List<BattleAnimalPlayer>();
    public static bool players_list_change = false;
    public static Dictionary<int, battle_effect> effects = new Dictionary<int, battle_effect>();
    public static List<battle_effect> effects_list = new List<battle_effect>();
    public static bool effects_list_change = false;
    public static Dictionary<int, battle_item> items = new Dictionary<int, battle_item>();
    public static Dictionary<int, battle_item> item_follows = new Dictionary<int, battle_item>();
    public static List<battle_item> item_speeds = new List<battle_item>();
    public static Dictionary<int, int> item_num = new Dictionary<int, int>();
    public static bool init_item = false;
    public static battle_message_link operations_head = null;
    public static battle_message_link operations_tail = null;
    public static int zhen;
    public static int max_zhen;
    public static int battle_type;
    public static int self_camp;
    public static bool start;
    public static bool jiasu;
    public static int tid;
    public static int dtime;
    public static int seed;
    public static int seed_add;
    public static int team_num;
    public static int member_num;
    public const int TICK = 50; //每帧的ms
    public const int TNUM = 20; //每秒跑的帧数
    public static BattleAnimalPlayer me = null;
    public static int dis = 170;
    public static bobjpool bobjpool = null;
    public static string battle_guid;
    public static string channel;
    public static Dictionary<int, List<string>> Camps = new Dictionary<int, List<string>>();
    public static BattleAnimalBoss Boss = null;
    public static List<BattleAnimalMonster> penguin_list = new List<BattleAnimalMonster>();
    public static int POWERUP = 300;

    //Boss >= 1000 < 2000   企鹅 >=3000 <4000  其他 >=5000
    public static void Init(string _battle_guid)
    {
        battle_guid = _battle_guid;
        players = new Dictionary<string, BattleAnimal>();
        players_list = new List<BattleAnimalPlayer>();
        players_list_change = false;
        effects = new Dictionary<int, battle_effect>();
        effects_list = new List<battle_effect>();
        effects_list_change = false;
        items = new Dictionary<int, battle_item>();
        item_follows = new Dictionary<int, battle_item>();
        item_speeds = new List<battle_item>();
        item_num = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5,0} };
        init_item = false;
        operations_head = null;
        operations_tail = null;
        zhen = 0;
        max_zhen = -1;
        battle_type = 0;
        self_camp = 0;
        team_num = 0;
        member_num = 0;
        start = true;
        jiasu = false;
        me = null;
        dis = 170;
        channel = null;
        BattleGrid.Init(Battle.name);
        bobjpool = new bobjpool();
        Camps = new Dictionary<int, List<string>>();
        Boss = null;
        penguin_list = new List<BattleAnimalMonster>();
        BattlePlayers.InitGroundEffect();
        GC.Collect();
    }

    private static void InitGroundEffect()
    {
        ItemMarksList.Clear();
        foreach (var item in Config.t_battle_refresh_items)
        {
            var obj = LuaHelper.GetResManager().CreateEffect("ItemMark");
            obj.transform.parent = LuaHelper.GetResManager().UnitRoot;
            obj.transform.localPosition = new Vector3(item.Value.pos_x * 1.0f / Battle.BL, 0.1f, item.Value.pos_y * 1.0f / Battle.BL);
            obj.transform.localScale = Vector3.one;
            ItemMarksList.Add(obj);
        }
    }

    public static void Fini()
    {
        if (bobjpool != null)
        {
            bobjpool.clear();
        }
        Battle.battle_panel.clear_min_pro();
        BattleGrid.Fini();
        BattlePlayerAI.Fini();

        for (int i = 0; i < players_list.Count; i++)
        {
            var bp = players_list[i];
            GameObject.Destroy(bp.posobj);
        }

        for (int i = 0; i < penguin_list.Count; i++)
        {
            var bp = penguin_list[i];
            GameObject.Destroy(bp.posobj);
        }
        players.Clear();
        players_list.Clear();
        penguin_list.Clear();
        players_list_change = false;

        for (int i = 0; i < effects_list.Count; i++)
        {
            var be = effects_list[i];
            LuaHelper.GetResManager().DeleteEffect(be.obj);
        }
        effects.Clear();
        effects_list.Clear();
        effects_list_change = false;

        foreach (var item in items)
            LuaHelper.GetResManager().DeleteEffect(item.Value.obj);
        items.Clear();

        foreach (var item in item_follows)
            LuaHelper.GetResManager().DeleteEffect(item.Value.obj);
        item_follows.Clear();

        start = false;
        Time.timeScale = 1;

        if (Boss != null)
            GameObject.Destroy(Boss.posobj);

        for (int i = ItemMarksList.Count - 1; i >= 0; i--)
            GameObject.Destroy(ItemMarksList[i]);
        ItemMarksList.Clear();


        GC.Collect();
    }
    public static void init_boss(battle_boss player)
    {
        var p = BattleOperation.get_avilialbe_xy();
        player.x = p[0];
        player.y = p[1];
        player.r = 0;
        player.r_py = 0;
        player.is_move = false;
        player.re_state = 0;
        player.re_id = 0;
        player.re_time = 0;
        player.re_x = 0;
        player.re_y = 0;
        player.re_r = 0;
        player.save_re_id.Clear();
        player.save_re_zhen.Clear();
        player.is_jf = false;
        player.jf_sx = 0;
        player.jf_sy = 0;
        player.jf_xx = 0;
        player.jf_yy = 0;
        player.jf_speed = 0;
        player.jf_r = 0;
        player.bhit_tids.Clear();
        player.death_time = 0;
        player.attackr = 0;

        player.rtime = -100;

        player.hp = 0;
        player.buffs.Clear();
        player.buffs_time.Clear();

        player.attack_state = 0;
        player.attackZhen = 0;
        player.boss_birth_time = 0;
    }
    public static void init_monster(battle_monster player)
    {
        var p = BattleOperation.get_avilialbe_xy();
        player.x = p[0];
        player.y = p[1];
        player.r = 0;
        player.r_py = 0;
        player.is_move = false;
        player.re_state = 0;
        player.re_id = 0;
        player.re_time = 0;
        player.re_x = 0;
        player.re_y = 0;
        player.re_r = 0;
        player.save_re_id.Clear();
        player.save_re_zhen.Clear();
        player.is_jf = false;
        player.jf_sx = 0;
        player.jf_sy = 0;
        player.jf_xx = 0;
        player.jf_yy = 0;
        player.jf_speed = 0;
        player.jf_r = 0;
        player.bhit_tids.Clear();
        player.death_time = 0;
        player.attackr = 0;

        player.skill_id = 0;
        player.skill_level = 0;
        player.rtime = -100;

        player.hp = 0;
        player.buffs.Clear();
        player.buffs_time.Clear();

        player.birth_x = 0;
        player.birth_y = 0;
        player.attack_guid = "";
    }
    public static void init_player(battle_player player)
    {
        var p = BattleOperation.get_avilialbe_xy();
        player.x = p[0];
        player.y = p[1];
        player.type = unit_type.Player;
        player.r = 0;
        player.r_py = 0;
        player.is_move = false;
        player.re_state = 0;
        player.re_id = 0;
        player.re_time = 0;
        player.re_x = 0;
        player.re_y = 0;
        player.re_r = 0;
        player.re_pre = 0;
        player.re_pre_zhen = 0;
        player.save_re_id.Clear();
        player.save_re_zhen.Clear();
        player.is_xueren = false;
        player.is_jf = false;
        player.jf_sx = 0;
        player.jf_sy = 0;
        player.jf_xx = 0;
        player.jf_yy = 0;
        player.jf_speed = 0;
        player.jf_r = 0;
        player.bhit_tids.Clear();
        player.death_time = 0;
        player.exp = 0;
        player.score = 0;
        player.cost = 0;

        player.score_level = 1;
        player.skill_id = 0;
        player.skill_level = 0;
        player.rtime = -100;
        player.level = 1;
        
        player.hp = 0;
        player.buffs.Clear();
        player.buffs_time.Clear();
        player.lattr_value.Clear();
        for (int i = 0; i < 9; i++)
            player.lattr_value.Add(0);
        player.talent_id.Clear();
        player.talent_level.Clear();
        player.talent_point = 0;
    }
    public static void init_boss1(BattleAnimalBoss player)
    {
        var p = BattleOperation.get_avilialbe_xy();
        var boss_index = BattleOperation.random(0, Config.t_boss_list.Count);
        var t_boss_attr = Config.t_boss_list[boss_index];

        player.animal.x = t_boss_attr.birth_x;
        player.animal.y = t_boss_attr.birth_y;
        player.player.boss_birth_time = BattlePlayers.zhen;

        player.animal.r = 0;
        player.animal.r_py = 0;
        player.animal.is_move = false;
        player.animal.re_state = 0;
        player.animal.re_id = 0;
        player.animal.re_time = 0;
        player.animal.re_x = 0;
        player.animal.re_y = 0;
        player.animal.re_r = 0;

        player.animal.save_re_id.Clear();
        player.animal.save_re_zhen.Clear();

        player.animal.is_jf = false;
        player.animal.jf_sx = 0;
        player.animal.jf_sy = 0;
        player.animal.jf_xx = 0;
        player.animal.jf_yy = 0;
        player.animal.jf_speed = 0;
        player.animal.jf_r = 0;
        player.animal.bhit_tids.Clear();
        player.animal.death_time = 0;

        player.animal.hp = 0;
        player.animal.buffs.Clear();
        player.animal.buffs_time.Clear();
        player.player.attack_state = 0;
        player.player.attackZhen = 0;
    }
    public static void init_monster1(BattleAnimalMonster player)
    {
        var p = BattleOperation.get_avilialbe_xy();
        player.animal.x = p[0];
        player.animal.y = p[1];
        player.player.birth_x = p[0];
        player.player.birth_y = p[1];

        player.animal.r = 0;
        player.animal.r_py = 0;
        player.animal.is_move = false;
        player.animal.re_state = 0;
        player.animal.re_id = 0;
        player.animal.re_time = 0;
        player.animal.re_x = 0;
        player.animal.re_y = 0;
        player.animal.re_r = 0;

        player.animal.save_re_id.Clear();
        player.animal.save_re_zhen.Clear();

        player.animal.is_jf = false;
        player.animal.jf_sx = 0;
        player.animal.jf_sy = 0;
        player.animal.jf_xx = 0;
        player.animal.jf_yy = 0;
        player.animal.jf_speed = 0;
        player.animal.jf_r = 0;
        player.animal.bhit_tids.Clear();
        player.animal.death_time = 0;

        player.animal.hp = 0;
        player.animal.buffs.Clear();
        player.animal.buffs_time.Clear();
        player.player.attack_guid = "";
    }
    public static void init_player1(BattleAnimalPlayer player)
    {
        if (Battle.is_newplayer_guide && BattlePlayers.me.player.guid == player.animal.guid)
        {
            player.animal.x = 395000;
            player.animal.y = 395000;
            player.player.birth_x = 395000;
            player.player.birth_y = 395000;
        }
        else
        {
            var p = BattleOperation.get_avilialbe_xy();

            player.animal.x = p[0];
            player.animal.y = p[1];
            player.player.birth_x = p[0];
            player.player.birth_y = p[1];
        }
       
       
        player.animal.r = 0;
        player.animal.r_py = 0;
        player.animal.is_move = false;
        player.animal.re_state = 0;
        player.animal.re_id = 0;
        player.animal.re_time = 0;
        player.animal.re_x = 0;
        player.animal.re_y = 0;
        player.animal.re_r = 0;
        player.player.re_pre = 0;
        player.player.re_pre_zhen = 0;
        player.animal.save_re_id.Clear();
        player.animal.save_re_zhen.Clear();
        player.player.is_xueren = false;
        player.animal.is_jf = false;
        player.animal.jf_sx = 0;
        player.animal.jf_sy = 0;
        player.animal.jf_xx = 0;
        player.animal.jf_yy = 0;
        player.animal.jf_speed = 0;
        player.animal.jf_r = 0;
        player.animal.bhit_tids.Clear();
        player.animal.death_time = 0;
        if (player.player.skill_id != 0 && player.player.skill_level > 1)
            player.player.skill_level = player.player.skill_level - 1;
        player.animal.hp = 0;
       
        player.animal.buffs.Clear();
        player.animal.buffs_time.Clear();

        player.player.attack_state = 0;
        player.player.attackZhen = 0;
        player.player.attack_guid = "";
        player.player.cost = 0;
    }

    public static void In(msg_battle_op msg)
    {
        var player = new battle_player();
        BattlePlayers.init_player(player);
        player.guid = msg.guid.ToString();
        player.name = msg.param_strings[0];
        player.camp = msg.param_ints[0];
        player.role_id = msg.param_ints[1];
        player.role_level = msg.param_ints[2];
        player.sex = msg.param_ints[3];
        player.avatar = msg.param_ints[4];
        player.cup = msg.param_ints[5];
        player.toukuang = msg.param_ints[6];
        player.region_id = msg.param_ints[7];  //msg.param_strings[1];
        player.re_tid = msg.param_ints[8];
        player.name_color = msg.param_ints[9];
        player.is_ai = 0;
        var num = msg.param_ints[10];
        for (int i = 0; i < num; i++)
        {
            player.attr_type.Add(msg.param_ints[11 + i * 4]);
            player.attr_param1.Add(msg.param_ints[12 + i * 4]);
            player.attr_param2.Add(msg.param_ints[13 + i * 4]);
            player.attr_param3.Add(msg.param_ints[14 + i * 4]);
        }
        int new_index = num * 4 + 11;
        num = msg.param_ints[new_index];

        for (int i = 0; i < num; i++)
            player.fashion.Add(msg.param_ints[new_index + 1 + i]);

        BattlePlayers.addplayer(player, true);
        BattlePlayers.change_player_list();
    }

    public static void InitAIBaseInfo(battle_player player)
    {
        player.ai_state = 0;
        player.eyeRange = BattleOperation.random(30000, 50000);
        player.nextPoint_x = 0;
        player.nextPoint_y = 0;
        player.totalZhen = 0;
        player.attack_state = 0;
        player.attackZhen = 0;
        player.birth_x = player.x;
        player.birth_y = player.y;
        player.attack_guid = "";
    }

    public static void InitOtherAIPos()
    {
        for (int i = 0;i < BattlePlayers.players_list.Count;i++)
        {
            var p = BattleOperation.get_avilialbe_xy();
            var bp = BattlePlayers.players_list[i];
            if (bp.animal.is_ai > 0)
            {
                bp.animal.x = p[0];
                bp.animal.y = p[1];
                bp.player.birth_x = p[0];
                bp.player.birth_y = p[1];
            }
        }
    }

    public static void add_ai(int id, int ai_level)
    {
        battle_player player = new battle_player();
        BattlePlayers.init_player(player);
        player.guid = id.ToString();
        player.name = Config.get_battle_random_name();
        if (!Battle.is_online || BattlePlayers.battle_type == 0)
            player.camp = id - 1;
        else
            player.camp = (id - 1) / BattlePlayers.member_num;

        if (Battle.is_newplayer_guide)
            player.role_id = 1004;
        else
            player.role_id = BattleOperation.random(1001, 1009);
        player.role_level = 1;
        player.sex = Config.get_t_role(player.role_id).sex;
        player.avatar = 1010000 + player.role_id;
        player.cup = BattleOperation.random(0, 30);
        player.toukuang = Config.get_battle_random_toukuang();
        // player.region_id = Config.get_random_region().id;
        player.region_id = Config.get_t_foregion(0).id;
        player.re_tid = id * 100000;
        player.is_ai = ai_level;
        player.type = unit_type.Player;
        BattlePlayers.InitAIBaseInfo(player);
        BattlePlayers.addplayer(player, true);
    }

    public static void addbossUnit()
    {
        var boss_index = BattleOperation.random(0,Config.t_boss_list.Count);
        var t_boss_attr = Config.t_boss_list[boss_index];
        var player = new battle_boss();
        BattlePlayers.init_boss(player);
        player.x = 18000000;
        player.y = 18000000;
        player.boss_id = t_boss_attr.id;
        player.camp = t_boss_attr.id;
        player.type = unit_type.Boss;
        player.guid = t_boss_attr.id.ToString();
        player.name = Config.get_t_role(t_boss_attr.role_id).name;
        player.role_id = t_boss_attr.role_id;
        player.role_level = 1;
        player.re_tid = 100000 * 21;
        player.ai_state = 0;
        player.eyeRange = t_boss_attr.max_dis;
        player.nextPoint_x = 0;
        player.nextPoint_y = 0;
        player.totalZhen = 0;
        player.attackZhen = 0;
        BattlePlayers.addboss(player,true);
    }
    public static void addSpecBossUnit(int boss_id)
    {
        var t_boss_attr = Config.get_t_boss_attr(boss_id);
        var player = new battle_boss();
        BattlePlayers.init_boss(player);
        player.x = t_boss_attr.birth_x;
        player.y = t_boss_attr.birth_y;
        player.boss_id = t_boss_attr.id;
        player.camp = t_boss_attr.id;
        player.type = unit_type.Boss;
        player.guid = t_boss_attr.id.ToString();
        player.name = Config.get_t_role(t_boss_attr.role_id).name;
        player.role_id = t_boss_attr.role_id;
        player.role_level = 1;
        player.re_tid = 100000 * 21;
        player.ai_state = 0;
        player.eyeRange = t_boss_attr.max_dis;
        player.nextPoint_x = 0;
        player.nextPoint_y = 0;
        player.totalZhen = 0;
        player.attackZhen = 0;
        player.boss_birth_time = BattlePlayers.zhen;
        BattlePlayers.addboss(player, true);
    }

    public static void addpenguin()
    {
        foreach (var item in Config.t_penguin_nums)
        {
            for (int i = 0; i < item.Value.num; i++)
            {
                t_penguin pg = Config.get_random_t_penguin(item.Key);
                var player = new battle_monster();
                BattlePlayers.init_monster(player);
                player.guid = (pg.id + 100 * i).ToString();
                player.type = unit_type.Monster;
                player.monster_id = pg.id;
                player.name = pg.name;
                player.camp = -1;
                player.role_id = pg.role_id;
                player.role_level = 1;
                player.re_tid = 100000 * (22 + i);
                player.is_ai = pg.id;
                player.ai_state = 0;
                player.eyeRange = pg.view_radius;
                player.nextPoint_x = 0;
                player.nextPoint_y = 0;
                player.totalZhen = 0;
                player.birth_x = player.x;
                player.birth_y = player.y;
                player.death_time = 0;
                player.skill_id = pg.skill_id;
                player.skill_level = 1;
                BattlePlayers.addmonster(player,true);
            }
        }
    }
    public static bool BossIsActive()
    {
        if (Boss == null)
            return false;
        else
            return (!BattlePlayers.Boss.is_die);
    }
    public static void addmonster(battle_monster monster,bool is_new = false)
    {
        monster.r = BattleOperation.checkr(monster.r);
        var t_role = Config.get_t_role(monster.role_id);
        if (t_role == null)
            return;

        var obj = LuaHelper.GetResManager().CreateUnit(t_role.res, false);
        int objid = BattlePlayers.bobjpool.add(obj);
        var posobj = new GameObject();
        int posobjid = BattlePlayers.bobjpool.add(posobj);
        Transform posobjt = posobj.transform;

        posobjt.parent = LuaHelper.GetResManager().UnitRoot;
        BattlePlayers.bobjpool.set_localPosition(posobjid, monster.x * 1.0f / Battle.BL, 0, monster.y * 1.0f / Battle.BL);
        BattlePlayers.bobjpool.set_localEulerAngles(posobjid, 0, 0, 0);
        BattlePlayers.bobjpool.set_localScale(posobjid, 1, 1, 1);

        var objt = obj.transform;
        obj.transform.parent = posobj.transform;
        BattlePlayers.bobjpool.set_localPosition(objid, 0, 0, 0);
        BattlePlayers.bobjpool.set_localEulerAngles(objid, 0, 90 - monster.r, 0);
        BattlePlayers.bobjpool.set_localScale(objid, 1, 1, 1);
        var cao = BattleGrid.get_cao(monster.x, monster.y);
        var unit = obj.GetComponent<unit>();
        Transform accept = unit.get_bone("accept");

        GameObject shadow_res = null;
        shadow_res = LuaHelper.GetResManager().CreateEffect("chr_showed_obj");
        shadow_res.transform.parent = posobj.transform;
        shadow_res.transform.localPosition = Vector3.zero;
        shadow_res.transform.localScale = new Vector3(1.5f,1.5f,1.5f);
        if (QualitySettings.GetQualityLevel() != 0)
            shadow_res.SetActive(false);


        BattleAnimalMonster bb = new BattleAnimalMonster(monster)
        {
            player = monster,
            accept = accept,
            action = "",
            action_speed = 1,
            jfr = 0,
            alpha = 1,
            last = new List<List<object>>() { new List<object> { monster.x, monster.y, monster.is_jf, null } },
            last_time = 0,
            obj = obj,
            objt = obj.transform,
            objid = objid,
            posobj = posobj,
            posobjt = posobj.transform,
            posobjid = posobjid,
            shadow_res = shadow_res,
            cao = cao,
            unit = unit,
            ur_ = unit.get_round(),
            is_die = false,
            xobj = null,
            xobjid = 0,
            xobjt = null,
            xunit = null
        };
        bb.InitBattlePlayer();
        BattlePlayers.players.Add(bb.animal.guid, bb);
        BattlePlayers.penguin_list.Add(bb);

        BattleGrid.add(grid_type.et_player, monster.guid, monster.x, monster.y);
        if(is_new)
            bb.set_hp(bb.attr.max_hp());

        BattlePlayers.CalcCao(bb);
        if (bb.player.hp <= 0)
            bb.is_die = true;
        Battle.battle_panel.add_monster_min_pro(bb);
    }
    public static void addboss(battle_boss player,bool is_new = false)
    {
        player.r = BattleOperation.checkr(player.r);
        var t_role = Config.get_t_role(player.role_id);
        if (t_role == null)
            return;
        var obj = LuaHelper.GetResManager().CreateUnit(t_role.res, false);
        int objid = BattlePlayers.bobjpool.add(obj);
        var posobj = new GameObject();
        int posobjid = BattlePlayers.bobjpool.add(posobj);
        Transform posobjt = posobj.transform;
        posobjt.parent = LuaHelper.GetResManager().UnitRoot;
        var boss_ring = LuaHelper.GetResManager().CreateEffect("Boss_CollisionRange");
        boss_ring.transform.parent = posobj.transform;
        boss_ring.transform.localPosition = Vector3.zero;
        boss_ring.transform.localScale = Vector3.one;

        BattlePlayers.bobjpool.set_localPosition(posobjid, player.x * 1.0f / Battle.BL, 0, player.y * 1.0f / Battle.BL);
        BattlePlayers.bobjpool.set_localEulerAngles(posobjid, 0, 0, 0);
        BattlePlayers.bobjpool.set_localScale(posobjid, 1, 1, 1);

        var objt = obj.transform;
        obj.transform.parent = posobj.transform;
        BattlePlayers.bobjpool.set_localPosition(objid, 0, 0, 0);
        BattlePlayers.bobjpool.set_localEulerAngles(objid, 0, 90 - player.r, 0);
        BattlePlayers.bobjpool.set_localScale(objid, 1, 1, 1);
        var cao = BattleGrid.get_cao(player.x, player.y);
        var unit = obj.GetComponent<unit>();
        Transform accept = unit.get_bone("accept");
        BattleAnimalBoss bb = new BattleAnimalBoss(player)
        {
            player = player,
            accept = accept,
            action = "",
            action_speed = 1,
            jfr = 0,
            alpha = 1,
            last = new List<List<object>>() { new List<object> { player.x, player.y, player.is_jf, null } },
            last_time = 0,
            obj = obj,
            objt = obj.transform,
            objid = objid,
            posobj = posobj,
            posobjt = posobj.transform,
            posobjid = posobjid,
            cao = cao,
            unit = unit,
            ur_ = unit.get_round(),
            is_die = false,
            xobj = null,
            xobjid = 0,
            xobjt = null,
            xunit = null
        };
        bb.InitBattlePlayer();
        BattlePlayers.players.Add(player.guid, bb);
        BattlePlayers.Boss = bb;
        BattleGrid.add(grid_type.et_player, player.guid, player.x, player.y);

        if (Battle.is_newplayer_guide)
        {
            // 设置雪怪的hp 是1000(在新手引导中)
            bb.set_hp(1000);
        }
        //bb.set_hp(bb.attr.max_hp());

        else if (is_new)
            bb.set_hp(0);
        BattlePlayers.CalcCao(bb);     
        if (bb.player.hp <= 0)
            bb.is_die = true;
        Battle.battle_panel.add_min_boss_pro(bb);
    }
    public static void addplayer(battle_player player, bool is_new = false)
    {
        is_new = is_new || false;
        player.r = BattleOperation.checkr(player.r);
        if (is_new && player.is_ai == 0)
        {
            if (BattlePlayers.players.ContainsKey(player.guid))
            {
                var mp = BattlePlayers.players[player.guid] as BattleAnimalPlayer;
                mp.player.is_ai = 0;
                mp.player.is_move = false;
                mp.player.re_pre = 0;
                if (self.guid == mp.player.guid)
                {
                    BattlePlayers.me = mp;
                    Battle.battle_panel.InitBattlePanel();
                }
                return;
            }
            else
            {
                for (int i = 0; i < BattlePlayers.players_list.Count; i++)
                {
                    var bp1 = BattlePlayers.players_list[i];
                    if (Convert.ToInt64(bp1.player.guid) < 100 && bp1.player.camp == player.camp)
                    {
                        BattlePlayers.delplayer(bp1.player.guid);
                        break;
                    }
                }
            }
        }

        if (is_new)
        {
            if (Battle.is_newplayer_guide)
            {
                if (Convert.ToInt64(player.guid) > 1 && player.guid != self.guid)
                {
                    player.x = 18000000;
                    player.y = 18000000;
                    player.birth_x = 18000000;
                    player.birth_y = 18000000;
                }
                else
                {
                    // 这个是 新手引导中 玩家的初始位置
                    player.x = 320000;
                    player.y = 360000;
                    player.birth_x = 320000;
                    player.birth_y = 360000;
                }
            }
        }

        var t_role = Config.get_t_role(player.role_id);
        if (t_role == null)
            return;
        var obj = LuaHelper.GetResManager().CreateUnit(t_role.res, false);
        if (t_role.type == 0)
        {
            var weapon_pack = "sn_shp002";
            if (player.fashion.Count >= 3 && player.fashion[2] != 0)
            {
                var fashion_temp = Config.get_t_fashion(player.fashion[2]);
                weapon_pack = fashion_temp.model;
            }
            obj.GetComponent<unit>().change_static_part("weapon", weapon_pack, "Bone001");
        }

        int objid = BattlePlayers.bobjpool.add(obj);
        var posobj = new GameObject();
        int posobjid = BattlePlayers.bobjpool.add(posobj);
        Transform posobjt = posobj.transform;

        posobjt.parent = LuaHelper.GetResManager().UnitRoot;

        GameObject shadow_res = null;
        shadow_res = LuaHelper.GetResManager().CreateEffect("chr_showed_obj");
        shadow_res.transform.parent = posobj.transform;
        shadow_res.transform.localPosition = Vector3.zero;
        shadow_res.transform.localScale = Vector3.one;
        if (QualitySettings.GetQualityLevel() != 0)
            shadow_res.SetActive(false);

        GameObject ring_res = null;
        if (Battle.is_online && BattlePlayers.battle_type == 1)
        {
            if ((BattlePlayers.me == null || player.camp != BattlePlayers.me.player.camp) && player.is_ai < 1000)
            {
                ring_res = LuaHelper.GetResManager().CreateEffect("EnemyMark");
                ring_res.transform.parent = posobj.transform;
                ring_res.transform.localPosition = new Vector3(0, 0.7f, 0);
                ring_res.transform.localScale = Vector3.one;
            }
        }

        //add pf 
        GameObject pf_res = null;
        pf_res = LuaHelper.GetResManager().CreateUnit("pf");
        pf_res.transform.parent = obj.GetComponent<unit>().get_bone("Bip001 Neck");
        pf_res.transform.localPosition = new Vector3(t_role.pf_x / 100.0f, t_role.pf_y / 100.0f, t_role.pf_z / 100.0f);
        pf_res.transform.localScale = new Vector3(2,2,2);
        pf_res.name = "pf";
        pf_res.GetComponentInChildren<SmoothJoint>().m_height_root = obj;
        

        BattlePlayers.bobjpool.set_localPosition(posobjid, player.x * 1.0f / Battle.BL, 0, player.y * 1.0f / Battle.BL);
        BattlePlayers.bobjpool.set_localEulerAngles(posobjid, 0, 0, 0);
        BattlePlayers.bobjpool.set_localScale(posobjid, 1, 1, 1);

        var objt = obj.transform;
        obj.transform.parent = posobj.transform;
        BattlePlayers.bobjpool.set_localPosition(objid, 0, 0, 0);
        BattlePlayers.bobjpool.set_localEulerAngles(objid, 0, 90 - player.r, 0);
        BattlePlayers.bobjpool.set_localScale(objid, 1, 1, 1);
        var cao = BattleGrid.get_cao(player.x, player.y);
        var unit = obj.GetComponent<unit>();
        Transform accept = unit.get_bone("accept");

        BattleAnimalPlayer bp = new BattleAnimalPlayer(player)
        {
            player = player,
            accept = accept,
            action = "",
            action_speed = 1,
            jfr = 0,
            alpha = 1,
            last = new List<List<object>>() { new List<object> { player.x, player.y, player.is_jf, null } },
            last_time = 0,
            obj = obj,
            objt = obj.transform,
            objid = objid,
            posobj = posobj,
            posobjt = posobj.transform,
            posobjid = posobjid,
            cao = cao,
            unit = unit,
            ur_ = unit.get_round(),
            is_die = false,
            achieveRecords = new AchieveRecords(),
            mtk = false,
            shadow_res = shadow_res,
            ring_res = ring_res,
            pf_res = pf_res,
            xobj = null,
            xobjid = 0,
            xobjt = null,
            xunit = null
        };
        bp.InitBattlePlayer();
        BattlePlayers.players.Add(player.guid,bp);
        BattlePlayers.players_list_change = true;

        BattleGrid.add(grid_type.et_player, player.guid, player.x, player.y);
        if (is_new)
        {
            if (bp.animal.is_ai >= 1000 && bp.animal.is_ai < 2000)
            {
                bp.set_hp(0);
                bp.is_die = true;
            }
            else
                bp.set_hp(bp.attr.max_hp());
        }

        if (bp.player.is_xueren)
            BattlePlayers.MakeXueren(bp, true, true);
        BattlePlayers.CalcCao(bp);
        if (self.guid == bp.player.guid)
        {
            BattlePlayers.me = bp;
            Battle.battle_panel.InitBattlePanel();
            GameObject.Destroy(bp.ring_res);
            bp.ring_res = null;
            foreach (var item in BattlePlayers.players)
            {
                if (item.Value.animal.type == unit_type.Player)
                {
                    var tp = item.Value as BattleAnimalPlayer;
                    if (tp.player.camp == bp.player.camp && tp.player.guid != self.guid)
                    {
                        GameObject.Destroy(tp.ring_res);
                        tp.ring_res = null;
                    }
                }
            }
        }

        if (bp.player.hp <= 0)
        {
            bp.is_die = true;
            if (BattlePlayers.me != null)
            {
                if (BattlePlayers.me.player.guid == bp.player.guid)
                    Battle.battle_panel.show_die(null);
            }
        }
        Battle.battle_panel.add_min_pro(bp);
    }

    public static void Out(msg_battle_op msg)
    {
        string guid = msg.guid.ToString();
        if (!BattlePlayers.players.ContainsKey(guid))
            return;

        var bp = BattlePlayers.players[guid] as BattleAnimalPlayer;
        bp.player.is_ai = 2;
        bp.player.re_pre = 0;
        bp.player.re_pre_zhen = 0;
        BattlePlayers.InitAIBaseInfo(bp.player);
    }

    public static void add_effect(int re_tid, int camp, int id, int x, int y, int xx, int yy, int r, string re_guid, ulong follow_guid = 0,int re_ur = 0)
    {
        r = BattleOperation.checkr(r);
        if (!BattlePlayers.players.ContainsKey(re_guid))
        {
            Debug.Log("不包含! effect");
            return;
        }

        var re_bp = BattlePlayers.players[re_guid];
        var t_skill_effect = Config.get_t_skill_effect(id);
        if (t_skill_effect == null)
        {
            Debug.Log("t_skill_effect == null");
            return;
        }
        var effect = new msg_battle_effect();
        effect.tid = BattlePlayers.tid;
        BattlePlayers.tid = BattlePlayers.tid + 1;
        effect.re_tid = re_tid;
        effect.id = id;
        effect.camp = camp;
        effect.sx = x;
        effect.sy = y;
        effect.x = x;
        effect.y = y;
        effect.xx = xx;
        effect.yy = yy;
        effect.r = r;
        effect.re_guid = re_guid;
        if (t_skill_effect.type == 10)
        {
            effect.follow_guid = follow_guid.ToString();
            effect.re_ur = 0;
        }
        else
        {
            effect.follow_guid = "";
            effect.re_ur = re_ur;
        }
        effect.time = BattlePlayers.zhen;
        effect.dd_time = BattlePlayers.zhen;
        effect.state = 0;
        effect.len = 0;
        if (re_bp.animal.type == unit_type.Player)
        {
            var tp = re_bp as BattleAnimalPlayer;
            effect.pre_zhen = tp.player.re_pre_zhen1;
        }
        else
            effect.pre_zhen = 0;

        if (t_skill_effect.type == 1)
            effect.len = t_skill_effect.get_range(re_bp, effect.pre_zhen) - effect.re_ur;
        else if (t_skill_effect.type == 2)
        {
            effect.len = (int)BattleOperation.get_distance(effect.sx, effect.sy, effect.xx, effect.yy);
            if (effect.len > t_skill_effect.get_range(re_bp, effect.pre_zhen))
                effect.len = t_skill_effect.get_range(re_bp, effect.pre_zhen);
        }

        if (t_skill_effect.type == 7 || (t_skill_effect.type < 2 && t_skill_effect.fx_speed == 0))
        {
            effect.sx = effect.xx;
            effect.sy = effect.yy;
            effect.x = effect.xx;
            effect.y = effect.yy;
        }
        BattlePlayers.add_effect2(effect);
        if (t_skill_effect.togather_effect > 0)
            BattlePlayers.add_effect(re_tid, camp, t_skill_effect.togather_effect, x, y, xx, yy, r, re_guid, follow_guid,re_ur);
    }

    public static void add_effect2(msg_battle_effect effect)
    {
        effect.r = BattleOperation.checkr(effect.r);
        var t_skill_effect = Config.get_t_skill_effect(effect.id);
        if (t_skill_effect == null)
            return;

        if (!BattlePlayers.players.ContainsKey(effect.re_guid))
            return;

        var re_bp = BattlePlayers.players[effect.re_guid];
        double bscale = re_bp.get_scale();
        GameObject obj = null;
        Transform objt = null;
        int objid = 0;
        if (!BattlePlayers.jiasu)
        {
            if (t_skill_effect.effect != "")
            {
                string efname = t_skill_effect.effect;
                if (re_bp.animal.type == unit_type.Player && t_skill_effect.skill_type == 1)
                {
                    var tp = re_bp as BattleAnimalPlayer;
                    if(tp.pskill_fashion != null)
                        efname = tp.pskill_fashion.model;
                }
                obj = LuaHelper.GetResManager().CreateEffect(efname,self.eff_quatity == 0);
                objid = BattlePlayers.bobjpool.add(obj);
                objt = obj.transform;
                objt.parent = LuaHelper.GetResManager().UnitRoot;
                BattlePlayers.bobjpool.set_localPosition(objid, (float)(effect.x * 1.0 / Battle.BL), (float)(t_skill_effect.fx_hight * bscale / Battle.BL), (float)(effect.y * 1.0 / Battle.BL));
                BattlePlayers.bobjpool.set_localEulerAngles(objid, 0, 90 - effect.r, 0);
                float sl = (float)(t_skill_effect.get_effect_scale(re_bp, effect.pre_zhen));
                BattlePlayers.bobjpool.set_localScale(objid, sl, sl, sl);
            }
            if (t_skill_effect.is_zp == 1)
            {
                if (BattlePlayers.me != null)
                {
                    var d = BattleOperation.get_distance(effect.x, effect.y, BattlePlayers.me.player.x, BattlePlayers.me.player.y);

                    if (d < 120000 * BattlePlayers.me.get_scale())
                        LuaHelper.GetMapManager().shake_cam(0.5f);
                }
            }
        }

        BattlePlayers.effects.Add(effect.tid, new battle_effect()
        {
            effect = effect,
            obj = obj,
            objt = objt,
            objid = objid,
            last_time = 0,
            destroyEffects = new Dictionary<int, int>(),
            effect_hums = new Dictionary<string, int>(),
            re_bp = re_bp,
            last = new List<List<object>>() { new List<object>() { effect.x, effect.y, null } }
        });
        BattlePlayers.effects_list_change = true;
        var be = BattlePlayers.effects[effect.tid];
        if (obj != null && t_skill_effect.type <= 2)
        {
            be.p0 = new Vector3((float)(effect.sx * 1.0 / Battle.BL), (float)(t_skill_effect.fx_hight * bscale / Battle.BL), (float)(effect.sy * 1.0 / Battle.BL));
            if (t_skill_effect.type == 1)
            {
                var p = BattleOperation.add_distance2(effect.sx, effect.sy, effect.r, effect.len);
                be.p3 = new Vector3(p[0] * 1.0f / Battle.BL, 0, p[1] * 1.0f / Battle.BL);
            }
            else if (t_skill_effect.type == 2)
            {
                var p = BattleOperation.add_distance2(effect.sx, effect.sy, effect.r, effect.len);
                be.p3 = new Vector3(p[0] * 1.0f / Battle.BL, 0, p[1] * 1.0f / Battle.BL);
            }

            

            be.p1 = be.p0 + (be.p3 - be.p0) / 2.0f;
            be.p1.y = (float)(t_skill_effect.fx_hight1 * bscale) / Battle.BL;
            be.p2 = be.p1 + (be.p3 - be.p1) / 2.0f;
            be.p2.y = (float)(t_skill_effect.fx_hight2 * bscale) / Battle.BL;
            var pos = BattleOperation.get_distance(effect.sx, effect.sy, effect.x, effect.y);
            be.pt = pos * 1.0 / effect.len;
            be.last[0][2] = be.pt;
        }
        BattleGrid.add(grid_type.et_effect, effect.tid, effect.x, effect.y);
    }

    public static void del_effect(int tid)
    {
        if (!BattlePlayers.effects.ContainsKey(tid))
            return;

        var be = BattlePlayers.effects[tid];
        if (be.effect.re_guid == self.guid)
        {
            var skill_id = Config.get_t_skill_effect(be.effect.id).skill_id;
            bool sign = false;
            foreach (var item in BattlePlayers.effects.Values)
            {
                if (item.effect.tid != tid && item.effect.re_tid == be.effect.re_tid)
                {
                    foreach (var sitem in be.destroyEffects)
                    {
                        if (item.destroyEffects.ContainsKey(sitem.Key))
                            item.destroyEffects[sitem.Key] = item.destroyEffects[sitem.Key] + sitem.Value;
                        else
                            item.destroyEffects.Add(sitem.Key, sitem.Value);
                    }
                    sign = true;
                }
            }

            if (!sign)
            {
                foreach (var item in be.destroyEffects)
                    BattleAchieve.OnlyBattleSkillRelationForSkill(skill_id, item.Key, item.Value);
                be.destroyEffects.Clear();
            }

            sign = false;
            foreach (var item in BattlePlayers.effects)
            {
                var v = item.Value;
                if (v.effect.tid != tid && v.effect.re_tid == be.effect.re_tid)
                {
                    foreach (var sitem in be.effect_hums)
                    {
                        if (v.effect_hums.ContainsKey(sitem.Key))
                        {
                            if ((sitem.Value == 1 && v.effect_hums[sitem.Key] == 0) || (sitem.Value == 0 && v.effect_hums[sitem.Key] == 1))
                                v.effect_hums[sitem.Key] = 0;
                            else
                                v.effect_hums[sitem.Key] = sitem.Value;
                        }
                        else
                            v.effect_hums.Add(sitem.Key, sitem.Value);
                    }
                    sign = true;
                }
            }

            if (!sign)
                BattleAchieve.OnlyBattleSkillRelationForMan(skill_id, be.effect_hums);
        }

        if (be.obj != null)
        {
            BattlePlayers.bobjpool.remove(be.objid);
            LuaHelper.GetResManager().DeleteEffect(be.obj);
        }
        BattlePlayers.effects.Remove(tid);
        BattlePlayers.effects_list_change = true;
        BattleGrid.del(grid_type.et_effect, tid);
    }

    public static void do_check_pos_item()
    {
        if (Battle.is_newplayer_guide)
            return;

        foreach (var item in Config.t_battle_refresh_items)
        {
            if (BattlePlayers.IsExistInArea(item.Value))
                continue;
            int snum = BattleOperation.random(1, item.Value.max_amount);
            for (int m = 0; m < snum; m++)
            {
                int total = 0;
                foreach (var inner in item.Value.bts)
                    total += inner.wt;
                int rd = BattleOperation.random(0, total);
                total = 0;
                for (int i = 0; i < item.Value.bts.Count; i++)
                {
                    total = total + item.Value.bts[i].wt;
                    if (total > rd)
                    {
                        for (int mm = 0; mm < item.Value.bts[i].amount; mm++)
                        {
                            int[] arr = BattleOperation.get_avilialbe_xy2(item.Value.pos_x, item.Value.pos_y, item.Value.max_dis);
                            BattlePlayers.add_item(item.Value.bts[i].id, arr[0], arr[1], 1, item.Value.id);
                        }    
                        break;
                    }
                }
            }
        }
    }

    public static void do_check_item()
    {
        if (Battle.is_newplayer_guide)
            return;

        for (int i = 0; i < 16; i++)
        {
            if (BattlePlayers.item_num[1] < 80)
            {
                var p = BattleOperation.get_avilialbe_xy1();
                BattlePlayers.add_item(0, p[0], p[1]);
            }
            else
                break;
        }

        //for (int i = 0; i < 1 * n; i++)
        //{
        //    if (BattlePlayers.item_num[2] < 5)
        //    {
        //        var p = BattleOperation.get_avilialbe_xy1();
        //        var r = BattleOperation.random(0, 99 + 1);
        //        int id = 1;
        //        if (r < 20)
        //            id = 2;
        //        BattlePlayers.add_item(id, p[0], p[1]);
        //    }
        //    else
        //        break;
        //}
        //for (int i = 0; i < 3 * n; i++)
        //{
        //    if (BattlePlayers.item_num[4] < 15)
        //    {
        //        var p = BattleOperation.get_avilialbe_xy1();
        //        int id = BattleOperation.random(200, 209 + 1);
        //        BattlePlayers.add_item(id, p[0], p[1]);
        //    }
        //    else
        //        break;
        //}
    }

    public static void init_ai()
    {
        if (!Battle.is_online)
        {
            var t_ai_setting = Config.get_t_ai_setting(self.player.level);
            int low_num = t_ai_setting.low_ai_amount;
            int mid_num = t_ai_setting.mid_ai_amount;
            int high_num = t_ai_setting.high_ai_amount;
            for (int i = 1; i <= BattlePlayers.member_num * BattlePlayers.team_num; i++)
            {
                if (low_num > 0)
                {
                    BattlePlayers.add_ai(i, 1);
                    low_num = low_num - 1;
                }
                else if (mid_num > 0)
                {
                    BattlePlayers.add_ai(i, 2);
                    mid_num = mid_num - 1;
                }
                else
                {
                    BattlePlayers.add_ai(i, 3);
                    high_num = high_num - 1;
                }
            }
        }
        else
        {
            for (int i = 1; i <= BattlePlayers.team_num * BattlePlayers.member_num; i++)
                BattlePlayers.add_ai(i, 2);
        }

        if (!Battle.is_newplayer_guide)
        {
            BattlePlayers.addbossUnit();
            BattlePlayers.addpenguin();
        }      
        BattlePlayers.change_player_list();
    }

    public static bool IsExistInArea(t_battle_item_pos item)
    {
        foreach (var it in BattlePlayers.items)
        {
            if (item.id == it.Value.item.birth_pos)
                continue;

            bool flag1 = false, flag2 = false;
            if (item.pos_x + item.max_dis >= it.Value.item.x && item.pos_x - item.max_dis <= it.Value.item.x)
                flag1 = true;
            if (item.pos_y + item.max_dis >= it.Value.item.y && item.pos_y - item.max_dis <= it.Value.item.y)
                flag2 = true;
            if (flag1 && flag2)
                return true;
        }
        return false;
    }

    public static battle_item add_item(int id, int x, int y, int? pr = null,int birth_pos = 0)
    {
        var item = new battle_item_base();
        item.birth_pos = birth_pos;
        item.tid = BattlePlayers.tid;
        BattlePlayers.tid = BattlePlayers.tid + 1;
        item.id = id;
        item.x = x;
        item.y = y;
        int param = 1;
        if (id == 0 || id == 3)
        {
            if (pr == null)
            {
                var r = BattleOperation.random(0, 100);
                if (r < 5)
                    param = 2;
                else if (r < 20)
                    param = 5;
            }
            else
                param = pr.GetValueOrDefault();
        }
        item.param = param;
        item.zhen = BattlePlayers.zhen;
        item.birth_pos = 0;
        return BattlePlayers.add_item2(item);
    }

    public static battle_item add_item2(battle_item_base item)
    {
        var t_battle_item = Config.get_t_battle_item(item.id);
        if (t_battle_item == null)
            return null;
        if (!BattlePlayers.items.ContainsKey(item.tid))
            BattlePlayers.items.Add(item.tid, new battle_item());
        BattlePlayers.items[item.tid].item = item;
        var bi = BattlePlayers.items[item.tid];
        if (!BattlePlayers.jiasu)
            BattlePlayers.add_item_obj(bi);
        BattleGrid.add(grid_type.et_item, item.tid, item.x, item.y);
        BattlePlayers.item_num[t_battle_item.type] = BattlePlayers.item_num[t_battle_item.type] + 1;
        return BattlePlayers.items[item.tid];
    }

    public static void add_item_obj(battle_item bi)
    {
        var t_battle_item = Config.get_t_battle_item(bi.item.id);
        if (t_battle_item == null)
            return;
        bi.obj = LuaHelper.GetResManager().CreateEffect("rongqi");
        bi.objid = BattlePlayers.bobjpool.add(bi.obj);
        bi.objt = bi.obj.transform;
        bi.objt.parent = LuaHelper.GetResManager().UnitRoot;
        BattlePlayers.bobjpool.set_localPosition(bi.objid, bi.item.x * 1.0f / Battle.BL, 0.1f, bi.item.y * 1.0f / Battle.BL);
        BattlePlayers.bobjpool.set_localEulerAngles(bi.objid, 0, 0, 0);
        var obj1 = LuaHelper.GetResManager().CreateEffect(t_battle_item.effect);
        var objt1 = obj1.transform;
        objt1.parent = bi.objt.Find("sub");
        objt1.localPosition = Vector3.zero;
        objt1.localEulerAngles = new Vector3(0, new System.Random().Next(0, 359), 0);
        objt1.localScale = Vector3.one;
        var scale = 0.75f + bi.item.param * 0.25f;
        if (scale > 3)
            scale = 3;
        BattlePlayers.bobjpool.set_localScale(bi.objid, scale, scale, scale);
    }

    public static void add_random_item(int id, int x, int y)
    {
        var p = BattleOperation.get_avilialbe_xy2(x, y, 20000);
        var bi = BattlePlayers.add_item(id, p[0], p[1]);
        if (!BattlePlayers.jiasu)
        {
            bi.xx = x * 1.0f / Battle.BL;
            bi.yy = y * 1.0f / Battle.BL;
            bi.ll = BattleOperation.get_distance(x, y, p[0], p[1]);
            bi.speed = bi.ll / 10000.0f / 0.7f;
            BattlePlayers.item_speeds.Add(bi);
        }
    }

    public static void add_random_item(int id, int x, int y,int max_dis)
    {
        var p = BattleOperation.get_avilialbe_xy2(x, y, max_dis);
        var bi = BattlePlayers.add_item(id, p[0], p[1]);
        if (!BattlePlayers.jiasu)
        {
            bi.xx = x * 1.0f / Battle.BL;
            bi.yy = y * 1.0f / Battle.BL;
            bi.ll = BattleOperation.get_distance(x, y, p[0], p[1]);
            bi.speed = bi.ll / 10000.0f / 0.7f;
            BattlePlayers.item_speeds.Add(bi);
        }
    }

    public static void add_random_item1(int x, int y, int r, int dis,int? id = null)
    {
        var p = BattleOperation.get_avilialbe_xy2(x, y, dis);
        battle_item bi = null;
        if(id == null)
            bi = BattlePlayers.add_item(0, p[0], p[1], r);
        else
            bi = BattlePlayers.add_item(id.GetValueOrDefault(), p[0], p[1], r);
        if (!BattlePlayers.jiasu)
        {
            bi.xx = x * 1.0f / Battle.BL;
            bi.yy = y * 1.0f / Battle.BL;
            bi.ll = BattleOperation.get_distance(x, y, p[0], p[1]);
            bi.speed = bi.ll * 1.0f / 10000 / 0.7f;
            BattlePlayers.item_speeds.Add(bi);
        }
    }

    public static void del_item(int tid, BattleAnimal bp)
    {
        if (!BattlePlayers.items.ContainsKey(tid))
            return;
        var bi = BattlePlayers.items[tid];
        var t_battle_item = Config.get_t_battle_item(bi.item.id);
        if (t_battle_item == null)
            return;
        if (bi.obj != null && bp != null)
        {
            bi.follow = bp.animal.guid;
            bi.follow_time = 0;
            bi.xx = bi.item.x * 1.0f / Battle.BL;
            bi.yy = bi.item.y * 1.0f / Battle.BL;
            bi.follow_speed = 1 + bp.attr_value[33] / 100.0f;
            BattlePlayers.item_follows[tid] = bi;
        }
        BattlePlayers.item_num[t_battle_item.type] = BattlePlayers.item_num[t_battle_item.type] - 1;
        BattlePlayers.items.Remove(tid);
        BattleGrid.del(grid_type.et_item, tid);
    }

    public static void Move(msg_battle_op msg)
    {
        string guid = msg.guid.ToString();
        var r = msg.param_ints[0];
        if (!BattlePlayers.players.ContainsKey(guid))
            return;
        var bp = BattlePlayers.players[guid];
        BattlePlayers.Move1(bp, r);
    }

    public static void Move1(BattleAnimal bp, int r)
    {
        if (bp.attr.is_hunluan())
        {
            r = r + 180;
        }


        r = BattleOperation.checkr(r);
        bp.animal.r = r;
        bp.animal.r_py = 0;
        bp.animal.is_move = true;
    }

    public static void Stop(msg_battle_op msg)
    {
        string guid = msg.guid.ToString();
        var r = msg.param_ints[0];
        if (!BattlePlayers.players.ContainsKey(guid))
            return;
        var bp = BattlePlayers.players[guid];
        BattlePlayers.Stop1(bp, r);
    }

    public static void Stop1(BattleAnimal bp, int r)
    {
        r = BattleOperation.checkr(r);
        bp.animal.is_move = false;
        bp.animal.r = r;
        bp.animal.r_py = 0;
        if (bp.animal.re_state > 0)
            bp.animal.r = bp.animal.re_r;
    }

    public static void Attackr(msg_battle_op msg)
    {
        string guid = msg.guid.ToString();
        var r = msg.param_ints[0];
        if (!BattlePlayers.players.ContainsKey(guid))
            return;
        var bp = BattlePlayers.players[guid];
        BattlePlayers.Attackr1(bp, r);
    }

    public static void Attackr1(BattleAnimal bp, int r)
    {
        bp.animal.attackr = r;
    }

    public static void PreRelease(msg_battle_op msg)
    {
        string guid = msg.guid.ToString();
        int state = msg.param_ints[0];
        if (!BattlePlayers.players.ContainsKey(guid))
            return;
        var bp = BattlePlayers.players[guid] as BattleAnimalPlayer;
        BattlePlayers.PreRelease1(bp, state);
    }

    public static void PreRelease1(BattleAnimalPlayer bp, int state)
    {
        bp.player.re_pre = state;
        bp.player.re_pre_zhen = 0;
    }

    public static void Release(msg_battle_op msg)
    {
        string guid = msg.guid.ToString();
        int id = msg.param_ints[0];
        int x = msg.param_ints[1];
        int y = msg.param_ints[2];
        int r = msg.param_ints[3];
        if (!BattlePlayers.players.ContainsKey(guid))
            return;
        var bp = BattlePlayers.players[guid];
        BattlePlayers.Release1(bp, id, x, y, r);
    }

    public static void Release1(BattleAnimal bp, int id, int x, int y, int r)
    {
        if (bp.attr.is_hunluan())
        {
            r = r + 180;
            x = bp.animal.x * 2 - x;
            y = bp.animal.y * 2 - y;
        }
        r = BattleOperation.checkr(r);
        var t_skill = Config.get_t_skill(id, bp.get_skill_level(id), bp.get_skill_level_add(id));
        if (t_skill == null)
            return;

        if (bp.animal.re_state > 0 && t_skill.type != 3)
            return;

        if (!bp.can_do())
            return;

        if (bp.animal.type == unit_type.Player)
        {
            var tp = bp as BattleAnimalPlayer;
            if (tp.player.is_xueren && t_skill.type != 3)
                return;
        }

        if (t_skill.release_type == 3)
        {
            if (BattleOperation.check_distance(x, y, bp.animal.x, bp.animal.y, t_skill.get_range(bp) + 5000))
            {
                return;
            }
        }

        if (t_skill.link_effect > 0)
        {
            var t_skill_effect = Config.get_t_skill_effect(t_skill.link_effect);
            if (t_skill_effect != null && t_skill.release_type == 2 && t_skill_effect.type == 2)
            {
                var p = BattleOperation.add_distance2(bp.animal.x, bp.animal.y, r, t_skill.get_range(bp));
                x = p[0];
                y = p[1];
            }
        }
        int index = -1;
        for (int i = 0; i < bp.animal.save_re_id.Count; i++)
        {
            if (bp.animal.save_re_id[i] == id)
            {
                index = i;
                break;
            }
        }
        if (index == -1)
        {
            index = bp.animal.save_re_id.Count;
            bp.animal.save_re_id.Add(id);
            bp.animal.save_re_zhen.Add(-999999);
        }
        long ttime = (BattlePlayers.zhen - bp.animal.save_re_zhen[index]) * BattlePlayers.TICK;
        if (t_skill.type == 1)
            ttime = ttime * (10000 + bp.attr.aspeed()) / 10000;

        if (ttime <= t_skill.get_cd(bp))
        {
            if (t_skill.type != 3)
                return;

            if (bp.animal.type != unit_type.Player)
                return;
            else
            {
                var tp = bp as BattleAnimalPlayer;
                if (!tp.player.is_xueren)
                    return;
            }
        }

        if (bp.attr.is_yinshen())
            bp.remove_yinshen_buff();

        if (self.guid == bp.animal.guid && t_skill.type != 1 && !BattlePlayers.me.achieveRecords.IsUseSkill)
            BattlePlayers.me.achieveRecords.IsUseSkill = true;

        if (t_skill.type == 3)
        {
            var tp = bp as BattleAnimalPlayer;    
            if (!tp.player.is_xueren)
            {
                BattlePlayers.MakeXueren(tp, true);
                tp.player.xueren_zhen = BattlePlayers.zhen;
                tp.player.save_re_zhen[index] = BattlePlayers.zhen;
                if (tp.animal.guid == self.guid)
                    tp.achieveRecords.useXueren = true;
            }
            else if ((BattlePlayers.zhen - tp.player.xueren_zhen) * BattlePlayers.TICK >= 1000)
            {
                BattlePlayers.MakeXueren(tp, false);
                tp.player.save_re_zhen[index] = BattlePlayers.zhen;
                tp.player.re_state = 0;
                return;
            }
        }
        else if (t_skill.type == 2)
        {
            bp.yy(1);
            if (BattleOperation.random(0, 100) < bp.attr_value[27])
                bp.set_hp(bp.attr.max_hp(), true);
            else
                bp.set_hp(bp.animal.hp + bp.attr_value[26], true);
            if (BattleOperation.random(0, 100) >= bp.attr_value[30])
            {
                if (!Battle.is_newplayer_guide)
                    bp.animal.save_re_zhen[index] = BattlePlayers.zhen;
                else
                    bp.animal.save_re_zhen[index] = BattlePlayers.zhen - BattlePlayers.TNUM * 15;
            }
        }
        else
        {
            bp.animal.rtime = BattlePlayers.zhen;
            if (bp.animal.type == unit_type.Player)
            {
                var tp = bp as BattleAnimalPlayer;
                tp.player.re_pre_zhen1 = tp.player.re_pre_zhen;
            }
            bp.animal.save_re_zhen[index] = BattlePlayers.zhen;
        }

        if (t_skill.release_type == 6)
        {
            x = bp.animal.x;
            y = bp.animal.y;
            r = bp.animal.r;
        }

        if (t_skill.release_type == 5)
        {
            if (t_skill.link_effect > 0)
            {
                var t_skill_effect = Config.get_t_skill_effect(t_skill.link_effect);
                BattlePlayers.Attach(bp, "accept", t_skill_effect.effect);
                var t_skill_xiaoguo = Config.get_t_skill_xiaoguo(t_skill_effect.link_xiaoguo);
                if (t_skill_xiaoguo != null)
                {
                    if (t_skill_xiaoguo.link_buff > 0)
                    {
                        int yc = bp.attr_value[25];
                        yc = yc + bp.get_skill_attr_value(t_skill_effect.skill_id, 5);
                        bp.add_buff(t_skill_xiaoguo.link_buff, yc, t_skill_effect.time);
                    }
                }
            }
        }
        else
        {
            bp.animal.re_id = id;
            bp.animal.re_level = bp.get_skill_level(id);
            bp.animal.re_state = 1;
            bp.animal.re_time = 0;
            bp.animal.re_x = x;
            bp.animal.re_y = y;
            bp.animal.re_r = r;
            bp.animal.re_tid = bp.animal.re_tid + 1;
            bp.animal.attackr = r;
        }

        if (bp.animal.type == unit_type.Player)
        {
            var tp = bp as BattleAnimalPlayer;
            if (tp.player.skill_id == id)
            {
                tp.player.skill_id = 0;
                tp.player.skill_level = 0;
            }

            if (t_skill.type == 2)
            {
                if(t_skill.id == 300201)
                    tp.add_power(t_skill.cost * -POWERUP);
                else
                    tp.add_power(bp.get_skill_level(t_skill.id) * -POWERUP);
            }     
            else
                tp.add_power(t_skill.cost * -POWERUP);
        }
    }

    public static void Talent(msg_battle_op msg)
    {
        string guid = msg.guid.ToString();
        int talent_id = msg.param_ints[0];
        if (!BattlePlayers.players.ContainsKey(guid))
            return;
        var bp = BattlePlayers.players[guid];
        if (bp is BattleAnimalPlayer)
        {
            var tmp = bp as BattleAnimalPlayer;
            BattlePlayers.Talent1(tmp, talent_id);
        }
    }

    public static void Talent1(BattleAnimalPlayer bp, int talent_id)
    {
        var t_talent = Config.get_t_talent(talent_id);
        if (bp == null || t_talent == null || bp.player.talent_point <= 0)
            return;
        int p = -1;
        for (int i = 0; i < bp.player.talent_id.Count; i++)
        {
            if (bp.player.talent_id[i] == talent_id)
            {
                if (bp.player.talent_level[i] >= t_talent.max_level)
                    p = -2;
                else
                    p = i;
                break;
            }
        }

        if (p >= -1)
        {
            int level = 1;
            if (p == -1)
            {
                bp.player.talent_id.Add(talent_id);
                bp.player.talent_level.Add(1);
                bp.player.talent_point = bp.player.talent_point - 1;
            }
            else if (p >= 0)
            {
                bp.player.talent_level[p] = bp.player.talent_level[p] + 1;
                bp.player.talent_point = bp.player.talent_point - 1;
                level = bp.player.talent_level[p];
            }
            bp.change_talent(talent_id, level);
            if (bp.player.guid == self.guid && bp.player.is_ai == 0)
                Battle.battle_panel.TalentFly(talent_id);
            if (bp.player.guid == self.guid && bp.player.talent_point > 0 && bp.player.is_ai == 0)
                Battle.battle_panel.ShowTalentPanel();
        }
    }

    public static void BattleState(msg_battle_op msg)
    {
        Battle.send_state();
    }

    public static void BattleInsideMsg(msg_battle_op msg)
    {
        if (BattlePlayers.me == null)
            return;
        string guid = msg.guid.ToString();
        int battle_msg_id = msg.param_ints[0];
        BattleInsideMsg1(guid,battle_msg_id);
    }

    public static void BattleInsideMsg1(string guid,int battle_msg_id)
    {
        var t_battle_msg = Config.get_t_battle_msg(battle_msg_id);
        if (BattlePlayers.players.ContainsKey(guid))
        {
            var bp = BattlePlayers.players[guid] as BattleAnimalPlayer;
            if (bp.player.camp == BattlePlayers.me.player.camp)
            {
                if (t_battle_msg != null)
                    Battle.battle_panel.ShowBattleInsideMsg(bp, t_battle_msg);
            }  
        }        
    }

    public static void BattleCode(msg_battle_op msg)
    {
        Battle.send_code();
    }

    public static void Disappear(int x, int y, double h, int r, double s, string name)
    {
        if (BattlePlayers.jiasu || LuaHelper.GetMapManager().in_view((float)(x * 1.0 / Battle.BL), (float)h, (float)(y * 1.0 / Battle.BL)) == -1)
            return;
        var eff = LuaHelper.GetResManager().CreateEffect(name,self.eff_quatity == 0);
        Transform efft = eff.transform;
        efft.parent = LuaHelper.GetResManager().UnitRoot;
        efft.localPosition = new Vector3(x * 1.0f / Battle.BL, (float)h, y * 1.0f / Battle.BL);
        efft.localEulerAngles = new Vector3(0, 90 - r, 0);
        efft.localScale = new Vector3((float)s, (float)s, (float)s);
        LuaHelper.GetResManager().DeleteEffect(eff, 1.8f);
    }

    public static void Attack(BattleAnimal bp, battle_effect be, int atype)
    {
        if (atype == 2)
        {
            BattlePlayers.Recover(bp, be);
            return;
        }

        if (!BattlePlayers.players.ContainsKey(be.effect.re_guid))
            return;
        var re_bp = be.re_bp;

        if (re_bp.animal.type == unit_type.Boss && bp.animal.type == unit_type.Monster)
            return;

        var t_skill_effect = Config.get_t_skill_effect(be.effect.id);
        if (t_skill_effect == null)
            return;

        //如果是penguin记录是谁攻击的
        if (bp.animal.type == unit_type.Monster && re_bp.animal.type == unit_type.Player)
        {
            var tp = bp as BattleAnimalMonster;
            if (String.IsNullOrEmpty(tp.player.attack_guid))
                tp.player.attack_guid = re_bp.animal.guid;
        }

        if (bp.animal.type == unit_type.Boss)
        {
            if (t_skill_effect.skill_type != 1)
            {
                Battle.battle_panel.add_text(bp, "[ffaa00]"+Config.get_t_script_str("BattlePlayers_001"), 4);
                return;
            }

            if (bp.animal.ai_state == 0)
                bp.animal.ai_state = 1;
        }

        if (!BattlePlayers.jiasu && !String.IsNullOrEmpty(t_skill_effect.xiaoshi_effect2))
        {
            float h = 0.1f;
            if (be.obj != null)
            {
                float ox, oy, oz;
                BattlePlayers.bobjpool.get_localPosition(be.objid, out ox, out oy, out oz);
                h = oy;
            }
            BattlePlayers.Disappear(be.effect.x, be.effect.y, h, be.effect.r, t_skill_effect.get_effect_scale(be.re_bp, be.effect.pre_zhen), t_skill_effect.xiaoshi_effect2);
        }

        if (t_skill_effect.hit_effect != 0)
        {
            BattlePlayers.add_effect(be.effect.re_tid, be.effect.camp, t_skill_effect.hit_effect, be.effect.x, be.effect.y, be.effect.x, be.effect.y, be.effect.r, be.effect.re_guid, Convert.ToUInt64(bp.animal.guid));
        }
            
        var t_skill_xiaoguo = Config.get_t_skill_xiaoguo(t_skill_effect.link_xiaoguo);
        if (t_skill_xiaoguo == null)
            return;
        if (!BattlePlayers.jiasu && !String.IsNullOrEmpty(t_skill_xiaoguo.sj_effect))
        {
            float h = 0.1f;
            if (be.obj != null)
            {
                float ox, oy, oz;
                BattlePlayers.bobjpool.get_localPosition(be.objid, out ox, out oy, out oz);
                h = oy;
            }
            var efname = t_skill_xiaoguo.sj_effect;
            if (t_skill_effect.skill_type == 1 && be.re_bp.animal.type == unit_type.Player)
            {
                var tmp = be.re_bp as BattleAnimalPlayer;
                if(tmp.pskill_fashion != null)
                    efname = tmp.pskill_fashion.hit_effect;
            }      
            BattlePlayers.Disappear(be.effect.x, be.effect.y, h, be.effect.r, t_skill_effect.get_effect_scale(be.re_bp, be.effect.pre_zhen), efname);
        }

        if (bp.animal.type == unit_type.Player)
        {
            var tp = bp as BattleAnimalPlayer;
            if (tp.player.is_xueren)
            {
                tp.xunit.action("injured");
                if (t_skill_xiaoguo.jf_type == 1)
                {
                    var dis = BattleOperation.toInt(t_skill_xiaoguo.jf_dis * (1 + re_bp.get_skill_attr_value(t_skill_effect.skill_id, 6) / 100.0));
                    tp.set_jf(be.effect.r, t_skill_xiaoguo.jf_dis, t_skill_xiaoguo.jf_speed);
                }
                else if (t_skill_xiaoguo.jf_type == 2)
                {
                    int dx = be.effect.x - tp.animal.x;
                    int dy = be.effect.y - tp.animal.y;
                    int dr = BattleOperation.toInt(Math.Atan2(dy, dx) * 180 / Math.PI);
                    dr = BattleOperation.checkr(dr);
                    int ddis = (int)BattleOperation.get_distance(be.effect.x, be.effect.y, tp.animal.x, tp.animal.y);
                    tp.set_jf(dr, ddis, t_skill_xiaoguo.jf_speed);
                }
                else if (t_skill_xiaoguo.jf_type == 3)
                {
                    int dx = be.effect.x - tp.animal.x;
                    int dy = be.effect.y - tp.animal.y;
                    int dr = 180 + BattleOperation.toInt(Math.Atan2(dy, dx) * 180 / Math.PI);
                    dr = BattleOperation.checkr(dr);
                    int ddis = t_skill_effect.get_range_param(be.re_bp, be.effect.pre_zhen) - (int)BattleOperation.get_distance(be.effect.x, be.effect.y, tp.animal.x, tp.animal.y);
                    tp.set_jf(dr, ddis, t_skill_xiaoguo.jf_speed);
                }

                if (be.re_bp.animal.guid == self.guid && t_skill_effect.skill_id == 100101)
                    BattleAchieve.AttackXueRen();
                return;
            }
        }

        if (BattlePlayers.me != null)
        {
            if (be.effect.re_guid == BattlePlayers.me.player.guid)
                bp.unit.white();
        }
        if (t_skill_effect.type == 1)
        {
            for (int i = bp.animal.bhit_tids.Count - 1; i >= 0; i--)
                if (bp.animal.bhit_tids[i] == be.effect.re_tid)
                    return;
            bp.animal.bhit_tids.Add(be.effect.re_tid);
            if (bp.animal.bhit_tids.Count > 5)
                bp.animal.bhit_tids.RemoveAt(0);
        }

        //if (bp.attr.is_yinshen())
        //    bp.remove_yinshen_buff();
        //普攻免疫
        if (t_skill_effect.skill_type == 1)
        {
            if (BattleOperation.random(0, 100) < bp.attr_value[22])
            {
                Battle.battle_panel.add_text(bp, "[ffaa00]"+Config.get_t_script_str("BattlePlayers_001"), 4);
                return;
            }
        }
        //无敌
        if (bp.attr.is_wudi())
        {
            Battle.battle_panel.add_text(bp, "[ffaa00]"+ Config.get_t_script_str("BattlePlayers_001"), 4);
            return;
        }
        //盾挡
        if (bp is BattleAnimalPlayer)
        {
            var tmp = bp as BattleAnimalPlayer;
            if (tmp.player.lattr_value[8] > 0)
            {
                tmp.player.lattr_value[8] = tmp.player.lattr_value[8] - 1;
                if (tmp.player.lattr_value[8] == 0)
                    tmp.remove_buff(3001);
                Battle.battle_panel.add_text(tmp, "[ffaa00]"+ Config.get_t_script_str("BattlePlayers_001"), 4);
                return;
            }
        }

        //对身上携带的技能免疫
        if (bp.animal.type == unit_type.Monster || bp.animal.type == unit_type.Player)
        {
            if (bp is BattleAnimalPlayer)
            {
                var tp = bp as BattleAnimalPlayer;
                if (t_skill_effect.skill_type == 2 && tp.player.skill_id == t_skill_effect.skill_id && BattleOperation.random(0, 100) < bp.attr_value[89])
                {
                    Battle.battle_panel.add_text(bp, "[ffaa00]"+ Config.get_t_script_str("BattlePlayers_001"), 4);
                    return;
                }
            }
            else if (bp is BattleAnimalMonster)
            {
                var tp = bp as BattleAnimalMonster;
                if (t_skill_effect.skill_type == 2 && tp.player.skill_id == t_skill_effect.skill_id && BattleOperation.random(0, 100) < bp.attr_value[89])
                {
                    Battle.battle_panel.add_text(bp, "[ffaa00]"+ Config.get_t_script_str("BattlePlayers_001"), 4);
                    return;
                }
            }
        }

        //伤害计算
        bool cri = false;
        if (BattleOperation.random(0, 100) < re_bp.attr_value[9] && t_skill_effect.skill_type == 1)
            cri = true;
        int attack = re_bp.attr.atk();
        attack = BattleOperation.toInt(t_skill_xiaoguo.dmg_per * attack * 1.0 / 100) + t_skill_xiaoguo.dmg_gd;
        int zjs = 100;
        zjs = zjs + re_bp.attr.zs() - bp.attr.js() + bp.attr_value[90];
        if (t_skill_effect.skill_type != 1)
            zjs = zjs + re_bp.attr_value[16] - bp.attr_value[17] + re_bp.get_skill_attr_value(t_skill_effect.skill_id, 1);
        //普攻加伤害
        int xuli = 100;
        if (t_skill_effect.skill_type == 1)
        {
            attack = attack + re_bp.attr_value[72];
            int pre = BattleOperation.calc_pre(be.re_bp, be.effect.pre_zhen);
            zjs = zjs + re_bp.attr_value[46];
            if (pre >= 100)
                xuli = BattleOperation.toInt((xuli + pre * 3) * 15 / 10.0);
            else
                xuli = xuli + pre * 3;
        }
        //普攻飞行距离算伤害
        if (t_skill_effect.skill_type == 1 && re_bp.attr_value[42] != 0)
        {
            var dis = BattleOperation.get_distance(bp.animal.x, bp.animal.y, re_bp.animal.x, re_bp.animal.y);
            zjs = zjs + BattleOperation.toInt(BattleOperation.toInt(dis * 1.0 / Battle.BL) * 1.0 / (3 * re_bp.attr_value[42]));
        }

        //对方携带相同技能，伤害增加百分比
        if (re_bp is BattleAnimalPlayer && bp is BattleAnimalPlayer)
        {
            var bp1 = bp as BattleAnimalPlayer;
            var re_bp1 = re_bp as BattleAnimalPlayer;
            if(bp1.player.skill_id != 0 && bp1.player.skill_id == re_bp1.player.skill_id)
                zjs = zjs + re_bp1.attr_value[80];
        }

        //技能冷却期间减伤
        if (bp is BattleAnimalPlayer)
        {
            var bp1 = bp as BattleAnimalPlayer;
            if (bp1.attr_value[28] != 0 && bp1.is_skillcd(bp1.player.skill_id))
                zjs = zjs - bp1.attr_value[28];
        }
        
        attack = BattleOperation.toInt(BattleOperation.toInt(attack * zjs / 100.0) * xuli / 100.0);
        var t_ai_attr = Config.get_t_ai_attr(BattlePlayers.players[be.effect.re_guid].animal.is_ai);
        if (t_ai_attr != null)
        {
            if (Battle.is_newplayer_guide)
                attack = attack * 2;
            else
                attack = BattleOperation.toInt(attack * t_ai_attr.damage_p / 100.0);
        }

        double def = bp.attr.def();
        def = def / (100 + def);
        if (BattleOperation.random(0, 100) < re_bp.attr_value[18])
            def = 0;
        attack = BattleOperation.toInt(attack * (1 - def));
        if (cri)
            attack = BattleOperation.toInt(attack * 1.5);
        if (attack <= 1)
            attack = 1;

        if (be.effect.re_guid == self.guid && bp.animal.guid != self.guid && bp is BattleAnimalPlayer)
        {
            var tmp = bp as BattleAnimalPlayer;
            var pert = BattleOperation.toInt(tmp.animal.hp * 100.0 / tmp.attr.max_hp());
            if (pert > 20)
                tmp.mtk = true;
            if (Battle.is_newplayer_guide)
            {
                if (t_skill_effect.skill_type == 1)
                {
                    if (be.effect.pre_zhen > 20)
                    {
                        tmp.attack_state = 2;
                        tmp.attack_num[1] = tmp.attack_num[1] + 1;
                    }
                    else
                    {
                        tmp.attack_state = 1;
                        tmp.attack_num[0] = tmp.attack_num[0] + 1;
                    }
                }
                else
                {
                    tmp.attack_state = 3;
                    tmp.attack_num[2] = tmp.attack_num[2] + 1;
                }    
            }
        }

        if (cri)
            Battle.battle_panel.add_text(bp, attack.ToString(), 2);
        else
            Battle.battle_panel.add_text(bp, attack.ToString(), 1);

        if (Battle.is_newplayer_guide)
        {
            if (bp.mianyi != null && bp.mianyi.GetValueOrDefault() > 0)
            {
                if (BattleOperation.toInt(bp.animal.hp - attack) > 0)
                    bp.set_hp(BattleOperation.toInt(bp.animal.hp - attack));
                else
                {
                    int sethp = BattleOperation.toInt(bp.attr.max_hp() / 2.0);
                    bp.set_hp(sethp);
                }
            }
            else
                bp.set_hp(BattleOperation.toInt(bp.animal.hp - attack));
        }
        else
            bp.set_hp(BattleOperation.toInt(bp.animal.hp - attack));
        
        if (t_skill_effect.skill_type == 1 && !re_bp.is_die)
        {
            //普攻吸血
            int xx = (int)(re_bp.attr_value[74] + attack * re_bp.attr_value[39] / 100);
            re_bp.set_hp(re_bp.animal.hp + xx, true);
        }
        else if (t_skill_effect.skill_type != 1 && !re_bp.is_die)
        {
            int xx = (int)(attack * re_bp.attr_value[40] / 100);
            re_bp.set_hp(re_bp.animal.hp + xx, true);
        }

        //反弹技能
        bool is_ft = false;
        if (bp.attr_value[29] != 0 && t_skill_effect.skill_type != 1 && !re_bp.is_die && !re_bp.attr.is_wudi())
        {
            var fantan = (int)(attack * bp.attr_value[29] / 100);
            re_bp.set_hp(re_bp.animal.hp - fantan);
            Battle.battle_panel.add_text(re_bp, fantan.ToString(), 1);
            is_ft = true;
        }
        // 触发无敌
        if (is_ft && re_bp.animal.hp < 0 && !re_bp.attr.is_wudicd())
        {
            if (re_bp is BattleAnimalPlayer)
            {
                var tmp = re_bp as BattleAnimalPlayer;
                var t = tmp.get_talent_value(3);
                if (t != null)
                {
                    tmp.add_buff(2006, 0, t);
                    tmp.add_buff(3002);
                    tmp.set_hp(1);
                }
            }
        }

        if (bp is BattleAnimalPlayer && bp.animal.hp <= 0 && !bp.attr.is_wudicd())
        {
            var tmp = bp as BattleAnimalPlayer;
            var t = tmp.get_talent_value(3);
            if (t != null)
            {
                tmp.add_buff(2006, 0, t);
                tmp.add_buff(3002);
                tmp.set_hp(1);
            }
        }

        //攻击雪怪给 玩家增加score
        if (bp.animal.type == unit_type.Boss && re_bp.animal.type == unit_type.Player)
        {
            var tp = re_bp as BattleAnimalPlayer;
            int pre = BattleOperation.calc_pre(be.re_bp,be.effect.pre_zhen);
            var t_boss_attr = Config.get_t_boss_attr(Convert.ToInt32(bp.animal.guid));
            if (pre >= 20)
                tp.add_power(t_boss_attr.xl_power);
            else
                tp.add_power(t_boss_attr.patk_power);
        }

        if (is_ft && bp.animal.hp <= 0 && re_bp.animal.hp < 0)
        {
            BattlePlayers.jisha(bp, re_bp);
            BattlePlayers.jisha(re_bp, bp);
        }
        else if (is_ft && re_bp.animal.hp <= 0)
            BattlePlayers.jisha(bp, re_bp);
        else if (bp.animal.hp <= 0)
            BattlePlayers.jisha(re_bp, bp);
        else
        {
            if (BattleOperation.random(0, 100) >= bp.attr_value[21])
            {
                if (t_skill_xiaoguo.link_buff > 0)
                {
                    var yc = re_bp.attr_value[25];
                    yc = yc + re_bp.get_skill_attr_value(t_skill_effect.skill_id, 5);
                    bp.add_buff(t_skill_xiaoguo.link_buff, yc);
                }
                //普攻加buff
                if (t_skill_effect.skill_type == 1)
                {
                    var v = re_bp.get_bskill_value(3);
                    if (v != null)
                    {
                        var yc = re_bp.attr_value[25];
                        yc = yc + re_bp.get_skill_attr_value(t_skill_effect.skill_id, 5);
                        bp.add_buff(v.GetValueOrDefault(), re_bp.attr_value[25], yc);
                    }
                }
            }
        }

        if (t_skill_xiaoguo.jf_type == 1)
        {
            var dis = BattleOperation.toInt(t_skill_xiaoguo.jf_dis * (1 + re_bp.get_skill_attr_value(t_skill_effect.skill_id, 6) / 100.0));
            bp.set_jf(be.effect.r, t_skill_xiaoguo.jf_dis, t_skill_xiaoguo.jf_speed);
        }
        else if (t_skill_xiaoguo.jf_type == 2)
        {
            int dx = be.effect.x - bp.animal.x;
            int dy = be.effect.y - bp.animal.y;
            int dr = BattleOperation.toInt(Math.Atan2(dy, dx) * 180 / Math.PI);
            dr = BattleOperation.checkr(dr);
            int ddis = (int)BattleOperation.get_distance(be.effect.x, be.effect.y, bp.animal.x, bp.animal.y);
            bp.set_jf(dr, ddis, t_skill_xiaoguo.jf_speed);
        }
        else if (t_skill_xiaoguo.jf_type == 3)
        {
            int dx = be.effect.x - bp.animal.x;
            int dy = be.effect.y - bp.animal.y;
            int dr = 180 + BattleOperation.toInt(Math.Atan2(dy, dx) * 180 / Math.PI);
            dr = BattleOperation.checkr(dr);
            int ddis = t_skill_effect.get_range_param(be.re_bp, be.effect.pre_zhen) - (int)BattleOperation.get_distance(be.effect.x, be.effect.y, bp.animal.x, bp.animal.y);
            bp.set_jf(dr, ddis, t_skill_xiaoguo.jf_speed);
        }

        if (bp.animal.type == unit_type.Player)
        {
            var tmp = bp as BattleAnimalPlayer;
            BattlePlayers.fillEffectInfo(tmp, be);
        }
    }

    public static void AttackPenguin(BattleAnimalPlayer bp, BattleAnimal re)
    {
        BattleAnimalMonster re_bp = re as BattleAnimalMonster;
        t_penguin t_penguin = Config.get_t_penguin(re_bp.player.monster_id);
        if (bp.player.is_xueren)
        {
            bp.xunit.action("injured");
            return;
        }

        if (bp.attr.is_yinshen())
            bp.remove_yinshen_buff();

        if (BattleOperation.random(0, 100) < bp.attr_value[22])
        {
            Battle.battle_panel.add_text(bp, "[ffaa00]"+ Config.get_t_script_str("BattlePlayers_001"), 4);
            return;
        }

        //无敌
        if (bp.attr.is_wudi())
        {
            Battle.battle_panel.add_text(bp, "[ffaa00]"+ Config.get_t_script_str("BattlePlayers_001"), 4);
            return;
        }
        //盾挡
        if (bp.player.lattr_value[8] > 0)
        {
            bp.player.lattr_value[8] = bp.player.lattr_value[8] - 1;
            if (bp.player.lattr_value[8] == 0)
                bp.remove_buff(3001);
            Battle.battle_panel.add_text(bp, "[ffaa00]"+ Config.get_t_script_str("BattlePlayers_001"), 4); 
            return;
        }

        //伤害计算
        int attack = re_bp.attr.atk();
        int zjs = 100;
        zjs = zjs + re_bp.attr.zs() - bp.attr.js() + bp.attr_value[90];
        zjs = zjs + re_bp.attr_value[16] - bp.attr_value[17] + re_bp.get_skill_attr_value(re_bp.player.skill_id, 1);

        //技能冷却期间减伤
        if (bp.attr_value[28] != 0 && bp.is_skillcd(bp.player.skill_id))
            zjs = zjs - bp.attr_value[28];

        attack = BattleOperation.toInt(attack * zjs / 100.0);
        double def = bp.attr.def();
        def = def / (100 + def);
        if (BattleOperation.random(0, 100) < re_bp.attr_value[18])
            def = 0;
        attack = BattleOperation.toInt(attack * (1 - def));
        if (attack <= 1)
            attack = 1;

        Battle.battle_panel.add_text(bp, attack.ToString(), 1);
        bp.set_hp(BattleOperation.toInt(bp.animal.hp - attack));

        //反弹技能
        bool is_ft = false;
        if (bp.attr_value[29] != 0 && !re_bp.is_die && !re_bp.attr.is_wudi())
        {
            var fantan = (int)(attack * bp.attr_value[29] / 100);
            re_bp.set_hp(re_bp.animal.hp - fantan);
            Battle.battle_panel.add_text(re_bp, fantan.ToString(), 1);
            is_ft = true;
        }
 
        if (bp.animal.hp <= 0 && !bp.attr.is_wudicd())
        {
            var t = bp.get_talent_value(3);
            if (t != null)
            {
                bp.add_buff(2006, 0, t);
                bp.add_buff(3002);
                bp.set_hp(1);
            }
        }

        if (is_ft && bp.animal.hp <= 0 && re_bp.animal.hp < 0)
        {
            BattlePlayers.jisha(bp, re_bp);
            BattlePlayers.jisha(re_bp, bp);
        }
        else if (is_ft && re_bp.animal.hp <= 0)
            BattlePlayers.jisha(bp, re_bp);
        else if (bp.animal.hp <= 0)
            BattlePlayers.jisha(re_bp, bp);
    }

    public static void Recover(BattleAnimal bp, battle_effect be, int atype = 0)
    {
        if (!BattlePlayers.players.ContainsKey(be.effect.re_guid))
            return;
        var re_bp = be.re_bp;
        var t_skill_effect = Config.get_t_skill_effect(be.effect.id);
        if (t_skill_effect == null)
            return;
        var t_skill_xiaoguo = Config.get_t_skill_xiaoguo(t_skill_effect.link_xiaoguo);
        if (t_skill_xiaoguo == null)
            return;
        int attack = re_bp.attr.atk();
        attack = t_skill_xiaoguo.dmg_per * attack / 100 + t_skill_xiaoguo.dmg_gd;
        if (attack <= 1)
            attack = 1;
        bp.set_hp(bp.animal.hp + attack, true);
        if (t_skill_xiaoguo.link_buff > 0)
        {
            var yc = re_bp.attr_value[25];
            yc = yc + re_bp.get_skill_attr_value(t_skill_effect.skill_id, 5);
            bp.add_buff(t_skill_xiaoguo.link_buff, yc);
        }
    }

    public static void jisha(BattleAnimal re_bp, BattleAnimal bp)
    {
        bool is_boss = false;
        bool is_penguin = false;
        if (bp.animal.type == unit_type.Boss)
        {
            is_boss = true;
            BattlePlayerAI.BossTips = false;
        }
        else if(bp.animal.type == unit_type.Monster)
            is_penguin = true;

        if (!is_boss && !is_penguin && re_bp is BattleAnimalPlayer)
        {
            var tp = re_bp as BattleAnimalPlayer;
            tp.player.sha = tp.player.sha + 1;
            tp.player.lsha = tp.player.lsha + 1;
            if (tp.player.lsha > tp.player.max_lsha)
                tp.player.max_lsha = tp.player.lsha;
            string s = Config.get_t_script_str("BattlePlayers_002");//"击败";
            if (tp.player.lsha >= 8)
                s = Config.get_t_script_str("BattlePlayers_003"); //"[ff9829]超神[-]";
            else if (tp.player.lsha >= 7)
                s = Config.get_t_script_str("BattlePlayers_004"); //"[ab7a3d]横扫千军[-]";
            else if (tp.player.lsha >= 6)
                s = Config.get_t_script_str("BattlePlayers_005"); //"[00ffff]主宰比赛[-]";
            else if (tp.player.lsha >= 5)
                s = Config.get_t_script_str("BattlePlayers_006"); //"[ff6d00]无人能挡[-]";
            else if (tp.player.lsha >= 4)
                s = Config.get_t_script_str("BattlePlayers_007"); //"[ff35ff]光芒四射[-]";
            else if (tp.player.lsha >= 3)
                s = Config.get_t_script_str("BattlePlayers_008"); //"[00ff50]实力初现[-]";
            Battle.battle_panel.add_text(re_bp, s, 5);
            if (bp is BattleAnimalPlayer)
            {
                int rank = 1;
                BattleAnimalPlayer tp1 = bp as BattleAnimalPlayer;
                for (int i = 0; i < BattlePlayers.players_list.Count; i++)
                {
                    var bp1 = BattlePlayers.players_list[i];
                    if (bp1.player.score > tp.player.score)
                        rank = rank + 1;
                }
                BattlePlayers.BeforeKills(tp, tp1, rank);
            }
        }



        if (re_bp is BattleAnimalPlayer && !re_bp.is_die)
        {
            var tp = re_bp as BattleAnimalPlayer;
            if (bp.animal.type == unit_type.Player)
            {
                var tmp = bp as BattleAnimalPlayer;
                var t_exp = Config.get_t_battle_exp(tmp.player.level);
                var gexp = t_exp.gexp;
                gexp = BattleOperation.toInt(gexp * (1 + tmp.attr_value[71] / 100.0));
                tp.add_exp(gexp);
                StringBuilder num_s = new StringBuilder(Config.get_t_script_str("BattlePlayers_009"));
                if (tp.player.lsha >= 8)    //增加得分和连杀数有关
                {
                    tp.add_score(8 * 1000);
                    num_s.Append(8 * 1000);
                }
                else
                {
                    tp.add_score(tp.player.lsha * 1000);
                    num_s.Append(tp.player.lsha * 1000);
                }
                Battle.battle_panel.add_text(tp,num_s.ToString(), 5);
            }

            tp.jisha_value();
            var v = tp.attr_value[19] + tp.attr.max_hp() * BattleOperation.toInt(tp.attr_value[20] / 100.0);
            if (v > 0)
                tp.set_hp(tp.player.hp + v, true);

            if (!is_boss && bp is BattleAnimalPlayer)
            {
                for (int i = 0; i < bp.animal.buffs.Count; i++)
                {
                    if ((bp.animal.buffs[i] == 4001) && (bp.animal.buffs_time[i] > BattlePlayers.zhen))
                    {
                        bp.remove_buff_effect(4001);
                        re_bp.add_buff(4001, null, null, bp.animal.buffs_time[i]);
                        break;
                    }
                }
            }
        }

        if (Battle.is_newplayer_guide && is_boss)
        {
            var tp = bp as BattleAnimalBoss;
            var t_boss_attr = Config.get_t_boss_attr(tp.player.boss_id);
            BattlePlayers.add_random_item(t_boss_attr.buff_id, bp.animal.x, bp.animal.y, t_boss_attr.max_dis);
        }

        //新手教程中死亡掉落去除
        if (!Battle.is_newplayer_guide)
        {
            if (is_boss)
            {
                var tp = bp as BattleAnimalBoss;
                var t_boss_attr = Config.get_t_boss_attr(tp.player.boss_id);
                var bexp = t_boss_attr.bexp;
                while (bexp > 0)
                {
                    var r = BattleOperation.random(1, t_boss_attr.maxb);
                    if (r * 20 > bexp)
                    {
                        r = BattleOperation.toInt(bexp / 20.0);
                        bexp = 0;
                    }
                    else
                        bexp = bexp - r * 20;
                    BattlePlayers.add_random_item1(bp.animal.x, bp.animal.y, r, t_boss_attr.max_dis, 3);
                }
                BattlePlayers.add_random_item(t_boss_attr.buff_id, bp.animal.x, bp.animal.y, t_boss_attr.max_dis);

                int p_num = BattleOperation.random(0, 100);
                if (t_boss_attr.b_pro >= p_num)
                {
                    for (int mm = 0; mm < t_boss_attr.b_amount; mm++)
                    {
                        int id = BattleOperation.random(200, 209 + 1);
                        BattlePlayers.add_random_item(id, bp.animal.x, bp.animal.y, t_boss_attr.max_dis);
                    }
                }
            }
            else if (is_penguin)
            {
                var tp = bp as BattleAnimalMonster;
                var t_penguin = Config.get_t_penguin(tp.player.monster_id);
                var bexp = t_penguin.exp;
                while (bexp > 0)
                {
                    var r = BattleOperation.random(1, t_penguin.max_bl + 1);
                    if (r * 20 > bexp)
                    {
                        r = BattleOperation.toInt(bexp / 20.0);
                        bexp = 0;
                    }
                    else
                        bexp = bexp - r * 20;
                    BattlePlayers.add_random_item1(bp.animal.x, bp.animal.y, r, t_penguin.max_dis);
                }

                int p_num = BattleOperation.random(0, 100);
                if (t_penguin.b_pro >= p_num)
                {
                    for (int mm = 0; mm < t_penguin.b_amount; mm++)
                    {
                        int id = BattleOperation.random(200, 209 + 1);
                        BattlePlayers.add_random_item(id, bp.animal.x, bp.animal.y, t_penguin.max_dis);
                    }
                }
            }
            else
            {
                var tmp = bp as BattleAnimalPlayer;
                var t_exp = Config.get_t_battle_exp(tmp.player.level);
                var bexp = t_exp.bexp;
                if (BattlePlayers.battle_type == 1)
                    bexp = BattleOperation.toInt(bexp * 0.2);
                while (bexp > 0)
                {
                    var r = BattleOperation.random(1, t_exp.max_bl);
                    if (r * 20 > bexp)
                    {
                        r = BattleOperation.toInt(bexp / 20.0);
                        bexp = 0;
                    }
                    else
                        bexp = bexp - r * 20;
                    BattlePlayers.add_random_item1(bp.animal.x, bp.animal.y, r, t_exp.max_dis);
                }

                for (int i = 0; i < t_exp.item_num2; i++)
                {
                    var r = BattleOperation.random(0, 100);
                    if (r < t_exp.item_rate2)
                    {
                        var iid = BattleOperation.random(200, 209 + 1);
                        BattlePlayers.add_random_item(iid, bp.animal.x, bp.animal.y);
                    }
                }
            }
        }

        bp.is_die = true;
        bp.animal.death_time = BattlePlayers.zhen;
        bp.yy(2);

        if (bp is BattleAnimalPlayer)
        {
            var tp = bp as BattleAnimalPlayer;
            tp.player.die = tp.player.die + 1;
            tp.player.lsha = 0;
            var t_exp = Config.get_t_battle_exp(tp.player.level);
            var eexp = tp.player.exp - BattleOperation.toInt((tp.player.exp - t_exp.exp) / 3.0);
            tp.player.exp = eexp;

            tp.player.score = BattleOperation.toInt(tp.player.score * 2 / 3.0);
            int final_score_level = 1;
            for (int i = 1; i <= Config.max_score_level; i++)
            {
                var t_bodysize = Config.get_t_bodysize(i);
                if (tp.player.score >= t_bodysize.score && final_score_level < t_bodysize.id)
                    final_score_level = t_bodysize.id;
            }
            tp.player.score_level = final_score_level;
        }

        if (bp.animal.type == unit_type.Player)
        {
            if (re_bp.animal.type == unit_type.Player)
            {
                var tmp = re_bp as BattleAnimalPlayer;
                Battle.battle_panel.show_kill(bp, tmp, tmp.player.lsha);
            }
            else if (re_bp.animal.type == unit_type.Boss)
            {
                var tmp = re_bp as BattleAnimalBoss;
                Battle.battle_panel.show_kill(bp, tmp, 1);
            }
        }
        else if(bp.animal.type == unit_type.Boss)
        {
            if (re_bp.animal.type == unit_type.Player)
            {
                var tmp = re_bp as BattleAnimalPlayer;
                Battle.battle_panel.show_kill(bp, tmp,1);
            }
        }

        if (BattlePlayers.me != null)
        {
            if (Battle.is_newplayer_guide)
            {

            }
            else
            {
                if (BattlePlayers.me.animal.guid == bp.animal.guid)
                    Battle.battle_panel.show_die(re_bp);
            }
        }
    }

    public static void AddOperation(msg_battle_op msg)
    {
        if (!BattlePlayers.start)
            return;
        var op = new battle_message_link() { msg = msg, nextp = null };
        if (BattlePlayers.operations_head == null)
        {
            BattlePlayers.operations_head = op;
            BattlePlayers.operations_tail = op;
        }
        else
        {
            BattlePlayers.operations_tail.nextp = op;
            BattlePlayers.operations_tail = op;
        }
    }

    public static void RemoveOperation()
    {
        if (BattlePlayers.operations_head == null)
            return;
        if (BattlePlayers.operations_head.nextp == null)
        {
            BattlePlayers.operations_head = null;
            BattlePlayers.operations_tail = null;
        }
        else
            BattlePlayers.operations_head = BattlePlayers.operations_head.nextp;
    }

    public static void do_iteminit()
    {
        BattlePlayers.init_item = true;
        BattlePlayers.do_check_item();
        BattlePlayers.init_ai();
    }

    public static void do_operation()
    {
        while (BattlePlayers.operations_head != null && BattlePlayers.operations_head.msg.zhen == BattlePlayers.zhen)
        {
            var op = BattlePlayers.operations_head.msg.opcode;
            var msg = BattlePlayers.operations_head.msg;
            if (op == (int)e_battle_msg.MSG_BATTLE_MOVE)
                BattlePlayers.Move(msg);
            else if (op == (int)e_battle_msg.MSG_BATTLE_STOP)
                BattlePlayers.Stop(msg);
            else if (op == (int)e_battle_msg.MSG_BATTLE_ATTACKR)
                BattlePlayers.Attackr(msg);
            else if (op == (int)e_battle_msg.MSG_BATTLE_IN)
                BattlePlayers.In(msg);
            else if (op == (int)e_battle_msg.MSG_BATTLE_OUT)
                BattlePlayers.Out(msg);
            else if (op == (int)e_battle_msg.MSG_BATTLE_PRERELEASE)
                BattlePlayers.PreRelease(msg);
            else if (op == (int)e_battle_msg.MSG_BATTLE_RELEASE)
                BattlePlayers.Release(msg);
            else if (op == (int)e_battle_msg.MSG_BATTLE_TALENT)
                BattlePlayers.Talent(msg);
            else if (op == (int)e_battle_msg.MSG_BATTLE_STATE)
                BattlePlayers.BattleState(msg);
            else if (op == (int)e_battle_msg.MSG_BATTLE_CODE)
                BattlePlayers.BattleCode(msg);
            else if (op == (int)e_battle_msg.MSG_BATTLE_INSIDE_MSG)
                BattlePlayers.BattleInsideMsg(msg);
            BattlePlayers.RemoveOperation();
        }
    }

    public static void do_effect()
    {
        BattlePlayers.do_effect1();
        for (int i = 0; i < BattlePlayers.effects_list.Count; i++)
        {
            var be = BattlePlayers.effects_list[i];
            bool need_grid = false;
            var t_skill_effect = Config.get_t_skill_effect(be.effect.id);
            if (t_skill_effect != null)
            {
                if (t_skill_effect.type == 1 || t_skill_effect.type == 2)
                {
                    var p = BattleOperation.add_distance(be.effect.x, be.effect.y, be.effect.r, t_skill_effect.get_fx_speed(be.re_bp), BattlePlayers.TICK);
                    be.effect.x = p[0];
                    be.effect.y = p[1];
                    need_grid = true;
                    var dis = BattleOperation.get_distance(be.effect.sx, be.effect.sy, be.effect.x, be.effect.y);
                    be.pt = dis * 1.0f / be.effect.len;
                }

                if (need_grid)
                    BattleGrid.add(grid_type.et_effect, be.effect.tid, be.effect.x, be.effect.y);
            }
        }
    }

    public static void do_effect1()
    {
        List<int> edels = new List<int>();
        for (int i = 0; i < BattlePlayers.effects_list.Count; i++)
        {
            var be = BattlePlayers.effects_list[i];
            var t_skill_effect = Config.get_t_skill_effect(be.effect.id);
            if (t_skill_effect != null)
            {
                bool flag = false;
                if (be.re_bp == null)
                {
                    flag = true;
                    edels.Add(be.effect.tid);
                }

                if (BattlePlayers.zhen % (BattlePlayers.TNUM / 10) == 0)
                {
                    //是否撞到障碍物
                    if (!flag && t_skill_effect.type == 1 && t_skill_effect.is_zaxs == 1)
                    {
                        if (!BattleGrid.can_effect_move(be.effect.x, be.effect.y))
                        {
                            flag = true;
                            edels.Add(be.effect.tid);
                            if (!String.IsNullOrEmpty(t_skill_effect.xiaoshi_effect1))
                            {
                                float h = 0.1f;
                                if (be.obj != null)
                                {
                                    float ox, oy, oz;
                                    BattlePlayers.bobjpool.get_localPosition(be.objid, out ox, out oy, out oz);
                                    h = oy;
                                }
                                string efname = t_skill_effect.xiaoshi_effect1;
                                if (be.re_bp is BattleAnimalPlayer && t_skill_effect.skill_type == 1)
                                {
                                    var tmp = be.re_bp as BattleAnimalPlayer;
                                    if(tmp.pskill_fashion != null)
                                        efname = tmp.pskill_fashion.hit_effect;
                                }     
                                BattlePlayers.Disappear(be.effect.x, be.effect.y, h, be.effect.r, t_skill_effect.get_effect_scale(be.re_bp, be.effect.pre_zhen), efname);
                            }
                        }
                    }
                    //是否打到销毁器
                    if (!flag && t_skill_effect.skill_type == 1)
                    {
                        string[] tids = null;
                        BattleGrid.get(grid_type.et_effect, be.effect.x, be.effect.y, out tids);
                        for (int j = 0; j < tids.Length; j++)
                        {
                            int t = Convert.ToInt32(tids[j]);
                            if (BattlePlayers.effects.ContainsKey(t))
                            {
                                var be1 = BattlePlayers.effects[t];
                                var t_skill_effect1 = Config.get_t_skill_effect(be1.effect.id);
                                if (t_skill_effect1 != null && t_skill_effect1.target_type == 2 && be.effect.tid != be1.effect.tid && be.effect.re_guid != be1.effect.re_guid)
                                {
                                    if (t_skill_effect1.type == 1)
                                    {
                                        if (!BattleOperation.check_distance(be1.effect.x, be1.effect.y, be.effect.x, be.effect.y, t_skill_effect.get_range_param(be.re_bp, be.effect.pre_zhen) / 2 + t_skill_effect1.get_range_param(be1.re_bp, be.effect.pre_zhen) / 2))
                                        {
                                            flag = true;
                                            if (be1.effect.re_guid == self.guid)
                                            {
                                                var sk_id = t_skill_effect.skill_id;
                                                if (!be1.destroyEffects.ContainsKey(sk_id))
                                                    be1.destroyEffects.Add(sk_id, 1);
                                                else
                                                    be1.destroyEffects[sk_id] = be1.destroyEffects[sk_id] + 1;
                                            }
                                            edels.Add(be.effect.tid);
                                            if (!String.IsNullOrEmpty(t_skill_effect.xiaoshi_effect1))
                                            {
                                                var h = 0.1f;
                                                if (be.obj != null)
                                                {
                                                    float ox, oy, oz;
                                                    BattlePlayers.bobjpool.get_localPosition(be.objid, out ox, out oy, out oz);
                                                    h = oy;
                                                }
                                                var efname = t_skill_effect.xiaoshi_effect1;
                                                if (be.re_bp is BattleAnimalPlayer && t_skill_effect.skill_type == 1)
                                                {
                                                    var tmp = be.re_bp as BattleAnimalPlayer;
                                                    if (tmp.pskill_fashion != null)
                                                        efname = tmp.pskill_fashion.down_effect;
                                                }      
                                                BattlePlayers.Disappear(be.effect.x, be.effect.y, h, be.effect.r, t_skill_effect.get_effect_scale(be.re_bp, be.effect.pre_zhen), efname);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //是否到最远距离
                    if (!flag && (t_skill_effect.type == 1 || t_skill_effect.type == 2))
                    {
                        if (BattleOperation.check_distance(be.effect.sx, be.effect.sy, be.effect.x, be.effect.y, be.effect.len))
                        {
                            flag = true;
                            edels.Add(be.effect.tid);
                            if (!String.IsNullOrEmpty(t_skill_effect.xiaoshi_effect))
                            {
                                string efname = t_skill_effect.xiaoshi_effect;
                                if (be.re_bp is BattleAnimalPlayer && t_skill_effect.skill_type == 1)
                                {
                                    var tmp = be.re_bp as BattleAnimalPlayer;
                                    if (tmp.pskill_fashion != null)
                                        efname = tmp.pskill_fashion.down_effect;
                                }  
                                BattlePlayers.Disappear(be.effect.x, be.effect.y, 0.1f, be.effect.r, t_skill_effect.get_effect_scale(be.re_bp, be.effect.pre_zhen), efname);
                            }

                            if (t_skill_effect.link_effect > 0)
                                BattlePlayers.add_effect(be.effect.re_tid, be.effect.camp, t_skill_effect.link_effect, be.effect.x, be.effect.y, be.effect.x, be.effect.y, be.effect.r, be.effect.re_guid, 0);
                            if (t_skill_effect.link_effect1 > 0)
                            {
                                var p = BattleOperation.add_distance2(be.effect.x, be.effect.y, be.effect.r, be.effect.len);
                                BattlePlayers.add_effect(be.effect.re_tid, be.effect.camp, t_skill_effect.link_effect1, be.effect.x, be.effect.y, p[0], p[1], be.effect.r, be.effect.re_guid, 0);
                            }
                        }
                    }
                }

                bool yxg = true;
                //是否到时间
                if (!flag && (t_skill_effect.type == 4 || t_skill_effect.type == 5 || t_skill_effect.type == 7 || t_skill_effect.type == 10))
                {
                    if ((BattlePlayers.zhen - be.effect.time) * BattlePlayers.TICK > t_skill_effect.sh_time)
                    {
                        //伤害到时间
                        yxg = false;
                        if (be.effect.state == 0)
                        {
                            be.effect.state = 1;
                            if (t_skill_effect.link_effect > 0)
                                BattlePlayers.add_effect(be.effect.re_tid, be.effect.camp, t_skill_effect.link_effect, be.effect.x, be.effect.y, be.effect.x, be.effect.y, be.effect.r, be.effect.re_guid, 0); 
                        }
                    }

                    if ((BattlePlayers.zhen - be.effect.time) * BattlePlayers.TICK > t_skill_effect.time)
                    {
                        flag = true;
                        edels.Add(be.effect.tid);
                    }
                }
                // 多段伤害时间重置
                if (!flag && t_skill_effect.dd_type == 1)
                {
                    if ((BattlePlayers.zhen - be.effect.dd_time) * BattlePlayers.TICK > t_skill_effect.dd_jg)
                    {
                        be.effect.dd_time = BattlePlayers.zhen;
                        be.effect.hit_guids.Clear();
                    }
                }
                //是否打到人
                if (!flag && t_skill_effect.target_type > 0 && yxg)
                {
                    string[] guids = null;
                    int grid = 1;
                    if (t_skill_effect.type == 4 || t_skill_effect.type == 5)
                        grid = t_skill_effect.get_range(be.re_bp, be.effect.pre_zhen);
                    else if (t_skill_effect.type == 7)
                        grid = t_skill_effect.get_range_param(be.re_bp, be.effect.pre_zhen);
                    BattleGrid.get(grid_type.et_player, be.effect.x, be.effect.y, out guids, grid);
                    for (int j = 0; j < guids.Length; j++)
                    {
                        var g = guids[j];
                        if (BattlePlayers.players.ContainsKey(g))
                        {
                            var bp = BattlePlayers.players[g];
                            bool nflag = false;
                            int atype = 1;
                            if (t_skill_effect.target_type == 1 || t_skill_effect.target_type == 2)
                            {
                                atype = 1;
                                nflag = (bp.animal.guid != be.effect.re_guid) && (bp.animal.camp != be.effect.camp || bp.animal.is_xueren);
                            }
                            else if (t_skill_effect.target_type == 3)
                            {
                                atype = 2;
                                nflag = bp.animal.camp == be.effect.camp;
                            }

                            if (!bp.is_die && nflag)
                            {
                                if (!BattleOperation.table_has<string>(be.effect.hit_guids, bp.animal.guid))
                                {
                                    if (t_skill_effect.type == 1)
                                    {
                                        if (!BattleOperation.check_distance(bp.animal.x, bp.animal.y, be.effect.x, be.effect.y, bp.ur() + t_skill_effect.get_range_param(be.re_bp, be.effect.pre_zhen) * 1.0 / 2))
                                        {
                                            if (t_skill_effect.is_zaxs == 1)
                                                edels.Add(be.effect.tid);
                                            else
                                                be.effect.hit_guids.Add(bp.animal.guid);
                                            BattlePlayers.Attack(bp, be, atype);
                                            break;
                                        }
                                    }
                                    else if (t_skill_effect.type == 4)
                                    {
                                        if (BattleOperation.IsCircleIntersectFan(bp.animal.x, bp.animal.y, (int)bp.ur(), be.effect.x, be.effect.y, be.effect.r, t_skill_effect.get_range(be.re_bp, be.effect.pre_zhen), t_skill_effect.get_range_param(be.re_bp, be.effect.pre_zhen)))
                                        {
                                            be.effect.hit_guids.Add(bp.animal.guid);
                                            BattlePlayers.Attack(bp, be, atype);
                                            break;
                                        }
                                    }
                                    else if (t_skill_effect.type == 5)
                                    {
                                        var per = (BattlePlayers.zhen - be.effect.time) * BattlePlayers.TICK * 1.0f / t_skill_effect.sh_time;
                                        var r1 = BattleOperation.toInt(t_skill_effect.get_range(be.re_bp, be.effect.pre_zhen) * per * 1.0);
                                        var r2 = BattleOperation.toInt(t_skill_effect.get_range(be.re_bp, be.effect.pre_zhen) * per * 1.0) - 10000;

                                        if (r2 < 0)
                                            r2 = 0;
                                        var f1 = BattleOperation.IsCircleIntersectFan(bp.animal.x, bp.animal.y, (int)bp.ur(), be.effect.x, be.effect.y, be.effect.r, r1, t_skill_effect.get_range_param(be.re_bp, be.effect.pre_zhen));
                                        var f2 = BattleOperation.IsCircleIntersectFan(bp.animal.x, bp.animal.y, (int)bp.ur(), be.effect.x, be.effect.y, be.effect.r, r2, t_skill_effect.get_range_param(be.re_bp, be.effect.pre_zhen));
                                        if (f1 && !f2)
                                        {
                                            be.effect.hit_guids.Add(bp.animal.guid);
                                            BattlePlayers.Attack(bp, be, atype);
                                            break;
                                        }
                                    }
                                    else if (t_skill_effect.type == 7)
                                    {
                                        if (!BattleOperation.check_distance(bp.animal.x, bp.animal.y, be.effect.x, be.effect.y, (long)bp.ur() + t_skill_effect.get_range_param(be.re_bp, be.effect.pre_zhen)))
                                        {
                                            be.effect.hit_guids.Add(bp.animal.guid);
                                            BattlePlayers.Attack(bp, be, atype);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < edels.Count; i++)
            BattlePlayers.del_effect(edels[i]);
        BattlePlayers.change_effect_list();
    }

    public static void do_effect_follow()
    {
        for (int ll = 0; ll < BattlePlayers.effects_list.Count; ll++)
        {
            var be = BattlePlayers.effects_list[ll];
            var t_skill_effect = Config.get_t_skill_effect(be.effect.id);
            if (t_skill_effect != null && t_skill_effect.type == 10 && !String.IsNullOrEmpty(be.effect.follow_guid))
            {
                if (BattlePlayers.players.ContainsKey(be.effect.follow_guid))
                {
                    var fbp = BattlePlayers.players[be.effect.follow_guid];
                    var need_grid = fbp.animal.x != be.effect.x || fbp.animal.y != be.effect.y;
                    if (need_grid)
                    {
                        be.effect.x = fbp.animal.x;
                        be.effect.y = fbp.animal.y;
                        BattleGrid.add(grid_type.et_effect, be.effect.tid, be.effect.x, be.effect.y);
                    }
                }
            }
        }

        for (int ll = 0; ll < BattlePlayers.effects_list.Count; ll++)
        {
            var be = BattlePlayers.effects_list[ll];
            be.last.Add(new List<object>() { be.effect.x, be.effect.y, be.pt });
        }
    }

    public static void do_item()
    {
        for (int ll = 0; ll < BattlePlayers.players_list.Count; ll++)
        {
            var bp = BattlePlayers.players_list[ll];
            if (!bp.is_die)
            {
                string[] tids = null;
                BattleGrid.get(grid_type.et_item, bp.player.x, bp.player.y, out tids);
                int num = 0;
                for (int i = 0; i < tids.Length; i++)
                {
                    int g = Convert.ToInt32(tids[i]);
                    var bi = BattlePlayers.items[g];
                    var xf = 10000 * (1 + bp.attr_value[33] / 100);
                    var yy = false;
                    var t_battle_item = Config.get_t_battle_item(bi.item.id);
                    if (t_battle_item.type == 4)
                    {
                        if (bp.player.skill_id == 0)
                            yy = false;
                        else
                        {
                            if (bp.player.skill_id == t_battle_item.skill && bp.player.skill_level < 3)
                                yy = false;
                            else
                                yy = true;
                        }
                    }
                    
                    if (!yy && bi.item.zhen + BattlePlayers.TNUM < BattlePlayers.zhen && !BattleOperation.check_distance(bp.player.x, bp.player.y, bi.item.x, bi.item.y, (long)bp.ur() + xf))
                    {
                        //记录自己一场战斗的时候 切换了多少技能
                        bool sign = false;
                        var tb = Config.get_t_battle_item(bi.item.id);
                        if (tb.type == 4 && bp.player.skill_id > 0) //技能
                            sign = true;
                        else if (tb.id == 2 && bp.player.lattr_value[8] > 0) //盾
                            sign = true;

                        if (!sign)
                        {
                            if (bp.player.guid == self.guid)
                            {
                                var s_pos = LuaHelper.GetMapManager().WorldToScreenPoint(BattlePlayers.me.objt.position);
                                bp.get_item(bi);
                            }
                            else
                                bp.get_item(bi);
                            num = num + 1;

                            BattlePlayers.del_item(bi.item.tid, bp);
                            if (num >= 3)
                                break;
                        }
                    }
                }
            }
        }
    }

    public static void do_player()
    {
        for (int ll = 0; ll < BattlePlayers.players_list.Count; ll++)
        {
            var bp = BattlePlayers.players_list[ll];
            BattlePlayers.do_player1(bp);
        }

        for (int ll = 0; ll < BattlePlayers.penguin_list.Count; ll++)
        {
            var bp = BattlePlayers.penguin_list[ll];
            BattlePlayers.do_player1(bp);
        }

        if (BattlePlayers.Boss != null)
            BattlePlayers.do_player1(BattlePlayers.Boss);

        for (int ll = 0; ll < BattlePlayers.players_list.Count; ll++)
        {
            var bp = BattlePlayers.players_list[ll];
            bp.last.Add(new List<object>() { bp.player.x, bp.player.y, bp.player.is_jf, bp.jf_pt });
        }

        for (int ll = 0; ll < BattlePlayers.penguin_list.Count; ll++)
        {
            var bp = BattlePlayers.penguin_list[ll];
            bp.last.Add(new List<object>() { bp.animal.x, bp.animal.y, bp.animal.is_jf, bp.jf_pt });
        }

        if (BattlePlayers.Boss != null)
        {
            var bp = BattlePlayers.Boss;
            bp.last.Add(new List<object>() { bp.player.x, bp.player.y, bp.player.is_jf, bp.jf_pt });
        }
    }
    public static void do_player1(BattleAnimal bp)
    {
        bool need_tick = ((BattlePlayers.zhen % 4) == 0);
        bool need_grid = false;
        if (need_tick)
            bp.do_buff();
        bp.do_tick();
        bool flag = false;
        if (!bp.can_do())
        {
            bp.animal.re_state = 0;
            flag = true;
        }

        //移动
        if (bp.animal.is_jf)
        {
            int[] p = BattleOperation.add_distance(bp.animal.x, bp.animal.y, bp.animal.jf_r, bp.animal.jf_speed, BattlePlayers.TICK);
            bp.animal.x = p[0];
            bp.animal.y = p[1];
            need_grid = true;
            var dis = BattleOperation.get_distance(bp.animal.jf_sx, bp.animal.jf_sy, bp.animal.x, bp.animal.y);
            if (bp.jf_len == 0)
                bp.jf_pt = 1;
            else
                bp.jf_pt = dis * 1.0f / bp.jf_len;

            if (bp.jf_pt >= 1)
            {
                bp.animal.x = bp.animal.jf_xx;
                bp.animal.y = bp.animal.jf_yy;
                bp.animal.is_jf = false;
            }
        }
        else if (bp.animal.type == unit_type.Player && (bp as BattleAnimalPlayer).player.is_xueren)
        {
            var tp = bp as BattleAnimalPlayer;
            var xueren_t = 10000;
            if (Battle.is_newplayer_guide)
                xueren_t = 20000;
            if ((BattlePlayers.zhen - tp.player.xueren_zhen) * BattlePlayers.TICK >= xueren_t)
                BattlePlayers.Release1(tp, 300101, 0, 0, 0);
        }
        else if (!flag)
        {
            if (bp.animal.re_state > 0)
            {
                t_skill t_skill = Config.get_t_skill(bp.animal.re_id, bp.animal.re_level, bp.get_skill_level_add(bp.animal.re_id));
                if (t_skill != null)
                {
                    if (t_skill.type == 1)
                    {
                        bp.animal.re_time = bp.animal.re_time + BattlePlayers.TICK * (10000 + bp.attr.aspeed()) / 10000;
                        if (bp.animal.type == unit_type.Player)
                        {
                            var tp = bp as BattleAnimalPlayer;
                            var pre = BattleOperation.calc_pre(tp,tp.player.re_pre_zhen1);
                            if (pre == 100)
                                t_skill = Config.get_t_skill(tp.player.re_id, 2, 0);
                        }
                    }
                    else
                        bp.animal.re_time = bp.animal.re_time + BattlePlayers.TICK;
                    if (t_skill.hy_td_time > 0)
                        bp._lock = true;
                    if (bp.animal.re_state == 1)
                    {
                        bp.animal.re_state = 2;
                        if (t_skill.action != "")
                        {
                            if (t_skill.type == 1)
                            {
                                float ass = 1 + bp.attr.aspeed() / 10000.0f;
                                if (t_skill.action == "attack01")
                                {
                                    if (bp.aaction)
                                    {
                                        bp.aaction = false;
                                        BattlePlayers.action(bp, t_skill.action, ass);
                                    }
                                    else
                                    {
                                        bp.aaction = true;
                                        BattlePlayers.action(bp, t_skill.action + "a", ass);
                                    }
                                }
                                else
                                    BattlePlayers.action(bp, t_skill.action);
                            }
                            else
                            {
                                BattlePlayers.action(bp, t_skill.action, 0, 1);
                            }
                        }
                    }

                    if (bp.animal.re_state == 2)
                    {
                        if (bp.animal.re_time > t_skill.qy_action_time)
                        {
                            bp.animal.re_time = bp.animal.re_time - t_skill.qy_action_time;
                            bp.animal.re_state = 4;
                            if (t_skill.wy_time > 0)
                                bp.animal.re_state = 3;

                            if (t_skill.link_effect > 0)
                            {
                                t_skill_effect t_skill_effect = Config.get_t_skill_effect(t_skill.link_effect);
                                int[] p = new int[] { bp.animal.x, bp.animal.y };
                                if (t_skill_effect.release_pos == 1)
                                {
                                    p = BattleOperation.add_distance2(bp.animal.x, bp.animal.y, bp.animal.re_r, bp.ur() * 2.5);
                                    p = BattleOperation.add_distance2(p[0], p[1], BattleOperation.checkr(bp.animal.re_r - 90), bp.ur() / 2);
                                }
                                if (t_skill_effect.type == 1)
                                {
                                    var num = 1;
                                    var dr = 20;
                                    if (t_skill.type == 1)
                                    {
                                        var duo = bp.attr_value[91];
                                        if (duo > 0 && BattleOperation.random(0, 100) < duo)
                                            num = 3;

                                    }
                                    else if (t_skill_effect.fl_num > 0)
                                    {
                                        num = t_skill_effect.fl_num;
                                        dr = t_skill_effect.fl_r;
                                    }

                                    for (int i = 1; i <= num; i++)
                                    {
                                        var r = bp.animal.re_r - 10 * (num - 1) + (i - 1) * dr;
                                        r = BattleOperation.checkr(r);
                                        BattlePlayers.add_effect(bp.animal.re_tid, bp.animal.camp, t_skill.link_effect, p[0], p[1], bp.animal.re_x, bp.animal.re_y, r, bp.animal.guid, 0, BattleOperation.toInt(bp.ur() * 2.5));
                                    }
                                }
                                else
                                    BattlePlayers.add_effect(bp.animal.re_tid, bp.animal.camp, t_skill.link_effect, p[0], p[1], bp.animal.re_x, bp.animal.re_y, bp.animal.re_r, bp.animal.guid, 0, BattleOperation.toInt(bp.ur() * 2.5));
                                BattlePlayers.change_effect_list();
                            }
                            else
                            {
                                if (t_skill.release_type == 100)
                                {
                                    string[] guids;
                                    BattleGrid.get(grid_type.et_player, bp.animal.x, bp.animal.y, out guids,t_skill.get_range(bp));
                                    for (int i = 0; i < guids.Length; i++)
                                    {
                                        var army = BattlePlayers.players[guids[i]];
                                        if (army.animal.type == unit_type.Player)
                                        {
                                            var tp = army as BattleAnimalPlayer;
                                            if(!tp.is_die)
                                                BattlePlayers.AttackPenguin(tp, bp);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (bp.animal.re_state == 3)
                    {
                        if (bp.animal.re_time > t_skill.wy_time)
                        {
                            bp.animal.re_time = bp.animal.re_time - t_skill.wy_time;
                            bp.animal.re_state = 4;
                        }
                        else
                        {
                            var tmp = BattleOperation.add_distance(bp.animal.x, bp.animal.y, bp.animal.re_r, t_skill.wy_speed, BattlePlayers.TICK);
                            var xx = tmp[0];
                            var yy = tmp[1];
                            if (BattleGrid.can_move(xx, yy))
                            {
                                bp.animal.x = xx;
                                bp.animal.y = yy;
                                need_grid = true;
                            }
                        }
                    }

                    if (bp.animal.re_state == 4)
                    {
                        if (bp.animal.re_time > t_skill.hy_td_time)
                        {
                            bp.animal.re_time = bp.animal.re_time - t_skill.hy_td_time;
                            bp.animal.re_state = 0;
                            bp._lock = false;
                            if (!bp.animal.is_move)
                                bp.animal.r = bp.animal.re_r;
                        }
                    }
                }
            }

            if ((!bp._lock || bp.animal.re_state == 0) && bp.animal.is_move)
            {
                int x = bp.animal.x;
                int y = bp.animal.y;
                int r = bp.animal.r;
                int speed = BattleOperation.toInt(bp.attr.speed() / 1.2);
                int[] tmp = BattleOperation.add_distance(x, y, r, speed, BattlePlayers.TICK);
                int xx = tmp[0];
                int yy = tmp[1];
                if (BattleGrid.can_move(xx, yy))
                {
                    bp.animal.x = xx;
                    bp.animal.y = yy;
                    need_grid = true;
                    bp.animal.r_py = 0;
                }
                else
                {
                    if (bp.animal.r_py == 0)
                    {
                        for (int i = 1; i <= 4; i++)
                        {
                            int new_r = BattleOperation.checkr(r + i * 30);
                            tmp = BattleOperation.add_distance(x, y, new_r, speed, BattlePlayers.TICK);
                            xx = tmp[0];
                            yy = tmp[1];
                            if (BattleGrid.can_move(xx, yy))
                            {
                                bp.animal.x = xx;
                                bp.animal.y = yy;
                                need_grid = true;
                                bp.animal.r_py = i * 30;
                                break;
                            }
                            new_r = BattleOperation.checkr(r - i * 30);
                            tmp = BattleOperation.add_distance(x, y, new_r, speed, BattlePlayers.TICK);
                            xx = tmp[0];
                            yy = tmp[1];
                            if (BattleGrid.can_move(xx, yy))
                            {
                                bp.animal.x = xx;
                                bp.animal.y = yy;
                                need_grid = true;
                                bp.animal.r_py = -i * 30;
                                break;
                            }
                        }
                    }
                    else if (bp.animal.r_py > 0)
                    {
                        for (int i = 1; i <= 3; i++)
                        {
                            var new_r = BattleOperation.checkr(r + i * 30);
                            tmp = BattleOperation.add_distance(x, y, new_r, speed, BattlePlayers.TICK);
                            xx = tmp[0];
                            yy = tmp[1];
                            if (BattleGrid.can_move(xx, yy))
                            {
                                bp.animal.x = xx;
                                bp.animal.y = yy;
                                need_grid = true;
                                bp.animal.r_py = i * 30;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= 3; i++)
                        {
                            var new_r = BattleOperation.checkr(r - i * 30);
                            tmp = BattleOperation.add_distance(x, y, new_r, speed, BattlePlayers.TICK);
                            xx = tmp[0];
                            yy = tmp[1];
                            if (BattleGrid.can_move(xx, yy))
                            {
                                bp.animal.x = xx;
                                bp.animal.y = yy;
                                need_grid = true;
                                bp.animal.r_py = -i * 30;
                                break;
                            }
                        }
                    }
                }
            }
        }

        if (need_grid)
        {
            BattleGrid.add(grid_type.et_player, bp.animal.guid, bp.animal.x, bp.animal.y);
            bp.cao = BattleGrid.get_cao(bp.animal.x, bp.animal.y);
        }
    }

    //保存这场游戏中同一个队伍的成员guid
    public static void change_player_camp_member()
    {
        BattlePlayers.Camps.Clear();
        var dicSort = from objDic in BattlePlayers.players orderby objDic.Key select objDic;
        foreach (KeyValuePair<string, BattleAnimal> kvp in dicSort)
        {
            var bp = kvp.Value;
            if (bp.animal.type == unit_type.Player)
            {
                int camp_id = bp.animal.camp;
                if (!BattlePlayers.Camps.ContainsKey(camp_id))
                    BattlePlayers.Camps.Add(camp_id, new List<string>());
                BattlePlayers.Camps[camp_id].Add(bp.animal.guid);
            }
        }
    }

    public static void change_effect_list()
    {
        if (BattlePlayers.effects_list_change)
        {
            BattlePlayers.effects_list.Clear();
            var dicSort = from objDic in BattlePlayers.effects orderby objDic.Key ascending select objDic;
            foreach (KeyValuePair<int, battle_effect> kvp in dicSort)
                BattlePlayers.effects_list.Add(kvp.Value);
            BattlePlayers.effects_list_change = false;
        }
    }

    public static bool Play()
    {
        if (BattlePlayers.zhen > BattlePlayers.max_zhen)
            return false;

        int num = 100;
        if (!BattlePlayers.init_item)
            num = 2000;
        else if (BattlePlayers.zhen % (BattlePlayers.TNUM * 30) == 0)
            num = 1000;

        BattleOperation.set_random_seed(BattlePlayers.seed + BattlePlayers.zhen * seed_add, num);
        if (!BattlePlayers.init_item)
            BattlePlayers.do_iteminit();
        BattlePlayers.do_operation();
        BattlePlayers.PlayersAI();
        if (BattlePlayers.Boss != null)
        {
            BattlePlayerAI.BossLifeCycle();
            BattlePlayerAI.BossAI(BattlePlayers.Boss);
        }

        if (BattlePlayers.zhen % (BattlePlayers.TNUM * 30) == 0)
            BattlePlayers.do_check_pos_item();

        if (BattlePlayers.zhen % (BattlePlayers.TNUM * 6) == 0)
            BattlePlayers.do_check_item();

        BattlePlayers.do_effect();
        if (BattlePlayers.zhen % (BattlePlayers.TNUM / 2) == 0)
            BattlePlayers.do_item();
        BattlePlayers.do_player();
        BattlePlayers.do_effect_follow();
        BattlePlayers.zhen = BattlePlayers.zhen + 1;

        if (zhen % 2 == 0)
        {
            Battle.send_move_stop_true();
            Battle.send_attackr_true();
        }

        return true;
    }

    public static void Update()
    {
        if (!BattlePlayers.start)
            return;
        if (Battle.is_end)
            return;
        
        var dzhen = BattlePlayers.max_zhen - BattlePlayers.zhen + 1;
        if (dzhen > BattlePlayers.TNUM * 8 && !BattlePlayers.jiasu && Battle.is_online)
        {
            Battle.send_reset();
            return;
        }

        int num = 0;
        int max_num = BattlePlayers.TNUM * 30;
        while (num <= max_num)
        {
            num = num + 1;
            dzhen = BattlePlayers.max_zhen - BattlePlayers.zhen + 1;
            
            if (dzhen == 0 || num > max_num)
            {
                if (dzhen == 0 && BattlePlayers.jiasu)
                    BattlePlayers.Disjiasu();
                break;
            }

            if (!BattlePlayers.Play())
            {
                break;
            }
        }

        if (!BattlePlayers.jiasu)
        {
            BattlePlayers.Pos();
            BattlePlayers.ItemPos();
        }
    }

    public static void Disjiasu()
    {
        BattlePlayers.jiasu = false;
        for (int ll = 0; ll < BattlePlayers.players_list.Count; ll++)
        {
            var bp = BattlePlayers.players_list[ll];
            bp.last = new List<List<object>>() { new List<object>() { bp.player.x, bp.player.y, bp.player.is_jf, bp.jf_pt } };
            bp.last_time = 0;
        }

        for (int ll = 0; ll < BattlePlayers.effects_list.Count; ll++)
        {
            var be = BattlePlayers.effects_list[ll];
            be.last = new List<List<object>>() { new List<object>() { be.effect.x, be.effect.y, be.pt } };
            be.last_time = 0;
        }

        var dict = from obj in BattlePlayers.items orderby obj.Value.item.id descending select obj;
        foreach (KeyValuePair<int, battle_item> kvp in dict)
            BattlePlayers.add_item_obj(kvp.Value);
    }

    public static void Pos()
    {
        for (int i = 0; i < BattlePlayers.effects_list.Count; i++)
        {
            var be = BattlePlayers.effects_list[i];
            if (be.last.Count >= 2)
            {
                var num = be.last.Count - 2;
                double tt = 1;
                if (num >= 32)
                    tt = 8;
                else if (num >= 16)
                    tt = 4;
                else if (num >= 8)
                    tt = 2;
                else
                    tt = 0.95 + 0.05 * num;

                be.last_time = be.last_time + Time.deltaTime / Time.timeScale * tt;
                double per1 = be.last_time / (BattlePlayers.TICK * 1.0 / 1000);
                while (per1 >= 1)
                {
                    if (be.last.Count > 2)
                    {
                        be.last.RemoveAt(0);
                        be.last_time = be.last_time - BattlePlayers.TICK / 1000.0;
                        per1 = per1 - 1;
                    }
                    else
                    {
                        be.last_time = BattlePlayers.TICK / 1000.0f;
                        per1 = 1;
                        break;
                    } 
                }

                if (be.obj != null)
                {
                    var v = new Vector3(((int)be.last[1][0]) * 1.0f / Battle.BL, 0, ((int)be.last[1][1]) * 1.0f / Battle.BL);
                    var v1 = new Vector3(((int)be.last[0][0]) * 1.0f / Battle.BL, 0, ((int)be.last[0][1]) * 1.0f / Battle.BL);


                    if (be.last[1][2] != null)
                        v.y = BattleOperation.CubicBezierCurve(be.p0, be.p1, be.p2, be.p3, (float)((double)be.last[1][2])).y;
                    if (be.last[0][2] != null)
                        v1.y = BattleOperation.CubicBezierCurve(be.p0, be.p1, be.p2, be.p3, (float)((double)be.last[0][2])).y;
                    v = v1 + (v - v1) * (float)per1;
                    BattlePlayers.bobjpool.set_localPosition(be.objid, v.x, v.y, v.z);
                }
            }
        }

        double ts = 1;
        for (int i = 0; i < BattlePlayers.players_list.Count; i++)
        {
            var bp = BattlePlayers.players_list[i];
            ts = BattlePlayers.Pos1(bp,ts);
        }

        for (int i = 0; i < BattlePlayers.penguin_list.Count; i++)
        {
            var bp = BattlePlayers.penguin_list[i];
            ts = BattlePlayers.Pos1(bp, ts);
        }

        if (BattlePlayers.Boss != null)
            ts = BattlePlayers.Pos1(BattlePlayers.Boss, ts);
        Time.timeScale = (float)ts;

        for (int i = 0; i < BattlePlayers.players_list.Count; i++)
        {
            var bp = BattlePlayers.players_list[i];
            BattlePlayers.CalcCao(bp);
        }

        for (int i = 0; i < BattlePlayers.penguin_list.Count; i++)
        {
            var bp = BattlePlayers.penguin_list[i];
            BattlePlayers.CalcCao(bp);
        }

        if (BattlePlayers.Boss != null)
            BattlePlayers.CalcCao(BattlePlayers.Boss);

        if (BattlePlayers.me != null)
        {
            float ox, oy, oz;
            BattlePlayers.bobjpool.get_localPosition(BattlePlayers.me.posobjid, out ox, out oy,out oz);
            LuaHelper.GetMapManager().SetCurCam(ox, oy, oz);
            LuaHelper.GetMapManager().SetVCam(0, BattlePlayers.dis / 10.0f, -BattlePlayers.dis / 10.0f + 0.8f);
        }
    }

    public static double Pos1(BattleAnimal bp, double ts)
    {
        if (bp.last.Count >= 2)
        {
            var num = bp.last.Count - 2;
            double tt = 1.0;
            if (num >= 32)
                tt = 8;
            else if (num >= 16)
                tt = 4;
            else if (num >= 8)
                tt = 2;
            else
                tt = 0.95 + 0.05 * num;
            if (bp.animal.guid == self.guid)
                ts = tt;
            bp.last_time = bp.last_time + Time.deltaTime / Time.timeScale * tt;
            var per1 = bp.last_time / (BattlePlayers.TICK / 1000.0);
            while (per1 >= 1)
            {
                if (bp.last.Count > 2)
                {
                    bp.last.RemoveAt(0);
                    bp.last_time = bp.last_time - BattlePlayers.TICK / 1000.0;
                    per1 = per1 - 1;
                }
                else
                {
                    bp.last_time = BattlePlayers.TICK / 1000.0;
                    per1 = 1;
                    break;
                }
            }

            var v1 = new Vector3((int)bp.last[0][0] * 1.0f / Battle.BL, 0, (int)bp.last[0][1] * 1.0f / Battle.BL);
            var v = new Vector3((int)bp.last[1][0] * 1.0f / Battle.BL, 0, (int)bp.last[1][1] * 1.0f / Battle.BL);

            double r = 90 - bp.animal.r;
            if (bp.animal.is_jf)
            {
                if (bp.last[1][3] != null)
                    v.y = BattleOperation.QuadBezierCurve(bp.jf_p0, bp.jf_p1, bp.jf_p2, (float)((double)bp.last[1][3])).y;
                if (bp.last[0][3] != null)
                    v1.y = BattleOperation.QuadBezierCurve(bp.jf_p0, bp.jf_p1, bp.jf_p2, (float)((double)bp.last[0][3])).y;
                bp.jfr = bp.jfr + Time.deltaTime * 360;
                r = bp.jfr;
                BattlePlayers.bobjpool.set_localEulerAngles(bp.objid, 0, (float)r, 0);
            }
            else
            {
                if (bp.can_do())
                {
                    if (bp.animal.re_state > 0)
                        r = 90 - bp.animal.re_r;
                    if (bp.animal.type == unit_type.Monster)
                    {
                        if (bp.animal.re_state == 0)
                        {
                            if (bp.animal.is_move)
                                BattlePlayers.action(bp, "run");
                            else
                                BattlePlayers.action(bp, "ready");
                        }
                    }
                    else
                    {
                        if (!bp._lock || bp.animal.re_state == 0)
                        {
                            if (!bp.is_fight())
                            {
                                if (bp.animal.is_move)
                                    BattlePlayers.action(bp, "run");
                                else
                                    BattlePlayers.action(bp, "ready");
                            }
                            else
                            {
                                if (bp.animal.is_move)
                                    BattlePlayers.action(bp, "run02");
                                else
                                    BattlePlayers.action(bp, "ready02");
                                r = 90 - bp.animal.attackr;
                            }
                        }
                    }
                }
                else if (bp.is_die)
                    BattlePlayers.action(bp, "death");
                else
                    BattlePlayers.action(bp, "ready");

                if (bp.can_do())
                {
                    if (bp.lastr != r)
                    {
                        bp.lastr = r;
                        bp.jfr = r;
                        BattlePlayers.bobjpool.set_localEulerAngles(bp.objid, 0, (float)r, 0);
                    }
                    else if (bp.jfr != r)
                        BattlePlayers.bobjpool.set_localEulerAngles(bp.objid, 0, (float)r, 0);
                }
            }
            v = v1 + (v - v1) * (float)per1;
            BattlePlayers.bobjpool.set_localPosition(bp.posobjid, v.x, v.y, v.z);
        }
        return ts;
    }

    public static void ItemPos()
    {
        for (int i = BattlePlayers.item_speeds.Count - 1; i >= 0; i--)
        {
            var bi = BattlePlayers.item_speeds[i];
            var v = new Vector3(bi.item.x * 1.0f / Battle.BL, 0, bi.item.y * 1.0f / Battle.BL);
            var v1 = new Vector3((float)bi.xx, 0, (float)bi.yy);
            var dv = v - v1;
            var dis = dv.magnitude;
            var l = bi.speed * Time.deltaTime;
            if (dis < l)
            {
                bi.speed = 0;
                BattlePlayers.bobjpool.set_localPosition(bi.objid, v.x, 0.1f, v.z);
                BattlePlayers.item_speeds.RemoveAt(i);
            }
            else
            {
                v1 = v1 + dv.normalized * (float)l;
                bi.xx = v1.x;
                bi.yy = v1.z;
                var y = bi.ll * 1.0f / Battle.BL / 2 - dis;
                if (y < 0)
                    y = -y;
                y = bi.ll *1.0f / Battle.BL / 2 - y;
                y = y * y;      
                BattlePlayers.bobjpool.set_localPosition(bi.objid, v1.x, 0.1f + y, v1.z);
            }
        }
        var idels = new List<int>();
        foreach (var item in BattlePlayers.item_follows)
        {
            var bi = item.Value;
            if (!BattlePlayers.players.ContainsKey(bi.follow))
                idels.Add(item.Key);
            else
            {
                var bp = BattlePlayers.players[bi.follow];
                bi.follow_time = bi.follow_time + Time.deltaTime;
                if (bi.follow_time >= 0.3f)
                {
                    var t_battle_item = Config.get_t_battle_item(bi.item.id);
                    if (t_battle_item != null)
                        BattlePlayers.Attach(bp, "accept", t_battle_item.geffect);
                    idels.Add(item.Key);
                }
                else
                {
                    var dv = new Vector3((float)(bp.animal.x *1.0 / Battle.BL - bi.xx), 0, (float)(bp.animal.y * 1.0 / Battle.BL - bi.yy));
                    var dis = dv.sqrMagnitude;
                    if (dis > 0.01f)
                    {
                        dv = dv.normalized * Time.deltaTime * 3 * (float)bi.follow_speed;
                        var ds = dv.sqrMagnitude;
                        if (ds < dis)
                        {
                            bi.xx = dv.x + bi.xx;
                            bi.yy = dv.y + bi.yy;
                            var yi = bi.follow_time * 5;
                            if (bi.follow_time > 0.2f)
                                yi = 2 - bi.follow_time * 5;
                            yi = yi * yi;
                            BattlePlayers.bobjpool.set_localPosition(bi.objid, (float)bi.xx, (float)(0.1 + yi),(float)(bi.yy));
                        }
                    }
                }
            }
        }

        for (int i = 0; i < idels.Count; i++)
        {
            var bi = BattlePlayers.item_follows[idels[i]];
            LuaHelper.GetResManager().DeleteEffect(bi.obj);
            BattlePlayers.item_follows.Remove(idels[i]);
        }
    }

    public static void PlayersAI()
    {
        if (Battle.is_newplayer_guide)
        {
            for (int i = 0; i < BattlePlayers.players_list.Count; i++)
            {
                var bp = BattlePlayers.players_list[i];
                if (bp.player.is_ai > 0)
                {
                    BattlePlayerAI.new_player_ai_process(bp);
                }           
            }
            return;
        }


        for (int i = 0; i < BattlePlayers.players_list.Count; i++)
        {
            var bp = BattlePlayers.players_list[i];
            if (bp.player.is_ai > 0 && bp.player.is_ai < 100)
            {
                BattlePlayerAI.NewRobotAI(bp);
            }
        }

        for (int i = 0; i < BattlePlayers.penguin_list.Count; i++)
        {
            var bp = BattlePlayers.penguin_list[i];
            BattlePlayerAI.PenguinAI(bp);
        }
    }

    public static void fillEffectInfo(BattleAnimalPlayer bp,battle_effect be)
    {
        if (be.effect.re_guid != self.guid)
            return;

        if (!(be.re_bp is BattleAnimalPlayer))
            return;
        var re_bp = be.re_bp as BattleAnimalPlayer;

        if (bp.is_die)
        {
            be.effect_hums[bp.player.guid] = 0;
            if (bp.player.sex == 1)
                re_bp.achieveRecords.killFemale = re_bp.achieveRecords.killFemale + 1;
            else if (bp.player.sex == 0)
                re_bp.achieveRecords.killMale = re_bp.achieveRecords.killMale + 1;

            if (be.effect.id == 10010101)
                re_bp.achieveRecords.IsUseNormalToKill = true;
            else
                re_bp.achieveRecords.IsUseSkillToKill = true;
            if (!bp.mtk)
                re_bp.achieveRecords.quietKills = re_bp.achieveRecords.quietKills + 1;

            if (re_bp.achieveRecords.kills == null)
                re_bp.achieveRecords.kills = new Dictionary<string, int>();

            if (re_bp.achieveRecords.kills.ContainsKey(bp.player.guid))
                re_bp.achieveRecords.kills[bp.player.guid] = re_bp.achieveRecords.kills[bp.player.guid] + 1;
            else
                re_bp.achieveRecords.kills.Add(bp.player.guid, 1);

            var pert = BattleOperation.toInt(re_bp.player.hp * 100.0 / bp.attr.max_hp());
            if (pert <= 10)
                re_bp.achieveRecords.blood_ten_pert = re_bp.achieveRecords.blood_ten_pert + 1;

            if (re_bp.cao == 0 && bp.cao > 0)
                re_bp.achieveRecords.killCaos = re_bp.achieveRecords.killCaos + 1;

            if (re_bp.player.hp <= 0 || re_bp.is_die)
                re_bp.achieveRecords.killDeath = re_bp.achieveRecords.killDeath + 1;
            BattlePlayers.AccumKillInBuffer(re_bp);
            BattleAchieve.OnlyBattleKillRelation(re_bp, bp);
        }
        else
        {
            if(!be.effect_hums.ContainsKey(bp.player.guid))
                be.effect_hums.Add(bp.player.guid, 1);
            else
                be.effect_hums[bp.player.guid] =1;
        }
    }

    public static void BeforeKills(BattleAnimalPlayer me, BattleAnimalPlayer armyPt,int rank)
    {
        if (me.animal.guid != self.guid || armyPt.animal.is_ai >= 1000)
            return;
        if (armyPt.animal.hp > 0)
            return;
        if (rank == 1)
            me.achieveRecords.killHeader = me.achieveRecords.killHeader + 1;
    }

    public static void AccumKillInBuffer(BattleAnimalPlayer bp)
    {
        if (bp.player.guid != self.guid)
            return;
        for (int i = bp.player.buffs.Count - 1; i >= 0; i--)
        {
            var t_battle_buff = Config.get_t_battle_buff(bp.player.buffs[i]);
            if (t_battle_buff != null)
            {
                if (BattlePlayers.zhen < bp.player.buffs_time[i] && t_battle_buff.time != -1)
                {
                    if (bp.achieveRecords.bufferKill == null)
                        bp.achieveRecords.bufferKill = new Dictionary<int, int>();
                    if (bp.achieveRecords.bufferKill.ContainsKey(t_battle_buff.id))
                        bp.achieveRecords.bufferKill[t_battle_buff.id] = bp.achieveRecords.bufferKill[t_battle_buff.id] + 1;
                    else
                        bp.achieveRecords.bufferKill.Add(t_battle_buff.id, 1);
                }
            }
        }
    }
    public static void CalcCao(BattleAnimal bp)
    {
        if (BattlePlayers.me == null)
        {
            if (bp.cao > 0 || bp.attr.is_yinshen())
                BattlePlayers.set_alpha(bp, 0);
            else
                BattlePlayers.set_alpha(bp, 1);
            return;
        }

        if (bp.animal.guid != BattlePlayers.me.player.guid && !BattleOperation.can_see(BattlePlayers.me, bp))
        {
            BattlePlayers.set_alpha(bp, 0);
            return;
        }

        if (bp.cao > 0 || bp.attr.is_yinshen())
            BattlePlayers.set_alpha(bp, 0.5f);
        else
            BattlePlayers.set_alpha(bp, 1);
    }

    public static void MakeXueren(BattleAnimalPlayer bp,bool flag,bool is_init = false)
    {
        bp.player.is_xueren = flag;
        bp.alpha = -1;
        if (bp.xobj != null)
        {
            BattlePlayers.bobjpool.remove(bp.xobjid);
            GameObject.Destroy(bp.xobj);
            bp.xobj = null;
            bp.xobjt = null;
            bp.xunit = null;
        }

        if (flag)
        {
            string s = "snowman_f01";
            if (bp.sex() == 0)
                s = "snowman_m01";
            var xobj = LuaHelper.GetResManager().CreateUnit(s, false);
            var xobjid = BattlePlayers.bobjpool.add(xobj);
            var xobjt = xobj.transform;
            xobjt.parent = bp.posobjt;
            BattlePlayers.bobjpool.set_localPosition(xobjid, 0, 0, 0);
            BattlePlayers.bobjpool.set_localEulerAngles(xobjid, 0, 90 - bp.animal.r, 0);
            BattlePlayers.bobjpool.set_localScale(xobjid, 1, 1, 1);
            bp.xobj = xobj;
            bp.xobjt = xobjt;
            bp.xunit = bp.xobj.GetComponent<unit>();
            bp.xobjid = xobjid;
            bp.unit.pause_action();
            BattlePlayers.bobjpool.set_localScale(bp.objid, 0.01f, 0.01f, 0.01f);

            if (!is_init)
                bp.add_buff(1001);
            BattlePlayers.Attach1(bp, "accept", "Unit_Release_snowman");
        }
        else
        {
            BattlePlayers.bobjpool.set_localScale(bp.objid, 1, 1, 1);
            bp.unit.continue_action();
            bp.remove_buff(1001);
            BattlePlayers.Attach1(bp, "accept", "Unit_die_snowman");
        }
    }
    public static void delplayer(string guid)
    {
        if (!BattlePlayers.players.ContainsKey(guid))
            return;

        var bp = BattlePlayers.players[guid];
        BattlePlayers.bobjpool.remove(bp.objid);
        BattlePlayers.bobjpool.remove(bp.posobjid);
        GameObject.Destroy(bp.posobj);
        BattlePlayers.players.Remove(guid);
        BattlePlayers.players_list_change = true;
        BattleGrid.del(grid_type.et_player, guid);
        Battle.battle_panel.del_min_pro(guid);
        if (self.guid == guid)
            BattlePlayers.me = null;
    }
    public static void change_player_list()
    {
        if (BattlePlayers.players_list_change)
        {
            BattlePlayers.players_list.Clear();
            var dicSort = from objDic in BattlePlayers.players orderby objDic.Key select objDic;
            foreach (KeyValuePair<string, BattleAnimal> kvp in dicSort)
            {
                if (kvp.Value.animal.type == unit_type.Player)
                {
                    var tp = kvp.Value as BattleAnimalPlayer;
                    BattlePlayers.players_list.Add(tp);
                }
            }    
            change_player_camp_member();
            BattlePlayers.players_list_change = false;
            Battle.battle_panel.refresh_self_teamer_info();
        }
    }
    public static GameObject Attach(BattleAnimal bp, string bone, string name, bool yj = false)
    {
        return bp.unit.Attach(bone, name, yj);
    }

    public static GameObject AttachSp(BattleAnimal bp, string bone, string name, bool yj,float speed)
    {
        return bp.unit.Attach(bone, name, yj,speed);
    }

    public static GameObject Attach1(BattleAnimal bp, string bone,string name)
    {
        var eff = bp.unit.Attach1(bone, name);
        if (eff != null)
        {
            var s = (float)bp.get_scale();
            eff.transform.localScale = new Vector3(s, s, s);
        }
        return eff;
    }
    public static void action(BattleAnimal bp, string name, int layer_index, float? speed = null)
    {
        if (name == "attack01" || name == "attack01a" || name == "attack03" || name == "attack04")
        {
            if (layer_index == 0)
            {
                bp.unit.action(name, speed.GetValueOrDefault());
                return;
            }
            else
            {
                bp.unit.action1(name, speed.GetValueOrDefault());
                return;
            }
        }

        if (speed == null)
        {
            if (name == "run" || name == "run02")
            {
                speed = bp.attr.speed() * 1.0f / 30000;
                if (speed > 3)
                    speed = 3;
                speed = (float)(speed.GetValueOrDefault() / ((bp.get_scale() - 1) * 0.5 + 1));
                var d = bp.action_speed - speed.GetValueOrDefault();
                if (bp.action == name && (bp.action == "run" || bp.action == "run02") && d < 0.001 && d > -0.001)
                    return;
            }
            else
                speed = 1;
        }

        if (bp.action == name && (name != "run" || name == "run02"))
            return;
        bp.action = name;
        bp.action_speed = speed.GetValueOrDefault();
        bp.unit.action(name, speed.GetValueOrDefault());
    }
    public static void action(BattleAnimal bp, string name, float? speed = null)
    {
        if (name == "attack01" || name == "attack01a" || name == "attack03" || name == "attack04")
        {
            bp.unit.action1(name, speed.GetValueOrDefault());
            return;
        }

        if (speed == null)
        {
            if (name == "run" || name == "run02")
            {
                speed = bp.attr.speed() * 1.0f / 30000;
                if (speed > 3)
                    speed = 3;
                speed = (float)(speed.GetValueOrDefault() / ((bp.get_scale() - 1) * 0.5 + 1));
                var d = bp.action_speed - speed.GetValueOrDefault();
                if (bp.action == name && (bp.action == "run" || bp.action == "run02") && d < 0.001 && d > -0.001)
                    return;
            }
            else
                speed = 1;
        }

        if (bp.action == name && (name != "run" || name == "run02"))
            return;
        bp.action = name;
        bp.action_speed = speed.GetValueOrDefault();
        bp.unit.action(name, speed.GetValueOrDefault());
    }
    public static void set_alpha(BattleAnimal bp, float a)
    {
        if (bp.alpha != a)
        {
            bp.alpha = a;
            bp.unit.set_alpha(a);
            if (bp.xobj != null)
                bp.xunit.set_alpha(a);
            if (bp.animal.type == unit_type.Player)
            {
                var tmp = bp as BattleAnimalPlayer;
                if (a == 0)
                {
                    if (tmp.shadow_res != null)
                        tmp.shadow_res.SetActive(false);
                    if (tmp.ring_res != null && tmp.ring_res.activeSelf)
                        tmp.ring_res.SetActive(false);
                    if (tmp.pf_res != null && tmp.pf_res.activeSelf)
                        tmp.pf_res.SetActive(false);
                }
                else if (a == 1)
                {
                    if (tmp.shadow_res != null)
                        tmp.shadow_res.SetActive(true);
                    if (tmp.ring_res != null && !tmp.ring_res.activeSelf)
                        tmp.ring_res.SetActive(true);
                    if (tmp.pf_res != null && !tmp.pf_res.activeSelf)
                        tmp.pf_res.SetActive(true);
                }
            }
            else if (bp.animal.type == unit_type.Monster)
            {
                var tmp = bp as BattleAnimalMonster;
                if (a == 0)
                {
                    if (tmp.shadow_res != null)
                        tmp.shadow_res.SetActive(false);
                }
                else if (a == 1)
                {
                    if (tmp.shadow_res != null)
                        tmp.shadow_res.SetActive(true);
                }
            }
        }
    }

    public static void Fuhuo(BattleAnimal bp)
    {
        if (!bp.is_die)
            return;

        bp.remove_all_buff();
        if (bp.animal.type == unit_type.Boss)
            BattlePlayers.init_boss1(bp as BattleAnimalBoss);
        else if(bp.animal.type == unit_type.Monster)
            BattlePlayers.init_monster1(bp as BattleAnimalMonster);
        else if(bp.animal.type == unit_type.Player)
            BattlePlayers.init_player1(bp as BattleAnimalPlayer);

        bp.set_hp(bp.attr.max_hp());
        bp.is_die = false;
        bp.init_attr();
        bp.do_scale();
        if (bp.animal.type == unit_type.Player)
            (bp as BattleAnimalPlayer).update_pf();

        BattleGrid.add(grid_type.et_player, bp.animal.guid, bp.animal.x, bp.animal.y);
        bp.cao = BattleGrid.get_cao(bp.animal.x, bp.animal.y);
        bp.last = new List<List<object>>() { new List<object>() { bp.animal.x, bp.animal.y, bp.animal.is_jf, null }, new List<object> { bp.animal.x, bp.animal.y, bp.animal.is_jf, null } };
        bp.last_time = 0;

        if (self.guid == bp.animal.guid)
            Battle.battle_panel.fuhuoInit();
    }
}

