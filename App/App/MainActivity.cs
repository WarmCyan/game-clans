//*************************************************************
//  File: MainActivity.cs
//  Date created: 12/9/2016
//  Date edited: 12/9/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: 
//*************************************************************

using System;
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
	[Activity(Label = "App", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : BaseActivity // TODO: just extend MainActivity in the other activities and it should automagically get the drawer navigation?
	{
		int count = 1;


		private List<string> m_lNavTitles;

		private DrawerLayout m_pDrawerLayout;
		private ListView m_pDrawerList;


		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
			base.CreateDrawer();
		}
	}
}

