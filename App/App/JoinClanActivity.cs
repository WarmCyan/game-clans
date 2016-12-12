//*************************************************************
//  File: JoinClanActivity.cs
//  Date created: 12/11/2016
//  Date edited: 12/12/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: 
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
	[Activity(Label = "Join Clan")]
	public class JoinClanActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.JoinGroup);

			EditText pClan = FindViewById<EditText>(Resource.Id.txtClanName);
			EditText pPass = FindViewById<EditText>(Resource.Id.txtClanPassword);
			EditText pUser = FindViewById<EditText>(Resource.Id.txtUserName);
			Button pJoin = FindViewById<Button>(Resource.Id.btnSubmitJoin);

			pJoin.Click += delegate
			{
				// TODO: SANITIZATION!
				_hidden.InitializeWeb();
				string sUserPass = File.ReadAllText(Master.GetBaseDir() + "_key.dat");
				string sBody = "<params><param name='sClanName'>" + pClan.Text.ToString() + "</param><param name='sClanPassPhrase'>" + pPass.Text.ToString() + "</param><param name='sUserName'>" + pUser.Text.ToString() + "</param><param name='sUserPassPhrase'>" + sUserPass +  "</param></params>";
				string sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/GameClansServer/GameClansServer/ClanServer/JoinClan", sBody, true);

				XElement pResponse = null;
				try
				{
					pResponse = XElement.Parse(Master.CleanResponse(sResponse));
				}
				catch (Exception e)
				{
					sResponse = "<Message Type='Error'><Text>" + e.Message + "</Text><Data /></Message>";
					pResponse = XElement.Parse(sResponse);
				}
				
				//Master.Popup(this, pResponse.Element("Text").Value);
				string sResponseMssage = pResponse.Element("Text").Value;

				var pBuilder = new AlertDialog.Builder(this);
				pBuilder.SetMessage(sResponseMssage);

				if (pResponse.Attribute("Type").Value == "Error" || pResponse.Element("Data").Element("ClanStub") == null)
				{
					pBuilder.SetPositiveButton("Ok", (e, s) => { return; });
				}
				else
				{
					pBuilder.SetPositiveButton("Ok", (e, s) =>
					{
						XElement pClanStub = pResponse.Element("Data").Element("ClanStub");
						string sClanName = pClanStub.Attribute("ClanName").Value;
						string sUserName = pClanStub.Attribute("UserName").Value;

						File.AppendAllLines(Master.GetBaseDir() + "_clans.dat", new List<string>() { sClanName + "|" + sUserName });
						this.SetResult(Result.Ok);
						this.Finish();
					});
				}
				pBuilder.Create().Show();
			};
		}
	}
}