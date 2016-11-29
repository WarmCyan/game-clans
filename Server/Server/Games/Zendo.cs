//*************************************************************
//  File: Zendo.cs
//  Date created: 11/28/2016
//  Date edited: 11/28/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: My implementation of the awesome game of Zendo!
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameClansServer.Games
{
	public class Zendo
	{
		// member variables
		ClanServer m_pServer;
		
		private string m_sClanName;
		private List<string> m_lPlayerNames;
		
		private string m_sGameID;

		private string m_sMaster;
		private List<string> m_lStudents;
		private string m_sStateStatus; // "setup" (waiting for players to join), "initial" (waiting for Master's 2 koans), "open" (users can submit koans or guesses), "awaiting master" (waiting for master to analyze koan), "awaiting students" (mondo, waiting for majority of students to make their prediction), "awaiting disproval"
		private List<string> m_lEventLog;

		// construction
		// (if id is passed in, load that game's xml)
		public Zendo() { }
		// NOTE: don't think the below constructors would ever actually be used
		public Zendo(ClanServer pServer)
		{
			this.m_pServer = pServer;
		}
		public Zendo(ClanServer pServer, string sGameID)
		{
			this.m_pServer = pServer;
			this.m_sGameID = sGameID;

			// load xml (route through server, since this is loading text from a blob)
		}

		// properties
		public XElement StateXml // NOTE: this should only be used for literal saving and loading
		{
			get
			{
				XElement pXml = new XElement("Game");
				pXml.SetAttributeValue("ID", m_sGameID);

				return pXml;
			}
			set // set all class properties from state assigned
			{
				
			}
		}

		// methods

		// outward facing methods

		public string CreateNewGame(string sClanName, string sUserName, string sUserPassPhrase)
		{
			this.Initialize();

			// verify user can create new game (part of this clan and valid password)
			if (!m_pServer.VerifyUserPassPhrase(sClanName, sUserName, sUserPassPhrase)) { return Master.MessagifyError("Invalid login."); }

			this.InitializeNewGame(sClanName);

			this.Save();
			return Master.MessagifySimple("Successfully started a new game of Zendo!");
		}

		public string JoinGame(string sGameID, string sClanName, string sUserName, string sUserPassPhrase)
		{
			this.Initialize();
			
			// verify user can join this game
			if (!m_pServer.VerifyUserPassPhrase(sClanName, sUserName, sUserPassPhrase)) { return Master.MessagifyError("Invalid login."); }

			this.Load(sGameID);
			this.AddUser(sUserName);
			this.Save();
			return "";
		}

		public string StartGame(string sGameID)
		{
			this.Initialize();
			this.Load(sGameID);

			// choose a master if one wasn't already chosen
			if (m_sMaster == "" || m_sMaster == null) { m_sMaster = this.ChooseRandomName(); }
			this.m_sStateStatus = "initial";

			this.Save();
			return "";
		}

		// TODO: programming while tired, double check!
		public string SubmitInitialKoans(string sGameID, string sClanName, string sUserName, string sUserPassPhrase)
		{
			this.Initialize();

			// verify user
			if (!m_pServer.VerifyUserPassPhrase(sClanName, sUserName, sUserPassPhrase)) { return Master.MessagifyError("Invalid login"); }

			this.Load(sGameID);

			// are we actually at this stage?
			if (m_sStateStatus != "initial")
			{
				if (m_sStateStatus == "setup") { return Master.MessagifyError("The game hasn't been started yet"); }
				return Master.MessagifyError("The game is no longer in the initial stage");
			}

			// make sure this user is the master and can in fact be submitting the initial koans
			if (sUserName != m_sMaster) { return Master.MessagifyError("Only the master can submit the initial koans"); }

			// TODO: handle the koans here

			this.Save();
			return "";
		}

		// inner methods

		private void InitializeNewGame(string sClanName)
		{
			m_sGameID = "g_Zendo_" + DateTime.Now.Ticks.ToString();
			m_sClanName = sClanName;
			m_sStateStatus = "setup";

			m_lPlayerNames = new List<string>();
			m_lStudents = new List<string>();
			m_sMaster = "";
		}

		private void AddUser(string sUserName) { m_lPlayerNames.Add(sUserName); }
		private void SetMaster(string sUserName) { m_sMaster = sUserName; }
		
		// any outward facing methods that are verbatim called need to use this method to appropriately set things up
		private void Initialize() { m_pServer = new ClanServer(); } 
		private void Load(string sGameID) { this.StateXml = m_pServer.LoadGame(sGameID); }
		private void Save() { m_pServer.SaveGame(this.StateXml, m_sGameID); }

		private string ChooseRandomName()
		{
			Random pRandom = new Random();
			int iRandomIndex = pRandom.Next(0, m_lPlayerNames.Count);
			return m_lPlayerNames[iRandomIndex];
		}
	}
}
