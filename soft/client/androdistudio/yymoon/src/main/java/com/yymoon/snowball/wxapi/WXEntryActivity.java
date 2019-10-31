package com.yymoon.snowball.wxapi;

import java.io.File;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Bitmap.CompressFormat;
import android.os.Bundle;
import android.util.Log;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.net.MalformedURLException;
import java.net.URL;

import com.tencent.mm.sdk.openapi.ConstantsAPI;
import com.tencent.mm.sdk.openapi.IWXAPI;
import com.tencent.mm.sdk.openapi.IWXAPIEventHandler;
import com.tencent.mm.sdk.openapi.SendMessageToWX;
import com.tencent.mm.sdk.openapi.BaseResp;
import com.tencent.mm.sdk.openapi.BaseReq;
import com.tencent.mm.sdk.openapi.WXAPIFactory;
import com.tencent.mm.sdk.openapi.WXMediaMessage;
import com.tencent.mm.sdk.openapi.WXTextObject;
import com.tencent.mm.sdk.openapi.WXWebpageObject;
import com.tencent.mm.sdk.openapi.WXImageObject;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;
import com.yymoon.snowball.R;

public class WXEntryActivity extends Activity implements IWXAPIEventHandler {
    public static IWXAPI api;
    public static String HandleGameObject;
    public static String HandleFuc;
    private static String AppID = "wx56db9d34f7d6c2ee";
    public  static int THUMB_SIZE = 150;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        api.handleIntent(getIntent(), this);
        Log.i("unity","sdk init");
    }

    public  void SetCallBack(String handleGame, String func, int thumb_size) {
        HandleGameObject = handleGame;
        HandleFuc = func;
        THUMB_SIZE = thumb_size;
    }

    public void InitSdk(Context context) {
        api = WXAPIFactory.createWXAPI(context, AppID, true);
        api.registerApp(AppID);
        System.out.print("InitSdk WeChat");
    }

    public void SendMsgWeb(int plat, String title, String text, String url, String imageUrl)
    {
        try {
            WXWebpageObject webpage = new WXWebpageObject();
            webpage.webpageUrl = url;
            WXMediaMessage msg = new WXMediaMessage(webpage);
            msg.title = title;
            msg.description = text;
            Bitmap bmp = BitmapFactory.decodeStream(new URL(imageUrl).openStream());
            Bitmap thumbBmp = Bitmap.createScaledBitmap(bmp, THUMB_SIZE, THUMB_SIZE, true);
            bmp.recycle();
            msg.thumbData = bmpToByteArray(thumbBmp, true);
            SendMessageToWX.Req req = new SendMessageToWX.Req();
            req.transaction = String.valueOf(System.currentTimeMillis());
            req.message = msg;
            switch (plat) {
                case 1:
                    req.scene = SendMessageToWX.Req.WXSceneSession;
                    break;
                case 2:
                    req.scene = SendMessageToWX.Req.WXSceneTimeline;
                    break;
                default:
                    req.scene = SendMessageToWX.Req.WXSceneSession;
                    break;
            }
            if (api == null) {
                System.out.print("api 为空");
            } else {
                api.sendReq(req);
            }
            finish();
        }
        catch(Exception e) {
            e.printStackTrace();
        }
    }

    public void SendMsgImage(int plat, String title, String text, String url, String path) {
        File file = new File(path);
        if (!file.exists()) {
            return;
        }
        WXWebpageObject webge = new WXWebpageObject();
        webge.webpageUrl = url;
        WXMediaMessage msg = new WXMediaMessage(webge);
        msg.title = title;
        msg.description = text;
        Bitmap bmp = BitmapFactory.decodeFile(path);
        Bitmap thumbBmp = Bitmap.createScaledBitmap(bmp, THUMB_SIZE, THUMB_SIZE, true);
        bmp.recycle();
        msg.thumbData = bmpToByteArray(thumbBmp, true);
        SendMessageToWX.Req req = new SendMessageToWX.Req();
        req.transaction = String.valueOf(System.currentTimeMillis());
        req.message = msg;
        switch (plat) {
            case 1:
                req.scene = SendMessageToWX.Req.WXSceneSession;
                break;
            case 2:
                req.scene = SendMessageToWX.Req.WXSceneTimeline;
                break;
            default:
                req.scene = SendMessageToWX.Req.WXSceneSession;
                break;
        }
        if(api == null) {
            System.out.print("api 为空");
        }
        else {
            api.sendReq(req);
        }
    }

    public void onReq(BaseReq req) {

    }

    public void onResp(BaseResp resp) {             //系统中的微信作出相应后会调用这个函数
        Intent iLaunchMyself = getPackageManager().getLaunchIntentForPackage(getPackageName());
        startActivity(iLaunchMyself);
        UnityPlayer.UnitySendMessage(HandleGameObject, HandleFuc, "" + resp.errCode);
    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        setIntent(intent);
        api.handleIntent(intent, this);
    }
    public static byte[] bmpToByteArray(final Bitmap bmp, final boolean needRecycle) {
        ByteArrayOutputStream output = new ByteArrayOutputStream();
        bmp.compress(CompressFormat.PNG, 100, output);
        if (needRecycle) {
            bmp.recycle();
        }

        byte[] result = output.toByteArray();
        try {
            output.close();
        } catch (Exception e) {
            e.printStackTrace();
        }

        return result;
    }
}