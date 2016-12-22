//*************************************************************
//  File: Zendo.cs
//  Date created: 11/28/2016
//  Date edited: 12/22/2016
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

	public class ZendoGuess
	{
		// construction
		public ZendoGuess() 
		{
			this.Guess = "";
			this.User = "";
			this.Time = DateTime.MinValue;
			this.Disproval = new ZendoKoan();
		}
		public ZendoGuess(string sGuess, string sUserName)
		{
			this.Guess = sGuess;
			this.User = sUserName;
			this.Time = DateTime.MinValue;
			this.Disproval = new ZendoKoan();
		}

		// properties
		public string Guess { get; set; }
		public string User { get; set; }
		public DateTime Time { get; set; } 
		public ZendoKoan Disproval { get; set; } 

		public XElement Xml
		{
			get
			{
				XElement pXml = new XElement("Guess");
				pXml.SetAttributeValue("User", this.User);
				pXml.SetAttributeValue("Time", this.Time);
				pXml.SetElementValue("Guess", this.Guess);
				//pXml.SetElementValue("Disproval", this.Disproval.Xml);
				pXml.Add(this.Disproval.Xml);
				return pXml;
			}
			set
			{
				this.User = value.Attribute("User").Value;
				this.Time = DateTime.Parse(value.Attribute("Time").Value);
				this.Guess = value.Element("Guess").Value;
				//this.Disproval = ZendoKoan.Load(value.Element("Disproval"));
				this.Disproval = ZendoKoan.Load(value.Element("Koan"));
			}
		}

		public static ZendoGuess Load(XElement pXml)
		{
			ZendoGuess pGuess = new ZendoGuess();
			pGuess.Xml = pXml;
			return pGuess;
		}
	}

	public class ZendoUser
	{
		// construction
		public ZendoUser() 
		{
			this.GuessingStones = 0;
			this.UserName = "";
		}
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
				XElement pXml = new XElement("Student");
				pXml.SetAttributeValue("GuessingStones", this.GuessingStones);
				pXml.SetAttributeValue("Name", this.UserName);
				return pXml;
			}
			set
			{
				this.GuessingStones = Convert.ToInt32(value.Attribute("GuessingStones").Value);
				this.UserName = value.Attribute("Name").Value;
			}
		}

		public static ZendoUser Load(XElement pXml)
		{
			ZendoUser pUser = new ZendoUser();
			pUser.Xml = pXml;
			return pUser;
		}
	}
		
	public class ZendoLogEvent
	{
		// construction
		public ZendoLogEvent() { }
		public ZendoLogEvent(string sMsg)
		{
			this.Message = sMsg;
			this.Time = DateTime.Now;
			this.Data = null;
		}
		public ZendoLogEvent(string sMsg, string sData)
		{
			this.Message = sMsg;
			this.Data = XElement.Parse(sData);
			this.Time = DateTime.Now;
		}
		public ZendoLogEvent(string sMsg, XElement pData)
		{
			this.Message = sMsg;
			this.Data = pData;
			this.Time = DateTime.Now;
		}
		
		// properties
		public string Message { get; set; } 
		public XElement Data { get; set; }
		public DateTime Time { get; set; }

		public XElement Xml
		{
			get
			{
				XElement pXml = new XElement("LogEvent");
				pXml.SetAttributeValue("Time", this.Time.ToString());
				pXml.SetElementValue("Message", this.Message);

				XElement pData = new XElement("Data");
				if (this.Data != null) 
				{
					if (this.Data.Name == "Data") { pData = this.Data; }
					else { pData.Add(this.Data); }
				}
				pXml.Add(pData);
				return pXml;
			}
			set
			{
				this.Time = DateTime.Parse(value.Attribute("Time").Value);
				this.Message = value.Element("Message").Value;
				this.Data = value.Element("Data");
			}
		}

		public static ZendoLogEvent Load(XElement pXml)
		{
			ZendoLogEvent pEvent = new ZendoLogEvent();
			pEvent.Xml = pXml;
			return pEvent;
		}
	}

	public class ZendoKoan
	{
		// construction
		public ZendoKoan() 
		{
			this.ID = 0;
			this.Koan = "";
			this.User = "";
			this.HasBuddhaNature = false;
		}
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
			set
			{
				this.ID = Convert.ToInt32(value.Attribute("ID").Value);
				this.User = value.Attribute("User").Value;
				this.HasBuddhaNature = Convert.ToBoolean(value.Attribute("BuddhaNature").Value);
				this.Koan = value.Value;
			}
		}

		public static ZendoKoan Load(XElement pXml)
		{
			ZendoKoan pKoan = new ZendoKoan();
			pKoan.Xml = pXml;
			return pKoan;
		}
	}
	
	public class Zendo
	{
		// member variables
		ClanServer m_pServer;
		
		private string m_sClanName;
		private List<string> m_lPlayerNames;
		
		private string m_sGameID;
		private string m_sGameName;

		private string m_sMaster;
		private List<ZendoUser> m_lStudents;
		private string m_sStateStatus; // "setup" (waiting for players to join), "initial" (waiting for Master's 2 koans), "open" (users can submit koans or guesses), "pending master" (waiting for master to analyze koan), "pending students" (mondo, waiting for majority of students to make their prediction), "pending disproval", "final" (game over, student has attained enlightenment)
		private List<ZendoLogEvent> m_lEventLog;
		private List<ZendoKoan> m_lKoans; // first character is T|F for whether has buddha nature. Then comma delimited character pairs (first character is color, second is direction)
		private ZendoKoan m_pPendingKoan;
		private bool m_bMondo;
		private Dictionary<string, bool> m_dMondoPredictions;
		private List<ZendoGuess> m_lGuesses;
		private ZendoGuess m_pPendingGuess;
		private string m_sRule;
		private string m_sWinningUser;
		private List<string> m_lUsersGivenUp; 
		
		// construction
		// (if id is passed in, load that game's xml)
		public Zendo() { }
		
		// properties
		public XElement StateXml // NOTE: this should only be used for literal saving and loading
		{
			get
			{
				XElement pXml = new XElement("Game");
				pXml.SetAttributeValue("ID", m_sGameID);
				pXml.SetAttributeValue("Name", m_sGameName);
				pXml.SetAttributeValue("ClanName", m_sClanName);
				pXml.SetAttributeValue("Master", m_sMaster);
				pXml.SetAttributeValue("StateStatus", m_sStateStatus);
				pXml.SetAttributeValue("Rule", m_sRule);
				pXml.SetAttributeValue("Mondo", m_bMondo);
				pXml.SetAttributeValue("WinningUser", m_sWinningUser);

				// pending
				XElement pPendingXml = new XElement("Pending");
				XElement pPendingKoan = new XElement("PendingKoan");
				pPendingKoan.Add(m_pPendingKoan.Xml);
				XElement pPendingGuess = new XElement("PendingGuess");
				pPendingGuess.Add(m_pPendingGuess.Xml);
				pPendingXml.Add(pPendingKoan);
				pPendingXml.Add(pPendingGuess);
				pXml.Add(pPendingXml);

				// player names
				XElement pUsersXml = new XElement("PlayerNames");
				foreach (string sName in m_lPlayerNames) { pUsersXml.Add(new XElement("PlayerName") { Value = sName }); }
				pXml.Add(pUsersXml);

				// students
				XElement pStudentsXml = new XElement("Students");
				foreach (ZendoUser pUser in m_lStudents) { pStudentsXml.Add(pUser.Xml); }
				pXml.Add(pStudentsXml);

				// log
				XElement pLogXml = new XElement("Log");
				foreach (ZendoLogEvent pEvent in m_lEventLog) { pLogXml.Add(pEvent.Xml); }
				pXml.Add(pLogXml);

				// koans
				XElement pKoansXml = new XElement("Koans");
				foreach (ZendoKoan pKoan in m_lKoans) { pKoansXml.Add(pKoan.Xml); }
				pXml.Add(pKoansXml);

				// predictions
				XElement pPredictionsXml = new XElement("Predictions");
				foreach (string sUser in m_dMondoPredictions.Keys)
				{
					XElement pPredictionXml = new XElement("Prediction");
					pPredictionXml.SetAttributeValue("User", sUser);
					pPredictionXml.SetAttributeValue("Value", m_dMondoPredictions[sUser]);
					pPredictionsXml.Add(pPredictionXml);
				}
				pXml.Add(pPredictionsXml);

				// guesses
				XElement pGuessesXml = new XElement("Guesses");
				foreach (ZendoGuess pGuess in m_lGuesses) { pGuessesXml.Add(pGuess.Xml); }
				pXml.Add(pGuessesXml);

				// votes to give up
				XElement pGivenUpXml = new XElement("GivenUp");
				foreach (string sUser in m_lUsersGivenUp)
				{
					XElement pGiveUpXml = new XElement("GiveUp");
					pGiveUpXml.SetValue(sUser);
					pGivenUpXml.Add(pGiveUpXml);
				}
				pXml.Add(pGivenUpXml);

				return pXml;
			}
			set // set all class properties from state assigned
			{
				// base stuff
				m_sGameID = value.Attribute("ID").Value;
				m_sGameName = value.Attribute("Name").Value;
				m_sClanName = value.Attribute("ClanName").Value;
				m_sMaster = value.Attribute("Master").Value;
				m_sStateStatus = value.Attribute("StateStatus").Value;
				m_sRule = value.Attribute("Rule").Value;
				m_bMondo = Convert.ToBoolean(value.Attribute("Mondo").Value);
				m_sWinningUser = value.Attribute("WinningUser").Value;

				// pending things
				XElement pPending = value.Element("Pending");
				m_pPendingGuess = new ZendoGuess();
				m_pPendingKoan = new ZendoKoan();
				m_pPendingGuess = ZendoGuess.Load(pPending.Element("PendingGuess").Element("Guess"));
				m_pPendingKoan = ZendoKoan.Load(pPending.Element("PendingKoan").Element("Koan"));

				// player names
				m_lPlayerNames = new List<string>();
				List<XElement> lNameXmls = value.Element("PlayerNames").Elements("PlayerName").ToList();
				foreach (XElement pNameXml in lNameXmls) { m_lPlayerNames.Add(pNameXml.Value); }

				// students
				m_lStudents = new List<ZendoUser>();
				List<XElement> lStudentXmls = value.Element("Students").Elements("Student").ToList();
				foreach (XElement pStudentXml in lStudentXmls) { m_lStudents.Add(ZendoUser.Load(pStudentXml)); }

				// log
				m_lEventLog = new List<ZendoLogEvent>();
				List<XElement> lLogXmls = value.Element("Log").Elements("LogEvent").ToList();
				foreach (XElement pLogXml in lLogXmls) { m_lEventLog.Add(ZendoLogEvent.Load(pLogXml)); }

				// koans
				m_lKoans = new List<ZendoKoan>();
				List<XElement> lKoanXmls = value.Element("Koans").Elements("Koan").ToList();
				foreach (XElement pKoanXml in lKoanXmls) { m_lKoans.Add(ZendoKoan.Load(pKoanXml)); }

				// predictions
				m_dMondoPredictions = new Dictionary<string, bool>();
				List<XElement> lPredictionXmls = value.Element("Predictions").Elements("Prediction").ToList();
				foreach (XElement pPredictionXml in lPredictionXmls) { m_dMondoPredictions.Add(pPredictionXml.Attribute("User").Value, Convert.ToBoolean(pPredictionXml.Attribute("Value").Value)); }

				// guesses
				m_lGuesses = new List<ZendoGuess>();
				List<XElement> lGuessXmls = value.Element("Guesses").Elements("Guess").ToList();
				foreach (XElement pGuessXml in lGuessXmls) { m_lGuesses.Add(ZendoGuess.Load(pGuessXml)); }

				// votes to give up
				m_lUsersGivenUp = new List<string>();
				List<XElement> lGiveUpXmls = value.Element("GivenUp").Elements("GiveUp").ToList();
				foreach (XElement pGiveUpXmls in lGiveUpXmls) { m_lUsersGivenUp.Add(pGiveUpXmls.Value); }
			}
		}

		// methods

		// outward facing methods

		public string CreateNewGame(string sClanName, string sUserName, string sUserPassPhrase, string sGameName)
		{
			this.Initialize();

			// verify user can create new game (part of this clan and valid password)
			if (!m_pServer.VerifyUserPassPhrase(sClanName, sUserName, sUserPassPhrase)) { return Master.MessagifyError("Invalid login."); }

			this.InitializeNewGame(sClanName, sGameName);

			m_pServer.AddActiveGame(sClanName, m_sGameID, "Zendo", sGameName);

			this.Save();
			return Master.MessagifySimple("Successfully started a new game of Zendo!");
		}

		public string JoinGame(string sGameID, string sClanName, string sUserName, string sUserPassPhrase)
		{
			string sResult = this.Prepare(sGameID, sClanName, sUserName, sUserPassPhrase, AuthenticationType.All);
			if (sResult != "") { return sResult; }

			// make sure user hasn't already joined
			if (m_lPlayerNames.Contains(sUserName)) { return Master.MessagifyError("You have already joined this game"); }
			
			this.AddUser(sUserName);
			this.Save();
			return Master.MessagifySimple("Successfully joined the Zendo game!");
		}

		public string StartGame(string sGameID)
		{
			this.Initialize();
			this.Load(sGameID);

			if (this.m_sStateStatus != "setup") { return Master.MessagifyError("The game has already been started."); }

			// choose a master if one wasn't already chosen
			if (m_sMaster == "" || m_sMaster == null) { m_sMaster = this.ChooseRandomName(); }
			this.m_sStateStatus = "initial";

			// make everyone else the students
			foreach (string sUser in m_lPlayerNames)
			{
				if (sUser == m_sMaster) { continue; }
				ZendoUser pUser = new ZendoUser(sUser);
				pUser.GuessingStones = 0;
				m_lStudents.Add(pUser);
			}

			// add event log and notify the master
			m_pServer.AddNotification(m_sClanName, m_sMaster, "You have been chosen as the master! Create your rule and initial koans!", m_sGameID, m_sGameName);
			m_lEventLog.Add(new ZendoLogEvent(m_sMaster + " has been chosen as the master"));

			this.Save();
			return Master.MessagifySimple("The Zendo game has been started!");
		}

		public string SubmitInitialKoans(string sGameID, string sClanName, string sUserName, string sUserPassPhrase, string sRule, string sBuddhaNatureKoan, string sNonBuddhaNatureKoan)
		{
			string sResult = this.Prepare(sGameID, sClanName, sUserName, sUserPassPhrase, AuthenticationType.Master);
			if (sResult != "") { return sResult; }

			// are we actually at this stage?
			if (m_sStateStatus != "initial")
			{
				if (m_sStateStatus == "setup") { return Master.MessagifyError("The game hasn't been started yet"); }
				return Master.MessagifyError("The game is no longer in the initial stage");
			}

			// set the rule
			m_sRule = sRule;
			
			// add the two koans
			ZendoKoan pBuddhaKoan = new ZendoKoan(0, sBuddhaNatureKoan, "master", true);
			ZendoKoan pNonBuddhaKoan = new ZendoKoan(1, sNonBuddhaNatureKoan, "master", false);
			m_lKoans.Add(pBuddhaKoan);
			m_lKoans.Add(pNonBuddhaKoan);

			// open up the game status
			m_sStateStatus = "open";

			// send notifications to all the students and add to the event log
			//foreach (string sUser in m_lStudents) { m_pServer.AddNotification(m_sClanName, sUser, "The game has started and the master has built the initial koans!"); }
			foreach (ZendoUser pUser in m_lStudents) { m_pServer.AddNotification(m_sClanName, pUser.UserName, "The game has started and the master has built the initial koans!", m_sGameID, m_sGameName); }
			m_lEventLog.Add(new ZendoLogEvent("The master has built the initial koans"));

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

			// set the game status
			m_sStateStatus = "pending master";

			// send a notification to the master and add to the event log
			m_pServer.AddNotification(m_sClanName, m_sMaster, "A student has built a new koan for you to analyze.", m_sGameID, m_sGameName);
			m_lEventLog.Add(new ZendoLogEvent(sUserName + " has built a new koan", m_pPendingKoan.Xml));

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
				m_pServer.AddNotification(m_sClanName, pUser.UserName, sUserName + " has called mondo on a koan. Hurry and make your prediction!", m_sGameID, m_sGameName);
			}
			m_lEventLog.Add(new ZendoLogEvent(sUserName + " has called mondo on their koan", m_pPendingKoan.Xml));

			this.Save();
			return "";
		}

		public string SubmitMondoPrediction(string sGameID, string sClanName, string sUserName, string sUserPassPhrase, bool bPrediction)
		{
			string sResult = this.Prepare(sGameID, sClanName, sUserName, sUserPassPhrase, AuthenticationType.Student);
			if (sResult != "") { return sResult; }

			// make sure a mondo has actually been called!
			if (!m_bMondo) { return Master.MessagifyError("No mondo has been called"); }

			m_dMondoPredictions[sUserName] = bPrediction;

			// if half of the students have made their predictions, move the game state to pending master 
			if (m_dMondoPredictions.Count >= Math.Ceiling((double)m_lStudents.Count / 2))
			{
				m_sStateStatus = "pending master";
				// send a notification to the master 
				m_pServer.AddNotification(m_sClanName, m_sMaster, "A student has called mondo on a new koan, and the students have made their predictions. The koan is ready for your analysis!", m_sGameID, m_sGameName);
			}

			this.Save();
			return "";
		}

		public string GetMasterRule(string sGameID, string sClanName, string sUserName, string sUserPassPhrase)
		{
			string sResult = this.Prepare(sGameID, sClanName, sUserName, sUserPassPhrase, AuthenticationType.Master);
			if (sResult != "") { return sResult; }

			return Master.MessagifySimple(m_sRule);
		}

		public string SubmitPendingKoanAnalysis(string sGameID, string sClanName, string sUserName, string sUserPassPhrase, bool bHasBuddhaNature)
		{
			string sResult = this.Prepare(sGameID, sClanName, sUserName, sUserPassPhrase, AuthenticationType.Master);
			if (sResult != "") { return sResult; }

			// make sure the state is 'pending master'
			if (m_sStateStatus != "pending master") { return Master.MessagifyError("There is no pending koan for you to analyze"); }

			// get the specified koan
			ZendoKoan pKoan = m_pPendingKoan;

			// set the koans buddha-nature and add it to the list
			pKoan.HasBuddhaNature = bHasBuddhaNature;
			pKoan.Koan = pKoan.StringKoan;
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

						m_pServer.AddNotification(sClanName, sUser, "The master has answered the mondo, your prediction was correct!", m_sGameID, m_sGameName);
					}
					else { m_pServer.AddNotification(sClanName, sUser, "The master has answered the mondo, your prediction was incorrect.", m_sGameID, m_sGameName); }
				}
				m_bMondo = false;
			}

			// clear the pending koan and set the status back to open
			m_sStateStatus = "open";

			// notify the user who submitted the koan and add this to the event log
			string sKeyPhrase = (bHasBuddhaNature) ? "has" : "does not have";
			m_pServer.AddNotification(m_sClanName, pKoan.User, "The master has declared that your koan " + sKeyPhrase + " the buddha-nature", m_sGameID, m_sGameName);
			m_lEventLog.Add(new ZendoLogEvent("The master has declared that this koan " + sKeyPhrase + " the buddha-nature", pKoan.Xml));

			this.Save();
			return "";
		}

		public string SubmitGuess(string sGameID, string sClanName, string sUserName, string sUserPassPhrase, string sGuess)
		{
			string sResult = this.Prepare(sGameID, sClanName, sUserName, sUserPassPhrase, AuthenticationType.Student);
			if (sResult != "") { return sResult; }

			// make sure the state status is open
			if (m_sStateStatus != "open")
			{
				if (m_sStateStatus == "pending master") { return Master.MessagifyError("The master has not analyzed the current pending koan"); }
				if (m_sStateStatus == "pending students") { return Master.MessagifyError("Mondo has been called on the pending koan"); }
				if (m_sStateStatus == "pending disproval") { return Master.MessagifyError("The master is currently attempting to disprove the current guess"); }
				return Master.MessagifyError("The game is not currently accepting koan submissions");
			}

			// make the guess
			ZendoGuess pGuess = new ZendoGuess(sGuess, sUserName);
			pGuess.Time = DateTime.Now;
			m_pPendingGuess = pGuess;

			// take away a guessing stone
			foreach (ZendoUser pUser in m_lStudents)
			{
				if (pUser.UserName == sUserName) { pUser.GuessingStones--; break; }
			}

			// set the status, add an event log and notify the master
			m_sStateStatus = "pending disproval";
			m_lEventLog.Add(new ZendoLogEvent(sUserName + " has made a guess: '" + sGuess + "'"));
			m_pServer.AddNotification(sClanName, sUserName, sUserName + " has submitted a guess. Go disprove it!", m_sGameID, m_sGameName);

			this.Save();
			return "";
		}

		public string DisproveGuess(string sGameID, string sClanName, string sUserName, string sUserPassPhrase, string sKoan, bool bHasBuddhaNature)
		{
			string sResult = this.Prepare(sGameID, sClanName, sUserName, sUserPassPhrase, AuthenticationType.Master);
			if (sResult != "") { return sResult; }
			
			// check game state
			if (m_sStateStatus != "pending disproval") { return Master.MessagifyError("There is no pending guess right now"); }

			// add the koan disprovivng the guess
			ZendoKoan pKoan = new ZendoKoan(m_lKoans.Count, sKoan, "master");
			pKoan.Koan = pKoan.StringKoan;
			pKoan.HasBuddhaNature = bHasBuddhaNature;
			m_pPendingGuess.Disproval = pKoan;

			m_lKoans.Add(pKoan);
			m_lGuesses.Add(m_pPendingGuess);
			//m_pPendingGuess = null;

			// change the state
			m_sStateStatus = "open";

			// notify the user who submitted the guess and add an event log
			m_pServer.AddNotification(sClanName, m_pPendingGuess.User, "The master has disproven your guess", m_sGameID, m_sGameName);
			m_lEventLog.Add(new ZendoLogEvent("The master has disproven " + m_pPendingGuess.User + "'s guess", pKoan.Xml));

			this.Save();
			return "";
		}

		public string GrantEnlightenment(string sGameID, string sClanName, string sUserName, string sUserPassPhrase)
		{
			string sResult = this.Prepare(sGameID, sClanName, sUserName, sUserPassPhrase, AuthenticationType.Master);
			if (sResult != "") { return sResult; }

			// make sure the state was right
			if (m_sStateStatus != "pending disproval") { return Master.MessagifyError("There is no pending guess right now"); }

			// notify the user that they won and add an event log
			m_pServer.AddNotification(sClanName, m_pPendingGuess.User, "The master was unable to disprove your guess. You have reached enlightenment!", m_sGameID, m_sGameName);
			m_lEventLog.Add(new ZendoLogEvent("The maser was unable to disprove " + m_pPendingGuess.User + "'s guess. " + m_pPendingGuess.User + " has become enlightened!"));

			m_sWinningUser = m_pPendingGuess.User;
			this.GameOver();

			this.Save();
			return "";
		}

		public string VoteToGiveUp(string sGameID, string sClanName, string sUserName, string sUserPassPhrase)
		{
			string sResult = this.Prepare(sGameID, sClanName, sUserName, sUserPassPhrase, AuthenticationType.Student);
			if (sResult != "") { return sResult; }

			// add the vote to the list
			if (!m_lUsersGivenUp.Contains(sUserName))
			{
				m_lUsersGivenUp.Add(sUserName);

				// add to the event log
				m_lEventLog.Add(new ZendoLogEvent(sUserName + " has voted to give up"));
			}

			// if unanimous, end the game
			if (m_lUsersGivenUp.Count == m_lStudents.Count) { this.GameOver(); }

			this.Save();
			return "";
		}

		// message format:
		// <Status></Status>
		// <Action></Action> (join, waiting, initial, open, build, judge, predcit, disprove, final)
		// <NumGuesses></NumGuesses>
		// <Master></Master>
		// <Players>
		//		<Player>...
		// </Players>
		// <Events>
		//		<Event>...
		// </Events>
		// <Koans>
		//		<Koan>...
		// </Koans>
		// this function should be called for obtaining the game-dashboard. All pertinent game data is contained, as well as suggested/possible client action
		public string GetUserBoard(string sGameID, string sClanName, string sUserName, string sUserPassPhrase)
		{
			string sResult = this.Prepare(sGameID, sClanName, sUserName, sUserPassPhrase, AuthenticationType.All);
			if (sResult != "") { return sResult; }

			string sAction = "";
			string sStatus = "";
			string sStatusData = "";

			// switches based on the game status and whether this player is playing or not and what their role is
			if (m_sStateStatus == "setup")
			{
				// if user hasn't joined
				if (!m_lPlayerNames.Contains(sUserName))
				{
					sAction = "join";
					sStatus = "This game hasn't started yet. Hit the join button below to join in the fun!";
				}
				else
				{
					sAction = "waiting";
					sStatus = "You have joined this game! Waiting for the game to start...";
				}
			}
			else if (m_sStateStatus == "initial")
			{
				if (m_sMaster == sUserName)
				{
					sAction = "initial";
					sStatus = "The game has started, and you are the master! Create your rule and build the initial koans!";
				}
				else
				{
					sAction = "waiting";
					sStatus = "The game has started, waiting for the master to create his rule and submit the initial koans...";
				}
			}
			else if (m_sStateStatus == "open")
			{
				if (m_sMaster == sUserName)
				{
					sAction = "waiting";
					sStatus = "Waiting for a student to build a koan or guess...";
				}
				else
				{
					sAction = "build";
					sStatus = "Build a koan and call either master or mondo!";

					// if only dealing with the initial two koans, display them to students
					if (m_lKoans.Count == 2) { sStatusData += m_lKoans[0].Xml.ToString() + m_lKoans[1].Xml.ToString(); }
				}
			}
			else if (m_sStateStatus == "pending master")
			{
				sStatusData = m_pPendingKoan.Xml.ToString();
				if (m_sMaster == sUserName)
				{
					sAction = "judge";
					sStatus = m_pPendingKoan.User + " has built a koan. Decide if it has the buddha-nature or not!";
				}
				else
				{
					if (!m_bMondo)
					{
						sAction = "waiting";
						sStatus = "Waiting for the master to decide if " + m_pPendingKoan.User + "'s koan has the buddha-nature...";
					}
					else
					{
						if (!m_dMondoPredictions.Keys.Contains(sUserName))
						{
							sAction = "predict";
							sStatus = m_pPendingKoan.User + " called mondo on their koan, but the master can now analyze it. Hurry and submit your prediction!";
						}
						else
						{
							sAction = "predict";
							string sPhrase = (m_dMondoPredictions[sUserName]) ? "has" : "does not have";
							sStatus = "You predicted that the master will say this koan " + sPhrase + " the buddha-nature. Waiting for the master to decide...";
						}
					}
				}
			}
			else if (m_sStateStatus == "pending students")
			{
				sStatusData = m_pPendingKoan.Xml.ToString(); 
				if (m_sMaster == sUserName)
				{
					sAction = "waiting";
					sStatus = m_pPendingKoan.User + " called mondo on their koan. Waiting for students to submit their predictions...";
				}
				else
				{
					if (!m_dMondoPredictions.Keys.Contains(sUserName))
					{
						sAction = "predict";
						sStatus = m_pPendingKoan.User + " called mondo on their koan. Predict whether the master will say this has the buddha-nature or not!";
					}
					else
					{
						sAction = "predict";
						string sPhrase = (m_dMondoPredictions[sUserName]) ? "has" : "does not have";
						sStatus = "You predicted that the master will say this koan " + sPhrase + " the buddha-nature.";
					}
				}
			}
			else if (m_sStateStatus == "pending disproval")
			{
				if (m_sMaster == sUserName)
				{
					sAction = "disprove";
					sStatus = m_pPendingGuess.User + " submitted a guess: '" + m_pPendingGuess.Guess + "'\nDisprove their guess or grant them enlightenment!";
				}
				else
				{
					sAction = "waiting";
					sStatus = "Waiting for the master to attempt to disprove " + m_pPendingGuess.User + "'s guess: '" + m_pPendingGuess.Guess + "'...";
				}
			}
			else if (m_sStateStatus == "final")
			{
				sAction = "final";
				string sPhrase = (m_sWinningUser != null && m_sWinningUser != "") ? m_sWinningUser + " attained enlightenment!" : "the students voted to give up, and no one has reached enlightenment. :(";
				sStatus = "Game over, " + sPhrase;
			}

			// get number of guessing stones
			int iNumStones = 0;
			foreach (ZendoUser pUser in m_lStudents)
			{
				if (pUser.UserName == sUserName) { iNumStones = pUser.GuessingStones; }
			}

			// build players list
			string sPlayers = "<Players>";
			foreach (string sPlayer in m_lPlayerNames) { sPlayers += "<Player>" + sPlayer + "</Player>"; }
			sPlayers += "</Players>";

			// master stuff
			string sMaster = "<Master";
			if (m_sMaster == sUserName) { sMaster += " Rule='" + m_sRule + "'"; }
			sMaster += ">" + m_sMaster + "</Master>";

			// if pending guess
			string sEntireStatus = "<Status";
			if (m_sStateStatus == "pending disproval" && m_sMaster == sUserName) { sEntireStatus += " Guess='" + Master.EncodeXML(m_pPendingGuess.Guess) + "'"; }
			sEntireStatus += "><Text>" + sStatus + "</Text><Data>" + sStatusData + "</Data></Status>";

			// return ze data
			string sAllData = sEntireStatus + "<Action>" + sAction + "</Action>" + sMaster + "<NumGuesses>" + iNumStones.ToString() + "</NumGuesses>" + sPlayers + this.GetKoansXml().ToString() + this.GetLogXml().ToString();
			return Master.MessagifyData(sAllData);
		}


		// inner methods

		private XElement GetKoansXml()
		{
			XElement pKoans = new XElement("Koans");
			foreach (ZendoKoan pKoan in m_lKoans) { pKoans.Add(pKoan.Xml); }
			return pKoans;
		}

		private XElement GetLogXml()
		{
			XElement pLog = new XElement("Events");
			foreach (ZendoLogEvent pEvent in m_lEventLog) { pLog.Add(pEvent.Xml); }
			return pLog;
		}

		private void GameOver()
		{
			m_sStateStatus = "final";

			if (m_sWinningUser == "" && m_sWinningUser != null)
			{
				// winner
			}
			else
			{
				// given up
			}
		}

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

		private void InitializeNewGame(string sClanName, string sGameName)
		{
			m_sGameID = "g_Zendo_" + DateTime.Now.Ticks.ToString();
			m_sClanName = sClanName;
			m_sGameName = sGameName;
			m_sStateStatus = "setup";

			m_lPlayerNames = new List<string>();
			m_lStudents = new List<ZendoUser>();
			m_lKoans = new List<ZendoKoan>();
			m_lEventLog = new List<ZendoLogEvent>();
			m_pPendingKoan = new ZendoKoan();
			m_pPendingGuess = new ZendoGuess();
			m_lUsersGivenUp = new List<string>();
			m_bMondo = false;
			m_dMondoPredictions = new Dictionary<string, bool>();
			m_sRule = "";
			m_sWinningUser = "";
			m_lGuesses = new List<ZendoGuess>();
			m_lUsersGivenUp = new List<string>();
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
