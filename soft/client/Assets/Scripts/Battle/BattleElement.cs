using BattleDB;
using protocol.game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum unit_type
{
    Monster,
    Player,
    Boss
}

public class battle_unit
{
    public string name;
    public unit_type type;
    public int role_level;
    public int camp;  //阵营
    public bool is_xueren;

    public string guid;
    public int role_id;
    public int x;
    public int y;
    public int r;
    public int r_py;
    public bool is_move;
    public int re_tid;
    public int re_state;
    public int re_id;
    public int re_level;
    public int re_time;
    public int re_x;
    public int re_y;
    public int re_r;
    public List<int> save_re_id = new List<int>();
    public List<int> save_re_zhen = new List<int>();
    public bool is_jf;
    public int jf_sx;
    public int jf_sy;
    public int jf_xx;
    public int jf_yy;
    public int jf_speed;
    public int jf_r;
    public List<int> bhit_tids = new List<int>();
    public int death_time;
    public int rtime;
    public int attackr;
    public int hp;
    public List<int> buffs = new List<int>();
    public List<int> buffs_time = new List<int>();
    public int is_ai;
    public int eyeRange;
    public int ai_state;
    public int nextPoint_x;
    public int nextPoint_y;
    public int totalZhen;
}
public class battle_boss : battle_unit
{
    public int boss_id;
    public int attack_state;
    public int attackZhen;
    public int boss_birth_time;

    public int skill_id;
    public int skill_level;
}
public class battle_monster : battle_unit
{
    public int monster_id;  // penguin表中的id
    public int birth_x;
    public int birth_y;
    public string attack_guid;

    public int skill_id;
    public int skill_level;
}
public class battle_player : battle_unit
{
    public int sex;
    
    public List<int> attr_type = new List<int>();
    public List<int> attr_param1 = new List<int>();
    public List<int> attr_param2 = new List<int>();
    public List<int> attr_param3 = new List<int>();
    public int avatar;
    public int cup;
    public int toukuang;
    public List<int> fashion = new List<int>();
    public int region_id;
    public int name_color;
    public int level;
    public int exp;
    public int score;
    public int score_level;
    public List<int> lattr_value = new List<int>();
    public List<int> talent_id = new List<int>();
    public List<int> talent_level = new List<int>();
    public int talent_point;
    public int die;
    public int sha;
    public int lsha;
    public int max_lsha;
    public int skill_id;
    public int skill_level;
    public int re_pre;
    public int re_pre_zhen;
    public int re_pre_zhen1;
    public int attack_state;
    public int attackZhen;
    public int birth_x;
    public int birth_y;
    public string attack_guid;
    public int cost;
    //public bool is_xueren;
    public int xueren_zhen;
}

public class BattleAnimal
{
    public battle_unit animal;
    public string action;
    public double action_speed;
    public double jfr;
    public double alpha;
    public List<List<object>> last;
    public double last_time;
    public bool _lock;
    public double lastr;
    public GameObject obj;
    public Transform objt;
    public int objid;
    public GameObject posobj;
    public Transform posobjt;
    public int posobjid;
    public Transform accept;
    public int cao;
    public unit unit;
    public int ur_;
    public bool is_die;
    public GameObject xobj;
    public Transform xobjt;
    public unit xunit;
    public int xobjid;
    public AnimalAttr attr;
    public CAttr cattr;
    public List<int> attr_value;
    public Dictionary<int, Dictionary<int, int>> skill_attr_value;
    public Dictionary<int, Buffer_Effect> buff_effect;
    public int? attack_state;
    public int[] attack_num = new int[] { 0,0,0};  //新手中使用
    public int? mianyi;
    public bool aaction;

    public Vector3 jf_p0;
    public Vector3 jf_p1;
    public Vector3 jf_p2;
    public int jf_len;
    public double jf_pt;

    public GameObject pre_eff;
    public int? pre_eff_state;
    public int? pre_eff_zhen;

    protected t_role t_role;
    public BattleAnimal(battle_unit animal)
    {
        this.animal = animal;
        attr = new AnimalAttr(this);
        cattr = new CAttr();
        t_role = Config.get_t_role(this.animal.role_id);
        buff_effect = new Dictionary<int, Buffer_Effect>();
    }

    public virtual void InitBattlePlayer()
    {
        this.init_attr();
        this.do_scale();
        this.init_buff();
        this.start_jf();
    }

    public virtual void init_attr()
    {
        this.attr_value = new List<int>();
        this.skill_attr_value = new Dictionary<int, Dictionary<int, int>>();
        for (int i = 0; i <= 200; i++)
            this.attr_value.Add(0);
    }

    public virtual int get_skill_attr_value(int skill_id, int t)
    {
        if (!skill_attr_value.ContainsKey(skill_id))
            skill_attr_value.Add(skill_id, new Dictionary<int, int>());

        if (!skill_attr_value[skill_id].ContainsKey(t))
            skill_attr_value[skill_id].Add(t, 0);

        return this.skill_attr_value[skill_id][t];
    }

    public virtual void set_skill_attr_value(int skill_id, int t, int value)
    {
        if (!this.skill_attr_value.ContainsKey(skill_id))
            this.skill_attr_value.Add(skill_id, new Dictionary<int, int>());
        this.skill_attr_value[skill_id][t] = value;
    }

    public virtual void set_hp(int hp, bool xs = false)
    {
        var yhp = this.animal.hp;
        this.animal.hp = hp;
        var max_hp = this.attr.max_hp();
        if (this.animal.hp > max_hp)
            this.animal.hp = max_hp;
        if (xs && this.animal.hp > yhp)
            Battle.battle_panel.add_text(this, (this.animal.hp - yhp).ToString(), 3);
    }

    public virtual void add_attr_value(int index, int value, bool reset = true)
    {
        this.attr_value[index] = this.attr_value[index] + value;
        if (reset)
            this.cattr = new CAttr();
    }

    public virtual void mod_attr_value(int index, int value, bool reset = true)
    {
        this.attr_value[index] = value;
        if (reset)
            this.cattr = new CAttr();
    }

    public virtual double ur()
    {
        return this.ur_ * this.get_scale();
    }

    public virtual int sex()
    {
        return t_role.sex;
    }

    public virtual double get_scale()
    {
        return 1.0;
    }

    public virtual void do_scale()
    {
        float s = (float)(this.get_scale());
        this.posobjt.localScale = new Vector3(s, s, s);
    }

    public bool can_do()
    {
        if (this.is_die || this.animal.is_jf || this.attr.is_stun() || this.attr.is_bing())
            return false;

        return true;
    }
    public virtual void yy(int i)
    {
        if (BattlePlayers.jiasu)
            return;
        this.unit.play_sound(t_role.yy[i], this.obj);
    }

