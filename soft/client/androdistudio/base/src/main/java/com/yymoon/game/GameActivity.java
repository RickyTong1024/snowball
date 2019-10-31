package com.yymoon.game;

import java.io.Console;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.*;

import com.unity3d.player.UnityPlayerActivity;

import android.content.Context;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.text.TextUtils;
import android.util.Log;
import android.view.OrientationEventListener;
import android.view.Surface;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Toast;
import com.yymoon.tool.AlarmReceiver;

public class GameActivity extends UnityPlayerActivity {
    private boolean m_zx = false;
    public Context mContext;
    public static final String TAG = "unity";
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
        HideSystemUI();
        Window window = getWindow();
        window.setFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON,
                WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
        AlarmReceiver.m_act = this;
    }

    private void startOrientationChangeListener() {
        OrientationEventListener mOrientationListener = new OrientationEventListener(this) {
            @Override
            public void onOrientationChanged(int rotation) {
                if (rotation > 45 && rotation < 135) {
                    if (!m_zx) {
                        m_zx = true;
                        GameActivity.this
                                .setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_REVERSE_LANDSCAPE);
                    }
                } else if (rotation > 225 && rotation < 315) {
                    if (m_zx) {
                        m_zx = false;
                        GameActivity.this
                                .setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE);
                    }
                }
            }
        };
        mOrientationListener.enable();
    }

    private void HideSystemUI() {
        if (Build.VERSION.SDK_INT >= 16) {
            final View decorView = this.getWindow().getDecorView();
            decorView.setSystemUiVisibility(0);
            decorView.setOnSystemUiVisibilityChangeListener(new View.OnSystemUiVisibilityChangeListener() {
                @Override
                public void onSystemUiVisibilityChange(int i) {
                    decorView.setSystemUiVisibility(0);
                }
            });
        }
    }

    protected void showToast(final String message) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Toast.makeText(GameActivity.this, message, Toast.LENGTH_SHORT).show();
            }
        });
    }

    public void game_login() {}

    public void game_logout() {}

    public void game_pay(String param) {}

    public void on_game_login(String parma, int role_new) {}

    public void on_game_user_upgrade(int level) {}

    public void game_update(String open_market, String open_url) {
        if (TextUtils.isEmpty(open_market)) {
            openLinkBySystem(open_url);
        } else {
            try {
                if (TextUtils.isEmpty(getPackageName())) {
                    return;
                }
                Uri uri = Uri.parse("market://details?id=" + getPackageName());
                Intent intent = new Intent(Intent.ACTION_VIEW, uri);
                intent.setPackage(open_market);
                intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                this.startActivity(intent);
            } catch (Exception e) {
                e.printStackTrace();
                openLinkBySystem(open_url);
            }
        }
    }

    private void openLinkBySystem(String url) {
        if (TextUtils.isEmpty(url)) {
            return;
        }
        Intent intent = new Intent(Intent.ACTION_VIEW);
        intent.setData(Uri.parse(url));
        startActivity(intent);
    }

    public int get_language() {
        Locale locale = getResources().getConfiguration().locale;
        String language = locale.getLanguage() + "-" + locale.getCountry();
        String m_language ="";
        if ("zh-CN".equals(language)) {
            return 0;
        }
        else if("zh-TW".equals(language))
        {
            return 1;
        }
        else if("zh-HK".equals(language))
        {
            return 1;
        }
        else {
            return 2;
        }
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
        this.runOnUiThread(new Runnable() {
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
                    Log.w(TAG, e.toString());
                    return;
                }
                bitmap.compress(Bitmap.CompressFormat.JPEG, 100, fos);
                try {
                    fos.flush();
                } catch (IOException e) {
                    // TODO Auto-generated catch block
                    Log.w(TAG, e.toString());
                    return;
                }
                try {
                    fos.close();
                } catch (IOException e) {
                    // TODO Auto-generated catch block
                    Log.w(TAG, e.toString());
                    return;
                }
                bitmap.recycle();
                mContext.sendBroadcast(new Intent(Intent.ACTION_MEDIA_SCANNER_SCAN_FILE, Uri.parse("file://" + Environment.getExternalStorageDirectory()
                        + "/DCIM/Camera/" + filename.replace(".png", ".jpg"))));

                Toast.makeText(mContext, "账号截图已保存到相册", Toast.LENGTH_SHORT).show();
            }
        });
    }

    public String get_bundle_name()
    {
        return this.getPackageName();
    }

    public String get_platform_id()
    {
        return  "";
    }

    public void share_prepared(String param){}

    public void share(String param){}
}
