using UnityEngine;
using System.Runtime.InteropServices;

public class platform : platform_common
{

    [DllImport("__Internal")]
    private static extern void tool_init();
    [DllImport("__Internal")]
    private static extern void createNotify(string text, int secondsFromNow);
    [DllImport("__Internal")]
    private static extern void cancelNotify();
    [DllImport("__Internal")]
    private static extern void copyTextToClipboard(string text);
    [DllImport("__Internal")]
    private static extern int getlanguage();
    [DllImport("__Internal")]
    private static extern void returnPrice(string sku);
    [DllImport("__Internal")]
    private static extern void game_open_store();
    [DllImport("__Internal")]
    private static extern string platform_id();
    [DllImport("__Internal")]
    private static extern string platform_shard();
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

    public override void init(GameObject gameObject)
    {
        m_photo_time = 1.2f;
        tool_init();
        base.init(gameObject);
    }

    public override void cancel_notify()
    {
        if (Application.isEditor)
        {
            return;
        }
        cancelNotify();
    }

    public override void create_notify(string title, string text, string ticker, int secondsFromNow)
    {
        if (Application.isEditor)
        {
            return;
        }
        createNotify(text, secondsFromNow);
    }

    public override void copy(string text)
    {
        if (Application.isEditor)
        {
            return;
        }
        copyTextToClipboard(text);
    }

    public override void on_game_login(string login_params, int is_new)
    {
        if (Application.isEditor)
        {
            return;
        }
        if (analytics_management.m_onGameLogin != null)
        {
            analytics_management.m_onGameLogin(login_params, is_new);
        }
    }

    public override void on_game_user_upgrade(int level)
    {
        if (Application.isEditor)
        {
            return;
        }
        if (analytics_management.m_onGameUserUpgrade != null)
        {
            analytics_management.m_onGameUserUpgrade(level);
        }
    }

    public override int get_language()
    {
        if (Application.isEditor)
        {
            return 0;
        }
        return getlanguage();
    }

    public override string get_platform_id()
    {
        if (Application.isEditor)
        {
            return "";
        }
        return platform_id();
    }

    public override void share(string plat, string title, string text, string url, string imageurl)
    {
        if (Application.isEditor)
        {
            return;
        }
        string param = plat + "_" + title + "_" + text + "_" + url + "_" + imageurl;
        platform_shard();
    }

    public override void on_purchase(string orderId, double currencyAmount, double virtualCurrencyAmount)
    {
        if (Application.isEditor)
        {
            return;
        }
        if (analytics_management.m_onPurchase != null)
        {
            analytics_management.m_onPurchase(orderId, currencyAmount, virtualCurrencyAmount);
        }
    }
}
