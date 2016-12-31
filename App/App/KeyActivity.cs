//*************************************************************
//  File: KeyActivity.cs
//  Date created: 12/11/2016
//  Date edited: 12/31/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: 
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
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
	[Activity(Label = "Register New User")]
	public class KeyActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Key);

			EditText pEmail = FindViewById<EditText>(Resource.Id.txtEmail);
			EditText pPass1 = FindViewById<EditText>(Resource.Id.txtUserPassword);
			EditText pPass2 = FindViewById<EditText>(Resource.Id.txtUserPassword2);

			Button pSubmit = FindViewById<Button>(Resource.Id.btnSetPassword);

			pSubmit.Click += delegate
			{
				if (pEmail.Text == "")
				{
					Master.Popup(this, "Your email cannot be blank.");
				}
				if (pPass1.Text == "")
				{
					Master.Popup(this, "Your password cannot be blank.");
					return;
				}

				if (pPass1.Text != pPass2.Text)
				{
					Master.Popup(this, "The password fields do not match.");
					return;
				}


				string sBody = "<params><param name='sEmail'>" + pEmail.Text.ToString() + "</param><param name='sPassword'>" + Security.Sha256Hash(pPass1.Text.ToString()) + "</param></params>";
				string sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetServerURL() + "RegisterUser", sBody, true);
				XElement pResponse = Master.ReadResponse(sResponse);

				string sResponseMessage = pResponse.Element("Text").Value;

				if (pResponse.Attribute("Type").Value == "Error")
				{
					Popup(sResponseMessage);
					return;
				}
				else
				{
					//MessageBox.Show(sResponseMessage);
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