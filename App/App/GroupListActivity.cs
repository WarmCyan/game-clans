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
		// member variables
		private List<string> m_lClanNames; // clan name - user name
		private List<string> m_lClanParts; // clan name only
		private List<string> m_lUserParts; // user name only
	
		
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
			m_lClanNames = File.ReadAllLines(Master.GetBaseDir() + "_clans.dat").ToList();
			m_lClanParts = new List<string>();
			m_lUserParts = new List<string>();

			// clan name|user name
			//for (int i = 0; i < aClansList.Length; i++) { aClansList[i] = aClansList[i].Substring(0, aClansList[i].IndexOf("|")); }
			for (int i = 0; i < m_lClanNames.Count; i++) 
			{
				m_lClanParts.Add(m_lClanNames[i].Substring(0, m_lClanNames[i].IndexOf("|")));
				m_lUserParts.Add(m_lClanNames[i].Substring(m_lClanNames[i].IndexOf("|") + 1));
				m_lClanNames[i] = m_lClanNames[i].Replace("|", " - "); 
			}
			
			ListView pClansList = FindViewById<ListView>(Resource.Id.clansList);
			pClansList.Adapter = new DrawerItemCustomAdapter(this, Resource.Layout.ListViewItemRow, m_lClanNames.ToArray());

			pClansList.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
			{
				int iChoice = e.Position;
				Master.SetActiveClan(m_lClanParts[iChoice]);
				Master.SetActiveUserName(m_lUserParts[iChoice]);

				// store this in the _active file, so as to persist between app openings and closings
				List<string> lLines = new List<string>();
				lLines.Add(m_lClanParts[iChoice]);
				lLines.Add(m_lUserParts[iChoice]);
				File.WriteAllLines(Master.GetBaseDir() + "_active.dat", lLines.ToArray());
				
				Intent pIntent = new Intent(this, (new MainActivity()).Class);
				this.Finish();
				StartActivity(pIntent);
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