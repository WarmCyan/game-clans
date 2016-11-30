//*************************************************************
//  File: Zendo.cs
//  Date created: 11/28/2016
//  Date edited: 11/29/2016
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
	public enum AuthenticationType
	{
		All = 0,
		Master = 1,
		Student = 2,
		Player = 3
	}

	public class ZendoUser
	{
		// construction
		public ZendoUser(string sUserName)
		{
			this.UserName = sUserName;
		}

		// properties
		public string UserName { get; set; }
		//public bool IsMaster { get; set; } // pretty sure this isn't necessary
		public int GuessingStones { get; set; }

		public XElement Xml
		{
			get
			{
				XElement pXml = new XElement("User");
				pXml.SetAttributeValue("GuessingStones", this.GuessingStones);
				pXml.SetAttributeValue("Name", this.UserName);
				return pXml;
			}
		}
	}
		
	public class ZendoLogEvent
	{
		// construction
		public ZendoLogEvent(string sMsg)
		{
			this.Message = sMsg;
			this.Time = DateTime.Now;
		}
		public ZendoLogEvent(string sMsg, string sData)
		{
			this.Message = sMsg;
			this.Data = sData;
			this.Time = DateTime.Now;
		}
		
		// properties
		public string Message { get; set; } 
		public string Data { get; set; }
		public DateTime Time { get; set; }

		public XElement Xml
		{
			get
			{
				XElement pXml = new XElement("LogEvent");
				pXml.SetAttributeValue("Time", this.Time.ToString());
				pXml.SetElementValue("Message", this.Message);
				pXml.SetElementValue("Data", this.Data);
				return pXml;
			}
		}
	}

	public class ZendoKoan
	{
		public ZendoKoan(int iID, string sKoan, string sUserName)
		{
			this.ID = iID;
			this.Koan = sKoan;
			this.User = sUserName;
		}
		public ZendoKoan(int iID, string sKoan, string sUserName, bool bHasBuddhaNature)
		{
			this.ID = iID;
			this.Koan = sKoan;
			this.User = sUserName;
			this.HasBuddhaNature = bHasBuddhaNature;
		}

		public string Koan { get; set; }
		public string User { get; set; }
		public bool HasBuddhaNature { get; set; } 
		public int ID { get; set; }

		public string StringKoan // not sure if this will ever actually be needed?
		{
			get
			{
				string sInitial = (this.HasBuddhaNature) ? "T" : "F";
				return sInitial + this.Koan;
			}
		}

		public XElement Xml
		{
			get
			{
				XElement pXml = new XElement("Koan");
				pXml.SetAttributeValue("ID", this.ID);
				pXml.SetAttributeValue("User", this.User);
				pXml.SetAttributeValue("BuddhaNature", this.HasBuddhaNature);
				pXml.Value = this.Koan;
				return pXml;
			}
		}
	}
	
	public class Zendo
	{
		// member variables
		ClanServer m_pServer;
		
		private string m_sClanName;
		private List<string> m_lPlayerNames;
		
		private string m_sGameID;

		private string m_sMaster;
		//private List<string> m_lStudents;
		private List<ZendoUser> m_lStudents;
		private string m_sStateStatus; // "setup" (waiting for players to join), "initial" (waiting for Master's 2 koans), "open" (users can submit koans or guesses), "pending master" (waiting for master to analyze koan), "pending students" (mondo, waiting for majority of students to make their prediction), "pending disproval"
		private List<ZendoLogEvent> m_lEventLog;
		private List<ZendoKoan> m_lKoans; // first character is T|F for whether has buddha nature. Then comma delimited character pairs (first character is color, second is direction)
		private ZendoKoan m_pPendingKoan;
		private bool m_bMondo;
		private Dictionary<string, bool> m_dMondoPredictions;
		//private Dictionary<string, int> m_dGuessingStones;

		// construction
		// (if id is passed in, load that game's xml)
		public Zendo() { }
		// NOTE: don't think the below constructors would ever actually be used
		/*public Zendo(ClanServer pServer)
		{
			this.m_pServer = pServer;
		}
		public Zendo(ClanServer pServer, string sGameID)
		{
			this.m_pServer = pServer;
			this.m_sGameID = sGameID;

			// load xml (route through server, since this is loading text from a blob)
		}*/

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
			string sResult = this.Prepare(sGameID, sClanName, sUserName, sUserPassPhrase, AuthenticationType.All);
			if (sResult != "") { return sResult; }
			
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

			// make everyone else the students
			foreach (string sUser in m_lPlayerNames)
			{
				if (sUser == m_sMaster) { continue; }
				ZendoUser pUser = new ZendoUser(sUser);
				pUser.GuessingStones = 0;
			}

			this.Save();
			return "";
		}

		public string SubmitInitialKoans(string sGameID, string sClanName, string sUserName, string sUserPassPhrase, string sBuddhaNatureKoan, string sNonBuddhaNatureKoan)
		{
			string sResult = this.Prepare(sGameID, sClanName, sUserName, sUserPassPhrase, AuthenticationType.Master);
			if (sResult != "") { return sResult; }

			// are we actually at this stage?
			if (m_sStateStatus != "initial")
			{
				if (m_sStateStatus == "setup") { return Master.MessagifyError("The game hasn't been started yet"); }
				return Master.MessagifyError("The game is no longer in the initial stage");
			}
			
			// add the two koans
			ZendoKoan pBuddhaKoan = new ZendoKoan(0, sBuddhaNatureKoan, "master", true);
			ZendoKoan pNonBuddhaKoan = new ZendoKoan(1, sNonBuddhaNatureKoan, "master", false);
			m_lKoans.Add(pBuddhaKoan);
			m_lKoans.Add(pNonBuddhaKoan);

			// open up the game status
			m_sStateStatus = "open";

			// send notifications to all the students and add to the event log
			//foreach (string sUser in m_lStudents) { m_pServer.AddNotification(m_sClanName, sUser, "The game has started and the master has built the initial koans!"); }
			foreach (ZendoUser pUser in m_lStudents) { m_pServer.AddNotification(m_sClanName, pUser.UserName, "The game has started and the master has built the initial koans!"); }
			m_lEventLog.Add(new ZendoLogEvent("The master has built the initial koans", pBuddhaKoan.Xml.ToString() + pNonBuddhaKoan.Xml.ToString()));

			this.Save();
			return "";
		}

		public string SubmitKoan(string sGameID, string sClanName, string sUserName, string sUserPassPhrase, string sKoan)
		{
			string sResult = this.Prepare(sGameID, sClanName, sUserName, sUserPassPhrase, AuthenticationType.Student);
			if (sResult != "") { return sResult; }

			// make sure the state is open
			if (m_sStateStatus != "open") 
			{
				if (m_sStateStatus == "pending master") { return Master.MessagifyError("The master has not analyzed the current pending koan"); }
				if (m_sStateStatus == "pending students") { return Master.MessagifyError("Mondo has been called on the pending koan"); }
				if (m_sStateStatus == "pending disproval") { return Master.MessagifyError("The master is currently attempting to disprove the current guess"); }
				return Master.MessagifyError("The game is not currently accepting koan submissions");
			}

			m_pPendingKoan = new ZendoKoan(m_lKoans.Count, sKoan, sUserName);

			// send a notification to the master and add to the event log
			m_pServer.AddNotification(m_sClanName, m_sMaster, "A student has built a new koan for you to analyze.");
			m_lEventLog.Add(new ZendoLogEvent(sUserName + " has built a new koan", m_pPendingKoan.Xml.ToString()));

			this.Save();
			return "";
		}

		public string SubmitMondo(string sGameID, string sClanName, string sUserName, string sUserPassPhrase, string sKoan)
		{
			string sResult = this.Prepare(sGameID, sClanName, sUserName, sUserPassPhrase, AuthenticationType.Student);
			if (sResult != "") { return sResult; }

			// make sure the state is open
			if (m_sStateStatus != "open")
			{
				if (m_sStateStatus == "pending master") { return Master.MessagifyError("The master has not analyzed the current pending koan"); }
				if (m_sStateStatus == "pending students") { return Master.MessagifyError("Mondo has been called on the pending koan"); }
				if (m_sStateStatus == "pending disproval") { return Master.MessagifyError("The master is currently attempting to disprove the current guess"); }
				return Master.MessagifyError("The game is not currently accepting koan submissions");
			}

			m_pPendingKoan = new ZendoKoan(m_lKoans.Count, sKoan, sUserName);
			m_bMondo = true;
			m_dMondoPredictions = new Dictionary<string, bool>();

			// set the game status
			m_sStateStatus = "pending students";

			// notify all students and create an event log		
			//foreach (string sUser in m_lStudents)
			foreach (ZendoUser pUser in m_lStudents)
			{
				if (pUser.UserName == sUserName) { continue; }
				m_pServer.AddNotification(m_sClanName, pUser.UserName, sUserName + " has called mondo on a koan. Hurry and make your prediction!");
			}
			m_lEventLog.Add(new ZendoLogEvent(sUserName + " has called mondo on their koan", m_pPendingKoan.Xml.ToString()));

			this.Save();
			return "";
		}

		public string SubmitMondoPrediction(string sGameID, string sClanName, string sUserName, string sUserPassPhrase, bool bPrediciton)
		{
			string sResult = this.Prepare(sGameID, sClanName, sUserName, sUserPassPhrase, AuthenticationType.Student);
			if (sResult != "") { return sResult; }

			// make sure a mondo has actually been called!
			if (!m_bMondo) { return Master.MessagifyError("No mondo has been called"); }

			m_dMondoPredictions[sUserName] = bPrediciton;

			// if half of the students have made their predictions, move the game state to pending master 
			if (m_dMondoPredictions.Count >= Math.Ceiling((double)m_lStudents.Count / 2))
			{
				m_sStateStatus = "pending master";
				// send a notification to the master 
				m_pServer.AddNotification(m_sClanName, m_sMaster, "A student has called mondo on a new koan, and the students have made their predictions. The koan is ready for your analysis!");
			}

			this.Save();
			return "";
		}

		public string SubmitPendingKoanAnalysis(string sGameId, string sClanName, string sUserName, string sUserPassPhrase, bool bHasBuddhaNature)
		{
			string sResult = this.Prepare(sGameId, sClanName, sUserName, sUserPassPhrase, AuthenticationType.Master);
			if (sResult != "") { return sResult; }

			// make sure the state is 'pending master'
			if (m_sStateStatus != "pending master") { return Master.MessagifyError("There is no pending koan for you to analyze"); }

			// get the specified koan
			ZendoKoan pKoan = m_pPendingKoan;

			// set the koans buddha-nature and add it to the list
			pKoan.HasBuddhaNature = bHasBuddhaNature;
			m_lKoans.Add(pKoan);

			// handle if the koan was a mondo		
			if (m_bMondo)
			{
				foreach (string sUser in m_dMondoPredictions.Keys)
				{
					if (m_dMondoPredictions[sUser] == bHasBuddhaNature)
					{
						ZendoUser pUser = this.GetStudentByName(sUser);
						if (pUser == null) { /* fail silently for now, we don't want this to stop execution. */ }
						pUser.GuessingStones++;

						m_pServer.AddNotification(sClanName, sUser, "The master has answered the mondo, your prediction was correct!");
					}
					else { m_pServer.AddNotification(sClanName, sUser, "The master has answered the mondo, your prediction was incorrect."); }
				}
				m_bMondo = false;
			}

			// clear the pending koan and set the status back to open
			m_pPendingKoan = null;
			m_sStateStatus = "open";

			// notify the user who submitted the koan and add this to the event log
			string sKeyPhrase = (bHasBuddhaNature) ? "has" : "does not have";
			m_pServer.AddNotification(m_sClanName, pKoan.User, "The master has declared that your koan " + sKeyPhrase + " the buddha-nature");
			m_lEventLog.Add(new ZendoLogEvent("The master has declared that this koan " + sKeyPhrase + " the buddha-nature", pKoan.Xml.ToString()));

			this.Save();
			return "";
		}

		// inner methods


		private string Prepare(string sGameID, string sClanName, string sUserName, string sUserPassPhrase, AuthenticationType pType)
		{
			this.Initialize();
			
			// make sure user password is correct
			if (!m_pServer.VerifyUserPassPhrase(sClanName, sUserName, sUserPassPhrase)) { return Master.MessagifyError("Invalid login"); }

			this.Load(sGameID);
			
			if (pType == AuthenticationType.Master && sUserName != m_sMaster) { return Master.MessagifyError("Only the master can do this"); }
			if (pType == AuthenticationType.Student && sUserName == m_sMaster) { return Master.MessagifyError("Only a student can do this"); }
			if (pType == AuthenticationType.Player && !m_lPlayerNames.Contains(sUserName)) { return Master.MessagifyError("You must be a player in the game to do this"); }
			
			return "";
		}

		private void InitializeNewGame(string sClanName)
		{
			m_sGameID = "g_Zendo_" + DateTime.Now.Ticks.ToString();
			m_sClanName = sClanName;
			m_sStateStatus = "setup";

			m_lPlayerNames = new List<string>();
			m_lStudents = new List<ZendoUser>();
			m_lKoans = new List<ZendoKoan>();
			m_lEventLog = new List<ZendoLogEvent>();
			m_pPendingKoan = null;
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

		// NOTE: not sure if I actually need this?
		private ZendoKoan GetKoanById(int iID)
		{
			foreach (ZendoKoan pKoan in m_lKoans)
			{
				if (pKoan.ID == iID) { return pKoan; }
			}
			return null;
		}

		private ZendoUser GetStudentByName(string sName)
		{
			foreach (ZendoUser pUser in m_lStudents)
			{
				if (pUser.UserName == sName) { return pUser; }
			}
			return null;
		}
	}
}
