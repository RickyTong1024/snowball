using UnityEngine;

public abstract class platform_common
{
    public float m_photo_time = 0.1f;
    public virtual void init(GameObject gameObject)
    {
        cancel_notify();
    }

    public virtual void create_notify(string title, string text, string ticker, int secondsFromNow)
    {
        if (Application.isEditor)
        {
            return;
        }
        AndroidJavaClass javaClass = new AndroidJavaClass("com.yymoon.tool.AlarmReceiver");
        javaClass.CallStatic("startAlarm", new object[4] { title, text, ticker, secondsFromNow });
    }

    public virtual void cancel_notify()
    {
        if (Application.isEditor)
        {
            return;
        }
        AndroidJavaClass javaClass = new AndroidJavaClass("com.yymoon.tool.AlarmReceiver");
        javaClass.CallStatic("clearNotification");
    }

    public virtual void copy(string text)      
    {
        if (Application.isEditor)
        {
            return;
        }
        AndroidJavaClass javaClass = new AndroidJavaClass("com.yymoon.tool.ClipboardTools");
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        javaClass.CallStatic("copyTextToClipboard", jo, text);
    }

    public virtual void do_save_photo_platform(string file_name, byte[] data)
    {
        if (Application.isEditor)
        {
            return;
        }
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("save_photo", file_name, data);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////

    public virtual void game_login()    //使用第三方登陆游戏
    {
        if (Application.isEditor)
        {
            return;
        }
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("game_login");
    }

    public virtual void platform_login_success(string s)      //第三方平台登入成功
    {

    }

    public virtual void game_logout()   //第三方平台中游戏登出
    {
        if (Application.isEditor)
        {
            return;
        }
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("game_logout");
    }

    public virtual void platfrom_logout_success(string s)   //第三放平台登出成功
    {
        
    }

    public virtual void on_game_login(string login_params,int is_new)  //游戏登入成功
    {
        if (Application.isEditor)
        {
            return;
        }
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("on_game_login", login_params, is_new);
        //appsflyer 监测
        analytics_management.m_onGameLogin(login_params,is_new);
    }

    public virtual void on_game_user_upgrade(int level)
    {
        if (Application.isEditor)
        {
            Debug.Log("on_game_user_upgrade,level: "+level);
            return;
        }
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("on_game_user_upgrade", level);
        //appsflyer 监测
        analytics_management.m_onGameUserUpgrade(level);
    }

    public virtual void game_update()
    {
      
    }

    public virtual void game_install(string path)
    {
        if (Application.isEditor)
        {
            return;
        }
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("install_apk", path);
    }

    public virtual int get_language()
    {
        if (Application.isEditor)
        {
            return 0;
        }
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        return jo.Call<int>("get_language");
    }

    public virtual string get_bundle_name()
    {
        if (Application.isEditor)
        {
            return Application.installerName;
        }
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        return jo.Call<string>("get_bundle_name");
    }

    public virtual string get_platform_id()
    {
        if (Application.isEditor)
        {
            return "";
        }
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        return jo.Call<string>("get_platform_id");
    }

    public virtual string get_platform_isbn()
    {
        return "";
    }

    public virtual void share_init()  
    {
        if (Application.isEditor)
        {    
            return;
        }
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("share_prepared", "null");
     }

    public virtual void share(string plat, string title, string text, string url, string imageurl)
    {
        if (Application.isEditor)
        {
            Debug.Log("share");
            return;
        }
        string param = plat + "_" + title + "_" + text + "_" + url + "_" + imageurl;
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("share", param);
    }

    //appsflyer 监测
    public virtual void on_purchase(string orderId, double currencyAmount, double virtualCurrencyAmount)
    {
        if (Application.isEditor)
        {
            return;
        }
        analytics_management.m_onPurchase(orderId, currencyAmount, virtualCurrencyAmount);
    }
}
