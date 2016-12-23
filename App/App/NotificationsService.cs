//*************************************************************
//  File: NotificationsService.cs
//  Date created: 12/22/2016
//  Date edited: 12/22/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: Takes care of sending notifications when the app isn't open (the current model pulls server data every 15 minutes)
//		Note: thanks to http://stackoverflow.com/questions/4459058/alarm-manager-example
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using DWL.Utility;

namespace App
{
	[Service]
	class NotificationsService : Service
	{
		NotificationAlarm m_pAlarm = new NotificationAlarm();

		public override void OnCreate() { base.OnCreate(); }

		public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
		{
			//return base.OnStartCommand(intent, flags, startId);
			m_pAlarm.SetAlarm(this);
			return StartCommandResult.Sticky;
		}

		public override IBinder OnBind(Intent intent) { return null; }
	}

	[BroadcastReceiver]
	class NotificationAlarm : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			PowerManager pPowerManager = (PowerManager)context.GetSystemService(Context.PowerService);
			PowerManager.WakeLock pWakeLock = pPowerManager.NewWakeLock(WakeLockFlags.Partial, "GC_NOTIFS");

			pWakeLock.Acquire();

			if (Master.GetSetting("notifications") == "on") 
			{ 
				this.OnWakeInitializationProtocol();

				foreach (string sLine in File.ReadAllLines(Master.GetBaseDir() + "_clans.dat").ToList())
				{
					string sClan = sLine.Substring(0, sLine.IndexOf("|"));
					string sUserName = sLine.Substring(sLine.IndexOf("|") + 1);
					this.CheckServerNotifications(context, sClan, sUserName);
				}
			}

			pWakeLock.Release();
		}

		private void OnWakeInitializationProtocol() 
		{ 
			//Directory.SetCurrentDirectory(Master.GetBaseDir());

			/*string[] aLines = File.ReadAllLines(Master.GetBaseDir() + "_active.dat");
			Master.SetActiveClan(aLines[0]);
			Master.SetActiveUserName(aLines[1]);*/
			Master.SetKey(File.ReadAllText(Master.GetBaseDir() + "_key.dat"));
		}

		private void CheckServerNotifications(Context pContext, string sClan, string sName)
		{
			XElement pResponse = null;
			//try
			//{
				string sPrevClan = Master.GetActiveClan();
				string sPrevUser = Master.GetActiveUserName();
				Master.SetActiveClan(sClan);
				Master.SetActiveUserName(sName);
				string sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetServerURL() + "GetUnseenNotifications", Master.BuildCommonBody(), true);
				pResponse = Master.ReadResponse(sResponse);
				Master.SetActiveClan(sPrevClan);
				Master.SetActiveUserName(sPrevUser);
			//}
			//catch (Exception e) { return; }

			if (pResponse.Element("Data") != null && pResponse.Element("Data").Element("Notifications").Value != "")
			{
				List<XElement> pNotifications = pResponse.Element("Data").Element("Notifications").Elements("Notification").ToList();
				foreach (XElement pNotification in pNotifications)
				{
					string sContent = pNotification.Value;
					string sGameID = pNotification.Attribute("GameID").Value;
					string sGameName = pNotification.Attribute("GameName").Value;

					Notification.Builder pBuilder = new Notification.Builder(pContext);
					pBuilder.SetContentTitle(sClan + " - " + sGameName);
					pBuilder.SetContentText(sContent);
					pBuilder.SetSmallIcon(Resource.Drawable.Icon);
					pBuilder.SetVibrate(new long[] { 200, 50, 200, 50 });
					pBuilder.SetVisibility(NotificationVisibility.Public);
					pBuilder.SetPriority((int)NotificationPriority.Default);

					Intent pIntent = null;
					if (sGameID.Contains("Zendo")) { pIntent = new Intent(pContext, typeof(ZendoActivity)); }
					pIntent.SetAction(sGameID);
					pIntent.PutExtra("GameName", sGameName);

					pBuilder.SetContentIntent(PendingIntent.GetActivity(pContext, 0, pIntent, 0));
					pBuilder.SetStyle(new Notification.BigTextStyle().BigText(sContent));

					Notification pNotif = pBuilder.Build();
					NotificationManager pManager = (NotificationManager)pContext.GetSystemService(Context.NotificationService);
					pManager.Notify((int)Java.Lang.JavaSystem.CurrentTimeMillis(), pNotif); // using time to make different ID every time, so doesn't replace old notification
					//pManager.Notify(DateTime.Now.Millisecond, pNotif); // using time to make different ID every time, so doesn't replace old notification
				}
			}
		}

		public void SetAlarm(Context pContext)
		{
			AlarmManager pManager = (AlarmManager)pContext.GetSystemService(Context.AlarmService);
			Intent pIntent = new Intent(pContext, typeof(NotificationAlarm));

			PendingIntent pPendingIntent = PendingIntent.GetBroadcast(pContext, 0, pIntent, 0);

			DateTime pCurrent = DateTime.Now;
			pCurrent.AddMinutes(1);

			long lInterval = 1000 * 60; // minute
			long lMS = this.GetMS(pCurrent);

			pManager.SetInexactRepeating(AlarmType.RtcWakeup, lMS, lInterval, pPendingIntent);
		}

		// also converts to utc and based on unix time
		private long GetMS(DateTime time)
		{
			DateTime UnixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

			long unixTicks = UnixTime.Ticks;
			long timeTicksUTC = time.ToUniversalTime().Ticks;

			long actualTicks = timeTicksUTC - unixTicks;

			return actualTicks / TimeSpan.TicksPerMillisecond;
		}
	}
}