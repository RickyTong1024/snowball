using BattleDB;
using protocol.game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WaitForMinus : CustomYieldInstruction
{
    private float t_s;
    public WaitForMinus(float ss)
    {
        t_s = ss;
    }

    public WaitForMinus()
    {
        t_s = 1.0f;
    }
    public override bool keepWaiting
    {
        get
        {
            t_s -= Time.deltaTime / Time.timeScale;
            if (t_s <= 0)
            {
                t_s = 0;
                return false;
            }
            else
                return true;
        }
    }
}

public static class IconPanel
{
    private static Color[] vip_color = { new Color(241.0f / 255, 30.0f / 255, 241.0f / 255), new Color(255.0f / 255, 206.0f / 255, 1.0f / 255), new Color(194.0f / 255, 229.0f / 255, 237.0f / 255) };
    public static List<UIAtlas> atlasList = null;
    private static Dictionary<string, GameObject> icon_list_ = null;
    private static Dictionary<int, string> frame_color_ = new Dictionary<int, string> (){ { 1, "djframe-blue" }, { 2, "djframe-purple" }, { 3, "djframe-y" } };
    private static Dictionary<int, string> frame_color_two_ = new Dictionary<int, string>() { { 1, "djframe-blue_sp" }, { 2, "djframe-purple_sp" }, { 3, "djframe-y_sp" } };
    private static Dictionary<int, Color> quality_color = new Dictionary<int, Color>() { { 1, new Color(74.0f / 255, 204.0f / 255, 246.0f / 255) }, { 2, new Color(184.0f / 255, 100.0f / 255, 255.0f / 255) }, { 3, new Color(1, 192.0f / 255, 82.0f / 255) } };
    private static Dictionary<int, Color> effect_color = new Dictionary<int, Color>() { { 1, new Color(10.0f / 255, 91.0f / 255, 132.0f / 255) }, { 2, new Color(98.0f / 255, 13.0f / 255, 204.0f / 255) }, { 3, new Color(163.0f / 255, 112.0f / 255, 3.0f / 255) } };

    public static void Init()
    {
        icon_list_ = new Dictionary<string, GameObject>();
        atlasList = new List<UIAtlas>();
        GameObject ui_root = GameObject.FindWithTag("UIRoot");
        Transform panel = ui_root.transform.Find("IconPanel/altas_root");
        Transform iconPanel = ui_root.transform.Find("IconPanel");
        for (int i = 0; i < panel.transform.childCount; i++)
        {
            UIAtlas atlas = panel.transform.GetChild(i).GetComponent<UISprite>().atlas;
            atlasList.Add(atlas);
        }

        for (int i = 0; i < iconPanel.transform.childCount; i++)
            icon_list_.Add(iconPanel.transform.GetChild(i).name, iconPanel.transform.GetChild(i).gameObject);  
    }

    public static UIAtlas GetAltas(string icon)
    {
        for (int i = 0; i < atlasList.Count; i++)
        {
            if (atlasList[i].GetSprite(icon) != null)
                return atlasList[i];
        }
        return null;
    }
    public static GameObject GetCup(int id,bool is_show = false,bool is_big = false)
    {

        float star_radio = 6125;
        var star_y_offset = 40;
        var pos_ref = 40;
        GameObject cup_t = null;
        if (is_big)
        {
            star_y_offset = 80;
            pos_ref = 60;
            star_radio = Mathf.Pow(125, 2);
            cup_t = LuaHelper.Instantiate(icon_list_["cup_big_res"]);
        }
        else
            cup_t = LuaHelper.Instantiate(icon_list_["cup_res"]);

        var cup_temp = Config.get_t_cup(id);
        var star_res = cup_t.transform.Find("star_res");
        var icon = cup_t.transform.GetComponent<UISprite>();
        var name = cup_t.transform.Find("name").GetComponent<UILabel>();
        icon.atlas = IconPanel.GetAltas(cup_temp.icon);
        icon.spriteName = cup_temp.icon;
        name.text = cup_temp.name;
        cup_t.transform.Find("bg").GetComponent<UISprite>().spriteName = cup_temp.dai_icon;
        IconPanel.InitCupLabel(cup_temp.dai_icon, name);
        if (Config.get_t_cup(id + 1).id != (id + 1))
        {
            var star = LuaHelper.Instantiate(star_res.gameObject);
            star.transform.parent = cup_t.transform;
            star.transform.localScale = Vector3.one;
            star.transform.localPosition = new Vector3(0, Mathf.Sqrt(star_radio - Mathf.Pow(0, 2)) + star_y_offset, 0);
            star.SetActive(true);
            UILabel lv = null;
            if (is_big)
                lv = cup_t.transform.Find("lv/lv_view/Label").GetComponent<UILabel>();
            else
                lv = cup_t.transform.Find("lv/Label").GetComponent<UILabel>();
            lv.text = (id - cup_temp.id + cup_temp.star).ToString();
            IconPanel.InitCupLabel(cup_temp.dai_icon, lv);
            cup_t.transform.Find("lv").gameObject.SetActive(true);
        }
        else
        {
            float pos_per = 0;
            if (cup_temp.max_star < 5)
                pos_per = pos_ref;
            else
                pos_per = pos_ref - pos_ref / 8.0f;


            float fir_pos = -(cup_temp.max_star - 1) / 2.0f * pos_per;
            for (int k = 1; k <= cup_temp.max_star; k++)
            {
                var star = LuaHelper.Instantiate(star_res.gameObject);
                star.transform.parent = cup_t.transform;
                star.transform.localScale = Vector3.one;
                star.name = k.ToString();
                var sj_pos = (5 - cup_temp.max_star) + k;
                var x_pos = fir_pos + (k - 1) * pos_per;
                if ((cup_temp.max_star >= 5 && k % 2 == 0))
                    x_pos = (k - 3) * pos_ref;
                star.transform.localEulerAngles = new Vector3(0, 0, -x_pos / 2);
                var y_pos = Mathf.Sqrt(star_radio - Mathf.Pow(x_pos, 2)) + star_y_offset;
                star.transform.localPosition = new Vector3(x_pos, y_pos, 0);
                if (cup_temp.star < k && !is_show)
                    star.transform.GetComponent<UISprite>().spriteName = "star_bg01";
                star.SetActive(true);
            }
        }
        return cup_t;
    }
    public static void InitCupLabel(string icon_name,UILabel cup_label)
    {
        if (cup_label == null)
            return;


        cup_label.applyGradient = true;
        cup_label.gradientTop = new Color(1, 1, 1);


        if (icon_name == "jb011")
        {
            cup_label.gradientBottom = new Color(247 / 255, 137 / 255, 75 / 255);
            cup_label.effectColor = new Color(132 / 255, 66 / 255, 10 / 255);
        }
        else if (icon_name == "jb022")
        {
            cup_label.gradientBottom = new Color(194.0f / 255, 204.0f / 255, 210.0f / 255);
            cup_label.effectColor = new Color(94.0f / 255, 105.0f / 255, 114.0f / 255);
        }
        else if (icon_name == "jb033")
        {
            cup_label.gradientBottom = new Color(246.0f / 255, 191.0f / 255, 73.0f / 255);
            cup_label.effectColor = new Color(132.0f / 255, 104.0f / 255, 10.0f / 255);
        }
        else if (icon_name == "jb044")
        {
            cup_label.gradientBottom = new Color(73.0f / 255, 246.0f / 255, 214.0f / 255);
            cup_label.effectColor = new Color(10.0f / 255, 132.0f / 255, 120.0f / 255);
        }
        else if (icon_name == "jb055")
        {
            cup_label.gradientBottom = new Color(74.0f / 255, 204.0f / 255, 246.0f / 255);
            cup_label.effectColor = new Color(10.0f / 255, 91.0f / 255, 132.0f / 255);
        }
        else if (icon_name == "jb066")
        {
            cup_label.gradientBottom = new Color(255.0f / 255, 192.0f / 255, 82.0f / 255);
            cup_label.effectColor = new Color(163.0f / 255, 112.0f / 255, 3.0f / 255);
        }
        else if (icon_name == "jb077")
        {
            cup_label.gradientBottom = new Color(184.0f / 255, 100.0f / 255, 255.0f / 255);
            cup_label.effectColor = new Color(98.0f / 255, 13.0f / 255, 204.0f / 255);
        }
        else if (icon_name == "jb088")
        {
            cup_label.gradientBottom = new Color(255.0f / 255, 137.0f / 255, 105.0f / 255);
            cup_label.effectColor = new Color(204.0f / 255, 13.0f / 255, 32.0f / 255);
        }
    }
    public static void InitQualityLabel(UILabel name_label,int color)
    {
        if (name_label == null)
            return;
        name_label.applyGradient = true;
        name_label.gradientTop = new Color(1,1,1);
        name_label.gradientBottom = quality_color[color];
        name_label.effectStyle = UILabel.Effect.Outline8;
        name_label.effectColor = effect_color[color];
        name_label.gameObject.GetComponent<UIWidget>().color = new Color(1,1,1);
    }
    public static void InitVipLabel(UILabel label, int color)
    {
        if (label == null)
            return;

        label.effectStyle = UILabel.Effect.None;
        if (color > 0)
        {
            label.applyGradient = true;
            label.gradientTop = new Color(255.0f / 255, 255.0f / 255, 255.0f / 255);
            label.gradientBottom = vip_color[color - 1];
            label.gameObject.GetComponent<UIWidget>().color = new Color(255.0f / 255, 255.0f / 255, 255.0f / 255);
        }
        else
        {
            label.applyGradient = false;
            label.gameObject.GetComponent<UIWidget>().color = vip_color[2];
        }
    }

    public static GameObject GetIcon(string icon_name, string icon_, int frame, int? num = null)
    {
        if (icon_list_.ContainsKey(icon_name))
        {
            var icon_res = LuaHelper.Instantiate(icon_list_[icon_name]);
            var icon = icon_res.transform.Find("icon").GetComponent<UISprite>();
            icon.atlas = IconPanel.GetAltas(icon_);
            icon.spriteName = icon_;
            if (num != null)
            {
                var num_label = icon_res.transform.Find("num").GetComponent<UILabel>();
                if (num > 1)
                {
                    num_label.gameObject.SetActive(true);
                    num_label.text = num.GetValueOrDefault().ToString();
                }
                else
                    num_label.gameObject.SetActive(false);
            }
            if (frame != 0)
            {
                var frame_icon = icon_res.transform.Find("frame").GetComponent<UISprite>();
                if (frame > 10)
                    frame_icon.spriteName = frame_color_two_[frame % 10];
                else
                    frame_icon.spriteName = frame_color_[frame];
            }

            if (icon_name == "reward_res" || icon_name == "gain_reward_res")
            {

            }
            return icon_res;
        }
        else
            return null;
    }

}

public static class AvaIconPanel
{
    private static Dictionary<string, GameObject> avatar_list_ = new Dictionary<string, GameObject>();
    private static Dictionary<string, UISprite> www_avatar_ = new Dictionary<string, UISprite>();
    private static Dictionary<string, Texture2D> url_avatar_ = new Dictionary<string, Texture2D>();
    private static Dictionary<int, string> sex_icon_ = new Dictionary<int, string>() { { 1, "boy_icon" }, { 2, "girl_icon" } };
    public static void Init()
    {
        avatar_list_.Clear();
        www_avatar_.Clear();
        url_avatar_.Clear();
        GameObject ui_root = GameObject.FindWithTag("UIRoot");
        Transform panel = ui_root.transform.Find("AvaIconPanel");
        if (panel.childCount > 0)
        {
            for (int i = 0; i < panel.childCount; i++)
                avatar_list_.Add(panel.GetChild(i).name, panel.GetChild(i).gameObject);
        }
    }

    public static void ModifyAvatar(Transform avatar_t, int avatar_, string avatar_url, int? frame = null,int? sex = null)
    {
        if (avatar_t != null)
        {
            var avatar_icon = avatar_t.Find("avatar").GetComponent<UISprite>();
            var frame_temp = Config.get_t_toukuang(frame.GetValueOrDefault());
            var avatar_temp = AvaIconPanel.GetIcon(avatar_icon, avatar_, avatar_url);
            if (avatar_temp != null)
            {
                avatar_icon.atlas = IconPanel.GetAltas(avatar_temp.icon);
                avatar_icon.spriteName = avatar_temp.icon;
            }
            if (frame != null)
            {
                var frame_avatar = avatar_t.Find("frame").GetComponent<UISprite>();
                frame_avatar.atlas = IconPanel.GetAltas(frame_temp.big_icon);
                frame_avatar.spriteName = frame_temp.big_icon;
                frame_avatar.MakePixelPerfect();
            }

            if (sex != null)
            {
                var sex_icon = avatar_t.Find("sex").GetComponent<UISprite>();
                sex_icon.spriteName = sex_icon_[sex.GetValueOrDefault() + 1];
            }
        }
    }

    public static t_avatar GetIcon(UISprite avatar_t, int avatar, string avatar_url)
    {
        return Config.get_t_avatar(avatar);
    }

    public static void GetAvatarIconUrl(WWW www)
    {
        if (www_avatar_.ContainsKey(www.url))
        {
            www_avatar_[www.url].mainTexture = www.texture;
            www_avatar_.Remove(www.url);

            if (url_avatar_.ContainsKey(www.url))
                url_avatar_[www.url] = www.texture;
            else
                url_avatar_.Add(www.url, www.texture);
        }
    }

    public static GameObject GetAvatar<T>(string avatar_name, int avatar_, string avatar_url, int? frame = null, T t = null, UIEventListener.VoidDelegate click_fun = null
        ) where T : BEventListener
    {
        if (avatar_list_.ContainsKey(avatar_name))
        {
            var avatar_res = LuaHelper.Instantiate(avatar_list_[avatar_name].gameObject);
            var avatar_icon = avatar_res.transform.Find("avatar").GetComponent<UISprite>();
            var frame_temp = Config.get_t_toukuang(frame.GetValueOrDefault());
            var avatar_temp = AvaIconPanel.GetIcon(avatar_icon, avatar_, avatar_url);
            if (avatar_temp != null)
            {
                avatar_icon.atlas = IconPanel.GetAltas(avatar_temp.icon);
                avatar_icon.spriteName = avatar_temp.icon;
            }

            if (click_fun != null)
            {
                var click_obj = avatar_res.transform.Find("avatar").gameObject;
                t.RegisterOnClick(click_obj, click_fun);
            }

            if (frame != null)
            {
                var frame_avatar = avatar_res.transform.Find("frame").GetComponent<UISprite>();
                frame_avatar.atlas = IconPanel.GetAltas(frame_temp.big_icon);
                frame_avatar.spriteName = frame_temp.big_icon;
                if (avatar_name != "social_res")
                    frame_avatar.MakePixelPerfect();
            }

            var sex_icon = avatar_res.transform.Find("sex");
            if (sex_icon != null)
                sex_icon.gameObject.SetActive(false);
            return avatar_res;
        }
        else
            return null;
    }
}


public class BattlePanel: BEventListener
{
    public class kill_msg_queue
    {
        public int start;
        public int length;
        public Dictionary<int, kill_message_unit> queue;
    }

    public class kill_message_unit
    {
        public double cur_time;
        public BattleAnimal bp;
        public BattleAnimal re_bp;
        public t_avatar bp_avatar;
        public t_avatar re_bp_avatar;
        public int bp_toukuang;
        public int re_bp_toukuang;
        public int lsha;
        public bool finish;
        public Func<kill_message_unit,bool> complete;
        public Action<kill_message_unit> execute;
    }
    public class delay_unit
    {
        public double time;
        public resource_info obj;
        public Action fe;
    }
    public class effect_play_list
    {
        //length = 0,paths = {},r_time={},funcs = {},delays = {}
        public int length;
        public Dictionary<int, delay_unit> delays;
        public Dictionary<int, effect_path> paths;
        public Dictionary<int,float> r_time;
        public Dictionary<int, task_unit>  funcs;
    }

    public class task_unit
    {
        public double s;
        public double t;
        public Action<task_unit> execute;
    }

    public class effect_path
    {
        public bool isMulti;
        public single_path s_path;
        public List<single_path> ms_path;
    }

    public class single_path
    {
        public Vector3 st_p;
        public Vector3 ed_p;
        public Vector3 mid_p1;
        public Vector3 mid_p2;
        public float cur_t;
        public float total_t;
        public resource_info dt;
    }

    public class power_unit
    {
        public UIProgressBar power;
        public GameObject light;
        public TweenAlpha twa;
    }

    public class xt_info
    {
        public GameObject level_icon_obj;
        public Transform objt;
        public int objid;
        public BattleAnimal bp;
        public GameObject bg;
        public UISprite expp;
        public UILabel level;
        public UIProgressBar hp;
        public UIProgressBar hpbg;
        public GameObject jg;
        public int last_max_hp;
        public float hpbg_value;
        public GameObject icon;
        public UISprite iconSp;
        public GameObject obj;

        //player中的能量槽
        public List<power_unit> powerList;
    }
    public class talent_info
    {
        public int objid;
        public Transform objt;
        public GameObject obj;
    }
    public class team_arrow_info
    {
        public BattleAnimal pt;
        public string name;
        public team_arrow_sub_info arrow_m;
    }
    public class team_arrow_sub_info
    {
        public GameObject obj;
        public int objid;
        public Transform objt;
        public int label_id;
        public UILabel label;
        public UISprite sp;
        public int arrow_id;
        public Transform arr_t;
    }

    public class util_info
    {
        public GameObject obj;
        public int objid;
        public Transform objt;
        public double time;
    }
    public class resource_info
    {
        public int objid;
        public GameObject obj;
        public Transform objt;
        public int state;
        public int typeid;
    }
    public class boss_info
    {
        public bool init;
        public GameObject obj;
        public Transform objt;
        public UILabel name;
        public UIProgressBar slider;
        public UILabel hplb;
        public UILabel timelb;
    }
    public class rank_info
    {
        public GameObject obj;
        public UILabel name;
        public UILabel score;
        public UILabel rank;
    }
    public class player_old_info
    {
        public int level_max_golds;
        public int extra_gold_add;
        public int battle_golds;
        public int box_zd_num;
    }
    public class roll_task
    {
        public int last_t;
        public List<roll_task_unit> process;
    }

    public class roll_task_unit
    {
        public Collider bc;
        public int speed;
        public List<Transform> objs;
        public double s;
        public double delay;
        public int lt;
        public bool status;
    }
    public class arrow_info
    {
        public int objid;
        public Transform objt;
        public GameObject obj;
        public int iconid;
    }

    public class un_normal_state_info
    {
        public int objid;
        public Transform objt;
        public GameObject obj;
        public int id;
        public UISprite sp;
    }
    public class un_normal_state_msg
    {
        public int buffer_id;
        public bool complete;
        public Action<un_normal_state_msg> Execute;
        public Func<un_normal_state_msg,bool> Complete;
        public Action<un_normal_state_msg> Finish;
    }
    public class sh_player_info
    {
        public List<inner_sh_player> list;
        public double tm;
    }

    public class team_msg_info
    {
        public float tm;
        public GameObject go;
        public UILabel lb;
    }
    public class talent_ck_msg
    {
        public int talent_id;
        public Transform objt;
    }
    public class inner_sh_player
    {
        public string num;
        public int t;
    }

    private void OnDestroy()
    {
        clear_min_pro();
        foreach (var item in talent_ball_pools_)
        {
            var v = item.Value;
            bobjpool.remove(v.objid);
            GameObject.Destroy(v.obj);
        }
        talent_ball_pools_.Clear();

        foreach (var item in un_normal_state_pools_)
        {
            var v = item.Value;
            bobjpool.remove(v.objid);
            GameObject.Destroy(v.obj);
        }
        un_normal_state_pools_.Clear();

        foreach (var item in resource_pools)
        {
            foreach (var inItem in item.Value)
            {
                bobjpool.remove(inItem.Value.objid);
                GameObject.Destroy(inItem.Value.obj);
            }
        }
        resource_pools.Clear();
        
        kill_message_queue_ = null;
        for (int i = 0; i < arrow_pools.Count; i++)
            bobjpool.remove(arrow_pools[i].objid);
        arrow_pools.Clear();

        if (teamer_arrows_ != null)
        {
            foreach (var item in teamer_arrows_)
            {
                bobjpool.remove(item.Value.arrow_m.objid);
                GameObject.Destroy(item.Value.arrow_m.obj);
            }
            teamer_arrows_.Clear();
        }
        teamer_arrows_ = null;
        bobjpool.clear();
        Old_Self_Data = null;
        tips_ = null;
        mvpList = null;
        max_killList = null;
        cgList = null;
        teamRankList = null;

        if (roll_process != null)
        {
            LuaHelper.GetTimerManager().RemoveRepeatTimer("RollObj");
            roll_process = null;
        }

        RemoveMessage();
    }

    //------------------------------------------------------------------------------------------------------------------//
    GameObject sskill_mask_,sskill_dian_, sskill_cd_, sskill_cicle_,sskill_icon_, sskill_2_mask_,sskill_2_dian_, sskill_2_cd_, sskill_2_cicle_, sskill_2_icon_, jskill_panel_, jskill_cd_, jskill_level_obj_, jskill_icon_obj_, jskill_cicle_, jskill_mask_obj_, jskill_2_panel_, jskill_2_cd_, jskill_2_level_obj_, jskill_2_icon_obj_;
    GameObject jskill_mask_,jskill_dian_, jskill_2_cicle_, jskill_2_mask_obj_, jskill_3_mask_obj_, skill_4_mask_obj, hp_panel_, min_pro_, talent_show_panel_, talent_title_panel_, spare_panel_;
    GameObject talent_panel_, talent_close_btn_, talent_sx_btn_, talent_sx_panel_, talent_tips_panel_, talent_detail_btn, voice_, task_restart_;
    GameObject voice_label_, kill_report_, die_report_, hreturn_, end_panel_, result_panel_, sh_num_, zhimang_, boss_pro_,monster_pro_, setting_panel_, mb_panel_, talent_sx_preb_;
    GameObject effect_panel_, effect_obj_, effect_explode_obj_, effect_skill_obj_, effect_ability_obj_,un_normal_panel_, un_normal_res_, hornor_list_panel_, ui_root_;
    GameObject guide_end_panel_, guide_res, guide_res_left_, guide_random_area_panel_, guide_title_, teamer_guide_panel_, teamer_res_,boss_buff_obj_,boss_buff_desc_;
    GameObject rankObj_,JlMsgRoot_,JlMsgItem_,team_msg_panel_, team_msg_item_, kjmsg_, guide_extra_achieveLog;


    UISprite boss_buff_sp_,sskill_cd_sp_,sskill_power_sp_, sskill_2_power_sp_, sskill_2_cd_sp_, jskill_cd_sp_, jskill_level_, jskill_icon_, jskill_alpha, jskill_2_cd_sp_, jskill_2_level_, jskill_2_icon_, jskill_2_alpha, spare_skill_icon_, sskill_icon_cd_;
    UILabel sskill_cd_text_, sskill_2_cd_text_, jskill_cd_text_, jskill_2_cd_text_, talent_tips_text_, tips_label_, level_, time_, kill_, die_, fuhuo_text_, guide_extra_lb_;
    UILabel end_panel_text_, kill_label, boss_buff_label_,score_label, end_fin_title_, end_fin_red_label_, end_fin_nor_label_, ping_, fps_;
    SphereCollider jskill_box_, jskill_2_box_;
    TweenAlpha tips_;
    UIProgressBar exp_;
    UIPanel kill_report_panel_;
    public Transform kill_report_tx_, die_report_tx_, detail_panel_, cup_panel_, num_panel_, quality_panel_, low_btn_, mid_btn_, high_btn_, normal_eff_, high_eff_, talentBall_panel_, talentBall_res_, guide_panel_, guide_skip_panel_;
    public AudioSource audiosource_;
    double kill_show_time_, die_show_time_, end_time_, tick_, tick_fd_, star_pos_, end_pos_, current_pos_;
    float screen_w_, screen_h_;
    int hp_depth_, sh_depth_, self_player_level, rank_type_, last_update_min_pro_zhen, end_time_play_state, total_sound_times;
    bool exp_change_state_, light_state_, get_max_white_, wave_state,boss_sound = false;
    int[] or_skill_ids_ = { 1001, 2001, 3001 };
    string share_type_,boss_tips = "";
    team_arrow_info team_boss_arrow_res_;


    public bobjpool bobjpool;
    public Dictionary<string, xt_info> min_pro_map_;
    public List<rank_info> rank_lists_;
    public rank_info rank_me_;
    public Dictionary<string, sh_player_info> sh_player_list_;
    public List<team_msg_info> team_msg_queues_;
    public List<util_info> sh_nums_;
    player_old_info Old_Self_Data;
    List<arrow_info> arrow_pools;
    Dictionary<int, un_normal_state_info> un_normal_state_pools_;
    effect_play_list pos_t_;
    Dictionary<int,talent_info> talent_ball_pools_;
    kill_msg_queue kill_message_queue_;
    Dictionary<int, Dictionary<int, resource_info>> resource_pools;
    List<Collider> wheelboxs_;
    boss_info boss_info_;
    Dictionary<string, team_arrow_info> teamer_arrows_;
    public List<string> mvpList,max_killList,cgList;
    List<BattleAnimalPlayer> teamRankList;

    public List<BattleAnimalPlayer> TeamListRank
    {
        get
        {
            return teamRankList;
        }
    }

