//*************************************************************
//  File: SettingsActivity.cs
//  Date created: 12/22/2016
//  Date edited: 12/24/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: 
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App
{
	[Activity(Label = "Settings")]
	public class SettingsActivity : BaseActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.SetContentView(Resource.Layout.Settings);

			base.CreateDrawer();

			TextView pVersionLabel = FindViewById<TextView>(Resource.Id.lblVersion);
			pVersionLabel.Text = "App Version " + Master.APP_VERSION;

			// Create your application here

			Button pPass = FindViewById<Button>(Resource.Id.btnChangePassword);
			pPass.Click += delegate
			{
				Intent pIntent = new Intent(this, typeof(KeyActivity));
				StartActivity(pIntent);
			};
		}
	}
}