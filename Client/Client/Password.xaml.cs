//*************************************************************
//  File: Password.xaml.cs
//  Date created: 12/15/2016
//  Date edited: 12/15/2016
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

using System.IO;

using DWL.Utility;

namespace Client
{
	/// <summary>
	/// Interaction logic for Password.xaml
	/// </summary>
	public partial class Password : Window
	{
		public Password()
		{
			InitializeComponent();
		}

		private void btnSubmitPassword_MouseLeave(object sender, MouseEventArgs e) { btnSubmitPassword.Background = Master.BUTTON_NORMAL; }
		private void btnSubmitPassword_MouseEnter(object sender, MouseEventArgs e) { btnSubmitPassword.Background = Master.BUTTON_HOVER; }

		private void btnSubmitPassword_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (txtPass.Password == "") { MessageBox.Show("Your password cannot be an empty string.", "Empty password", MessageBoxButton.OK, MessageBoxImage.Error); return; }
			if (txtPass.Password != txtPass2.Password) { MessageBox.Show("The passwords do not match.", "Inconsistent password", MessageBoxButton.OK, MessageBoxImage.Error); return; }

			if (File.Exists(Master.GetBaseDir() + "_key.dat"))
			{
				bool bContinue = false;
				var result = MessageBox.Show("Please note that your password is used by default for all of your usernames in the different clans. If this device does not have every clan you play with, your passwords will be out of sync. Are you sure you want to continue?", "Caution", MessageBoxButton.YesNo, MessageBoxImage.Warning);
				if (result == MessageBoxResult.Yes) { bContinue = true; }
				else { return; }

				// unnecessary safe-guard to make me feel better
				if (!bContinue) { return; }

				// TODO: make the server call
			}

			string sEncrypted = Security.Sha256Hash(txtPass.Password);
			File.WriteAllText(Master.GetBaseDir() + "_key.dat", sEncrypted);
			Master.SetKey(sEncrypted);
			this.Close();
		}
	}
}