    roll_task roll_process;
    private int quality_id_,eff_quatity_;
    Dictionary<string, int> ScoreRankByGUID;
    Dictionary<int, BattleAnimalPlayer> ScoreRankByRank;
    Dictionary<string, int> KillRankByGUID;
    Dictionary<int, BattleAnimalPlayer> KillRankByRank;
    Dictionary<int, int> ScoreTeamRankByTeamID;
    Dictionary<int, int> ScoreTeamRankByRank;
    Dictionary<int, int> TeamScore;
    List<talent_ck_msg> talent_time_list;
    Dictionary<int, int> un_normal_states_time = new Dictionary<int, int>();
    Dictionary<int, un_normal_state_msg> unNormalStateList = new Dictionary<int, un_normal_state_msg>();
    List<guide_obj> guide_objs_ = new List<guide_obj>();
    private void Awake()
    {
        IconPanel.Init();
        AvaIconPanel.Init();
        var sskill_panel = this.transform.Find("bottomright/sskill").gameObject;
       
        RegisterOnClick(sskill_panel, OnClick);
        var sskill_2_panel = this.transform.Find("bottomright/sskill_2").gameObject;

        RegisterOnClick(sskill_2_panel, OnClick);

        task_restart_ = this.transform.Find("task_restart").gameObject;

        sskill_cd_ = this.transform.Find("bottomright/sskill/cd").gameObject;
        sskill_cd_sp_ = this.transform.Find("bottomright/sskill/cd/sp").GetComponent<UISprite>();
        sskill_cd_text_ = this.transform.Find("bottomright/sskill/cd/text").GetComponent<UILabel>();
        sskill_icon_ = this.transform.Find("bottomright/sskill/Sprite/xuerenIcon").gameObject;
        sskill_power_sp_ = this.transform.Find("bottomright/sskill/Sprite/pow").GetComponent<UISprite>();
        sskill_cicle_ = this.transform.Find("bottomright/sskill/Sprite/Sprite").gameObject;
        sskill_dian_ = this.transform.Find("bottomright/sskill/Sprite/dian").gameObject;
        sskill_mask_ = this.transform.Find("bottomright/sskill/Sprite/mask").gameObject;

        sskill_2_cd_ = this.transform.Find("bottomright/sskill_2/cd").gameObject;
        sskill_2_cd_sp_ = this.transform.Find("bottomright/sskill_2/cd/sp").GetComponent<UISprite>();
        sskill_2_cd_text_ = this.transform.Find("bottomright/sskill_2/cd/text").GetComponent<UILabel>();
        sskill_2_icon_ = this.transform.Find("bottomright/sskill_2/Sprite/xuerenIcon").gameObject;
        sskill_2_power_sp_ = this.transform.Find("bottomright/sskill_2/Sprite/pow").GetComponent<UISprite>();
        sskill_2_cicle_ = this.transform.Find("bottomright/sskill_2/Sprite/Sprite").gameObject;
        sskill_2_dian_ = this.transform.Find("bottomright/sskill_2/Sprite/dian").gameObject;
        sskill_2_mask_ = this.transform.Find("bottomright/sskill_2/Sprite/mask").gameObject;

        jskill_panel_ = this.transform.Find("bottomright/jskill").gameObject;
        jskill_cd_ = this.transform.Find("bottomright/jskill/cd").gameObject;
        jskill_cd_sp_ = this.transform.Find("bottomright/jskill/cd/sp").GetComponent<UISprite>();
        jskill_cd_text_ = this.transform.Find("bottomright/jskill/cd/text").GetComponent<UILabel>();
        jskill_level_ = this.transform.Find("bottomright/jskill/di/level").GetComponent<UISprite>();
        jskill_level_obj_ = this.transform.Find("bottomright/jskill/di/level").gameObject;
        jskill_icon_ = this.transform.Find("bottomright/jskill/di/icon").GetComponent<UISprite>();
        jskill_icon_.spriteName = "";
        jskill_box_ = this.transform.Find("bottomright/jskill").GetComponent<SphereCollider>();
        jskill_icon_obj_ = this.transform.Find("bottomright/jskill/di/icon").gameObject;
        jskill_cicle_ = this.transform.Find("bottomright/jskill/di/Sprite").gameObject;
        jskill_alpha = this.transform.Find("bottomright/jskill/di").GetComponent<UISprite>();
        jskill_mask_obj_ = this.transform.Find("bottomright/skill_mask").gameObject;
        jskill_dian_ = this.transform.Find("bottomright/jskill/di/dian").gameObject;
        jskill_mask_ = this.transform.Find("bottomright/jskill/di/mask").gameObject;

        jskill_2_panel_ = this.transform.Find("bottomright/jskill_2").gameObject;
        jskill_2_cd_ = this.transform.Find("bottomright/jskill_2/cd").gameObject;
        jskill_2_cd_sp_ = this.transform.Find("bottomright/jskill_2/cd/sp").GetComponent<UISprite>();
        jskill_2_cd_text_ = this.transform.Find("bottomright/jskill_2/cd/text").GetComponent<UILabel>();
        jskill_2_level_ = this.transform.Find("bottomright/jskill_2/di/level").GetComponent<UISprite>();
        jskill_2_level_obj_ = this.transform.Find("bottomright/jskill_2/di/level").gameObject;
        jskill_2_icon_ = this.transform.Find("bottomright/jskill_2/di/icon").GetComponent<UISprite>();
        jskill_2_icon_.spriteName = "";
        jskill_2_icon_obj_ = this.transform.Find("bottomright/jskill_2/di/icon").gameObject;
        jskill_2_box_ = this.transform.Find("bottomright/jskill_2").GetComponent<SphereCollider>();
        jskill_2_box_.enabled = false;
        jskill_2_cicle_ = this.transform.Find("bottomright/jskill_2/di/Sprite").gameObject;
        jskill_2_alpha = this.transform.Find("bottomright/jskill_2/di").GetComponent<UISprite>();
        jskill_2_mask_obj_ = this.transform.Find("bottomright/skill_mask_2").gameObject;
        jskill_3_mask_obj_ = this.transform.Find("bottomright/skill_mask_3").gameObject;
        skill_4_mask_obj = this.transform.Find("bottomright/skill_mask_4").gameObject;

        //局类消息
        kjmsg_ = this.transform.Find("topright/kjMsg").gameObject;
        if (BattlePlayers.battle_type == 1)
            kjmsg_.SetActive(true);
        else
            kjmsg_.SetActive(false);
        RegisterOnClick(kjmsg_, OnClick);

        JlMsgRoot_ = this.transform.Find("topright/cts").gameObject;
        JlMsgItem_ = this.transform.Find("topright/kjs_item").gameObject;

        team_msg_panel_ = this.transform.Find("left/team_msg_panel").gameObject;
        team_msg_item_ = this.transform.Find("left/team_msg_item").gameObject;
        team_msg_queues_ = new List<team_msg_info>();

        if (!Battle.is_newplayer_guide)
        {
            jskill_mask_obj_.SetActive(false);
            jskill_2_mask_obj_.SetActive(false);
            jskill_3_mask_obj_.SetActive(false);
            skill_4_mask_obj.SetActive(false);
        }

        hp_panel_ = this.transform.Find("bottomleft/hp_panel").gameObject;
        min_pro_ = this.transform.Find("bottomleft/hp_panel/min_pro").gameObject;
        talent_show_panel_ = this.transform.Find("Bottom/talent_panel").gameObject;
        talent_title_panel_ = this.transform.Find("Bottom/talent_title").gameObject;

        for (int i = 1; i <= 3; i++)
        {
            var td = talent_show_panel_.transform.Find("area_" + i + "/2nd");
            RegisterOnClick(td.gameObject, OnClick);
        }

        bobjpool = new bobjpool();

        spare_panel_ = this.transform.Find("bottomright/thskill").gameObject;
        spare_skill_icon_ = spare_panel_.transform.Find("bg/icon").GetComponent<UISprite>();
        RegisterOnClick(spare_panel_, OnClick);
        talent_panel_ = this.transform.Find("JobLines_panel").gameObject;
        talent_close_btn_ = talent_panel_.transform.Find("close_btn").gameObject;
        talent_sx_btn_ = talent_panel_.transform.Find("ContentBlock/leftBlock/attrBtn").gameObject;
        talent_sx_panel_ = talent_panel_.transform.Find("ContentBlock/rightBlock/sxPanel").gameObject;
        talent_tips_panel_ = this.transform.Find("JsDesPanel").gameObject;
        talent_tips_text_ = talent_tips_panel_.transform.Find("talentDescBlock").GetComponent<UILabel>();
        talent_detail_btn = this.transform.Find("bottomright/jkb").gameObject;
        tips_ = this.transform.Find("top/tips").GetComponent<TweenAlpha>();
        tips_label_ = this.transform.Find("top/tips").GetComponent<UILabel>();
        RegisterOnClick(talent_close_btn_, CloseTalentDetailInfo);
        RegisterOnClick(talent_sx_btn_, OpenTalentSXPage);
        RegisterOnClick(talent_detail_btn,ShowJobLinesDetailInfo);

        min_pro_map_ = new Dictionary<string, xt_info>();

        level_ = this.transform.Find("bottomleft/level/text").GetComponent<UILabel>();
        exp_ = this.transform.Find("bottomleft/exp").GetComponent<UIProgressBar>();
        time_ = this.transform.Find("topright/mb/time").GetComponent<UILabel>();
        kill_ = this.transform.Find("topright/mb/kill").GetComponent<UILabel>();
        die_ = this.transform.Find("topright/mb/die").GetComponent<UILabel>();

        voice_ = this.transform.Find("topright/voice").gameObject;
        voice_label_ = this.transform.Find("topright/voice/voiceLabel").gameObject;

        kill_report_ = this.transform.Find("top/kill_report").gameObject;
        kill_report_panel_ = kill_report_.GetComponent<UIPanel>();
        kill_report_tx_ = kill_report_.transform.Find("tx");

        die_report_ = this.transform.Find("die_report").gameObject;
        die_report_tx_ = die_report_.transform.Find("tx");
        kill_show_time_ = 0;
        fuhuo_text_ = die_report_.transform.Find("fuhuo").GetComponent<UILabel>();
        hreturn_ = die_report_.transform.Find("return").gameObject;
        RegisterOnClick(hreturn_, hreturn);
        die_show_time_ = 0;
        //ZDB
        rankObj_ = this.transform.Find("topright/rank").gameObject;


        rank_lists_ = new List<rank_info>();
        for (int i = 0; i <= 4; i++)
        {
            var item = new rank_info();
            item.obj = this.transform.Find("topright/rank/rank_list" + i).gameObject;
            item.name = item.obj.transform.Find("name").GetComponent<UILabel>();
            item.score = item.obj.transform.Find("score").GetComponent<UILabel>();
            rank_lists_.Add(item);
        }

        rank_me_ = new rank_info();
        rank_me_.obj = this.transform.Find("topright/rank/rank_list_me").gameObject;
        rank_me_.rank = rank_me_.obj.transform.Find("rank").GetComponent<UILabel>();
        rank_me_.name = rank_me_.obj.transform.Find("name").GetComponent<UILabel>();
        rank_me_.score = rank_me_.obj.transform.Find("score").GetComponent<UILabel>();

        end_panel_ = this.transform.Find("end_panel").gameObject;
        end_panel_text_ = end_panel_.transform.Find("top/mid/text").GetComponent<UILabel>();

        var end_back = end_panel_.transform.Find("back").gameObject;
        RegisterOnClick(end_back, CountPlayerCup);
        end_time_ = 0;

        result_panel_ = this.transform.Find("result_panel").gameObject;
        detail_panel_ = this.transform.Find("detail_panel");
        cup_panel_ = this.transform.Find("cup_panel");

        var return_button = result_panel_.transform.Find("baseboard/return").gameObject;
        if (BattlePlayers.battle_type == 1)
        {
            result_panel_.transform.Find("baseboard/me/name").localPosition = new Vector3(-155, -8, 0);
            result_panel_.transform.Find("baseboard/me/name/team_name").gameObject.SetActive(true);
        }
        else
        {
            result_panel_.transform.Find("baseboard/me/name").localPosition = new Vector3(-155, 0, 0);
            result_panel_.transform.Find("baseboard/me/name/team_name").gameObject.SetActive(false);
        }

        var cup_share_btn = cup_panel_.Find("main_panel/Anchor_b/share_btn").gameObject;
        var end_btn = cup_panel_.Find("bg").gameObject;
        RegisterOnClick(end_btn, InitDetailPanel);
        RegisterOnClick(cup_share_btn, Share);
        RegisterOnClick(return_button, hreturn);
        var share_btn = detail_panel_.Find("main_panel/share_btn").gameObject;
        var ok_btn = detail_panel_.Find("main_panel/ok_btn").gameObject;
        RegisterOnClick(share_btn, Share);
        RegisterOnClick(ok_btn, end_click);
        num_panel_ = this.transform.Find("bottomleft/num_panel");
        sh_num_ = this.transform.Find("bottomleft/num_panel/sh").gameObject;
        zhimang_ = this.transform.Find("zhimang").gameObject;
        sh_nums_ = new List<util_info>();
        sh_player_list_ = new Dictionary<string, sh_player_info>();

        tick_ = 0;
        tick_fd_ = 2;
        hp_depth_ = -2;
        sh_depth_ = -201;

        self.pre_battle_num = self.player.box_zd_num;
        Old_Self_Data = new player_old_info();

        
        boss_pro_ = this.transform.Find("bottomleft/hp_panel/boss_pro").gameObject;
        monster_pro_ = this.transform.Find("bottomleft/hp_panel/monster_pro").gameObject;

        setting_panel_ = this.transform.Find("topright/settings").gameObject;
        // ZDB 
        mb_panel_ = this.transform.Find("topright/mb").gameObject;
        //ZDB 隐藏 MB
        if (Battle.is_newplayer_guide)
        {
            rankObj_.SetActive(false);
            mb_panel_.SetActive(false);

        }
        var set_btn = setting_panel_.transform.Find("pic");
        var picQua_btn = this.transform.Find("topright/mb/picQua");
        RegisterOnClick(set_btn.gameObject, GoBackHall);
        RegisterOnClick(picQua_btn.gameObject, OnClick);
        talent_sx_preb_ = this.transform.Find("JobLines_panel/ContentBlock/rightBlock/info").gameObject;
        sskill_icon_cd_ = this.transform.Find("bottomright/sskill/Sprite/lq").GetComponent<UISprite>();
        quality_panel_ = this.transform.Find("quality_panel");
        var quality_close_btn = quality_panel_.Find("close_btn");
        var quality_ok_btn = quality_panel_.Find("quality_ok_btn");
        low_btn_ = quality_panel_.Find("low");
        mid_btn_ = quality_panel_.Find("mid");
        high_btn_ = quality_panel_.Find("high");
        normal_eff_ = quality_panel_.Find("normal_eff");
        high_eff_ = quality_panel_.Find("high_eff");

        RegisterOnClick(quality_close_btn.gameObject, OnClick);
        RegisterOnClick(quality_ok_btn.gameObject, OnClick);
        RegisterOnClick(low_btn_.gameObject, OnClick);
        RegisterOnClick(mid_btn_.gameObject, OnClick);
        RegisterOnClick(high_btn_.gameObject, OnClick);
        RegisterOnClick(normal_eff_.gameObject, OnClick);
        RegisterOnClick(high_eff_.gameObject, OnClick);

        if (Battle.is_online)
        {
            setting_panel_.SetActive(false);
            mb_panel_.transform.localPosition = new Vector3(-140.5f, -20, 0);
            voice_.SetActive(false); //暂时屏蔽语音
        }
        else
        {
            setting_panel_.SetActive(true);
            mb_panel_.transform.localPosition = new Vector3(-185, -20, 0);
            voice_.SetActive(false);
        }
        self_player_level = self.player.level;

        effect_panel_ = this.transform.Find("bottomleft/effectpanel").gameObject;
        effect_obj_ = this.transform.Find("bottomleft/effectpanel/movePoint").gameObject;
        effect_explode_obj_ = this.transform.Find("bottomleft/effectpanel/item_effect").gameObject;
        effect_skill_obj_ = this.transform.Find("bottomleft/effectpanel/movePoint1").gameObject;
        effect_ability_obj_ = this.transform.Find("bottomleft/effectpanel/movePoint2").gameObject;

        //////////////////////////////////////////////////////////////////////////////////////////////
        
        talentBall_panel_ = this.transform.Find("Bottom/foodsIcon");
        talentBall_res_ = this.transform.Find("Bottom/food_item");
        un_normal_panel_ = this.transform.Find("Bottom/un_normal_state").gameObject;
        un_normal_res_ = this.transform.Find("Bottom/un_normal_state_res").gameObject;
        hornor_list_panel_ = this.transform.Find("center/hornor_list_panel").gameObject;
        var hor_close = this.transform.Find("center/hornor_list_panel/hor_close").gameObject;
        RegisterOnClick(hor_close, OnClick);

        arrow_pools = new List<arrow_info>();
        var arrow_boss_obj_ = this.transform.Find("center/guid/boss_sign").gameObject;
        var arrow_st_obj_ = this.transform.Find("center/guid/first").gameObject;
        var arrow_se_obj_ = this.transform.Find("center/guid/sec").gameObject;
        var arrow_rd_obj_ = this.transform.Find("center/guid/third").gameObject;

        int objid = bobjpool.add(arrow_boss_obj_);
        var objt = arrow_boss_obj_.transform;
        var objd = arrow_boss_obj_;
        var iconid = bobjpool.add(arrow_boss_obj_.transform.Find("rank").gameObject);
        arrow_pools.Add(new arrow_info() { objid = objid, objt = objt, obj = objd, iconid = iconid });


        objid = bobjpool.add(arrow_st_obj_);
        objt = arrow_st_obj_.transform;
        objd = arrow_st_obj_;
        iconid = bobjpool.add(arrow_st_obj_.transform.Find("rank").gameObject);
        arrow_pools.Add(new arrow_info() { objid = objid, objt = objt, obj = objd, iconid = iconid });

        objid = bobjpool.add(arrow_se_obj_);
        objt = arrow_se_obj_.transform;
        objd = arrow_se_obj_;
        iconid = bobjpool.add(arrow_se_obj_.transform.Find("rank").gameObject);
        arrow_pools.Add(new arrow_info() { objid = objid, objt = objt, obj = objd, iconid = iconid });

        objid = bobjpool.add(arrow_rd_obj_);
        objt = arrow_rd_obj_.transform;
        objd = arrow_rd_obj_;
        iconid = bobjpool.add(arrow_rd_obj_.transform.Find("rank").gameObject);
        arrow_pools.Add(new arrow_info() { objid = objid, objt = objt, obj = objd, iconid = iconid });

        screen_w_ = LuaHelper.GetPanelManager().get_w();
        screen_h_ = LuaHelper.GetPanelManager().get_h();

        var score_btn_ = this.transform.Find("topright/rank/toggle/scoreRankBtn").gameObject;
        var kill_btn_ = this.transform.Find("topright/rank/toggle/killRankBtn").gameObject;

        un_normal_state_pools_ = new Dictionary<int, un_normal_state_info>();
        un_normal_states_time = new Dictionary<int, int>();
        unNormalStateList = new Dictionary<int, un_normal_state_msg>();

        foreach (var item in Config.t_battle_attrs)
        {
            var v = item.Value;
            if (v.isBadState >= 1)
            {
                var obj = LuaHelper.Instantiate(un_normal_res_);
                var obt = obj.transform;
                obt.parent = un_normal_panel_.transform;
                obt.localPosition = Vector3.zero;
                obt.localScale = Vector3.one;

                obj.name = item.Key.ToString();
                var sp_bg = obt.GetComponent<UISprite>();
                var sp_fo = obt.Find("state_bg").GetComponent<UISprite>();
                sp_bg.spriteName = v.icon_bg;
                sp_fo.spriteName = v.icon;

                var oid = bobjpool.add(obj);
                var sp = obt.Find("state_bg").GetComponent<UISprite>();
                un_normal_state_pools_.Add(v.id, new un_normal_state_info() { objid = oid, objt = obt, obj = obj, id = v.id, sp = sp });
                obj.gameObject.SetActive(false);
            }
        }
        
        kill_label = kill_btn_.transform.Find("killRank").GetComponent<UILabel>();
        score_label = score_btn_.transform.Find("scoreRank").GetComponent<UILabel>();

        rank_type_ = 1;
        if (BattlePlayers.battle_type == 1)
            kill_label.text = "[B5F0EC]"+Config.get_t_script_str("BattlePanel_001");
        else
            kill_label.text = "[B5F0EC]" + Config.get_t_script_str("BattlePanel_002");

        score_label.text = "[D0A851]" + Config.get_t_script_str("BattlePanel_003");

        RegisterOnClick(score_btn_, OnClick);
        RegisterOnClick(kill_btn_, OnClick);


        exp_change_state_ = false;
        light_state_ = false;
        get_max_white_ = false;

        star_pos_ = 1;
        end_pos_ = 1;
        current_pos_ = 1;

        if (BattlePlayers.me != null)
        {
            star_pos_ = BattlePlayers.me.player.level + BattlePlayers.me.get_exp_per();
            end_pos_ = BattlePlayers.me.player.level + BattlePlayers.me.get_exp_per();
            current_pos_ = BattlePlayers.me.player.level + BattlePlayers.me.get_exp_per();
        }

        pos_t_ = new effect_play_list() { length = 0, paths = new Dictionary<int, effect_path>(), r_time = new Dictionary<int, float>(), funcs = new Dictionary<int, task_unit>(), delays = new Dictionary<int, delay_unit>() };

        talent_ball_pools_ = new Dictionary<int, talent_info>();
        kill_message_queue_ = new kill_msg_queue() { queue = new Dictionary<int, kill_message_unit>(), start = 0, length = 0 };
       
        end_fin_title_ = this.transform.Find("top/time_panel/title").GetComponent<UILabel>();
        end_fin_red_label_ = end_fin_title_.transform.Find("end_time_red").GetComponent<UILabel>();
        end_fin_nor_label_ = end_fin_title_.transform.Find("end_time_normal").GetComponent<UILabel>();

        resource_pools = new Dictionary<int, Dictionary<int, resource_info>>();
        wheelboxs_ = new List<Collider>();
        var dir_wheel = this.transform.Find("bottomleft/move").GetComponent<BoxCollider>();
        var attack_wheel = this.transform.Find("bottomright/attack").GetComponent<SphereCollider>();
        var skill_wheel = this.transform.Find("bottomright/jskill").GetComponent<SphereCollider>();
        var skill_2_wheel = this.transform.Find("bottomright/jskill_2").GetComponent<SphereCollider>();
        var ljskill = this.transform.Find("bottomright/sskill_2").GetComponent<SphereCollider>();
        wheelboxs_.Add(dir_wheel);
        wheelboxs_.Add(attack_wheel);
        wheelboxs_.Add(skill_wheel);
        wheelboxs_.Add(skill_2_wheel);
        wheelboxs_.Add(ljskill);
        
        ping_ = this.transform.Find("topright/netfps_panel/label/netdelay").GetComponent<UILabel>();
        fps_ = this.transform.Find("topright/netfps_panel/label/fps").GetComponent<UILabel>();
        ui_root_ = GameObject.FindWithTag("UIRoot");
        guide_panel_ = this.transform.Find("guide_panel");
        guide_skip_panel_ = this.transform.Find("guide_skip_panel");

        
        guide_end_panel_ = this.transform.Find("guide_end_panel").gameObject;
        guide_res = this.transform.Find("guide_panel/guide_res").gameObject;
        guide_res_left_ = this.transform.Find("guide_panel/guide_res_left").gameObject;
        guide_random_area_panel_ = this.transform.Find("guide_mask_panel").gameObject;
        guide_title_ = this.transform.Find("guide_mask_panel/guide_title").gameObject;
        guide_extra_lb_ = this.transform.Find("guide_mask_panel/count_num").GetComponent<UILabel>();
        guide_extra_achieveLog = this.transform.Find("guide_mask_panel/achieveLog").gameObject;

        var skip_btn = guide_random_area_panel_.transform.Find("skip_step").gameObject;
        var guide_ok_btn = guide_end_panel_.transform.Find("guide_ok_btn");
        RegisterOnClick(skip_btn, OnClick);
        RegisterOnClick(guide_ok_btn.gameObject, OnClick);
        audiosource_ = this.GetComponent<AudioSource>();
        
        boss_buff_obj_ = this.transform.Find("left/buff").gameObject;
        boss_buff_sp_ = boss_buff_obj_.transform.Find("boss_buff_icon/mask").GetComponent<UISprite>();
        boss_buff_label_ = boss_buff_obj_.transform.Find("label").GetComponent<UILabel>();
        boss_buff_desc_ = boss_buff_obj_.transform.Find("desc").gameObject;
        boss_buff_obj_.transform.Find("desc/label").GetComponent<UILabel>().text = Config.get_t_battle_item(301).desc;
        RegisterOnPress(boss_buff_obj_.transform.Find("boss_buff_icon").gameObject, OnPress);

        NewPlayerGuide.OnInit();

        var boss_obj = this.transform.Find("topleft/boss_info").gameObject;
        var name = boss_obj.transform.Find("name").GetComponent<UILabel>();
        var slider = boss_obj.transform.Find("hp").GetComponent<UISlider>();
        var timelb = boss_obj.transform.Find("time_label").GetComponent<UILabel>();
        var hplb = boss_obj.transform.Find("hp/num").GetComponent<UILabel>();

        boss_info_ = new boss_info() { init = false, obj = boss_obj, objt = boss_obj.transform, name = name, slider = slider, hplb = hplb, timelb = timelb};
        teamer_guide_panel_ = this.transform.Find("center/teamer_guide_panel").gameObject;
        teamer_res_ = this.transform.Find("center/teamer_guide_panel/teamer_res").gameObject;

        team_boss_arrow_res_ = null;

        exp_.value = 0;
        mvpList = new List<string>();
        max_killList = new List<string>();
        cgList = new List<string>();
        teamRankList = new List<BattleAnimalPlayer>();
        ScoreRankByGUID = new Dictionary<string, int>();
        ScoreRankByRank = new Dictionary<int, BattleAnimalPlayer>();
        KillRankByGUID = new Dictionary<string, int>();
        KillRankByRank = new Dictionary<int, BattleAnimalPlayer>();
        ScoreTeamRankByTeamID = new Dictionary<int, int>();
        ScoreTeamRankByRank = new Dictionary<int, int>();
        TeamScore = new Dictionary<int, int>();
        talent_time_list = new List<talent_ck_msg>();

        RegisterMessage();
    }

    private void RegisterMessage()
    {
        CSharpNetMessage.AddCSharpNetEvent(opclient_t.SMSG_PLAYER_LOOK,SMSG_PLAYER_LOOK,true);
    }

    private void RemoveMessage()
    {
        CSharpNetMessage.RemoveCSharpNetEvent(opclient_t.SMSG_PLAYER_LOOK, SMSG_PLAYER_LOOK);
    }

    private void Start()
    {
        
    }

    public void UpdateBattlePanel()
    {
        if (BattlePlayers.jiasu || BattlePlayers.me == null)
            return;

        update_min_pro();
        if (bobjpool != null)
            bobjpool.update();

        if(BattlePlayers.bobjpool != null)
            BattlePlayers.bobjpool.update();

        if (BattlePlayers.me == null)
            return;

        if (kill_show_time_ > 0)
        {
            kill_show_time_ = kill_show_time_ - Time.deltaTime;
            if (kill_show_time_ <= 0)
                kill_report_.SetActive(false);
            else if (kill_show_time_ <= 1)
                kill_report_panel_.alpha = (float)kill_show_time_;
        }

        if (die_show_time_ > 0)
        {
            die_show_time_ = die_show_time_ - Time.deltaTime / Time.timeScale;
            if (die_show_time_ <= 0)
            {
                die_show_time_ = 0;
                die_report_.SetActive(false);
            }
        }

        if (end_time_ > 0)
        {
            end_time_ = end_time_ - Time.deltaTime / Time.timeScale;
            if (end_time_ <= 0)
                end_time_ = 0;
        }
        for (int i = sh_nums_.Count - 1; i >= 0; i--)
        {
            var sh = sh_nums_[i];
            sh.time = sh.time - Time.deltaTime;
            if (sh.time < 0)
            {
                bobjpool.remove(sh.objid);
                GameObject.Destroy(sh.obj);
                sh_nums_.RemoveAt(i);
            }
        }

        //更新团队消息
        if (BattlePlayers.battle_type == 1)
        {
            int delete_num = 0;
            for (int i = 0; i < team_msg_queues_.Count && i < 5; i++)
            {
                if (team_msg_queues_[i].tm <= 0)
                {
                    delete_num++;
                    continue;
                }
                if (!team_msg_queues_[i].go.activeInHierarchy)
                {
                    team_msg_queues_[i].go.SetActive(true);
                    if(team_msg_queues_[i].lb != null)
                        StartCoroutine(ColorChange(team_msg_queues_[i].lb));
                }
                
                team_msg_queues_[i].tm = team_msg_queues_[i].tm - Time.deltaTime / Time.timeScale;
                if (team_msg_queues_[i].tm <= 0)
                    team_msg_queues_[i].tm = 0;

                //float t = (5 - team_msg_queues_[i].tm) / 5;
                //Debug.LogError("t :" + t + "," + (Time.deltaTime / Time.timeScale));
                //t = BattleOperation.Lerp(0, 200, t);
                //team_msg_queues_[i].go.transform.localPosition = new Vector3(20, t, 0);
            }

            if (delete_num > 0)
            {
                for (int i = delete_num - 1; i >= 0; i--)
                {
                    GameObject.Destroy(team_msg_queues_[i].go);
                    team_msg_queues_.RemoveAt(i);
                }   
            }
        }
        
        foreach (var item in sh_player_list_)
        {
            if (item.Value.tm > 0)
                item.Value.tm -= Time.deltaTime;
            while (item.Value.tm <= 0 && item.Value.list.Count > 0)
            {
                var p = item.Value.list[0];
                item.Value.list.RemoveAt(0);
                item.Value.tm = item.Value.tm + 0.2;
                if(BattlePlayers.players.ContainsKey(item.Key))
                    add_text1(BattlePlayers.players[item.Key], p.num, p.t);      
            }
        }

        if (exp_change_state_)
            UpdateExpProgress();
        Animation();
        UnNormalStateAnimation();
        kill_message_queue();
        //当是新手引导的时候
        if (Battle.is_newplayer_guide)
        {
            NewPlayerGuide.UpdateGuide();
            CheckSkIcon();
        }

        tick_fd_ = tick_fd_ + Time.deltaTime;
        if (tick_fd_ >= 2)
        {
            tick_fd_ = 0;
            UpdateFD();
        }
        tick_ = tick_ + Time.deltaTime / Time.timeScale;

        bool flag = false;
        if (tick_ > 0.1)
        {
            tick_ = 0;
            flag = true;
        }
        if (!flag)
            return;

        if (die_show_time_ > 0)
            fuhuo_text_.text = BattleOperation.toInt(die_show_time_ + 0.99).ToString();

        update_cd();
        var bpme = BattlePlayers.me;
        level_.text = Config.get_t_script_str("BattlePanel_004")+" " + bpme.player.level;

        if (!Battle.is_newplayer_guide)
        {
            var zhen = BattlePlayers.TNUM * 60 * Battle.turnTime + Battle.exTime - BattlePlayers.zhen;
            if (zhen < 0)
                zhen = 0;
            var miao = BattleOperation.toInt(zhen / BattlePlayers.TNUM);
            var fen = BattleOperation.toInt(miao / 60);
            time_stage();
            fen = Math.Abs(fen);
            miao = miao - fen * 60;
            var sfen = fen.ToString();
            if (fen < 10)
                sfen = "0" + sfen;
            var smiao = miao.ToString();
            if (miao < 10)
                smiao = "0" + Math.Abs(miao);
            time_.text = sfen + ":" + smiao;
        }

        kill_.text = bpme.player.sha.ToString();
        die_.text = bpme.player.die.ToString();
        if (BattlePlayers.battle_type == 1)
        {
            RefreshRankByTeam();
            update_teamer_guide_arrow();
        }
        else
        {
            RefreshRankList();
            SetGuidArrow();
        }
        RefreshRankUI();
        UpdateBossInfo();
        XueGuaiBuff();
    }
    private void update_boss_res_info()
    {
        var boss_res_ = this.transform.Find("center/teamer_guide_panel/boss_res").gameObject;
        var objt = boss_res_.transform;
        var sp = objt.Find("rank").GetComponent<UISprite>();
        var arr_t = objt.Find("rank/arrow");
        boss_res_.transform.parent = teamer_guide_panel_.transform;
        objt.localScale = Vector3.one;
        objt.localPosition = Vector3.zero;

        team_boss_arrow_res_ = new team_arrow_info();
        team_boss_arrow_res_.arrow_m = new team_arrow_sub_info() { obj = boss_res_,objt = objt, arr_t = arr_t,sp = sp};
        team_boss_arrow_res_.pt = BattlePlayers.Boss;
        team_boss_arrow_res_.name = BattlePlayers.Boss.player.name;
    }
    public void update_teamer_guide_arrow()
    {
        if (teamer_arrows_ == null || BattlePlayers.Camps.Count <= 0 || BattlePlayers.players_list.Count <= 0)
            return;

        if (team_boss_arrow_res_ == null || (team_boss_arrow_res_.pt.animal.camp != BattlePlayers.Boss.player.camp))
            update_boss_res_info();

        if (BattlePlayers.Boss == null || !BattlePlayers.BossIsActive())
        {
            if (team_boss_arrow_res_.arrow_m.obj.activeInHierarchy)
                team_boss_arrow_res_.arrow_m.obj.SetActive(false);
        }
        else
        {
            update_team_arrow_info(team_boss_arrow_res_,true);
        }

        foreach (var item in teamer_arrows_)
        {
            var k = item.Key;
            var v = item.Value;
            var ta = v;
            update_team_arrow_info(ta);
        }
    }