    public virtual void add_buff(int id, int? yc = null, int? time = null, int? jt = null)
    {
        if (yc == null)
            yc = 0;

        var t_battle_buff = Config.get_t_battle_buff(id);
        if (t_battle_buff == null)
            return;

        int index = -1;
        for (int i = 0; i < this.animal.buffs.Count; i++)
        {
            if (this.animal.buffs[i] == id)
            {
                index = i;
                break;
            }
        }

        int bt = 0;
        if (jt == null)
        {
            bt = BattlePlayers.zhen + BattleOperation.toInt(t_battle_buff.time * (1 + yc.GetValueOrDefault() / 100.0) / BattlePlayers.TICK);
            if (time != null)
                bt = BattlePlayers.zhen + BattleOperation.toInt(time.GetValueOrDefault() * (1 + yc.GetValueOrDefault() / 100.0) / BattlePlayers.TICK);
        }
        else
            bt = jt.GetValueOrDefault();

        if (index == -1)
        {
            this.animal.buffs.Add(id);
            this.animal.buffs_time.Add(bt);
            this.add_buff_effect(id);
            for (int j = 0; j < t_battle_buff.attr.Count; j++)
            {
                if (t_battle_buff.attr[j].ptype == 1)
                {

                    this.add_attr_value(t_battle_buff.attr[j].param1, t_battle_buff.attr[j].param2);
                    if (t_battle_buff.attr[j].param1 == 103)
                    {
                        if (this.animal.is_move)
                            this.animal.r = BattleOperation.checkr(this.animal.r + 180);
                        this.animal.r_py = 0;
                    }
                }
            }
        }
        else
            this.animal.buffs_time[index] = bt;
    }
    public virtual void add_buff_effect(int id)
    {
        var t_battle_buff = Config.get_t_battle_buff(id);
        if (t_battle_buff == null)
            return;

        id = t_battle_buff.effect_type;
        if (id == 0)
            return;
        if (!this.buff_effect.ContainsKey(id))
        {
            this.buff_effect.Add(id, new Buffer_Effect());
            this.buff_effect[id].num = 1;
            if (!String.IsNullOrEmpty(t_battle_buff.effect))
                this.buff_effect[id].obj = BattlePlayers.Attach(this, t_battle_buff.effect_bone, t_battle_buff.effect, true);

            if (id == 6)
                this.unit.pause_action();
        }
        else
            this.buff_effect[id].num = this.buff_effect[id].num + 1;
    }
    public void remove_buff(int id)
    {
        var t_battle_buff = Config.get_t_battle_buff(id);
        if (t_battle_buff == null)
            return;

        for (int i = this.animal.buffs.Count - 1; i >= 0; i--)
        {
            if (this.animal.buffs[i] == id)
            {
                this.animal.buffs.RemoveAt(i);
                this.animal.buffs_time.RemoveAt(i);
                this.remove_buff_effect(id);
                for (int j = 0; j < t_battle_buff.attr.Count; j++)
                {
                    if (t_battle_buff.attr[j].ptype == 1)
                    {
                        this.add_attr_value(t_battle_buff.attr[j].param1, -t_battle_buff.attr[j].param2);
                        if (t_battle_buff.attr[j].param1 == 103)
                        {
                            if (this.animal.is_move)
                                this.animal.r = BattleOperation.checkr(this.animal.r + 180);
                            this.animal.r_py = 0;
                        }
                    }
                }
                break;
            }
        }
    }
    public virtual void remove_buff_effect(int id)
    {
        var t_battle_buff = Config.get_t_battle_buff(id);
        if (t_battle_buff == null)
            return;

        id = t_battle_buff.effect_type;
        if (id == 0)
            return;

        if (this.buff_effect.ContainsKey(id))
        {
            if (this.buff_effect[id].num == 1)
            {
                if (this.buff_effect[id].obj != null)
                {
                    LuaHelper.GetResManager().DeleteEffect(this.buff_effect[id].obj);
                    if (!String.IsNullOrEmpty(t_battle_buff.effect_end))
                        BattlePlayers.Attach1(this, t_battle_buff.effect_bone, t_battle_buff.effect_end);
                }
                
                if (id == 6)
                    this.unit.continue_action();
                this.buff_effect.Remove(id);
            }
            else
                this.buff_effect[id].num = this.buff_effect[id].num - 1;
        }
    }

    public void remove_yinshen_buff()
    {
        var rbuffs = new List<int>();
        for (int i = 0; i < this.animal.buffs.Count; i++)
        {
            var id = this.animal.buffs[i];
            var t_battle_buff = Config.get_t_battle_buff(id);
            if (t_battle_buff != null)
            {
                for (int j = 0; j < t_battle_buff.attr.Count; j++)
                {
                    if (t_battle_buff.attr[j].ptype == 1 && t_battle_buff.attr[j].param1 == 107)
                        rbuffs.Add(id);
                }
            }
        }

        for (int i = 0; i < rbuffs.Count; i++)
            this.remove_buff(rbuffs[i]);
    }

    public void remove_all_buff()
    {
        for (int i = 0; i < this.animal.buffs.Count; i++)
        {
            var id = this.animal.buffs[i];
            this.remove_buff_effect(id);
            var t_battle_buff = Config.get_t_battle_buff(id);
            if (t_battle_buff != null)
            {
                for (int j = 0; j < t_battle_buff.attr.Count; j++)
                {
                    if (t_battle_buff.attr[j].ptype == 1)
                    {
                        this.add_attr_value(t_battle_buff.attr[j].param1, -t_battle_buff.attr[j].param2);
                        if (t_battle_buff.attr[j].param1 == 103)
                        {
                            if (this.animal.is_move)
                                this.animal.r = BattleOperation.checkr(this.animal.r + 180);
                            this.animal.r_py = 0;
                        }
                    }
                }
            }
        }
        this.buff_effect.Clear();
    }

    public virtual void do_buff()
    {
        for (int i = this.animal.buffs.Count - 1; i >= 0; i--)
        {
            var t_battle_buff = Config.get_t_battle_buff(this.animal.buffs[i]);
            if (t_battle_buff != null)
            {
                if (BattlePlayers.zhen >= this.animal.buffs_time[i] && t_battle_buff.time != -1)
                {
                    this.animal.buffs.RemoveAt(i);
                    this.animal.buffs_time.RemoveAt(i);
                    this.remove_buff_effect(t_battle_buff.id);

                    for (int j = 0; j < t_battle_buff.attr.Count; j++)
                    {
                        if (t_battle_buff.attr[j].ptype == 1)
                        {
                            this.add_attr_value(t_battle_buff.attr[j].param1, -t_battle_buff.attr[j].param2);
                            if (t_battle_buff.attr[j].param1 == 103)
                            {
                                if (this.animal.is_move)
                                    this.animal.r = BattleOperation.checkr(this.animal.r + 180);
                                this.animal.r_py = 0;
                            }
                            if (t_battle_buff.attr[j].param1 <= 2)
                                this.set_hp(this.animal.hp);
                        }
                    }
                }
            }
        }
    }

    public void set_jf(int r, int l, int s)
    {
        if (this.animal.is_jf)
            return;
        this.animal.is_jf = true;
        this.animal.jf_sx = this.animal.x;
        this.animal.jf_sy = this.animal.y;
        this.animal.jf_speed = s;
        var p = BattleOperation.add_distance2(this.animal.jf_sx, this.animal.jf_sy, r, l);
        p = BattleGrid.get_move_point(this.animal.jf_sx, this.animal.jf_sy, p[0], p[1]);
        this.animal.jf_xx = p[0];
        this.animal.jf_yy = p[1];
        this.animal.jf_r = r;
        this.start_jf();
    }

    public void start_jf()
    {
        if (!this.animal.is_jf)
            return;
        this.jf_p0 = new Vector3(this.animal.jf_sx * 1.0f / Battle.BL, 0, this.animal.jf_sy * 1.0f / Battle.BL);
        this.jf_p2 = new Vector3(this.animal.jf_xx * 1.0f / Battle.BL, 0, this.animal.jf_yy * 1.0f / Battle.BL);
        this.jf_len = (int)BattleOperation.get_distance(this.animal.jf_sx, this.animal.jf_sy, this.animal.jf_xx, this.animal.jf_yy);
        this.jf_p1 = this.jf_p0 + (this.jf_p2 - this.jf_p0) / 2;
        this.jf_p1.y = this.jf_len / 2.0f / Battle.BL;
        var pos = BattleOperation.get_distance(this.animal.jf_sx, this.animal.jf_sy, this.animal.x, this.animal.y);
        if (this.jf_len == 0)
            this.jf_pt = 1;
        else
            this.jf_pt = pos * 1.0f / this.jf_len;

        if (!this.is_die)
            BattlePlayers.action(this, "ready");
    }

