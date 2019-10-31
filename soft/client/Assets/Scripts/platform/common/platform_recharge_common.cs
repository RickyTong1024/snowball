using UnityEngine;

public class platform_recharge_common
{
    public virtual void init()
    {
    }

    public virtual void do_buy(string recharge_param, int huodongMid = 0, int huodongEid = 0)
    {
        if (Application.isEditor)
        {
            Debug.Log("buy in editor,it is invalid");
            return;
        }
    }

    public virtual void recharge_done(string s)
    {

    }

    public virtual void recharge_cancel(string s)
    {

    }

    public virtual void recharge_product(string s)
    {

    }

    public virtual void recharge_onOderNo(string s)  //生成订单
    {

    }

}