    private void update_team_arrow_info(team_arrow_info ta,bool is_boss = false)
    {
        var bpme = BattlePlayers.me;
        var pos = LuaHelper.GetMapManager().WorldToScreenPoint(ta.pt.objt.position);
        var obj = ta.arrow_m.obj;
        var pt = ta.pt;
        if (!(pos.x < -10 || pos.x > screen_w_ + 10 || pos.y < -10 || pos.y > screen_h_ + 50))
        {
            if (obj.activeSelf)
                obj.SetActive(false);
        }
        else
        {
            long dx = pt.animal.x - bpme.player.x;
            long dy = pt.animal.y - bpme.player.y;
            double angle = 0;
            if (dx == 0 && pt.animal.y >= bpme.player.y)
                angle = 180;
            else if (dx == 0 && pt.animal.y < bpme.player.y)
                angle = 0;
            else
            {
                angle = Math.Atan(dy * 1.0 / dx) * 180 / 3.1415;
                angle = (angle + 180) % 180;
                if (dy < 0)
                    angle = angle + 180;
            }

            double width = 0, height = 0, now_x = 0, now_y = 0;
            if (is_boss)
            {
                width = BattleOperation.toInt(ta.arrow_m.sp.width / 2);
                height = BattleOperation.toInt(ta.arrow_m.sp.height / 2);
            }
            else
            {
                width = BattleOperation.toInt(ta.arrow_m.label.width / 2);
                height = BattleOperation.toInt(ta.arrow_m.label.height / 2);
            }
            
            double maxLen_1 = Math.Abs(screen_w_ / 2 / Math.Cos(angle * 3.1415 / 180));
            double maxLen_2 = Math.Abs(screen_h_ / 2 / Math.Cos((angle + 90) * 3.1415 / 180));
            double min = Math.Min(maxLen_1 - Math.Abs((40 + width / 2.0) / Math.Cos(angle * 3.1415 / 180)), maxLen_2 - Math.Abs((30 + height / 2.0) / Math.Cos((angle + 90) * 3.1415 / 180)));
            double x = min * dx / Math.Sqrt(dx * dx + dy * dy);
            double y = min * dy / Math.Sqrt(dx * dx + dy * dy);
            var x_pert = (x + screen_w_ / 2) / screen_w_;
            var y_pert = (y + screen_h_ / 2) / screen_h_;

            if (is_boss)
            {
                now_x = x_pert * ta.arrow_m.sp.width - width;
                now_y = y_pert * ta.arrow_m.sp.height - height;
            }
            else
            {
                now_x = x_pert * ta.arrow_m.label.width - width;
                now_y = y_pert * ta.arrow_m.label.height - height;
            }   

            double ag = 0;
            if (now_x == 0 && now_y >= 0)
                ag = 180;
            else if (now_x == 0 && now_y < 0)
                ag = 0;
            else
            {
                ag = Math.Atan(now_y * 1.0 / now_x) * 180 / 3.1415;
                ag = (ag + 180) % 180;
                if (now_y < 0)
                    ag = ag + 180;
            }

            var arr_len = Math.Min(Math.Abs((width + 11) / Math.Cos(ag * 3.1415 / 180)), Math.Abs((height + 9) / Math.Cos((ag + 90) * 3.1415 / 180)));
            var x1 = arr_len * now_x / Math.Sqrt(now_x * now_x + now_y * now_y);
            var y1 = arr_len * now_y / Math.Sqrt(now_x * now_x + now_y * now_y);
            ag = ag - 90;

            if (is_boss)
            {
                ta.arrow_m.sp.transform.localPosition = new Vector3((float)x, (float)y, 0);
                ta.arrow_m.sp.transform.localEulerAngles = Vector3.zero;
                ta.arrow_m.arr_t.localPosition = new Vector3((float)x1, (float)y1, 0);
                ta.arrow_m.arr_t.localEulerAngles = new Vector3(0, 0, (float)ag);
            }
            else
            {
                bobjpool.set_localPosition(ta.arrow_m.label_id, (float)x, (float)y, 0);
                bobjpool.set_localEulerAngles(ta.arrow_m.label_id, 0, 0, 0);
                bobjpool.set_localPosition(ta.arrow_m.arrow_id, (float)x1, (float)y1, 0);
                bobjpool.set_localEulerAngles(ta.arrow_m.arrow_id, 0, 0, (float)ag);
            }
            
            if (ta.name != ta.pt.animal.name)
            {
                ta.name = ta.pt.animal.name;
                ta.arrow_m.label.text = ta.pt.animal.name;
            }

            if (!obj.activeInHierarchy)
                obj.SetActive(true);
        }
    }

    public void SetGuidArrow()
    {
        var bpme = BattlePlayers.me;
        if (bpme.attr.is_zhimang())
        {
            for (int i = 0; i < arrow_pools.Count; i++)
                arrow_pools[i].obj.SetActive(false);
            return;
        }

        if (BattlePlayers.players_list.Count <= 0 || Battle.is_newplayer_guide)
            return;

        for (int i = 0; (i < arrow_pools.Count) && (i <= BattlePlayers.players_list.Count); i++)
        {
            GameObject obj = obj = arrow_pools[i].obj;
            BattleAnimal pt = null;
            if (i == 0)
            {
                pt = BattlePlayers.Boss;
                if (pt == null || !BattlePlayers.BossIsActive())
                {
                    if (obj.activeInHierarchy)
                        obj.SetActive(false);
                    continue;
                }           
            }   
            else
                pt = get_rank_by_rank(i);

            

            var v = LuaHelper.GetMapManager().WorldToScreenPoint(pt.objt.position);
            if (pt.animal.guid == bpme.animal.guid)
            {
                if (obj.activeSelf)
                    obj.SetActive(false);
            }
            else if (!(v.x < -10 || v.x > screen_w_ + 10 || v.y < -10 || v.y > screen_h_ + 50))
            {
                if (obj.activeSelf)
                    obj.SetActive(false);
            }
            else
            {
                long dx = pt.animal.x - bpme.player.x;
                long dy = pt.animal.y - bpme.player.y;
                double angle = 0;
                if (dx == 0 && pt.animal.y >= bpme.animal.y)
                    angle = 180;
                else if (dx == 0 && pt.animal.y < bpme.animal.y)
                    angle = 0;
                else
                {
                    angle = Math.Atan(dy * 1.0 / dx) * 180 / 3.1415;
                    angle = (angle + 180) % 180;
                    if (dy < 0)
                        angle = angle + 180;
                }

                var maxLen_1 = Math.Abs(screen_w_ / 2 / Math.Cos(angle * 3.1415 / 180));
                var maxLen_2 = Math.Abs(screen_h_ / 2 / Math.Cos((angle + 90) * 3.1415 / 180));
                var min = Math.Min(maxLen_1 - Math.Abs(20 / Math.Cos(angle * 3.1415 / 180)), maxLen_2 - Math.Abs(20 / Math.Cos((angle + 90) * 3.1415 / 180)));
                angle = angle - 90;

                var x = min * dx / Math.Sqrt(dx * dx + dy * dy);
                var y = min * dy / Math.Sqrt(dx * dx + dy * dy);

                bobjpool.set_localEulerAngles(arrow_pools[i].objid, 0, 0, (float)(angle));
                bobjpool.set_localEulerAngles(arrow_pools[i].iconid, 0, 0, (float)(-angle));
                bobjpool.set_localPosition(arrow_pools[i].objid, (float)x, (float)y, 0);
 
                if (!obj.activeSelf)
                    obj.SetActive(true);
            }
        }
    }

    public void ShowBattleInsideMsg(BattleAnimalPlayer bp,t_battle_msg t_battle_msg)
    {
        GameObject go = GameObject.Instantiate(team_msg_item_);
        go.transform.parent = team_msg_panel_.transform;
        go.transform.localPosition = new Vector3(20, 0, 0);
        go.transform.localScale = Vector3.one;
        go.transform.localEulerAngles = Vector3.zero;
        go.GetComponent<UILabel>().text = "[F5DE0CFF]" + bp.player.name + "[-] : " + t_battle_msg.msg;
        go.SetActive(false);
        
        team_msg_info tmi = new team_msg_info();
        tmi.go = go;
        tmi.tm = 2;
        foreach (var item in teamer_arrows_)
        {
            if (item.Value.pt.animal.guid == bp.player.guid)
            {
                tmi.lb = item.Value.arrow_m.label;
                break;
            }
        }

        team_msg_queues_.Add(tmi);
    }

    public void UpdateBossInfo()
    {
        if (BattlePlayers.Boss == null)
        {
            if (boss_info_.obj.activeInHierarchy)
                boss_info_.obj.SetActive(false);
            return;
        }

        if (BattlePlayers.Boss.is_die)
        {
            if (boss_info_.obj.activeInHierarchy)
                boss_info_.obj.SetActive(false);
            return;
        }

        if (!boss_info_.obj.activeInHierarchy)
            boss_info_.obj.SetActive(true);

        var bp = BattlePlayers.Boss;
        var t_boss_attr = Config.get_t_boss_attr(bp.animal.camp);

        if (!boss_info_.init)
        {
            var t_role = Config.get_t_role(t_boss_attr.role_id);
            boss_info_.obj.transform.Find("avatar_inf/avatar").GetComponent<UISprite>().atlas = IconPanel.GetAltas(t_role.icon); 
            boss_info_.obj.transform.Find("avatar_inf/avatar").GetComponent<UISprite>().spriteName = t_role.icon;
            var t_toukuang = Config.get_t_toukuang(t_boss_attr.toukuang_id);
            boss_info_.objt.Find("avatar_inf/frame").GetComponent<UISprite>().atlas = IconPanel.GetAltas(t_toukuang.big_icon);
            boss_info_.objt.Find("avatar_inf/frame").GetComponent<UISprite>().spriteName = t_toukuang.big_icon;
            boss_info_.name.text = t_boss_attr.name;
            boss_info_.init = true;
        }

        var slider_value = bp.player.hp * 1.0f / bp.attr.max_hp();
        if (slider_value >= 1)
            slider_value = 1;
        else if (slider_value < 0)
            slider_value = 0;

        boss_info_.slider.value = slider_value;
        boss_info_.hplb.text = bp.player.hp + "/" + bp.attr.max_hp();

        if (Battle.is_newplayer_guide)
            boss_info_.timelb.text = "∞";
        else
        {
            var s = Mathf.CeilToInt(t_boss_attr.life_t - (BattlePlayers.zhen - bp.player.boss_birth_time) * BattlePlayers.TICK / 1000.0f);
            boss_info_.timelb.text = string.Format("{0:D2}:{1:D2}", Mathf.FloorToInt(s / 60.0f), s % 60);
        }
    }
    public void UpdateFD()
    {
        int ping_num = LuaHelper.GetNetManager().GetPing("BattleTcp");
        int fps_num = LuaHelper.GetGameManager().GetFPS();
        string ping_text = "";
        if (ping_num < 10)
            ping_text = "<10ms";
        else if (ping_num > 2000)
            ping_text = ">2000ms";
        else
            ping_text = ping_num + "ms";

        if (ping_num <= 80)
            ping_.text = "[00ff00]" + ping_text + "[-]";
        else if (ping_num > 150)
            ping_.text = "[ff0000]" + ping_text + "[-]";
        else
            ping_.text = "[ffff00]"+ping_text+"[-]";

        if (fps_num >= 20)
            fps_.text = "[00ff00]" + fps_num + "[-]";
        else if (fps_num < 15)
            fps_.text = "[ff0000]" + fps_num + "[-]";
        else
            fps_.text = "[ffff00]" + fps_num + "[-]";
    }
    public void CheckSkIcon()
    {
        if (BattlePlayers.me.player.skill_id == 0)
            jskill_icon_.spriteName = "";
    }
    public void UnNormalStateAnimation()
    {
        foreach (var item in un_normal_state_pools_)
        {
            if (item.Value.obj.activeSelf)
                item.Value.obj.SetActive(false);
        }

        foreach (var item in un_normal_states_time)
        {
            if (!un_normal_state_pools_[item.Key].obj.activeSelf)
                un_normal_state_pools_[item.Key].obj.SetActive(true);
        }

        AdjustUnNormalStatePos();

        List<int> dels = new List<int>();
        foreach (var item in unNormalStateList)
        {
            if (!item.Value.Complete(item.Value))
                item.Value.Execute(item.Value);

            if (item.Value.Complete(item.Value))
            {
                item.Value.Finish(item.Value);
                dels.Add(item.Key);
            }
        }

        for (int i = 0; i < dels.Count; i++)
            unNormalStateList.Remove(dels[i]);
    }

    public void AdjustUnNormalStatePos()
    {
        int index = 0;
        int total = un_normal_states_time.Count * 95;
        foreach (var item in un_normal_states_time)
        {
            var obj = un_normal_panel_.transform.Find(item.Key.ToString()).gameObject;
            var pos_x = index * 95 + 47.5f - total / 2.0f;
            var id = un_normal_state_pools_[item.Key].objid;
            bobjpool.set_localPosition(id, pos_x, 0, 0);
            un_normal_state_pools_[item.Key].obj.SetActive(true);
            index = index + 1;
        }
    }
    public void Animation()
    {
        if (pos_t_.length <= 0)
            return;
        List<int> dels = new List<int>();
        foreach (var item in pos_t_.paths)
        {
            var k = item.Key;
            var v = item.Value;
            if (v.isMulti)
            {
                bool allComplete = true;
                for (int i = v.ms_path.Count - 1; i >= 0; i--)
                {
                    var it = v.ms_path[i];
                    if (it.cur_t < it.total_t)
                    {
                        var t = it.cur_t / it.total_t;
                        var p = BattleOperation.BezierCurve(it.st_p, it.mid_p1, it.mid_p2, it.ed_p, t);
                        bobjpool.set_localPosition(it.dt.objid, p.x, p.y, p.z);
                        it.cur_t += Time.deltaTime / Time.timeScale;
                        allComplete = false;
                    }
                    else
                    {
                        bobjpool.set_localPosition(it.dt.objid, it.ed_p.x, it.ed_p.y, it.ed_p.z);
                        remove_type_obj(it.dt);
                        pos_t_.paths[k].ms_path.RemoveAt(i);
                    }
                }

                if (allComplete)
                {
                    if (pos_t_.funcs.ContainsKey(item.Key))
                        pos_t_.funcs[k].execute(pos_t_.funcs[k]);
                }
                dels.Add(k);
            }
            else
            {
                if (pos_t_.r_time[k] < v.s_path.total_t)
                {
                    var t = pos_t_.r_time[k] / v.s_path.total_t;
                    var p = BattleOperation.BezierCurve(v.s_path.st_p, v.s_path.mid_p1, v.s_path.mid_p2, v.s_path.ed_p, t);
                    bobjpool.set_localPosition(v.s_path.dt.objid, p.x, p.y, p.z);
                    pos_t_.r_time[k] += Time.deltaTime / Time.timeScale;
                }
                else
                {
                    bobjpool.set_localPosition(v.s_path.dt.objid, v.s_path.ed_p.x, v.s_path.ed_p.y, v.s_path.ed_p.z);
                    remove_type_obj(v.s_path.dt);
                    if (pos_t_.funcs.ContainsKey(k))
                        pos_t_.funcs[k].execute(pos_t_.funcs[k]);

                    dels.Add(k);
                }
            }
        }

        if (dels.Count > 0)
        {
            for (int i = 0; i < dels.Count; i++)
                pos_t_.paths.Remove(dels[i]);
        }
        dels.Clear();
        //几秒后删除
        foreach (var item in pos_t_.delays)
        {
            var k = item.Key;
            var v = item.Value;
            if (pos_t_.r_time[k] < v.time)
                pos_t_.r_time[k] += Time.deltaTime / Time.timeScale;
            else
            {
                if (v.fe != null)
                    v.fe();

                remove_type_obj(v.obj);
                dels.Add(k);
            }
        }

        if (dels.Count > 0)
        {
            for (int i = 0; i < dels.Count; i++)
                pos_t_.delays.Remove(dels[i]);
        }
    }
    public void remove_type_obj(resource_info go)
    {
        go.state = 0;
        go.obj.SetActive(false);
    }
    public resource_info get_type_obj(int typeid)
    {
        GameObject clone_obj = null;
        if (typeid == 1)
            clone_obj = effect_obj_;
        else if (typeid == 2)
            clone_obj = effect_skill_obj_;
        else if (typeid == 3)
            clone_obj = effect_explode_obj_;
        else if (typeid == 4)
            clone_obj = effect_ability_obj_;

        if (!resource_pools.ContainsKey(typeid))
        {
            resource_pools.Add(typeid, new Dictionary<int, resource_info>());
            var obj = LuaHelper.Instantiate(clone_obj);
            var objid = bobjpool.add(obj);
            var objt = obj.transform;
            obj.transform.parent = effect_panel_.transform;
            bobjpool.set_localPosition(objid, 0, 0, 0);
            bobjpool.set_localScale(objid, 1, 1, 1);
            if (!resource_pools[typeid].ContainsKey(objid))
                resource_pools[typeid].Add(objid, new resource_info() { objid = objid, objt = objt, obj = obj, state = 1, typeid = typeid });
            obj.SetActive(true);
            return resource_pools[typeid][objid];
        }
        else
        {
            int check_free_key = -1;
            foreach (var item in resource_pools[typeid])
            {
                if (item.Value.state == 0)
                {
                    check_free_key = item.Key;
                    break;
                }
                   
            }

            if (check_free_key >= 0)
            {
                resource_pools[typeid][check_free_key].state = 1;
                var objid = resource_pools[typeid][check_free_key].objid;
                bobjpool.set_localScale(objid, 1, 1, 1);
                resource_pools[typeid][check_free_key].obj.SetActive(true);
                return resource_pools[typeid][check_free_key];
            }
            else
            {
                GameObject obj = LuaHelper.Instantiate(clone_obj);
                int objid = bobjpool.add(obj);
                var objt = obj.transform;
                obj.transform.parent = effect_panel_.transform;
                bobjpool.set_localPosition(objid, 0, 0, 0);
                bobjpool.set_localScale(objid, 1, 1, 1);
                resource_pools[typeid][objid] = new resource_info() { objid = objid, objt = objt, obj = obj, state = 1, typeid = typeid };
                obj.SetActive(true);
                return resource_pools[typeid][objid];
            }
        }
    }
    public void ReplaceSkill()
    {
        if (BattlePlayers.me == null)
            return;

        var ss = spare_panel_.transform.GetComponent<SphereCollider>();
        ss.enabled = false;
        var info = get_type_obj(3);
        Vector3 v = spare_panel_.transform.TransformPoint(info.objt.localPosition);
        Vector3 tp = info.obj.transform.InverseTransformPoint(v);
        bobjpool.set_localPosition(info.objid, tp.x, tp.y, tp.z);
        info.obj.SetActive(true);
        delay_unit d = new delay_unit() { time = 0.5, obj = info, fe = null };
        pos_t_.delays.Add(pos_t_.length, d);
        pos_t_.r_time.Add(pos_t_.length, 0);
        pos_t_.length = pos_t_.length + 1;

        pos_t_.paths[pos_t_.length] = new effect_path() { isMulti = true, ms_path = new List<single_path>() };
        for (int i = 1; i <= 3; i++)
        {
            var dt = get_type_obj(2);
            v = spare_panel_.transform.TransformPoint(dt.objt.localPosition);
            var beginPos = dt.objt.InverseTransformPoint(v);
            bobjpool.set_localPosition(dt.objid, beginPos.x, beginPos.y, beginPos.z);
            v = jskill_panel_.transform.TransformPoint(dt.objt.localPosition);
            var finalPos = dt.objt.InverseTransformPoint(v);

            var x_1 = BattlePanel.Random(beginPos.x - 100, finalPos.x + 100);
            var x_2 = BattlePanel.Random(beginPos.x - 100, finalPos.x + 100);
            var y_1 = BattlePanel.Random(beginPos.y - 100, finalPos.y + 100);
            var y_2 = BattlePanel.Random(beginPos.y - 100, finalPos.y + 100);
            var p1 = new Vector3(x_1, y_1, 2);
            var p2 = new Vector3(x_2, y_2, 2);
            pos_t_.paths[pos_t_.length].ms_path.Add(new single_path() { st_p = beginPos,  ed_p =finalPos,  mid_p1 = p1, mid_p2 = p2,  cur_t = 0,  total_t = 0.5f, dt = dt});
            var tn = new task_unit();
            tn.execute = delegate (task_unit t)
            {
                if (spare_panel_.activeSelf)
                {
                    spare_panel_.SetActive(false);
                    spare_panel_.transform.GetComponent<SphereCollider>().enabled = true;
                }

                //添加一个爆炸特效
                var ob = get_type_obj(3);
                v = jskill_panel_.transform.TransformPoint(dt.objt.localPosition);
                tp = dt.objt.InverseTransformPoint(v);
                bobjpool.set_localPosition(dt.objid, tp.x, tp.y, tp.z);
                bobjpool.set_localScale(dt.objid, 1.2f, 1.2f, 1.2f);
                Battle.send_change_skill();

                pos_t_.delays[pos_t_.length] = new delay_unit() { time = 0.5, obj = dt, fe = null };
                pos_t_.r_time[pos_t_.length] = 0;
                pos_t_.length = pos_t_.length + 1;
            };
            pos_t_.funcs[pos_t_.length] = tn;

                
                

            pos_t_.r_time[pos_t_.length] = 0;
            pos_t_.length = pos_t_.length + 1;
        }
    }

    private static float Random(float st, float ed)
    {
        if (ed >= st)
            return UnityEngine.Random.Range(st, ed);
        else
            return UnityEngine.Random.Range(ed, st);
    }
    public void InitDetailPanel(GameObject go)
    {
        if (end_time_ > 0)
            return;
        LuaHelper.GetTimerManager().RemoveTimer("BattlePanel");
        LuaHelper.GetTimerManager().RemoveTimer("DuanUp");
        LuaHelper.GetTimerManager().RemoveTimer("StarHit");
        cup_panel_.gameObject.SetActive(false);
        end_panel_.SetActive(false);
        result_panel_.SetActive(false);

        var main_panel = detail_panel_.Find("main_panel");
        var avatar_inf = main_panel.Find("avatar_inf");
        var name = main_panel.Find("name").GetComponent<UILabel>();
        var role_name = main_panel.Find("role_name").GetComponent<UILabel>();
        var rank = main_panel.Find("rank").GetComponent<UILabel>();
        var kill = main_panel.Find("kill").GetComponent<UILabel>();
        var death = main_panel.Find("death").GetComponent<UILabel>();
        var exp_add = main_panel.Find("exp").GetComponent<UILabel>();
        var battle_gold = main_panel.Find("battle_gold").GetComponent<UILabel>();
        var score = main_panel.Find("score").GetComponent<UILabel>();
        var score_title = main_panel.Find("score/title").GetComponent<UILabel>();
        var tip = main_panel.Find("tip").GetComponent<UILabel>(); 
        var share_code = main_panel.Find("z_code").GetComponent<UITexture>();
        share_code.mainTexture = LuaHelper.GetShareManager().ShareZcode(self.share_url, 512, 512);
        int rank_num = 0;
        int self_rank_num = get_rank_by_guid(self.guid);
        var player = BattlePlayers.me.player;
        if (BattlePlayers.battle_type == 1)
        {
            rank_num = get_rank_by_team_id(BattlePlayers.me.player.camp);
            if (rank_num == 1)
                share_type_ = "battle_mvp";
            else if (rank_num >= 2 && rank_num <= 4)
                share_type_ = "battle_general";
        }
        else
        {
            rank_num = self_rank_num;
            if (rank_num == 1)
                share_type_ = "battle_mvp";
            else if (rank_num >= 2 && rank_num <= 10)
                share_type_ = "battle_better";
            else if (rank_num >= 11 && rank_num <= 20)
                share_type_ = "battle_general";
        }

        score.text = player.score.ToString();
        var gold = 31 - self_rank_num;
        gold = gold + Old_Self_Data.extra_gold_add;
        bool is_double = false;
        if (Battle.is_online)
        {
            if (Old_Self_Data.box_zd_num < 3)
            {
                gold = gold * 2;
                is_double = true;
            }
        }
        else
        {
            if (self_player_level < 3 && Old_Self_Data.box_zd_num < 3)
            {
                gold = gold * 2;
                is_double = true;
            }
        }

        if (Old_Self_Data.battle_golds + gold > Old_Self_Data.level_max_golds)
            gold = Old_Self_Data.level_max_golds - Old_Self_Data.battle_golds;

        if (gold < 0)
            gold = 0;

        var gold_text = "";
        if(gold == 0)
            gold_text = self.font_color[4]+Config.get_t_script_str("BattlePanel_005");
        else if (gold > 0)
        {
            if (is_double)
                gold_text = "[00ff00]" + gold + "[-][ff8a00]（"+Config.get_t_script_str("BattlePanel_006") + "）";
            else
                gold_text = "[00ff00]" + gold;
        }

        AvaIconPanel.ModifyAvatar(avatar_inf, self.player.avatar_on,"", self.player.toukuang_on, self.player.sex);
        name.text = self.player.name;
        var color = self.GetVipNameColor();
        IconPanel.InitVipLabel(name, color);
        role_name.text = Config.get_t_role(player.role_id).name;
        IconPanel.InitQualityLabel(role_name, Config.get_t_role(player.role_id).color);
        rank.text = rank_num.ToString();
        kill.text = player.sha.ToString();
        death.text = player.die.ToString();
        exp_add.text = "[00ff00]" + (31 - self_rank_num);

        if (self.pre_battle_num < 3)
        {
            string content = string.Format(Config.get_t_script_str("BattlePanel_007"), (2 - self.pre_battle_num));
            tip.text = content; //"今日双倍加成剩余 [00ff00]" + (2 - self.pre_battle_num) + "[-] 次";
            exp_add.text = "[00ff00]" + ((31 - self_rank_num) * 2) + "[ff8a00]（"+Config.get_t_script_str("BattlePanel_006") +"）";
        }
        else
            tip.text = "";

        if (!Battle.is_online && self_player_level >= 3)
        {
            tip.text = "";
            exp_add.text = "[ff0000]"+Config.get_t_script_str("BattlePanel_008");
        }
        battle_gold.text = gold_text;
        Util.CallMethod("GUIRoot", "UIEffectScalePos", main_panel.gameObject, true, 1);
        detail_panel_.gameObject.SetActive(true);
    }

    public void InitCupPanel()
    {
        int rank_num = 0;
        if (BattlePlayers.battle_type == 1)
            rank_num = ScoreTeamRankByTeamID[BattlePlayers.me.player.camp];
        else
            rank_num = get_rank_by_guid(self.guid);
        RefreshCupShow(rank_num, BattlePlayers.me.player.cup);
    }

