using UnityEngine;
using LuaInterface;
using UnityEngine.Purchasing;
public class platform_recharge : platform_recharge_common,IStoreListener
{
    private IStoreController controller;
    private IExtensionProvider extensions;
    string m_code;
    private static platform_recharge _platform_recharge;
    public static platform_recharge _instance
    {
        get
        {
            if (_platform_recharge == null)
            {
                _platform_recharge = new platform_recharge();
            }
            return _platform_recharge;
        }
    }

    public override void init()
    {
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        var db_recharge = new dbc();
        db_recharge.load_txt("t_recharge");
        for (int i = 0; i < db_recharge.get_y(); i++)
        {
            string platform_id = db_recharge.get_string(8, i); 
            builder.AddProduct(platform_id, ProductType.Consumable, new IDs { { platform_id, AppleAppStore.Name } });
        }
        UnityPurchasing.Initialize(this, builder);
    }

    public override void do_buy(string recharge_param, int huodongMid = 0, int huodongEid = 0)
    {
        if (Application.isEditor)
        {
            if (Canelfunc != null)
            {
                Canelfunc.Call();
            }

            Debug.Log("buy in editor,it is invalid");
            return;
        }
        string[] paramArray = recharge_param.Split('|');
        if(paramArray.Length < 9)
        {
            return;
        }
        string ios_id = paramArray[7];

        controller.InitiatePurchase(ios_id);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized");
        this.controller = controller;
        this.extensions = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed" + error.ToString());
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Debug.Log(i.ToString());
        Debug.Log("PurchaseFailureReason :" + p.ToString());
        if (Canelfunc != null)
        {
            Canelfunc.Call();
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Debug.Log("ProcessPurchase");
        if (m_code == extensions.GetExtension<IAppleExtensions>().GetTransactionReceiptForProduct(e.purchasedProduct))
        {
            return PurchaseProcessingResult.Complete;
        }
        m_code = string.Format("{0}|{1}",e.purchasedProduct.definition.id, extensions.GetExtension<IAppleExtensions>().GetTransactionReceiptForProduct(e.purchasedProduct));

        if (Donefunc != null)
        {
            Donefunc.Call("apple", m_code);
        }
        return PurchaseProcessingResult.Complete;
    }

    LuaFunction donefunc;
    LuaFunction canelfunc;

    public LuaFunction Donefunc
    {
        get
        {
            return donefunc;
        }

        set
        {
            donefunc = value;
        }
    }

    public LuaFunction Canelfunc
    {
        get
        {
            return canelfunc;
        }

        set
        {
            canelfunc = value;
        }
    }
}
