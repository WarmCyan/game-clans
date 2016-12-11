//*************************************************************
//  File: KeyActivity.cs
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
using System.Security.Cryptography;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using DWL.Utility;

namespace App
{
	[Activity(Label = "Enter Password")]
	public class KeyActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Key);

			EditText pPass1 = FindViewById<EditText>(Resource.Id.txtUserPassword);
			EditText pPass2 = FindViewById<EditText>(Resource.Id.txtUserPassword2);

			Button pSubmit = FindViewById<Button>(Resource.Id.btnSetPassword);

			pSubmit.Click += delegate
			{
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

				// if non-blank key file, user is changing password, meaning if not all clans joined on this device, passwords will get out of sync
				if (File.Exists(Master.GetBaseDir() + "_key.dat"))
				{
					bool bContinue = false;
					var pBuilder = new AlertDialog.Builder(this);
					pBuilder.SetMessage("Please note that your password is used by default for all of your usernames in the different clans. If this device does not have every clan you play with, your passwords will be out of sync. Are you sure you want to continue?");
					pBuilder.SetPositiveButton("Yes", (s, e) => { bContinue = true; });
					pBuilder.SetNegativeButton("No", (s, e) => { return; });
					pBuilder.Create().Show();

					// unnecessary safe-guard to make me feel better
					if (!bContinue) { return; }

					// TODO: make the server call
				}

				File.WriteAllText(Master.GetBaseDir() + "_key.dat", this.Sha256Hash(pPass1.Text));
				this.Finish();
			};
		}

		private void Popup(string sMessage)
		{
			var pBuilder = new AlertDialog.Builder(this);
			pBuilder.SetMessage(sMessage);
			pBuilder.Create().Show();
		}
		
		// thanks to http://www.codeshare.co.uk/blog/sha-256-and-sha-512-hash-examples/
		private string Sha256Hash(string sString)
		{
			SHA256 p256 = SHA256Managed.Create();
			byte[] aBytes = Encoding.UTF8.GetBytes(sString);
			byte[] aHashBytes = p256.ComputeHash(aBytes);
			string sHash = this.GetStringFromHash(aHashBytes);
			return sHash;
		}

		private string GetStringFromHash(byte[] aHashBytes)
		{
			StringBuilder pResult = new StringBuilder();
			for (int i = 0; i < aHashBytes.Length; i++)
			{
				pResult.Append(aHashBytes[i].ToString("X2"));
			}
			return pResult.ToString();
		}
	}
}