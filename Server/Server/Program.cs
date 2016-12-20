using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameClansServer.Games;

using DWL.Utility;

namespace GameClansServer 
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello world!\n");

			/*string sMsg = CreateClan();*/

			/*string sMsg = JoinClan("WildfireXIII", "password");*/

			/*string sMsg = urlJoinClan("WildfireXIII", "password");*/

			/*string sMsg = JoinClan("Dude", "testing");*/

			//string sMsg = JoinClan("SomeGuy", "testing");

			string sMsg = CreateZendoGame();

			/*string sMsg = JoinGame("WildfireXIII", "password", "g_Zendo_636167986558817917");*/

			/*string sMsg = GetClanLeaderboard("WildfireXIII", "testing");*/

			/*string sMsg = StartZendoGame();*/

			//string sMsg = GetGameBoard();

			Console.WriteLine(sMsg);
			
			Console.WriteLine("\nComplete!");
			Console.Read();
		}

		static string GetGameBoard()
		{
			Zendo z = new Zendo();
			ClanServer cs = new ClanServer();
			return z.GetUserBoard("g_Zendo_636178400449774608", "Testing Clan", "WildfireXIII", cs.Sha256Hash("testing"));
		}


		static string StartZendoGame()
		{
			Zendo z = new Zendo();
			return z.StartGame("g_Zendo_636178400449774608");
		}
		
		static string CreateZendoGame()
		{
			Zendo z = new Zendo();
			ClanServer cs = new ClanServer();
			return z.CreateNewGame("Testing Clan", "WildfireXIII", cs.Sha256Hash("testing"));
		}
		
		static string JoinGame(string sUserName, string sPassword, string sGameID)
		{
			Zendo z = new Zendo();
			return z.JoinGame(sGameID, "Testing Clan", sUserName, sPassword);
		}

		static string StartGame(string sGameID)
		{
			Zendo z = new Zendo();
			return z.StartGame(sGameID);
		}

		static string CreateClan()
		{
			ClanServer cs = new ClanServer();
			return cs.CreateClan("Testing Clan", "testing");
		}

		static string JoinClan(string sUserName, string sPassword)
		{
			ClanServer cs = new ClanServer();
			return cs.JoinClan("Testing Clan", "testing", sUserName, sPassword);
		}

		static string GetClanLeaderboard(string sUserName, string sPassword)
		{
			ClanServer cs = new ClanServer();
			return cs.GetClanLeaderboard("Testing Clan", sUserName, cs.Sha256Hash(sPassword));
		}

		static string urlJoinClan(string sUserName, string sPassword)
		{


			string sClan = "Testing Clan";
			string sClanPass = "testing";
		
			string sBody = "<params><param name='sClanName'>" + sClan + "</param><param name='sClanPassPhrase'>" + sClanPass + "</param><param name='sUserName'>" + sUserName + "</param><param name='sUserPassPhrase'>" + sPassword + "</param></params>";
			string sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/GameClansServer/GameClansServer/ClanServer/JoinClan", sBody, true);

			return sResponse;
		}

	}
}
