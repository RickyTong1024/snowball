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
}
