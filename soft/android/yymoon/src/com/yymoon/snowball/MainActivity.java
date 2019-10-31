package com.yymoon.snowball;

import com.alipay.sdk.app.PayTask;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;
import com.yymoon.snowball.wxapi.WXEntryActivity;
import com.tencent.gcloud.voice.GCloudVoiceEngine;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.os.Handler;
import android.os.Message;
import android.text.TextUtils;
import android.util.Log;
import android.view.OrientationEventListener;
import android.view.Surface;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Toast;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.Map;

public class MainActivity extends UnityPlayerActivity {
    public static Context mContext;
    public boolean m_flag = false;
    private boolean m_zx = false;
    private OrientationEventListener mOrientationListener;

    public static final String APPID = "2016090501851430";
    static String orderParam = "";

    private static final int SDK_PAY_FLAG = 1;
    private static final int SDK_AUTH_FLAG = 2;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        mContext = this;
        int rotation = this.getWindowManager().getDefaultDisplay()
                .getRotation();
        switch (rotation) {
            case Surface.ROTATION_0:
                m_zx = false;
                this.setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE);
                break;
            case Surface.ROTATION_90:
                m_zx = false;
                this.setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE);
                break;
            case Surface.ROTATION_180:
                m_zx = true;
                this.setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_REVERSE_LANDSCAPE);
                break;
            case Surface.ROTATION_270:
                m_zx = true;
                this.setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_REVERSE_LANDSCAPE);
            default:
                break;
        }
        startOrientationChangeListener();

        Window window = getWindow();
        window.setFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON,
                WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
        HideSystemUI();
        AlarmReceiver.m_act = this;
        GCloudVoiceEngine.getInstance().init(getApplicationContext(), this);
        if (mContext == null) {
            mContext = UnityPlayer.currentActivity.getApplicationContext();
        }
        WXEntryActivity.InitSdk(mContext);
    }

    @SuppressLint("InlinedApi")
    private void HideSystemUI() {
        if (Build.VERSION.SDK_INT >= 16) {
            final View decorView = this.getWindow().getDecorView();

            int uiOptions = 0;

            if (Build.VERSION.SDK_INT >= 19) {
                uiOptions = View.SYSTEM_UI_FLAG_FULLSCREEN
                        | View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                        | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                        | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                        | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION // hide nav bar
                        | View.SYSTEM_UI_FLAG_IMMERSIVE;
            } else {
                uiOptions = View.SYSTEM_UI_FLAG_FULLSCREEN;
            }
            decorView.setSystemUiVisibility(uiOptions);

            int updatedUIOptions = 0;
            // Fix input method showing causes ui show issue.
            if (Build.VERSION.SDK_INT >= 19) {
                updatedUIOptions = View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                        | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                        | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                        | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
                        | View.SYSTEM_UI_FLAG_FULLSCREEN
                        | View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY;
            } else {
                updatedUIOptions = View.SYSTEM_UI_FLAG_FULLSCREEN;
            }

            final int finalUiOptions = updatedUIOptions;
            decorView
                    .setOnSystemUiVisibilityChangeListener(new View.OnSystemUiVisibilityChangeListener() {
                        @Override
                        public void onSystemUiVisibilityChange(int i) {
                            decorView.setSystemUiVisibility(finalUiOptions);
                        }
                    });
        }
    }

    private final void startOrientationChangeListener() {
        mOrientationListener = new OrientationEventListener(this) {
            @Override
            public void onOrientationChanged(int rotation) {
                if (rotation > 45 && rotation < 135) {
                    if (!m_zx) {
                        m_zx = true;
                        MainActivity.this
                                .setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_REVERSE_LANDSCAPE);
                    }
                } else if (rotation > 225 && rotation < 315) {
                    if (m_zx) {
                        m_zx = false;
                        MainActivity.this
                                .setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE);
                    }
                }
            }
        };
        mOrientationListener.enable();
    }

    private void showToast(final String message) {
        runOnUiThread(new Runnable() {

            @Override
            public void run() {
                // TODO Auto-generated method stub
                Toast.makeText(MainActivity.this, message, Toast.LENGTH_SHORT)
                        .show();
            }
        });
    }

    public void do_aliinfo(String body, String subject, String price) {
        Map<String, String> params = OrderInfoUtil2_0.buildOrderParamMap(APPID, body, subject, price);

        orderParam = OrderInfoUtil2_0.buildOrderParam(params, true);

        String scrSign = OrderInfoUtil2_0.buildOrderParam(params, false);
        Log.i("unity", scrSign);
        UnityPlayer.UnitySendMessage("Tool", "recharge_alipay_info", scrSign);
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
                        UnityPlayer.UnitySendMessage("Tool", "recharge_alipay_done", "");
                    } else {
                        Toast.makeText(UnityPlayer.currentActivity, "支付失败", Toast.LENGTH_SHORT).show();
                        UnityPlayer.UnitySendMessage("Tool", "recharge_alipay_cancel", "");
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
                Log.i("unity", orderInfo);
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

    public void install_apk(String path) {
        File apkfile = new File(path);
        if (!apkfile.exists()) {
            return;
        }
        Intent i = new Intent(Intent.ACTION_VIEW);
        i.setDataAndType(Uri.parse("file://" + apkfile.toString()), "application/vnd.android.package-archive");
        mContext.startActivity(i);
        finish();
        System.exit(0);
    }

    public void save_photo(final String filename,final byte[] data) {
        MainActivity.this.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Bitmap bitmap = BitmapFactory.decodeByteArray(data, 0, data.length);
                File file = new File(Environment.getExternalStorageDirectory()
                        + "/DCIM/Camera", filename.replace(".png", ".jpg"));
                FileOutputStream fos = null;
                try {
                    fos = new FileOutputStream(file);
                } catch (FileNotFoundException e) {
                    // TODO Auto-generated catch block
                    Log.w("cat", e.toString());
                }
                bitmap.compress(Bitmap.CompressFormat.JPEG, 100, fos);
                try {
                    fos.flush();
                } catch (IOException e) {
                    // TODO Auto-generated catch block
                    Log.w("cat", e.toString());
                }
                try {
                    fos.close();
                } catch (IOException e) {
                    // TODO Auto-generated catch block
                    Log.w("cat", e.toString());
                }
                bitmap.recycle();
                mContext.sendBroadcast(new Intent(Intent.ACTION_MEDIA_SCANNER_SCAN_FILE, Uri.parse("file://" + Environment.getExternalStorageDirectory()
                        + "/DCIM/Camera/" + filename.replace(".png", ".jpg"))));

                Toast.makeText(mContext, "账号截图已保存到相册", Toast.LENGTH_SHORT).show();
            }
        });
    }

}