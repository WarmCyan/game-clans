//*************************************************************
//  File: ZendoExampleRulesActivity.cs
//  Date created: 12/22/2016
//  Date edited: 12/22/2016
//  Author: Nathan Martindale
//  Copyright � 2016 Digital Warrior Labs
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
	[Activity(Label = "Example Zendo Rules")]
	public class ZendoExampleRulesActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.SetContentView(Resource.Layout.Game_ZendoExampleRules);

			// Create your application here
		}
	}
}