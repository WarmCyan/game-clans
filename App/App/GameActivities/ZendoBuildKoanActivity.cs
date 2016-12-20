//*************************************************************
//  File: ZendoBuildKoanActivity.cs
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
	[Activity(Label = "Build Koan")]
	public class ZendoBuildKoanActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Game_ZendoBuildKoan);

			// Create your application here

			string sKoans = this.Intent.GetStringExtra("Koans");
			XElement pKoansXml = XElement.Parse(sKoans); // TODO: error handling?


			// fill koans box
			/*LinearLayout pKoansLayout = FindViewById<LinearLayout>(Resource.Id.lstKoans);
			pKoansLayout.RemoveAllViews();
			List<XElement> pKoansXmlChildren = pKoansXml.Elements("Koan").ToList();
			for (int i = pKoansXmlChildren.Count - 1; i >= 0; i--)
			{
				XElement pKoan = pKoansXmlChildren[i];

				View pView = LayoutInflater.From(this).Inflate(Resource.Layout.Game_ZendoKoanRow, pKoansLayout, false);

				FlowLayout pFlow = pView.FindViewById<FlowLayout>(Resource.Id.lstKoanImages);

				string sKoanText = pKoan.Value;
				List<string> lPieces = Master.GetPieceParts(sKoanText);
				foreach (string sPiece in lPieces)
				{
					int iRes = Master.GetPieceImage(sPiece);
					if (iRes == 0) { continue; } // TODO: error handling?Jk;lw

					ImageView pKoanView = new ImageView(this);
					pKoanView.SetImageResource(iRes);
					pFlow.AddView(pKoanView);
				}

				pKoansLayout.AddView(pView);
			}*/
		}
	}
}