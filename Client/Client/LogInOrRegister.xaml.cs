//*************************************************************
//  File: LogInOrRegister.xaml.cs
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

using DWL.Utility;

namespace Client
{
	/// <summary>
	/// Interaction logic for LogInOrRegister.xaml
	/// </summary>
	public partial class LogInOrRegister : Window
	{
		public LogInOrRegister()
		{
			InitializeComponent();
		}

		private void btnNewUser_MouseLeave(object sender, MouseEventArgs e) { btnNewUser.Background = Master.BUTTON_NORMAL; } 
		private void btnNewUser_MouseEnter(object sender, MouseEventArgs e) { btnNewUser.Background = Master.BUTTON_HOVER; }

		private void btnExistingUser_MouseLeave(object sender, MouseEventArgs e) { btnExistingUser.Background = Master.BUTTON_NORMAL; }
		private void btnExistingUser_MouseEnter(object sender, MouseEventArgs e) { btnExistingUser.Background = Master.BUTTON_HOVER; }
		
		private void btnNewUser_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Password pPassword = new Password();
			pPassword.ShowDialog();
			this.Close();
		}

		private void btnExistingUser_MouseUp(object sender, MouseButtonEventArgs e)
		{
			LogIn pLogIn = new LogIn();
			pLogIn.ShowDialog();
			this.Close();
		}
	}
}