    public virtual void do_tick()
    {
        if (BattlePlayers.zhen % BattlePlayers.TNUM == 0)
        {
            if (!this.is_die)
            {
                var v = 0;
                if (this.attr_value[10] > 0)
                    v = v + this.attr_value[10];
                if (this.attr_value[11] > 0)
                    v = v + BattleOperation.toInt(this.attr.max_hp() * this.attr_value[11] / 100.0);
                if (v > 0)
                    this.set_hp(this.animal.hp + v, true);
            }
        }

        if (this.is_die)
        {
            if (Battle.is_newplayer_guide)
            {

            }
            else
            {
                if ((BattlePlayers.zhen - this.animal.death_time) * BattlePlayers.TICK >= 5000)
                    BattlePlayers.Fuhuo(this);
            }
        }
    }
    public virtual void init_buff()
    {
        for (int i = 0; i < this.animal.buffs.Count; i++)
        {
            var id = this.animal.buffs[i];
            var t_battle_buff = Config.get_t_battle_buff(id);
            if (t_battle_buff != null)
            {
                for (int j = 0; j < t_battle_buff.attr.Count; j++)
                {
                    if (t_battle_buff.attr[j].ptype == 1)
                        this.add_attr_value(t_battle_buff.attr[j].param1, t_battle_buff.attr[j].param2);
                }
                this.add_buff_effect(id);
            }
        }
    }
    public virtual int get_skill_level(int id)
    {
        return 1;
    }

    public virtual bool is_skillcd(int skill_id)
    {
        var t_skill = Config.get_t_skill(skill_id, this.get_skill_level(skill_id), this.get_skill_level_add(skill_id));
        if (t_skill == null)
            return true;

        int rz = -1;
        for (int i = 0; i < this.animal.save_re_id.Count; i++)
        {
            if (this.animal.save_re_id[i] == t_skill.id)
            {
                rz = this.animal.save_re_zhen[i];
                break;
            }
        }

        if (rz == -1)
            return false;
        else
            return (BattlePlayers.zhen - rz) * BattlePlayers.TICK < t_skill.get_cd(this);
    }

    public virtual int? get_bskill_value(int t)
    {
        if (t != 3)
            return null;
        for (int i = 0; i < t_role.bskills.Count; i++)
        {
            var t_role_skill = Config.get_t_role_skill(t_role.bskills[i]);
            if (t_role_skill != null)
            {
                for (int j = 0; j < t_role_skill.attrs.Count; j++)
                {
                    var sattr = t_role_skill.attrs[j];
                    if (sattr.type == t)
                    {
                        var r = sattr.param_value(this.animal.role_level);
                        if (BattleOperation.random(0, 100) < r)
                            return sattr.param1;
                    }
                }
            }
        }
        return null;
    }

    public virtual int get_skill_level_add(int skill_id)
    {
        for (int i = 0; i < t_role.bskills.Count; i++)
        {
            var t_role_skill = Config.get_t_role_skill(t_role.bskills[i]);
            if (t_role_skill != null)
            {
                for (int j = 0; j < t_role_skill.attrs.Count; j++)
                {
                    var sattr = t_role_skill.attrs[j];
                    if (sattr.type == 4)
                        if (sattr.param1 == skill_id)
                            return sattr.param2;
                }
            }
        }
        return 0;
    }

    public virtual bool is_fight()
    {
        return ((BattlePlayers.zhen - this.animal.rtime) * BattlePlayers.TICK <= 500);
    }
}

public class BattleAnimalBoss : BattleAnimal
{
    public battle_boss player;
    public BattleAnimalBoss(battle_boss animal):base(animal)
    {
        this.player = animal;
        attr = new AnimalBossAttr(this);
        cattr = new CAttr();
        t_role = Config.get_t_role(this.animal.role_id);
        buff_effect = new Dictionary<int, Buffer_Effect>();
    }
    public override double get_scale()
    {
        var t_boss_attr = Config.get_t_boss_attr(player.boss_id);
        if (t_boss_attr == null)
            return 1;

        return t_boss_attr.body_scale;
    }

    public override void do_scale()
    {
        var t_boss_attr = Config.get_t_boss_attr(player.boss_id);

        if(t_boss_attr == null)
            this.posobjt.localScale = Vector3.one;
        else
            this.posobjt.localScale = new Vector3(t_boss_attr.body_scale, t_boss_attr.body_scale, t_boss_attr.body_scale);
    }

    public override void do_tick()
    {
        if (BattlePlayers.zhen % BattlePlayers.TNUM == 0)
        {
            if (!this.is_die)
            {
                var v = 0;
                if (this.attr_value[10] > 0)
                    v = v + this.attr_value[10];
                if (this.attr_value[11] > 0)
                    v = v + BattleOperation.toInt(this.attr.max_hp() * this.attr_value[11] / 100.0);
                if (v > 0)
                    this.set_hp(this.animal.hp + v, true);
            }
        }

        if (this.is_die)
        {
            if ((BattlePlayers.zhen - this.animal.death_time) * BattlePlayers.TICK >= 2000)
            {
                this.player.x = 1800000;
                this.player.y = 1800000;
            }
        }
    }
}

public class BattleAnimalPlayer : BattleAnimal
{
    public battle_player player;
    public AchieveRecords achieveRecords;   //成就记录
    public bool mtk;
    public GameObject shadow_res;
    public GameObject ring_res;
    public GameObject pf_res;
    public t_fashion pskill_fashion;
    public t_fashion xuli_fashion;

    public int ai_r;
    public int ori_x;
    public int ori_y;

    public BattleAnimalPlayer(battle_player animal):base(animal)
    {
        this.player = animal;
        attr = new AnimalPlayerAttr(this);
        cattr = new CAttr();
        t_role = Config.get_t_role(this.animal.role_id);
        buff_effect = new Dictionary<int, Buffer_Effect>();
    }
    public override void init_attr()
    {
        base.init_attr();
        this.inc_bskill();
        this.inc_talent();

        for (int i = 0; i < this.player.fashion.Count; i++)
        {
            if (this.player.fashion[i] != 0)
            {
                var t_fashion = Config.get_t_fashion(this.player.fashion[i]);
                if (t_fashion != null)
                {
                    if (t_fashion.type == 1)
                        this.pskill_fashion = t_fashion;
                    else if (t_fashion.type == 2)
                        this.xuli_fashion = t_fashion;
                }
            }
        }
    }
    public override double get_scale()
    {
        var t_bodysize = Config.get_t_bodysize(player.score_level);
        if (t_bodysize == null)
            return 1;
        return t_bodysize.scale / 100.0;
    }
    
    public override void remove_buff_effect(int id)
    {
        var t_battle_buff = Config.get_t_battle_buff(id);
        if (t_battle_buff == null)
            return;

        id = t_battle_buff.effect_type;
        if (id == 0)
            return;

        if (this.buff_effect.ContainsKey(id))
        {
            if (this.buff_effect[id].num == 1)
            {
                if (this.buff_effect[id].obj != null)
                {
                    LuaHelper.GetResManager().DeleteEffect(this.buff_effect[id].obj);
                    if (!String.IsNullOrEmpty(t_battle_buff.effect_end))
                        BattlePlayers.Attach1(this, t_battle_buff.effect_bone, t_battle_buff.effect_end);
                }
                if (id == 3)
                    Battle.battle_panel.zhimang(this, false);
                else if (id == 6)
                    this.unit.continue_action();
                this.buff_effect.Remove(id);
            }
            else
                this.buff_effect[id].num = this.buff_effect[id].num - 1;
        }
    }

