//*************************************************************
//  File: Zendo.xaml.cs
//  Date created: 12/15/2016
//  Date edited: 12/23/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: Zendo game window
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

using DWL.Utility;

namespace Client.GameWindows
{
	/// <summary>
	/// Interaction logic for Zendo.xaml
	/// </summary>
	public partial class Zendo : Window
	{
		// member variables
		private string m_sGameID;
		private string m_sGameName;

		private string m_sClanName;
		private string m_sUserName;

		// used for restoring master state
		private string m_sPrevClan;
		private string m_sPrevUser; 

		public Zendo(string sGameID, string sGameName)
		{
			InitializeComponent();

			// save these in case you're playing multiple games simultaneously (multiple game windows open)
			m_sClanName = Master.GetActiveClan();
			m_sUserName = Master.GetActiveUserName();

			Master.AddImageToWrapPanel(pnlNotButton, "F");
			Master.AddImageToWrapPanel(pnlYesbutton, "T");

			m_sGameID = sGameID;
			m_sGameName = sGameName;

			this.Title = Master.GetActiveClan() + " - " + m_sGameName;

			//pnlStatusKoan.
			/*Master.AddImageToWrapPanel(pnlStatusKoan, "BD");
			Master.AddImageToWrapPanel(pnlStatusKoan, "BD");
			Master.AddImageToWrapPanel(pnlStatusKoan, "BD");
			Master.AddImageToWrapPanel(pnlStatusKoan, "BD");
			Master.AddImageToWrapPanel(pnlStatusKoan, "BD");
			Master.AddImageToWrapPanel(pnlStatusKoan, "BD");
			Master.AddImageToWrapPanel(pnlStatusKoan, "BD");
			Master.AddImageToWrapPanel(pnlStatusKoan, "BD");
			Master.AddImageToWrapPanel(pnlStatusKoan, "BD");
			Master.AddImageToWrapPanel(pnlStatusKoan, "BD");
			Master.AddImageToWrapPanel(pnlStatusKoan, "BD");
			Master.AddImageToWrapPanel(pnlStatusKoan, "BD");
			Master.AddImageToWrapPanel(pnlStatusKoan, "BD");
			Master.AddImageToWrapPanel(pnlStatusKoan, "BD");
			Master.AddImageToWrapPanel(pnlStatusKoan, "BD");
			Master.AddImageToWrapPanel(pnlStatusKoan, "BD");
			Master.AddImageToWrapPanel(pnlStatusKoan, "BD");*/

			this.GetUserBoard();
		}

		private void TempSetMaster()
		{
			m_sPrevClan = Master.GetActiveClan();
			m_sPrevUser = Master.GetActiveUserName();
			Master.SetActiveClan(m_sClanName);
			Master.SetActiveUserName(m_sUserName);
		}
		private void TempResetMaster()
		{
			Master.SetActiveClan(m_sPrevClan);
			Master.SetActiveUserName(m_sPrevUser);
		}

		// stuff
		private void DisplayGuessPanel()
		{
			// stuff
		}

		private void GetUserBoard()
		{
			this.TempSetMaster();

			string sBody = Master.BuildCommonBody(Master.BuildGameIDBodyPart(m_sGameID));
			string sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetGameURL("Zendo") + "GetUserBoard", sBody, true);
			
			this.TempResetMaster();

