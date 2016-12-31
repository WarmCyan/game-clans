//*************************************************************
//  File: RegisterOrLogInActivity.cs
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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App
{
	[Activity(Label = "Register or Log In")]
	public class RegisterOrLogInActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.SetContentView(Resource.Layout.RegisterOrLogin);


			Button pRegister = FindViewById<Button>(Resource.Id.btnRegister);
			pRegister.Click += delegate
			{
				Intent pIntent = new Intent(this, typeof(KeyActivity));
				this.Finish();
				StartActivity(pIntent);
			};

			Button pLogin = FindViewById<Button>(Resource.Id.btnLogIn);
			pLogin.Click += delegate
			{
				Intent pIntent = new Intent(this, typeof(Login));
				this.Finish();
				StartActivity(pIntent);
			};
		}
	}
}