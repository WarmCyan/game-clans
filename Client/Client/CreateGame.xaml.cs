//*************************************************************
//  File: CreateGame.xaml.cs
//  Date created: 12/23/2016
//  Date edited: 12/23/2016
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
using System.Windows.Shapes;
using System.Xml.Linq;

using DWL.Utility;

namespace Client
{
	/// <summary>
	/// Interaction logic for CreateGame.xaml
	/// </summary>
	public partial class CreateGame : Window
	{
		public CreateGame()
		{
			InitializeComponent();
		}

		private void btnCreateGame_MouseLeave(object sender, MouseEventArgs e) { btnCreateGame.Background = Master.BUTTON_NORMAL; }
		private void btnCreateGame_MouseEnter(object sender, MouseEventArgs e) { btnCreateGame.Background = Master.BUTTON_HOVER; }

		private void btnCreateGame_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (txtGameName.Text == "") { MessageBox.Show("The game name can't be blank", "Error", MessageBoxButton.OK, MessageBoxImage.Asterisk); }
			if (!(bool)(rbtnZendo.IsChecked)) { MessageBox.Show("Please check which game type you would like to create", "Error", MessageBoxButton.OK, MessageBoxImage.Asterisk); }


			string sGameType = "";
			
			if ((bool)rbtnZendo.IsChecked) { sGameType = "Zendo"; }

			string sBody = Master.BuildCommonBody("<param name='sGameName'>" + txtGameName.Text.ToString() + "</param>");
			string sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetGameURL(sGameType) + "CreateNewGame", sBody, true);

			XElement pResponse = Master.ReadResponse(sResponse);

			if (pResponse.Attribute("Type").Value == "Error")
			{
				MessageBox.Show(pResponse.Element("Text").Value, "Error", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}
			else
			{
				MessageBox.Show(pResponse.Element("Text").Value);
				this.Close();
			}
		}
	}
}
