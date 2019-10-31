using BattleDB;
using protocol.game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
/*
public class ForeBattle:BEventListener
{
    public class talent_obj
    {
        public UILabel label;
        public UISprite icon;
        public GameObject obj;
        public Transform objt;
        public int objid;
        public GameObject cicleObj;
        public Collider collider;
    }
    public class light_obj
    {
        public GameObject obj;
        public Transform objt;
        public int objid;
        public GameObject lightObj;
    }

    public List<int> show_skills = new List<int>();
    public bobjpool bobjpool = null;
    public int light_t = 0, offline_rolls = 0, EnterTime = 0;
    private bool runState = false;
    private bool runState2 = false;
    private bool runState3 = false;
    private bool waitForPlaySound = false;
    public int[] turnTime = new int[] { 0, 0, 0 };

    private List<t_talent> all_talents = null;
    private List<t_skill> all_skills = null;
    private List<talent_obj> talentObjs, skill1Objs, skill2Objs;
    private List<light_obj> lightObjs;

    private UILabel remain_time_lb_, refresh_time_lb_, cost_label_;
    private Animator langan_anim_, left_anim, mid_anim, right_anim;

    private void Awake()
    {
        remain_time_lb_ = this.transform.Find("center_anchor/djs").GetComponent<UILabel>();
        refresh_time_lb_ = this.transform.Find("center_anchor/daily/times_label").GetComponent<UILabel>();
        cost_label_ = this.transform.Find("center_anchor/reward_frame/cost/label").GetComponent<UILabel>();

        if (Battle.is_online)
        {
            this.transform.Find("center_anchor/reward_frame/offline_cost").gameObject.SetActive(false);
            this.transform.Find("center_anchor/reward_frame/cost").gameObject.SetActive(true);
        }
        else
        {
            this.transform.Find("center_anchor/reward_frame/offline_cost").gameObject.SetActive(true);
            this.transform.Find("center_anchor/reward_frame/cost").gameObject.SetActive(false);
        }

        var resObj = this.transform.Find("center_anchor/rollRes").gameObject;
        bobjpool = new bobjpool();

        var ball = this.transform.Find("center_anchor/reward_frame/lg/ball").gameObject;
        RegisterOnClick(ball, OnClick);

        var ctObj = this.transform.Find("center_anchor/reward_frame/ct").gameObject;
        RegisterOnClick(ctObj, OnClick);

        var enter_btn = this.transform.Find("center_anchor/enter").gameObject;
        RegisterOnClick(enter_btn, OnClick);

        langan_anim_ = this.transform.Find("center_anchor/reward_frame/lg").GetComponent<Animator>();
        left_anim = this.transform.Find("center_anchor/reward_panel/left_area").GetComponent<Animator>();
        mid_anim = this.transform.Find("center_anchor/reward_panel/mid_area").GetComponent<Animator>();
        right_anim = this.transform.Find("center_anchor/reward_panel/right_area").GetComponent<Animator>();

        talentObjs = new List<talent_obj>();
        var left_area_t = this.transform.Find("center_anchor/reward_panel/left_area");
        for (int i = 0; i < left_area_t.transform.childCount; i++)
        {
            var childObj = left_area_t.transform.GetChild(i).gameObject;
            var icon = childObj.transform.GetComponent<UISprite>();
            childObj.transform.localScale = Vector3.one;
            childObj.transform.localPosition = new Vector3(0, 100 - 100 * i, 0);

            var objid = bobjpool.add(childObj);
            var cicleObj = childObj.transform.Find("border").gameObject;
            var collider = childObj.transform.GetComponent<BoxCollider>();
            var label = childObj.transform.Find("label").GetComponent<UILabel>();
            collider.enabled = false;
            RegisterOnClick(childObj, OnClick);
            talentObjs.Add(new talent_obj() { label = label, icon = icon, obj = childObj, objt = childObj.transform, objid = objid, cicleObj = cicleObj, collider = collider });
        }

        skill1Objs = new List<talent_obj>();
        var mid_area_t = this.transform.Find("center_anchor/reward_panel/mid_area");
        for (int i = 0; i < mid_area_t.transform.childCount; i++)
        {
            var childObj = mid_area_t.transform.GetChild(i).gameObject;

            var icon = childObj.transform.GetComponent<UISprite>();
            childObj.transform.localScale = Vector3.one;
            childObj.transform.localPosition = new Vector3(0, 100 - 100 * i, 0);

            var objid = bobjpool.add(childObj);
            var cicleObj = childObj.transform.Find("border").gameObject;
            var collider = childObj.transform.GetComponent<BoxCollider>();
            var label = childObj.transform.Find("label").GetComponent<UILabel>();
            collider.enabled = false;
            RegisterOnClick(childObj, OnClick);
            skill1Objs.Add(new talent_obj() { label = label, icon = icon, obj = childObj, objt = childObj.transform, objid = objid, cicleObj = cicleObj, collider = collider });
        }

        skill2Objs = new List<talent_obj>();
        var right_area_t = this.transform.Find("center_anchor/reward_panel/right_area");
        for (int i = 0; i < right_area_t.transform.childCount; i++)
        {
            var childObj = right_area_t.transform.GetChild(i).gameObject;

            var icon = childObj.transform.GetComponent<UISprite>();
            childObj.transform.localScale = Vector3.one;
            childObj.transform.localPosition = new Vector3(0, 100 - 100 * i, 0);

            var objid = bobjpool.add(childObj);
            var cicleObj = childObj.transform.Find("border").gameObject;
            var collider = childObj.transform.GetComponent<BoxCollider>();
            var label = childObj.transform.Find("label").GetComponent<UILabel>();
            collider.enabled = false;
            RegisterOnClick(childObj, OnClick);
            skill2Objs.Add(new talent_obj() { label = label, icon = icon, obj = childObj, objt = childObj.transform, objid = objid, cicleObj = cicleObj, collider = collider });
        }

        lightObjs = new List<light_obj>();
        for (int i = 1; i <= 4; i++)
        {
            var t = this.transform.Find("center_anchor/reward_frame/light/light_"+i).gameObject;
            var lightObj = t.transform.Find("li").gameObject;
            var objid = bobjpool.add(t);
            lightObjs.Add(new light_obj() { obj = t, objt = t.transform, objid = objid, lightObj = lightObj });
        }

        all_talents = new List<t_talent>();
        all_skills = new List<t_skill>();

        var dict = from obj in Config.t_talents orderby obj.Key select obj;
        foreach (KeyValuePair<int, t_talent> item in dict)
            all_talents.Add(item.Value);

        var dict1 = from obj in Config.t_skills orderby obj.Key select obj;
        foreach (KeyValuePair<int, Dictionary<int, t_skill>> item in dict1)
        {
            var t_skill = Config.get_t_skill(item.Key);
            if (t_skill.type == 2 && t_skill.id != 400201 && t_skill.id != 400301)
                all_skills.Add(t_skill);
        }

        runState = false;
        runState2 = false;
        runState3 = false;
        waitForPlaySound = false;
        RegisterMessage();
        offline_rolls = 0;
    }

    private void RegisterMessage()
    {
        CSharpNetMessage.AddCSharpNetEvent(opclient_t.SMSG_BATTLE_RESET_SKILL_HALL, SMSG_BATTLE_RESET_SKILL_HALL,true);
        CSharpNetMessage.AddCSharpNetEvent(opclient_t.SMSG_BATTLE_RESET_SKILL, SMSG_BATTLE_RESET_SKILL,true);
    }

    private void RemoveMessage()
    {
        CSharpNetMessage.RemoveCSharpNetEvent(opclient_t.SMSG_BATTLE_RESET_SKILL_HALL, SMSG_BATTLE_RESET_SKILL_HALL);
        CSharpNetMessage.RemoveCSharpNetEvent(opclient_t.SMSG_BATTLE_RESET_SKILL, SMSG_BATTLE_RESET_SKILL);
    }

    public override void OnParam(params object[] param)
    {
        if (param.Length < 1)
            return;
        
        show_skills = param[0] as List<int>;

        Animation();
        EnterTime = BattlePlayers.zhen;
        light_t = BattlePlayers.zhen;
    }

    private void Animation()
    {
        langan_anim_.enabled = true;
        langan_anim_.Rebind();
        PlaySound("switch");
        waitForPlaySound = true;

        for (int i = 0; i < talentObjs.Count; i++)
        {
            if (talentObjs[i].cicleObj.activeInHierarchy)
            {
                talentObjs[i].cicleObj.SetActive(false);
                talentObjs[i].collider.enabled = false;
            }
        }

        for (int i = 0; i < skill1Objs.Count; i++)
        {
            if (skill1Objs[i].cicleObj.activeInHierarchy)
            {
                skill1Objs[i].cicleObj.SetActive(false);
                skill1Objs[i].collider.enabled = false;
            }
        }

        for (int i = 0; i < skill2Objs.Count; i++)
        {
            if (skill2Objs[i].cicleObj.activeInHierarchy)
            {
                skill2Objs[i].cicleObj.SetActive(false);
                skill2Objs[i].collider.enabled = false;
            }
        }

        if (Battle.is_online)
        {
            refresh_time_lb_.text = self.player.battle_reset_skill_num + "/3";
            cost_label_.text = "x" + (self.player.battle_reset_skill_num * 10 + 10);
        }
        else
            refresh_time_lb_.text = offline_rolls+"/3";

        var collo = new List<t_talent>();
        List<int> ids = new List<int>();
        int total = talentObjs.Count - 1;

        while (total > 0)
        {
            var id = BattleOperation.random(0, all_talents.Count);
            if (all_talents[id].id != show_skills[0] && !ids.Contains(id))
            {
                ids.Add(id);
                collo.Add(all_talents[id]);
                total = total - 1;
                if (total == 1)
                    collo.Add(Config.get_t_talent(show_skills[0]));
            }
        }
        
        for (int i = 0; i < talentObjs.Count; i++)
        {
            talentObjs[i].icon.atlas = IconPanel.GetAltas(collo[i].icon);
            talentObjs[i].icon.spriteName = collo[i].icon;
            talentObjs[i].label.text = collo[i].id.ToString();
        }

        var collo1 = new List<t_skill>();
        ids.Clear();
        total = skill1Objs.Count - 1;
        while (total > 0)
        {
            var id = BattleOperation.random(0, all_skills.Count);
            if (all_skills[id].id != show_skills[1] && !ids.Contains(id))
            {
                ids.Add(id);
                collo1.Add(all_skills[id]);
                total = total - 1;
                if(total == 1)
                    collo1.Add(Config.get_t_skill(show_skills[1]));
            }
        }

        for (int i = 0; i < skill1Objs.Count; i++)
        {
            skill1Objs[i].icon.atlas = IconPanel.GetAltas(collo1[i].icon);
            skill1Objs[i].icon.spriteName = collo1[i].icon;
            skill1Objs[i].label.text = collo1[i].id.ToString();
        }


        collo1.Clear();
        ids.Clear();
        total = skill2Objs.Count - 1;
        while (total > 0)
        {
            var id = BattleOperation.random(0, all_skills.Count);
            if (all_skills[id].id != show_skills[2] && !ids.Contains(id))
            {
                ids.Add(id);
                collo1.Add(all_skills[id]);
                total = total - 1;
                if(total == 1)
                    collo1.Add(Config.get_t_skill(show_skills[2]));
            }
        }
        
        for (int i = 0; i < skill2Objs.Count; i++)
        {
            skill2Objs[i].icon.atlas = IconPanel.GetAltas(collo1[i].icon);
            skill2Objs[i].icon.spriteName = collo1[i].icon;
            skill2Objs[i].label.text = collo1[i].id.ToString();
        }

        runState = true;
        runState2 = true;
        runState3 = true;
        for (int i = 0; i < turnTime.Length; i++)
            turnTime[i] = 0;
    }

    private void Update()
    {
        var t = BattleOperation.toInt(30 - (BattlePlayers.zhen - EnterTime) * BattlePlayers.TICK / 1000.0);
        if (t <= 0)
        {
            GUIRoot.HideGUI("ForeBattle");
            Battle.send_in();
            Battle.ShowBattlePanel();
            return;
        }
        else
            remain_time_lb_.text = t.ToString();

        if (t > 0)
        {
            var t1 = (BattlePlayers.zhen - light_t) * BattlePlayers.TICK / 1000.0;
            if (runState || runState2 || runState3)
            {
                if (t1 >= 0.1)
                {
                    lightState();
                    light_t = BattlePlayers.zhen;
                }
                else
                {
                    if (t1 >= 0.5)
                    {
                        lightState();
                        light_t = BattlePlayers.zhen;
                    }
                }
            }
        }

        if (runState || runState2 || runState3 && !GUIRoot.guis.ContainsKey("LoadPanel"))
        {
            if (waitForPlaySound)
            {
                PlaySound("roll");
                waitForPlaySound = false;
            }

            if (runState)
            {
                if (turnTime[0] > 6 * 2)
                {
                    runState = false;
                    Show_Circle();
                    PlaySound("dingdong");
                }
                else if (turnTime[0] == 0)
                {
                    left_anim.enabled = true;
                    left_anim.Rebind();
                    left_anim.speed = 5;
                    left_anim.Play("roll_reward");
                    turnTime[0] = turnTime[0] + 1;
                }
                else
                {
                    var info = left_anim.GetCurrentAnimatorStateInfo(0);
                    if (info.normalizedTime >= 1.0)
                    {
                        left_anim.enabled = true;
                        left_anim.Rebind();
                        left_anim.speed = 5;
                        left_anim.Play("roll_reward");
                        turnTime[0] = turnTime[0] + 1;
                    }
                }
            }

            if (runState2)
            {
                if (turnTime[1] > 6 * 2 + 3)
                {
                    runState2 = false;
                    Show_Circle();
                    PlaySound("dingdong");
                }
                else if (turnTime[1] == 0)
                {
                    mid_anim.enabled = true;
                    mid_anim.Rebind();
                    mid_anim.speed = 5;
                    mid_anim.Play("roll_reward");
                    turnTime[1] = turnTime[1] + 1;
                }
                else
                {
                    var info = mid_anim.GetCurrentAnimatorStateInfo(0);
                    if (info.normalizedTime >= 1.0)
                    {
                        mid_anim.enabled = true;
                        mid_anim.Rebind();
                        mid_anim.speed = 5;
                        mid_anim.Play("roll_reward");
                        turnTime[1] = turnTime[1] + 1;
                    }
                }
            }

            if (runState3)
            {
                if (turnTime[2] > 6 * 2 + 6)
                {
                    runState3 = false;
                    Show_Circle();
                    PlaySound("dingdong");
                }
                else if (turnTime[2] == 0)
                {
                    right_anim.enabled = true;
                    right_anim.Rebind();
                    right_anim.speed = 5;
                    right_anim.Play("roll_reward");
                    turnTime[2] = turnTime[2] + 1;
                }
                else
                {
                    var info = right_anim.GetCurrentAnimatorStateInfo(0);
                    if (info.normalizedTime >= 1.0)
                    {
                        right_anim.enabled = true;
                        right_anim.Rebind();
                        right_anim.speed = 5;
                        right_anim.Play("roll_reward");
                        turnTime[2] = turnTime[2] + 1;
                    }
                }
            }
        }
    }

    private void Show_Circle()
    {
        if (!runState && !runState2 && !runState3)
        {
            talentObjs[4].cicleObj.SetActive(true);
            talentObjs[4].collider.enabled = true;
            skill1Objs[4].cicleObj.SetActive(true);
            skill1Objs[4].collider.enabled = true;
            skill2Objs[4].cicleObj.SetActive(true);
            skill2Objs[4].collider.enabled = true;
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < talentObjs.Count; i++)
        {
            bobjpool.remove(talentObjs[i].objid);
            GameObject.Destroy(talentObjs[i].obj);
        }

        for (int i = 0; i < skill1Objs.Count; i++)
        {
            bobjpool.remove(skill1Objs[i].objid);
            GameObject.Destroy(skill1Objs[i].obj);
        }

        for (int i = 0; i < skill2Objs.Count; i++)
        {
            bobjpool.remove(skill2Objs[i].objid);
            GameObject.Destroy(skill2Objs[i].obj);
        }

        for (int i = 0; i < lightObjs.Count; i++)
        {
            bobjpool.remove(lightObjs[i].objid);
            GameObject.Destroy(lightObjs[i].obj);
        }

        talentObjs = null;
        skill1Objs = null;
        skill2Objs = null;
        lightObjs = null;
        bobjpool.clear();
        RemoveMessage();
        Util.CallMethod("GUIRoot", "HideGUI", "DetailPanel");
        if(m_bundle)
            AppFacade._instance.ResourceManager.UnloadBundle(m_bundle_name);
    }

    private void lightState()
    {
        for (int i = 0; i < lightObjs.Count; i++)
        {
            if(lightObjs[i].lightObj.activeInHierarchy)
                lightObjs[i].lightObj.SetActive(false);
            else
                lightObjs[i].lightObj.SetActive(true);
        }
    }

    private void OnClick(GameObject obj)
    {
        if (obj.name == "ct" || obj.name == "ball")
        {
            if (!(runState || runState2 || runState3))
            {
                var t = BattleOperation.toInt(30 - (BattlePlayers.zhen - EnterTime) * BattlePlayers.TICK / 1000.0);
                if (t > 2)
                    CMSG_BATTLE_RESET_SKILL_HALL();
                else
                {
                    string s = @"client_key = { '入场前最后2秒无法刷新战前能力!'}";
                    LuaInterface.LuaTable lt = AppFacade._instance.LuaManager.GetLuaTable(s, "client_key");
                    Util.CallMethod("GUIRoot", "ShowGUI", "MessagePanel", lt);
                }
            }
        }
        else if (obj.name == "enter")
        {
            GUIRoot.HideGUI("ForeBattle");
            Battle.send_in();
            GUIRoot.ShowGUI<BattlePanel>("BattlePanel");
            return;
        }
        else
        {
            var id = Convert.ToInt32(obj.transform.Find("label").GetComponent<UILabel>().text);
            if (id < 100)
            {
                string s = @"client_key = { '11+" + id+"'}";
                LuaInterface.LuaTable t = AppFacade._instance.LuaManager.GetLuaTable(s, "client_key");
                Util.CallMethod("GUIRoot", "ShowGUI", "DetailPanel", t);
            }
            else
            {
                var item_id = GetItem(id);
                if (item_id != null)
                {
                    string s = @"client_key = { '2+" + item_id.GetValueOrDefault() + "'}";
                    LuaInterface.LuaTable t = AppFacade._instance.LuaManager.GetLuaTable(s, "client_key");
                    Util.CallMethod("GUIRoot", "ShowGUI", "DetailPanel", t);
                }
            }
        }
    }

    private int? GetItem(int skill_id)
    {
        foreach (var item in Config.t_items)
        {
            if (item.Value.def1 == skill_id)
                return item.Value.id;
        }
        return null;
    }

    //////////////////////////////////////////////////////////////////
    private void CMSG_BATTLE_RESET_SKILL_HALL()
    {
        if (!Battle.is_online)
        {
            if (offline_rolls >= 3)
            {
                string s = @"client_key = { '已达到最大次数!'}";
                LuaInterface.LuaTable t = AppFacade._instance.LuaManager.GetLuaTable(s, "client_key");
                Util.CallMethod("GUIRoot", "ShowGUI", "MessagePanel", t);
            }
            else
            {
                offline_rolls = offline_rolls + 1;
                OFFLINE_BATTLE_RESET_SKILL();
            }
            return;
        }

        if (self.player.battle_reset_skill_num >= 3)
        {
            string s = @"client_key = { '已达到最大次数!'}";
            LuaInterface.LuaTable t = AppFacade._instance.LuaManager.GetLuaTable(s, "client_key");
            Util.CallMethod("GUIRoot", "ShowGUI", "MessagePanel", t);
            return;
        }

        var jewel = 10 + self.player.battle_reset_skill_num * 10;
        if (self.player.jewel < jewel)
        {
            string s = @"client_key = { '钻石不足!'}";
            LuaInterface.LuaTable t = AppFacade._instance.LuaManager.GetLuaTable(s, "client_key");
            Util.CallMethod("GUIRoot", "ShowGUI", "MessagePanel", t);
        }
        else
        {
            List<string> smsg = new List<string>() { "opcodes.SMSG_BATTLE_RESET_SKILL_HALL" };
            GameTcp.SendCircle(opclient_t.CMSG_BATTLE_RESET_SKILL_HALL,smsg);
        }
    }

    private void OFFLINE_BATTLE_RESET_SKILL()
    {
        Battle.self_skills = Battle.RandomForeBattleMsg();
        show_skills = Battle.self_skills;
        Animation();
    }
    private void SMSG_BATTLE_RESET_SKILL_HALL(s_net_message msg)
    {
        var jewel = 10 + self.player.battle_reset_skill_num * 10;
        if (self.player.jewel < jewel)
        {
            string s = @"client_key = { '钻石不足!'}";
            LuaInterface.LuaTable t = AppFacade._instance.LuaManager.GetLuaTable(s, "param");
            Util.CallMethod("GUIRoot", "ShowGUI", "MessagePanel", t);
            return;
        }
        else
        {
            Util.CallMethod("PlayerData", "add_resource", 2, -jewel);
            self.add_resource(2, -jewel);
        }

        self.player.battle_reset_skill_num = self.player.battle_reset_skill_num + 1;
        Util.CallMethod("State", "reset_skill_num",1);
        Battle.SendBattleNullTcp(opclient_t.CMSG_BATTLE_RESET_SKILL);
    }

    private void SMSG_BATTLE_RESET_SKILL(s_net_message msg)
    {
        using (MemoryStream ms = new MemoryStream(msg.buffer))
        {
            smsg_battle_reset_skill sbs = ProtoBuf.Serializer.Deserialize<smsg_battle_reset_skill>(ms);
            Battle.self_skills = sbs.self_skills;
            show_skills = sbs.self_skills;
            Animation();
        }
    }
}

*/