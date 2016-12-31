//*************************************************************
//  File: LogIn.xaml.cs
//  Date created: 12/30/2016
//  Date edited: 12/30/2016
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
	/// Interaction logic for LogIn.xaml
	/// </summary>
	public partial class LogIn : Window
	{
		public LogIn()
		{
			InitializeComponent();
		}

		private void btnSubmitPassword_MouseLeave(object sender, MouseEventArgs e) { btnSubmitPassword.Background = Master.BUTTON_NORMAL; }
		private void btnSubmitPassword_MouseEnter(object sender, MouseEventArgs e) { btnSubmitPassword.Background = Master.BUTTON_HOVER; }

		private void btnSubmitPassword_MouseUp(object sender, MouseButtonEventArgs e)
		{
			string sBody = "<params><param name='sEmail'>" + txtEmail.Text + "</param><param name='sPassword'>" + Security.Sha256Hash(txtPass.Password) + "</param></params>";
			string sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetServerURL() + "ReturningUser", sBody, true);
			XElement pResponse = Master.ReadResponse(sResponse);

			string sResponseMessage = pResponse.Element("Text").Value;

			if (pResponse.Attribute("Type").Value == "Error")
			{
				MessageBox.Show(sResponseMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
			else
			{
				MessageBox.Show(sResponseMessage);
				Master.HandleUserRegistrationData(pResponse);
				this.Close();
			}
		}
	}
}
