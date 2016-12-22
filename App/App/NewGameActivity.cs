//*************************************************************
//  File: NewGameActivity.cs
//  Date created: 12/22/2016
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
	[Activity(Label = "Start New Game")]
	public class NewGameActivity : Activity
	{
		// member variables
		private List<string> m_lGameTypes = new List<string>() { "Zendo" };
		private string m_sSelectedType = "";
	
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.SetContentView(Resource.Layout.NewGame);

			// Create your application here

			EditText pName = FindViewById<EditText>(Resource.Id.txtGameName);

			// games list
			ListView pGamesList = FindViewById<ListView>(Resource.Id.gamesList);
			pGamesList.Adapter = new DrawerItemCustomAdapter(this, Resource.Layout.ListViewItemRow, m_lGameTypes.ToArray());

			pGamesList.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
			{
				int iChoice = e.Position;
				m_sSelectedType = m_lGameTypes[iChoice];

				TextView pGameType = FindViewById<TextView>(Resource.Id.txtGameType);
				pGameType.Text = "Game type - " + m_sSelectedType;
			};

			Button pSubmit = FindViewById<Button>(Resource.Id.btnSubmitGame);
			pSubmit.Click += delegate
			{
				if (m_sSelectedType == "")
				{
					var pBuilder = new AlertDialog.Builder(this);
					pBuilder.SetMessage("You must select a game type");
					pBuilder.SetPositiveButton("Ok", (e, s) => { return; });
					pBuilder.Show();
				}
				else if (pName.Text == "")
				{
					var pBuilder = new AlertDialog.Builder(this);
					pBuilder.SetMessage("Please enter a name for your game");
					pBuilder.SetPositiveButton("Ok", (e, s) => { return; });
					pBuilder.Show();
				}
				else
				{
					string sBody = Master.BuildCommonBody("<param name='sGameName'>" + pName.Text.ToString() + "</param>");
					string sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetGameURL(m_sSelectedType) + "CreateNewGame", sBody, true);
					XElement pResponse = Master.ReadResponse(sResponse);
					string sResponseMessage = pResponse.Element("Text").Value;

					var pBuilder = new AlertDialog.Builder(this);
					pBuilder.SetMessage(sResponseMessage);

					if (pResponse.Attribute("Type").Value == "Error") 
					{
						pBuilder.SetPositiveButton("Ok", (e, s) => { return; });
					}
					else
					{
						pBuilder.SetPositiveButton("Ok", (e, s) =>
						{
							this.SetResult(Result.Ok);
							this.Finish();
						});
					}
					pBuilder.Create().Show();
				}
			};
		}
	}
}