    public void RefreshCupShow(int rank,int cup)
    {
        var cur_cup = Config.get_t_cup(cup);
        var up_num = 0;
        var down_num = 0;            
        var tsb_num = 0;
        var team_socre = 0;
        if (BattlePlayers.battle_type == 1)
        {
            up_num = Config.get_t_cup(cup).tsb;
            down_num = Config.get_t_cup(cup).tjb;
            tsb_num = Config.get_t_cup(cup).tsbnum;
            team_socre = BattlePlayers.me.player.score;
        }
        else
        {
            up_num = Config.get_t_cup(cup).sb;
            down_num = Config.get_t_cup(cup).jb;
        }

        var cup_view = cup_panel_.Find("main_panel/cup_view");
        var pre_cup = cup_view.Find("pre_cup");
        if (rank > down_num && down_num > 0)
        {
            if (cur_cup.max_star > 0)
            {
                if (cur_cup.star > 0)
                {
                    pre_cup.Find(cur_cup.star.ToString()).GetComponent<UISprite>().spriteName = "star_bg01";
                    var down_effect = pre_cup.Find(cur_cup.star + "/effect").gameObject;
                    down_effect.SetActive(true);
                    GameObject.Destroy(down_effect, 1);
                    PlaySound("bad");
                }
                else
                {
                    ShowCup(cup, cup - 1);
                    PlaySound("rankdown");
                }
            }
            else if (cur_cup.max_star == 0)
            {
                if (cup > cur_cup.id)
                {
                    TopCupShow(pre_cup, false, cup - cur_cup.id - 1);
                    PlaySound("rankdown");
                }
                else
                {
                    ShowCup(cup, cup - 1);
                    PlaySound("rankdown");
                }
            }
        }
        else if (rank <= up_num)
        {
            if ((BattlePlayers.battle_type == 1 && team_socre >= tsb_num) || BattlePlayers.battle_type != 1)
            {
                if ((cur_cup.star >= cur_cup.max_star && cur_cup.max_star > 0))
                {
                    ShowCup(cup, cup + 1);
                    PlaySound("rankup");
                    LuaHelper.GetTimerManager().AddTimer("DuanUp", DuanUp, 0.4f);
                }
                else
                {
                    if (cur_cup.max_star == 0)
                    {
                        TopCupShow(pre_cup, true, cup - cur_cup.id + 1);
                        PlaySound("rankup");
                    }
                    else if (cur_cup.max_star > 0)
                    {
                        var star_res = pre_cup.Find("star_res");
                        var star_t = LuaHelper.Instantiate(star_res.gameObject);
                        star_t.transform.parent = pre_cup;
                        star_t.transform.localScale = Vector3.one;
                        star_t.transform.localPosition = star_res.localPosition;
                        star_t.name = "star_up";
                        star_t.SetActive(true);
                        PlaySound("star_fly");
                        star_t.transform.GetComponent<Animator>().enabled = true;               
                        LuaHelper.GetTweenManager().Add_Tween_Postion(star_t, 0.6f, star_t.transform.localPosition, pre_cup.Find((cur_cup.star + 1).ToString()).transform.localPosition, 3, 0.6f);
                        LuaHelper.GetTimerManager().AddTimer("StarHit", StarHit, 1.2f);   
                    }
                }
            }
        }
    }

    public void DuanUp()
    {
        Util.CallMethod("GUIRoot", "ShakePanel", 0.5f, cup_panel_.Find("main_panel"));
        cup_panel_.Find("main_panel/effect").gameObject.SetActive(true);
    }
    public void StarHit()
    {
        var pre_cup = cup_panel_.Find("main_panel/cup_view/pre_cup");
        pre_cup.Find("star_up").GetComponent<Animator>().enabled = false;
        pre_cup.Find("star_up").localEulerAngles = pre_cup.Find((Config.get_t_cup(BattlePlayers.me.player.cup).star + 1).ToString()).localEulerAngles;
        Util.CallMethod("GUIRoot", "ShakePanel", 0.5f,cup_panel_.Find("main_panel"));
        PlaySound("star_hit");
        cup_panel_.Find("main_panel/effect").gameObject.SetActive(true);
    }

    public void ShowCup(int pre_cup,int next_cup)
    {
        var cup_view = cup_panel_.Find("main_panel/cup_view");
        if (cup_view.Find("pre_cup") != null)
            cup_view.Find("pre_cup").gameObject.SetActive(false);

        var cup_pre = IconPanel.GetCup(pre_cup, false, true);
        cup_pre.transform.parent = cup_view;
        cup_pre.transform.localScale = Vector3.one;
        cup_pre.transform.localPosition = new Vector3(0, -100, 0);

        var cup_next = IconPanel.GetCup(next_cup, false, true);
        cup_next.transform.parent = cup_view;
        cup_next.transform.localScale = Vector3.one;
        if (pre_cup > next_cup)
        {
            cup_next.transform.localPosition = new Vector3(-270, -100, 0);
            LuaHelper.GetTweenManager().Add_Tween_Postion(cup_pre, 0.4f, cup_pre.transform.localPosition, new Vector3(270, -100, 0));
            LuaHelper.GetTweenManager().Add_Tween_Postion(cup_next, 0.4f, cup_next.transform.localPosition, new Vector3(0, -100, 0));
        }
        else
        {
            cup_next.transform.localPosition = new Vector3(270, -100, 0);
            LuaHelper.GetTweenManager().Add_Tween_Postion(cup_pre, 0.4f, cup_pre.transform.localPosition, new Vector3(-270, -100, 0));
            LuaHelper.GetTweenManager().Add_Tween_Postion(cup_next, 0.4f, cup_next.transform.localPosition, new Vector3(0, -100, 0));
        }
        cup_pre.SetActive(true);
        cup_next.SetActive(true);
    }

    public void TopCupShow(Transform pre_cup,bool is_up,int num)
    {
        var lv_view = pre_cup.Find("lv/lv_view");
        var pre_label = lv_view.Find("Label");
        var cur_label = LuaHelper.Instantiate(pre_label.gameObject);

        cur_label.transform.parent = lv_view;
        cur_label.transform.localScale = Vector3.one;
        cur_label.transform.GetComponent<UILabel>().text = num.ToString();
        if (is_up)
        {
            cur_label.transform.localPosition = new Vector3(0, 20, 0);
            LuaHelper.GetTweenManager().Add_Tween_Postion(pre_label.gameObject, 0.4f, pre_label.transform.localPosition, new Vector3(0, -20, 0));
            LuaHelper.GetTweenManager().Add_Tween_Postion(cur_label, 0.4f, cur_label.transform.localPosition, new Vector3(0, 0, 0));
        }
        else
        {
            cur_label.transform.localPosition = new Vector3(0, -20, 0);
            LuaHelper.GetTweenManager().Add_Tween_Postion(pre_label.gameObject, 0.4f, pre_label.transform.localPosition, new Vector3(0, 20, 0));
            LuaHelper.GetTweenManager().Add_Tween_Postion(cur_label, 0.4f, cur_label.transform.localPosition, new Vector3(0, 0, 0));
        }
        cur_label.SetActive(true);
    }
    public void Share(GameObject go)
    {
        string s = @" 
                client_key = { }
                client_key[1] = 2
                client_key[2] = " + share_type_;
        LuaInterface.LuaTable tt = AppFacade._instance.LuaManager.GetLuaTable(s, "client_key");
        Util.CallMethod("GUIRoot", "ShowGUI", "SharePanel", tt);
    }

    public void CountPlayerCup(GameObject go)
    {
        if (!Battle.is_online && self_player_level >= 3)
        {
            InitDetailPanel(null);
            return;
        }

        if (end_time_ > 0)
            return;

        detail_panel_.gameObject.SetActive(false);
        end_panel_.SetActive(false);
        result_panel_.SetActive(false);
        var player = BattlePlayers.me.player;
        if (BattlePlayers.battle_type == 1)
        {
            var rank_num = ScoreTeamRankByTeamID[player.camp];
            var socre = player.score;
            var up_num = Config.get_t_cup(player.cup).tsb;
            var down_num = Config.get_t_cup(player.cup).tjb;
            var sb_num = Config.get_t_cup(player.cup).tsbnum;
            if (rank_num <= up_num && socre >= sb_num)
                share_type_ = "cup";
            else if (rank_num > down_num && down_num > 0)
                share_type_ = "cup_down";
            else if (down_num == 0)
                share_type_ = "cup_normal";
        }
        else
        {
            var rank_num = get_rank_by_guid(self.guid);
            var up_num = Config.get_t_cup(player.cup).sb;
            var down_num = Config.get_t_cup(player.cup).jb;
            if (rank_num <= up_num)
                share_type_ = "cup";
            else if (rank_num > down_num && down_num > 0)
                share_type_ = "cup_down";
            else if (down_num == 0)
                share_type_ = "cup_normal";
        }

        SetPlayerCup(player.cup);
        LuaHelper.GetTweenManager().Add_Tween_Scale(cup_panel_.Find("main_panel").gameObject, 0.2f, Vector3.zero, Vector3.one);
        cup_panel_.gameObject.SetActive(true);
        LuaHelper.GetTimerManager().AddTimer("BattlePanel", InitCupPanel, 0.3f);
    }

    public GameObject SetPlayerCup(int id)
    {
        var cup_view = cup_panel_.Find("main_panel/cup_view");
        var cup_t = IconPanel.GetCup(id, false, true);
        cup_t.transform.parent = cup_view;
        cup_t.transform.localScale = Vector3.one;
        cup_t.transform.localPosition = new Vector3(0, -100, 0);
        cup_t.name = "pre_cup";
        cup_t.SetActive(true);
        return cup_t;
    }

    public void CloseTalentDetailInfo(GameObject go)
    {
        if (talent_panel_.activeSelf)
            talent_panel_.SetActive(false);
    }

    public void GoBackHall(GameObject go)
    {
        if (Battle.is_newplayer_guide)
        {

            string ss = string.Format("client_key = {3} client_key[1] = '{0}' client_key[2] = '{1}' client_key[3] = Battle.hreturn client_key[4] = '{2}'", Config.get_t_script_str("BattlePanel_009"), Config.get_t_script_str("BattlePanel_010"), Config.get_t_script_str("BattlePanel_011"),"{}");
            LuaInterface.LuaTable tt = AppFacade._instance.LuaManager.GetLuaTable(ss, "client_key");
            Util.CallMethod("GUIRoot", "ShowGUI", "SelectPanel", tt);
            Battle.send_offline_state(0);
            return;
        }
       
        string s = string.Format("client_key = {3}  client_key[1] = '{0}' client_key[2] = '{1}' client_key[3] = function() State.ChangeState(State.state.ss_hall) end client_key[4] = '{2}' client_key[5] = function() GUIRoot.HideGUI('SelectPanel') end ", Config.get_t_script_str("BattlePanel_012"), Config.get_t_script_str("BattlePanel_010"), Config.get_t_script_str("BattlePanel_011"),"{}");
        LuaInterface.LuaTable t = AppFacade._instance.LuaManager.GetLuaTable(s, "client_key");
        Util.CallMethod("GUIRoot", "ShowGUI", "SelectPanel", t);
        Battle.send_offline_state(0);
    }
    public void ShowJobLinesDetailInfo(GameObject go)
    {
        if (talent_panel_.activeSelf)
            talent_panel_.SetActive(false);
        else
        {
            talent_panel_.SetActive(true);
            OpenTalentSXPage(null);
        }
    }
    public void OpenTalentSXPage(GameObject go)
    {
        var sx_label = talent_sx_btn_.transform.Find("name").GetComponent<UILabel>();
        sx_label.text = "[E8FCFF]"+Config.get_t_script_str("BattlePanel_013") +"[-]";
        if (!talent_sx_panel_.activeSelf)
            talent_sx_panel_.SetActive(true);

        var role = self.get_role_id(BattlePlayers.me.player.role_id);
        var role_temp = Config.get_t_role(BattlePlayers.me.player.role_id);
        var chParent = talent_sx_panel_.transform.Find("character");
        GameObject role_item = null;

        if (chParent.Find("headbg") == null)
        {
            role_item = IconPanel.GetIcon("role_res", role_temp.icon, role_temp.color);
            role_item.transform.parent = chParent;
            role_item.name = "headbg";
        }
        else
            role_item = chParent.Find("headbg").gameObject;

        var star_res = chParent.transform.Find("star_res").gameObject;
        var total = role.level * 12;
        for (int i = 1; i <= role.level; i++)
        {
            var star_obj = LuaHelper.Instantiate(star_res);
            star_obj.transform.parent = role_item.transform;
            star_obj.transform.localPosition = new Vector3((i - 1) * 12 + 6 - total / 2, -32, 0);
            star_obj.transform.localScale = Vector3.one;
            star_obj.SetActive(true);
        }

        role_item.GetComponent<BoxCollider>().enabled = false;
        role_item.transform.localPosition = new Vector3(-240, 0, 0);
        role_item.transform.localScale = Vector3.one;
        role_item.gameObject.SetActive(true);
        var sexSp = chParent.transform.Find("sex").GetComponent<UISprite>();
        if (role_temp.sex == 0)
            sexSp.spriteName = "boy_icon";
        else if (role_temp.sex == 1)
            sexSp.spriteName = "girl_icon";

        var nameLabel = talent_sx_panel_.transform.Find("character/name").GetComponent<UILabel>();
        nameLabel.text = role_temp.name;
        IconPanel.InitQualityLabel(nameLabel, role_temp.color % 10);
        var descLabel = talent_sx_panel_.transform.Find("character/desc").GetComponent<UILabel>();
        descLabel.text = role_temp.desc;
        var pt = BattlePlayers.me;

        var JobInfo = talent_sx_panel_.transform.Find("JobInfo");

        if (JobInfo.childCount > 0)
        {
            for (int i = 0; i < JobInfo.childCount; i++)
                GameObject.Destroy(JobInfo.GetChild(i).gameObject);
        }
        var total_len = pt.player.talent_id.Count * 90;
        for (int i = 0; i < pt.player.talent_id.Count; i++)
        {
            var t_talent = Config.get_t_talent(pt.player.talent_id[i]);
            var ins = LuaHelper.Instantiate(talent_sx_preb_);
            ins.transform.parent = JobInfo;
            ins.transform.localScale = Vector3.one;
            ins.transform.localPosition = new Vector3((i - 1) * 90 + 45 - total_len / 2, -7, 0); 
            ins.transform.Find("icon_p/iconbg").GetComponent<UISprite>().atlas = IconPanel.GetAltas(t_talent.icon);
            ins.transform.Find("icon_p/iconbg").GetComponent<UISprite>().spriteName = t_talent.icon;
            ins.transform.Find("level").GetComponent<UILabel>().text = pt.player.talent_level[i].ToString();
            RegisterOnPress(ins.transform.Find("icon_p").gameObject, ShowCurrentTalentInfo);
            ins.transform.Find("icon_p").name = t_talent.id.ToString();
            ins.SetActive(true);
        }

        var attr_obj = talent_sx_panel_.transform.Find("attrInfos");
        for (int i = 1; i <= 7; i++)
        {
            var cur_attr = attr_obj.Find("attr_" + i);
            var attrLabel = cur_attr.Find("valueLabel").GetComponent<UILabel>();
            int dvalue = 0;
            if (i == 1)
            {
                dvalue = Mathf.CeilToInt(pt.attr.max_hp() - pt.attr.init_max_hp());
                attrLabel.text = "[55DD77]" + pt.attr.max_hp() + "[-] ([98E7EE]" + pt.attr.init_max_hp() + "[-]+[55DD77]" + dvalue + "[-])";
            }
            else if (i == 2)
            {
                dvalue = Mathf.CeilToInt(pt.attr.atk() - pt.attr.init_atk());
                attrLabel.text = "[55DD77]" + pt.attr.atk() + "[-] ([98E7EE]" + pt.attr.init_atk() + "[-]+[55DD77]" + dvalue + "[-])";
            }
            else if (i == 3)
            {
                dvalue = Mathf.CeilToInt(pt.attr.def() - pt.attr.init_def());
                attrLabel.text = "[55DD77]" + pt.attr.def() + "[-] ([98E7EE]" + pt.attr.init_def() + "[-]+[55DD77]" + dvalue + "[-]) |[55DD77]" + pt.attr.defper() + "%[-]";
            }
            else if (i == 4)
            {
                dvalue = Mathf.CeilToInt((pt.attr.range() - pt.attr.init_range()) / 100);
                attrLabel.text = "[55DD77]" + Mathf.CeilToInt(pt.attr.range() / 100.0f) + "[-] ([98E7EE]" + Mathf.CeilToInt(pt.attr.init_range() / 100.0f) + "[-]+[55DD77]" + dvalue + "[-])";
            }
            else if (i == 5)
            {
                dvalue = Mathf.CeilToInt((pt.attr.speed() - pt.attr.init_speed()) / 100.0f);

                if (dvalue >= 0)
                    attrLabel.text = "[55DD77]" + Mathf.CeilToInt(pt.attr.speed() / 100.0f) + "[-] ([98E7EE]" + Mathf.CeilToInt(pt.attr.init_speed() / 100.0f) + "[-]+[55DD77]" + Mathf.Abs(dvalue) + "[-])";
                else
                    attrLabel.text = "[55DD77]" + Mathf.CeilToInt(pt.attr.speed() / 100.0f) + "[-] ([98E7EE]" + Mathf.CeilToInt(pt.attr.init_speed() / 100.0f) + "[-][ff0000]" + dvalue + "[-])";
            }
            else if (i == 6)
            {
                dvalue = pt.attr_value[9];
                attrLabel.text = "[55DD77]" + dvalue + "%[-]";
            }
            else if (i == 7)
            {
                attrLabel.text = "[55DD77]" + Mathf.CeilToInt(pt.attr_value[10]) + "[-]";
            }
        }
    }
    public void ShowCurrentTalentInfo(GameObject obj, bool state)
    {
        if (state)
        {
            var talent_id = Convert.ToInt32(obj.name);
            var t_talent = Config.get_t_talent(talent_id);
            var v = obj.transform.TransformPoint(talent_tips_panel_.transform.localPosition);
            var tp = talent_tips_panel_.transform.InverseTransformPoint(v);
            talent_tips_panel_.transform.localPosition = new Vector3(tp.x, tp.y + 60, 0);
            float level = 1;
            for (int i = 0; i < BattlePlayers.me.player.talent_id.Count; i++)
            {
                if (BattlePlayers.me.player.talent_id[i] == talent_id)
                {
                    level = BattlePlayers.me.player.talent_level[i];
                    break;
                }
            }
            if (t_talent.id == 3)
                level = level * t_talent.param3 / 100;
            else
                level = level * t_talent.param3;

            talent_tips_text_.text = t_talent.desc2.Replace("{N2}", Mathf.CeilToInt(level).ToString());
            talent_tips_panel_.SetActive(true);
        }
        else
            talent_tips_panel_.SetActive(false);
    }

    public void ShowTalentPanel()
    {
        if (!talent_show_panel_.gameObject.activeSelf)
        {
            talent_show_panel_.gameObject.SetActive(true);
            talent_title_panel_.gameObject.SetActive(false);
            LuaHelper.GetTimerManager().AddRepeatTimer("RollObj", RollObj, 0.02f, 0.05f);   
        }
    }
    public void ShowRankPanel()
    {
        if (!rankObj_.activeSelf)
            rankObj_.SetActive(true);
        if (!mb_panel_.activeSelf)
        {
            mb_panel_.SetActive(true);
        }       
    }

    public void SMSG_PLAYER_LOOK(s_net_message message)
    {

    }
    public void SetQuality()
    {
        if (self.quality != quality_id_ || eff_quatity_ != self.eff_quatity)
        {
            self.quality = quality_id_;
            self.eff_quatity = eff_quatity_;
            QualitySettings.SetQualityLevel(self.quality, true);
            self.save_set();
        }

        if (self.quality != 0)
        {
            for (int ll = 0; ll < BattlePlayers.players_list.Count; ll++)
            {
                var bp = BattlePlayers.players_list[ll];
                var ts = bp.posobjt;
                if (bp.shadow_res != null)
                    bp.shadow_res.gameObject.SetActive(false);
            }

            for (int ll = 0; ll < BattlePlayers.penguin_list.Count; ll++)
            {
                var bp = BattlePlayers.penguin_list[ll];
                var ts = bp.posobjt;
                if (bp.shadow_res != null)
                    bp.shadow_res.gameObject.SetActive(false);
            }
        }
        else
        {
            for (int ll = 0; ll < BattlePlayers.players_list.Count; ll++)
            {
                var bp = BattlePlayers.players_list[ll];
                var ts = bp.posobjt;
                if (bp.shadow_res != null)
                    bp.shadow_res.gameObject.SetActive(true);
            }

            for (int ll = 0; ll < BattlePlayers.penguin_list.Count; ll++)
            {
                var bp = BattlePlayers.penguin_list[ll];
                var ts = bp.posobjt;
                if (bp.shadow_res != null)
                    bp.shadow_res.gameObject.SetActive(true);
            }
        }
        quality_panel_.gameObject.SetActive(false);
    }

    public void ShowQuality(int level,int eff_quality)
    {
        quality_id_ = level;
        eff_quatity_ = eff_quality;

        low_btn_.Find("tip").gameObject.SetActive(false);
        high_btn_.Find("tip").gameObject.SetActive(false);
        mid_btn_.Find("tip").gameObject.SetActive(false);
        normal_eff_.Find("tip").gameObject.SetActive(false);
        high_eff_.Find("tip").gameObject.SetActive(false);

        if (quality_id_ == 0)
            low_btn_.Find("tip").gameObject.SetActive(true);
        else if (quality_id_ == 1)
            mid_btn_.Find("tip").gameObject.SetActive(true);
        else if (quality_id_ == 2)
            high_btn_.Find("tip").gameObject.SetActive(true);

        if(eff_quatity_ == 0)
            normal_eff_.Find("tip").gameObject.SetActive(true);
        else if(eff_quatity_ == 1)
            high_eff_.Find("tip").gameObject.SetActive(true);
    }

    public void clear_min_pro()
    {
        foreach (var item in min_pro_map_)
        {
            var mip = item.Value;
            BattlePlayers.bobjpool.remove(mip.objid);
            GameObject.Destroy(mip.obj);
        }
        min_pro_map_.Clear();
    }
    protected override void OnClick(GameObject go)
    {
        
        base.OnClick(go);
        if (go.name == "sskill")
        {
            if (BattlePlayers.me != null)
            {
                var t_skill = Config.get_t_skill(300101);
                if (BattlePlayers.me.player.cost >= BattlePlayers.POWERUP * t_skill.cost || BattlePlayers.me.player.is_xueren)
                {
                    if (Battle.is_newplayer_guide && !BattlePlayers.me.player.is_xueren)
                    {
                        skill_4_mask_obj.SetActive(true);
                    }                           
                    Battle.send_release(300101, 0, 0, 0, 0);                  
                }                  
                else
                {
                    string s = @"client_msg = {'"+Config.get_t_script_str("BattlePanel_014") +"'}";
                    var msg = AppFacade._instance.LuaManager.GetLuaTable(s, "client_msg");
                    Util.CallMethod("GUIRoot", "ShowGUI", "MessagePanel", msg);
                }
            }
        }
        else if (go.name == "sskill_2")
        {
            if (BattlePlayers.me != null)
            {
                var t_skill = Config.get_t_skill(300201);
                if (BattlePlayers.me.player.cost >= BattlePlayers.POWERUP * t_skill.cost)
                    Battle.send_release(300201, 0, 0, 0, 0);
                else
                {
                    string s = @"client_msg = {'"+ Config.get_t_script_str("BattlePanel_014") + "'}";
                    var msg = AppFacade._instance.LuaManager.GetLuaTable(s, "client_msg");
                    Util.CallMethod("GUIRoot", "ShowGUI", "MessagePanel", msg);
                }
            }
        }
        else if (go.name == "thskill")
        {
            ReplaceSkill();
        }
        else if (go.name == "killRankBtn")
        {
            rank_type_ = 2;
            if (BattlePlayers.battle_type == 1)
                kill_label.text = "[D0A851]"+ Config.get_t_script_str("BattlePanel_001");
            else
                kill_label.text = "[D0A851]" + Config.get_t_script_str("BattlePanel_002");
            score_label.text = "[B5F0EC]" + Config.get_t_script_str("BattlePanel_003");
            RefreshRankUI();
        }
        else if (go.name == "scoreRankBtn")
        {
            rank_type_ = 1;
            if (BattlePlayers.battle_type == 1)
                kill_label.text = "[B5F0EC]" + Config.get_t_script_str("BattlePanel_001");
            else
                kill_label.text = "[B5F0EC]" + Config.get_t_script_str("BattlePanel_002");
            score_label.text = "[D0A851]" + Config.get_t_script_str("BattlePanel_003");
            RefreshRankUI();
        }
        else if (go.name == "picQua")
        {
            ShowQuality(self.quality, self.eff_quatity);
            quality_panel_.gameObject.SetActive(true);
        }
        else if (go.name == "close_btn")
            quality_panel_.gameObject.SetActive(false);
        else if (go.name == "low")
            ShowQuality(0, self.eff_quatity);
        else if (go.name == "mid")
            ShowQuality(1, self.eff_quatity);
        else if (go.name == "high")
            ShowQuality(2, self.eff_quatity);
        else if (go.name == "normal_eff")
            ShowQuality(self.quality, 0);
        else if (go.name == "high_eff")
            ShowQuality(self.quality, 1);
        else if (go.name == "quality_ok_btn")
            SetQuality();
        else if (go.name == "guide_ok_btn")
            NewPlayerGuide.CMSG_GUIDE();
        else if (go.name == "skip_close_btn")
            guide_skip_panel_.gameObject.SetActive(false);
        else if (go.name == "friend")
        {
            string guid = go.transform.GetChild(0).name;
            Util.CallMethod("NoticePanel", "InitAddPanel", guid);
        }
        else if (go.name == "black_list")
        {
            string guid = go.transform.GetChild(0).name;
            if (!(Convert.ToUInt64(guid) < 100))
            {
                string s = @" client_player = {}
                client_player.guid = " + guid;
                var player = AppFacade._instance.LuaManager.GetLuaTable(s, "client_player");
                Util.CallMethod("FriendPanel", "AddPlayerBlack", player);
            }
            else
            {
                string s = @"client_msg = {'"+Config.get_t_script_str("BattlePanel_015") +"'}";
                var msg = AppFacade._instance.LuaManager.GetLuaTable(s, "client_msg");
                Util.CallMethod("GUIRoot", "ShowGUI", "MessagePanel", msg);
            }
            go.SetActive(false);
        }
        else if (go.name == "hor_item")
            hornor_list_panel_.SetActive(true);
        else if (go.name == "hor_close")
            hornor_list_panel_.SetActive(false);
        else if (go.name == "2nd")
        {
            PlaySound("addskill");
            var talent_id = Convert.ToInt32(go.transform.Find("hide_text").GetComponent<UILabel>().text);
            HideTalentPanel();
            talent_ck_msg tmp = new talent_ck_msg() { talent_id = talent_id, objt = go.transform };
            talent_time_list.Add(tmp);
            Battle.send_talent(talent_id);
        }
        else if (go.name == "kjMsg")  //弹出快捷消息
        {
            if (!JlMsgRoot_.activeInHierarchy)
            {
                JlMsgRoot_.SetActive(true);
                InitBattleMsg();
            }
            else
                JlMsgRoot_.SetActive(false);
        }
    }

    private void InitBattleMsg()
    {
        for (int i = JlMsgRoot_.transform.childCount - 1; i>=0; i--)
            GameObject.Destroy(JlMsgRoot_.transform.GetChild(i).gameObject);
        for (int i = 0; i < Config.l_battle_msg.Count; i++)
        {
            GameObject go = GameObject.Instantiate(JlMsgItem_);
            go.transform.parent = JlMsgRoot_.transform;
            go.transform.localPosition = new Vector3(0,i * -40,0);
            go.transform.localScale = Vector3.one;
            go.transform.localEulerAngles = Vector3.zero;
            go.GetComponent<UILabel>().text = Config.l_battle_msg[i].msg;
            var bx = go.transform.Find("bx").gameObject;
            RegisterOnClick(bx, BattleMsgClick);
            bx.name = Config.l_battle_msg[i].id.ToString();
            go.SetActive(true);
        }
    }

    private void BattleMsgClick(GameObject go)
    {
        Battle.send_battle_inside_msg(Convert.ToInt32(go.name));
        if (JlMsgRoot_.activeInHierarchy)
            JlMsgRoot_.SetActive(false);
    }

    public void HideTalentPanel()
    {
        talent_show_panel_.gameObject.SetActive(false);
        talent_title_panel_.gameObject.SetActive(false);
    }