    public override void do_buff()
    {
        for (int i = this.animal.buffs.Count - 1; i >= 0; i--)
        {
            var t_battle_buff = Config.get_t_battle_buff(this.animal.buffs[i]);
            if (t_battle_buff != null)
            {
                if (BattlePlayers.zhen >= this.animal.buffs_time[i] && t_battle_buff.time != -1)
                {
                    this.animal.buffs.RemoveAt(i);
                    this.animal.buffs_time.RemoveAt(i);
                    this.remove_buff_effect(t_battle_buff.id);

                    if (this.animal.guid == self.guid)
                    {
                        BattleAchieve.BattleBuffer(t_battle_buff.id);
                        if (this.achieveRecords.bufferKill != null)
                            this.achieveRecords.bufferKill.Remove(t_battle_buff.id);
                    }

                    for (int j = 0; j < t_battle_buff.attr.Count; j++)
                    {
                        if (t_battle_buff.attr[j].ptype == 1)
                        {
                            this.add_attr_value(t_battle_buff.attr[j].param1, -t_battle_buff.attr[j].param2);
                            if (t_battle_buff.attr[j].param1 == 103)
                            {
                                if (this.animal.is_move)
                                    this.animal.r = BattleOperation.checkr(this.animal.r + 180);
                                this.animal.r_py = 0;
                            }
                            if (t_battle_buff.attr[j].param1 <= 2)
                                this.set_hp(this.animal.hp);
                        }
                    }
                }
            }
        }
    }

    public override void add_buff_effect(int id)
    {
        var t_battle_buff = Config.get_t_battle_buff(id);
        if (t_battle_buff == null)
            return;

        id = t_battle_buff.effect_type;
        if (id == 0)
            return;
        if (!this.buff_effect.ContainsKey(id))
        {
            this.buff_effect.Add(id, new Buffer_Effect());
            this.buff_effect[id].num = 1;
            if (!String.IsNullOrEmpty(t_battle_buff.effect))
                this.buff_effect[id].obj = BattlePlayers.Attach(this, t_battle_buff.effect_bone, t_battle_buff.effect, true);
            if (id == 3)
                Battle.battle_panel.zhimang(this, true);
            else if (id == 6)
                this.unit.pause_action();
        }
        else
            this.buff_effect[id].num = this.buff_effect[id].num + 1;
    }

    private float get_xuli_p()
    {
        float t = 1.0f;
        int lq = 100 - this.attr_value[93];
        if (lq < 0)
            lq = 0;
        t = lq / 100.0f;
        return t;
    }

    public void add_pre_eff()
    {
        string efname = "Unit_chargeAttack";
        if (this.xuli_fashion != null)
            efname = this.xuli_fashion.model;
        
        this.pre_eff = BattlePlayers.AttachSp(this, "accept", efname, true,1/get_xuli_p());
        this.pre_eff_state = 0;
        this.pre_eff_zhen = BattlePlayers.zhen;
    }

    public void add_scorebuff(int score)
    {
        var t = BattlePlayers.zhen * 1.0f / BattlePlayers.TNUM;
        if (t > 480)
            score = score * 2;

        for (int i = 0; i <= Config.max_score_buff_id; i++)
        {
            var t_hp_recover = Config.get_t_hp_recover(i);
            if (t_hp_recover.min_score <= score && t_hp_recover.max_score > score)
            {
                this.add_buff(t_hp_recover.buff_id);
                break;
            } 
        }
    }

    public void update_pf()
    {
        if (this.pf_res == null)
            return;
        for (int i = 1; i <= Config.max_pf_level; i++)
        {
            var t_scarf = Config.get_t_scarf(i);
            if (t_scarf.min <= this.player.lsha && t_scarf.max > this.player.lsha)
                this.pf_res.transform.Find(t_scarf.pf_name).gameObject.SetActive(true);
            else
            {
                if(i == Config.max_pf_level && this.player.lsha > t_scarf.max)
                    this.pf_res.transform.Find(t_scarf.pf_name).gameObject.SetActive(true);
                else
                    this.pf_res.transform.Find(t_scarf.pf_name).gameObject.SetActive(false);
            }     
        }
    }
    public void del_pre_eff()
    {
        LuaHelper.GetResManager().DeleteEffect(this.pre_eff);
        this.pre_eff_state = null;
        this.pre_eff_zhen = null;
    }

    public override void do_tick()
    {
        base.do_tick();
        if (this.player.re_pre > 0)
        {
            if (!this.can_do() || this.player.is_xueren || this.player.re_state > 0)
                this.player.re_pre_zhen = 0;
            else
                this.player.re_pre_zhen = this.player.re_pre_zhen + 1;
        }
        else
            this.player.re_pre_zhen = 0;

        if (this.pre_eff_state == null && this.player.re_pre_zhen * BattlePlayers.TICK > 500)
            this.add_pre_eff();
        else if (this.pre_eff_state != null && this.player.re_pre_zhen == 0)
            this.del_pre_eff();

        if (this.pre_eff_state == 0)
        {
            var t = (BattlePlayers.zhen - this.pre_eff_zhen.GetValueOrDefault()) * BattlePlayers.TICK;
            if (t > 1500 * this.get_xuli_p())
            {
                this.pre_eff_state = 1;
                LuaHelper.GetResManager().DeleteEffect(this.pre_eff);
                string efname = "Unit_chargeAttack_complete";
                if (this.xuli_fashion != null)
                    efname = this.xuli_fashion.hit_effect;
                this.pre_eff = BattlePlayers.Attach(this, "accept", efname, true);
            }
        }

        if (this.is_die)
        {
            if (Battle.is_newplayer_guide)
            {
                            
            }
            else
            {
                if ((BattlePlayers.zhen - this.animal.death_time) * BattlePlayers.TICK >= 5000)
                    BattlePlayers.Fuhuo(this);
            }           
        }


    }

    public override int get_skill_level(int id)
    {
        var t_skill = Config.get_t_skill(id);
        if (t_skill.type == 2)
        {
            if (t_skill.id == 300201)
                return 1;
            else
                return (this.player.cost / BattlePlayers.POWERUP);
        }
        return 1;
    }
    public virtual void inc_bskill()
    {
        for (int i = 0; i < t_role.bskills.Count; i++)
        {
            var t_role_skill = Config.get_t_role_skill(t_role.bskills[i]);
            if (t_role_skill != null)
            {
                for (int j = 0; j < t_role_skill.attrs.Count; j++)
                {
                    var sattr = t_role_skill.attrs[j];
                    if (sattr.type == 1)
                        this.add_attr_value(sattr.param1, sattr.param_value(this.player.role_level));
                    else if (sattr.type == 2)
                    {
                        var v = this.get_skill_attr_value(sattr.param1, sattr.param2) + sattr.param_value(this.player.role_level);
                        this.set_skill_attr_value(sattr.param1, sattr.param2, v);
                    }
                }
            }
        }

        for (int i = 0; i < this.player.attr_type.Count; i++)
        {
            if (this.player.attr_type[i] == 1)
                this.add_attr_value(this.player.attr_param1[i], this.player.attr_param3[i]);
            else if (this.player.attr_type[i] == 2)
            {
                var v = this.get_skill_attr_value(this.player.attr_param1[i], this.player.attr_param2[i]) + this.player.attr_param3[i];
                this.set_skill_attr_value(this.player.attr_param1[i], this.player.attr_param2[i], v);
            }
        }
    }
    public void inc_talent()
    {
        for (int i = 0; i < this.player.talent_id.Count; i++)
        {
            var t_talent = Config.get_t_talent(this.player.talent_id[i]);
            if (t_talent != null)
            {
                if (t_talent.type == 1)
                    this.add_attr_value(t_talent.param1, t_talent.param3 * this.player.talent_level[i]);
                else if (t_talent.type == 2)
                {
                    var v = this.get_skill_attr_value(t_talent.param1, t_talent.param2) + t_talent.param3 * this.player.talent_level[i];
                    this.set_skill_attr_value(t_talent.param1, t_talent.param2, v);
                }
            }
        }
    }
    
