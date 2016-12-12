//*************************************************************
//  File: GroupListActivity.cs
//  Date created: 12/9/2016
//  Date edited: 12/12/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: The activity where you can join new groups, create a group, or switch active groups
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App
{
	[Activity(Label = "Your Clans")]
	public class GroupListActivity : BaseActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			this.SetContentView(Resource.Layout.Groups);
			base.CreateDrawer();

			// Create your application here
			this.RefreshClanList();

			Button pJoinButton = FindViewById<Button>(Resource.Id.btnJoin);

			pJoinButton.Click += delegate
			{
				Intent pIntent = new Intent(this, (new JoinClanActivity()).Class);
				StartActivityForResult(pIntent, 0); // TODO: actually just have join clan activity add the stuff to the text file and just call a function to update?
				//this.RefreshClanList();
			};
		}

		private void RefreshClanList()
		{
			string[] aClansList = File.ReadAllLines(Master.GetBaseDir() + "_clans.dat");

			// clan name|user name
			//for (int i = 0; i < aClansList.Length; i++) { aClansList[i] = aClansList[i].Substring(0, aClansList[i].IndexOf("|")); }
			for (int i = 0; i < aClansList.Length; i++) { aClansList[i] = aClansList[i].Replace("|", " - "); }
			
			ListView pClansList = FindViewById<ListView>(Resource.Id.clansList);
			pClansList.Adapter = new DrawerItemCustomAdapter(this, Resource.Layout.ListViewItemRow, aClansList);

			pClansList.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
			{
				int iChoice = e.Position;
				string sClanName = aClansList[iChoice];
				// TODO: handle clicking on that clan name here
			};
		}
		
		// meant for getting a new joined clan from the join clan activity
		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (resultCode == Result.Ok)
			{
				// TODO: HANDLE EXTRA HERE
				this.RefreshClanList();
			}
		}
	}
}