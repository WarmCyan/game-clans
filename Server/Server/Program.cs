using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClansServer 
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello world!\n");

			/*string sMsg = CreateClan();
			Console.WriteLine(sMsg);*/

			/*string sMsg = JoinClan();
			Console.WriteLine(sMsg);*/

			Console.WriteLine("\nComplete!");
			Console.Read();
		}

		static string CreateClan()
		{
			ClanServer cs = new ClanServer();
			return cs.CreateClan("Testing Clan", "testing");
		}

		static string JoinClan()
		{
			ClanServer cs = new ClanServer();
			return cs.JoinClan("Testing Clan", "testing", "WildfireXIII", "password");
		}
	}
}
