using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class analytics_management  {
    public delegate void DelegateonPurchase(string orderId, double currencyAmount, double virtualCurrencyAmount);
    public static DelegateonPurchase m_onPurchase;

    public delegate void DelegateonGameLogin(string login_param,int is_new);
    public static DelegateonGameLogin m_onGameLogin;

    public delegate void DelegateonGameUserUpgrade(int level);
    public static DelegateonGameUserUpgrade m_onGameUserUpgrade;
    
    public static void analytics_config()
    {
#if APPSFLYER
        if (AppFacade._instance.Appsflyer_Mgr == null)
        {
            Debug.LogError("AppFacade._instance.Appsflyer_Mgr is null");
            return;
        }
        Appsflyer_management appsflyer_Management = AppFacade._instance.Appsflyer_Mgr;
        m_onPurchase +=appsflyer_Management.onPurchase;
        m_onGameLogin +=appsflyer_Management.onGameLogin;
        m_onGameUserUpgrade +=appsflyer_Management.onGameUserUpgrade;
#endif
    }
}
