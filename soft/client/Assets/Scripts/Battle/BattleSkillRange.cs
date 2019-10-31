using BattleDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class BattleSkillRange
{
    public static GameObject obj_ = null;
    public static GameObject obj_sub_ = null;
    public static List<Material> materials_ = new List<Material>();
    public static string name_ = "";
    public static int type_;
    public static float range_;
    public static bool cancel_ = false;
    public static float x_;
    public static float y_;
    public static float r_;
    public static string GetName()
    {
        return name_;
    }

    public static void Create(string name,int rtype,float range,float param)
    {
        BattleSkillRange.Destroy();
        name_ = name;
        type_ = rtype;
        range_ = range;
        range = range / Battle.BL;
        if (type_ != 1)
            param = param / Battle.BL;
        if (type_ == 1)
        {
            obj_ = LuaHelper.GetResManager().CreateEffect("skill_release_di");
            obj_.transform.parent = LuaHelper.GetResManager().UnitRoot;
            obj_.transform.localScale = new Vector3(range / 5, 1, range / 5);
            Material m = obj_.transform.Find("zd_fz001").GetComponent<Renderer>().material;
            materials_.Add(m);
            obj_sub_ = LuaHelper.GetResManager().CreateEffect("skill_release_shan");
            obj_sub_.transform.parent = LuaHelper.GetResManager().UnitRoot;
            obj_sub_.transform.localScale = new Vector3(range / 5, 1, range / 5);
            var left = obj_sub_.transform.Find("left");
            m = left.Find("zd_fz002c").GetComponent<Renderer>().material;
            materials_.Add(m);
            var right = obj_sub_.transform.Find("right");
            m = right.Find("zd_fz002a").GetComponent<Renderer>().material;
            materials_.Add(m);
            var center = obj_sub_.transform.Find("center");
            var num = Mathf.Floor(param / 15 - 2);
            left.localEulerAngles = new Vector3(0, -7.5f - 7.5f * num, 0);
            right.localEulerAngles = new Vector3(0, 7.5f + 7.5f * num, 0);
            for (int i = 0; i < num; i++)
            {
                var tcenter = LuaHelper.Instantiate(center.gameObject);
                tcenter.transform.parent = obj_sub_.transform;
                tcenter.transform.localPosition = Vector3.zero;
                tcenter.transform.localEulerAngles = new Vector3(0, i * 15 - 7.5f * (num - 1), 0);
                tcenter.transform.localScale = Vector3.one;
                tcenter.SetActive(true);
                m = tcenter.transform.Find("zd_fz002b").GetComponent<Renderer>().material;
                materials_.Add(m);
            }
        }
        else if (type_ == 2)
        {
            obj_ = LuaHelper.GetResManager().CreateEffect("skill_release_di");
            obj_.transform.parent = LuaHelper.GetResManager().UnitRoot;
            obj_.transform.localScale = new Vector3(range / 5, 1, range / 5);
            var m = obj_.transform.Find("zd_fz001").GetComponent<Renderer>().material;
            materials_.Add(m);
            obj_sub_ = LuaHelper.GetResManager().CreateEffect("skill_release_jian");
            obj_sub_.transform.parent = LuaHelper.GetResManager().UnitRoot;
            obj_sub_.transform.localScale = new Vector3(param / 2, 1, 1);
            var head = obj_sub_.transform.Find("head");
            head.localPosition = new Vector3(0, 0, range - 2);
            m = head.Find("zd_fz003b").GetComponent<Renderer>().material;
            materials_.Add(m);
            var tail = obj_sub_.transform.Find("tail");
            tail.localScale = new Vector3(1, 1, (range - 2) / 3);
            m = tail.Find("zd_fz003a").GetComponent<Renderer>().material;
            materials_.Add(m);
        }
        else if (type_ == 3)
        {
            obj_ = LuaHelper.GetResManager().CreateEffect("skill_release_di");
            obj_.transform.parent = LuaHelper.GetResManager().UnitRoot;
            obj_.transform.localScale = new Vector3(range / 5, 1, range / 5);
            var m = obj_.transform.Find("zd_fz001").GetComponent<Renderer>().material;
            materials_.Add(m);
            obj_sub_ = LuaHelper.GetResManager().CreateEffect("skill_release_quan");
            obj_sub_.transform.parent = LuaHelper.GetResManager().UnitRoot;
            obj_sub_.transform.localScale = new Vector3(param / 5, 1, param / 5);
            m = obj_sub_.transform.Find("zd_fz004").GetComponent<Renderer>().material;
            materials_.Add(m);
        }
        else if(type_ == 4 || type_ == 5 || type_ == 6)
        {
            obj_ = LuaHelper.GetResManager().CreateEffect("skill_release_di");
            obj_.transform.parent = LuaHelper.GetResManager().UnitRoot;
            obj_.transform.localScale = new Vector3(range / 5, 1, range / 5);
            var m = obj_.transform.Find("zd_fz001").GetComponent<Renderer>().material;
            materials_.Add(m);
        }
        cancel_ = false;
        BattleSkillRange.SetBlue();
    }

    public static void Destroy()
    {
        if (obj_ != null)
        {
            GameObject.Destroy(obj_);
            obj_ = null;
        }

        if (obj_sub_ != null)
        {
            GameObject.Destroy(obj_sub_);
            obj_sub_ = null;
        }

        materials_.Clear();
        name_ = "";
    }

    public static void SetPosition(BattleAnimal bp,float r,float xx, float yy)
    {
        x_ = bp.animal.x + xx * range_;
        y_ = bp.animal.y + yy * range_;
        r_ = r;
        float ox, oy, oz;
        BattlePlayers.bobjpool.get_localPosition(bp.posobjid, out ox, out oy, out oz);
        if (obj_ != null)
            obj_.transform.localPosition = new Vector3(ox, 0.1f, oz);
        if (obj_sub_ != null)
        {
            if (type_ == 1)
            {
                obj_sub_.transform.localEulerAngles = new Vector3(0, 90 - r, 0);
                obj_sub_.transform.localPosition = new Vector3(ox, 0.1f, oz);
            }
            else if (type_ == 2)
            {
                obj_sub_.transform.localEulerAngles = new Vector3(0, 90 - r, 0);
                obj_sub_.transform.localPosition = new Vector3(ox, 0.1f, oz);
            }
            else if (type_ == 3)
                obj_sub_.transform.localPosition = new Vector3(ox + xx * range_ / Battle.BL, 0.1f, oz + yy * range_ / Battle.BL);
        }
    }

    public static void SetPosition1(BattleAnimal bp,int r, BattleAnimal tbp)
    {
        if (tbp == null)
            tbp = bp;
        x_ = tbp.animal.x;
        y_ = tbp.animal.y;
        r_ = r;
        float ox, oy, oz;
        BattlePlayers.bobjpool.get_localPosition(bp.posobjid,out ox,out oy,out oz);
        if (obj_ != null)
            obj_.transform.localPosition = new Vector3(ox, 0.1f, oz);

        if (obj_sub_ != null)
        {
            if (type_ == 1)
            {
                obj_sub_.transform.localEulerAngles = new Vector3(0, 90 - r, 0);
                obj_sub_.transform.localPosition = new Vector3(ox, 0.1f, oz);
            }
            else if (type_ == 2)
            {
                obj_sub_.transform.localEulerAngles = new Vector3(0, 90 - r, 0);
                obj_sub_.transform.localPosition = new Vector3(ox, 0.1f, oz);
            }
            else if (type_ == 3)
            {
                BattlePlayers.bobjpool.get_localPosition(tbp.posobjid, out ox, out oy, out oz);
                obj_sub_.transform.localPosition = new Vector3(ox, 0.1f, oz);
            }
        }
    }

    public static float[] GetPosition()
    {
        return new float[] { x_, y_, r_ };
    }

    public static void SetRange(float range,float param)
    {
        range_ = range;
        range = range / Battle.BL;
        if (type_ != 1)
            param = param / Battle.BL;
        if (type_ == 1)
        {
            obj_.transform.localScale = new Vector3(range / 5, 1, range / 5);
            obj_sub_.transform.localScale = new Vector3(range / 5, 1, range / 5);
        }
        else if (type_ == 2)
        {
            obj_.transform.localScale = new Vector3(range / 5, 1, range / 5);
            obj_sub_.transform.localScale = new Vector3(param / 2, 1, 1);
            var head = obj_sub_.transform.Find("head");
            head.localPosition = new Vector3(0, 0, range - 2);
            var tail = obj_sub_.transform.Find("tail");
            tail.localScale = new Vector3(1, 1, (range - 2) / 3);
        }
        else if (type_ == 3)
        {
            obj_.transform.localScale = new Vector3(range / 5, 1, range / 5);
            obj_sub_.transform.localScale = new Vector3(param / 5, 1, param / 5);
        }
        else if (type_ == 4)
            obj_.transform.localScale = new Vector3(range / 5, 1, range / 5);


    }

    public static void SetColor(Color c)
    {
        for (int i = 0; i < materials_.Count; i++)
            materials_[i].SetColor("_TintColor", c);
    }

    public static void SetBlue()
    {
        BattleSkillRange.SetColor(new Color(0, 0.165f, 0.941f, 0.5f));
    }

    public static void SetRed()
    {
        BattleSkillRange.SetColor(new Color(1, 0, 0, 0.5f));
    }

    public static void SetC(bool b)
    {
        if (cancel_ != b)
        {
            cancel_ = b;
            if (cancel_)
                BattleSkillRange.SetRed();
            else
                BattleSkillRange.SetBlue();
        }
    }
}

