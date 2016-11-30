//*************************************************************
//  File: UserNotifTableEntity.cs
//  Date created: 11/29/2016
//  Date edited: 11/29/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: 
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Table;

namespace GameClansServer.TableEntities
{
	public class UserNotifTableEntity : TableEntity
	{
		// partition key is "clan name|user name|NOTIF"
		// row key is n id (doesn't actually point to anything, just an id)

		public UserNotifTableEntity(string sClanName, string sUserName)
		{
			this.PartitionKey = Master.BuildUserNotifPartitionKey(sClanName, sUserName);
			this.RowKey = "n" + DateTime.Now.Ticks.ToString();
			this.ClanName = sClanName;
		}

		public UserNotifTableEntity() { }

		public string ClanName { get; set; }
		public string Content { get; set; }
		public bool Seen { get; set; } 
		public DateTime Time { get; set; }
	}
}
