//*************************************************************
//  File: Zendo.xaml.cs
//  Date created: 12/15/2016
//  Date edited: 12/15/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: Zendo game window
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

namespace Client.GameWindows
{
	/// <summary>
	/// Interaction logic for Zendo.xaml
	/// </summary>
	public partial class Zendo : Window
	{
		// member variables
		private string m_sGameID;

		public Zendo(string sGameID)
		{
			InitializeComponent();

			m_sGameID = sGameID;
		}
	}
}