    public void zhimang(BattleAnimalPlayer bp, bool flag)
    {
        if (BattlePlayers.me != null && BattlePlayers.me.player.guid == bp.player.guid)
        {
            zhimang_.SetActive(flag);
            if (flag)
            {
                var tc = zhimang_.GetComponent<TweenColor>();
                if (tc != null)
                    GameObject.Destroy(tc);
                var rt = zhimang_.GetComponent<UITexture>();
                if (rt != null)
                {
                    rt.alpha = 0;
                    TweenAlpha.Begin(zhimang_, 1, 1);
                }
            }
        }
    }
    public void InitBattlePanel()
    {
        var bp = BattlePlayers.me;
        if (bp == null)
            return;

        if (BattlePlayers.battle_type == 1)
            kjmsg_.SetActive(true);
        else
            kjmsg_.SetActive(false);

        if (BattlePlayers.battle_type == 1)
        {
            kill_report_.transform.Find("left_role").localPosition = new Vector3(-130, -85, 0);
            kill_report_.transform.Find("right_role").localPosition = new Vector3(130, -85, 0);
            kill_report_.transform.Find("left_role/left_team_name").gameObject.SetActive(true);
            kill_report_.transform.Find("right_role/right_team_name").gameObject.SetActive(true);
        }
        else
        {
            kill_report_.transform.Find("left_role").localPosition = new Vector3(-130, -60, 0);
            kill_report_.transform.Find("right_role").localPosition = new Vector3(130, -60, 0);
            kill_report_.transform.Find("left_role/left_team_name").gameObject.SetActive(false);
            kill_report_.transform.Find("right_role/right_team_name").gameObject.SetActive(false);
        }

        if (BattlePlayers.battle_type == 1)
        {
            result_panel_.transform.Find("baseboard/me/name").localPosition = new Vector3(-155, -8, 0);
            result_panel_.transform.Find("baseboard/me/name/team_name").gameObject.SetActive(true);
        }
        else
        {
            result_panel_.transform.Find("baseboard/me/name").localPosition = new Vector3(-155, 0, 0);
            result_panel_.transform.Find("baseboard/me/name/team_name").gameObject.SetActive(false);
        }
        var zhen = BattlePlayers.TNUM * 60 * Battle.turnTime + Battle.exTime - BattlePlayers.zhen;
        if (zhen < 0)
            zhen = 0;
        //ZDB 注释
        //if(!rankObj_.activeSelf)
        //    rankObj_.SetActive(true);

        if (BattlePlayers.battle_type == 1)
        {
            if (rank_type_ == 1)
                kill_label.text = "[B5F0EC]" + Config.get_t_script_str("BattlePanel_001");
            else
                kill_label.text = "[D0A851]" + Config.get_t_script_str("BattlePanel_001");
        }
        else
        {
            if (rank_type_ == 1)
                kill_label.text = "[B5F0EC]" + Config.get_t_script_str("BattlePanel_002");
            else
                kill_label.text = "[D0A851]" + Config.get_t_script_str("BattlePanel_002");
        }
        zhimang(bp, bp.attr.is_zhimang());
        if(sskill_icon_cd_.gameObject.activeSelf)
            sskill_icon_cd_.gameObject.SetActive(false);
        exp_change_state_ = false;
        light_state_ = false;
        get_max_white_ = false;
        star_pos_ = bp.player.level + bp.get_exp_per();
        end_pos_ = bp.player.level + bp.get_exp_per();
        current_pos_ = bp.player.level + bp.get_exp_per();
        exp_.value = (float)bp.get_exp_per();

        if (bp.is_die)
        {
            show_die(null);
        }

        else
            die_report_.SetActive(false);

        if (bp.player.talent_point > 0)
            ShowTalentPanel();
        else
            HideTalentPanel();

        if (BattlePlayers.me.player.skill_id > 0)
        {
            var sk = Config.get_t_skill(BattlePlayers.me.player.skill_id, BattlePlayers.me.player.skill_level);
            if (sk != null)
            {
                if (jskill_icon_.spriteName != sk.icon)
                {
                    jskill_icon_.atlas = IconPanel.GetAltas(sk.icon);
                    jskill_icon_.spriteName = sk.icon;
                }
            }
        }
        else
            jskill_icon_.spriteName = "";

        InitUnNormalState();
        foreach (var item in talent_ball_pools_)
        {
            if (item.Value.obj.activeSelf)
                item.Value.obj.SetActive(false);
        }

        int talent_num = 0;
        Dictionary<int, int> talent_dic = new Dictionary<int, int>();
        Dictionary<int, Vector3> talent_p = new Dictionary<int, Vector3>();
        for (int i = 0; i < bp.player.talent_id.Count; i++)
        {
            talent_dic.Add(bp.player.talent_id[i], bp.player.talent_level[i]);
            talent_num = talent_num + 1;
        }
        int index = 0;
        var dict = from obj in talent_dic orderby obj.Key select obj;

        foreach (KeyValuePair<int, int> item in dict)
        {
            talent_p[item.Key] = new Vector3(index * 56 + 28 - talent_num * 56 / 2.0f, 0, 0);
            index = index + 1;
        }

        var dict2 = from obj in talent_p orderby obj.Key select obj;
        foreach (KeyValuePair<int, Vector3> item in talent_p)
        {
            var v = item.Value;
            var k = item.Key;
            var target_item = GetTalentBallByID(k);
            bobjpool.set_localPosition(target_item.objid, v.x, v.y, v.z);
            bobjpool.set_localScale(target_item.objid, 1, 1, 1);
            var lb = target_item.objt.Find("sprite/num").GetComponent<UILabel>();
            lb.text = talent_dic[k].ToString();
            target_item.objt.gameObject.SetActive(true);
        }

        kill_message_queue_.start = 0;
        kill_message_queue_.length = 0;
        kill_message_queue_.queue.Clear();
        RefreshRankList();
    }
    public void show_die(BattleAnimal re_bp)
    {
        die_report_.SetActive(true);
        var text = die_report_.transform.Find("text").GetComponent<UILabel>();
        if (re_bp != null)
        {
            string content = string.Format(Config.get_t_script_str("BattlePanel_016"), re_bp.animal.name);
            text.text = content;//"你被 " + re_bp.animal.name + " 杀死了";
        }   
        else
        {
            text.text = Config.get_t_script_str("BattlePanel_017");//"你死了";
            re_bp = BattlePlayers.me;
        }

        double die_t = (BattlePlayers.zhen - BattlePlayers.me.player.death_time) * BattlePlayers.TICK;
        if (die_t >= 5000)
            die_t = 5000;
        else if (die_t <= 0)
            die_t = 0;

        die_t = BattleOperation.toInt((5000 - die_t) / 1000.0);    
            die_show_time_ = die_t;
        
        LuaHelper.GetPanelManager().RemoveAllChild(die_report_tx_.gameObject);
        t_avatar t_avatar = null;
        if(re_bp is BattleAnimalPlayer && !(re_bp.animal.is_ai >= 2000 && re_bp.animal.is_ai < 3000))
        {
            var tmp = re_bp as BattleAnimalPlayer;
            if (BattlePlayers.Boss != null && tmp.animal.guid == BattlePlayers.Boss.animal.guid)
                t_avatar = Config.get_t_avatar(tmp.player.avatar);
            else if (tmp.player.is_ai <= 100)
                t_avatar = Config.get_t_avatar_id(tmp.player.role_id);
            var avatar = AvaIconPanel.GetAvatar<BattlePanel>("avatar_res", t_avatar.id, "", tmp.player.toukuang);
            avatar.transform.parent = die_report_tx_;
            avatar.transform.localPosition = Vector3.zero;
            avatar.transform.localScale = Vector3.one;
            avatar.SetActive(true);
        }
    }
    public void RollObj()
    {
        if (roll_process == null)
        {
            roll_process = new roll_task();
            roll_process.last_t = BattlePlayers.zhen;
            roll_process.process = new List<roll_task_unit>();

            List<int> talent_list = new List<int>();
            List<int> tmp = new List<int>();
            List<int> select_list = new List<int>();
            List<int> full_talent = new List<int>();

            for (int i = 0; i < BattlePlayers.me.player.talent_id.Count; i++)
            {
                var t_talent = Config.get_t_talent(BattlePlayers.me.player.talent_id[i]);
                if (BattlePlayers.me.player.talent_level[i] == t_talent.max_level)
                    full_talent.Add(t_talent.id);
            }

            foreach (var item in Config.t_talents)
            {
                if (!full_talent.Contains(item.Key))
                    talent_list.Add(item.Key);
            }

            Func<int, List<int>> f = delegate (int talent_id)
            {
                List<int> list = new List<int>();
                int _n = 0;
                while (_n < 2)
                {
                    var id = BattleOperation.random(0, talent_list.Count);
                    id = talent_list[id];
                    if (!list.Contains(id) && talent_id != id)
                    {
                        if (_n == 1)
                            list.Add(talent_id);
                        list.Add(id);
                        _n++;
                    }
                }
                return list;
            };

            int num = 0;
            while (true)
            {
                var index = BattleOperation.random(0, talent_list.Count);
                if (!tmp.Contains(talent_list[index]))
                {
                    tmp.Add(talent_list[index]);
                    num = num + 1;
                }
                if (num == 3)
                    break;
            }

            for (int i = 0; i < tmp.Count; i++)
            {
                var list = f(tmp[i]);
                for (int j = 0; j < list.Count; j++)
                    select_list.Add(list[j]);
            }

            for (int i = 1; i <= 3; i++)
            {
                List<Transform> objs = new List<Transform>();
                var t_objt = talent_show_panel_.transform.Find("area_" + i);
                var _st = t_objt.Find("1st");
                int t_id = select_list[(i - 1) * 3];
                var t_talent = Config.get_t_talent(t_id);
                _st.GetComponent<UISprite>().atlas = IconPanel.GetAltas(t_talent.icon);
                _st.GetComponent<UISprite>().spriteName = t_talent.icon;
                int talent_level = 0;
                for (int j = 0; j < BattlePlayers.me.player.talent_id.Count; j++)
                {
                    if (BattlePlayers.me.player.talent_id[j] == select_list[(i - 1) * 3 + 1])
                    {
                        talent_level = BattlePlayers.me.player.talent_level[j];
                        break;
                    }
                }

                talent_level = talent_level + 1;
                var _nd = t_objt.Find("2nd");
                var bc = _nd.GetComponent<BoxCollider>();
                _nd.Find("hide_text").GetComponent<UILabel>().text = select_list[(i - 1) * 3 + 1].ToString();
                bc.enabled = false;
                t_talent = Config.get_t_talent(select_list[(i - 1) * 3 + 1]);
                _nd.GetComponent<UISprite>().atlas = IconPanel.GetAltas(t_talent.icon);
                _nd.GetComponent<UISprite>().spriteName = t_talent.icon;
                var title_obj = talent_title_panel_.transform.Find("area_" + i);
                title_obj.Find("name").GetComponent<UILabel>().text = t_talent.desc1.Replace("{N1}", talent_level.ToString());
                double value = 0;
                if (t_talent.id == 3)
                    value = t_talent.param3 / 100.0 * talent_level;
                else
                    value = t_talent.param3 * talent_level;

                title_obj.Find("effect").GetComponent<UILabel>().text = t_talent.desc2.Replace("{N2}", value.ToString());
                var _rd = t_objt.Find("3rd");
                t_id = select_list[(i - 1) * 3 + 2];
                t_talent = Config.get_t_talent(t_id);
                _rd.GetComponent<UISprite>().atlas = IconPanel.GetAltas(t_talent.icon);
                _rd.GetComponent<UISprite>().spriteName = t_talent.icon;
                objs.Add(_st);
                objs.Add(_nd);
                objs.Add(_rd);

                roll_task_unit rtu = new roll_task_unit() { status = false,bc = bc, speed = 56 * 20, objs = objs, s = 1260, delay = (i - 1) * 0.2, lt = BattlePlayers.zhen };
                roll_process.process.Add(rtu);
            }
        }
        else
        {
            double t = (BattlePlayers.zhen - roll_process.last_t) * BattlePlayers.TICK / 1000.0;
            for (int i = 0; i < roll_process.process.Count; i++)
            {
                if (!roll_process.process[i].status)
                {
                    if (t >= roll_process.process[i].delay)
                    {
                        double s = (BattlePlayers.zhen - roll_process.process[i].lt) * BattlePlayers.TICK / 1000.0 * roll_process.process[i].speed;
                        roll_process.process[i].s = roll_process.process[i].s - s;
                        if (roll_process.process[i].s <= 0)
                        {
                            s = s + roll_process.process[i].s;
                            roll_process.process[i].s = 0;
                            roll_process.process[i].status = true;
                        }
                        for (int j = 0; j < roll_process.process[i].objs.Count; j++)
                        {
                            var v = roll_process.process[i].objs[j].localPosition + new Vector3(0, (float)s, 0);
                            v.y = v.y % (84 * 3);
                            roll_process.process[i].objs[j].localPosition = v;
                        }
                        roll_process.process[i].lt = BattlePlayers.zhen;
                    }
                }
            }

            bool sign = true;
            for (int i = 0; i < roll_process.process.Count; i++)
            {
                if (!roll_process.process[i].status)
                {
                    sign = false;
                    break;
                }
            }
            if (sign)
            {
                for (int i = 0; i < roll_process.process.Count; i++)
                    roll_process.process[i].bc.enabled = true;
                talent_title_panel_.gameObject.SetActive(true);
                LuaHelper.GetTimerManager().RemoveRepeatTimer("RollObj");
                roll_process = null;
            }
        }
    }

    public void InitUnNormalState()
    {
        List<int> check_buffs = new List<int>();
        var dict = from obj in Config.t_battle_attrs orderby obj.Key select obj;
        foreach (KeyValuePair<int, t_battle_attr> item in dict)
        {
            t_battle_attr v = item.Value;
            if (v.isBadState >= 1)
                check_buffs.Add(item.Key);
        }

        for (int i = 0; i < check_buffs.Count; i++)
        {
            var buffer_id = check_buffs[i];
            var buff_final_time = UpdateSkillBuffer(buffer_id);
            if (buff_final_time != null)
            {
                var buff_time = buff_final_time - BattlePlayers.zhen;
                if (buff_time <= 0)
                    buff_time = 0;
                un_normal_states_time[buffer_id] = buff_time.GetValueOrDefault();
            }
        }
        Dictionary<int, un_normal_state_msg> etc = new Dictionary<int, un_normal_state_msg>();
        foreach (var item in un_normal_states_time)
        {
            var k = item.Key;
            var v = item.Value;
            un_normal_state_msg state_event = new un_normal_state_msg() { buffer_id = k, complete = false };
            state_event.Execute = delegate (un_normal_state_msg self)
            {
                if (!self.complete)
                {
                    var sp = un_normal_state_pools_[self.buffer_id].sp;
                    var cur_time = UpdateSkillBuffer(self.buffer_id) - BattlePlayers.zhen;
                    float p = cur_time.GetValueOrDefault() * 1.0f / un_normal_states_time[self.buffer_id];
                    sp.fillAmount = p;
                    if (p <= 0)
                        self.complete = true;
                }
            };

            state_event.Complete = delegate (un_normal_state_msg self)
            {
                return self.complete;
            };

            state_event.Finish = delegate (un_normal_state_msg self)
            {
                var obj = un_normal_state_pools_[self.buffer_id].obj;
                obj.SetActive(false);
                if (un_normal_states_time.ContainsKey(self.buffer_id))
                    un_normal_states_time.Remove(self.buffer_id);
            };

            etc.Add(k, state_event);
        }
        unNormalStateList = etc;
    }

    public void UnNormalState(int skill_id)
    {
        var bp = BattlePlayers.me;
        if (bp == null)
            return;

        var t_battle_buff = Config.get_t_battle_buff(skill_id);
        if (t_battle_buff == null)
            return;

        List<int> buffer_ids = new List<int>();
        for (int i = 0; i < t_battle_buff.attr.Count; i++)
        {
            int buffer_id = 0;
            if (t_battle_buff.attr[i].ptype == 1)
                buffer_id = t_battle_buff.attr[i].param1;
            else
                buffer_id = t_battle_buff.attr[i].ptype;
            buffer_ids.Add(buffer_id);
        }

        for (int i = 0; i < buffer_ids.Count; i++)
        {
            var buffer_id = buffer_ids[i];
            var buff_final_time = UpdateSkillBuffer(buffer_id);
            if (buff_final_time != null)
            {
                var buff_time = buff_final_time - BattlePlayers.zhen;
                if (buff_time <= 0)
                    buff_time = 0;

                if (!un_normal_states_time.ContainsKey(buffer_id))
                    un_normal_states_time.Add(buffer_id, buff_time.GetValueOrDefault());
                else
                    un_normal_states_time[buffer_id] = buff_time.GetValueOrDefault();
            }
        }

        Dictionary<int, un_normal_state_msg> etc = new Dictionary<int, un_normal_state_msg>();
        foreach (var item in un_normal_states_time)
        {
            var k = item.Key;
            var v = item.Value;
            un_normal_state_msg state_event = new un_normal_state_msg() { buffer_id = k, complete = false };
            state_event.Execute = delegate (un_normal_state_msg self)
            {
                if (!self.complete)
                {
                    var sp = un_normal_state_pools_[self.buffer_id].sp;
                    var cur_time = UpdateSkillBuffer(self.buffer_id) - BattlePlayers.zhen;
                    float p = cur_time.GetValueOrDefault() * 1.0f / un_normal_states_time[self.buffer_id];
                    sp.fillAmount = p;
                    if (p <= 0)
                        self.complete = true;
                }
            };

            state_event.Complete = delegate (un_normal_state_msg self)
            {
                return self.complete;
            };

            state_event.Finish = delegate (un_normal_state_msg self)
            {
                var obj = un_normal_state_pools_[self.buffer_id].obj;
                obj.SetActive(false);
                if (un_normal_states_time.ContainsKey(self.buffer_id))
                    un_normal_states_time.Remove(self.buffer_id);
            };

            etc.Add(k, state_event);
        }
        unNormalStateList = etc;

    }

    public int? UpdateSkillBuffer(int buffer_id)
    {
        var bp = BattlePlayers.me;
        if (bp == null)
            return null;

        List<int> check_buffs = new List<int>();
        var dict = from obj in Config.t_battle_attrs orderby obj.Key select obj;
        foreach (KeyValuePair<int, t_battle_attr> item in dict)
        {
            t_battle_attr v = item.Value;
            if (v.isBadState >= 1)
                check_buffs.Add(item.Key);
        }

        bool sign = false;
        for (int i = 0; i < check_buffs.Count; i++)
        {
            if (check_buffs[i] == buffer_id)
                sign = true;
        }

        if (!sign)
            return null;
        int total_num = 0;
        for (int i = 0; i < bp.player.buffs.Count; i++)
        {
            var t_battle_buff = Config.get_t_battle_buff(bp.player.buffs[i]);
            for (int j = 0; j < t_battle_buff.attr.Count; j++)
            {
                if (t_battle_buff.attr[j].ptype == 1)
                {
                    var t_battle_attr = Config.get_t_battle_attr(t_battle_buff.attr[j].param1);
                    if ((t_battle_attr.isBadState == 2 && bp.attr_value[t_battle_attr.id] < 0) || t_battle_attr.isBadState == 1)
                    {
                        if (t_battle_buff.attr[j].param1 == buffer_id)
                        {
                            if (bp.player.buffs_time[i] > total_num)
                                total_num = bp.player.buffs_time[i];
                        }
                    }
                }
                else if (t_battle_buff.attr[j].ptype == buffer_id)
                {
                    var t_battle_attr = Config.get_t_battle_attr(t_battle_buff.attr[j].param1);
                    if ((t_battle_attr.isBadState == 2 && bp.attr_value[t_battle_attr.id] < 0) || t_battle_attr.isBadState == 1)
                    {
                        if (bp.player.buffs_time[i] > total_num)
                            total_num = bp.player.buffs_time[i];
                    }
                }
            }
        }
        return total_num;
    }
    public talent_info GetTalentBallByID(int talent_ball_id)
    {
        if (talent_ball_pools_.ContainsKey(talent_ball_id))
            return talent_ball_pools_[talent_ball_id];

        GameObject obj = LuaHelper.Instantiate(talentBall_res_.gameObject);
        int objid = bobjpool.add(obj);
        Transform objt = obj.transform;

        talent_ball_pools_.Add(talent_ball_id, new talent_info() { objid = objid, objt = objt, obj = obj });
        objt.parent = talentBall_panel_.transform;
        RegisterOnPress(obj, ShowAblibityBallInfo);
        var sp = objt.Find("sprite").GetComponent<UISprite>();
        var t_talent = Config.get_t_talent(talent_ball_id);
        sp.atlas = IconPanel.GetAltas(t_talent.icon);
        sp.spriteName = t_talent.icon;
        obj.name = talent_ball_id.ToString();
        obj.SetActive(true);
        return talent_ball_pools_[talent_ball_id];
    }
    public void ShowAblibityBallInfo(GameObject obj,bool state)
    {
        if (state)
        {
            int talent_id = Convert.ToInt32(obj.name);
            var t_talent = Config.get_t_talent(talent_id);
            int level = 1;
            for (int i = 0; i < BattlePlayers.me.player.talent_id.Count; i++)
            {
                if (BattlePlayers.me.player.talent_id[i] == talent_id)
                {
                    level = BattlePlayers.me.player.talent_level[i];
                    break;
                }
            }

            var v = obj.transform.TransformPoint(talent_tips_panel_.transform.localPosition);
            var tp = talent_tips_panel_.transform.InverseTransformPoint(v);
            talent_tips_panel_.transform.localPosition = new Vector3(tp.x, tp.y + 60, 0);
            double value = 0;
            if (t_talent.id == 3)
                value = t_talent.param3 / 100.0;
            else
                value = t_talent.param3;

            talent_tips_text_.text = t_talent.desc2.Replace("{N2}", (level * value).ToString());
            talent_tips_panel_.SetActive(true);
        }
        else
            talent_tips_panel_.SetActive(false);
    }
    public void RefreshRankUI()
    {
        if (BattlePlayers.battle_type == 1)
            refresh_ui_team();
        else
            refresh_ui_single();
    }

    public void refresh_ui_team()
    {
        var t_teamid = Config.get_t_teamid(BattlePlayers.me.player.camp);
        string me_color = t_teamid.name.Substring(0, 8);
        int num = 0;
        if (rank_type_ == 1)
        {
            for (int i = 0; i < BattlePlayers.players_list.Count; i++)
            {
                num = num + 1;
                var bp = get_rank_by_rank(i + 1);
                t_teamid = Config.get_t_teamid(bp.player.camp);
                var color = t_teamid.name.Substring(0, 8);
                rank_lists_[i].name.color = Color.white;
                rank_lists_[i].name.text = color + bp.player.name;
                rank_lists_[i].score.text = bp.player.score.ToString();
                if (!rank_lists_[i].obj.activeSelf)
                    rank_lists_[i].obj.SetActive(true);
                if (i >= 4)
                    break;
            }
        }
        else if (rank_type_ == 2)
        {
            for (int i = 0; i < BattlePlayers.Camps.Count; i++)
            {
                num = num + 1;
                var team_id = get_team_id_by_num(i + 1);
                t_teamid = Config.get_t_teamid(team_id);
                rank_lists_[i].name.text = t_teamid.name;
                rank_lists_[i].name.color = Color.white;
                rank_lists_[i].score.text = get_team_score(team_id).ToString();
                if(!rank_lists_[i].obj.activeSelf)
                    rank_lists_[i].obj.SetActive(true);
                if (i >= 4)
                    break;
            }
        }

        for (int i = num ; i <= 4; i++)
        {
            if (rank_lists_[i].obj.activeSelf)
                rank_lists_[i].obj.SetActive(false);
        }

        if (rank_type_ == 1)
        {
            var rank = get_rank_by_guid(BattlePlayers.me.player.guid);
            if (rank >= 0)
            {
                rank_me_.name.text = me_color + BattlePlayers.me.player.name;
                rank_me_.rank.text = me_color + rank;
                rank_me_.score.text = "[B5F0ECFF]" + Math.Abs(BattlePlayers.me.player.score);
            }
        }
        else if (rank_type_ == 2)
        {
            t_teamid = Config.get_t_teamid(BattlePlayers.me.player.camp);
            rank_me_.rank.text = me_color + get_rank_by_team_id(t_teamid.id);
            rank_me_.name.text = me_color + BattlePlayers.me.player.name;
            rank_me_.score.text = "[B5F0ECFF]" + get_team_score(t_teamid.id);
        }
    }

    public int get_team_score(int camp_id)
    {
        if (!BattlePlayers.Camps.ContainsKey(camp_id))
            return 0;
        int totalScore = 0;
        for (int i = 0; i < BattlePlayers.Camps[camp_id].Count; i++)
        {
            var guid = BattlePlayers.Camps[camp_id][i];
            var tmp = BattlePlayers.players[guid] as BattleAnimalPlayer;
            totalScore = totalScore + tmp.player.score;
        }
        return totalScore;
    }
   
    public void refresh_ui_single()
    {
        var me_color = "[D0A851FF]";
        for (int i = 0; i < BattlePlayers.players_list.Count; i++)
        {
            if (i > 4)
                break;
            int num = 0;
            BattleAnimalPlayer pt = null;
            if (rank_type_ == 1)
            {
                pt = get_rank_by_rank(i + 1);
                num = Math.Abs(pt.player.score);
            }
            else if (rank_type_ == 2)
            {
                pt = get_info_by_rank_num(i + 1);
                num = Math.Abs(pt.player.sha);
            }

            rank_lists_[i].name.color = new Color(181 * 1.0f / 255, 240 * 1.0f / 255, 236 * 1.0f / 255);
            if (i >= 3)
                rank_lists_[i].obj.transform.Find("rank").GetComponent<UILabel>().color = new Color(181 * 1.0f / 255, 240 * 1.0f / 255, 236 * 1.0f / 255);

            rank_lists_[i].name.text = pt.player.name;
            rank_lists_[i].score.text = num.ToString();
            if (!rank_lists_[i].obj.activeSelf)
                rank_lists_[i].obj.SetActive(true);
        }

        for (int i = BattlePlayers.players_list.Count; i <= 4; i++)
        {
            if (rank_lists_[i].obj.activeSelf)
                rank_lists_[i].obj.SetActive(false);
        }

        var rank_me = get_rank_by_guid(BattlePlayers.me.player.guid);
        rank_me_.rank.text = me_color + rank_me;
        rank_me_.name.text = me_color + BattlePlayers.me.player.name;
        if (rank_type_ == 1)
            rank_me_.score.text = me_color + Math.Abs(BattlePlayers.me.player.score);
        else if (rank_type_ == 2)
            rank_me_.score.text = me_color + Math.Abs(BattlePlayers.me.player.sha); 
    }

    public void RefreshRankList()
    {
        UpdatePersonScore();
        UpdatePersonKill();
    }

    public void RefreshRankByTeam()
    {
        UpdatePersonScore();
        UpdatePersonKill();
        UpdateTeamScore();
    }

    public void UpdatePersonScore()
    {
        ScoreRankByGUID.Clear();
        ScoreRankByRank.Clear();

        if (BattlePlayers.players_list.Count <= 0)
            return;

        List<BattleAnimalPlayer> players = new List<BattleAnimalPlayer>();
        for (int i = 0; i < BattlePlayers.players_list.Count; i++)
        {
            var bp = BattlePlayers.players_list[i];
            players.Add(bp);
        }

        players.Sort((a, b) => {
            return b.player.score - a.player.score;
        });

        for (int i = 0; i < players.Count; i++)
        {
            ScoreRankByGUID.Add(players[i].player.guid, i + 1);
            ScoreRankByRank.Add(i + 1, players[i]);
        }
    }

    public void UpdatePersonKill()
    {
        KillRankByGUID.Clear();
        KillRankByRank.Clear();

        if (BattlePlayers.players_list.Count <= 0)
            return;

        List<BattleAnimalPlayer> players = new List<BattleAnimalPlayer>();
        for (int i = 0; i < BattlePlayers.players_list.Count; i++)
        {
            var bp = BattlePlayers.players_list[i];
            players.Add(bp);
        }
        players.Sort((a, b) => {
            return b.player.sha - a.player.sha;
        });

        for (int i = 0; i < players.Count; i++)
        {
            KillRankByGUID.Add(players[i].player.guid, i + 1);
            KillRankByRank.Add(i + 1, players[i]);
        }
    }

    public void UpdateTeamScore()
    {
        TeamScore.Clear();
        ScoreTeamRankByTeamID.Clear();
        ScoreTeamRankByRank.Clear();
        if (BattlePlayers.players_list.Count < 0)
            return;
        List<BattleAnimalPlayer> players = new List<BattleAnimalPlayer>();
        for (int i = 0; i < BattlePlayers.players_list.Count; i++)
        {
            var bp = BattlePlayers.players_list[i];
            players.Add(bp);
        }
        List<int> team_list = new List<int>();

        for (int i = 0; i < BattlePlayers.players_list.Count; i++)
        {
            var bp = BattlePlayers.players_list[i];
            if (!TeamScore.ContainsKey(bp.player.camp))
            {
                TeamScore.Add(bp.player.camp, 0);
                team_list.Add(bp.player.camp);
            }
            TeamScore[bp.player.camp] = TeamScore[bp.player.camp] + bp.player.score;
        }

        team_list.Sort((a, b) =>
        {
            return TeamScore[b] - TeamScore[a];
        });

        for (int i = 0; i < team_list.Count; i++)
        {
            ScoreTeamRankByRank.Add(i + 1, team_list[i]);
            ScoreTeamRankByTeamID.Add(team_list[i], i + 1);
        }
    }

