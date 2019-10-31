package com.yymoon.snowball;

import android.app.Activity;
import android.content.ClipData;
import android.content.ClipboardManager;
import android.content.Context;
import android.os.Looper;
import android.util.Log;

public class ClipboardTools {
    public static ClipboardManager clipboard = null;

    public void copyTextToClipboard(final Context context, final String str) {
        Log.i("copy_text:", str.toString());
        Thread runOnUiThread = new Thread(new Runnable() {
            @Override
            public void run() {
                if (Looper.myLooper() == null) {
                    Looper.prepare();
                }
                ClipboardManager clipboardManager = (ClipboardManager) context.getSystemService(Context.CLIPBOARD_SERVICE);
                ClipData clipData = ClipData.newPlainText("data", str);
                clipboardManager.setPrimaryClip(clipData);
                Looper.myLooper().quit();
            }
        });
        runOnUiThread.start();
    }
}
