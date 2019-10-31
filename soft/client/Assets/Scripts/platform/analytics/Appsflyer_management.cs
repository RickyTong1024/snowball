using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appsflyer_management : MonoBehaviour, Ianalytics
{

    public void onPurchase(string orderId, double currencyAmount, double virtualCurrencyAmount)
    {
#if APPSFLYER
        Debug.Log("appsflyer 准备购买监测");
        Dictionary<string, string> eventValue = new Dictionary<string, string>();
        eventValue.Add("af_revenue", currencyAmount.ToString());
        eventValue.Add("orderId", orderId);
        eventValue.Add("jewel", virtualCurrencyAmount.ToString());
        eventValue.Add("af_currency", "CNY");
        AppsFlyer.trackRichEvent(AFInAppEvents.PURCHASE, eventValue);
        Debug.Log("appsflyer 已执行购买监测");
#endif
    }

    public void onGameLogin(string login_param,int is_new)
    {
#if APPSFLYER
        Debug.Log("appsflyer 准备登陆监测");
        Dictionary<string, string> purchaseEvent = new Dictionary<string, string>();
        string[] paramArray = login_param.Split('|');
        if (paramArray.Length < 4)
        {
            return;
        }
        string guid = paramArray[0];
        string serverid = paramArray[1];
        string playername = paramArray[2];
        string level = paramArray[3];
        purchaseEvent.Add("isnew 1or0 1new", is_new.ToString());
        purchaseEvent.Add("server_id", serverid);
        purchaseEvent.Add("guid", guid);
        purchaseEvent.Add("player_name", playername);
        purchaseEvent.Add("player_level", level);
        AppsFlyer.trackRichEvent("ongamelogin", purchaseEvent);
        Debug.Log("appsflyer 已执行登陆监测");
# endif
    }

    public void onGameUserUpgrade(int level)
    {
#if APPSFLYER
        Debug.Log("appsflyer 准备升級监测");
        Dictionary<string, string> purchaseEvent = new Dictionary<string, string>();
        purchaseEvent.Add("level", level.ToString());
        purchaseEvent.Add("guid", self.guid);
        purchaseEvent.Add("server_id", self.player.serverid.ToString());
        purchaseEvent.Add("player_level", self.player.level.ToString());
        AppsFlyer.trackRichEvent("ongameuserupgrade", purchaseEvent);
        Debug.Log("appsflyer 已执行升級监测");
#endif

    }
    
    void Start()
    {
#if APPSFLYER && UNITY_IPHONE
        string APPSFLYER_DEV_KEY = platform_config_common.APPSFLYER_DEV_KEY;
        AppsFlyer.setAppsFlyerKey(APPSFLYER_DEV_KEY);
        string APP_ID = platform_config_common.YOUR_APP_ID_HERE;
        AppsFlyer.setAppID (APP_ID);
        AppsFlyer.trackAppLaunch ();
        analytics_management.analytics_config();  //注册事件
#elif APPSFLYER && UNITY_ANDROID
        string APPSFLYER_DEV_KEY = platform_config_common.APPSFLYER_DEV_KEY;
        AppsFlyer.setAppsFlyerKey(APPSFLYER_DEV_KEY);
        AppsFlyer.setAppID(platform._instance.get_bundle_name());
        AppsFlyer.init(APPSFLYER_DEV_KEY, "AppsFlyerTrackerCallbacks");
        analytics_management.analytics_config();  //注册事件
# endif
    }
}

