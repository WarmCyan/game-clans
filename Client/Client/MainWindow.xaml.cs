//*************************************************************
//  File: MainWindow.xaml.cs
//  Date created: 12/8/2016
//  Date edited: 12/8/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: 
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
		}

		private void btnJoin_Click(object sender, RoutedEventArgs e)
		{
			string sUser = txtUserName.Text;
			string sPass = txtPassword.Text;
			string sGame = txtGameID.Text;

			string sClan = "Testing Clan";
			string sBody = "<params><param name='sGameID'>" + sGame + "</param><param name='sClanName'>" + sClan + "</param><param name='sUserName'>" + sUser + "</param><param name='sUserPassPhrase'>" + sPass + "</param></params>";
			
			string sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/GameClansServer/GameClansServer.Games/Zendo/JoinGame", sBody, true);
			sResponse = Master.CleanResponse(sResponse);
			blkLog.Text += sResponse + "\n";
		}

		private void btnListGames_Click(object sender, RoutedEventArgs e)
		{
			string sUser = txtUserName.Text;
			string sPass = txtPassword.Text;

			string sClan = "Testing Clan";

			string sBody = "<params><param name='sClanName'>" + sClan + "</param><param name='sUserName'>" + sUser + "</param><param name='sUserPassPhrase'>" + sPass + "</param></params>";

			string sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/GameClansServer/GameClansServer/ClanServer/ListActiveGames", sBody, true);
			sResponse = Master.CleanResponse(sResponse);
			blkLog.Text += sResponse + "\n";
		}

		private void btnStartGame_Click(object sender, RoutedEventArgs e)
		{
			string sGame = txtGameID.Text;

			string sBody = "<params><param name='sGameID'>" + sGame + "</param></params>";
			
			string sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/GameClansServer/GameClansServer.Games/Zendo/StartGame", sBody, true);
			sResponse = Master.CleanResponse(sResponse);
			blkLog.Text += sResponse + "\n";
		}
	}
}
