//*************************************************************
//  File: JoinClanActivity.cs
//  Date created: 12/11/2016
//  Date edited: 12/11/2016
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
	[Activity(Label = "JoinClanActivity")]
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

				string sBody = "<params><param name=sClanName='>" + pClan.Text + "</param><param name='sClanPassPhrase'>" + pPass.Text + "</param><param name='sUserName'>" + pUser.Text + "</param><param name='sUserPassPhrase'>" + File.ReadAllText(Master.GetBaseDir() + "_key.dat") + "</param></params>";
				string sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/GameClansServer/GameClansServer/ClanServer/JoinClan", sBody, true);

				XElement pResponse = XElement.Parse(sResponse);
				Master.Popup(this, pResponse.Element("Text").Value);
				if (pResponse.Attribute("Type").Value == "Error") { return; }
				
				this.Finish();
			};
		}
	}
}