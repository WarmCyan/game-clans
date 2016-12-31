//*************************************************************
//  File: JoinClan.xaml.cs
//  Date created: 12/15/2016
//  Date edited: 12/30/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: 
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using DWL.Utility;

namespace Client
{
	/// <summary>
	/// Interaction logic for JoinClan.xaml
	/// </summary>
	public partial class JoinClan : Window
	{
		public JoinClan()
		{
			InitializeComponent();
		}

		private void btnSubmitJoinClan_MouseLeave(object sender, MouseEventArgs e) { btnSubmitJoinClan.Background = Master.BUTTON_NORMAL; }
		private void btnSubmitJoinClan_MouseEnter(object sender, MouseEventArgs e) { btnSubmitJoinClan.Background = Master.BUTTON_HOVER; }

		private void btnSubmitJoinClan_MouseUp(object sender, MouseButtonEventArgs e)
		{
			// TODO: data sanitization!!!

			string sUserPass = Master.GetKey();
			string sBody = "<params><param name='sEmail'>" + Master.GetEmail() + "</param><param name='sClanName'>" + txtClanName.Text + "</param><param name='sClanPassPhrase'>" + txtClanPassword.Password + "</param><param name='sUserName'>" + txtUserName.Text + "</param><param name='sUserPassPhrase'>" + sUserPass + "</param></params>";
			string sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/GameClansServer/GameClansServer/ClanServer/JoinClan", sBody, true);

			XElement pResponse = Master.ReadResponse(sResponse);

			string sResponseMessage = pResponse.Element("Text").Value;

			if (pResponse.Attribute("Type").Value == "Error" || pResponse.Element("Data").Element("ClanStub") == null)
			{
				MessageBox.Show(sResponseMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
			else
			{
				MessageBox.Show(sResponseMessage);
				XElement pClanStub = pResponse.Element("Data").Element("ClanStub");
				string sClanName = pClanStub.Attribute("ClanName").Value;
				string sUserName = pClanStub.Attribute("UserName").Value;

				File.AppendAllLines(Master.GetBaseDir() + "_clans.dat", new List<string>() { sClanName + "|" + sUserName });
				this.Close();
			}
		}
	}
}
