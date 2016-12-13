//*************************************************************
//  File: MainActivity.cs
//  Date created: 12/9/2016
//  Date edited: 12/11/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: 
//*************************************************************

using System;
using System.IO;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Android.Support.V4.Widget;

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

			if (Master.GetActiveClan() != "") { this.Title = Master.GetActiveClan() + " - " + Master.GetActiveUserName(); }
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