    public void add_lattr_value(int index, int value, bool reset = true)
    {
        this.player.lattr_value[index] = this.player.lattr_value[index] + value;
        if (reset)
            this.cattr = new CAttr();
    }
    public void mod_lattr_value(int index, int value, bool reset = true)
    {
        this.player.lattr_value[index] = value;
        if (reset)
            this.cattr = new CAttr();
    }
    public void add_exp(int ep, bool fj = true, bool exp_bar = false)
    {
        double old_pos = this.player.level;
        double new_pos = this.player.level;
        if (this.player.guid == self.guid)
            old_pos = old_pos + this.get_exp_per();
        double t = BattlePlayers.zhen * 1.0 / BattlePlayers.TNUM;
        int bl = 1;
        if (t > 480)
            bl = 2;
        this.player.exp = this.player.exp + BattleOperation.toInt(ep * bl * 1.0);
        var t_next_exp = Config.get_t_battle_exp(this.player.level + 1);
        if (t_next_exp == null)
            return;

        bool flag = false;
        while (this.player.exp > t_next_exp.exp)
        {
            var yhp = this.attr.max_hp() - this.player.hp;
            this.player.level = this.player.level + 1;
            int v = BattleOperation.toInt(this.attr.max_hp() * t_next_exp.huifu / 100.0);
            this.set_hp(this.attr.max_hp() - yhp + v);
            this.player.talent_point = this.player.talent_point + 1;
            flag = true;
            t_next_exp = Config.get_t_battle_exp(this.player.level + 1);
            new_pos = new_pos + 1;
            if (t_next_exp == null)
                break;
        }
        if (this.player.guid == self.guid)
        {
            new_pos = new_pos + this.get_exp_per();
            var role_pos = LuaHelper.GetMapManager().WorldToScreenPoint(this.objt.position);
            Battle.battle_panel.PickUpAnimation(role_pos, old_pos, new_pos, exp_bar);
        }

        if (flag)
        {
            BattlePlayers.Attach(this, "accept", "Unit_LevelUP");
            if (BattlePlayers.me != null && this.player.guid == BattlePlayers.me.player.guid && this.player.is_ai == 0)
            {
                Battle.battle_panel.ShowTalentPanel();
                BattleAchieve.BattleExpChange();
            }
            if (self.guid == this.player.guid)
                BattleAchieve.BattleRoleLevelCheck(this);
        }
    }

    public void add_score(int sc)
    {
        var t = BattlePlayers.zhen * 1.0f / BattlePlayers.TNUM;
        int bl = 1;
        if (t > 480)
            bl = 2;
        this.player.score = this.player.score + BattleOperation.toInt(sc * bl * 1.0);
        var t_next_score_level = Config.get_t_bodysize(this.player.score_level + 1);
        if (t_next_score_level == null)
            return;
        bool flag = false;
        while (this.player.score > t_next_score_level.score)
        {
            this.player.score_level = this.player.score_level + 1;
            flag = true;
            t_next_score_level = Config.get_t_bodysize(this.player.score_level + 1);
            if (t_next_score_level == null)
                break;
        }
        if (flag)
            this.do_scale();
        this.update_pf();
    }

    public double get_exp_per()
    {
        var t_exp = Config.get_t_battle_exp(this.player.level);
        var t_next_exp = Config.get_t_battle_exp(this.player.level + 1);
        if (t_exp == null || t_next_exp == null)
            return 1;
        return (this.player.exp - t_exp.exp) * 1.0 / (t_next_exp.exp - t_exp.exp);
    }
    public void add_power(int power_num)
    {
        this.player.cost += power_num;
        if (this.player.cost >= BattlePlayers.POWERUP * 3)
            this.player.cost = BattlePlayers.POWERUP * 3;

        if (this.player.cost < 0)
            this.player.cost = 0;
    }

    public void get_item(battle_item bi)
    {
        var t_battle_item = Config.get_t_battle_item(bi.item.id);
        if (t_battle_item != null)
        {
            if (t_battle_item.id == 0 || t_battle_item.id == 3)
            {
                int e = bi.item.param * 20;
                if (BattleOperation.random(0, 100) < this.attr_value[45])
                    e = e * 2;
                e = e + this.attr_value[31];
                e = BattleOperation.toInt(e * (1 + this.attr_value[32] / 100.0));
                this.add_exp(e, true, true);
                this.add_power(e);
                this.add_score(e);
                this.add_scorebuff(e);
                this.set_hp(this.player.hp + this.attr_value[34], true);
            }
            else if (t_battle_item.id == 1)
                this.add_buff(1010);             //this.set_hp(this.player.hp + BattleOperation.toInt(200 * (1 + this.attr_value[35] / 100.0)), true);
            else if (t_battle_item.id == 2)
                this.add_buff(3001);
            else if (t_battle_item.type == 4)
            {
                int num = 1;
                //if (BattleOperation.random(0, 100) < this.attr_value[44])   //技能翻倍
                //    num = 2;

                ////战斗开始0 - 3分钟拾取的是一级技能 3-6分钟是二级技能 后面拾取的都是三级技能
                //if (BattlePlayers.zhen < BattlePlayers.TNUM * 3 * 60)
                //    num = 1;
                //else if (BattlePlayers.zhen < BattlePlayers.TNUM * 6 * 60)
                //    num = 2;
                //else
                //    num = 3;

                if (this.player.skill_id == t_battle_item.skill)
                {
                    if (num > this.player.skill_level)
                    {
                        this.player.skill_level = num;
                        this.change_skill(t_battle_item.id, this.player.skill_level, 1);
                    }
                }
                else if (this.player.skill_id == 0)
                {
                    this.player.skill_id = t_battle_item.skill;
                    this.player.skill_level = num;
                    this.change_skill(t_battle_item.id, this.player.skill_level, 1);
                    if (this.player.guid == self.guid)
                        Battle.battle_panel.AddMainSkill(1);
                }
            }
            else if (t_battle_item.type == 5)
            {
                this.add_buff(4001);
            }
        }
    }
    public void change_skill(int id, int level, int pos)
    {
        if (BattlePlayers.me != null && this.player.guid != BattlePlayers.me.player.guid)
            return;

        var t_battle_item = Config.get_t_battle_item(id);
        if (t_battle_item == null)
            return;
        if (t_battle_item.type == 4)
            Battle.ChangeSkill(pos);
    }
    public void jisha_value()
    {
        this.add_lattr_value(0, this.attr_value[81], false);
        this.add_lattr_value(1, this.attr_value[82], false);
        this.add_lattr_value(2, this.attr_value[83], false);
        this.add_lattr_value(3, this.attr_value[84], false);
        this.add_lattr_value(4, this.attr_value[85], false);
        this.add_lattr_value(5, this.attr_value[86], false);
        this.add_lattr_value(6, this.attr_value[87], false);
        this.add_lattr_value(7, this.attr_value[88]);
    }
    public void change_talent(int talent_id, int level)
    {
        var t_talent = Config.get_t_talent(talent_id);
        if (t_talent != null)
        {
            if (level > 1)
            {
                if (t_talent.type == 1)
                    this.add_attr_value(t_talent.param1, -t_talent.param3 * (level - 1));
                else if (t_talent.type == 2)
                {
                    var v = this.get_skill_attr_value(t_talent.param1, t_talent.param2) - t_talent.param3 * (level - 1);
                    this.set_skill_attr_value(t_talent.param1, t_talent.param2, v);
                }
            }

            int? pre_hp_max = null;
            if (t_talent.type == 1 && t_talent.param1 == 2)
                pre_hp_max = this.attr.max_hp();
            if (t_talent.type == 1)
                this.add_attr_value(t_talent.param1, t_talent.param3 * level);
            else if (t_talent.type == 2)
            {
                var v = this.get_skill_attr_value(t_talent.param1, t_talent.param2) + t_talent.param3 * level;
                this.set_skill_attr_value(t_talent.param1, t_talent.param2, v);
            }

            if (pre_hp_max != null)
            {
                var dvalue = this.attr.max_hp() - pre_hp_max.GetValueOrDefault();
                this.set_hp(this.player.hp + dvalue);
            }
        }
    }

