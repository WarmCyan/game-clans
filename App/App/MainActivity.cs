﻿//*************************************************************
//  File: MainActivity.cs
//  Date created: 12/9/2016
//  Date edited: 12/16/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: 
//*************************************************************

using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Android.Support.V4.Widget;
using DWL.Utility;

namespace App
{
	[Activity(Label = "Game Clans", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : BaseActivity // TODO: just extend MainActivity in the other activities and it should automagically get the drawer navigation?
	{
	
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			_hidden.InitializeWeb();

			_RunTempCommands(); // TODO: REMOVE

			// make sure that all the necessary files exist
			if (!File.Exists(Master.GetBaseDir() + "_clans.dat")) { File.Create(Master.GetBaseDir() + "_clans.dat").Dispose(); }
			if (!File.Exists(Master.GetBaseDir() + "_settings.dat")) { File.WriteAllText(Master.GetBaseDir() + "_settings.dat", "notifications=on"); }
			if (!File.Exists(Master.GetBaseDir() + "_key.dat"))
			{
				Intent pIntent = new Intent(this, (new KeyActivity().Class));
				StartActivity(pIntent);
			}
			else { Master.SetKey(File.ReadAllText(Master.GetBaseDir() + "_key.dat")); }

			// TODO: save temp currently active clan so persistent between app open times, and load it here
			if (File.Exists(Master.GetBaseDir() + "_active.dat")) 
			{ 
				string[] aLines = File.ReadAllLines(Master.GetBaseDir() + "_active.dat");
				Master.SetActiveClan(aLines[0]);
				Master.SetActiveUserName(aLines[1]);
			}

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
			base.CreateDrawer();

			if (Master.GetActiveClan() != "") 
			{ 
				this.Title = Master.GetActiveClan() + " - " + Master.GetActiveUserName();
				this.BuildHomeDashboard();
			}

			//StartService(new Intent(this, typeof(NotificationsService)));
		}

		private void BuildHomeDashboard()
		{
			// get the scoreboard from the server
			string sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetServerURL() + "GetClanLeaderboard", Master.BuildCommonBody(), true);
			XElement pResponse = Master.ReadResponse(sResponse);

			// build the scoreboard
			if (pResponse.Element("Data").Element("Leaderboard") != null)
			{
				XElement pLeaderboard = pResponse.Element("Data").Element("Leaderboard");

				TextView pMyUser = FindViewById<TextView>(Resource.Id.lblMyStatsName);
				TextView pMyScore = FindViewById<TextView>(Resource.Id.lblMyStatsScore);
				TextView pMyPlace = FindViewById<TextView>(Resource.Id.lblMyStatsPlace);

				pMyUser.Text = Master.GetActiveUserName();
				pMyScore.Text = pLeaderboard.Attribute("Score").Value;
				pMyPlace.Text = pLeaderboard.Attribute("Place").Value;

				LinearLayout pScoreboardLayout = FindViewById<LinearLayout>(Resource.Id.lstScoreBoard);
				pScoreboardLayout.RemoveAllViews();

				foreach (XElement pScoreXml in pLeaderboard.Elements("Score"))
				{
					View pScoreRow = LayoutInflater.From(this).Inflate(Resource.Layout.ScoreRow, pScoreboardLayout, false);

					TextView pUser = pScoreRow.FindViewById<TextView>(Resource.Id.lblScoreName);
					TextView pPlace = pScoreRow.FindViewById<TextView>(Resource.Id.lblScorePlace);
					TextView pScore = pScoreRow.FindViewById<TextView>(Resource.Id.lblScoreScore);

					pUser.Text = pScoreXml.Attribute("User").Value;
					pPlace.Text = pScoreXml.Attribute("Place").Value;
					pScore.Text = pScoreXml.Attribute("Score").Value;

					pScoreboardLayout.AddView(pScoreRow);
				}
			}

			// get the notifcations list from the server
			sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetServerURL() + "GetUnreadNotifications", Master.BuildCommonBody(), true);
			pResponse = Master.ReadResponse(sResponse);

			LinearLayout pNotifLayout = FindViewById<LinearLayout>(Resource.Id.lstNotifications);
			pNotifLayout.RemoveAllViews();
			if (pResponse.Element("Data").Element("Notifications") != null)
			{
				foreach (XElement pNotif in pResponse.Element("Data").Element("Notifications").Elements("Notification"))
				{
					// make the datarow layout
					View pDataRow = LayoutInflater.From(this).Inflate(Resource.Layout.DataRow, pNotifLayout, false);

					TextView pDataText = pDataRow.FindViewById<TextView>(Resource.Id.txtText);
					pDataText.Text = pNotif.Attribute("GameName").Value + " - " + pNotif.Value;

					pDataText.Click += delegate
					{
						string sGameID = pNotif.Attribute("GameID").Value;
						Intent pIntent = null;
						if (sGameID.Contains("Zendo")) { pIntent = new Intent(this, typeof(ZendoActivity)); }
						pIntent.SetAction(sGameID);
						pIntent.PutExtra("GameName", pNotif.Attribute("GameName").Value);
						this.Finish();
						StartActivity(pIntent);
					};

					pNotifLayout.AddView(pDataRow);
				}
			}

			Button pMarkRead = FindViewById<Button>(Resource.Id.btnMarkRead);
			pMarkRead.Click += delegate
			{
				WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetServerURL() + "MarkUnreadNotificationsRead", Master.BuildCommonBody(), true);
				this.BuildHomeDashboard();
			};
		}

		/*protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (resultCode == Result.Ok)
			{
				// TODO: HANDLE EXTRA HERE
			}
		}*/

		private void _RunTempCommands()
		{
			if (Master.TEMP_RUN) { return; }


			//File.Delete(Master.GetBaseDir() + "_clans.dat");

			
			Master.TEMP_RUN = true;
		}
	}
}

