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
	/// Interaction logic for CreateClan.xaml
	/// </summary>
	public partial class CreateClan : Window
	{
		public CreateClan()
		{
			InitializeComponent();
		}

		private void btnSubmitCreateClan_MouseLeave(object sender, MouseEventArgs e) { btnSubmitCreateClan.Background = Master.BUTTON_NORMAL; }
		private void btnSubmitCreateClan_MouseEnter(object sender, MouseEventArgs e) { btnSubmitCreateClan.Background = Master.BUTTON_HOVER; }

		private void btnSubmitCreateClan_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (txtClanName.Text == "" || txtClanPassword.Password == "" || txtClanPassword2.Password == "")
			{
				MessageBox.Show("No fields can be blank", "Error", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}
			if (txtClanPassword.Password != txtClanPassword2.Password)
			{
				MessageBox.Show("The passwords don't match", "Error", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}

			string sBody = "<params><param name='sClanName'>" + txtClanName.Text.ToString() + "</param><param name='sClanPassPhrase'>" + txtClanPassword.Password + "</param></params>";
			string sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetServerURL() + "CreateClan", sBody, true);

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
