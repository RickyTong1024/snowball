package com.yymoon.snowball;

import com.alipay.sdk.app.PayTask;
import com.unity3d.player.UnityPlayer;
import com.yymoon.game.GameActivity;
import com.yymoon.snowball.wxapi.WXEntryActivity;

import android.Manifest;
import android.annotation.SuppressLint;
import android.content.Context;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.text.TextUtils;
import android.util.Log;
import android.widget.Toast;
import android.support.v4.content.PermissionChecker;

import java.util.Map;

public class MainActivity extends GameActivity {
    public static final String APPID = "2016090501851430";
    private String m_url = "http://121.43.107.164:8080/sign";
    static String orderParam = "";
    private static final int SDK_PAY_FLAG = 1;
    private static final int SDK_AUTH_FLAG = 2;
    private WXEntryActivity shareActivity;
    static final int REQUEST_READ_PHONE_STATE=1;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        premission_prepared();
        shareActivity=new WXEntryActivity();
        shareActivity.InitSdk(mContext);
    }

    @Override
    public void game_pay(String param){
        String[] s = param.split("\\|");
        String body = s[0];
        String subject = s[1];
        String price = s[2];
        do_aliinfo(body,subject,price);
    }

    public void do_aliinfo(String body, String subject, String price) {
        Map<String, String> params = OrderInfoUtil2_0.buildOrderParamMap(APPID, body, subject, price);

        orderParam = OrderInfoUtil2_0.buildOrderParam(params, true);

        String scrSign = OrderInfoUtil2_0.buildOrderParam(params, false);
        Log.i("unity", scrSign);
        UnityPlayer.UnitySendMessage("Tool", "recharge_alipay_info", scrSign);
        do_alisign(scrSign,m_url);
    }

    public void do_alisign(String param, String recharge_url)
    {
        String sign = Util.httpPost(recharge_url, param);
        if (sign==null||sign.equals(""))
        {
            this.toastMes("请求失败");
            this.pay_cancel("");
            return;
        }
        Log.d("unity", "sign:"+sign);
        do_alipay(sign);
    }

    @SuppressLint({"HandlerLeak"})
    private Handler mHandler = new Handler() {
        public void handleMessage(Message msg) {
            switch (msg.what) {
                case SDK_PAY_FLAG:
                    PayResult payResult = new PayResult((String) msg.obj);
                    String resultStatus = payResult.getResultStatus();
                    if (TextUtils.equals(resultStatus, "9000")) {
                        Toast.makeText(UnityPlayer.currentActivity, "支付成功", Toast.LENGTH_SHORT).show();
                        pay_done("");
                    } else {
                        Toast.makeText(UnityPlayer.currentActivity, "支付失败", Toast.LENGTH_SHORT).show();
                        pay_cancel("");
                    }
                    break;
                case SDK_AUTH_FLAG:
                    AuthResult authResult = new AuthResult((Map) msg.obj, true);
                    resultStatus = authResult.getResultStatus();
                    if ((TextUtils.equals(resultStatus, "9000")) && (TextUtils.equals(authResult.getResultCode(), "200"))) {
                        Toast.makeText(UnityPlayer.currentActivity, "授权成功\n" + String.format("authCode:%s", new Object[]{authResult.getAuthCode()}), Toast.LENGTH_SHORT).show();
                    } else {
                        Toast.makeText(UnityPlayer.currentActivity, "授权失败" + String.format("authCode:%s", new Object[]{authResult.getAuthCode()}), Toast.LENGTH_SHORT).show();
                    }
                    break;
            }
        }
    };

    public void do_alipay(final String sign) {
        final String orderInfo = orderParam + "&sign=" + sign;
        Runnable payRunnable = new Runnable() {
            @Override
            public void run() {
                PayTask alipay = new PayTask(UnityPlayer.currentActivity);
                Log.i(TAG, orderInfo);
                String result = alipay.pay(orderInfo, true);
                Log.i("msp", result.toString());
                Message msg = new Message();
                msg.what = 1;
                msg.obj = result;
                MainActivity.this.mHandler.sendMessage(msg);
            }
        };

        Thread payThread = new Thread(payRunnable);
        payThread.start();
    }

    public void pay_done(String message)
    {
        UnityPlayer.UnitySendMessage("Tool","recharge_done", message);
    }

    public void pay_cancel(String message)
    {
        UnityPlayer.UnitySendMessage("Tool","recharge_cancel", message);
    }

    public void toastMes(String mes)
    {
        showToast(mes);
    }

    @Override
    public String get_platform_id()
    {
        return "android_yymoon";
    }

    @Override
    public void share_prepared(String param) {
        String[] ss = param.split("_");
        String s1 = ss[0];
        String s2 = ss[1];
        String s3 = ss[2];
        shareActivity.SetCallBack(s1,s2, Integer.parseInt(s3));
    }

    @Override
    public void share(String param) {
        String[] ss = param.split("_");
        String s1 = ss[0];
        String s2 = ss[1];
        String s3 = ss[2];
        String s4 = ss[3];
        String s5 = ss[4];
        shareActivity.SendMsgWeb(Integer.parseInt(s1),s2,s3,s4,s5);
    }

    private void premission_prepared(){ //一些手机对保存图片的一些权限要手动开启
        requestPremission(this, Manifest.permission.WRITE_EXTERNAL_STORAGE);
    }

    private  void requestPremission(Context context, String permission){
        boolean result = true;
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            if (getTargetSdkVersion(context) >= Build.VERSION_CODES.M) {
                result = context.checkSelfPermission(permission) == PackageManager.PERMISSION_GRANTED;
            } else {
                result = PermissionChecker.checkSelfPermission(context, permission) == PermissionChecker.PERMISSION_GRANTED;
            }
            if (!result){
                this.requestPermissions(new String[]{permission}, REQUEST_READ_PHONE_STATE);
            }
        }
    }

    private int getTargetSdkVersion(Context context) {
        int version = 0;
        try {
            final PackageInfo info = context.getPackageManager().getPackageInfo(
                    context.getPackageName(), 0);
            version = info.applicationInfo.targetSdkVersion;
        } catch (PackageManager.NameNotFoundException e) {
            e.printStackTrace();
        }
        return version;
    }
}