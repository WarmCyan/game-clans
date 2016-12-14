//*************************************************************
//  File: GamesActivity.cs
//  Date created: 12/13/2016
//  Date edited: 12/13/2016
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
	[Activity(Label = "GamesActivity")]
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
				string sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/GameClansServer/GameClansServer/ClanServer/ListActiveGames", Master.BuildCommonBody(), true);
				XElement pResponse = Master.ReadResponse(sResponse);

				List<string> lGames = new List<string>();
				List<string> lGameIDs = new List<string>();

				if (pResponse.Element("Data").Element("Games") != null)
				{
					foreach (XElement pGame in pResponse.Element("Data").Element("Games").Elements("Game"))
					{
						lGameIDs.Add(pGame.Value);
						lGames.Add(pGame.Attribute("GameType").Value);
					}
				}
				
				ListView pGamesList = FindViewById<ListView>(Resource.Id.gamesList);
				pGamesList.Adapter = new DrawerItemCustomAdapter(this, Resource.Layout.ListViewItemRow, lGames.ToArray());

				pGamesList.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
				{
					int iChoice = e.Position;

					Intent pIntent = null;
					if (lGames[iChoice] == "Zendo") { pIntent = new Intent(this, (new ZendoActivity()).Class); }
					pIntent.SetAction(lGameIDs[iChoice]);
					this.Finish();
					StartActivity(pIntent);
				};
			}
			
		}
	}
}