using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;

public class platform_recharge : platform_recharge_common
{
    private static platform_recharge _platform_recharge;
    public static platform_recharge _instance
    {
        get
        {
            if (_platform_recharge == null)
            {
                _platform_recharge = new platform_recharge();
            }
            return _platform_recharge;
        }
    }

    public override void do_buy(string recharge_param, int huodongMid = 0, int huodongEid = 0)
    {
        if (Application.isEditor)
        {
            Debug.Log("buy in editor,it is invalid");
            return;
        }
        string[] paramArray = recharge_param.Split('|');
        if (paramArray.Length<10)
        {
            return;
        }
        string guid = paramArray[0];
        string serverid = paramArray[1];
        string id = paramArray[2];
        string name = paramArray[3];
        string type = paramArray[4];
        string check = paramArray[5];
        string price = paramArray[6];
        string ios_id = paramArray[7];
        string desc = paramArray[8];
        string body = "snowball_" + guid + "_" + serverid + "_" + id;
        string subject = name;
        string re_price = price;
        string param = body + "|" + subject + "|" + re_price;
        do_pay(param);
    }

    private void do_pay(string param)
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("game_pay", param);
    }

    public override void recharge_done(string s)
    {
        if (Donefunc != null)
        {
            Donefunc.Call("android_yymoon", s);
        }
    }

    public override void recharge_cancel(string s)
    {
        if (Canelfunc != null)
        {
            Canelfunc.Call();
        }
    }

    LuaFunction donefunc;
    LuaFunction canelfunc;

    public LuaFunction Donefunc
    {
        get
        {
            return donefunc;
        }

        set
        {
            donefunc = value;
        }
    }

    public LuaFunction Canelfunc
    {
        get
        {
            return canelfunc;
        }

        set
        {
            canelfunc = value;
        }
    }
}
