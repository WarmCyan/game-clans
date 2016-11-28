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

		// construction
		public ClanServer()
		{
			this.Initialize();
		}

		// outward facing methods

		public string JoinClan(string sClanName, string sPassPhrase, string sUserName)
		{

			// if matches, add username 

			return "";
		}

		// does this do something like end 
		public string GetClanStuff(string sClanName)
		{
			return "";
		}

		// inner methods

		private bool VerifyClanPassPhrase(string sClanName, string sPassPhrase)
		{
			// get the requested clan row from the table 
			TableOperation pClanRetrieveOp = TableOperation.Retrieve<ClanTableEntity>("CLAN", sClanName);
			TableResult pClanRetrieveResult = this.Table.Execute(pClanRetrieveOp);
			ClanTableEntity pClan = (ClanTableEntity)pClanRetrieveResult.Result;

			// encrypt passphrase
			SHA256 p256 = SHA256Managed.Create();
			byte[] aBytes = Encoding.UTF8.GetBytes(sPassPhrase);
			byte[] aHashBytes = p256.ComputeHash(aBytes);
			string sHash = this.GetStringFromHash(aHashBytes);

			// check if the passphrase matches 


			return false;
		}

		// thanks to http://www.codeshare.co.uk/blog/sha-256-and-sha-512-hash-examples/
		private string GetStringFromHash(byte[] aHashBytes)
		{
			StringBuilder pResult = new StringBuilder();
			for (int i = 0; i < aHashBytes.Length; i++)
			{
				pResult.Append(aHashBytes[i].ToString("X2"));
			}
			return pResult.ToString();
		}

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
