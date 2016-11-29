//*************************************************************
//  File: ClanTableEntity.cs
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
	public class ClanTableEntity : TableEntity
	{
		// partition key is CLAN
		// row key is the name of the clan
	
		public ClanTableEntity(string sClanName) 
		{
			this.PartitionKey = "CLAN";
			this.RowKey = sClanName;
			this.PassPhrase = "";
		}
		public ClanTableEntity() { }

		public string PassPhrase { get; set; }
	}
}
