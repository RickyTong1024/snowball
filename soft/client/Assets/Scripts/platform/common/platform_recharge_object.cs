using UnityEngine;

public class platform_recharge_object : MonoBehaviour
{
    void recharge_done(string s)
    {
        platform_recharge._instance.recharge_done(s);
    }

    void recharge_cancel(string s)
    {
        platform_recharge._instance.recharge_cancel(s);
    }

    void recharge_onOderNo(string s)
    {
        platform_recharge._instance.recharge_onOderNo(s);
    }

    void recharge_product(string s)
    {
        platform_recharge._instance.recharge_product(s);
    }
}
