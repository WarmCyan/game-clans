//*************************************************************
//  File: ClanServer.cs
//  Date created: 11/28/2016
//  Date edited: 11/28/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: Main server that has all the outward facing REST API functions
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;

using GameClansServer.TableEntities;

namespace GameClansServer
{
	public class ClanServer
	{
		// member variables
		private CloudStorageAccount m_pStorageAccount;
		private CloudTableClient m_pTableClient;
		private CloudBlobClient m_pBlobClient;

		private CloudTable m_pTable = null;
		private CloudBlobContainer m_pContainer = null;

		// construction
		public ClanServer()
		{
			this.Initialize();
		}
		
		// properties
		public CloudTable Table
		{
			get
			{
				if (m_pTable == null) { m_pTable = this.FetchTable(); }
				return m_pTable;
			}
		}
		public CloudBlobContainer Container
		{
			get
			{
				if (m_pContainer == null) { m_pContainer = this.FetchContainer(); }
				return m_pContainer;
			}
		}

		// outward facing methods

		public string CreateClan(string sClanName, string sClanPassPhrase)
		{
			// make sure a clan with this name doesn't already exist	
			TableOperation pClanRetrieveOp = TableOperation.Retrieve<ClanTableEntity>("CLAN", sClanName);
			if (this.Table.Execute(pClanRetrieveOp).Result != null) { return Master.MessagifyError("A clan with this name already exists."); }

			// if we make it to this point we're good, add the new clan!
			ClanTableEntity pClan = new ClanTableEntity(sClanName);
			pClan.PassPhrase = this.Sha256Hash(sClanPassPhrase);
			this.Table.Execute(TableOperation.Insert(pClan));

			return Master.MessagifySimple("You have successfully created the clan " + sClanName + "!");
		}

		public string JoinClan(string sClanName, string sClanPassPhrase, string sUserName, string sUserPassPhrase)
		{
			// make sure the clan password is correct
			if (!VerifyClanPassPhrase(sClanName, sClanPassPhrase)) { return Master.MessagifyError("Clan password incorrect."); }

			// make sure the username doesn't already exist
			TableOperation pUserRetrieveOp = TableOperation.Retrieve<UserTableEntity>(Master.BuildUserPartitionKey(sClanName), sUserName);
			if (this.Table.Execute(pUserRetrieveOp).Result != null) { return Master.MessagifyError("User with this name already exists in this clan."); }

			// if we make it to this point, we're good, add a new user!
			UserTableEntity pUser = new UserTableEntity(sClanName, sUserName);
			pUser.PassPhrase = this.Sha256Hash(sUserPassPhrase);
			this.Table.Execute(TableOperation.Insert(pUser));

			//return "You have successfully joined the clan " + sClanName + " as " + sUserName;
			return Master.Messagify("You have successfully joined the clan " + sClanName + " as " + sUserName, Master.MSGTYPE_BOTH, "<ClanStub ClanName='" + sClanName + "' UserName='" + sUserName + "' />");
		}

		// inner methods

		private bool VerifyClanPassPhrase(string sClanName, string sClanPassPhrase)
		{
			// get the requested clan row from the table 
			TableOperation pClanRetrieveOp = TableOperation.Retrieve<ClanTableEntity>("CLAN", sClanName);
			TableResult pClanRetrieveResult = this.Table.Execute(pClanRetrieveOp);
			ClanTableEntity pClan = (ClanTableEntity)pClanRetrieveResult.Result;

			// hash clan passphrase
			string sClanHash = this.Sha256Hash(sClanPassPhrase);

			// check if the passphrase matches 
			return sClanHash == pClan.PassPhrase;
		}

		private bool VerifyUserPassPhrase(string sClanName, string sUserName, string sUserPassPhrase)
		{
			// get the requested user row from the table
			TableOperation pUserRetrieveOp = TableOperation.Retrieve<UserTableEntity>(Master.BuildUserPartitionKey(sClanName), sUserName);
			TableResult pUserRetrieveResult = this.Table.Execute(pUserRetrieveOp);
			UserTableEntity pUser = (UserTableEntity)pUserRetrieveResult.Result;

			// hash user passphrase
			string sUserHash = this.Sha256Hash(sUserPassPhrase);

			// check if the passphrase matches
			return sUserHash == pUser.PassPhrase;
		}

		// thanks to http://www.codeshare.co.uk/blog/sha-256-and-sha-512-hash-examples/
		private string Sha256Hash(string sString)
		{
			SHA256 p256 = SHA256Managed.Create();
			byte[] aBytes = Encoding.UTF8.GetBytes(sString);
			byte[] aHashBytes = p256.ComputeHash(aBytes);
			string sHash = this.GetStringFromHash(aHashBytes);
			return sHash;
		}

		private string GetStringFromHash(byte[] aHashBytes)
		{
			StringBuilder pResult = new StringBuilder();
			for (int i = 0; i < aHashBytes.Length; i++)
			{
				pResult.Append(aHashBytes[i].ToString("X2"));
			}
			return pResult.ToString();
		}

		// set up cloud storage account stuff
		private void Initialize()
		{
			m_pStorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
			m_pTableClient = m_pStorageAccount.CreateCloudTableClient();
			m_pBlobClient = m_pStorageAccount.CreateCloudBlobClient();
		}

		private CloudTable FetchTable()
		{
			CloudTable pTable = m_pTableClient.GetTableReference("gameclansdb");

			// make sure it exists
			pTable.CreateIfNotExists();
			return pTable;
		}

		private CloudBlobContainer FetchContainer()
		{
			CloudBlobContainer pContainer = m_pBlobClient.GetContainerReference("gameclansgames");

			// make sure it exists
			pContainer.CreateIfNotExists();
			return pContainer;
		}
	}
}
