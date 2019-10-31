using protocol.game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BattleDB
{
    public class AchieveRecords
    {
        public bool IsUseSkill;
        public bool useXueren;
        public List<int> changeSkill;
        public int killMale;
        public int killFemale;
        public bool IsUseNormalToKill;
        public bool IsUseSkillToKill;
        public int quietKills;
        public Dictionary<string, int> kills;
        public int blood_ten_pert;
        public int killCaos;
        public int killDeath;
        public int killHeader;
        public Dictionary<int, int> bufferKill;
    }

    public class Buffer_Effect
    {
        public GameObject obj;
        public int num;
    }
    public class battle_effect
    {
        public msg_battle_effect effect;
        public GameObject obj;
        public Transform objt;
        public int objid;
        public List<List<object>> last;
        public double last_time;
        public BattleAnimal re_bp;
        public Dictionary<int, int> destroyEffects;
        public Dictionary<string, int> effect_hums;
        public Vector3 p0;
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 p3;
        public double pt;
    }

    public class battle_item_base
    {
        public int tid;
        public int id;
        public int x;
        public int y;
        public int param;
        public int zhen;
        public int birth_pos;
    }

    public class battle_item
    {
        public battle_item_base item;
        public GameObject obj;
        public int objid;
        public Transform objt;
        public double xx;
        public double yy;
        public long ll;
        public double speed;
        public string follow;
        public double follow_time;
        public double follow_speed;
    }

    public class battle_message_link
    {
        public msg_battle_op msg;
        public battle_message_link nextp;
    }

    public class t_lang
    {
        public string id;
        public string[] lang;
    }
    public class t_role
    {
        public int id;
        public string name;
        public int color;
        public int sex;
        public string icon;
        public string res;
        public string desc;
        public string[] yy;
        public int hp;
        public int hp_add;
        public int atk;
        public int atk_add;
        public int def;
        public int def_add;
        public int range;
        public int aspeed;
        public int speed;
        public List<int> gskills;
        public List<int> bskills;
        public int suipian_id;
        public int suipian_cost;
        public int type;
        public int get_type;
        public int pf_x;
        public int pf_y;
        public int pf_z;
    }

    public class t_boss_attr
    {
        public int id;
        public string name;
        public int role_id;
        public int refresh_t;
        public int life_t;
        public int body_scale;
        public int birth_x;
        public int birth_y;
        public int def;
        public int skill1;
        public int skill2;
        public int patk_power;
        public int xl_power;
        public int bexp;
        public int bscore;
        public int maxb;
        public int max_dis;
        public int toukuang_id;
        //BUFF道具	权重	item2爆出	item2爆出
        public int buff_id;
        public int wt;
        public int b_amount;
        public int b_pro;
    }

    public class fQua
    {
        public int type;
        public int param1;
        public int param2;
        public int param3;
    }

    public class pattr
    {
        public int ptype;
        public int param1;
        public int param2;
    }

    public class t_toukuang
    {
        public int id;
        public string name;
        public string big_icon;
        public int color;
        public string desc1;
        public string desc2;
        public string icon;
        public List<fQua> gskills;
        public string desc;
    }

    public class t_fashion
    {
        public int id;
        public string name;
        public int type;
        public string icon;
        public int color;
        public string model;
        public string hit_effect;
        public string down_effect;
        public string show;
        public string desc1;
        public string desc2;
        public string desc;
        public int buchang;
        public int param_type;
        public int param1;
        public int param2;
        public int param3;
    }

    public class t_bodysize
    {
        public int id;
        public int score;
        public int scale;
        public int dis;
        public int snowsize;
    }

    public class t_battle_exp
    {
        public int level;
        public int exp;
        public int gexp;
        public int bexp;
        public int bscore;
        public int max_bl;
        public int max_dis;
        public int huifu;
        public int item_num2;
        public int item_rate2;
    }

    public class t_battle_item
    {
        public int id;
        public string name;
        public int type;
        public string effect;
        public string geffect;
        public int skill;
        public int max;
        public string desc;
        public int icon;
        public int param1;
        public int param2;
    }

    public class t_battle_buff
    {
        public int id;
        public string name;
        public int effect_type;
        public string effect;
        public string effect_end;
        public string effect_bone;
        public List<pattr> attr;
        public int time;
    }

    public class t_ai_attr
    {
        public int ai_type;
        public int attack_p;
        public int attack_t;
        public int damage_p;
        public int snow_p;
        public int fire_p;
        public int exp_t;
        public int talent_p;
        public int stand_p;
        public int stand_min;
        public int stand_max;
        public int charge_attack_p;
        public int attack_player_p;
    }

    public class t_skill
    {
        public int id;
        public string name;
        public int level;
        public int type;
        public int cd;
        public int release_type;
        public int range;
        public int range_param;
        public string action;
        public int qy_action_time;
        public int link_effect;
        public int wy_speed;
        public int wy_time;
        public int hy_td_time;
        public string icon;
        public int cost;

        public int get_range(BattleAnimal bp)
        {
            int range = this.range;
            if (this.type == 1 && bp.animal.type == unit_type.Player)
            {
                var tp = bp as BattleAnimalPlayer; 
                int pre = BattleOperation.calc_pre(tp,tp.player.re_pre_zhen);
                range = bp.attr.range() + 200 * pre;
            }
            else if (type == 2)
                range = range * (1 + BattleOperation.toInt(bp.get_skill_attr_value(id, 2) / 100.0));
            else
                range = bp.attr.range();
            return range;
        }

        public int get_range_param(BattleAnimal bp)
        {
            int range_param_ = this.range_param;
            if (type == 1 && bp.animal.type == unit_type.Player)
            {
                var tp = bp as BattleAnimalPlayer;
                var pre = BattleOperation.calc_pre(tp,tp.player.re_pre_zhen);
                range_param_ = range_param_ + 2000 * pre / 100;
                if (range_param_ > 20000)
                    range_param_ = 20000;
            }
            else if (type == 2 && (release_type == 2 || release_type == 3))
            {
                range_param_ = range_param_ * (1 + BattleOperation.toInt(bp.get_skill_attr_value(id, 3) / 100.0));
            }
            return range_param_;
        }

        public int get_cd(BattleAnimal bp)
        {
            var scd = cd;
            if (type == 1)
            {
                if (bp.animal.is_ai > 0)
                {
                    if (bp.animal.type == unit_type.Player)
                    {
                        var t_ai_attr = Config.get_t_ai_attr(bp.animal.is_ai);
                        scd = t_ai_attr.attack_t;
                    }

                    if (Battle.is_newplayer_guide)
                        scd = 800;
                }
                else
                {
                    scd = BattleOperation.toInt(cd / (1 + bp.attr_value[92] / 100.0));  //92 增加攻速
                }
            }
            if (bp.animal.type == unit_type.Player && bp.animal.is_ai > 0 && type == 1)
            {
                var t_ai_attr = Config.get_t_ai_attr(bp.animal.is_ai);
                scd = t_ai_attr.attack_t;
            }

            if (type == 2)
            {
                var v = 1 - (bp.attr_value[23] + bp.get_skill_attr_value(id, 7)) / 100.0;
                scd = BattleOperation.toInt(scd * v);
                if (Battle.is_newplayer_guide)
                {
                    scd = 800;
                }
            }

            if (type == 3)
            {
                var v = 1 - bp.attr_value[24] / 100.0;
                scd = BattleOperation.toInt(scd * v);
                if (Battle.is_newplayer_guide )
                {
                    scd = 800;
                }
            }

            if (Battle.is_newplayer_guide && bp.animal.is_ai > 0 && type == 1)
                scd = 800;

            return scd;
        }
    }
    public class SAttr
    {
        public int type;
        public int param1;
        public int param2;
        public int param3;
        public int param4;
        public int param_value(int level)
        {
            return param3 + param4 * (level - 1);
        }
    }
    public class t_role_skill
    {
        public int id;
        public string name;
        public string desc;
        public List<SAttr> attrs;
    }

    public class t_talent
    {
        public int id;
        public string desc1;
        public string desc2;
        public int max_level;
        public string icon;
        public int type;
        public int param1;
        public int param2;
        public int param3;
    }

    public class t_skill_effect
    {
        public int id;
        public string name;
        public int skill_id;
        public int skill_type;
        public int type;
        public int range;
        public int range_param;
        public int release_pos;
        public string effect;
        public string xiaoshi_effect;
        public int is_zaxs;
        public string xiaoshi_effect1;
        public int is_jzxs;
        public string xiaoshi_effect2;
        public double effect_scale;
        public int fx_speed;
        public int fx_hight;
        public int fx_hight1;
        public int fx_hight2;
        public int sh_time;
        public int time;
        public int target_type;
        public int dd_type;
        public double dd_jg;
        public int is_zp;
        public int fl_num;
        public int fl_r;
        public int link_xiaoguo;
        public int link_effect;
        public int link_effect1;
        public int togather_effect;
        public int hit_effect;

        public int get_range(BattleAnimal bp, int zhen)
        {
            int range = this.range;
            if (skill_type == 1)
            {
                var pre = BattleOperation.calc_pre(bp,zhen);
                range = bp.attr.range() + 200 * pre;
            }
            else if (skill_type == 2)
                range = range *BattleOperation.toInt((1 + bp.get_skill_attr_value(skill_id, 2) / 100.0));
            return range;
        }

        public int get_range_param(BattleAnimal bp, int zhen)
        {
            var range_param = this.range_param;
            if (skill_type == 1)
            {
                var pre = BattleOperation.calc_pre(bp,zhen);
                range_param = range_param + 2000 * pre / 100;
                if (range_param > 20000)
                    range_param = 20000;
            }
            else if (skill_type == 2)
                range_param = range_param * BattleOperation.toInt((1 + bp.get_skill_attr_value(skill_id, 3) / 100.0));
            return range_param;
        }

        public double get_effect_scale(BattleAnimal bp, int zhen)
        {
            double scale = this.effect_scale;
            if (skill_type == 1 && bp is BattleAnimalPlayer)
            {
                var tmp = bp as BattleAnimalPlayer;
                var pre = BattleOperation.calc_pre(bp,zhen);
                var t_bodysize = Config.get_t_bodysize(tmp.player.score_level);
                scale = t_bodysize.snowsize / 100.0 + pre / 100.0;
                if (scale > 3)
                    scale = 3;
            }
            else if (skill_type == 2)
                scale = scale * (1 + bp.get_skill_attr_value(skill_id, 3) / 100.0f);

            return scale;
        }

        public int get_fx_speed(BattleAnimal bp)
        {
            var fx_speed = this.fx_speed;
            if (skill_type == 1)
            {
                if (fx_speed > 200000)
                    fx_speed = 200000;
            }
            else if (skill_type == 2)
                fx_speed = BattleOperation.toInt(fx_speed * (1 + bp.get_skill_attr_value(skill_id, 4) / 100.0));
            return fx_speed;
        }
    }

    public class t_ai_setting
    {
        public int level;
        public int low_ai_amount;
        public int mid_ai_amount;
        public int high_ai_amount;
    }

    public class t_skill_xiaoguo
    {
        public int id;
        public string name;
        public string sj_effect;
        public int dmg_type;
        public int dmg_per;
        public int dmg_gd;
        public int jf_type;
        public int jf_dis;
        public int jf_speed;
        public int link_buff;
    }

    public class t_role_buff
    {
        public int id;
        public string name;
        public string desc;
        public int type;
        public int param1;
        public int param2;
        public int param3;
        public int param4;

        public int param_value(int level)
        {
            return param3 + param4 * (level - 1);
        }
    }

    public class t_achievement
    {
        public int id;
        public string name;
        public string desc;
        public int dtype;
        public int pre;
        public int target_num;
        public int type_id;
        public int param1;
        public int param2;
        public int param3;
        public int param4;
        public int point;
        public int max_star;
        public int star;
        public string icon;
        public int? next_achieve_id;
    }

    public class Rds
    {
        public int type;
        public int value1;
        public int value2;
        public int value3;
    }

    public class t_task
    {
        public int id;
        public string desc;
        public int level;
        public int dtype;
        public int target_num;
        public int type;
        public int param1;
        public int param2;
        public int param3;
        public int param4;
        public List<Rds> rewards;
    }

    public class t_daily
    {
        public int id;
        public string name;
        public string desc;
        public int dtype;
        public int target_num;
        public int type;
        public int param1;
        public int param2;
        public int param3;
        public int param4;
        public List<Rds> rewards;
    }

    public class guide_step_info
    {
        public List<t_guide_init> inits;   //初始化这个步骤显示的效果
        public List<t_guide_condition> conditions;  //这个小步骤检测的条件
    }

    public class guide_step
    {
        public int guide_id;
        public Dictionary<int, guide_step_info> steps;
    }

    public class t_preguide
    {
        public int id;
        public int guide_id;
        public int type;
        public int offset_x;
        public int offset_y;
        public int param_int;
        public int param1;
        public int param2;
        public int param3;
    }

    public class t_battle_attr
    {
        public int id;
        public int isBadState;
        public string icon;
        public string icon_bg;
    }

    public class t_teamid
    {
        public int id;
        public string name;
    }

    public class t_avatar
    {
        public int id;
        public string name;
        public int role_id;
        public string icon;
        public string desc1;
        public string desc2;
        public int color;
    }

    public class t_exp
    {
        public int level;
        public int exp;
        public List<Rds> rewards;
        public List<int> tasks;
        public List<fQua> level_add;
    }

    public class t_vip_attr
    {
        public int id;
        public string color;
        public int first_id;
        public int day_id;
        public List<fQua> attrs;
    }

    public class t_cup
    {
        public int id;
        public string name;
        public int star;
        public int max_star;
        public string icon;
        public string small_icon;
        public string dai_icon;
        public int duan;
        public int down;
        public int sb;
        public int jb;
        public int tsb;
        public int tjb;
        public int tsbnum;
        public List<Rds> rewards;
    }

    public class t_item
    {
        public int id;
        public string name;
        public int color;
        public int type;
        public int level;
        public string desc;
        public string icon;
        public int price;
        public int sell_type;
        public int sell;
        public int def1;
        public int def2;
        public int def3;
        public int def4;
    }

    public class t_penguin
    {
        public int id;
        public string name;
        public int type;
        public int role_id;
        public int scale;
        public int view_radius;
        public int exp;
        public int max_bl;
        public int max_dis;
        public int skill_id;
        public int standp;
        public int mint;
        public int maxt;
        public int b_amount;
        public int b_pro;
    }

    public class t_penguin_num
    {
        public int type_id;
        public int num;
    }

    public class t_battle_refresh_item
    {
        public int id;
        public string name;
        public int amount;
        public int wt;
    }

    public class t_battle_item_pos
    {
        public int id;
        public string name;
        public int pos_x;
        public int pos_y;
        public int max_dis;
        public int max_amount;
        public List<t_battle_refresh_item> bts;
    }

    public class t_scarf
    {
        public int id;
        public int min;
        public int max;
        public string pf_name;
    }

    public class t_battle_msg
    {
        public int id;
        public string msg;
    }

    public class t_hp_recover
    {
        public int id;
        public int min_score;
        public int max_score;
        public int buff_id;
    }

    public class t_script_str
    {
        public string id;
        public List<string> lang;
    }

    public class t_guide_btn_path
    {
        public int id;
        public string path;
    }

    public class t_guide
    {
        public int id;
        public int step_id;
        public int step_child_id;
    }

    public class t_guide_init
    {
        public int id;
        public int guide_id;
        public int type;
        public int param_int;
        public int param1;
        public int param2;
        public int param3;
        public string param_string;
        public int offset_x;
        public int offset_y;
    }

    public class t_guide_condition
    {
        public int id;
        public int guide_id;
        public int type;
        public int type_child_id;
        public int param_int;
        public int param1;
        public int param2;
        public int param3;
        public string param_string;
    }

    public class t_ui_label
    {
        public string id;
        public string[] lang;
    }

    public class t_region
    {
        public int id;
        public string icon;
    }
}