    public int get_team_id_by_num(int rank_num)
    {
        if (ScoreTeamRankByRank.Count <= 0)
            UpdateTeamScore();
        if(!ScoreTeamRankByRank.ContainsKey(rank_num))
            UpdateTeamScore();
        if (ScoreTeamRankByRank.ContainsKey(rank_num))
            return ScoreTeamRankByRank[rank_num];
        else
            return -1;
    }

    public int get_rank_by_team_id(int team_id)
    {
        if (ScoreTeamRankByTeamID.Count <= 0 || ScoreTeamRankByRank.Count <= 0)
            UpdateTeamScore();
        if (!ScoreTeamRankByTeamID.ContainsKey(team_id))
            UpdateTeamScore();

        if (ScoreTeamRankByTeamID.ContainsKey(team_id))
            return ScoreTeamRankByTeamID[team_id];
        else
            return -1;
    }

    public BattleAnimalPlayer get_info_by_rank_num(int rank_num)
    {
        if (KillRankByRank.Count <= 0)
            UpdatePersonKill();
        if (!KillRankByRank.ContainsKey(rank_num))
            UpdatePersonKill();

        if (KillRankByRank.ContainsKey(rank_num))
            return KillRankByRank[rank_num];
        else
            return null;
    }

    public int get_killrank_by_guid(string guid)
    {
        if (KillRankByGUID.Count <= 0)
            UpdatePersonKill();
        if (!KillRankByGUID.ContainsKey(guid))
            UpdatePersonKill();

        if (KillRankByGUID.ContainsKey(guid))
            return KillRankByGUID[guid];
        else
            return -1;
    }

    public int get_rank_by_guid(string guid)
    {
        if (ScoreRankByGUID.Count <= 0)
            UpdatePersonScore();
        if (!ScoreRankByGUID.ContainsKey(guid))
            UpdatePersonScore();
        if (ScoreRankByGUID.ContainsKey(guid))
            return ScoreRankByGUID[guid];
        else
            return -1;
    }

    public BattleAnimalPlayer get_rank_by_rank(int rank_num)
    {
        if (ScoreRankByRank.Count <= 0)
            UpdatePersonScore();
        if (rank_num > ScoreRankByRank.Count)
            UpdatePersonScore();
        if (ScoreRankByRank.ContainsKey(rank_num))
            return ScoreRankByRank[rank_num];
        else
            return null;
    }

    public int get_cd(int skill_id)
    {
        if (BattlePlayers.me == null)
            return 0;
        var bp = BattlePlayers.me;
        var t_skill = Config.get_t_skill(skill_id, bp.get_skill_level(skill_id), bp.get_skill_level_add(skill_id));
        if (t_skill == null)
            return 0;

        int rz = -1;
        for (int i = 0; i < bp.player.save_re_id.Count; i++)
        {
            if (bp.player.save_re_id[i] == t_skill.id)
            {
                rz = bp.player.save_re_zhen[i];
                break;
            }
        }
        if (rz == -1)
            return 0;
        else
            return (t_skill.get_cd(bp) - (BattlePlayers.zhen - rz) * BattlePlayers.TICK);
    }

    void update_cd()
    {
        var bp = BattlePlayers.me;
        if (bp == null)
            return;

        GameObject cd = null;
        UISprite cd_sp = null;
        UILabel cd_text = null;
        int skill_id = 0;
        int skill_level = 0;

        for (int ll = 1; ll <= 3; ll++)
        {
            if (ll == 1)
            {
                cd = jskill_cd_;
                cd_sp = jskill_cd_sp_;
                cd_text = jskill_cd_text_;
                skill_id = bp.player.skill_id;
                skill_level = bp.player.skill_level;
            }
            else if (ll == 2)
            {
                cd = sskill_2_cd_;
                cd_sp = sskill_2_cd_sp_;
                cd_text = sskill_2_cd_text_;
                skill_id = 300201;
                skill_level = 1;
            }
            else if (ll == 3)
            {
                cd = sskill_cd_;
                cd_sp = sskill_cd_sp_;
                cd_text = sskill_cd_text_;
                skill_id = 300101;
                skill_level = 1;
            }
            
            if (ll == 3)
            {
                #region
                int index = -1;
                int rz = 0;
                var t_skill = Config.get_t_skill(skill_id, skill_level, bp.get_skill_level_add(skill_id));

                for (int i = 0; i < bp.player.save_re_id.Count; i++)
                {
                    if (bp.player.save_re_id[i] == skill_id)
                    {
                        index = i;
                        rz = bp.player.save_re_zhen[i];
                        break;
                    }
                }

                float pro = 0, t = 0;
                if (index != -1)
                {
                    if (bp.player.is_xueren)
                    {
                        var xueren_t = 10000;
                        if (Battle.is_newplayer_guide)
                        {
                            xueren_t = 20000;
                        }
                           
                        var lqAmount = 1 - (BattlePlayers.zhen - bp.player.xueren_zhen) * BattlePlayers.TICK * 1.0f / xueren_t;
                        if (lqAmount < 0)
                            lqAmount = 0;
                        else if (lqAmount > 1)
                            lqAmount = 1;

                        if (!sskill_icon_cd_.gameObject.activeSelf)
                            sskill_icon_cd_.gameObject.SetActive(true);

                        sskill_icon_cd_.fillAmount = lqAmount;
                    }
                    else
                    {
                        if (sskill_icon_cd_.gameObject.activeSelf)
                            sskill_icon_cd_.gameObject.SetActive(false);

                        if (t_skill != null)
                        {
                            pro = 1 - (BattlePlayers.zhen - rz) * BattlePlayers.TICK * 1.0f / t_skill.get_cd(bp);
                            if (pro < 0)
                            {
                                pro = 0;
                                t = 0;
                            }
                            else if (pro > 1)
                                pro = 1;
                            t = t_skill.get_cd(bp) / 1000 * pro;
                            t = Mathf.CeilToInt(t);// BattleOperation.toInt(t);
                        }
                    }
                }

                if (cd.activeSelf && pro == 0)
                {
                    cd.SetActive(false);
                    GameObject lightObj = null;
                    if (ll == 3)
                        lightObj = sskill_cd_;
                    PlayLightForSkill(lightObj);
                }
                else if (!cd.activeSelf && pro > 0)
                    cd.SetActive(true);
                #endregion

                var sp = sskill_icon_.GetComponent<UISprite>();
                if (!BattlePlayers.me.player.is_xueren)
                {
                    if (sp.spriteName != "sfic_bxr1")
                        sp.spriteName = "sfic_bxr1";
                }
                else if (BattlePlayers.me.player.is_xueren)
                {
                    if (sp.spriteName != "sfic_bxr2")
                        sp.spriteName = "sfic_bxr2";
                }

                cd_sp.fillAmount = pro;
                cd_text.text = t.ToString();

                var amount = bp.player.cost * 1.0f / (BattlePlayers.POWERUP * t_skill.cost);
                if (amount > 1)
                    amount = 1;
                else if (amount < 0)
                    amount = 0;
                sskill_power_sp_.fillAmount = amount;
                if (bp.player.cost >= BattlePlayers.POWERUP * t_skill.cost)
                {
                    if (!sskill_cicle_.activeInHierarchy)
                        sskill_cicle_.SetActive(true);
                    if (sskill_mask_.activeInHierarchy)
                        sskill_mask_.SetActive(false);
                }
                else
                {
                    if (bp.player.is_xueren)
                    {
                        if (!sskill_cicle_.activeInHierarchy)
                            sskill_cicle_.SetActive(true);
                        if (sskill_mask_.activeInHierarchy)
                            sskill_mask_.SetActive(false);
                    }
                    else
                    {
                        if (sskill_cicle_.activeInHierarchy)
                            sskill_cicle_.SetActive(false);
                        if (!sskill_mask_.activeInHierarchy)
                            sskill_mask_.SetActive(true);
                    }
                }
            }
            else if (ll == 2)
            {
                #region
                int index = -1;
                int rz = 0;
                var t_skill = Config.get_t_skill(skill_id, skill_level, bp.get_skill_level_add(skill_id));

                for (int i = 0; i < bp.player.save_re_id.Count; i++)
                {
                    if (bp.player.save_re_id[i] == skill_id)
                    {
                        index = i;
                        rz = bp.player.save_re_zhen[i];
                        break;
                    }
                }

                float pro = 0, t = 0;
                if (index != -1)
                {
                    if (t_skill != null)
                    {
                        pro = 1 - (BattlePlayers.zhen - rz) * BattlePlayers.TICK * 1.0f / t_skill.get_cd(bp);
                        if (pro < 0)
                        {
                            pro = 0;
                            t = 0;
                        }
                        else if (pro > 1)
                            pro = 1;
                        t = t_skill.get_cd(bp) / 1000 * pro;
                        t = Mathf.CeilToInt(t); //BattleOperation.toInt(t);
                    }
                }

                if (cd.activeSelf && pro == 0)
                {
                    cd.SetActive(false);
                    PlayLightForSkill(cd);
                }
                else if (!cd.activeSelf && pro > 0)
                    cd.SetActive(true);
                cd_sp.fillAmount = pro;
                cd_text.text = t.ToString();
                #endregion

                var amount = bp.player.cost * 1.0f / (BattlePlayers.POWERUP * t_skill.cost);
                if (amount > 1)
                    amount = 1;
                else if (amount < 0)
                    amount = 0;
                sskill_2_power_sp_.fillAmount = amount;
                if (bp.player.cost >= BattlePlayers.POWERUP * t_skill.cost)
                {
                    if (!sskill_2_cicle_.activeInHierarchy)
                        sskill_2_cicle_.SetActive(true);
                    if (sskill_2_mask_.activeInHierarchy)
                        sskill_2_mask_.SetActive(false);
                }
                else
                {
                    if (sskill_2_cicle_.activeInHierarchy)
                        sskill_2_cicle_.SetActive(false);
                    if (!sskill_2_mask_.activeInHierarchy)
                        sskill_2_mask_.SetActive(true);
                }
            }
            else if (ll == 1)
            {
                if (bp.player.skill_id == 0)
                {
                    if (jskill_level_obj_.activeSelf)
                        jskill_level_obj_.SetActive(false);

                    if (jskill_icon_obj_.activeSelf)
                        jskill_icon_obj_.SetActive(false);

                    if (jskill_icon_.spriteName != "")
                        jskill_icon_.spriteName = "";

                    if (jskill_box_.enabled)
                        jskill_box_.enabled = false;

                    if (jskill_cicle_.activeSelf)
                        jskill_cicle_.SetActive(false);

                    cd.SetActive(false);
                    if(jskill_mask_.activeInHierarchy)
                        jskill_mask_.SetActive(false);
                }
                else
                {
                    if (!jskill_icon_obj_.activeSelf)
                        jskill_icon_obj_.SetActive(true);

                    if (!jskill_box_.enabled)
                        jskill_box_.enabled = true;

                    var t_skill = Config.get_t_skill(skill_id, skill_level, bp.get_skill_level_add(skill_id));
                    #region
                    //var t = get_cd(bp.player.skill_id);

                    //if (t <= 0 && cd.activeSelf)
                    //    cd.SetActive(false);
                    //else if (t > 0)
                    //{
                    //    var p = t * 1.0f / t_skill.get_cd(bp);
                    //    if (!cd.activeSelf)
                    //        cd.SetActive(true);
                    //    cd_sp.fillAmount = p;
                    //    cd_text.text = BattleOperation.toInt(t * 1.0f / 1000).ToString();
                    //    cd.SetActive(true);
                    //}
                    #endregion

                    jskill_level_.fillAmount = bp.player.cost / (BattlePlayers.POWERUP * 3.0f);

                    //if (Battle.is_newplayer_guide)
                    //{
                    //    if (bp.player.cost >= t_skill.cost * BattlePlayers.POWERUP && Battle.is_frist_Skill_Hold)
                    //    {
                    //        if (jskill_mask_.activeInHierarchy)
                    //            jskill_mask_.SetActive(false);
                    //        if (!jskill_cicle_.activeInHierarchy)
                    //            jskill_cicle_.SetActive(true);
                    //    }
                    //    else
                    //    {
                    //        if (!jskill_mask_.activeInHierarchy)
                    //            jskill_mask_.SetActive(true);
                    //        if (jskill_cicle_.activeInHierarchy)
                    //            jskill_cicle_.SetActive(false);
                    //    }
                    //}
                    //else
                    //{
                    //    if (bp.player.cost >= t_skill.cost * BattlePlayers.POWERUP)
                    //    {
                    //        if (jskill_mask_.activeInHierarchy)
                    //            jskill_mask_.SetActive(false);
                    //        if (!jskill_cicle_.activeInHierarchy)
                    //            jskill_cicle_.SetActive(true);
                    //    }
                    //    else
                    //    {
                    //        if (!jskill_mask_.activeInHierarchy)
                    //            jskill_mask_.SetActive(true);
                    //        if (jskill_cicle_.activeInHierarchy)
                    //            jskill_cicle_.SetActive(false);
                    //    }
                    //}
                    if (bp.player.cost >= t_skill.cost * BattlePlayers.POWERUP)
                    {
                        if (jskill_mask_.activeInHierarchy)
                            jskill_mask_.SetActive(false);
                        if (!jskill_cicle_.activeInHierarchy)
                            jskill_cicle_.SetActive(true);
                    }
                    else
                    {
                        if (!jskill_mask_.activeInHierarchy)
                            jskill_mask_.SetActive(true);
                        if (jskill_cicle_.activeInHierarchy)
                            jskill_cicle_.SetActive(false);
                    }

                    if (Battle.is_newplayer_guide)
                    {
                        if (jskill_icon_.spriteName != t_skill.icon)
                        {
                            jskill_icon_.atlas = IconPanel.GetAltas(t_skill.icon);
                            jskill_icon_.spriteName = t_skill.icon;
                        }

                        jskill_mask_.GetComponent<UISprite>().spriteName = "sfic_bxr1a";
                        jskill_cicle_.GetComponent<UISprite>().spriteName = "fight_bg2";

                        if (!jskill_level_obj_.activeSelf)
                            jskill_level_obj_.SetActive(true);
                    }
                }
            }
        }
    }

    IEnumerator ColorChange(UILabel label)
    {
        int num = 2;
        while (num > 0)
        {
            label.color = new Color(246 / 255.0f, 223 / 255.0f, 13 / 255.0f);
            label.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            yield return new WaitForMinus(0.5f);
            label.color = Color.white;
            label.transform.localScale = Vector3.one;
            num--;
            yield return new WaitForMinus(0.5f);
        }
    }

    public void add_text(BattleAnimal bp, string num, int t)
    {
        if (BattlePlayers.jiasu)
            return;
        if (!sh_player_list_.ContainsKey(bp.animal.guid))
            sh_player_list_.Add(bp.animal.guid, new sh_player_info() { list = new List<inner_sh_player>(), tm = 0 });

        sh_player_list_[bp.animal.guid].list.Add(new inner_sh_player() { num = num, t = t });
    }
    public void PlayLightForSkill(GameObject obj)
    {
        var info = get_type_obj(3);
        var v = obj.transform.TransformPoint(info.objt.localPosition);
        var tp = info.objt.InverseTransformPoint(v);
        bobjpool.set_localPosition(info.objid, tp.x, tp.y, tp.z);
        info.obj.SetActive(true);

        pos_t_.delays.Add(pos_t_.length, new delay_unit() { time = 0.5, obj = info });
        pos_t_.r_time.Add(pos_t_.length, 0);
        pos_t_.length = pos_t_.length + 1;
    }
    public void add_text1(BattleAnimal bp, string num, int t)
    {
        if (!bp.unit.is_vis())
            return;

        var v = bp.accept.position;
        GameObject obj = LuaHelper.Instantiate(sh_num_);
        int objid = bobjpool.add(obj);
        bobjpool.set_type2(objid, v.x, v.y, v.z);
        var objt = obj.transform;
        objt.parent = num_panel_;
        bobjpool.set_localEulerAngles(objid, 0, 0, 0);
        bobjpool.set_localScale(objid, 1, 1, 1);
        obj.SetActive(true);
        obj.GetComponent<UIPanel>().depth = sh_depth_;
        sh_depth_ = sh_depth_ - 1;
        if (sh_depth_ < -300)
            sh_depth_ = -201;

        if (t == 1)
        {
            objt.Find("con/text").GetComponent<UILabel>().text = num;
            objt.Find("con/text").gameObject.SetActive(true);
        }
        else if (t == 2)
        {
            objt.Find("con/text_cri").GetComponent<UILabel>().text = num;
            objt.Find("con/text_cri").gameObject.SetActive(true);
        }
        else if (t == 3)
        {
            objt.Find("con/text_re").GetComponent<UILabel>().text = num;
            objt.Find("con/text_re").gameObject.SetActive(true);
        }
        else if (t == 5)
        {
            objt.Find("con/text_s").GetComponent<UILabel>().text = num;
            objt.Find("con/text_s").gameObject.SetActive(true);
        }
        else
        {
            objt.Find("con/text_t").GetComponent<UILabel>().text = num;
            objt.Find("con/text_t").gameObject.SetActive(true);
        }
        util_info sh = new util_info();
        sh.obj = obj;
        sh.objt = objt;
        sh.objid = objid;
        sh.time = 1;
        sh_nums_.Add(sh);
    }

    public void add_min_boss_pro(BattleAnimalBoss bp)
    {
        GameObject obj = LuaHelper.Instantiate(boss_pro_);
        int objid = BattlePlayers.bobjpool.add(obj);
        BattlePlayers.bobjpool.set_type1(objid, bp.objid, 1, 3);
        var objt = obj.transform;
        objt.parent = hp_panel_.transform;
        BattlePlayers.bobjpool.set_localEulerAngles(objid, 0, 0, 0);
        BattlePlayers.bobjpool.set_localScale(objid, 1, 1, 1);
        obj.SetActive(false);
        var bg = objt.Find("bg").gameObject;
        var expp = objt.Find("bg1/exp").GetComponent<UISprite>();
        var level = objt.Find("bg1/level").GetComponent<UILabel>();
        var level_icon_obj = objt.Find("bg1/max_level_icon").gameObject;
        var hp1 = obj.transform.Find("bg/hp");
        var hp = hp1.GetComponent<UIProgressBar>();
        var icon = objt.Find("icon");
        var iconSp = icon.GetComponent<UISprite>();
        var team_label = objt.Find("team_name").GetComponent<UILabel>();
        team_label.gameObject.SetActive(false);

        hp1.GetComponent<UISprite>().spriteName = "zdxt_005";
        obj.GetComponent<UIPanel>().depth = hp_depth_;
        hp_depth_ = hp_depth_ - 1;
        if (hp_depth_ < -200)
            hp_depth_ = -2;

        var hpbg = objt.Find("bg/hpbg").GetComponent<UIProgressBar>();
        var jg = objt.Find("bg/jg").gameObject;
        string name = bp.animal.name;
        objt.Find("name").GetComponent<UILabel>().text = bp.animal.name;

        if (min_pro_map_.ContainsKey(bp.animal.guid))
            min_pro_map_[bp.animal.guid] = new xt_info() { level_icon_obj = level_icon_obj, obj = obj, objt = objt, objid = objid, bp = bp, bg = bg, expp = expp, level = level, hp = hp, hpbg = hpbg, jg = jg, last_max_hp = -1, hpbg_value = 0, icon = icon.gameObject, iconSp = iconSp };
        else
            min_pro_map_.Add(bp.animal.guid, new xt_info() { level_icon_obj = level_icon_obj, obj = obj, objt = objt, objid = objid, bp = bp, bg = bg, expp = expp, level = level, hp = hp, hpbg = hpbg, jg = jg, last_max_hp = -1, hpbg_value = 0, icon = icon.gameObject, iconSp = iconSp });
    }
    public void add_monster_min_pro(BattleAnimalMonster bp)
    {
        GameObject obj = LuaHelper.Instantiate(monster_pro_);
        int objid = BattlePlayers.bobjpool.add(obj);
        BattlePlayers.bobjpool.set_type1(objid, bp.objid, 1, 1.5f);

        var objt = obj.transform;
        objt.parent = hp_panel_.transform;
        BattlePlayers.bobjpool.set_localEulerAngles(objid, 0, 0, 0);
        BattlePlayers.bobjpool.set_localScale(objid, 1, 1, 1);
        obj.SetActive(false);
        var bg = objt.Find("bg").gameObject;
        if (bp.animal.type == unit_type.Monster)
            objt.Find("bg1").gameObject.SetActive(false);
        else
            objt.Find("bg1").gameObject.SetActive(true);

        var expp = objt.Find("bg1/exp").GetComponent<UISprite>();
        var level = objt.Find("bg1/level").GetComponent<UILabel>();
        var level_icon_obj = objt.Find("bg1/max_level_icon").gameObject;
        var hp1 = obj.transform.Find("bg/hp");
        var hp = hp1.GetComponent<UIProgressBar>();
        var icon = objt.Find("icon");
        var iconSp = icon.GetComponent<UISprite>();
        var team_label = objt.Find("team_name").GetComponent<UILabel>();

        hp1.GetComponent<UISprite>().spriteName = "zdxt_005";
        obj.GetComponent<UIPanel>().depth = hp_depth_;
        hp_depth_ = hp_depth_ - 1;
        if (hp_depth_ < -200)
            hp_depth_ = -2;

        var hpbg = objt.Find("bg/hpbg").GetComponent<UIProgressBar>();
        var jg = objt.Find("bg/jg").gameObject;
        string name = bp.animal.name;
        team_label.gameObject.SetActive(false);

        objt.Find("name").GetComponent<UILabel>().text = bp.animal.name;
        if (min_pro_map_.ContainsKey(bp.animal.guid))
            min_pro_map_[bp.animal.guid] = new xt_info() { level_icon_obj = level_icon_obj, obj = obj, objt = objt, objid = objid, bp = bp, bg = bg, expp = expp, level = level, hp = hp, hpbg = hpbg, jg = jg, last_max_hp = -1, hpbg_value = 0, icon = icon.gameObject, iconSp = iconSp };
        else
            min_pro_map_.Add(bp.animal.guid, new xt_info() { level_icon_obj = level_icon_obj, obj = obj, objt = objt, objid = objid, bp = bp, bg = bg, expp = expp, level = level, hp = hp, hpbg = hpbg, jg = jg, last_max_hp = -1, hpbg_value = 0, icon = icon.gameObject, iconSp = iconSp });
    }
    public void add_min_pro(BattleAnimalPlayer bp)
    {
        GameObject obj = LuaHelper.Instantiate(min_pro_);
        int objid = BattlePlayers.bobjpool.add(obj);
        BattlePlayers.bobjpool.set_type1(objid, bp.objid, 1, 1.5f);

        var objt = obj.transform;
        objt.parent = hp_panel_.transform;
        BattlePlayers.bobjpool.set_localEulerAngles(objid, 0, 0, 0);
        BattlePlayers.bobjpool.set_localScale(objid, 1, 1, 1);
        obj.SetActive(false);
        var bg = objt.Find("bg").gameObject;
        if (bp.animal.type == unit_type.Monster)
            objt.Find("bg1").gameObject.SetActive(false);
        else
            objt.Find("bg1").gameObject.SetActive(true);

        var expp = objt.Find("bg1/exp").GetComponent<UISprite>();
        var level = objt.Find("bg1/level").GetComponent<UILabel>();
        var level_icon_obj = objt.Find("bg1/max_level_icon").gameObject;
        var hp1 = obj.transform.Find("bg/hp");
        var hp = hp1.GetComponent<UIProgressBar>();
        var icon = objt.Find("icon");
        var iconSp = icon.GetComponent<UISprite>();
        var team_label = objt.Find("team_name").GetComponent<UILabel>();
        
        if (bp.animal.guid == self.guid)
            obj.GetComponent<UIPanel>().depth = -1;
        else
        {
            if (bp.player.camp == BattlePlayers.self_camp)
            {
                hp1.GetComponent<UISprite>().spriteName = "zdxt_004";
                obj.GetComponent<UIPanel>().depth = hp_depth_;
                hp_depth_ = hp_depth_ - 1;
                if (hp_depth_ < -200)
                    hp_depth_ = -2;
            }
            else
            {
                hp1.GetComponent<UISprite>().spriteName = "zdxt_005";
                obj.GetComponent<UIPanel>().depth = hp_depth_;
                hp_depth_ = hp_depth_ - 1;
                if (hp_depth_ < -200)
                    hp_depth_ = -2;
            }
        }

        var hpbg = objt.Find("bg/hpbg").GetComponent<UIProgressBar>();
        var jg = objt.Find("bg/jg").gameObject;
        string name = bp.animal.name;

        objt.Find("region").GetComponent<UISprite>().spriteName = Config.get_t_foregion(bp.player.region_id).icon;
        //bp.player.region_id
        //if (bp.player.region != "" && Battle.is_online && bp.player.is_ai == 0 && Convert.ToInt64(bp.player.guid) > 100)
        //    name = name + "[77ff77](" + bp.player.region + ")[-]";

        IconPanel.InitVipLabel(objt.Find("name").GetComponent<UILabel>(), bp.player.name_color);
        if (BattlePlayers.battle_type == 1 && bp.player.type == unit_type.Player)
        {
            var t_teamid = Config.get_t_teamid(bp.player.camp);
            team_label.text = t_teamid.name;
            team_label.gameObject.SetActive(true);
        }
        else
            team_label.gameObject.SetActive(false);

        objt.Find("name").GetComponent<UILabel>().text = bp.animal.name;
        List<power_unit> powerList = new List<power_unit>();
        for (int i = 1; i <= 3; i++)
        {
            var powerObj = objt.Find("pow_bg/pow_"+i);
            var pu = new power_unit();
            pu.power = powerObj.GetComponent<UIProgressBar>();
            pu.light = powerObj.Find("border").gameObject;
            pu.twa = pu.light.GetComponent<TweenAlpha>();
            powerList.Add(pu);
        }

        if (min_pro_map_.ContainsKey(bp.animal.guid))
            min_pro_map_[bp.animal.guid] = new xt_info() { level_icon_obj = level_icon_obj, obj = obj, objt = objt, objid = objid, bp = bp, bg = bg, expp = expp, level = level, hp = hp, hpbg = hpbg, jg = jg, last_max_hp = -1, hpbg_value = 0, icon = icon.gameObject, iconSp = iconSp, powerList = powerList };
        else
            min_pro_map_.Add(bp.animal.guid, new xt_info() { level_icon_obj = level_icon_obj, obj = obj, objt = objt, objid = objid, bp = bp, bg = bg, expp = expp, level = level, hp = hp, hpbg = hpbg, jg = jg, last_max_hp = -1, hpbg_value = 0, icon = icon.gameObject, iconSp = iconSp, powerList = powerList });
    }

    public void del_min_pro(string guid)
    {
        if (!min_pro_map_.ContainsKey(guid))
            return;

        var mip = min_pro_map_[guid];
        BattlePlayers.bobjpool.remove(mip.objid);
        GameObject.Destroy(mip.obj);
        min_pro_map_.Remove(guid);
    }

