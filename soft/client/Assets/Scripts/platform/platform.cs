using UnityEngine;
using System.Collections;

public class platform : platform_common
{
    private static platform _platform;

    public static platform _instance
    {
        get
        {
            if (_platform == null)
            {
                _platform = new platform();
            }
            return _platform;
        }
    }

    public override void share_init()
    {
        if (Application.isEditor)
        {
            return;
        }
        string str = "Tool" + "_" + "OnShareResultHandler" + "_" + "100";
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("share_prepared", str);
    }
}
