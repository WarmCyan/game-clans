//*************************************************************
//  File: Settings.xaml.cs
//  Date created: 12/24/2016
//  Date edited: 12/24/2016
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

namespace Client
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : Window
	{
		public Settings()
		{
			InitializeComponent();

			lblVersion.Content = "Client Version " + Master.CLIENT_VERSION;
		}

		private void btnPassword_MouseLeave(object sender, MouseEventArgs e) { btnPassword.Background = Master.BUTTON_NORMAL; }
		private void btnPassword_MouseEnter(object sender, MouseEventArgs e) { btnPassword.Background = Master.BUTTON_HOVER; }

		private void btnPassword_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Password pPassword = new Password();
			pPassword.ShowDialog();
		}
	}
}
