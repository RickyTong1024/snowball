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
