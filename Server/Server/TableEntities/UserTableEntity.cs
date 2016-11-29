//*************************************************************
//  File: UserTableEntity.cs
//  Date created: 11/28/2016
//  Date edited: 11/28/2016
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
	public class UserTableEntity : TableEntity
	{
		// partition key is "clan name|USER"
		// row key is username 
	
		public UserTableEntity(string sClanName, string sUserName)
		{
			this.PartitionKey = Master.BuildUserPartitionKey(sClanName);
			this.RowKey = sUserName;
			this.PassPhrase = "";
			this.ZendoScore = 0;
		}

		public string PassPhrase { get; set; }
		public int ZendoScore { get; set; }
	}
}