    public override void add_buff(int id, int? yc = default(int?), int? time = default(int?), int? jt = default(int?))
    {
        base.add_buff(id, yc, time, jt);
        if (id == 3001)
            this.mod_lattr_value(8, 1 + this.attr_value[78], false);
        if (this.animal.guid == self.guid)
            Battle.battle_panel.UnNormalState(id);
    }
    public int? get_talent_value(int t)
    {
        for (int i = 0; i < this.player.talent_id.Count; i++)
        {
            var t_talent = Config.get_t_talent(this.player.talent_id[i]);
            if (t_talent != null && t == 3 && t_talent.type == t)
                return t_talent.param1 * this.player.talent_level[i];
        }
        return null;
    }
    public override bool is_fight()
    {
        return ((BattlePlayers.zhen - this.player.rtime) * BattlePlayers.TICK <= 500) || (this.player.re_pre_zhen > 0);
    }
}

public class BattleAnimalMonster : BattleAnimal
{
    public battle_monster player;
    public GameObject shadow_res;
    public BattleAnimalMonster(battle_monster animal):base(animal)
    {
        this.player = animal;
        attr = new AnimalMonsterAttr(this);
        cattr = new CAttr();
        t_role = Config.get_t_role(this.animal.role_id);
        buff_effect = new Dictionary<int, Buffer_Effect>();
    }

    public override double get_scale()
    {
        var t_penguin = Config.get_t_penguin(this.player.monster_id);
        if (t_penguin != null)
            return t_penguin.scale / 100.0;
        else
            return 1.0;
    }

    public override int get_skill_level(int id)
    {
        if (id == this.player.skill_id)
            return this.player.skill_level;
        return 1;
    }

    public override void yy(int i)
    {
        if (BattlePlayers.jiasu)
            return;
        this.unit.play_monster_sound(t_role.yy[i], this.obj);
    }
}


public static class TypeTransform
{
    public static battle_player toBattlePlayer(msg_battle_player player)
    {
        battle_player bp = new battle_player();
        bp.attackZhen = player.attackZhen;
        bp.attack_guid = player.attack_guid;
        bp.attack_state = player.attack_state;
        bp.attr_param1 = player.attr_param1;
        bp.attr_param2 = player.attr_param2;
        bp.attr_param3 = player.attr_param3;
        bp.attr_type = player.attr_type;
        bp.avatar = player.avatar;
        bp.birth_x = player.birth_x;
        bp.birth_y = player.birth_y;
        bp.camp = player.camp;
        bp.cup = player.cup;
        bp.die = player.die;
        bp.exp = player.exp;
        bp.fashion = player.fashion;
        bp.is_xueren = player.is_xueren;
        bp.lattr_value = player.lattr_value;
        bp.level = player.level;
        bp.lsha = player.lsha;
        bp.max_lsha = player.max_lsha;
        bp.name = player.name;
        bp.name_color = player.name_color;
        bp.region_id = player.region_id;
        bp.re_pre = player.re_pre;
        bp.re_pre_zhen = player.re_pre_zhen;
        bp.re_pre_zhen1 = player.re_pre_zhen1;
        bp.role_level = player.role_level;
        bp.score = player.score;
        bp.score_level = player.score_level;
        bp.sex = player.sex;
        bp.sha = player.sha;
        bp.skill_id = player.skill_id;
        bp.skill_level = player.skill_level;
        bp.talent_id = player.talent_id;
        bp.talent_level = player.talent_level;
        bp.talent_point = player.talent_point;
        bp.toukuang = player.toukuang;
        bp.xueren_zhen = player.xueren_zhen;
        bp.cost = player.cost;

        //unit base
        bp.ai_state = player.unit.ai_state;
        bp.attackr = player.unit.attackr;
        bp.bhit_tids = player.unit.bhit_tids;
        bp.buffs = player.unit.buffs;
        bp.buffs_time = player.unit.buffs_time;
        bp.death_time = player.unit.death_time;
        bp.eyeRange = player.unit.eyeRange;
        bp.guid = player.unit.guid.ToString();
        bp.hp = player.unit.hp;
        bp.is_ai = player.unit.is_ai;
        bp.is_jf = player.unit.is_jf;
        bp.is_move = player.unit.is_move;
        bp.jf_r = player.unit.jf_r;
        bp.jf_speed = player.unit.jf_speed;
        bp.jf_sx = player.unit.jf_sx;
        bp.jf_sy = player.unit.jf_sy;
        bp.jf_xx = player.unit.jf_xx;
        bp.jf_yy = player.unit.jf_yy;
        bp.nextPoint_x = player.unit.nextPoint_x;
        bp.nextPoint_y = player.unit.nextPoint_y;
        bp.r = player.unit.r;
        bp.re_id = player.unit.re_id;
        bp.re_level = player.unit.re_level;
        bp.re_r = player.unit.re_r;
        bp.re_state = player.unit.re_state;
        bp.re_tid = player.unit.re_tid;
        bp.re_time = player.unit.re_time;
        bp.re_x = player.unit.re_x;
        bp.re_y = player.unit.re_y;
        bp.role_id = player.unit.role_id;
        bp.rtime = player.unit.rtime;
        bp.r_py = player.unit.r_py;
        bp.save_re_id = player.unit.save_re_id;
        bp.save_re_zhen = player.unit.save_re_zhen;
        bp.totalZhen = player.unit.totalZhen;
        bp.x = player.unit.x;
        bp.y = player.unit.y;
        bp.type = unit_type.Player;
        return bp;
    }

    public static battle_boss toBattleBoss(msg_battle_boss player)
    {
        battle_boss bp = new battle_boss();
        bp.is_xueren = false;
        bp.ai_state = player.unit.ai_state;
        bp.attackr = player.unit.attackr;
        bp.bhit_tids = player.unit.bhit_tids;
        bp.buffs = player.unit.buffs;
        bp.buffs_time = player.unit.buffs_time;
        bp.death_time = player.unit.death_time;
        bp.eyeRange = player.unit.eyeRange;
        bp.guid = player.unit.guid.ToString();
        bp.hp = player.unit.hp;
        bp.is_ai = player.unit.is_ai;
        bp.is_jf = player.unit.is_jf;
        bp.is_move = player.unit.is_move;
        bp.jf_r = player.unit.jf_r;
        bp.jf_speed = player.unit.jf_speed;
        bp.jf_sx = player.unit.jf_sx;
        bp.jf_sy = player.unit.jf_sy;
        bp.jf_xx = player.unit.jf_xx;
        bp.jf_yy = player.unit.jf_yy;
        bp.nextPoint_x = player.unit.nextPoint_x;
        bp.nextPoint_y = player.unit.nextPoint_y;
        bp.r = player.unit.r;
        bp.re_id = player.unit.re_id;
        bp.re_level = player.unit.re_level;
        bp.re_r = player.unit.re_r;
        bp.re_state = player.unit.re_state;
        bp.re_tid = player.unit.re_tid;
        bp.re_time = player.unit.re_time;
        bp.re_x = player.unit.re_x;
        bp.re_y = player.unit.re_y;
        bp.role_id = player.unit.role_id;
        bp.rtime = player.unit.rtime;
        bp.r_py = player.unit.r_py;
        bp.save_re_id = player.unit.save_re_id;
        bp.save_re_zhen = player.unit.save_re_zhen;
        bp.totalZhen = player.unit.totalZhen;
        bp.x = player.unit.x;
        bp.y = player.unit.y;
        bp.type = unit_type.Boss;

        var t_boss_attr = Config.get_t_boss_attr(Convert.ToInt32(player.unit.guid));
        bp.boss_id = t_boss_attr.id;
        bp.attack_state = player.attack_state;
        bp.attackZhen = player.attackZhen;
        bp.boss_birth_time = player.boss_birth_time;
        bp.name = t_boss_attr.name;
        bp.role_level = 1;
        bp.camp = t_boss_attr.id;
        return bp;
    }

