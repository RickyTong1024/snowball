package com.yymoon.tool;

import android.app.Activity;
import android.content.ClipData;
import android.content.ClipboardManager;
import android.content.Context;
import android.os.Looper;

import com.yymoon.game.GameActivity;

public class ClipboardTools {
	public static ClipboardManager clipboard = null;

	public void copyTextToClipboard(final Context context,final String str)
	{
		if (Looper.myLooper() == null)
		{
			Looper.prepare();
		}
		clipboard = (ClipboardManager) context.getSystemService(Activity.CLIPBOARD_SERVICE);
		ClipData textCd = ClipData.newPlainText("data", str);
		clipboard.setPrimaryClip(textCd);
		Looper.myLooper().quit();
	}
}
