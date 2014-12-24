using System;
using ChiamataLibrary;
using System.Collections.Generic;

namespace ConsolePerDebug
{
	class MainClass
	{
		public static void Main (string [] args)
		{

			Board board = new Board ();	
			board.initialize (new string[]{ "A", "B", "C", "D", "E" }, 2);//il mazziere è C

			board.Players.ForEach (delegate(Player p) {
				Console.WriteLine (p.ToString () + " possiede:");
				board.getPlayerHand (p).ForEach (delegate(Card c) {
					Console.WriteLine (c.ToString ());
				});
			});


			IBid wb = null;

			while (!board.isAuctionClosed) {

				if (wb == null)
					Console.WriteLine ("non c'è nessuna bid");
				else
					Console.WriteLine ("offerta vincente:" + wb.ToString ());

				Console.WriteLine (board.ActiveAuctionPlayer.ToString () + " deve fare una offerta");

				Console.WriteLine ("passi?");
				if (Console.ReadLine () == "0")
					board.auctionPass (board.ActiveAuctionPlayer);
				else {
					Console.WriteLine ("numero:");
					EnNumbers n = (EnNumbers) int.Parse (Console.ReadLine ());
					Console.WriteLine ("punti:");
					int p = int.Parse (Console.ReadLine ());
					NormalBid nb = new NormalBid (board.ActiveAuctionPlayer, n, p);
					if (board.isBidBetter (nb))
						board.auctionPlaceABid (nb);
				}

				wb = board.currentAuctionWinningBid;

			}

			if (wb == null)
				Console.WriteLine ("monte");

			Console.WriteLine (wb.Bidder + " ha vinto l'asta e deve scegliere il seme");
			board.finalizeAuction ((EnSemi) int.Parse (Console.ReadLine ()));

			Console.WriteLine ("tipo partita: " + board.GameType.ToString ());
			Console.WriteLine ("Chiamante: " + board.PlayerChiamante.ToString ());
			Console.WriteLine ("Socio: " + board.PlayerChiamante.ToString ());

			foreach (Player p in board.PlayerAltri)
				Console.WriteLine ("Altro: " + p.ToString ());


		}
	}
}