    public static battle_monster toBattleMonster(msg_battle_monster player)
    {
        battle_monster bp = new battle_monster();
        bp.camp = -1;
        bp.is_xueren = false;
        bp.ai_state = player.unit.ai_state;
        bp.attackr = player.unit.attackr;
        bp.bhit_tids = player.unit.bhit_tids;
        bp.buffs = player.unit.buffs;
        bp.buffs_time = player.unit.buffs_time;
        bp.death_time = player.unit.death_time;
        bp.eyeRange = player.unit.eyeRange;
        bp.guid = player.unit.guid.ToString();
        bp.hp = player.unit.hp;
        bp.is_ai = player.unit.is_ai;
        bp.is_jf = player.unit.is_jf;
        bp.is_move = player.unit.is_move;
        bp.jf_r = player.unit.jf_r;
        bp.jf_speed = player.unit.jf_speed;
        bp.jf_sx = player.unit.jf_sx;
        bp.jf_sy = player.unit.jf_sy;
        bp.jf_xx = player.unit.jf_xx;
        bp.jf_yy = player.unit.jf_yy;
        bp.nextPoint_x = player.unit.nextPoint_x;
        bp.nextPoint_y = player.unit.nextPoint_y;
        bp.r = player.unit.r;
        bp.re_id = player.unit.re_id;
        bp.re_level = player.unit.re_level;
        bp.re_r = player.unit.re_r;
        bp.re_state = player.unit.re_state;
        bp.re_tid = player.unit.re_tid;
        bp.re_time = player.unit.re_time;
        bp.re_x = player.unit.re_x;
        bp.re_y = player.unit.re_y;
        bp.role_id = player.unit.role_id;
        bp.rtime = player.unit.rtime;
        bp.r_py = player.unit.r_py;
        bp.save_re_id = player.unit.save_re_id;
        bp.save_re_zhen = player.unit.save_re_zhen;
        bp.totalZhen = player.unit.totalZhen;
        bp.x = player.unit.x;
        bp.y = player.unit.y;
        bp.type = unit_type.Monster;

        var t_penguin = Config.get_t_penguin(player.unit.is_ai);
        bp.name = t_penguin.name;
        bp.role_level = 1;
        bp.monster_id = t_penguin.id;
        bp.birth_x = player.birth_x;
        bp.birth_y = player.birth_y;
        bp.attack_guid = player.attack_guid;
        bp.skill_id = t_penguin.skill_id;
        bp.skill_level = 1;

        return bp;
    }

    public static msg_battle_boss toMsgBattleBoss(battle_boss info)
    {
        msg_battle_boss msg = new msg_battle_boss();
        msg.attackZhen = info.attackZhen;
        msg.attack_state = info.attack_state;
        msg.boss_birth_time = info.boss_birth_time;
        //unit
        msg.unit = new msg_battle_unit();
        msg.unit.ai_state = info.ai_state;
        msg.unit.attackr = info.attackr;

        msg.unit.bhit_tids.Clear();
        for (int i = 0; i < info.bhit_tids.Count; i++)
            msg.unit.bhit_tids.Add(info.bhit_tids[i]);
        msg.unit.buffs.Clear();
        for (int i = 0; i < info.buffs.Count; i++)
            msg.unit.buffs.Add(info.buffs[i]);
        msg.unit.buffs_time.Clear();
        for (int i = 0; i < info.buffs_time.Count; i++)
            msg.unit.buffs_time.Add(info.buffs_time[i]);
        msg.unit.death_time = info.death_time;
        msg.unit.eyeRange = info.eyeRange;
        msg.unit.guid = Convert.ToUInt64(info.guid);
        msg.unit.hp = info.hp;
        msg.unit.is_ai = info.is_ai;
        msg.unit.is_jf = info.is_jf;
        msg.unit.is_move = info.is_move;
        msg.unit.jf_r = info.jf_r;
        msg.unit.jf_speed = info.jf_speed;
        msg.unit.jf_sx = info.jf_sx;
        msg.unit.jf_sy = info.jf_sy;
        msg.unit.jf_xx = info.jf_xx;
        msg.unit.jf_yy = info.jf_yy;
        msg.unit.nextPoint_x = info.nextPoint_x;
        msg.unit.nextPoint_y = info.nextPoint_y;
        msg.unit.r = info.r;
        msg.unit.re_id = info.re_id;
        msg.unit.re_level = info.re_level;
        msg.unit.re_r = info.re_r;
        msg.unit.re_state = info.re_state;
        msg.unit.re_tid = info.re_tid;
        msg.unit.re_time = info.re_time;
        msg.unit.re_x = info.re_x;
        msg.unit.re_y = info.re_y;
        msg.unit.role_id = info.role_id;
        msg.unit.rtime = info.rtime;
        msg.unit.r_py = info.r_py;

        msg.unit.save_re_id.Clear();
        for (int i = 0; i < info.save_re_id.Count; i++)
            msg.unit.save_re_id.Add(info.save_re_id[i]);
        msg.unit.save_re_zhen.Clear();
        for (int i = 0; i < info.save_re_zhen.Count; i++)
            msg.unit.save_re_zhen.Add(info.save_re_zhen[i]);
        msg.unit.totalZhen = info.totalZhen;
        msg.unit.x = info.x;
        msg.unit.y = info.y;
        return msg;
    }

    public static msg_battle_monster toMsgBattleMonster(battle_monster info)
    {
        msg_battle_monster msg = new msg_battle_monster();
        msg.attack_guid = info.attack_guid;
        msg.birth_x = info.birth_x;
        msg.birth_y = info.birth_y;

        msg.unit = new msg_battle_unit();
        msg.unit.ai_state = info.ai_state;
        msg.unit.attackr = info.attackr;

        msg.unit.bhit_tids.Clear();
        for (int i = 0; i < info.bhit_tids.Count; i++)
            msg.unit.bhit_tids.Add(info.bhit_tids[i]);
        msg.unit.buffs.Clear();
        for (int i = 0; i < info.buffs.Count; i++)
            msg.unit.buffs.Add(info.buffs[i]);
        msg.unit.buffs_time.Clear();
        for (int i = 0; i < info.buffs_time.Count; i++)
            msg.unit.buffs_time.Add(info.buffs_time[i]);
        msg.unit.death_time = info.death_time;
        msg.unit.eyeRange = info.eyeRange;
        msg.unit.guid = Convert.ToUInt64(info.guid);
        msg.unit.hp = info.hp;
        msg.unit.is_ai = info.is_ai;
        msg.unit.is_jf = info.is_jf;
        msg.unit.is_move = info.is_move;
        msg.unit.jf_r = info.jf_r;
        msg.unit.jf_speed = info.jf_speed;
        msg.unit.jf_sx = info.jf_sx;
        msg.unit.jf_sy = info.jf_sy;
        msg.unit.jf_xx = info.jf_xx;
        msg.unit.jf_yy = info.jf_yy;
        msg.unit.nextPoint_x = info.nextPoint_x;
        msg.unit.nextPoint_y = info.nextPoint_y;
        msg.unit.r = info.r;
        msg.unit.re_id = info.re_id;
        msg.unit.re_level = info.re_level;
        msg.unit.re_r = info.re_r;
        msg.unit.re_state = info.re_state;
        msg.unit.re_tid = info.re_tid;
        msg.unit.re_time = info.re_time;
        msg.unit.re_x = info.re_x;
        msg.unit.re_y = info.re_y;
        msg.unit.role_id = info.role_id;
        msg.unit.rtime = info.rtime;
        msg.unit.r_py = info.r_py;

        msg.unit.save_re_id.Clear();
        for (int i = 0; i < info.save_re_id.Count; i++)
            msg.unit.save_re_id.Add(info.save_re_id[i]);
        msg.unit.save_re_zhen.Clear();
        for (int i = 0; i < info.save_re_zhen.Count; i++)
            msg.unit.save_re_zhen.Add(info.save_re_zhen[i]);
        msg.unit.totalZhen = info.totalZhen;
        msg.unit.x = info.x;
        msg.unit.y = info.y;
        return msg;
    }

