//*************************************************************
//  File: ZendoCreateRuleActivity.cs
//  Date created: 12/16/2016
//  Date edited: 12/20/2016
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
		private FlowLayout m_pImageRowBad;
		
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.SetContentView(Resource.Layout.Game_ZendoCreateRule);

			// Create your application here

			EditText pRule = FindViewById<EditText>(Resource.Id.txtRule);
			Button pSubmit = FindViewById<Button>(Resource.Id.btnSubmitInitial);
			
			// get the good koan elements
			m_pImageRowGood = FindViewById<FlowLayout>(Resource.Id.lstKoanImagesGood);
			m_pGoodKoan = FindViewById<EditText>(Resource.Id.txtGoodKoan);
			
			// get the bad koan elements
			m_pImageRowBad = FindViewById<FlowLayout>(Resource.Id.lstKoanImagesBad);
			m_pBadKoan = FindViewById<EditText>(Resource.Id.txtBadKoan);
			
			// add element event handlers
			m_pGoodKoan.TextChanged += delegate { this.FillGoodKoan(); };
			m_pBadKoan.TextChanged += delegate { this.FillBadKoan(); };

			pSubmit.Click += delegate
			{
				if (m_pGoodKoan.Text == "" || m_pBadKoan.Text == "" || pRule.Text == "")
				{
					var pBuilder = new AlertDialog.Builder(this);
					pBuilder.SetMessage("No fields can be blank.");
					pBuilder.SetPositiveButton("Ok", (e, s) => { return; });
					pBuilder.Show();
				}
				else
				{
					Intent pIntent = new Intent(this, typeof(ZendoActivity));
					pIntent.PutExtra("Type", "initial");
					pIntent.PutExtra("Rule", pRule.Text);
					pIntent.PutExtra("CorrectKoan", "T" + m_pGoodKoan.Text.ToUpper());
					pIntent.PutExtra("IncorrectKoan", "F" + m_pBadKoan.Text.ToUpper());
					SetResult(Result.Ok, pIntent);
					this.Finish();
				}
			};
			

			// put the initial truth/false stones
			this.FillGoodKoan();
			this.FillBadKoan();
		}

		private void FillGoodKoan()
		{
			// clear current koan
			m_pImageRowGood.RemoveAllViews();
			m_pGoodKoan.SetBackgroundColor(Android.Graphics.Color.DarkGray);
			
			string sKoan = "T" + m_pGoodKoan.Text;

			// get the list of pieces and for each one insert the image into the layout
			List<string> lPieces = Master.GetPieceParts(sKoan);
			foreach (string sPiece in lPieces)
			{
				int iRes = Master.GetPieceImage(sPiece);
				if (iRes == 0) 
				{ 
					m_pGoodKoan.SetBackgroundColor(Android.Graphics.Color.Red);
					continue; 
				}
				
				ImageView pView = new ImageView(this);
				pView.SetImageResource(iRes);
				m_pImageRowGood.AddView(pView);
			}
		}

		private void FillBadKoan()
		{
			// clear current koan
			m_pImageRowBad.RemoveAllViews();
			m_pBadKoan.SetBackgroundColor(Android.Graphics.Color.DarkGray);

			string sKoan = "F" + m_pBadKoan.Text;

			// get the list of pieces and for each one insert the image into the layout
			List<string> lPieces = Master.GetPieceParts(sKoan);
			foreach (string sPiece in lPieces)
			{
				int iRes = Master.GetPieceImage(sPiece);
				if (iRes == 0)
				{
					m_pBadKoan.SetBackgroundColor(Android.Graphics.Color.Red);
					continue;
				}

				ImageView pView = new ImageView(this);
				pView.SetImageResource(iRes);
				m_pImageRowBad.AddView(pView);
			}
		}
	}
}