﻿//*************************************************************
//  File: MainWindow.xaml.cs
//  Date created: 12/8/2016
//  Date edited: 12/15/2016
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
		private Dictionary<string, string> m_dClanStack;
		private Dictionary<string, Border> m_dClanStackLabels;

		private List<string> m_lGames;
		private List<string> m_lGameIDs;
		private List<string> m_lGameNames;
		private List<string> m_lDisplay;
	
		public MainWindow()
		{
			InitializeComponent();
			_hidden.InitializeWeb();

			m_dClanStack = new Dictionary<string, string>();
			m_dClanStackLabels = new Dictionary<string, Border>();

			// make sure necessary files exist
			if (!File.Exists(Master.GetBaseDir() + "_clans.dat")) { File.Create(Master.GetBaseDir() + "_clans.dat").Dispose(); }
			if (!File.Exists(Master.GetBaseDir() + "_key.dat"))
			{
				Password pPassWindow = new Password();
				pPassWindow.ShowDialog();
			}
			else { Master.SetKey(File.ReadAllText(Master.GetBaseDir() + "_key.dat")); }

			// set data if a persistant clan thing was saved (persists active clan between application closing/opening)
			if (File.Exists(Master.GetBaseDir() + "_active.dat"))
			{
				string[] aLines = File.ReadAllLines(Master.GetBaseDir() + "_active.dat");
				Master.SetActiveClan(aLines[0]);
				Master.SetActiveUserName(aLines[1]);
			}

			this.RefreshClanStack();
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
				pLabel.Margin = new Thickness(2, 2, 2, 0);
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
		}

		public void ChangeActiveClan(string sClanText) // NOTE: clantext includes both clan name and username
		{
			// determine parts of the text
			string sClanName = sClanText.Substring(0, sClanText.IndexOf("|"));
			string sUserName = sClanText.Substring(sClanText.IndexOf("|") + 1);

			// set currently active components
			Master.SetActiveClan(sClanName);
			Master.SetActiveUserName(sUserName);

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

		private void btnJoin_Click(object sender, RoutedEventArgs e)
		{
			/*string sUser = txtUserName.Text;
			string sPass = txtPassword.Text;
			string sGame = txtGameID.Text;

			string sClan = "Testing Clan";
			string sBody = "<params><param name='sGameID'>" + sGame + "</param><param name='sClanName'>" + sClan + "</param><param name='sUserName'>" + sUser + "</param><param name='sUserPassPhrase'>" + sPass + "</param></params>";
			
			string sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/GameClansServer/GameClansServer.Games/Zendo/JoinGame", sBody, true);
			sResponse = Master.CleanResponse(sResponse);
			blkLog.Text += sResponse + "\n";*/
		}

		private void btnListGames_Click(object sender, RoutedEventArgs e)
		{
			/*string sUser = txtUserName.Text;
			string sPass = txtPassword.Text;

			string sClan = "Testing Clan";

			string sBody = "<params><param name='sClanName'>" + sClan + "</param><param name='sUserName'>" + sUser + "</param><param name='sUserPassPhrase'>" + sPass + "</param></params>";

			string sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/GameClansServer/GameClansServer/ClanServer/ListActiveGames", sBody, true);
			sResponse = Master.CleanResponse(sResponse);
			blkLog.Text += sResponse + "\n";*/
		}

		private void btnStartGame_Click(object sender, RoutedEventArgs e)
		{
			/*string sGame = txtGameID.Text;

			string sBody = "<params><param name='sGameID'>" + sGame + "</param></params>";
			
			string sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/GameClansServer/GameClansServer.Games/Zendo/StartGame", sBody, true);
			sResponse = Master.CleanResponse(sResponse);
			blkLog.Text += sResponse + "\n";*/
		}

		private void btnJoinClan_MouseEnter(object sender, MouseEventArgs e) { btnJoinClan.Background = Master.BUTTON_HOVER; }
		private void btnJoinClan_MouseLeave(object sender, MouseEventArgs e) { btnJoinClan.Background = Master.BUTTON_NORMAL; }
		
		private void btnJoinClan_MouseUp(object sender, MouseButtonEventArgs e)
		{
			JoinClan pJoinClan = new JoinClan();
			pJoinClan.ShowDialog();
		}
		
		private void btnCreateClan_MouseEnter(object sender, MouseEventArgs e) { btnCreateClan.Background = Master.BUTTON_HOVER; }
		private void btnCreateClan_MouseLeave(object sender, MouseEventArgs e) { btnCreateClan.Background = Master.BUTTON_NORMAL; }

		private void btnCreateClan_MouseUp(object sender, MouseButtonEventArgs e)
		{

		}
	}
}