			XElement pResponse = Master.ReadResponse(sResponse);
			if (pResponse.Attribute("Type").Value == "Error") { MessageBox.Show(pResponse.Element("Text").Value, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
			else
			{
				this.CollapseAll();
			
				// get the specific parts
				XElement pStatusXml = pResponse.Element("Data").Element("Status");
				XElement pActionXml = pResponse.Element("Data").Element("Action");
				XElement pNumGuessesXml = pResponse.Element("Data").Element("NumGuesses");
				XElement pMasterXml = pResponse.Element("Data").Element("Master");
				XElement pPlayersXml = pResponse.Element("Data").Element("Players");
				XElement pEventsXml = pResponse.Element("Data").Element("Events");
				XElement pKoansXml = pResponse.Element("Data").Element("Koans");

				// fill status
				lblStatus.Text = pStatusXml.Element("Text").Value;
				if (pStatusXml.Element("Data").Value != "")
				{
					string sKoan = pStatusXml.Element("Data").Element("Koan").Value;
					Master.FillKoanDisplay(pnlStatusKoan, sKoan);
				}
				else { pnlStatusKoan.Children.Clear(); }


				// display panels based on action
				string sAction = pActionXml.Value;
				if (sAction == "join")
				{
					pnlJoin.Visibility = Visibility.Visible;
				}
				else if (sAction == "initial")
				{
					pnlRuleCreator.Visibility = Visibility.Visible;
				}
				else if (sAction == "build")
				{
					pnlSingleKoanCreator.Visibility = Visibility.Visible;
					grdSingleKoanSubmissionButtons.Visibility = Visibility.Visible;
					// display actual guess stuff
					pnlGiveUp.Visibility = Visibility.Visible;
				}
				else if (sAction == "judge")
				{
					pnlRuleDisplay.Visibility = Visibility.Visible;
					pnlBuddhaNature.Visibility = Visibility.Visible;
					lblRule.Text = "Your rule - '" + pMasterXml.Attribute("Rule").Value + "'";
				}
				else if (sAction == "predict")
				{
					pnlBuddhaNature.Visibility = Visibility.Visible;
					pnlGiveUp.Visibility = Visibility.Visible;
				}
				else if (sAction == "disprove")
				{
					pnlRuleDisplay.Visibility = Visibility.Visible;
					pnlBuddhaNature.Visibility = Visibility.Visible;
					pnlGrantEnlightenment.Visibility = Visibility.Visible;
					lblRule.Text = "Your rule - '" + pMasterXml.Attribute("Rule").Value + "'";
					lblGuessRule.Text = "Guess - '" + pStatusXml.Attribute("Guess").Value + "'";
				}
				else if (sAction == "final")
				{
					pnlStartGame.Visibility = Visibility.Visible;
				}
				else if (sAction == "waiting")
				{
					pnlWaiting.Visibility = Visibility.Visible;
				}
				
				// panels visible no matter what
				pnlRules.Visibility = Visibility.Visible;

				// set number of guesses
				int iNumGuesses = Convert.ToInt32(pNumGuessesXml.Value);
				if (sAction != "initial" && sAction != "disprove" && sAction != "judge" && sAction != "final" && sAction != "join")
				{
					pnlGuess.Visibility = Visibility.Visible;
					lblGuessingStoneCount.Content = "You have " + iNumGuesses + " guessing stones";
					if (iNumGuesses == 1) { lblGuessingStoneCount.Content = "You have 1 guessing stone"; } // proper grammar is best grammar
					if (sAction == "build" && iNumGuesses > 0)
					{
						pnlActualGuess.Visibility = Visibility.Visible;
					}
				}

				// set master label
				string sMaster = pMasterXml.Value;
				lblMaster.Content = "Master - " + sMaster;

				// fill players box
				stkPlayers.Children.Clear();
				foreach (string sPlayer in pPlayersXml.Elements("Player"))
				{
					Border pBorder = new Border();
					pBorder.BorderThickness = new Thickness(2, 0, 2, 2);
					pBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(68, 68, 68));

					Label pLabel = new Label();
					pLabel.Foreground = new SolidColorBrush(Colors.White);
					pLabel.Content = sPlayer;

					pBorder.Child = pLabel;

					stkPlayers.Children.Add(pBorder);

					//StackPanel pPanel = new StackPanel();
					
					/*Rectangle pRectangle = new Rectangle();
					pRectangle.*/
				}
			}
		}

		private void CollapseAll()
		{
			pnlWaiting.Visibility = Visibility.Collapsed;
			pnlJoin.Visibility = Visibility.Collapsed;
			pnlRuleDisplay.Visibility = Visibility.Collapsed;
			pnlSingleKoanCreator.Visibility = Visibility.Collapsed;
			grdSingleKoanSubmissionButtons.Visibility = Visibility.Collapsed;
			pnlRuleCreator.Visibility = Visibility.Collapsed;
			pnlBuddhaNature.Visibility = Visibility.Collapsed;
			pnlGuess.Visibility = Visibility.Collapsed;
			pnlActualGuess.Visibility = Visibility.Collapsed;
			pnlGrantEnlightenment.Visibility = Visibility.Collapsed;
			pnlRules.Visibility = Visibility.Collapsed;
			pnlGiveUp.Visibility = Visibility.Collapsed;
			pnlStartGame.Visibility = Visibility.Collapsed;
		}

		private void btnJoinGame_MouseLeave(object sender, MouseEventArgs e) { btnJoinGame.Background = Master.BUTTON_NORMAL; }
		private void btnJoinGame_MouseEnter(object sender, MouseEventArgs e) { btnJoinGame.Background = Master.BUTTON_HOVER; }
		
		private void btnCallMaster_MouseLeave(object sender, MouseEventArgs e) { btnCallMaster.Background = Master.BUTTON_NORMAL; }
		private void btnCallMaster_MouseEnter(object sender, MouseEventArgs e) { btnCallMaster.Background = Master.BUTTON_HOVER; }
		
		private void btnCallMondo_MouseLeave(object sender, MouseEventArgs e) { btnCallMondo.Background = Master.BUTTON_NORMAL; }
		private void btnCallMondo_MouseEnter(object sender, MouseEventArgs e) { btnCallMondo.Background = Master.BUTTON_HOVER; }
		
