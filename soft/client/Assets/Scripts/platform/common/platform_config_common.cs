using UnityEngine;
using System.Collections;

public class platform_config_common 
{
    // 版本号
    public const string version = "1.0";

    /// <summary>
    /// 此段信息可在platform_config的init中修改
    /// </summary>
    public static string m_common_url = "";
    public static string m_platform = "";
    public static bool m_debug = false;
    public static int languageType = 0;
    public static string APPSFLYER_DEV_KEY = "ryDMeYCsxuVUT654DVrnnD";  //appsflyer dev_key
    public static string YOUR_APP_ID_HERE = "";     //appsflyer ios id

    // 是否使用第三方的登陆
    public static int m_login = 0;

    /// <summary>
    /// 不可修改
    /// </summary>
    public const int GameFrameRate = 30;                    //游戏帧频
    public const string AppName = "SnowBall";               //应用程序名称

    public static void init()
    {
        if (Application.isEditor)
        {
            m_debug = true;
            m_platform = "test";
            m_common_url = "http://snowball.oss.yymoon.com/test/";
        }
    }

    public static string FrameworkRoot
    {
        get
        {
            return Application.dataPath;
        } 
    }
}
