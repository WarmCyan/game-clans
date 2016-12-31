//*************************************************************
//  File: RegistrationClanUserTableEntity.cs
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
	public class RegistrationClanUserTableEntity : TableEntity
	{
		public RegistrationClanUserTableEntity(string sEmail, string sClanName, string sUserName)
		{
			this.PartitionKey = sEmail;
			this.RowKey = sClanName + "|" + sUserName;
		}

		public RegistrationClanUserTableEntity() { }
	}
}
