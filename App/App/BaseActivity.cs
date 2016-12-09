//*************************************************************
//  File: BaseActivity.cs
//  Date created: 12/9/2016
//  Date edited: 12/9/2016
//  Author: Nathan Martindale
//  Copyright � 2016 Digital Warrior Labs
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
	public class BaseActivity : Activity
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
			m_lNavTitles = new List<string>() { "Home", "Games", /*"Chats",*/ "Profile", "Notifications", "Groups" };

			m_pDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.appDrawerLayout);
			m_pDrawerList = FindViewById<ListView>(Resource.Id.appDrawerList);
			m_pDrawerList.Adapter = new DrawerItemCustomAdapter(this, Resource.Layout.ListViewItemRow, m_lNavTitles.ToArray());

			m_pDrawerList.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
			{
				int iChoice = e.Position;
				//string sChoice = m_lNavTitles[iChoice];
				switch (iChoice)
				{
					case 0: // home
						break;
					case 1: // games
						break;
					case 2: // profile (eventually chats)
						break;
					case 3: // notifications	
						break;
					case 4: // groups
						Intent pIntent = new Intent(this, (new GroupListActivity()).Class);
						StartActivity(pIntent);
						break;
				}
			};
		}
	}
}