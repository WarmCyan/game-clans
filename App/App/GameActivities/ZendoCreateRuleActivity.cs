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
		// member variables
		private EditText m_pGoodKoan;
		private EditText m_pBadKoan;

		private FlowLayout m_pImageRowGood;
		private LinearLayout m_pImageRowBad;
		
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.SetContentView(Resource.Layout.Game_ZendoCreateRule);

			// Create your application here

			EditText pRule = FindViewById<EditText>(Resource.Id.txtRule);
			
			m_pImageRowGood = FindViewById<FlowLayout>(Resource.Id.lstKoanImagesGood);
			m_pGoodKoan = FindViewById<EditText>(Resource.Id.txtGoodKoan);
			
			/*ImageView pViewGood1 = new ImageView(this);
			pViewGood1.SetImageResource(Resource.Drawable.T);
			pImageRowGood.AddView(pViewGood1);*/

			m_pImageRowBad = FindViewById<LinearLayout>(Resource.Id.lstKoanImagesBad);
			m_pBadKoan = FindViewById<EditText>(Resource.Id.txtBadKoan);
			
			/*ImageView pViewBad1 = new ImageView(this);
			pViewBad1.SetImageResource(Resource.Drawable.F);
			pImageRowBad.AddView(pViewBad1);*/
			
			m_pGoodKoan.TextChanged += delegate { this.FillGoodKoan(); };

			this.FillGoodKoan();
		}

		private void FillGoodKoan()
		{
			// clear current koan
			m_pImageRowGood.RemoveAllViews();
			
			string sKoan = "T" + m_pGoodKoan.Text;

			// TODO: display if there's an error

			// get the list of pieces and for each one insert the image into the layout
			List<string> lPieces = Master.GetPieceParts(sKoan);
			foreach (string sPiece in lPieces)
			{
				int iRes = Master.GetPieceImage(sPiece);
				if (iRes == 0) { continue; }
				
				ImageView pView = new ImageView(this);
				pView.SetImageResource(iRes);
				m_pImageRowGood.AddView(pView);
			}
		}
	}
}