//*************************************************************
//  File: ZendoCreateRuleActivity.cs
//  Date created: 12/16/2016
//  Date edited: 12/16/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: Note, this activity should also handle the creation of the two initial koans. (since the server handles it all in one request)
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
	[Activity(Label = "CreateRule")]
	public class ZendoCreateRuleActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
		}
	}
}