    public void update_min_pro()
    {
        var bpme = BattlePlayers.me;
        bool nlz = true;
        if (last_update_min_pro_zhen == BattlePlayers.zhen)
            nlz = false;
        else
            last_update_min_pro_zhen = BattlePlayers.zhen;


        var cac = false;
        if (nlz && BattlePlayers.zhen % (BattlePlayers.TNUM / 4) == 0)
            cac = true;

        Dictionary<string, int> rank = new Dictionary<string, int>();
        if(cac && BattlePlayers.battle_type == 0)
        {
            if (BattlePlayers.players_list.Count >= 3)
            {
                for (int i = 1; i <= 3; i++)
                {
                    var pt = get_rank_by_rank(i);
                    rank.Add(pt.player.guid, i);
                }
            }
        }

        foreach (var item in min_pro_map_)
        {
            var guid = item.Key;
            var mip = item.Value;
            if (cac)
            {
                bool active = true;
                if (mip.bp.is_die)
                    active = false;
                if (BattlePlayers.me != null)
                {
                    if (active && !BattleOperation.can_see(BattlePlayers.me, mip.bp))
                        active = false;
                }

                if (active && !mip.bp.unit.is_vis())
                    active = false;

                if (active && !mip.obj.activeSelf)
                    mip.obj.SetActive(true);
                else if (!active && mip.obj.activeSelf)
                    mip.obj.SetActive(false);
            }
            if (mip.obj.activeSelf)
            {
                var max_hp = mip.bp.attr.max_hp();
                var hpper = mip.bp.animal.hp * 1.0f / max_hp;
                if (nlz)
                {
                    if (mip.last_max_hp != max_hp)
                    {
                        LuaHelper.GetPanelManager().RemoveAllChild(mip.bg, "jg_copy");
                        int a = 250;
                        while (a < max_hp)
                        {
                            var obj = LuaHelper.Instantiate(mip.jg);
                            var objt = obj.transform;
                            objt.parent = mip.bg.transform;
                            obj.name = "jg_copy";
                            objt.localPosition = new Vector3(-58 + 114 * (a *1.0f / max_hp), 0, 0);
                            objt.localEulerAngles = Vector3.zero;
                            objt.localScale = Vector3.one;
                            obj.SetActive(true);
                            a = a + 250;
                        }
                        mip.last_max_hp = max_hp;
                    }
                    if (mip.bp.animal.type == unit_type.Player)
                    {
                        var bp = mip.bp as BattleAnimalPlayer;
                        mip.expp.fillAmount = (float)bp.get_exp_per();
                        if (Config.max_battle_level == bp.player.level)
                        {
                            if (!mip.level_icon_obj.activeSelf)
                                mip.level_icon_obj.SetActive(true);
                             if(mip.level.gameObject.activeInHierarchy)
                                mip.level.gameObject.SetActive(false);
                        }
                        else
                        {
                            if(!mip.level.gameObject.activeInHierarchy)
                                mip.level.gameObject.SetActive(true);
                            mip.level.text = bp.player.level.ToString();
                            if (mip.level_icon_obj.activeSelf)
                                mip.level_icon_obj.SetActive(false);
                        }

                        //更新能量槽数值
                        bool flag = false;
                        for (int m = 1; m <= mip.powerList.Count; m++)
                        {
                            if (bp.player.cost >= m * BattlePlayers.POWERUP)
                            {
                                if (!mip.powerList[m - 1].light.activeInHierarchy)
                                {
                                    mip.powerList[m - 1].light.SetActive(true);
                                    flag = true;
                                }
                            }
                            else
                            {
                                if (mip.powerList[m - 1].light.activeInHierarchy)
                                    mip.powerList[m - 1].light.SetActive(false);
                            }

                            int dv = bp.player.cost - (m - 1) * BattlePlayers.POWERUP;
                            if (dv >= 0)
                                mip.powerList[m - 1].power.value = dv * 1.0f / BattlePlayers.POWERUP;
                            else
                                mip.powerList[m - 1].power.value = 0;
                        }

                        if (flag)
                        {
                            for (int m = 0; m < mip.powerList.Count; m++)
                            {
                                var twa = mip.powerList[m].twa;
                                twa.from = 1;
                                twa.to = 0;
                                twa.duration = 1;
                                twa.style = UITweener.Style.Loop;
                                twa.ResetToBeginning();
                                twa.PlayForward();
                            }
                        }
                    }
                    mip.hp.value = hpper;
                }

                if (mip.hpbg_value < hpper)
                {
                    mip.hpbg_value = hpper;
                    mip.hpbg.value = mip.hpbg_value;
                }
                else if (mip.hpbg_value > hpper)
                {
                    mip.hpbg_value = mip.hpbg_value - 0.5f * Time.deltaTime / Time.timeScale;
                    if (mip.hpbg_value < hpper)
                        mip.hpbg_value = hpper;
                    mip.hpbg.value = mip.hpbg_value;
                }
                if (cac)
                {
                    if (BattlePlayers.battle_type == 0)
                    {
                        if (rank.ContainsKey(guid))
                        {
                            if (rank[guid] == 1)
                                mip.iconSp.spriteName = "mc_one";
                            else if (rank[guid] == 2)
                                mip.iconSp.spriteName = "mc_two";
                            else if (rank[guid] == 3)
                                mip.iconSp.spriteName = "mc_three";

                            if (!mip.icon.activeSelf)
                                mip.icon.SetActive(true);
                        }
                        else
                        {
                            if (mip.icon.activeSelf)
                                mip.icon.SetActive(false);
                        }
                    }
                    else
                    {
                        if(mip.icon.activeSelf)
                            mip.icon.SetActive(false);
                    }
                }
            }
        }
    }

    public void kill_message_queue()
    {
        if (kill_message_queue_.start < kill_message_queue_.length)
        {
            var index = kill_message_queue_.start;
            if (!kill_message_queue_.queue[index].complete(kill_message_queue_.queue[index]))
            {
                kill_message_queue_.queue[index].execute(kill_message_queue_.queue[index]);
                if (kill_message_queue_.queue[index].complete(kill_message_queue_.queue[index]))
                {
                    kill_message_queue_.start = kill_message_queue_.start + 1;
                    kill_message_queue_.queue.Remove(index);
                }
            }
        }
    }
    public void add_kill_queue(BattleAnimal bp, BattleAnimal re_bp,int lsha)
    {
        kill_message_unit msg = new kill_message_unit();
        msg.cur_time = 0;
        msg.bp = bp;
        msg.re_bp = re_bp;

        t_avatar t_avatar = Config.get_t_avatar_id(bp.animal.role_id);
        msg.bp_avatar = t_avatar;
        if (bp.animal.type == unit_type.Boss)
            msg.bp_toukuang = 104;
        else if (bp.animal.type == unit_type.Player)
            msg.bp_toukuang = (bp as BattleAnimalPlayer).player.toukuang;

        msg.re_bp_avatar = Config.get_t_avatar_id(re_bp.animal.role_id);
        if (re_bp.animal.type == unit_type.Boss)
            msg.re_bp_toukuang = 104;
        else if (re_bp.animal.type == unit_type.Player)
            msg.re_bp_toukuang = (re_bp as BattleAnimalPlayer).player.toukuang;
        msg.lsha = lsha;
        msg.finish = false;
        msg.execute = delegate (kill_message_unit self)
        {
            if (self.cur_time == 0)
                show_kill1(self.bp, self.re_bp, self.lsha, self.bp_avatar, self.bp_toukuang, self.re_bp_avatar, self.re_bp_toukuang);

            self.cur_time = self.cur_time + Time.deltaTime / Time.timeScale;
            if (self.cur_time >= 3)
                self.finish = true;
        };
        msg.complete = delegate (kill_message_unit self)
        {
            return self.finish;
        };

        kill_message_queue_.queue.Add(kill_message_queue_.length, msg);
        kill_message_queue_.length = kill_message_queue_.length + 1;
    }

    public void show_kill(BattleAnimal bp, BattleAnimal re_bp,int lsha)
    {
        if (BattlePlayers.jiasu)
            return;

        if (bp.animal.type == unit_type.Monster || re_bp.animal.type == unit_type.Monster)
            return;
        add_kill_queue(bp, re_bp, lsha);
    }

    public void show_kill1(BattleAnimal bp, BattleAnimal re_bp,int lsha,t_avatar bp_avatar,int bp_toukuang,t_avatar re_bp_avatar,int re_bp_toukuang)
    {
        kill_report_.SetActive(true);
        kill_report_.GetComponent<Animator>().Rebind();
        kill_report_panel_.alpha = 1;
        kill_show_time_ = 3;
        LuaHelper.GetPanelManager().RemoveAllChild(kill_report_tx_.gameObject);
        var avatar = AvaIconPanel.GetAvatar<BattlePanel>("avatar_res", re_bp_avatar.id, "", re_bp_toukuang);
        avatar.transform.parent = kill_report_tx_;
        avatar.transform.localPosition = new Vector3(-130, 5, 0);
        avatar.transform.localScale = Vector3.one;
        avatar.SetActive(true);
        avatar = AvaIconPanel.GetAvatar<BattlePanel>("avatar_res", bp_avatar.id, "", bp_toukuang);
        avatar.transform.parent = kill_report_tx_;
        avatar.transform.localPosition = new Vector3(130, 5, 0);
        avatar.transform.localScale = Vector3.one;
        avatar.SetActive(true);

        string s = Config.get_t_script_str("BattlePanel_018");//"击败";
        if (re_bp.animal.type == unit_type.Boss || bp.animal.type == unit_type.Boss)
            s = Config.get_t_script_str("BattlePanel_019");//"击败了";
        else
        {
            if (lsha >= 8)
            {
                s = Config.get_t_script_str("BattlePanel_020");//"超神";
                PlaySound("killrs_006");
            }
            else if (lsha >= 7)
            {
                s = Config.get_t_script_str("BattlePanel_021");//"横扫千军";
                PlaySound("killrs_005");
            }
            else if (lsha >= 6)
            {
                s = Config.get_t_script_str("BattlePanel_022");//"主宰比赛";
                PlaySound("killrs_004");
            }
            else if (lsha >= 5)
            {
                s = Config.get_t_script_str("BattlePanel_023");//"无人能挡";
                PlaySound("killrs_003");
            }
            else if (lsha >= 4)
            {
                s = Config.get_t_script_str("BattlePanel_024");//"光芒四射";
                PlaySound("killrs_002");
            }
            else if (lsha >= 3)
            {
                s = Config.get_t_script_str("BattlePanel_025");//"实力初现";
                PlaySound("killrs_001");
            }
        }
        kill_report_.transform.Find("text").GetComponent<UILabel>().text = s;
        var kill_name_label = kill_report_.transform.Find("left_role").GetComponent<UILabel>();
        var killed_name_label = kill_report_.transform.Find("right_role").GetComponent<UILabel>();
        var left_team_label = kill_report_.transform.Find("left_role/left_team_name").GetComponent<UILabel>();
        var right_team_label = kill_report_.transform.Find("right_role/right_team_name").GetComponent<UILabel>();
        if (BattlePlayers.battle_type == 1)
        {
            string left_team = "";
            string right_team = "";
            if(re_bp.animal.type == unit_type.Boss)
                left_team = re_bp.animal.name;
            else
                left_team = Config.get_t_teamid(re_bp.animal.camp).name;

            if (bp.animal.type == unit_type.Boss)
                right_team = bp.animal.name;
            else
                right_team = Config.get_t_teamid(bp.animal.camp).name;

            left_team_label.text = left_team;
            right_team_label.text = right_team;
        }
        kill_name_label.text = re_bp.animal.name;
        killed_name_label.text = bp.animal.name;
        
        if (re_bp.animal.type == unit_type.Player)
        {
            var tp = re_bp as BattleAnimalPlayer;
            IconPanel.InitVipLabel(kill_name_label, tp.player.name_color); 
        }
        else
            IconPanel.InitVipLabel(kill_name_label,0);

        if (bp.animal.type == unit_type.Player)
        {
            var tp = bp as BattleAnimalPlayer;
            IconPanel.InitVipLabel(killed_name_label, tp.player.name_color);
        }
        else
            IconPanel.InitVipLabel(killed_name_label,0);
    }

    public void fuhuoInit()
    {
        if (BattlePlayers.me == null)
            return;
        var bp = BattlePlayers.me;
        if (bp.player.talent_point > 0)
            ShowTalentPanel();
        else
            HideTalentPanel();

        foreach (var item in talent_ball_pools_)
        {
            if (item.Value.obj.activeSelf)
                item.Value.obj.SetActive(false);
        }

        Dictionary<int, int> talent_dic = new Dictionary<int, int>();
        Dictionary<int, Vector3> talent_p = new Dictionary<int, Vector3>();
        for (int i = 0; i < bp.player.talent_id.Count; i++)
            talent_dic.Add(bp.player.talent_id[i], bp.player.talent_level[i]);

        var dict = from obj in talent_dic orderby obj.Key select obj;
        int index = 0;
        foreach (KeyValuePair<int, int> item in dict)
        {
            talent_p.Add(item.Key, new Vector3(index * 56 + 28 - talent_dic.Count * 56.0f / 2, 0, 0));
            index++;
        }

        var dict2 = from obj in talent_p orderby obj.Key select obj;
        foreach (KeyValuePair<int, Vector3> item in dict2)
        {
            var target_item = GetTalentBallByID(item.Key);
            bobjpool.set_localPosition(target_item.objid, item.Value.x, item.Value.y, item.Value.z);
            bobjpool.set_localScale(target_item.objid, 1, 1, 1);
            var label = target_item.objt.Find("sprite/num").GetComponent<UILabel>();
            label.text = talent_dic[item.Key].ToString();
            target_item.objt.gameObject.SetActive(true);
        }

        exp_change_state_ = false;
        star_pos_ = bp.player.level + bp.get_exp_per();
        end_pos_ = bp.player.level + bp.get_exp_per();
        current_pos_ = bp.player.level + bp.get_exp_per();
        var sp = exp_.transform.Find("expft").GetComponent<UISprite>();
        sp.white = 1;
        exp_.value = (float)bp.get_exp_per();
    }

    public void hreturn(GameObject go)
    {
        if (Battle.is_newplayer_guide)
        {
            NewPlayerGuide.CMSG_GUIDE(true);
            guide_skip_panel_.gameObject.SetActive(false);
        }
        else
        {
            if (Battle.is_online)
                Util.CallMethod("BattleTcp", "Disconnect2");

            Util.CallMethod("State", "ChangeState", 2);
        }
    }

    public void show_end()
    {

        die_report_.SetActive(false);
        for (int ll = 0; ll < BattlePlayers.players_list.Count; ll++)
        {
            var bp = BattlePlayers.players_list[ll];
            if (bp.pre_eff != null)
                bp.del_pre_eff();
        }

        if (talent_panel_.activeSelf)
            talent_panel_.SetActive(false);

        if (talent_tips_panel_.activeSelf)
            talent_tips_panel_.SetActive(false);

        end_panel_.SetActive(true);
        int rank = 30;
        if (BattlePlayers.battle_type == 0)
            rank = get_rank_by_guid(BattlePlayers.me.player.guid);
        else
            rank = get_rank_by_team_id(BattlePlayers.me.player.camp);

        CMSG_OFFLINE_BATTLE_END(rank);
        end_panel_text_.text = rank.ToString();
        end_time_ = 1;

        mvpList.Clear();
        max_killList.Clear();
        cgList.Clear();

        if (Battle.is_online && BattlePlayers.battle_type != 0)
        {
            var team_id = get_team_id_by_num(1);
            List<string> sl_teammates = new List<string>();
            for (int i = 0; i < BattlePlayers.Camps[team_id].Count; i++)
                sl_teammates.Add(BattlePlayers.Camps[team_id][i]);
            sl_teammates.Sort((a, b) => {
                var player_1 = BattlePlayers.players[a] as BattleAnimalPlayer;
                var player_2 = BattlePlayers.players[b] as BattleAnimalPlayer;
                var a_score = player_1.player.score;
                var b_score = player_2.player.score;
                if (a_score == b_score)
                {
                    if (player_1.player.sha == player_2.player.sha)
                        return player_1.player.die - player_2.player.die;
                    else
                        return player_2.player.sha - player_1.player.sha;
                }
                else
                    return b_score - a_score;
            });

            var mvp = BattlePlayers.players[sl_teammates[0]] as BattleAnimalPlayer;
            for (int i = 0; i < sl_teammates.Count; i++)
            {
                var bp = BattlePlayers.players[sl_teammates[i]] as BattleAnimalPlayer;
                if (bp.animal.guid == mvp.animal.guid)
                    mvpList.Add(bp.animal.guid);
                else
                {
                    if(bp.player.score == mvp.player.score && bp.player.sha == mvp.player.sha && bp.player.die == mvp.player.die)
                        mvpList.Add(bp.player.guid);
                }
            }
        }

        var max_kill_player = get_info_by_rank_num(1);
        for (int i = 0; i < BattlePlayers.players_list.Count; i++)
        {
            var bp = BattlePlayers.players_list[i];
            if (bp.player.max_lsha >= 8)
                cgList.Add(bp.player.guid);
            if (max_kill_player.player.guid == bp.player.guid)
                max_killList.Add(bp.player.guid);
            else if (max_kill_player.player.sha == bp.player.sha)
                max_killList.Add(bp.player.guid);
        }

        teamRankList.Clear();
        if (BattlePlayers.battle_type == 1)
        {
            List<int> team_ids = new List<int>();
            for (int i = 0; i < BattlePlayers.players_list.Count; i++)
            {
                var camp_id = BattlePlayers.players_list[i].player.camp;
                if (!team_ids.Contains(camp_id))
                    team_ids.Add(camp_id);
            }

            team_ids.Sort((a, b) => {
                return TeamScore[b] - TeamScore[a];
            });

            for (int i = 0; i < team_ids.Count; i++)
            {
                var team_list = BattlePlayers.Camps[team_ids[i]];
                team_list.Sort((a, b) =>
                {
                    var player_a = BattlePlayers.players[a] as BattleAnimalPlayer;
                    var player_b = BattlePlayers.players[b] as BattleAnimalPlayer;
                    if (player_a.player.score == player_b.player.score)
                    {
                        if (player_a.player.sha == player_b.player.sha)
                            return player_a.player.die - player_b.player.die;
                        else
                            return player_b.player.sha - player_a.player.sha;
                    }
                    else
                        return player_b.player.score - player_a.player.score;
                });

                for (int j = 0; j < team_list.Count; j++)
                    teamRankList.Add(BattlePlayers.players[team_list[j]] as BattleAnimalPlayer);
            }
        }

        foreach (var item in BattlePlayers.effects)
        {
            if (item.Value.obj != null)
            {
                var ad = item.Value.obj.GetComponent<AudioSource>();
                if (ad != null)
                    ad.enabled = false;
            }
        }

        Old_Self_Data.level_max_golds = self.get_out_attr(8);
        Old_Self_Data.extra_gold_add = self.get_out_attr(7);
        Old_Self_Data.battle_golds = self.player.battle_gold;
        Old_Self_Data.box_zd_num = self.player.box_zd_num;
    }

    public void end_click(GameObject go)
    {
        LuaHelper.GetTimerManager().RemoveRepeatTimer("BattlePanel");
        end_panel_.SetActive(false);
        detail_panel_.gameObject.SetActive(false);
        cup_panel_.gameObject.SetActive(false);
        result_panel_.SetActive(true);

        var view = result_panel_.transform.Find("baseboard/Panel/view");
        var tobj = result_panel_.transform.Find("baseboard/me").gameObject;
        if (view.childCount > 0)
        {
            for (int i = 0; i < view.childCount; i++)
                GameObject.Destroy(view.GetChild(i).gameObject);
        }
        if (BattlePlayers.battle_type == 1)
        {
            for (int i = 0; i < BattlePlayers.team_num; i++)
            {
                for (int j = 0; j < BattlePlayers.member_num; j++)
                {
                    var pt = teamRankList[BattlePlayers.member_num * i + j];
                    var obj = LuaHelper.Instantiate(tobj);
                    var objt = obj.transform;
                    objt.parent = view;
                    objt.localPosition = new Vector3(0, 90 - 105 * (i * BattlePlayers.member_num + j), 0);
                    objt.localEulerAngles = Vector3.zero;
                    objt.localScale = Vector3.one;
                    initme(obj, pt, i + 1);
                    if (pt.player.guid == BattlePlayers.me.player.guid)
                        initme(tobj, pt, i + 1);
                }
            }
        }
        else
        {
            for (int i = 0; i < BattlePlayers.players_list.Count; i++)
            {
                var obj = LuaHelper.Instantiate(tobj);
                var objt = obj.transform;
                objt.parent = view;
                objt.localPosition = new Vector3(0, 90 - 105 * i, 0);
                objt.localEulerAngles = Vector3.zero;
                objt.localScale = Vector3.one;
                var pt = get_rank_by_rank(i + 1);
                initme(obj, pt, i + 1);
                if (pt.player.guid == BattlePlayers.me.player.guid)
                    initme(tobj, pt, i + 1);
            }
        }
    }

    private void initme(GameObject obj,BattleAnimalPlayer bp,int rank,GameObject downObj = null)
    {
        if (bp.player.guid == BattlePlayers.me.player.guid)
            obj.GetComponent<UISprite>().spriteName = "nxxt_002";
        else if (rank == 1)
            obj.GetComponent<UISprite>().spriteName = "nxxt_003";
        else
            obj.GetComponent<UISprite>().spriteName = "nxxt_001";

        if (rank == 1)
            obj.transform.Find("rankp").GetComponent<UISprite>().spriteName = "mc_one";
        else if (rank == 2)
            obj.transform.Find("rankp").GetComponent<UISprite>().spriteName = "mc_two";
        else if (rank == 3)
            obj.transform.Find("rankp").GetComponent<UISprite>().spriteName = "mc_three";
        else
        {
            obj.transform.Find("rank").gameObject.SetActive(true);
            obj.transform.Find("rank").GetComponent<UILabel>().text = rank.ToString();
            obj.transform.Find("rankp").gameObject.SetActive(false);
        }

        var tx = obj.transform.Find("tx");

        if (tx.childCount > 0)
        {
            for (int i = 0; i < tx.childCount; i++)
                GameObject.Destroy(tx.GetChild(i).gameObject);
        }
        var avatar = AvaIconPanel.GetAvatar<BattlePanel>("avatar_res", bp.player.avatar,"", bp.player.toukuang);
        avatar.transform.parent = tx;
        avatar.transform.localPosition = new Vector3(0, 0, 0);
        avatar.transform.localScale = Vector3.one;

        var frame = avatar.transform.Find("frame").GetComponent<UISprite>();
        var frame_icon = avatar.transform.Find("avatar").GetComponent<UISprite>();
        frame.width = 100;
        frame.height = 100;
        frame_icon.width = 64;
        frame_icon.height = 64;
        avatar.SetActive(true);

        var team_name_objt = obj.transform.Find("name/team_name");
        if (BattlePlayers.battle_type == 1)
        {
            var t_teamid = Config.get_t_teamid(bp.player.camp);
            team_name_objt.GetComponent<UILabel>().text = t_teamid.name;
        }
        var friendobj = obj.transform.Find("friend").gameObject;
        var black_listobj = obj.transform.Find("black_list").gameObject;

        if (Battle.is_online)
        {
            if (bp.player.guid != self.guid)
            {
                object[] result = Util.CallMethod("PlayerData", "social_type", bp.player.guid);
                if (result.Length == 1)
                {
                    double social_type = 0;
                    double.TryParse(result[0].ToString(), out social_type);
                    if (social_type == 2)
                        friendobj.SetActive(false);
                    else
                    {
                        if (!friendobj.activeSelf)
                            friendobj.SetActive(true);
                        RegisterOnClick(friendobj, OnClick);
                        obj.transform.Find("friend/guid").name = bp.player.guid;
                    }
                    if (social_type == 3)
                        black_listobj.SetActive(false);
                    else
                    {
                        if (!black_listobj.activeSelf)
                            black_listobj.SetActive(true);
                        RegisterOnClick(black_listobj, OnClick);
                        obj.transform.Find("black_list/guid").name = bp.player.guid;
                    }
                }
            }
            else
            {
                black_listobj.SetActive(false);
                friendobj.SetActive(false);
            }
        }
        else
        {
            black_listobj.SetActive(false);
            friendobj.SetActive(false);
        }
        obj.transform.Find("region").GetComponent<UISprite>().spriteName = Config.get_t_foregion(bp.player.region_id).icon;
        obj.transform.Find("name").GetComponent<UILabel>().text = bp.player.name;
        IconPanel.InitVipLabel(obj.transform.Find("name").GetComponent<UILabel>(), bp.player.name_color);
        if(BattlePlayers.battle_type == 1)
            obj.transform.Find("exp").GetComponent<UILabel>().text = "[E4AC01FF]"+Math.Abs(bp.player.score)+"[-]\n[00ff00ff]("+get_team_score(bp.player.camp)+")[-]";
        else
            obj.transform.Find("exp").GetComponent<UILabel>().text = "[E4AC01FF]"+Math.Abs(bp.player.score)+"[-]";

        obj.transform.Find("killDealth").GetComponent<UILabel>().text = "[00ff00]" + bp.player.sha + "[-][B5F0EC]/[ff0000]" + bp.player.die + "[-]";
        if (!Battle.is_online)
            obj.transform.Find("cup_add").GetComponent<UILabel>().text = "--";
        else
        {
            var t_cup = Config.get_t_cup(bp.player.cup);
            var cup = bp.player.cup;
            int change = 0;
            if (BattlePlayers.battle_type == 1)
            {
                var score = bp.player.score;//TeamScore[];
                if (rank <= t_cup.tsb && score >= t_cup.tsbnum)
                    change = 1;
                else if (t_cup.tjb > 0 && rank > t_cup.tjb)
                    change = -1;
            }
            else
            {
                if (rank <= t_cup.sb)
                    change = 1;
                else if (t_cup.jb > 0 && rank > t_cup.jb)
                    change = -1;
            }

            if (change > 0)
                obj.transform.Find("cup_add").GetComponent<UILabel>().text = "[00ff00]+1[-]";
            else if (change < 0)
                obj.transform.Find("cup_add").GetComponent<UILabel>().text = "[ff0000]-1[-]";
            else
                obj.transform.Find("cup_add").GetComponent<UILabel>().text = "--";
        }

        int self_rank_num = 30;
        if (BattlePlayers.battle_type == 1)
        {
            for (int i = 0; i < teamRankList.Count; i++)
            {
                if (teamRankList[i].player.guid == bp.player.guid)
                {
                    self_rank_num = i;
                    break;
                }
            }
        }
        else
            self_rank_num = get_rank_by_guid(bp.player.guid);

        int gold = 31 - self_rank_num;
        bool d_gold = false;
        if (bp.player.guid == self.guid)
        {
            gold = gold + Old_Self_Data.extra_gold_add;
            if (!Battle.is_online)
            {
                if (self.player.level < 3 && Old_Self_Data.box_zd_num <= 2)
                {
                    gold = gold * 2;
                    d_gold = true;
                }
            }
            else
            {
                if (Old_Self_Data.box_zd_num <= 2)
                {
                    gold = gold * 2;
                    d_gold = true;
                }
            }

            if (Old_Self_Data.battle_golds + gold > Old_Self_Data.level_max_golds)
                gold = Old_Self_Data.level_max_golds - Old_Self_Data.battle_golds;

            if (gold < 0)
                gold = 0;
        }

        if (gold == 0)
            obj.transform.Find("gold").GetComponent<UILabel>().text = Config.get_t_script_str("BattlePanel_026");//"获取上限";
        else if (gold > 0)
            obj.transform.Find("gold").GetComponent<UILabel>().text = gold.ToString();

        var dgold_lb = obj.transform.Find("double_label");
        if (d_gold)
            dgold_lb.gameObject.SetActive(true);
        else
            dgold_lb.gameObject.SetActive(false);

        var hor_root = obj.transform.Find("hornor_icons");
        var hor_res = obj.transform.Find("hor_item").gameObject;
        for (int i = 0; i < hor_root.childCount; i++)
            GameObject.Destroy(hor_root.GetChild(i).gameObject);

        int hor_index = 0;
        if (mvpList.Contains(bp.player.guid))
        {
            var hor_obj = LuaHelper.Instantiate(hor_res);
            hor_obj.transform.parent = hor_root;
            hor_obj.transform.localPosition = new Vector3(0, 5 - 30 * hor_index, 0);
            hor_obj.transform.localScale = Vector3.one;
            hor_obj.GetComponent<UISprite>().spriteName = "zdjsbq_002";
            hor_obj.name = "hor_item";
            hor_obj.SetActive(true);
            RegisterOnClick(hor_obj, OnClick);
            hor_index = hor_index + 1;
        }

        if (cgList.Contains(bp.player.guid))
        {
            var hor_obj = LuaHelper.Instantiate(hor_res);
            hor_obj.transform.parent = hor_root;
            hor_obj.transform.localPosition = new Vector3(0, 5 - 30 * hor_index, 0);
            hor_obj.transform.localScale = Vector3.one;
            hor_obj.GetComponent<UISprite>().spriteName = "zdjsbq_001";
            hor_obj.name = "hor_item";
            hor_obj.SetActive(true);
            RegisterOnClick(hor_obj, OnClick);
            hor_index = hor_index + 1;
        }

        if (max_killList.Contains(bp.player.guid))
        {
            var hor_obj = LuaHelper.Instantiate(hor_res);
            hor_obj.transform.parent = hor_root;
            hor_obj.transform.localPosition = new Vector3(0, 5 - 30 * hor_index, 0);
            hor_obj.transform.localScale = Vector3.one;
            hor_obj.GetComponent<UISprite>().spriteName = "zdjsbq_003";
            hor_obj.name = "hor_item";
            hor_obj.SetActive(true);
            RegisterOnClick(hor_obj, OnClick);
            hor_index = hor_index + 1;
        }
    }

