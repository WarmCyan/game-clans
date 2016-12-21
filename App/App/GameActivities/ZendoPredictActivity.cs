//*************************************************************
//  File: ZendoPredictActivity.cs
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

namespace App
{
	[Activity(Label = "Predict Buddha-Nature")]
	public class ZendoPredictActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.SetContentView(Resource.Layout.Game_ZendoPredict);

			// Create your application here

			string sKoan = this.Intent.GetStringExtra("Koan");
			string sKoans = this.Intent.GetStringExtra("Koans"); // xml

			// fill the primary koan thing
			FlowLayout pPredictFlow = FindViewById<FlowLayout>(Resource.Id.flowKoan);
			Master.FillKoanDisplay(this, pPredictFlow, sKoan);

			// fill the list of koans
			LinearLayout pKoansLayout = FindViewById<LinearLayout>(Resource.Id.lstKoans);
			pKoansLayout.RemoveAllViews();
			XElement pKoansXml = XElement.Parse(sKoans);
			List<XElement> pKoans = pKoansXml.Elements("Koan").ToList();
			for (int i = pKoans.Count - 1; i >= 0; i--)
			{
				XElement pKoan = pKoans[i];

				// create the row view
				View pView = LayoutInflater.From(this).Inflate(Resource.Layout.Game_ZendoKoanRow, pKoansLayout, false);
				FlowLayout pFlow = pView.FindViewById<FlowLayout>(Resource.Id.lstKoanImages);
				
				string sKoanText = pKoan.Value;
				Master.FillKoanDisplay(this, pFlow, sKoanText);

				pKoansLayout.AddView(pView);
			}

			// buttons
			Button pYes = FindViewById<Button>(Resource.Id.btnHasBuddhaNature);
			Button pNo = FindViewById<Button>(Resource.Id.btnHasNotBuddhaNature);

			pYes.Click += delegate
			{
				Intent pIntent = new Intent(this, typeof(ZendoActivity));
				pIntent.PutExtra("Type", "predict");
				pIntent.PutExtra("Prediction", true);
				this.SetResult(Result.Ok, pIntent);
				this.Finish();
			};
			pNo.Click += delegate
			{
				Intent pIntent = new Intent(this, typeof(ZendoActivity));
				pIntent.PutExtra("Type", "predict");
				pIntent.PutExtra("Prediction", false);
				this.SetResult(Result.Ok, pIntent);
				this.Finish();
			};
		}
	}
}