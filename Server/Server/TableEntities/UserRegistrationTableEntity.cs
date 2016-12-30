//*************************************************************
//  File: UserRegistrationTableEntity.cs
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

using Microsoft.WindowsAzure.Storage.Table;

namespace GameClansServer.TableEntities
{
	public class UserRegistrationTableEntity : TableEntity
	{
		public UserRegistrationTableEntity(string sEmail)
		{
			this.PartitionKey = "USER";
			this.RowKey = sEmail;
		}

		public UserRegistrationTableEntity() { }

		public string Password { get; set; }
		public string Key { get; set; }
		public bool Notifications { get; set; }
	}
}
