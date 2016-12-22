//*************************************************************
//  File: NotificationsActivity.cs
//  Date created: 12/22/2016
//  Date edited: 12/22/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: 
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using DWL.Utility;

namespace App
{
	[Activity(Label = "Notifications")]
	public class NotificationsActivity : BaseActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.SetContentView(Resource.Layout.Notifications);

			base.CreateDrawer();

			// get the notifcations list from the server
			string sResponse = WebCommunications.SendPostRequest(Master.GetBaseURL() + Master.GetServerURL() + "GetLastNNotifications", Master.BuildCommonBody(), true);
			XElement pResponse = Master.ReadResponse(sResponse);

			List<string> lDisplay = new List<string>();
			List<string> lIDs = new List<string>();
			List<string> lGameNames = new List<string>();

			if (pResponse.Element("Data").Element("Notifications") != null)
			{
				foreach (XElement pNotif in pResponse.Element("Data").Element("Notifications").Elements("Notification"))
				{
					lDisplay.Add(pNotif.Attribute("GameName").Value + " - " + pNotif.Value);
					lIDs.Add(pNotif.Attribute("GameID").Value);
					lGameNames.Add(pNotif.Attribute("GameName").Value);
				}
			}

			ListView pNotifList = FindViewById<ListView>(Resource.Id.lstNotifications);
			pNotifList.Adapter = new DrawerItemCustomAdapter(this, Resource.Layout.ListViewItemRow, lDisplay.ToArray());

			pNotifList.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
			{
				int iChoice = e.Position;

				Intent pIntent = null;
				if (lIDs[iChoice].Contains("Zendo")) { pIntent = new Intent(this, typeof(ZendoActivity)); }
				pIntent.SetAction(lIDs[iChoice]);
				pIntent.PutExtra("GameName", lGameNames[iChoice]);
				this.Finish();
				StartActivity(pIntent);
			};

			/*LinearLayout pNotifLayout = FindViewById<LinearLayout>(Resource.Id.lstNotifications);
			pNotifLayout.RemoveAllViews();
			if (pResponse.Element("Data").Element("Notifications") != null)
			{
				foreach (XElement pNotif in pResponse.Element("Data").Element("Notifications").Elements("Notification"))
				{
					// make the datarow layout
					View pDataRow = LayoutInflater.From(this).Inflate(Resource.Layout.DataRow, pNotifLayout, false);

					TextView pDataText = pDataRow.FindViewById<TextView>(Resource.Id.txtText);
					pDataText.Text = pNotif.Attribute("GameName").Value + " - " + pNotif.Value;

					pDataText.Click += delegate
					{
						string sGameID = pNotif.Attribute("GameID").Value;
						Intent pIntent = null;
						if (sGameID.Contains("Zendo")) { pIntent = new Intent(this, typeof(ZendoActivity)); }
						pIntent.SetAction(sGameID);
						pIntent.PutExtra("GameName", pNotif.Attribute("GameName").Value);
						this.Finish();
						StartActivity(pIntent);
					};

					pNotifLayout.AddView(pDataRow);
				}
			}*/

		}
	}
}