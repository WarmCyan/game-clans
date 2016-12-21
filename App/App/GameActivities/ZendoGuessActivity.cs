//*************************************************************
//  File: ZendoGuessActivity.cs
//  Date created: 12/21/2016
//  Date edited: 12/21/2016
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
	[Activity(Label = "Guess Rule")]
	public class ZendoGuessActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.SetContentView(Resource.Layout.Game_ZendoGuess);

			// get extras
			string sKoans = this.Intent.GetStringExtra("Koans");
			XElement pKoansXml = XElement.Parse(sKoans);

			// fill koans box
			LinearLayout pKoansLayout = FindViewById<LinearLayout>(Resource.Id.lstKoans);
			pKoansLayout.RemoveAllViews();
			List<XElement> pKoansXmlChildren = pKoansXml.Elements("Koan").ToList();
			for (int i = pKoansXmlChildren.Count - 1; i >= 0; i--)
			{
				XElement pKoan = pKoansXmlChildren[i];

				View pView = LayoutInflater.From(this).Inflate(Resource.Layout.Game_ZendoKoanRow, pKoansLayout, false);

				FlowLayout pFlow = pView.FindViewById<FlowLayout>(Resource.Id.lstKoanImages);

				string sKoanText = pKoan.Value;
				Master.FillKoanDisplay(this, pFlow, sKoanText);
				
				pKoansLayout.AddView(pView);
			}

			// submit button
			Button pSubmit = FindViewById<Button>(Resource.Id.btnGuess);
			pSubmit.Click += delegate
			{
				EditText pGuessText = FindViewById<EditText>(Resource.Id.txtGuess);
				Intent pIntent = new Intent(this, typeof(ZendoActivity));
				pIntent.PutExtra("Type", "guess");
				pIntent.PutExtra("Guess", pGuessText.Text);
				this.SetResult(Result.Ok, pIntent);
				this.Finish();
			};
		}
	}
}