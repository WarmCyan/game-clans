//*************************************************************
//  File: GroupListActivity.cs
//  Date created: 12/9/2016
//  Date edited: 12/9/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: The activity where you can join new groups, create a group, or switch active groups
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
	[Activity(Label = "GroupListActivity")]
	public class GroupListActivity : BaseActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			this.SetContentView(Resource.Layout.Groups);
			base.CreateDrawer();

			// Create your application here
		}
	}
}