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
			WebCommunications.SendGetRequest("http://dwlapi.azurewebsites.net/api/reflection/");
		}

		private void btnListGames_Click(object sender, RoutedEventArgs e)
		{
			string sUser = txtUserName.Text;
			string sPass = txtPassword.Text;

			string sClan = "Testing Clan";

			string sBody = "<params><param name='sClanNAme'></param></params>";
			
			//WebCommunications.SendPostRequest
		}
	}
}
