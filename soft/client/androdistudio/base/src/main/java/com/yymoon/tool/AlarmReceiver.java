package com.yymoon.tool;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;

import android.app.ActivityManager;
import android.app.ActivityManager.RunningTaskInfo;
import android.app.AlarmManager;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.content.pm.PackageManager.NameNotFoundException;
import android.os.Bundle;
import android.support.v4.app.NotificationCompat;
import com.yymoon.game.GameActivity;

public class AlarmReceiver extends BroadcastReceiver {
	
	public static List<PendingIntent> piList = new ArrayList<PendingIntent>();
	public static GameActivity m_act;
	public static int m_count = 0;

	public static void startAlarm(String title, String text, String ticker, int secondsFromNow) {
		Calendar c = Calendar.getInstance();
		c.add(Calendar.SECOND, secondsFromNow);
		long alarmTime = c.getTimeInMillis();
		
		AlarmManager am = (AlarmManager) m_act.getSystemService(Context.ALARM_SERVICE);
		Intent ii = new Intent(m_act, AlarmReceiver.class);
		ii.putExtra("title", title);
		ii.putExtra("text", text);
		ii.putExtra("ticker", ticker);

		PendingIntent pi = PendingIntent.getBroadcast(m_act, m_count++, ii, PendingIntent.FLAG_UPDATE_CURRENT);
		am.set(AlarmManager.RTC_WAKEUP, alarmTime, pi);

		piList.add(pi);
	}

	public static void clearNotification() {
		NotificationManager mNM = (NotificationManager) m_act.getSystemService(Context.NOTIFICATION_SERVICE);
		mNM.cancelAll();
		
		AlarmManager am = (AlarmManager) m_act.getSystemService(Context.ALARM_SERVICE);
		for(PendingIntent pi : piList){
			am.cancel(pi);
		}
		piList.clear();
	}
	
	public static boolean isApplicationTop(final Context context) {
	    ActivityManager am = (ActivityManager) context.getSystemService(Context.ACTIVITY_SERVICE);
	    List<RunningTaskInfo> tasks = am.getRunningTasks(1);
	    if (!tasks.isEmpty()) {
	        ComponentName topActivity = tasks.get(0).topActivity;
	        if (topActivity.getPackageName().equals(context.getPackageName())) {
	            return true;
	        }
	    }
	    return false;
	}

	@Override
	public void onReceive(Context context, Intent intent) {
		if (isApplicationTop(context))
		{
			return;
		}
		NotificationManager mNM = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);

		Bundle bb = intent.getExtras();

		final PackageManager pm = context.getPackageManager();
		ApplicationInfo applicationInfo = null;
		try {
			applicationInfo = pm.getApplicationInfo(context.getPackageName(), PackageManager.GET_META_DATA);
		} catch (NameNotFoundException e) {
			e.printStackTrace();
			return;
		}

		int id = (int) (Math.random() * 10000.0f) + 1;
		Intent gameIntent = new Intent(context, GameActivity.class);
		gameIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
		gameIntent.addFlags(Intent.FLAG_ACTIVITY_SINGLE_TOP);
		PendingIntent contentIntent = PendingIntent.getActivity(context, id, gameIntent, 0);

		final int appIconResId = applicationInfo.icon;

		NotificationCompat.Builder builder = new NotificationCompat.Builder(context);
		builder.setContentTitle((String) bb.get("title"));
		builder.setContentText((String) bb.get("text"));
		builder.setSmallIcon(appIconResId).setContentIntent(contentIntent);
		builder.setAutoCancel(true);
		builder.setTicker((String) bb.get("ticker"));
		builder.setDefaults(Notification.DEFAULT_ALL);
		mNM.notify(id, builder.build());
	}
}
