//*************************************************************
//  File: ZendoActivity.cs
//  Date created: 12/13/2016
//  Date edited: 12/16/2016
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
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Android.Support.V4.Widget;

using DWL.Utility;

namespace App
{
	[Activity(Label = "Zendo Game")]
	public class ZendoActivity : BaseActivity
	{

		// member variables
		private string m_sGameID;

	
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.SetContentView(Resource.Layout.Game_Zendo);

			// Create your application here

			base.CreateDrawer();

			ScrollView pMainScroll = FindViewById<ScrollView>(Resource.Id.scrlZendoMain);
			pMainScroll.SetOnScrollChangeListener(this);

			SwipeRefreshLayout pRefresher = FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
			pRefresher.Refresh += delegate
			{
				// do things here!
				this.GetUserBoard();
				pRefresher.Refreshing = false;
			};

			m_sGameID = this.Intent.Action;
			this.GetUserBoard();
		}

		private void GetUserBoard()
		{
			// send the request to the server
			string sBody = Master.BuildCommonBody(Master.BuildGameIDBodyPart(m_sGameID));
			string sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetGameURL("Zendo") + "GetUserBoard", sBody, true);

			XElement pResponse = Master.ReadResponse(sResponse);
			if (pResponse.Attribute("Type").Value == "Error") { throw new Exception(pResponse.ToString()); }

			// get the specific parts
			XElement pStatusXml = pResponse.Element("Data").Element("Status");
			XElement pActionXml = pResponse.Element("Data").Element("Action");
			XElement pNumGuessesXml = pResponse.Element("Data").Element("NumGuesses");
			XElement pMasterXml = pResponse.Element("Data").Element("Master");
			XElement pPlayersXml = pResponse.Element("Data").Element("Players");
			XElement pEventsXml = pResponse.Element("Data").Element("Events");
			XElement pKoansXml = pResponse.Element("Data").Element("Koans");

			TextView pGameStatusText = FindViewById<TextView>(Resource.Id.txtGameStatus);
			pGameStatusText.Text = pStatusXml.Element("Text").Value;

			// TODO: handle data tag in status

