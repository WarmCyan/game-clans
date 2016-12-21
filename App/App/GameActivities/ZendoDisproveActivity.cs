//*************************************************************
//  File: ZendoDisproveActivity.cs
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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App
{
	[Activity(Label = "Disprove Guess")]
	public class ZendoDisproveActivity : Activity
	{
		// member variables
		FlowLayout m_pKoanDisplay;
		EditText m_pKoanTextEditor;
	
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.SetContentView(Resource.Layout.Game_ZendoDisprove);

			// get data
			string sRule = this.Intent.GetStringExtra("Rule");
			string sGuess = this.Intent.GetStringExtra("Guess");

			// fill the elements
			TextView pRule = FindViewById<TextView>(Resource.Id.txtRule);
			pRule.Text = "Your rule - '" + sRule + "'";
			TextView pRuleGuess = FindViewById<TextView>(Resource.Id.txtRuleGuess);
			pRuleGuess.Text = "Guessed rule - '" + sGuess + "'";

			// get other important gui elements
			m_pKoanDisplay = FindViewById<FlowLayout>(Resource.Id.flowKoan);
			m_pKoanTextEditor = FindViewById<EditText>(Resource.Id.txtKoanBuilder);

			m_pKoanTextEditor.TextChanged += delegate { this.FillKoan(); };

			FillKoan();
		}

		private void FillKoan()
		{
			m_pKoanDisplay.RemoveAllViews();
			m_pKoanTextEditor.SetBackgroundColor(Android.Graphics.Color.DarkGray);

			string sKoan = m_pKoanTextEditor.Text;
			Master.FillKoanDisplay(this, m_pKoanDisplay, sKoan.ToUpper());

			// get list of pieces and insert image into layout for each one
			/*List<string> lPieces = Master.GetPieceParts(sKoan);
			foreach (string sPiece in lPieces)
			{
				int iRes = Master.GetPieceImage(sPiece);
				if (iRes == 0)
				{
					m_pKoanTextEditor.SetBackgroundColor(Android.Graphics.Color.Red);
					continue;
				}

				ImageView pView = new ImageView(this);
				pView.SetImageResource(iRes);
				m_pKoanDisplay.AddView(pView);
			}*/
		}
	}
}