		private void btnExampleRules_MouseLeave(object sender, MouseEventArgs e) { btnExampleRules.Background = Master.BUTTON_NORMAL; }
		private void btnExampleRules_MouseEnter(object sender, MouseEventArgs e) { btnExampleRules.Background = Master.BUTTON_HOVER; }
		
		private void btnSubmitRule_MouseLeave(object sender, MouseEventArgs e) { btnSubmitRule.Background = Master.BUTTON_NORMAL; }
		private void btnSubmitRule_MouseEnter(object sender, MouseEventArgs e) { btnSubmitRule.Background = Master.BUTTON_HOVER;  }
		
		private void btnHasBuddhaNature_MouseLeave(object sender, MouseEventArgs e) { brdrHasBuddhaNature.Background = Master.BUTTON_NORMAL; }
		private void btnHasBuddhaNature_MouseEnter(object sender, MouseEventArgs e) {  brdrHasBuddhaNature.Background = Master.BUTTON_HOVER; }
		
		private void btnHasNotBuddhaNature_MouseLeave(object sender, MouseEventArgs e) { brdrHasNotBuddhaNature.Background = Master.BUTTON_NORMAL; }
		private void btnHasNotBuddhaNature_MouseEnter(object sender, MouseEventArgs e) { brdrHasNotBuddhaNature.Background = Master.BUTTON_HOVER; }
		
		private void btnSubmitGuess_MouseLeave(object sender, MouseEventArgs e) { btnSubmitGuess.Background = Master.BUTTON_NORMAL; }
		private void btnSubmitGuess_MouseEnter(object sender, MouseEventArgs e) { btnSubmitGuess.Background = Master.BUTTON_HOVER; }
		
		private void btnGrantEnlightenment_MouseLeave(object sender, MouseEventArgs e) { btnGrantEnlightenment.Background = Master.BUTTON_NORMAL; }
		private void btnGrantEnlightenment_MouseEnter(object sender, MouseEventArgs e) { btnGrantEnlightenment.Background = Master.BUTTON_HOVER; }
		
		private void btnRules_MouseLeave(object sender, MouseEventArgs e) { btnRules.Background = Master.BUTTON_NORMAL; }
		private void btnRules_MouseEnter(object sender, MouseEventArgs e) { btnRules.Background = Master.BUTTON_HOVER; }
		
		private void btnGiveUp_MouseLeave(object sender, MouseEventArgs e) { btnGiveUp.Background = Master.BUTTON_NORMAL; }
		private void btnGiveUp_MouseEnter(object sender, MouseEventArgs e) { btnGiveUp.Background = Master.BUTTON_HOVER; }
		
		private void btnRefresh_MouseLeave(object sender, MouseEventArgs e) { btnRefresh.Background = Master.BUTTON_NORMAL; }
		private void btnRefresh_MouseEnter(object sender, MouseEventArgs e) { btnRefresh.Background = Master.BUTTON_HOVER; }

		// button clicks
		
		private void btnJoinGame_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.TempSetMaster();
			string sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetGameURL("Zendo") + "JoinGame", Master.BuildCommonBody(Master.BuildGameIDBodyPart(m_sGameID)), true);
			XElement pResponse = Master.ReadResponse(sResponse);
			this.TempResetMaster();
			if (pResponse.Attribute("Type").Value == "Error") { MessageBox.Show(pResponse.Element("Text").Value, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
			else { this.GetUserBoard();	}
		}

		private void btnCallMaster_MouseUp(object sender, MouseButtonEventArgs e)
		{

		}

		private void btnCallMondo_MouseUp(object sender, MouseButtonEventArgs e)
		{

		}

		private void btnExampleRules_MouseUp(object sender, MouseButtonEventArgs e)
		{

		}
		
		private void btnSubmitRule_MouseUp(object sender, MouseButtonEventArgs e)
		{

		}
		
		private void btnHasBuddhaNature_MouseUp(object sender, MouseButtonEventArgs e)
		{

		}

		private void btnHasNotBuddhaNature_MouseUp(object sender, MouseButtonEventArgs e)
		{

		}
		
		private void btnSubmitGuess_MouseUp(object sender, MouseButtonEventArgs e)
		{

		}

		private void btnGrantEnlightenment_MouseUp(object sender, MouseButtonEventArgs e)
		{

		}

		private void btnRules_MouseUp(object sender, MouseButtonEventArgs e)
		{

		}

		private void btnGiveUp_MouseUp(object sender, MouseButtonEventArgs e)
		{

		}
		
		private void btnRefresh_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.GetUserBoard();
		}
		
		// koan editors
		
		private void txtKoanEditor_KeyUp(object sender, KeyEventArgs e)
		{

		}

		private void txtKoanRule1Editor_KeyUp(object sender, KeyEventArgs e)
		{

		}

		private void txtKoanRule2Editor_KeyUp(object sender, KeyEventArgs e)
		{

		}


	}
}
