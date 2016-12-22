//*************************************************************
//  File: BaseActivity.cs
//  Date created: 12/9/2016
//  Date edited: 12/22/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: Base activity that other activities should extend to the same drawer layouts
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

using Android.Support.V4.Widget;

namespace App
{
	[Activity(Label = "BaseActivity")]
	public class BaseActivity : Activity, ScrollView.IOnScrollChangeListener
	{

		private List<string> m_lNavTitles;

		private DrawerLayout m_pDrawerLayout;
		private ListView m_pDrawerList;


		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		protected void CreateDrawer()
		{
			m_lNavTitles = new List<string>() { "Home", "Games", /*"Chats",*/ "Profile", "Notifications", "Clans", "Settings" };
			string sActive = Master.GetActiveClan();
			if (sActive != "") 
			{ 
				m_lNavTitles[0] = sActive;
				m_lNavTitles[2] = Master.GetActiveUserName() + "'s Profile";
			}

			m_pDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.appDrawerLayout);
			m_pDrawerList = FindViewById<ListView>(Resource.Id.appDrawerList);
			m_pDrawerList.Adapter = new DrawerItemCustomAdapter(this, Resource.Layout.ListViewItemRow, m_lNavTitles.ToArray());

			m_pDrawerList.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
			{
				int iChoice = e.Position;
				//string sChoice = m_lNavTitles[iChoice];
				Intent pIntent = null;
				switch (iChoice)
				{
					case 0: // home
						pIntent = new Intent(this, typeof(MainActivity));
						break;
					case 1: // games
						pIntent = new Intent(this, typeof(GamesActivity));
						break;
					case 2: // profile (eventually chats)
						break;
					case 3: // notifications	
						pIntent = new Intent(this, typeof(NotificationsActivity));
						break;
					case 4: // groups
						pIntent = new Intent(this, typeof(GroupListActivity));
						break;
					case 5: // settings
						break;
				}
				if (pIntent == null) { return; }
				this.Finish();
				StartActivity(pIntent);
			};
		}

		// enables the pull-down-to-refresh ONLY when scroll is all the way at the top of the page
		public void OnScrollChange(View v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
		{
			SwipeRefreshLayout pRefresher = FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
			if (scrollY == 0) { pRefresher.Enabled = true; }
			else { pRefresher.Enabled = false; }
		}
	}
}