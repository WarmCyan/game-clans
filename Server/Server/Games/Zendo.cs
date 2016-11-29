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

		// construction
		// (if id is passed in, load that game's xml)
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

		// methods
		public void StartNewGame() { }
		public void StartNewGame(string sMasterUsername) // for starting a game with the winner of a previous game as the master for this one
		{
			
		}
	}
}
