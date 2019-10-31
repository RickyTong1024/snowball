using UnityEngine;
using System.Collections;

public class platform_config 
{
    public static void init()
    {
        if (Application.isEditor)
        {
            return;
        }
        platform_config_common.m_common_url = "http://snowball.oss.yymoon.com/test/";
        platform_config_common.m_platform = "test";
        platform_config_common.languageType = platform._instance.get_language();
    }
}
