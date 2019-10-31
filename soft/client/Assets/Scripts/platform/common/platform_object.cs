using UnityEngine;
using System.Collections;

public class platform_object : MonoBehaviour
{
    void Start()
    {
        platform._instance.init(this.gameObject);
    }

    void platform_login_success(string s)
    {
        platform._instance.platform_login_success(s);
    }

    void platform_login_fail(string s)
    {
       
    }

    void platform_logout(string s)
    {
        platform._instance.platfrom_logout_success(s);
    }
}
