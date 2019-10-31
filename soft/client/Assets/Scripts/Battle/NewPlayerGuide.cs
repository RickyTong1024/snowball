using BattleDB;
using protocol.game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class guid_step_info
{
    public int type;
    public int x;
    public int y;
    public GameObject obj;
    public BattleAnimalPlayer pt;
    public battle_item_base item;
}

public class guid_effect_info
{
    public int state;
    public GameObject obj;
    public Transform objt;
    public int objid;
    public string name;
}

public class guide_obj
{
    public int state;
    public UILabel guide_label;
    public GameObject obj;
    public int type;
    public Transform objt;
    public int objid;
}

public class NewPlayerGuide
{
    public class OldState
    {
        public int level;
        public int talent_point;
    }

    //重置技能按钮
    private static int last_zhen;
    //隐藏按钮
    public static Dictionary<int, GameObject> current_step_hide_ = new Dictionary<int, GameObject>();
    //当前大步骤下需要加载的资源
    public static Dictionary<int, guid_step_info> current_guide_equips = new Dictionary<int, guid_step_info>();
    //当前的指引的步骤
    public static guide_step current_guide;
    //当前引导所需的数据
    private static Dictionary<int, guide_step> guide_steps = new Dictionary<int, guide_step>();
    //当期条件是否完成
    private static Dictionary<int, bool> current_step_complete_ = new Dictionary<int, bool>();
    //当前步骤所对应的特效 int=对应的t_guide_init.id  string对应特效的名称
    public static Dictionary<int, Dictionary<string, List<guid_effect_info>>> current_step_effect_ = new Dictionary<int, Dictionary<string, List<guid_effect_info>>>();
    //当前引导是否结束
    public static bool is_end;

    //------------------------------------------------------------------------------------------------------
    public static bobjpool guidepool = null;
    public static Dictionary<string, List<guid_effect_info>> guide_effects_type_ = new Dictionary<string, List<guid_effect_info>>();
    
    public static Dictionary<int, int> guide_dic = new Dictionary<int, int>();
    
    public static Dictionary<int, guide_obj> current_step_guides_obj_ = new Dictionary<int, guide_obj>();
    
    
    public static double wait_time;
    public static bool skip_fini_fight;

    public static string limit_btns;
    public static OldState old_state;
    private static bool IsWait = false;
    private static bool IsReStart = false;
    private static bool sk_relation = false;

    public static void OnInit()
    {
        NewPlayerGuide.guidepool = new bobjpool();
        current_guide_equips.Clear();
        guide_effects_type_.Clear();
        current_step_hide_.Clear();
        guide_dic.Clear();
        current_step_guides_obj_.Clear();
        current_step_complete_.Clear();
        current_step_effect_.Clear();
        old_state = new OldState();
        is_end = false;
        wait_time = 0;
        skip_fini_fight = false;
        current_guide = null;
        limit_btns = "";
        last_zhen = Int32.MinValue;
        IsWait = false;
        sk_relation = false;
        extraList.Clear();
        InitGuideStepData();
       
    }
    
