//*************************************************************
//  File: ZendoJudgeKoanActivity.cs
//  Date created: 12/16/2016
//  Date edited: 12/20/2016
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
	[Activity(Label = "Got Buddha-Nature?")]
	public class ZendoJudgeKoanActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.SetContentView(Resource.Layout.Game_ZendoAnalyzeKoan);

			// display the koan
			FlowLayout pFlow = FindViewById<FlowLayout>(Resource.Id.flowKoan);
			string sKoan = this.Intent.GetStringExtra("Koan");
			string sRule = this.Intent.GetStringExtra("Rule");
			Master.FillKoanDisplay(this, pFlow, sKoan);

			TextView pRuleText = FindViewById<TextView>(Resource.Id.txtRule);
			pRuleText.Text = "Your rule - '" + sRule + "'";


			Button pYes = FindViewById<Button>(Resource.Id.btnHasBuddhaNature);
			Button pNo = FindViewById<Button>(Resource.Id.btnHasNotBuddhaNature);

			pYes.Click += delegate
			{
				Intent pIntent = new Intent(this, typeof(ZendoActivity));
				pIntent.PutExtra("Type", "analysis");
				pIntent.PutExtra("HasBuddhaNature", true);
				this.SetResult(Result.Ok, pIntent);
				this.Finish();
			};
			pNo.Click += delegate
			{
				Intent pIntent = new Intent(this, typeof(ZendoActivity));
				pIntent.PutExtra("Type", "analysis");
				pIntent.PutExtra("HasBuddhaNature", false);
				this.SetResult(Result.Ok, pIntent);
				this.Finish();
			};
		}
	}
}