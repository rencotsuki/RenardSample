package com.volpe;

import android.content.Context;
import android.content.Intent;
import android.os.BatteryManager;
import android.app.Activity;
import android.content.IntentFilter;

import com.unity3d.player.UnityPlayer;

public class  DeviceBatteryInfo
{
	public static int Level()
	{
		final Intent batteryStatus = GetBatteryStatus();
		return batteryStatus.getIntExtra(BatteryManager.EXTRA_LEVEL, -1);
	}

	public static int MaxLevel()
	{
		final Intent batteryStatus = GetBatteryStatus();
		return batteryStatus.getIntExtra(BatteryManager.EXTRA_SCALE, -1);
	}

	public static int Status()
	{
		final Intent batteryStatus = GetBatteryStatus();
		return batteryStatus.getIntExtra(BatteryManager.EXTRA_STATUS, -1);
	}

	private static Intent GetBatteryStatus()
	{
		final Activity activity = UnityPlayer.currentActivity;
		final Context context = activity.getApplicationContext();
		final IntentFilter ifilter = new IntentFilter(Intent.ACTION_BATTERY_CHANGED);
		final Intent batteryStatus = context.registerReceiver(null, ifilter);
		return batteryStatus;
	}

}