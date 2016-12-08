//*************************************************************
//  File: GameTableEntity
//  Date created: 12/8/2016
//  Date edited: 12/8/2016
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
	class GameTableEntity : TableEntity
	{
		public GameTableEntity(string sClanName, string sGameID)
		{
			this.PartitionKey = Master.BuildGamePartitionKey(sClanName);
			this.RowKey = sGameID;
			this.Active = false;
		}
	
		public GameTableEntity() { }

		public bool Active { get; set; }
		public string GameType { get; set; }
	}
}
