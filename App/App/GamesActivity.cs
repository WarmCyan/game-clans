//*************************************************************
//  File: GamesActivity.cs
//  Date created: 12/13/2016
//  Date edited: 12/22/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: 
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	[Activity(Label = "Games")]
	public class GamesActivity : BaseActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Games);

			// Create your application here
			base.CreateDrawer();

			// get the games list
			if (Master.GetActiveClan() != "")
			{
				//string sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/GameClansServer/GameClansServer/ClanServer/ListActiveGames", Master.BuildCommonBody(), true);
				this.RefreshGameList();

				Button pNew = FindViewById<Button>(Resource.Id.btnNewGame);
				pNew.Click += delegate
				{
					Intent pIntent = new Intent(this, typeof(NewGameActivity));
					this.StartActivityForResult(pIntent, 0);
				};
			}
		}

		private void RefreshGameList()
		{
			string sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetServerURL() + "ListActiveGames", Master.BuildCommonBody(), true);
			XElement pResponse = Master.ReadResponse(sResponse);

			List<string> lGames = new List<string>();
			List<string> lGameIDs = new List<string>();
			List<string> lGameNames = new List<string>();
			List<string> lDisplay = new List<string>();

			if (pResponse.Element("Data").Element("Games") != null)
			{
				foreach (XElement pGame in pResponse.Element("Data").Element("Games").Elements("Game"))
				{
					lGameIDs.Add(pGame.Value);
					lGames.Add(pGame.Attribute("GameType").Value);
					lGameNames.Add(pGame.Attribute("GameName").Value);

					lDisplay.Add(pGame.Attribute("GameName").Value + " (" + pGame.Attribute("GameType").Value + ")");
				}
			}

			ListView pGamesList = FindViewById<ListView>(Resource.Id.gamesList);
			pGamesList.Adapter = new DrawerItemCustomAdapter(this, Resource.Layout.ListViewItemRow, lDisplay.ToArray());

			pGamesList.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
			{
				int iChoice = e.Position;

				Intent pIntent = null;
				if (lGames[iChoice] == "Zendo") { pIntent = new Intent(this, (new ZendoActivity()).Class); }
				pIntent.SetAction(lGameIDs[iChoice]);
				pIntent.PutExtra("GameName", lGameNames[iChoice]);
				this.Finish();
				StartActivity(pIntent);
			};
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (resultCode == Result.Ok)
			{
				Intent pIntent = new Intent(this, typeof(GamesActivity));
				StartActivity(pIntent);
				this.Finish();
			}
		}
	}
}