    public void AddPassSkPoint(GameObject obj)
    {
        PlaySound("click");
        int sk_id = Convert.ToInt32(obj.name);
        var sk_dt = Config.get_t_talent(sk_id);
        if (sk_dt == null || BattlePlayers.me == null)
            return;

        for (int i = 0; i < BattlePlayers.me.player.talent_id.Count; i++)
        {
            if (sk_id == BattlePlayers.me.player.talent_id[i])
            {
                if (BattlePlayers.me.player.talent_level[i] == 3)
                    return;
            }
        }
        Battle.send_talent(sk_id);
        var info = get_type_obj(3);
        var v = obj.transform.TransformPoint(info.objt.localPosition);
        var tp = info.objt.InverseTransformPoint(v);
        bobjpool.set_localPosition(info.objid, tp.x, tp.y, tp.z);
        info.obj.SetActive(true);
        pos_t_.delays.Add(pos_t_.length, new delay_unit() { time = 0.5, obj = info });
        pos_t_.r_time[pos_t_.length] = 0;
        pos_t_.length = pos_t_.length + 1;

        pos_t_.paths[pos_t_.length] = new effect_path() { isMulti = true, ms_path = new List<single_path>() };
        var finalPos = LuaHelper.GetMapManager().WorldToScreenPoint(BattlePlayers.me.objt.position);
        finalPos.y = finalPos.y + 50;
        for (int i = 1; i <= 3; i++)
        {
            resource_info rf = get_type_obj(2);
            var x_1 = BattlePanel.Random(tp.x, finalPos.x);
            var x_2 = BattlePanel.Random(tp.x, finalPos.x);
            var y_1 = BattlePanel.Random(tp.y, finalPos.y);
            var y_2 = BattlePanel.Random(tp.y, finalPos.y);
            var p1 = new Vector3(x_1, y_1, 2);
            var p2 = new Vector3(x_2, y_2, 2);
            var path = new single_path() { st_p = tp, ed_p = finalPos, mid_p1 = p1, mid_p2 = p2, cur_t = 0, total_t = 1, dt = rf };
            pos_t_.paths[pos_t_.length].ms_path.Add(path);
        }

        pos_t_.r_time.Add(pos_t_.length, 0);
        task_unit tu = new task_unit();
        tu.execute = delegate (task_unit t) {
            BattlePlayers.Attach(BattlePlayers.me, "accept", "Unit_buff_addSkill");
        };
        pos_t_.funcs.Add(pos_t_.length,tu);
        pos_t_.length = pos_t_.length + 1;
    }
    public void AddMainSkill(int ps)
    {
        if (BattlePlayers.me == null)
            return;

        jskill_icon_.spriteName = "";
        //jskill_dian_.GetComponent<UISprite>().spriteName = "";
        jskill_mask_.GetComponent<UISprite>().spriteName = "";
        jskill_cicle_.GetComponent<UISprite>().spriteName = "";

        GameObject t_panel = null;
        if (ps == 1)
            t_panel = jskill_panel_;
        else if (ps == 2)
            t_panel = jskill_2_panel_;

        var role_pos = LuaHelper.GetMapManager().WorldToScreenPoint(BattlePlayers.me.objt.position);
        role_pos.y = role_pos.y + 50;
        var dt = get_type_obj(2);
        var v = t_panel.transform.TransformPoint(dt.objt.localPosition);
        var tp = dt.objt.InverseTransformPoint(v);
        var x_1 = BattlePanel.Random(role_pos.x, tp.x); 
        var x_2 = BattlePanel.Random(role_pos.x, tp.x);
        var y_1 = BattlePanel.Random(role_pos.y, tp.y);
        var y_2 = BattlePanel.Random(role_pos.y, tp.y);
        var p1 = new Vector3(x_1, y_1, 2);
        var p2 = new Vector3(x_2, y_2, 2);
        task_unit tu = new task_unit();
        tu.execute =  delegate (task_unit t)
        {
            if (ps == 1)
            {
                var sk = Config.get_t_skill(BattlePlayers.me.player.skill_id, BattlePlayers.me.player.skill_level);
                if (sk == null)
                    return;
                if (jskill_icon_.spriteName != sk.icon)
                {
                    jskill_icon_.atlas = IconPanel.GetAltas(sk.icon);
                    jskill_icon_.spriteName = sk.icon;
                }

                //var jd = jskill_dian_.GetComponent<UISprite>();
                //jd.spriteName = "sfic_bxr1b";
                jskill_mask_.GetComponent<UISprite>().spriteName = "sfic_bxr1a";
                jskill_cicle_.GetComponent<UISprite>().spriteName = "fight_bg2";

                if (!jskill_mask_.activeInHierarchy)
                    jskill_mask_.SetActive(true);
                if (!jskill_level_obj_.activeSelf)
                    jskill_level_obj_.SetActive(true);
            }
        };

        pos_t_.paths.Add(pos_t_.length, new effect_path() { isMulti = false, s_path = new single_path() { st_p = role_pos, ed_p = tp,mid_p1 = p1,mid_p2 = p2, cur_t = 0, total_t = 0.5f,  dt = dt } });
        pos_t_.r_time.Add(pos_t_.length, 0);
        pos_t_.funcs.Add(pos_t_.length, tu);
        pos_t_.length = pos_t_.length + 1;
    }

    public void show_boss_tips(string content, int b_t, bool sound = false)
    {
        total_sound_times = b_t;
        boss_tips = content;
        boss_sound = sound;
        wave_state = true;
        PlayTimesForLabel();
    }
    public void PlayTimesForLabel()
    {
        LuaHelper.GetTimerManager().RemoveTimer("PlayerBossWarning_" + total_sound_times);
        if (tips_ == null || !wave_state)
            return;

        if (total_sound_times > 1)
        {
            if (boss_sound)
                PlaySound("boss_warning");
            tips_label_.text = boss_tips.Replace("{0}", total_sound_times.ToString());
            total_sound_times = total_sound_times - 1;
            LuaHelper.GetTimerManager().AddTimer("PlayerBossWarning" + total_sound_times, PlayTimesForLabel, 1);
        }
        else if (total_sound_times == 1)
        {
            if (boss_sound)
                PlaySound("boss_warning");
            tips_label_.text = boss_tips.Replace("{0}", total_sound_times.ToString());
        }
        tips_.ResetToBeginning();
        tips_.PlayForward();
    }
    public void ClosePlayertime()
    {
        wave_state = false;
    }

    protected override void OnPress(GameObject obj, bool state)
    {
        if (obj.name == "boss_buff_icon")
        {
            if (state)
            {
                if(!boss_buff_desc_.activeSelf)
                    boss_buff_desc_.gameObject.SetActive(true);
            }
            else
            {
                if(boss_buff_desc_.activeSelf)
                    boss_buff_desc_.gameObject.SetActive(false);
            }
        }
    }

    public void XueGuaiBuff()
    {
        if (BattlePlayers.me == null)
        {
            if (boss_buff_obj_.activeSelf)
                boss_buff_obj_.SetActive(false);
            return;
        }

        var bp = BattlePlayers.me;
        if (!bp.player.buffs.Contains(4001))
        {
            if (boss_buff_obj_.activeSelf)
                boss_buff_obj_.SetActive(false);
            return;
        }

        int end_zhen = 0;
        for (int i = 0; i < bp.player.buffs.Count; i++)
        {
            if (bp.player.buffs[i] == 4001)
            {
                end_zhen = bp.player.buffs_time[i];
                break;
            }
        }

        if (BattlePlayers.zhen >= end_zhen)
        {
            if (boss_buff_obj_.activeSelf)
                boss_buff_obj_.SetActive(false);
            return;
        }
        else
        {
            if(!boss_buff_obj_.activeSelf)
                boss_buff_obj_.SetActive(true);
        }

        t_battle_buff boss_buff = Config.get_t_battle_buff(4001);
        int dvalue = (end_zhen - BattlePlayers.zhen) * BattlePlayers.TICK;
        float p = (boss_buff.time - dvalue) * 1.0f / boss_buff.time;
        boss_buff_sp_.fillAmount = p;
        boss_buff_label_.text = (dvalue / 1000).ToString();
    }

    public void TalentFly(int change_talent_id)
    {
        var bp = BattlePlayers.me;
        if (bp == null)
            return;

        Transform ori_objt = null;
        for (int i = 0; i < talent_time_list.Count; i++)
        {
            if (talent_time_list[i].talent_id == change_talent_id)
            {
                ori_objt = talent_time_list[i].objt;
                talent_time_list.RemoveAt(i);
                break;
            }
        }

        if (ori_objt == null)
            return;
        Dictionary<int, Vector3> talent_p = new Dictionary<int, Vector3>();
        Dictionary<int, int> talent_dic = new Dictionary<int, int>();
        for(int i = 0;i < bp.player.talent_id.Count;i++)
            talent_dic.Add(bp.player.talent_id[i],bp.player.talent_level[i]);

        int index = 0;
        var dict = from obj in talent_dic orderby obj.Key select obj;
        foreach (KeyValuePair<int, int> item in dict)
        {
            talent_p.Add(item.Key, new Vector3(index * 56 + 28 - talent_dic.Count * 56.0f / 2, 0, 0));
            index = index + 1;
        }

        var info = get_type_obj(4);
        var v = ori_objt.TransformPoint(info.objt.localPosition);
        var tp = info.obj.transform.InverseTransformPoint(v);
        bobjpool.set_localPosition(info.objid, tp.x, tp.y, tp.z);
        info.obj.SetActive(true);
        v = talentBall_panel_.TransformPoint(info.objt.localPosition);
        var t_pos = info.objt.InverseTransformPoint(v);
        t_pos.z = 2;
        t_pos.x = t_pos.x + talent_p[change_talent_id].x;
        var x_1 = BattlePanel.Random(tp.x, t_pos.x);
        var x_2 = BattlePanel.Random(tp.x, t_pos.x);
        var y_1 = BattlePanel.Random(tp.y, t_pos.y);
        var y_2 = BattlePanel.Random(tp.y, t_pos.y);
        var p1 = new Vector3(x_1, y_1, 2);
        var p2 = new Vector3(x_2, y_2, 2);
        task_unit tu = new task_unit();
        tu.execute =  delegate (task_unit tt)
        {
            foreach (var item in Config.t_talents)
            {
                if (!talent_dic.ContainsKey(item.Key) && talent_ball_pools_.ContainsKey(item.Key) && talent_ball_pools_[item.Key].obj.activeSelf)
                    talent_ball_pools_[item.Key].obj.SetActive(false);
            }

            var dic = from obj in talent_p orderby obj.Key select obj;
            foreach (KeyValuePair<int, Vector3> item in dic)
            {
                var target_item = GetTalentBallByID(item.Key);
                bobjpool.set_localPosition(target_item.objid, item.Value.x, item.Value.y, item.Value.z);
                bobjpool.set_localScale(target_item.objid, 1, 1, 1);
                var label = target_item.objt.Find("sprite/num").GetComponent<UILabel>();
                label.text = talent_dic[item.Key].ToString();
                target_item.objt.gameObject.SetActive(true);
            }
        };

        pos_t_.paths.Add(pos_t_.length, new effect_path() { isMulti = false, s_path = new single_path() { st_p = tp, ed_p = t_pos, mid_p1 = p1, mid_p2 = p2, cur_t = 0, total_t = 0.5f, dt = info } });
        pos_t_.r_time.Add(pos_t_.length, 0);
        pos_t_.funcs.Add(pos_t_.length, tu);
        pos_t_.length = pos_t_.length + 1;
    }


    public void CMSG_OFFLINE_BATTLE_END(int rank)
    {
        if (Battle.is_online)
            return;
        var msg = new cmsg_offline_battle_end();
        msg.rank = rank;
        GameTcp.Send<cmsg_offline_battle_end>(opclient_t.CMSG_OFFLINE_BATTLE_END, msg);
    }

    public void PickUpAnimation(Vector3 s_pos,double s, double t,bool exp_bar = false)
    {
        if (exp_bar)
        {
            var dt = get_type_obj(1);
            var target_obj = exp_;
            var offset_x = (exp_.value - 0.5f) * target_obj.transform.GetComponent<UIWidget>().width;
            var v = target_obj.transform.TransformPoint(dt.objt.localPosition);
            var tp = dt.objt.InverseTransformPoint(v);
            tp.z = 2;
            var star_pos = s_pos + new Vector3(0, 0, 2);
            tp.x = tp.x + offset_x;
            var end_pos = tp;

            var x_1 = BattlePanel.Random(star_pos.x, end_pos.x);
            var x_2 = BattlePanel.Random(star_pos.x, end_pos.x);
            var y_1 = BattlePanel.Random(star_pos.y, end_pos.y);
            var y_2 = BattlePanel.Random(star_pos.y, end_pos.y);

            var p1 = new Vector3(x_1, y_1, 2);
            var p2 = new Vector3(x_2, y_2, 2);
            var use_time = 0.5f;
            pos_t_.paths.Add(pos_t_.length, new effect_path() { isMulti = false, s_path = new single_path() { st_p = star_pos, ed_p = end_pos, mid_p1 = p1, mid_p2 = p2, cur_t = 0, total_t = use_time, dt = dt } });
            pos_t_.r_time.Add(pos_t_.length, 0);
            task_unit tu = new task_unit();
            tu.s = s;
            tu.t = t;
            tu.execute = delegate (task_unit tt)
            {
                if (!BattlePlayers.me.is_die)
                {
                    exp_change_state_ = true;
                    light_state_ = true;
                    SetProgress(tt.s, tt.t);
                }
                else
                {
                    exp_change_state_ = false;
                    light_state_ = false;
                }
            };
            pos_t_.funcs.Add(pos_t_.length, tu);
            pos_t_.length = pos_t_.length + 1;
        }
        else
        {
            exp_change_state_ = true;
            SetProgress(s, t);
        }
    }

    public void SetProgress(double old_exp, double new_exp)
    {
        if (old_exp == new_exp)
            return;

        if (new_exp > end_pos_)
            end_pos_ = new_exp;
    }

    public void UpdateExpLighting()
    {
        if (!light_state_)
            return;

        var sp = exp_.transform.Find("expft").GetComponent<UISprite>();
        float num = sp.white;
        if (num >= 10)
            get_max_white_ = true;
        if (get_max_white_)
            num = num - Time.deltaTime / Time.timeScale * 16;
        else
            num = num + Time.deltaTime / Time.timeScale * 20;

        if (num <= 1)
            num = 1;

        sp.white = num;
        if (get_max_white_ && num <= 1)
        {
            light_state_ = false;
            get_max_white_ = false;
        }
    }

    public void UpdateExp()
    {
        if (current_pos_ >= end_pos_)
        {
            current_pos_ = end_pos_;
            star_pos_ = current_pos_;
        }
        else
        {
            exp_.value = GetPercent(current_pos_);
            current_pos_ = current_pos_ + 0.01;
        }
    }
    public float GetPercent(double num)
    {
        var t_num = BattleOperation.toInt(num * 100);
        while (t_num >= 100)
            t_num = t_num - 100;
        return t_num / 100.0f;
    }

    public void UpdateExpProgress()
    {
        UpdateExpLighting();
        UpdateExp();
        if (!light_state_ && current_pos_ == end_pos_)
            exp_change_state_ = false;
    }

    public void time_stage()
    {
        var zhen = BattlePlayers.TNUM * 60 * Battle.turnTime + Battle.exTime - BattlePlayers.zhen;
        if (zhen < 0)
            zhen = 0;
        var miao = BattleOperation.toInt(zhen / BattlePlayers.TNUM);
        if (miao <= 120 && miao > 10)
        {
            if (!end_fin_title_.gameObject.activeSelf)
                end_fin_title_.gameObject.SetActive(true);

            if (!end_fin_nor_label_.gameObject.activeSelf)
                end_fin_nor_label_.gameObject.SetActive(true);

            if (end_fin_red_label_.gameObject.activeSelf)
                end_fin_red_label_.gameObject.SetActive(false);
        }
        else if (miao <= 10 && miao >= 0)
        {
            if (!end_fin_title_.gameObject.activeSelf)
                end_fin_title_.gameObject.SetActive(true);

            if (end_fin_nor_label_.gameObject.activeSelf)
                end_fin_nor_label_.gameObject.SetActive(false);

            if (!end_fin_red_label_.gameObject.activeSelf)
                end_fin_red_label_.gameObject.SetActive(true);
        }
        else
        {
            if (end_fin_title_.gameObject.activeSelf)
                end_fin_title_.gameObject.SetActive(false);
        }

        if(miao <= 120 && miao > 10)
        {
            end_fin_title_.text = Config.get_t_script_str("BattlePanel_027");//"双倍得分时间";
            end_fin_nor_label_.text = string.Format("{0:D2}:{1:D2}", (int)Math.Abs(Math.Floor(miao / 60.0)),(int)Math.Abs(miao % 60));
        }
        else if (miao <= 10 && miao >= 0)
        {
            if (end_time_play_state != miao)
            {
                PlaySound("timeup");
                end_time_play_state = miao;
            }
            end_fin_title_.text = Config.get_t_script_str("BattlePanel_028"); //"本局即将结束";
            end_fin_red_label_.text = string.Format("{0:D2}:{1:D2}", (int)Math.Abs(Math.Floor(miao / 60.0)), (int)Math.Abs(miao % 60));
        }
    }

    public void refresh_self_teamer_info()
    {
        if (BattlePlayers.Camps.Count <= 0 || BattlePlayers.me == null)
            return;
        var pt = BattlePlayers.me;
        int sum = 0;
        List<team_arrow_info> tmp = new List<team_arrow_info>();
        if (teamer_arrows_ != null)
        {
            foreach (var item in teamer_arrows_)
            {
                tmp.Add(item.Value);
                sum += 1;
            }
        }

        List<string> self_teammate_list = new List<string>();
        for (int i = 0; i < BattlePlayers.Camps[pt.player.camp].Count; i++)
        {
            if (BattlePlayers.Camps[pt.player.camp][i] != pt.player.guid)
                self_teammate_list.Add(BattlePlayers.Camps[pt.player.camp][i]);
        }

        if (teamer_arrows_ != null && sum > 0)
        {
            if (sum > self_teammate_list.Count)
            {
                for (int i = sum - 1; i >= self_teammate_list.Count; i--)
                {
                    GameObject.Destroy(tmp[i].arrow_m.obj);
                    bobjpool.remove(tmp[i].arrow_m.objid);
                    tmp.RemoveAt(i);
                }
            }
            teamer_arrows_ = new Dictionary<string, team_arrow_info>();
            for (int i = 0; i < self_teammate_list.Count; i++)
            {
                var bp = BattlePlayers.players[self_teammate_list[i]] as BattleAnimalPlayer;
                var data = tmp[i];
                data.pt = bp;
                teamer_arrows_.Add(bp.player.guid, data);
            }
        }
        else
        {
            teamer_arrows_ = new Dictionary<string, team_arrow_info>();
            for (int i = 0; i < self_teammate_list.Count; i++)
            {
                var bp = BattlePlayers.players[self_teammate_list[i]] as BattleAnimalPlayer;
                var arrow = get_teamer_arrow();
                arrow.label.text = bp.animal.name;
                teamer_arrows_.Add(bp.animal.guid, new team_arrow_info() { pt = bp, arrow_m = arrow, name = bp.animal.name });
            }
        }
    }

    public team_arrow_sub_info get_teamer_arrow()
    {
        GameObject obj = LuaHelper.Instantiate(teamer_res_);
        var objid = bobjpool.add(obj);
        var objt = obj.transform;
        var label_id = bobjpool.add(objt.Find("rank").gameObject);
        var arr_t = objt.Find("rank/arrow");
        var arrow_id = bobjpool.add(arr_t.gameObject);
        obj.transform.parent = teamer_guide_panel_.transform;
        bobjpool.set_localPosition(objid, 0, 0, 0);
        bobjpool.set_localScale(objid, 1, 1, 1);

        team_arrow_sub_info info = new team_arrow_sub_info() { obj = obj, objid = objid, objt = objt, label_id = label_id, label = objt.Find("rank").GetComponent<UILabel>(),arrow_id = arrow_id,arr_t = arr_t };
        return info;
    }

    //新手引导相关
    public void EnableButton(string btn_ids)
    {
        if (btn_ids == "")
            DisableButtonClickLimit();
        else
            EnableButtonClickLimit(btn_ids);
    }

    public GameObject get_obj_by_path(string path)
    {
        return ui_root_.transform.Find(path).gameObject;
    }
    public GameObject get_wheel_by_id(int wheel_id)
    {
        if (wheel_id == 1 || wheel_id == 2 || wheel_id == 3 || wheel_id == 4)
            return wheelboxs_[wheel_id - 1].gameObject;
        return null;
    }

    public void SetWheelStatus(int wheel_num)
    {
        if (wheel_num == 1)
        {
            jskill_mask_obj_.SetActive(true);
            jskill_2_mask_obj_.SetActive(false);
            jskill_3_mask_obj_.SetActive(false);
        }
        else if (wheel_num == 2)
        {
            jskill_mask_obj_.SetActive(false);
            jskill_2_mask_obj_.SetActive(true);
            jskill_3_mask_obj_.SetActive(false);
        }
        else if (wheel_num == 3)
        {
            jskill_mask_obj_.SetActive(true);
            jskill_2_mask_obj_.SetActive(true);
            jskill_3_mask_obj_.SetActive(false);
        }
        else if (wheel_num == 4)
        {
            jskill_mask_obj_.SetActive(false);
            jskill_2_mask_obj_.SetActive(false);
            jskill_3_mask_obj_.SetActive(true);
        }
        else if (wheel_num == 5)
        {
            jskill_mask_obj_.SetActive(true);
            jskill_2_mask_obj_.SetActive(false);
            jskill_3_mask_obj_.SetActive(true);
        }
        else if (wheel_num == 6)
        {
            jskill_mask_obj_.SetActive(false);
            jskill_2_mask_obj_.SetActive(true);
            jskill_3_mask_obj_.SetActive(true);
        }
        else if (wheel_num == 7)
        {
            jskill_mask_obj_.SetActive(true);
            jskill_2_mask_obj_.SetActive(true);
            jskill_3_mask_obj_.SetActive(true);
        }
        else
        {
            jskill_mask_obj_.SetActive(false);
            jskill_2_mask_obj_.SetActive(false);
            jskill_3_mask_obj_.SetActive(false);
            skill_4_mask_obj.SetActive(false);
        }
    }

    public void set_guide_3d(float x, float y, float z,t_guide_init gd, bool is_exist= false)
    {
        guide_obj obj = null;
        if (is_exist)
            obj = NewPlayerGuide.get_current_step_obj(gd.id);
        else
        {
            obj = get_guides_obj(0);
            NewPlayerGuide.set_current_step_obj(gd.id, obj);
        }
        var tmp = NavUtil.worldpos_to_localpos(new Vector3(x, y, z), obj.obj);
        tmp = tmp + new Vector3(gd.offset_x, gd.offset_y, 0);
        bobjpool.set_localPosition(obj.objid, tmp.x, tmp.y, tmp.z);
        obj.guide_label.text = gd.param_string;
    }

    public void new_player_guide_end()
    {
        if (!guide_end_panel_.activeSelf)
        {
            DisableButtonClickLimit();
            guide_end_panel_.SetActive(true);
        }  
    }

    public guide_obj get_guides_obj(int type_id)
    {
        for (int i = 0; i < guide_objs_.Count; i++)
        {
            if (guide_objs_[i].state == 0 && guide_objs_[i].type == type_id)
            {
                guide_objs_[i].state = 1;
                guide_objs_[i].obj.SetActive(true);             
                bobjpool.set_localPosition(guide_objs_[i].objid, 0, 0, 0);
                bobjpool.set_localScale(guide_objs_[i].objid, 1, 1, 1);
                return guide_objs_[i];
            }
        }

        GameObject obj = null;
        if (type_id == 0)
            obj = LuaHelper.Instantiate(guide_res);
        else
            obj = LuaHelper.Instantiate(guide_res_left_);

        var objid = bobjpool.add(obj);
        var objt = obj.transform;
        objt.parent = guide_panel_;
        bobjpool.set_localPosition(objid, 0, 0, 0);
        bobjpool.set_localScale(objid, 1, 1, 1);

        var guide_label = objt.Find("guide_label").GetComponent<UILabel>();
        obj.SetActive(true);
        guide_obj gs = new guide_obj() { state = 1, guide_label = guide_label, obj = obj, type = type_id, objt = obj.transform, objid = objid };
        guide_objs_.Add(gs);
        return gs;
    }

    public void set_guide(GameObject obj,t_guide_init gd,int is_left = 0)
    {
        guide_obj guideoj = null;

        guideoj = NewPlayerGuide.get_current_step_obj(gd.id);
        if (guideoj == null)
        {
            guideoj = get_guides_obj(is_left);
            NewPlayerGuide.set_current_step_obj(gd.id, guideoj);
        }

        var v = obj.transform.TransformPoint(guideoj.objt.localPosition);
        var tp = guideoj.objt.InverseTransformPoint(v);
        if (is_left == 1)
            tp = tp + new Vector3(300, 80, 0);
        tp = tp + new Vector3(gd.offset_x, gd.offset_y, 0);
        bobjpool.set_localPosition(guideoj.objid, tp.x, tp.y, tp.z);
        if (String.IsNullOrEmpty(gd.param_string))
            guideoj.guide_label.gameObject.SetActive(false);
        else
        {
            guideoj.guide_label.text = Config.get_t_lang(gd.param_string);
            guideoj.guide_label.gameObject.SetActive(true);
        }
    }

    public void NewGuidePlaySound(string sound_name)
    {
        NavUtil.PlaySound(audiosource_, sound_name);
    }
    public void DrawCalSum(int current,int limit)
    {
        guide_extra_lb_.text = current + "/" + limit;
    }

    public void DrawCalTime(int time)
    {
        guide_extra_lb_.text = time.ToString();
    }

    public void ShowRestartTask(Action ac)  //提示任务失败重新开始
    {
        if (!task_restart_.activeInHierarchy)
            task_restart_.SetActive(true);
        var btnObj = task_restart_.transform.Find("btn").gameObject;
        UIEventListener.VoidDelegate uv = delegate (GameObject go)
        {
            if (task_restart_.activeInHierarchy)
                task_restart_.SetActive(false);
            ac();
        };
        RegisterOnClick(btnObj, uv);
    }

    public void show_newplay_mask(string content,int dialog_type)
    {
        guide_extra_achieveLog.gameObject.SetActive(false);
        if (dialog_type == 0)
        {
            guide_random_area_panel_.transform.Find("skip_step").gameObject.SetActive(true);
            guide_random_area_panel_.transform.Find("down_label").gameObject.SetActive(true);
            guide_random_area_panel_.transform.Find("mt").gameObject.SetActive(true);
        }
        else if (dialog_type == 1 || dialog_type == 3)
        {
            guide_random_area_panel_.transform.Find("skip_step").gameObject.SetActive(false);
            guide_random_area_panel_.transform.Find("down_label").gameObject.SetActive(false);
            guide_random_area_panel_.transform.Find("mt").gameObject.SetActive(false);
        }
        else if (dialog_type == 2)
        {
            guide_random_area_panel_.transform.Find("skip_step").gameObject.SetActive(true);
            guide_random_area_panel_.transform.Find("down_label").gameObject.SetActive(true);
            guide_random_area_panel_.transform.Find("mt").gameObject.SetActive(true);
        }

        if (dialog_type == 6)
        {
            guide_random_area_panel_.transform.Find("skip_step").gameObject.SetActive(true);
            guide_random_area_panel_.transform.Find("down_label").gameObject.SetActive(true);
            guide_random_area_panel_.transform.Find("mt").gameObject.SetActive(true);
            guide_extra_achieveLog.gameObject.SetActive(true);
        }

        if (dialog_type != 2)
        {
            var cl = guide_title_.GetComponent<UILabel>();
            cl.text = content;
            guide_title_.SetActive(true);
        }
        else
            guide_title_.SetActive(false);

        if (dialog_type == 3)
        {
            guide_extra_lb_.text = "";          
            guide_extra_lb_.gameObject.SetActive(true);
        }
        else        
            guide_extra_lb_.gameObject.SetActive(false);

        guide_random_area_panel_.SetActive(true);        
    }
    public void hide_newplay_mask()
    {
        if (guide_random_area_panel_.activeSelf)
            guide_random_area_panel_.SetActive(false);
        if (guide_title_.activeSelf)
            guide_title_.SetActive(false);
    }

    public void clear_new_guide()
    {
        foreach (var item in guide_objs_)
        {
            if (item != null)
            {
                GameObject.Destroy(item.obj);
                bobjpool.remove(item.objid);
            }
        }
        guide_objs_.Clear();
    }
    protected override void OnHover(GameObject go, bool isOver)
    {
        base.OnHover(go, isOver);
    }
    protected override void OnDrag(GameObject go, Vector2 delta)
    {
        base.OnDrag(go, delta);
    }

    protected override void OnDragFinished(GameObject go)
    {
        base.OnDragFinished(go);
    }
}