			// set the action button accordingly
			Button pActionButton = FindViewById<Button>(Resource.Id.btnAction);
			pActionButton.Enabled = true;
			string sAction = pActionXml.Value;
			if (sAction == "join")
			{
				pActionButton.Text = "Join Game";
				pActionButton.Click += delegate
				{
					string sResponse2 = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetGameURL("Zendo") + "JoinGame", sBody, true);
					XElement pResponse2 = Master.ReadResponse(sResponse2);

					var pBuilder = new AlertDialog.Builder(this);
					pBuilder.SetMessage(pResponse2.Element("Text").Value);
					pBuilder.SetPositiveButton("Ok", (e, s) => { this.GetUserBoard(); });
					pBuilder.Show();
				};
			}
			else if (sAction == "initial")
			{
				pActionButton.Text = "Create Initial Koans";

				Intent pIntent = new Intent(this, (new ZendoCreateRuleActivity()).Class);
				this.StartActivityForResult(pIntent, 0);
			}
			else if (sAction == "open")
			{
				pActionButton.Text = "Build Koan";

				Intent pIntent = new Intent(this, (new ZendoBuildKoanActivity()).Class);
				this.StartActivityForResult(pIntent, 0);
			}
			else if (sAction == "judge")
			{
				pActionButton.Text = "Analyze Koan";

				Intent pIntent = new Intent(this, (new ZendoJudgeKoanActivity()).Class);
				this.StartActivityForResult(pIntent, 0);
			}
			else if (sAction == "predict")
			{
				pActionButton.Text = "Predict Master's Analysis";

				Intent pIntent = new Intent(this, (new ZendoPredictActivity()).Class);
				this.StartActivityForResult(pIntent, 0);
			}
			else if (sAction == "disprove")
			{
				pActionButton.Text = "Disprove Guess";
			}
			else if (sAction == "final")
			{
				pActionButton.Text = "Game Over!";
				pActionButton.Enabled = false;
			}
			else if (sAction == "waiting")
			{
				pActionButton.Text = "Waiting...";
				pActionButton.Enabled = false;
			}

			// set number of guesses
			int iNumGuesses = Convert.ToInt32(pNumGuessesXml.Value);
			Button pGuessButton = FindViewById<Button>(Resource.Id.btnGuess);
			pGuessButton.Text = "Guess (" + iNumGuesses.ToString() + " guess tokens)";
			if (iNumGuesses > 0) { pGuessButton.Enabled = true; }
			else { pGuessButton.Enabled = false; }

			// set master label
			string sMaster = pMasterXml.Value;
			TextView pMaster = FindViewById<TextView>(Resource.Id.txtMaster);
			pMaster.Text = "Master - " + sMaster;

			// fill players box
			LinearLayout pPlayersLayout = FindViewById<LinearLayout>(Resource.Id.lstPlayers);
			pPlayersLayout.RemoveAllViews();
			foreach (string sPlayer in pPlayersXml.Elements("Player"))
			{
				View pDataRow = LayoutInflater.From(this).Inflate(Resource.Layout.DataRow, pPlayersLayout, false);

				TextView pDataText = pDataRow.FindViewById<TextView>(Resource.Id.txtText);
				pDataText.Text = sPlayer;

				pPlayersLayout.AddView(pDataRow);
			}
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (resultCode == Result.Ok)
			{
				// TODO: HANDLE EXTRA HERE
				string sType = data.GetStringExtra("Type");

				if (sType == "initial")
				{
					string sRule = data.GetStringExtra("Rule");
					string sInitialCorrectKoan = data.GetStringExtra("CorrectKoan");
					string sInitialIncorrectKoan = data.GetStringExtra("IncorrectKoan");

					string sBody = Master.BuildCommonBody(Master.BuildGameIDBodyPart(m_sGameID) + "<param name='sRule'>" + sRule + "</param><param name='sBuddhaNatureKoan'>" + sInitialCorrectKoan + "</param><param name='sNonBuddhaNatureKoan'>" + sInitialIncorrectKoan + "</param>");
					string sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetGameURL("Zendo") + "SubmitInitialKoans", sBody, true);
					if (sResponse != "") { XElement pResponse = Master.ReadResponse(sResponse); } // what is this on success??

					this.GetUserBoard();
				}
				else if (sType == "build")
				{
					string sKoan = data.GetStringExtra("Koan");
					bool bMondo = data.GetBooleanExtra("Mondo", false);

					string sBody = Master.BuildCommonBody(Master.BuildGameIDBodyPart(m_sGameID) + "<param name='sKoan'>" + sKoan + "</param>");

					string sResponse = "";
					if (!bMondo) { sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetGameURL("Zendo") + "SubmitKoan", sBody, true); }
					if (sResponse != "") { XElement pResponse = Master.ReadResponse(sResponse); }

					this.GetUserBoard();
				}
				else if (sType == "analysis")
				{
					bool bHasBuddhaNature = data.GetBooleanExtra("HasBuddhaNature", false);

					string sBody = Master.BuildCommonBody(Master.BuildGameIDBodyPart(m_sGameID) + "<param name='bHasBuddhaNature'>" + bHasBuddhaNature + "</param>");
					string sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetGameURL("Zendo") + "SubmitPendingKoanAnalysis", sBody, true);
					if (sResponse != "") { XElement pResponse = Master.ReadResponse(sResponse); }

					this.GetUserBoard();
				}
				else if (sType == "predict")
				{
					bool bPrediction = data.GetBooleanExtra("Prediction", false);

					string sBody = Master.BuildCommonBody(Master.BuildGameIDBodyPart(m_sGameID) + "<param name='bPrediction'>" + bPrediction + "</param>");
					string sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetGameURL("Zendo") + "SubmitMondoPrediction", sBody, true);
					if (sResponse != "") { XElement pResponse = Master.ReadResponse(sResponse); }

					this.GetUserBoard();
				}
			}
		}
	}
}