    public static void InitGuideStepData()
    {
        guide_steps = new Dictionary<int, guide_step>();
        List<List<t_guide>> guideList = new List<List<t_guide>>();
        for (int i = 0; i < Config.max_guide_step; i++)
            // 开辟出 有多少个 大步骤
            guideList.Add(new List<t_guide>());
        // 按照id 从小到大排序 出 t_guide table
        var guides = from obj in Config.t_guides orderby obj.Value.id select obj.Value;
        foreach (var item in guides)
            guideList[item.step_id - 1].Add(item);

        for (int i = 0; i < guideList.Count; i++)
        {
            for (int j = 0; j < guideList[i].Count; j++)
            {
                if (!guide_steps.ContainsKey(i + 1))
                {
                    guide_steps.Add(i + 1, new guide_step());
                    guide_steps[i + 1].guide_id = i + 1;
                    guide_steps[i + 1].steps = new Dictionary<int, guide_step_info>();
                }
                // 这个是每个item (t_guide 中的item) 的信息
                var d = guideList[i][j];
                if (!guide_steps[i + 1].steps.ContainsKey(d.step_child_id))
                    guide_steps[i + 1].steps[d.step_child_id] = new guide_step_info();
                guide_steps[i + 1].steps[d.step_child_id].inits = Config.get_t_guide_init(d.id);
                var cds = Config.get_t_guide_condition(d.id);
                if (cds == null)
                    guide_steps[i + 1].steps[d.step_child_id].conditions = new List<t_guide_condition>();
                else
                    guide_steps[i + 1].steps[d.step_child_id].conditions = cds;
            }
        }
    }
    public static void Fini()
    {
        foreach (var item in current_guide_equips)
        {
            if (item.Value != null && item.Value.type == 3)
                GameObject.Destroy(item.Value.obj);
        }

        current_guide_equips.Clear();

        foreach (var item in guide_effects_type_)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                GameObject.Destroy(item.Value[i].obj);
                NewPlayerGuide.guidepool.remove(item.Value[i].objid);
            }
        }
        NewPlayerGuide.guidepool.clear();
    }
    // 新手引导的 入口
    public static void UpdateGuide()
    {
        //Debug.Log("player pos.x =" + BattlePlayers.me.player.x + "player pos.y = " + BattlePlayers.me.player.y);
        if (NewPlayerGuide.is_end)
            return;
        if (current_guide == null)
        {
            current_guide = NewPlayerGuide.get_nearest_guide();
            if (current_guide == null)
            {
                NewPlayerGuide.CMSG_GUIDE();
                NewPlayerGuide.is_end = true;
                return;
            }
            guide_dic[current_guide.guide_id] = 1;
            NewPlayerGuide.InitGuide(current_guide);
        }

        if (wait_time > 0)
        {
            wait_time = wait_time - Time.deltaTime / Time.timeScale;
            return;
        }
        else
            wait_time = 0;

        if (guide_dic[current_guide.guide_id] != -1)
            NewPlayerGuide.check_guide();
        else
            current_guide = null;
    }

    public static guide_step get_nearest_guide()
    {
        foreach (var item in guide_steps)
        {
            if (!guide_dic.ContainsKey(item.Key))
                return item.Value;
        }
        return null;
    }

    public static void InitGuide(guide_step t_guide)
    {
        if (BattlePlayers.me == null || t_guide == null)
            return;
        current_guide_equips.Clear();
        Dictionary<string, bool> robots = new Dictionary<string, bool>();
        List<t_preguide> preguides = Config.get_t_preguide(t_guide.guide_id);
        if (preguides != null)
        {
            for (int i = 0; i < preguides.Count; i++)
            {              
                //1 = 物品id  2=机器人  3=指示箭头
                t_preguide pg = preguides[i];
                //Debug.Log(" the preGuides id is :" + pg.guide_id);
                if (pg.type == 1)
                {
                    battle_item item = null;
                    if (pg.param_int == 0)
                        item = BattlePlayers.add_item(pg.param_int, pg.offset_x, pg.offset_y, pg.param1);
                    else
                        item = BattlePlayers.add_item(pg.param_int, pg.offset_x, pg.offset_y);

                    if (!current_guide_equips.ContainsKey(pg.id))
                        current_guide_equips.Add(pg.id, new guid_step_info() { type = pg.type, x = pg.offset_x, y = pg.offset_y, obj = item.obj, item = item.item });
                    else
                        current_guide_equips[pg.id] = new guid_step_info() { type = pg.type, x = pg.offset_x, y = pg.offset_y, obj = item.obj, item = item.item };
                }
                else if (pg.type == 2)
                {
                    int x = pg.offset_x;
                    int y = pg.offset_y;
                    var players = from obj in BattlePlayers.players orderby obj.Key select obj;
                    foreach (KeyValuePair<string, BattleAnimal> item in players)
                    {
                        if (!(item.Value is BattleAnimalPlayer))
                            continue;
                        var bp = item.Value as BattleAnimalPlayer;
                        bool sign = true;
                        if (bp.animal.death_time != 0 && (BattlePlayers.zhen - bp.animal.death_time) * BattlePlayers.TICK < 5000 || bp.is_die)
                            sign = false;
                        if (bp.player.is_ai > 0 && sign && !robots.ContainsKey(bp.animal.guid))
                        {
                            bp.animal.x = x;
                            bp.animal.y = y;
                            bp.attack_state = null;
                            BattleGrid.add(grid_type.et_player, bp.animal.guid, bp.animal.x, bp.animal.y);
                            if (current_guide_equips.ContainsKey(pg.id))
                                current_guide_equips[pg.id] = new guid_step_info() { type = pg.type, x = x, y = y, obj = bp.obj, pt = bp };
                            else
                                current_guide_equips.Add(pg.id, new guid_step_info() { type = pg.type, x = x, y = y, obj = bp.obj, pt = bp });
                            robots.Add(bp.animal.guid, true);
                            for (int nn = 0; nn < bp.attack_num.Length; nn++)
                                bp.attack_num[nn] = 0;
                            bp.mianyi = pg.param_int;
                            break;
                        }
                    }
                    
                }
                else if (pg.type == 3)
                {
                    int x = pg.offset_x;
                    int y = pg.offset_y;
                    GameObject obj = LuaHelper.GetResManager().CreateEffect("arrow_direction01");
                    obj.transform.parent = LuaHelper.GetResManager().UnitRoot;
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localPosition = new Vector3(x * 1.0f / Battle.BL, 0, y * 1.0f / Battle.BL);
                    if (current_guide_equips.ContainsKey(pg.id))
                        current_guide_equips[pg.id] = new guid_step_info() { type = pg.type, x = x, y = y, obj = obj };
                    else
                        current_guide_equips.Add(pg.id, new guid_step_info() { type = pg.type, x = x, y = y, obj = obj });
                }
                else if (pg.type == 4)  //场景中的物品
                {
                    BattlePlayers.addSpecBossUnit(pg.param_int);
                    BattleAnimalBoss bab = BattlePlayers.Boss;
                    if (current_guide_equips.ContainsKey(pg.id))
                        current_guide_equips[pg.id] = new guid_step_info() { type = pg.type, x = bab.player.x, y = bab.player.y, obj = bab.posobj };
                    else
                        current_guide_equips.Add(pg.id, new guid_step_info() { type = pg.type, x = bab.player.x, y = bab.player.y, obj = bab.posobj });
                }
            }
        }
        NewPlayerGuide.init_current_step_guide();
    }
    private static void InitStepGuide(t_guide_init gd)
    {
        if (gd.type == 1)  //显示下提示框 
        {
            string path = Config.get_t_guide_btn_path(gd.param_int);
            GameObject obj = Battle.battle_panel.get_obj_by_path(path);
            Battle.battle_panel.set_guide(obj, gd);      
        }
        else if (gd.type == 9)  //左边提示款type = 9
        {
            string path = Config.get_t_guide_btn_path(gd.param_int);
            GameObject obj = Battle.battle_panel.get_obj_by_path(path);
            Battle.battle_panel.set_guide(obj, gd, 1);
        }
        else if (gd.type == 2)  //路径 指引
        {
            var dt = current_guide_equips[gd.param_int];
            NewPlayerGuide.draw_current_step_effect(gd, dt.x, dt.y);
        }
        else if (gd.type == 3)   //3d特效
        {
            var dt = current_guide_equips[gd.param_int];
            if (dt.type == 1)
            {
                float x = dt.obj.transform.position.x;
                float y = dt.obj.transform.position.y;
                float z = dt.obj.transform.position.z;
                Battle.battle_panel.set_guide_3d(x, y, z, gd);
            }
        }
        else if (gd.type == 4)   //Ai指令
        {
            var robot = current_guide_equips[gd.param_int].pt;
            BattlePlayerAI.set_ai_attribute(robot, gd.param1, gd.param2);
        }
        else if (gd.type == 5)  //等待时间
            wait_time = gd.param_int;
        else if (gd.type == 6)  //指引提示语
        {
            if (!String.IsNullOrEmpty(gd.param_string))
                Battle.battle_panel.NewGuidePlaySound(gd.param_string);
        }
        else if (gd.type == 7) //停止人物所有操作
        {
            Battle.ch_pause = true;
            Battle.send_stop(BattlePlayers.me.animal.r);
        }
        else if (gd.type == 8) //界面显示提示语 遮罩
        {
            string temp = Config.get_t_lang(gd.param_string);       
            Battle.battle_panel.show_newplay_mask(temp, gd.param_int);                      
        }
        else if (gd.type == 10)  //隐藏按钮
        {
            string[] hides = gd.param_string.Split('|');
            HashSet<int> hide_ints = new HashSet<int>();  //当前要隐藏的按钮
            for (int m = 0; m < hides.Length; m++)
            {
                if (!String.IsNullOrEmpty(hides[m]))
                    hide_ints.Add(Convert.ToInt32(hides[m]));
            }

            List<int> dels = new List<int>();
            foreach (var item in current_step_hide_)
            {
                if (!hide_ints.Contains(item.Key))
                    dels.Add(item.Key);
            }           
            foreach (var item in dels)
            {
                if (!current_step_hide_[item].activeInHierarchy)
                    current_step_hide_[item].gameObject.SetActive(true);
                current_step_hide_.Remove(item);
            }

            foreach (var item in hide_ints)
            {
                if (!current_step_hide_.ContainsKey(item))
                {
                    string path = Config.get_t_guide_btn_path(item);
                    GameObject obj = Battle.battle_panel.get_obj_by_path(path);
                    if (obj.activeInHierarchy)
                        obj.SetActive(false);
                    current_step_hide_.Add(item, obj);
                }
            }
        }
        else if (gd.type == 11)  //点击取消
        {
            Battle.battle_panel.EnableButton(gd.param_string);
        }
        else if (gd.type == 12) //滑轮取消滑动(技能按钮的遮罩)
        {
            Battle.battle_panel.SetWheelStatus(gd.param_int);
        }
        else if (gd.type == 13)
        {
            GameObject obj = Battle.battle_panel.min_pro_map_[self.guid].obj;
            Battle.battle_panel.set_guide(obj, gd);
        }
        else if (gd.type == 14)
        {
            //指引导向场景中的某个物体
            battle_item wings = null;
            foreach (var item in BattlePlayers.items)
            {
                if (item.Value.item.id == 301)
                {
                    wings = item.Value;
                    break;
                }
            }
            NewPlayerGuide.draw_current_step_effect(gd, (long)wings.xx, (long)wings.yy);
        }
        else if (gd.type == 15)
        {
            BattlePlayers.me.player.cost = gd.param_int * BattlePlayers.POWERUP;
        }
        else if (gd.type == 16) // 重置玩家的位置
        {
            BattlePlayers.me.player.x = gd.offset_x;
            BattlePlayers.me.player.y = gd.offset_y;
            BattlePlayers.me.player.birth_x = gd.offset_x;
            BattlePlayers.me.player.birth_y = gd.offset_y;
        }
        else if(gd.type == 17) // 显示rank等的信息
        {
            Battle.battle_panel.ShowRankPanel();
        }
    }
    public static void init_current_step_guide()
    {
        var step_id = guide_dic[current_guide.guide_id];
        if (!current_guide.steps.ContainsKey(step_id))
        {
            guide_dic[current_guide.guide_id] = -1;  //-1 == true
            current_guide = null;
            return;
        }

        wait_time = 0;        
        for (int i = 0; i < current_guide.steps[step_id].inits.Count; i++)
        {
            var gd = current_guide.steps[step_id].inits[i];
            InitStepGuide(gd);
        }
    }

    private static List<t_guide_condition> extraList = new List<t_guide_condition>();
    private static void IsComplete(t_guide_condition gd,ref bool allcomplete,ref bool ReBackStep)
    {
        if (gd.type == 1)  //人物相关
        {
            if (gd.type_child_id == 1) //移动到某个位置
            {
                int x = 0, y = 0;
                if (gd.param_int > 0)
                {
                    int dtime = (BattlePlayers.zhen - last_zhen) * BattlePlayers.TICK;
                    float t = (gd.param_int * 1000 - dtime) / 1000.0f;
                    if (t < 0)
                        t = 0;
                    Battle.battle_panel.DrawCalTime(Mathf.CeilToInt(t));
                    if (dtime >= gd.param_int * 1000)
                    {
                        ReBackStep = true;                      
                        current_step_complete_.Add(gd.id, true);
                    }
                    else
                    {
                        var dt = current_guide_equips[gd.param1];
                        x = dt.x;
                        y = dt.y;
                        int dis = 5000;
                        if (gd.param2 > 0)
                            dis = gd.param2;
                        if (BattleOperation.check_distance(BattlePlayers.me.player.x, BattlePlayers.me.player.y, x, y, dis))
                        {
                            allcomplete = false;
                        }
                        else
                        {
                            current_step_complete_.Add(gd.id, true);
                        }
                            
                    }
                }
                else
                {
                    var dt = current_guide_equips[gd.param1];
                    x = dt.x;
                    y = dt.y;
                    int dis = 5000;
                    if (gd.param2 > 0)
                        dis = gd.param2;
                    if (BattleOperation.check_distance(BattlePlayers.me.player.x, BattlePlayers.me.player.y, x, y, dis))
                        allcomplete = false;
                    else
                    {
                        current_step_complete_.Add(gd.id, true);
                    }
                       
                }
            }
            else if (gd.type_child_id == 2)
            {
                var dt = current_guide_equips[gd.param_int];
                if (!BattleOperation.check_distance(dt.pt.player.x, dt.pt.player.y, dt.pt.ori_x, dt.pt.ori_y, Convert.ToInt32(gd.param3)))
                    allcomplete = false;
                else
                {
                    current_step_complete_.Add(gd.id, true);
                    BattlePlayerAI.set_ai_attribute(dt.pt, 0, 0);
                }
            }
        }
        else if (gd.type == 2)  //机器人身上的计数器
        {
            int robot_id = gd.param_int;
            int state = current_guide_equips[robot_id].pt.attack_state.GetValueOrDefault();
            if (gd.type_child_id == 1)
            {
                if (state == 1)
                {
                    if (current_step_complete_.ContainsKey(gd.id))
                        current_step_complete_[gd.id] = true;
                    else
                        current_step_complete_.Add(gd.id, true);
                }
                else
                    allcomplete = false;
            }
            else if (gd.type_child_id == 2)    //蓄力攻击
            {
                int at_num = current_guide_equips[robot_id].pt.attack_num[1];
                Battle.battle_panel.DrawCalSum(at_num, gd.param1);
                if (at_num >= gd.param1)
                {
                    if (current_step_complete_.ContainsKey(gd.id))
                        current_step_complete_[gd.id] = true;
                    else
                        current_step_complete_.Add(gd.id, true);
                }
                else
                    allcomplete = false;
            }
            else if (gd.type_child_id == 3)  //技能的释放次数
            {
                int at_num = current_guide_equips[robot_id].pt.attack_num[2];
                Battle.battle_panel.DrawCalSum(at_num, gd.param1);
                if (at_num >= gd.param1)
                {
                    if (current_step_complete_.ContainsKey(gd.id))
                        current_step_complete_[gd.id] = true;
                    else
                        current_step_complete_.Add(gd.id, true);
                }
                else
                    allcomplete = false;

                if (!allcomplete)
                {
                    if (BattlePlayers.me.player.skill_id == 0)
                    {
                        if (!sk_relation)
                        {
                            sk_relation = true;
                            last_zhen = BattlePlayers.zhen;
                        }
                        else
                        {
                            long t = (BattlePlayers.zhen - last_zhen) * BattlePlayers.TICK;
                            if (t > 4500)
                            {
                                BattlePlayers.me.player.skill_id = gd.param2;
                                BattlePlayers.me.player.skill_level = 1;
                                BattlePlayers.me.player.cost = BattlePlayers.POWERUP * gd.param3;
                                sk_relation = false;
                            }
                        }
                    }
                }
            }
            else if (gd.type_child_id == 4)
            {
                if (state == 1 || state == 2)
                {
                    if (current_step_complete_.ContainsKey(gd.id))
                        current_step_complete_[gd.id] = true;
                    else
                        current_step_complete_.Add(gd.id, true);
                }
                else
                    allcomplete = false;
            }
            else if (gd.type_child_id == 5)
            {
                if (state == 3)
                {
                    if (current_step_complete_.ContainsKey(gd.id))
                        current_step_complete_[gd.id] = true;
                    else
                        current_step_complete_.Add(gd.id, true);
                }
                else
                    allcomplete = false;
            }
            else if (gd.type_child_id == 6) //机器人被攻击次数
            {                
                int at_num = current_guide_equips[robot_id].pt.attack_num[0] + current_guide_equips[robot_id].pt.attack_num[1];
                Battle.battle_panel.DrawCalSum(at_num, gd.param1);
                if (at_num >= gd.param1)
                {
                    if (current_step_complete_.ContainsKey(gd.id))
                        current_step_complete_[gd.id] = true;
                    else
                        current_step_complete_.Add(gd.id, true);
                }
                else
                    allcomplete = false;
            }
        }
        else if (gd.type == 3)
        {
            var bp = BattlePlayers.me;
            if (gd.type_child_id == 1) //获得技能
            {
                int skill_id = gd.param_int;
                if (bp.player.skill_id == skill_id)
                {
                    if (current_step_complete_.ContainsKey(gd.id))
                        current_step_complete_[gd.id] = true;
                    else
                        current_step_complete_.Add(gd.id, true);
                }
                else
                    allcomplete = false;
            }
            else if (gd.type_child_id == 3)
            {
                if (bp.player.cost >= BattlePlayers.POWERUP * gd.param_int)
                {
                    if (current_step_complete_.ContainsKey(gd.id))
                        current_step_complete_[gd.id] = true;
                    else
                        current_step_complete_.Add(gd.id, true);
                }
                else
                    allcomplete = false;
            }
            else if (gd.type_child_id == 4)
            {
                if (bp.player.level > old_state.level)
                {
                    if (current_step_complete_.ContainsKey(gd.id))
                        current_step_complete_[gd.id] = true;
                    else
                        current_step_complete_.Add(gd.id, true);

                    if (gd.param_int > 0)
                    {
                        t_guide_condition gd_ex = new t_guide_condition()
                        {
                            id = gd.id,
                            guide_id = gd.guide_id,
                            type = gd.param_int,
                            param_string = gd.param_string
                        };
                        extraList.Add(gd_ex);

                        //初始化 引导 数据
                        var gd_init_ex = Config.get_t_guide_init(gd.param1);
                        foreach (var init in gd_init_ex)
                        {
                            init.id = gd.id;
                            init.guide_id = gd.guide_id;
                            InitStepGuide(init);
                        }                
                     }
                }
                else
                    allcomplete = false;
            }
            else if (gd.type_child_id == 5)
            {
                if (bp.player.skill_level == 3)
                {
                    if (current_step_complete_.ContainsKey(gd.id))
                        current_step_complete_[gd.id] = true;
                    else
                        current_step_complete_.Add(gd.id, true);
                }
                else
                    allcomplete = false;
            }
            else if (gd.type_child_id == 6)
            {
                var talent_num = BattlePlayers.me.player.talent_id.Count;
                if (talent_num > old_state.talent_point)
                {
                    if (current_step_complete_.ContainsKey(gd.id))
                        current_step_complete_[gd.id] = true;
                    else
                        current_step_complete_.Add(gd.id, true);
                }
                else
                    allcomplete = false;
            }
            else if (gd.type_child_id == 7)
            {
                if (bp.player.cost <= BattlePlayers.POWERUP * gd.param_int)
                {
                    if (current_step_complete_.ContainsKey(gd.id))
                        current_step_complete_[gd.id] = true;
                    else
                        current_step_complete_.Add(gd.id, true);
                }
                else
                    allcomplete = false;
            }
        }
        else if (gd.type == 4)
        {
            string btn_name = Battle.battle_panel.current_click_name();
            int re = gd.param_string.IndexOf(btn_name);
            if (re != -1 && !String.IsNullOrEmpty(btn_name))
            {
                if (current_step_complete_.ContainsKey(gd.id))
                    current_step_complete_[gd.id] = true;
                else
                    current_step_complete_.Add(gd.id, true);
            }
            else
                allcomplete = false;
        }
        else if (gd.type == 5)
        {
            if (gd.type_child_id == 1)
            {
                int blood_pert = BattleOperation.toInt(BattlePlayers.me.player.hp * 100 / BattlePlayers.me.attr.max_hp());
                if (blood_pert <= Convert.ToInt32(gd.param3))
                {
                    if (current_step_complete_.ContainsKey(gd.id))
                        current_step_complete_[gd.id] = true;
                    else
                        current_step_complete_.Add(gd.id, true);

                    var dt = current_guide_equips[gd.param_int];
                    if (dt != null)
                        BattlePlayerAI.set_ai_attribute(dt.pt, 0, 0);
                }
                else
                    allcomplete = false;
            }
            else if (gd.type_child_id == 2)
            {
                bool state = Battle.battle_panel.audiosource_.isPlaying;
                if (!state)
                {
                    if (current_step_complete_.ContainsKey(gd.id))
                        current_step_complete_[gd.id] = true;
                    else
                        current_step_complete_.Add(gd.id, true);
                }
                else
                    allcomplete = false;
            }
            else if (gd.type_child_id == 3)
            {
                if (current_step_complete_.ContainsKey(gd.id))
                    current_step_complete_[gd.id] = true;
                else
                    current_step_complete_.Add(gd.id, true);
            }
            else if (gd.type_child_id == 4)
            {
                if (gd.param_int == 0)
                {
                    if (!BattlePlayers.me.player.is_xueren)
                    {
                        if (current_step_complete_.ContainsKey(gd.id))
                            current_step_complete_[gd.id] = true;
                        else
                            current_step_complete_.Add(gd.id, true);
                    }
                    else
                        allcomplete = false;
                }
                else if (gd.param_int == 1)
                {
                    if (BattlePlayers.me.player.is_xueren)
                    {
                        if (current_step_complete_.ContainsKey(gd.id))
                            current_step_complete_[gd.id] = true;
                        else
                            current_step_complete_.Add(gd.id, true);
                    }
                    else
                        allcomplete = false;
                }
            }
        }
        else if (gd.type == 6)  //处理按钮点击完成或者播放完毕 都算作完成
        {
            bool state = Battle.battle_panel.audiosource_.isPlaying;
            string btn_name = Battle.battle_panel.current_click_name();
            int re = gd.param_string.IndexOf(btn_name);
            if (re != -1 && !String.IsNullOrEmpty(btn_name))
            {
                if (current_step_complete_.ContainsKey(gd.id))
                    current_step_complete_[gd.id] = true;
                else
                    current_step_complete_.Add(gd.id, true);
            }
            else if (!state)
            {
                if (current_step_complete_.ContainsKey(gd.id))
                    current_step_complete_[gd.id] = true;
                else
                    current_step_complete_.Add(gd.id, true);
            }
            else
                allcomplete = false;
        }
        else if (gd.type == 7) // 这个是 判断玩家 是否死亡的type
        {
            int dtime = (BattlePlayers.zhen - last_zhen) * BattlePlayers.TICK;
            float t = (gd.param_int * 1000 - dtime) / 1000.0f;
            if (t < 0)
                t = 0;
            int tt = Mathf.CeilToInt(t);
            Battle.battle_panel.DrawCalTime(tt);
          
            if (dtime >= gd.param_int * 1000)
            {
                if (current_step_complete_.ContainsKey(gd.id))
                    current_step_complete_[gd.id] = true;
                else                
                    current_step_complete_.Add(gd.id, true);                                    
            }
            else if (BattlePlayers.me.player.hp <= 0)
            {
                ReBackStep = true;
                current_step_complete_.Add(gd.id, true);
            }
            else
                allcomplete = false;
        }
        else if (gd.type == 8)  //进入视野
        {
            if (BattlePlayers.Boss == null || BattlePlayers.Boss.is_die)
            {
                ReBackStep = true;
                current_step_complete_.Add(gd.id, true);
            }
            else
            {
                if (BattlePlayers.Boss.unit.CheckScreenVis())
                {
                    if (current_step_complete_.ContainsKey(gd.id))
                        current_step_complete_[gd.id] = true;
                    else
                        current_step_complete_.Add(gd.id, true);
                }
                else
                    allcomplete = false;
            }
        }
        else if (gd.type == 9)
        {
            if (BattlePlayers.Boss.is_die)
            {
                if (current_step_complete_.ContainsKey(gd.id))
                    current_step_complete_[gd.id] = true;
                else
                    current_step_complete_.Add(gd.id, true);
            }
            else
                allcomplete = false;
        }
        else if (gd.type == 10)
        {
            bool sign = false;
            for (int mm = 0; mm < BattlePlayers.me.animal.buffs.Count; mm++)
            {
                if (BattlePlayers.me.animal.buffs[mm] == gd.param_int)
                {
                    sign = true;
                    break;
                }
            }

            if (sign)
            {
                if (current_step_complete_.ContainsKey(gd.id))
                    current_step_complete_[gd.id] = true;
                else
                    current_step_complete_.Add(gd.id, true);
            }
            else
                allcomplete = false;
        }
    }

    public static void check_guide()
    {
        var step_id = guide_dic[current_guide.guide_id];
        if (!current_guide.steps.ContainsKey(step_id))
        {
            guide_dic[current_guide.guide_id] = -1;
            current_guide = null;
            return;
        }

        bool allcomplete = true;
        bool ReBackStep = false;

        for (int i = 0; i < current_guide.steps[step_id].conditions.Count; i++)
        {
            var gd = current_guide.steps[step_id].conditions[i];
            //current_step_complete_ 这个table中记录的是 小步奏完成的id (小)
            if (!current_step_complete_.ContainsKey(gd.id))
                IsComplete(gd, ref allcomplete, ref ReBackStep);
        }

        for (int i = 0; i < extraList.Count; i++)
        {
            var gd = extraList[i];   
            IsComplete(gd, ref allcomplete, ref ReBackStep);
        }

        NewPlayerGuide.UpdateGUI(step_id);   
 
        if (allcomplete)
        {
            if (!IsWait)
            {
                if (!ReBackStep)
                {
                    if (IsReStart)
                    {
                        NewPlayerGuide.RepeatStep();
                        IsReStart = false;
                    }
                    else
                        NewPlayerGuide.next_step();
                }     
                else
                {
                    IsWait = true;
                    Battle.battle_panel.ShowRestartTask(delegate ()
                    {
                        if (BattlePlayers.me.is_die)
                        {
                            BattlePlayers.Fuhuo(BattlePlayers.me);
                        }
                        IsWait = false;
                        IsReStart = true;
                    });
                }
            }
        }
    }

    public static guid_effect_info get_effect(string effect_name)
    {
        if (guide_effects_type_.ContainsKey(effect_name))
        {
            for (int i = 0; i < guide_effects_type_[effect_name].Count; i++)
            {
                if (guide_effects_type_[effect_name][i].state == 0)
                {
                    guide_effects_type_[effect_name][i].state = 1;
                    return guide_effects_type_[effect_name][i];
                }
            }
        }
        if (!guide_effects_type_.ContainsKey(effect_name))
            guide_effects_type_.Add(effect_name, new List<guid_effect_info>());

        var obj = LuaHelper.GetResManager().CreateEffect(effect_name);
        int objid = NewPlayerGuide.guidepool.add(obj);
        Transform objt = obj.transform;
        obj.transform.parent = LuaHelper.GetResManager().UnitRoot;
        guid_effect_info d = new guid_effect_info() { state = 1, obj = obj, objt = objt, objid = objid, name = effect_name };
        guide_effects_type_[effect_name].Add(d);        
        return d;
    }

    public static void draw_current_step_effect(t_guide_init t_guide_init,long x,long y)
    {
        if (!current_step_effect_.ContainsKey(t_guide_init.id))
        {
            current_step_effect_.Add(t_guide_init.id, new Dictionary<string, List<guid_effect_info>>());
           
            current_step_effect_[t_guide_init.id].Add("arrow_direction02", new List<guid_effect_info>());
            long len = 10000;
            long ch_x = BattleOperation.toInt(BattlePlayers.me.posobjt.localPosition.x * 10000.0);
            long ch_y = BattleOperation.toInt(BattlePlayers.me.posobjt.localPosition.z * 10000.0);
            int r = BattleOperation.get_r(ch_x, ch_y, x, y);
            long maxLen = (long)Math.Sqrt((x - ch_x) * (x - ch_x) + (y - ch_y) * (y - ch_y));
            int[] p;
            while (true)
            {
                if (maxLen >= len)
                {
                    ch_x = BattleOperation.toInt(BattlePlayers.me.posobjt.localPosition.x * 10000);
                    ch_y = BattleOperation.toInt(BattlePlayers.me.posobjt.localPosition.z * 10000);
                    p = BattleOperation.add_distance2(ch_x, ch_y, r, len);
                }
                else
                    break;

                guid_effect_info obt = NewPlayerGuide.get_effect("arrow_direction02");
                obt.obj.SetActive(true);
                NewPlayerGuide.guidepool.set_localPosition(obt.objid, p[0] * 1.0f / Battle.BL, 0, p[1] * 1.0f / Battle.BL);
                NewPlayerGuide.guidepool.set_localScale(obt.objid, 0.5f, 1, 0.5f);
                NewPlayerGuide.guidepool.set_localEulerAngles(obt.objid, 0, 90 - r + 180, 0);
                len = len + 10000;
                current_step_effect_[t_guide_init.id]["arrow_direction02"].Add(obt);
            }
        }
        else
        {
            long len = 10000;
            long ch_x = BattleOperation.toInt(BattlePlayers.me.posobjt.localPosition.x * 10000.0);
            long ch_y = BattleOperation.toInt(BattlePlayers.me.posobjt.localPosition.z * 10000.0);
            int r = BattleOperation.get_r(ch_x, ch_y, x, y);
            long maxLen = (long)Math.Sqrt((x - ch_x) * (x - ch_x) + (y - ch_y) * (y - ch_y));
            List<int[]> point_t = new List<int[]>();
            int count = 0;
            while (true)
            {
                if (maxLen >= len)
                {
                    ch_x = BattleOperation.toInt(BattlePlayers.me.posobjt.localPosition.x * 10000.0);
                    ch_y = BattleOperation.toInt(BattlePlayers.me.posobjt.localPosition.z * 10000.0);
                    ch_y = BattleOperation.toInt(BattlePlayers.me.posobjt.localPosition.z * 10000.0);
                    var p = BattleOperation.add_distance2(ch_x, ch_y, r, len);
                    point_t.Add(p);
                    len = len + 10000;
                }
                else
                    break;
            }
            
            if (point_t.Count > current_step_effect_[t_guide_init.id]["arrow_direction02"].Count)
            {
                for (int i = 0; i < (point_t.Count - current_step_effect_[t_guide_init.id]["arrow_direction02"].Count); i++)
                {
                    var obt = NewPlayerGuide.get_effect("arrow_direction02");
                    current_step_effect_[t_guide_init.id]["arrow_direction02"].Add(obt);
                }
            }

            for (int i = 0; i < current_step_effect_[t_guide_init.id]["arrow_direction02"].Count; i++)
            {
                if (i < point_t.Count)
                {
                    var ct = current_step_effect_[t_guide_init.id]["arrow_direction02"][i];
                    var p = point_t[i];
                    current_step_effect_[t_guide_init.id]["arrow_direction02"][i].obj.SetActive(true);
                    NewPlayerGuide.guidepool.set_localPosition(ct.objid, p[0] * 1.0f / Battle.BL, 0, p[1] * 1.0f / Battle.BL);
                    NewPlayerGuide.guidepool.set_localScale(ct.objid, 0.5f, 1, 0.5f);
                    NewPlayerGuide.guidepool.set_localEulerAngles(ct.objid, 0, 90 - r + 180, 0);
                }
                else
                    current_step_effect_[t_guide_init.id]["arrow_direction02"][i].obj.SetActive(false);
            }
        }
    }

    public static void UpdateGUI(int step_id)
    {
        for (int i = 0; i < current_guide.steps[step_id].inits.Count; i++)
        {
            var gd = current_guide.steps[step_id].inits[i];
            if (gd.type == 3)
            {
                if (current_step_complete_.ContainsKey(gd.id))
                {
                    if (current_step_guides_obj_.ContainsKey(gd.id))
                    {
                        current_step_guides_obj_[gd.id].state = 0;
                        current_step_guides_obj_[gd.id].obj.SetActive(false);
                        current_step_guides_obj_.Remove(gd.id);
                    }
                }
                else
                {
                    var dt = current_guide_equips[gd.param_int];
                    if (dt.type == 1 )
                    {
                        var tid = dt.item.tid;
                        if (!BattlePlayers.items.ContainsKey(tid))
                        {
                            if (current_step_guides_obj_.ContainsKey(gd.id))
                            {
                                current_step_guides_obj_[gd.id].state = 0;
                                current_step_guides_obj_[gd.id].obj.SetActive(false);
                                current_step_guides_obj_.Remove(gd.id);
                            }
                        }
                        else
                        {
                            float x = dt.obj.transform.position.x;
                            float y = dt.obj.transform.position.y;
                            float z = dt.obj.transform.position.z;
                            Battle.battle_panel.set_guide_3d(x, y, z, gd, true);
                        }
                    }
                }
            }
            else if (gd.type == 2)
            {
                var dt = current_guide_equips[gd.param_int];
                if (dt.type == 1 )
                {
                    var tid = dt.item.tid;
                    if (!BattlePlayers.items.ContainsKey(tid))
                    {
                        if (current_step_effect_[gd.id].ContainsKey("arrow_direction02"))
                        {
                            for (int j = 0; j < current_step_effect_[gd.id]["arrow_direction02"].Count; j++)
                            {
                                current_step_effect_[gd.id]["arrow_direction02"][j].state = 0;
                                current_step_effect_[gd.id]["arrow_direction02"][j].obj.SetActive(false);
                            }
                            current_step_effect_[gd.id].Remove("arrow_direction02");
                        }
                    }
                    else
                        NewPlayerGuide.draw_current_step_effect(gd, dt.x, dt.y);
                }
                else if (dt.type == 3 || dt.type == 4 || dt.type == 2)
                {
                    if (current_step_complete_.ContainsKey(gd.id))
                    {
                        for (int j = 0; j < current_step_effect_[gd.id]["arrow_direction02"].Count; j++)
                        {
                            current_step_effect_[gd.id]["arrow_direction02"][j].state = 0;
                            current_step_effect_[gd.id]["arrow_direction02"][j].obj.SetActive(false);
                        }
                        current_step_effect_[gd.id].Remove("arrow_direction02");
                    }
                    else
                        NewPlayerGuide.draw_current_step_effect(gd, dt.x, dt.y);
                }
            }
            else if (gd.type == 14)
            {
                if (current_step_complete_.ContainsKey(gd.id))
                {
                    for (int j = 0; j < current_step_effect_[gd.id]["arrow_direction02"].Count; j++)
                    {
                        current_step_effect_[gd.id]["arrow_direction02"][j].state = 0;
                        current_step_effect_[gd.id]["arrow_direction02"][j].obj.SetActive(false);
                    }
                    current_step_effect_[gd.id].Remove("arrow_direction02");
                }
                else
                {
                    battle_item wings = null;
                    foreach (var item in BattlePlayers.items)
                    {
                        if (item.Value.item.id == 301)
                        {
                            wings = item.Value;
                            break;
                        }
                    }

                    if (wings != null)
                    {
                        long xx = (long)(wings.xx * Battle.BL);
                        long yy = (long)(wings.yy * Battle.BL);
                        NewPlayerGuide.draw_current_step_effect(gd,xx,yy);
                    }
                }
            }
        }
    }

    public static void RepeatStep()
    {
        List<int> dels = new List<int>();
        foreach (var item in current_guide_equips)
        {
            if (item.Value.type == 1)
                dels.Add(item.Key);
            else if (item.Value.type == 2)
            {
                item.Value.pt.attack_state = null;
                item.Value.pt.mianyi = null;
                for (int nn = 0; nn < item.Value.pt.attack_num.Length; nn++)
                    item.Value.pt.attack_num[nn] = 0;
                item.Value.pt.player.x = 18000000;
                item.Value.pt.player.y = 18000000;
                BattleGrid.add(grid_type.et_player, item.Value.pt.player.guid, item.Value.pt.player.x, item.Value.pt.player.y);
                item.Value.pt.set_hp(item.Value.pt.attr.max_hp());
            }
            else if (item.Value.type == 4)  //Boss
            {
                BattlePlayers.Boss.animal.x = 18000000;
                BattlePlayers.Boss.animal.y = 18000000;
                BattlePlayers.Boss.set_hp(0);
                BattlePlayers.Boss.is_die = true;

                BattlePlayers.delplayer(BattlePlayers.Boss.player.guid);
                BattlePlayers.Boss = null;
            }
            else
                GameObject.Destroy(item.Value.obj);
        }
        if (dels.Count > 0)
        {
            for (int i = dels.Count - 1; i >= 0; i--)
                current_guide_equips.Remove(dels[i]);
        }
        current_guide_equips.Clear();
        for (int i = 0; i < current_guide.steps[guide_dic[current_guide.guide_id]].inits.Count; i++)
        {
            var gd = current_guide.steps[guide_dic[current_guide.guide_id]].inits[i];
            if (current_step_effect_.ContainsKey(gd.id))
            {
                if (current_step_effect_[gd.id].ContainsKey("arrow_direction02"))
                {
                    for (int j = 0; j < current_step_effect_[gd.id]["arrow_direction02"].Count; j++)
                    {
                        current_step_effect_[gd.id]["arrow_direction02"][j].state = 0;
                        current_step_effect_[gd.id]["arrow_direction02"][j].obj.SetActive(false);
                    }
                }
            }
        }

        current_step_effect_.Clear();
        foreach (var item in current_step_guides_obj_)
        {
            item.Value.state = 0;
            item.Value.obj.SetActive(false);
        }

        if (guide_dic.ContainsKey(current_guide.guide_id))
            guide_dic.Remove(current_guide.guide_id);

        current_step_guides_obj_.Clear();
        current_guide = null;
        Battle.battle_panel.hide_newplay_mask();

        old_state.level = BattlePlayers.me.player.level;
        old_state.talent_point = BattlePlayers.me.player.talent_id.Count;

        current_step_complete_.Clear();
        Battle.battle_panel.ResetCurrentClick();
        last_zhen = BattlePlayers.zhen;
        IsWait = false;
        IsReStart = false;
        extraList.Clear();
        sk_relation = false;       
    }

    public static void next_step()
    {
        Battle.ch_pause = false;
        var next_step_id = guide_dic[current_guide.guide_id] + 1;      
        if (!current_guide.steps.ContainsKey(next_step_id))
        {
            List<int> dels = new List<int>();
            foreach (var item in current_guide_equips)
            {
                if (item.Value.type == 1)
                    dels.Add(item.Key);
                else if (item.Value.type == 2)
                {
                    item.Value.pt.attack_state = null;
                    item.Value.pt.mianyi = null;
                    for (int nn = 0; nn < item.Value.pt.attack_num.Length; nn++)
                        item.Value.pt.attack_num[nn] = 0;
                    item.Value.pt.player.x = 18000000;
                    item.Value.pt.player.y = 18000000;
                    BattleGrid.add(grid_type.et_player, item.Value.pt.player.guid, item.Value.pt.player.x, item.Value.pt.player.y);
                    item.Value.pt.set_hp(item.Value.pt.attr.max_hp());
                }
                else if (item.Value.type == 4)  //Boss
                {
                    BattlePlayers.Boss.animal.x = 18000000;
                    BattlePlayers.Boss.animal.y = 18000000;
                    BattlePlayers.Boss.set_hp(0);
                    BattlePlayers.Boss.is_die = true;

                    BattlePlayers.delplayer(BattlePlayers.Boss.player.guid);
                    BattlePlayers.Boss = null;
                }
                else
                    GameObject.Destroy(item.Value.obj);
            }
            if (dels.Count > 0)
            {
                for (int i = dels.Count - 1; i >= 0; i--)
                    current_guide_equips.Remove(dels[i]);
            }
            current_guide_equips.Clear();
            for (int i = 0; i < current_guide.steps[guide_dic[current_guide.guide_id]].inits.Count; i++)
            {
                var gd = current_guide.steps[guide_dic[current_guide.guide_id]].inits[i];
                if (current_step_effect_.ContainsKey(gd.id))
                {
                    if (current_step_effect_[gd.id].ContainsKey("arrow_direction02"))
                    {
                        for (int j = 0; j < current_step_effect_[gd.id]["arrow_direction02"].Count; j++)
                        {
                            current_step_effect_[gd.id]["arrow_direction02"][j].state = 0;
                            current_step_effect_[gd.id]["arrow_direction02"][j].obj.SetActive(false);
                        }
                    }
                }
            }
            
            current_step_effect_.Clear();
            foreach (var item in current_step_guides_obj_)
            {
                item.Value.state = 0;
                item.Value.obj.SetActive(false);
            }

            guide_dic[current_guide.guide_id] = -1;
            current_step_guides_obj_.Clear();
            current_guide = null;
            Battle.battle_panel.hide_newplay_mask();
            IsWait = false;
            IsReStart = false;
        }
        else
        {
            foreach (var item in current_guide_equips)
            {
                if (item.Value.type == 2)
                {
                    item.Value.pt.attack_state = null;
                    for(int nn = 0;nn < item.Value.pt.attack_num.Length;nn++)
                        item.Value.pt.attack_num[nn] = 0;
                    if(item.Value.pt.mianyi != null)
                        item.Value.pt.mianyi = 3;
                }
            }

            for (int i = 0; i < current_guide.steps[guide_dic[current_guide.guide_id]].inits.Count; i++)
            {
                var gd = current_guide.steps[guide_dic[current_guide.guide_id]].inits[i];
                if (current_step_effect_.ContainsKey(gd.id))
                {
                    if (current_step_effect_[gd.id].ContainsKey("arrow_direction02"))
                    {
                        for (int j = 0; j < current_step_effect_[gd.id]["arrow_direction02"].Count; j++)
                        {
                            current_step_effect_[gd.id]["arrow_direction02"][j].state = 0;
                            current_step_effect_[gd.id]["arrow_direction02"][j].obj.SetActive(false);
                        }
                    }
                }
            }

            current_step_effect_.Clear();
            if (guide_dic.ContainsKey(current_guide.guide_id))
                guide_dic[current_guide.guide_id] = next_step_id;
            else
                guide_dic.Add(current_guide.guide_id, next_step_id);

            foreach (var item in current_step_guides_obj_)
            {
                item.Value.state = 0;
                item.Value.obj.SetActive(false);
            }

            current_step_guides_obj_.Clear();
            Battle.battle_panel.hide_newplay_mask();
            BattlePlayers.Stop1(BattlePlayers.me, BattlePlayers.me.player.r);
            NewPlayerGuide.init_current_step_guide();
        }

        old_state.level = BattlePlayers.me.player.level;
        old_state.talent_point = BattlePlayers.me.player.talent_id.Count;

	    current_step_complete_.Clear();
        Battle.battle_panel.ResetCurrentClick();
        Battle.isPause = false;
        last_zhen = BattlePlayers.zhen;
        extraList.Clear();
        sk_relation = false;  
    }

    public static guide_obj get_current_step_obj(int id)
    {
        if (current_step_guides_obj_.ContainsKey(id))
            return current_step_guides_obj_[id];
        else
            return null; 
    }

    public static void set_current_step_obj(int id, guide_obj obj)
    {
        if (current_step_guides_obj_.ContainsKey(id))
            current_step_guides_obj_[id] = obj;
        else
            current_step_guides_obj_.Add(id, obj);
    }

    public static void CMSG_GUIDE(bool skip = false)
    {
        skip_fini_fight = skip;
        GameTcp.SendNull(opclient_t.CMSG_GUIDE);
    }

    public static void SMSG_GUIDE()
    {
        NewPlayerGuide.Fini();
        Battle.battle_panel.clear_new_guide();
        if (skip_fini_fight)
            Util.CallMethod("State", "ChangeState", 2);
        else
            BattlePlayers.InitOtherAIPos();
    }
}
