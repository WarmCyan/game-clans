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

			/*string sMsg = CreateClan();
			Console.WriteLine(sMsg);*/

			/*string sMsg = JoinClan("WildfireXIII", "password");
			Console.WriteLine(sMsg);*/
			
			string sMsg = urlJoinClan("WildfireXIII", "password");
			Console.WriteLine(sMsg);

			/*string sMsg = JoinClan("Dude", "testing");
			Console.WriteLine(sMsg);*/

			/*string sMsg = JoinClan("SomeGuy", "testing");
			Console.WriteLine(sMsg);*/

			/*string sMsg = CreateZendoGame();
			Console.WriteLine(sMsg);*/

			/*string sMsg = JoinGame("WildfireXIII", "password", "g_Zendo_636167986558817917");
			Console.WriteLine(sMsg);*/

			Console.WriteLine("\nComplete!");
			Console.Read();
		}

		static string CreateZendoGame()
		{
			Zendo z = new Zendo();
			return z.CreateNewGame("Testing Clan", "WildfireXIII", "password");
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
