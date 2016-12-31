//*************************************************************
//  File: Login.cs
//  Date created: 12/31/2016
//  Date edited: 12/31/2016
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
	[Activity(Label = "Login")]
	public class Login : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.SetContentView(Resource.Layout.Login);

			EditText pEmail = FindViewById<EditText>(Resource.Id.txtEmail);
			EditText pPass = FindViewById<EditText>(Resource.Id.txtUserPassword);
			
			Button pSubmit = FindViewById<Button>(Resource.Id.btnLogin);
			pSubmit.Click += delegate
			{
				string sBody = "<params><param name='sEmail'>" + pEmail.Text.ToString() + "</param><param name='sPassword'>" + Security.Sha256Hash(pPass.Text.ToString()) + "</param></params>";
				string sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetServerURL() + "ReturningUser", sBody, true);
				XElement pResponse = Master.ReadResponse(sResponse);

				string sResponseMessage = pResponse.Element("Text").Value;

				if (pResponse.Attribute("Type").Value == "Error") { this.Popup(sResponseMessage); }
				else
				{
					Master.HandleUserRegistrationData(pResponse);
					this.Finish();
				}
			};
		}
		private void Popup(string sMessage)
		{
			var pBuilder = new AlertDialog.Builder(this);
			pBuilder.SetMessage(sMessage);
			pBuilder.Create().Show();
		}
	}
}