    public static msg_battle_player toMsgBattlePlayer(battle_player info)
    {
        msg_battle_player msg = new msg_battle_player();
        msg.attackZhen = info.attackZhen;
        msg.attack_guid = info.attack_guid;
        msg.attack_state = info.attack_state;
        msg.attr_param1.Clear();
        for (int i = 0; i < info.attr_param1.Count; i++)
            msg.attr_param1.Add(info.attr_param1[i]);
        msg.attr_param2.Clear();
        for (int i = 0; i < info.attr_param2.Count; i++)
            msg.attr_param2.Add(info.attr_param2[i]);
        msg.attr_param3.Clear();
        for (int i = 0; i < info.attr_param3.Count; i++)
            msg.attr_param3.Add(info.attr_param3[i]);
        msg.attr_type.Clear();
        for (int i = 0; i < info.attr_type.Count; i++)
            msg.attr_type.Add(info.attr_type[i]);
        msg.avatar = info.avatar;
        msg.birth_x = info.birth_x;
        msg.birth_y = info.birth_y;
        msg.camp = info.camp;
        msg.cup = info.cup;
        msg.die = info.die;
        msg.exp = info.exp;
        msg.fashion.Clear();
        for (int i = 0; i < info.fashion.Count; i++)
            msg.fashion.Add(info.fashion[i]);
        msg.is_xueren = info.is_xueren;
        msg.lattr_value.Clear();
        for (int i = 0; i < info.lattr_value.Count; i++)
            msg.lattr_value.Add(info.lattr_value[i]);
        msg.level = info.level;
        msg.lsha = info.lsha;
        msg.max_lsha = info.max_lsha;
        msg.name = info.name;
        msg.name_color = info.name_color;
        msg.region_id = info.region_id;
        msg.re_pre = info.re_pre;
        msg.re_pre_zhen = info.re_pre_zhen;
        msg.re_pre_zhen1 = info.re_pre_zhen1;
        msg.role_level = info.role_level;
        msg.score = info.score;
        msg.score_level = info.score_level;
        msg.sex = info.sex;
        msg.sha = info.sha;
        msg.skill_id = info.skill_id;
        msg.skill_level = info.skill_level;
        msg.talent_id.Clear();
        for (int i = 0; i < info.talent_id.Count; i++)
            msg.talent_id.Add(info.talent_id[i]);
        msg.talent_level.Clear();
        for (int i = 0; i < info.talent_level.Count; i++)
            msg.talent_level.Add(info.talent_level[i]);
        msg.talent_point = info.talent_point;
        msg.toukuang = info.toukuang;
        msg.xueren_zhen = info.xueren_zhen;
        msg.cost = info.cost;

        msg.unit = new msg_battle_unit();
        msg.unit.ai_state = info.ai_state;
        msg.unit.attackr = info.attackr;

        msg.unit.bhit_tids.Clear();
        for (int i = 0; i < info.bhit_tids.Count; i++)
            msg.unit.bhit_tids.Add(info.bhit_tids[i]);
        msg.unit.buffs.Clear();
        for (int i = 0; i < info.buffs.Count; i++)
            msg.unit.buffs.Add(info.buffs[i]);
        msg.unit.buffs_time.Clear();
        for (int i = 0; i < info.buffs_time.Count; i++)
            msg.unit.buffs_time.Add(info.buffs_time[i]);
        msg.unit.death_time = info.death_time;
        msg.unit.eyeRange = info.eyeRange;
        msg.unit.guid = Convert.ToUInt64(info.guid);
        msg.unit.hp = info.hp;
        msg.unit.is_ai = info.is_ai;
        msg.unit.is_jf = info.is_jf;
        msg.unit.is_move = info.is_move;
        msg.unit.jf_r = info.jf_r;
        msg.unit.jf_speed = info.jf_speed;
        msg.unit.jf_sx = info.jf_sx;
        msg.unit.jf_sy = info.jf_sy;
        msg.unit.jf_xx = info.jf_xx;
        msg.unit.jf_yy = info.jf_yy;
        msg.unit.nextPoint_x = info.nextPoint_x;
        msg.unit.nextPoint_y = info.nextPoint_y;
        msg.unit.r = info.r;
        msg.unit.re_id = info.re_id;
        msg.unit.re_level = info.re_level;
        msg.unit.re_r = info.re_r;
        msg.unit.re_state = info.re_state;
        msg.unit.re_tid = info.re_tid;
        msg.unit.re_time = info.re_time;
        msg.unit.re_x = info.re_x;
        msg.unit.re_y = info.re_y;
        msg.unit.role_id = info.role_id;
        msg.unit.rtime = info.rtime;
        msg.unit.r_py = info.r_py;

        msg.unit.save_re_id.Clear();
        for (int i = 0; i < info.save_re_id.Count; i++)
            msg.unit.save_re_id.Add(info.save_re_id[i]);
        msg.unit.save_re_zhen.Clear();
        for (int i = 0; i < info.save_re_zhen.Count; i++)
            msg.unit.save_re_zhen.Add(info.save_re_zhen[i]);
        msg.unit.totalZhen = info.totalZhen;
        msg.unit.x = info.x;
        msg.unit.y = info.y;
        return msg;
    }

    public static List<battle_item_base> toBattleItem(List<msg_battle_item> items,List<msg_battle_item_base> bases)
    {
        List<battle_item_base> list = new List<battle_item_base>(); 
        for (int i = 0; i < items.Count; i++)
        {
            battle_item_base btb = new battle_item_base();
            btb.birth_pos = items[i].birth_pos;
            btb.id = items[i].item.id;
            btb.param = items[i].item.param;
            btb.tid = items[i].item.tid;
            btb.x = items[i].item.x;
            btb.y = items[i].item.y;
            btb.zhen = items[i].item.zhen;
            list.Add(btb);
        }

        for (int i = 0; i < bases.Count; i++)
        {
            battle_item_base mbb = new battle_item_base();
            mbb.birth_pos = 0;
            mbb.id = bases[i].id;
            mbb.param = bases[i].param;
            mbb.tid = bases[i].tid;
            mbb.x = bases[i].x;
            mbb.y = bases[i].y;
            mbb.zhen = bases[i].zhen;
            list.Add(mbb);
        }
        return list;
    }

    public static void toBattleItem(List<battle_item_base> list,out List<msg_battle_item> items,out List<msg_battle_item_base> bases)
    {
        items = new List<msg_battle_item>();
        bases = new List<msg_battle_item_base>();

        foreach (var item in list)
        {
            if (item.birth_pos == 0)
            {
                msg_battle_item_base mb = new msg_battle_item_base();
                mb.id = item.id;
                mb.param = item.param;
                mb.tid = item.tid;
                mb.x = item.x;
                mb.y = item.y;
                mb.zhen = item.zhen;
                bases.Add(mb);
            }
            else
            {
                msg_battle_item mb = new msg_battle_item() {  item = new msg_battle_item_base()};
                mb.item.id = item.id;
                mb.item.param = item.param;
                mb.item.tid = item.tid;
                mb.item.x = item.x;
                mb.item.y = item.y;
                mb.item.zhen = item.zhen;
                mb.birth_pos = item.birth_pos;
                items.Add(mb);
            }
        }
    }
}