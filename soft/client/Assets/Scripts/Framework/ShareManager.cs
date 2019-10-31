using UnityEngine;
using System.Collections;
using ZXing;
using ZXing.QrCode;
using System;
using ZXing.Common;
using ZXing.Rendering;
using System.Collections.Generic;
using System.Text;
using LuaInterface;
using System.Runtime.InteropServices;
using System.IO;
public class ShareManager : MonoBehaviour
{
    /*
     * [WXApi registerApp:@"wxd930ea5d5a258f4f" enableMTA:YES];
     - (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url  
{  
    return [WXApi handleOpenURL:url delegate:self];  
}
    - (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url {
    return  [WXApi handleOpenURL:url delegate:[WXApiManager sharedManager]];
}

- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation {
    return [WXApi handleOpenURL:url delegate:[WXApiManager sharedManager]];
}
*/
    private Texture2D share_code;
    private LuaFunction shareFunc;
#if UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern void ShareByIos(int plat, string title, string desc, string url, string imageurl);
    [DllImport("__Internal")]
    private static extern void share_init();
#endif
    // Use this for initialization
    void Start()
    {
        if (Application.isEditor)
        {
            Debug.Log("share in editor,无效");
            return;
        }
#if UNITY_IPHONE
        share_init();
#elif UNITY_ANDROID
        platform._instance.share_init();
#else
        return;
#endif
    }

    // Update is called once per frame

    public void Share(int plat,string title, string text, string url, string imageurl)
    {
        if (Application.isEditor)
        {
            return;
        }
#if UNITY_IPHONE
        ShareByIos(plat, title,text,url, imageurl);
#elif UNITY_ANDROID
        platform._instance.share(plat.ToString(), title, text, url, imageurl);
#else
        OnShareResultHandler("0");
#endif
    }

    public void ShareByCapture(string url, string title, string text)
    {
        
    }

    public Texture2D  ShareZcode(string url, int z_width, int z_height)
    {
        //设置二维码扫描结果

        Texture2D zcode = new Texture2D(z_width, z_height);
        zcode.filterMode = FilterMode.Point;
        zcode.mipMapBias = 0;
#if UNITY_EDITOR
        zcode.alphaIsTransparency = false;
#endif
        //二维码边框  
        BitMatrix BIT;

        Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();

        //设置编码方式
        hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
        int bk = 40;
        BIT = new MultiFormatWriter().encode(url, BarcodeFormat.QR_CODE, z_width + bk * 2, z_height + bk * 2, hints);
        int width = BIT.Width;
        int height = BIT.Width;

        for (int x = bk; x < height - bk; x++)
        {
            for (int y = bk; y < width - bk; y++)
            {
                if (BIT[x, y])
                {
                    zcode.SetPixel(y - bk, x - bk, Color.black);
                }
                else
                {
                    zcode.SetPixel(y - bk, x - bk, Color.white);
                }

            }
        }
        zcode.Apply();
        share_code = zcode;
        return zcode;
    }

    public void fini()
    {
        GameObject.Destroy(share_code);
    }

    string jz52 = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public LuaFunction ShareFunc
    {
        get
        {
            return shareFunc;
        }

        set
        {
            shareFunc = value;
        }
    }

    public string GetShareID(string guid)
    {
        ulong id = (ulong.Parse(guid) & 0x00FFFFFFFFFFFFFF) | (((ulong)9 << 56) & 0xFF00000000000000);
        string s = "";
        while (id > 0)
        {
            int a = (int)(id % 52);
            id = id / 52;
            s = jz52[a] + s;
        }
        return s;
    }
    public string GetPackName()
    {

        return "";
    }

    void OnShareResultHandler(string code)
    {
        Debug.Log(code);
        if (code == "0")
        {
            Debug.Log("share successfully");
            if (ShareFunc != null)
            {
                ShareFunc.Call();
            }
        }
        else if (code == "-2")
        {
            Debug.Log("cancel !");
        }
        else
        {
            Debug.Log("error code !" + code);
        }
    }

    private void OnDestroy()
    {
        fini();
    }
}