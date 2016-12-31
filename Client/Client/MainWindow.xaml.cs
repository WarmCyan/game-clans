//*************************************************************
//  File: MainWindow.xaml.cs
//  Date created: 12/8/2016
//  Date edited: 12/30/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: This windows has all the clans and clan stuff on it. Games will open in a new window
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Xml.Linq;

using DWL.Utility;

using Client.GameWindows;

namespace Client
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		// member variables
		private Dictionary<string, Border> m_dClanStackLabels;

		private List<string> m_lGames;
		private List<string> m_lGameIDs;
		private List<string> m_lGameNames;
		private List<string> m_lDisplay;
	
		public MainWindow()
		{
			InitializeComponent();
			_hidden.InitializeWeb();

			// check version with server
			if (!this.CheckVersion()) { this.Close(); return; }

			m_dClanStackLabels = new Dictionary<string, Border>();

			// make sure necessary files exist
			if (!File.Exists(Master.GetBaseDir() + "_clans.dat")) { File.Create(Master.GetBaseDir() + "_clans.dat").Dispose(); }
			if (!File.Exists(Master.GetBaseDir() + "_key.dat"))
			{
				/*Password pPassWindow = new Password();
				pPassWindow.ShowDialog();*/
				LogInOrRegister pLogInOrRegister = new LogInOrRegister();
				pLogInOrRegister.ShowDialog();
			}
			//else { Master.SetKey(File.ReadAllText(Master.GetBaseDir() + "_key.dat")); }
			else { Master.FillKeyEmail(); }

			this.RefreshClanStack();
			
			// set data if a persistant clan thing was saved (persists active clan between application closing/opening)
			if (File.Exists(Master.GetBaseDir() + "_active.dat"))
			{
				string[] aLines = File.ReadAllLines(Master.GetBaseDir() + "_active.dat");
				this.ChangeActiveClan(aLines[0] + "|" + aLines[1]);
			}
		}

		public bool CheckVersion()
		{
			string sResponse = WebCommunications.SendGetRequest(Master.GetBaseURL() + Master.GetServerURL() + "RequiredClientVersion", true);
			XElement pResponse = Master.ReadResponse(sResponse);

			if (pResponse.Element("Text").Value != Master.CLIENT_VERSION)
			{
				// thanks to http://stackoverflow.com/questions/14819426/how-to-create-hyperlink-in-messagebox-show	
				if (MessageBox.Show("You have an outdated client version. Please reinstall the application from http://digitalwarriorlabs.com/games/game_clans\n(" + Master.CLIENT_VERSION + " -> " + pResponse.Element("Text").Value + ")", "Bad Version", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk) == MessageBoxResult.OK)
				{
					System.Diagnostics.Process.Start("http://digitalwarriorlabs.com/games/game_clans");
				}
				return false;
			}
			
			return true;
		}
		
		public void BuildDashboard()
		{
			// clear everything out from the games panel
			stkActiveGames.Children.Clear();
			if (Master.GetActiveClan() == "") { return; }

			// query the server
			string sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/GameClansServer/GameClansServer/ClanServer/ListActiveGames", Master.BuildCommonBody(), true);
			XElement pResponse = Master.ReadResponse(sResponse);

			// handle the games list
			m_lGames = new List<string>();
			m_lGameIDs = new List<string>();
			m_lGameNames = new List<string>();
			m_lDisplay = new List<string>();

			if (pResponse.Element("Data").Element("Games") != null)
			{
				foreach (XElement pGame in pResponse.Element("Data").Element("Games").Elements("Game"))
				{
					m_lGameIDs.Add(pGame.Value);
					m_lGames.Add(pGame.Attribute("GameType").Value);
					m_lGameNames.Add(pGame.Attribute("GameName").Value);

					m_lDisplay.Add(pGame.Attribute("GameName").Value + " (" + pGame.Attribute("GameType").Value + ")");
				}
			}
			for (int i = 0; i < m_lGames.Count; i++) 
			{
				int iIndex = i;
				Label pLabel = new Label();
				pLabel.Padding = new Thickness(10);
				pLabel.Margin = new Thickness(2, 0, 2, 2);
				pLabel.Background = Master.BUTTON_NORMAL;
				pLabel.Foreground = new SolidColorBrush(Colors.White);
				pLabel.Content = m_lDisplay[i];

				pLabel.MouseEnter += delegate { pLabel.Background = Master.BUTTON_HOVER; };
				pLabel.MouseLeave += delegate { pLabel.Background = Master.BUTTON_NORMAL; };
				pLabel.MouseUp += delegate
				{
					if (m_lGames[iIndex] == "Zendo")
					{
						Zendo pWindow = new Zendo(m_lGameIDs[iIndex], m_lGameNames[iIndex]);
						pWindow.Show();
					}
				};

				stkActiveGames.Children.Add(pLabel);
			}

			// fill notifications list
			sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetServerURL() + "GetUnreadNotifications", Master.BuildCommonBody(), true);
			pResponse = Master.ReadResponse(sResponse);

			stkNotifications.Children.Clear();
			XElement pNotificationsXml = pResponse.Element("Data").Element("Notifications");
			if (pNotificationsXml != null)
			{
				foreach (XElement pNotification in pNotificationsXml.Elements("Notification"))
				{
					// get the data from the element
					string sMessage = pNotification.Value;
					string sGameID = pNotification.Attribute("GameID").Value;
					string sGameName = pNotification.Attribute("GameName").Value; 
					
					// make the gui elements
					Border pBorder = new Border();
					pBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(68, 68, 68));
					pBorder.BorderThickness = new Thickness(2, 0, 2, 2);
					pBorder.Padding = new Thickness(10);

					TextBlock pLabel = new TextBlock();
					pLabel.Foreground = new SolidColorBrush(Colors.White);
					pLabel.Text = sGameName + " - " + sMessage;
					pLabel.TextWrapping = TextWrapping.Wrap;

					pLabel.MouseEnter += delegate { pBorder.Background = new SolidColorBrush(Color.FromRgb(100, 100, 100)); };
					pLabel.MouseLeave += delegate { pBorder.Background = new SolidColorBrush(Colors.Transparent); };

					// on click, open up that game window
					pLabel.MouseUp += delegate
					{
						if (sGameID.Contains("Zendo"))
						{ 
							Zendo pWindow = new Zendo(sGameID, sGameName);
							pWindow.Show();
						}
					};

					// add the gui elements to the stack
					pBorder.Child = pLabel;
					stkNotifications.Children.Add(pBorder);
				}
			}

			// build scoreboard
			sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetServerURL() + "GetClanLeaderboard", Master.BuildCommonBody(), true);
			pResponse = Master.ReadResponse(sResponse);

			if (pResponse.Element("Data").Element("Leaderboard") != null)
			{
				XElement pLeaderboard = pResponse.Element("Data").Element("Leaderboard");
				stkScoreBoard.Children.Clear();

				lblUserName.Content = Master.GetActiveUserName();
				lblPlaceScore.Content = pLeaderboard.Attribute("Place").Value + " - " + pLeaderboard.Attribute("Score").Value + " points";
				foreach (XElement pScoreXml in pLeaderboard.Elements("Score"))
				{

					Border pBorder = new Border();
					pBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(68, 68, 68));
					pBorder.BorderThickness = new Thickness(2, 0, 2, 2);
					pBorder.Padding = new Thickness(10);

					// thanks to http://stackoverflow.com/questions/9803710/programmatically-setting-the-width-of-a-grid-column-with-in-wpf
					Grid pGrid = new Grid();
					ColumnDefinition c1 = new ColumnDefinition();
					c1.Width = new GridLength(3, GridUnitType.Star);
					ColumnDefinition c2 = new ColumnDefinition();
					c2.Width = new GridLength(2, GridUnitType.Star);
					ColumnDefinition c3 = new ColumnDefinition();
					c3.Width = new GridLength(2, GridUnitType.Star);

					pGrid.ColumnDefinitions.Add(c1);
					pGrid.ColumnDefinitions.Add(c2);
					pGrid.ColumnDefinitions.Add(c3);

					Label pName = new Label();
					pName.Foreground = new SolidColorBrush(Colors.White);
					pName.Content = pScoreXml.Attribute("User").Value;
					Grid.SetColumn(pName, 0);
					
					Label pPlace = new Label();
					pPlace.Foreground = new SolidColorBrush(Colors.White);
					pPlace.Content = pScoreXml.Attribute("Place").Value;
					Grid.SetColumn(pPlace, 1);
					
					Label pScore = new Label();
					pScore.Foreground = new SolidColorBrush(Colors.White);
					pScore.Content = pScoreXml.Attribute("Score").Value;
					Grid.SetColumn(pScore, 2);

					pGrid.Children.Add(pName);
					pGrid.Children.Add(pPlace);
					pGrid.Children.Add(pScore);

					pBorder.Child = pGrid;
					stkScoreBoard.Children.Add(pBorder);
				}
			}
		}

		public void ChangeActiveClan(string sClanText) // NOTE: clantext includes both clan name and username
		{
			// determine parts of the text
			string sClanName = sClanText.Substring(0, sClanText.IndexOf("|"));
			string sUserName = sClanText.Substring(sClanText.IndexOf("|") + 1);

			// set currently active components
			Master.SetActiveClan(sClanName);
			Master.SetActiveUserName(sUserName);

			// write currently active to _active
			File.WriteAllLines(Master.GetBaseDir() + "_active.dat", new List<string>() { sClanName, sUserName });

			// highlight label in sidebar
			foreach (Border pBorder in stkClanStack.Children)
			{
				if (pBorder.Child is Grid)
				{
					TextBlock pTxtLabel = (TextBlock)((Grid)pBorder.Child).Children[0];
					if (pTxtLabel.Text == sClanName + " - " + sUserName) { pBorder.Background = new SolidColorBrush(Color.FromArgb(255, 69, 186, 255)); }
					else { pBorder.Background = new SolidColorBrush(Color.FromArgb(100, 40, 40, 40)); }
				}
			}

			this.BuildDashboard();
		}

		private void RefreshClanStack()
		{
			// delete all previous stuff
			stkClanStack.Children.Clear();
			m_dClanStackLabels.Clear();

			// read in file and fill new stuff
			string[] aClans = File.ReadAllLines(Master.GetBaseDir() + "_clans.dat");
			for (int i = 0; i < aClans.Length; i++) { this.AddClanToStack(aClans[i]); }
		}
		private void AddClanToStack(string sClanText) // NOTE: clantext includes both clan name and username
		{

			//string sClanName = sClanText.Substring(0, sClanText.IndexOf("|"));
			string sClanName = sClanText.Replace("|", " - ");

			// border container
			Border pBorder = new Border();
			pBorder.BorderThickness = new Thickness(0, 0, 0, 1);
			pBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(100, 70, 70, 70));
			pBorder.Background = new SolidColorBrush(Color.FromArgb(100, 40, 40, 40));
			pBorder.MouseEnter += delegate { if (sClanName != Master.GetActiveClan() + " - " + Master.GetActiveUserName()) { pBorder.Background = new SolidColorBrush(Color.FromArgb(100, 60, 60, 60)); } };
			pBorder.MouseLeave += delegate { if (sClanName != Master.GetActiveClan() + " - " + Master.GetActiveUserName()) { pBorder.Background = new SolidColorBrush(Color.FromArgb(100, 40, 40, 40)); } };

			Grid pGrid = new Grid();

			// query label
			TextBlock pTxtLabel = new TextBlock();
			pTxtLabel.Text = sClanName;
			pTxtLabel.Foreground = new SolidColorBrush(Colors.White);
			pTxtLabel.Padding = new Thickness(10);
			pTxtLabel.HorizontalAlignment = HorizontalAlignment.Stretch;
			pTxtLabel.MouseUp += delegate { this.ChangeActiveClan(sClanText); }; // NOTE: this is here because if on border, and user clicks on exit, it registers for both exit AND border!

			// add all the things!
			pGrid.Children.Add(pTxtLabel);
			pBorder.Child = pGrid;
			stkClanStack.Children.Add(pBorder);
			m_dClanStackLabels.Add(sClanText, pBorder);
		}

		private void btnJoinClan_MouseEnter(object sender, MouseEventArgs e) { btnJoinClan.Background = Master.BUTTON_HOVER; }
		private void btnJoinClan_MouseLeave(object sender, MouseEventArgs e) { btnJoinClan.Background = Master.BUTTON_NORMAL; }
		
		private void btnCreateClan_MouseEnter(object sender, MouseEventArgs e) { btnCreateClan.Background = Master.BUTTON_HOVER; }
		private void btnCreateClan_MouseLeave(object sender, MouseEventArgs e) { btnCreateClan.Background = Master.BUTTON_NORMAL; }
		
		private void btnCreateGame_MouseLeave(object sender, MouseEventArgs e) { btnCreateGame.Background = Master.BUTTON_NORMAL; }
		private void btnCreateGame_MouseEnter(object sender, MouseEventArgs e) { btnCreateGame.Background = Master.BUTTON_HOVER; }
		
		private void btnMarkNotificationsRead_MouseLeave(object sender, MouseEventArgs e) { btnMarkNotificationsRead.Background = Master.BUTTON_NORMAL; }
		private void btnMarkNotificationsRead_MouseEnter(object sender, MouseEventArgs e) { btnMarkNotificationsRead.Background = Master.BUTTON_HOVER; }
		
		private void btnRefresh_MouseLeave(object sender, MouseEventArgs e) { btnRefresh.Background = Master.BUTTON_NORMAL; }
		private void btnRefresh_MouseEnter(object sender, MouseEventArgs e) { btnRefresh.Background = Master.BUTTON_HOVER; }
		
		private void btnSettings_MouseLeave(object sender, MouseEventArgs e) { btnSettings.Background = Master.BUTTON_NORMAL; }
		private void btnSettings_MouseEnter(object sender, MouseEventArgs e) { btnSettings.Background = Master.BUTTON_HOVER; }
		
		private void btnJoinClan_MouseUp(object sender, MouseButtonEventArgs e)
		{
			JoinClan pJoinClan = new JoinClan();
			pJoinClan.ShowDialog();
			this.RefreshClanStack();
		}
		
		private void btnCreateClan_MouseUp(object sender, MouseButtonEventArgs e)
		{
			CreateClan pCreateClan = new CreateClan();
			pCreateClan.ShowDialog();
		}

		private void btnMarkNotificationsRead_MouseUp(object sender, MouseButtonEventArgs e)
		{
			WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetServerURL() + "MarkUnreadNotificationsRead", Master.BuildCommonBody(), true);
			this.BuildDashboard();
		}

		private void btnCreateGame_MouseUp(object sender, MouseButtonEventArgs e)
		{
			CreateGame pCreateGame = new CreateGame();
			pCreateGame.ShowDialog();
		}

		private void btnRefresh_MouseUp(object sender, MouseButtonEventArgs e) { this.BuildDashboard(); }

		private void btnSettings_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Settings pSettings = new Settings();
			pSettings.ShowDialog();
		}
	}
}
