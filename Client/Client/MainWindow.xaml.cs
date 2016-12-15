//*************************************************************
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

using DWL.Utility;

namespace Client
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			_hidden.InitializeWeb();

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

		private void btnJoinClan_MouseUp(object sender, MouseButtonEventArgs e)
		{

		}

		private void btnJoinClan_MouseEnter(object sender, MouseEventArgs e)
		{

		}

		private void btnJoinClan_MouseLeave(object sender, MouseEventArgs e)
		{

		}

		private void btnCreateClan_MouseLeave(object sender, MouseEventArgs e)
		{

		}

		private void btnCreateClan_MouseEnter(object sender, MouseEventArgs e)
		{

		}

		private void btnCreateClan_MouseUp(object sender, MouseButtonEventArgs e)
		{

		}
	}
}
