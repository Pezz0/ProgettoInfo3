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

			board.AllPlayers.ForEach (delegate(Player p) {
				Console.WriteLine (p.ToString () + " possiede:");
				board.getPlayerHand (p).ForEach (delegate(Card c) {
					Console.WriteLine (c.ToString ());
				});
			});


			IBid wb = null;

			while (!board.isAuctionClosed) {

				Console.WriteLine ("------------------------------");
				if (wb == null)
					Console.WriteLine ("Non c'è nessuna bid");
				else
					Console.WriteLine ("Offerta vincente:" + wb.ToString ());

				Console.WriteLine (board.ActiveAuctionPlayer.ToString () + " deve fare una offerta [passo=p; carichi=c, normale=qualsiasi altra cosa]");

				string a = Console.ReadLine ();
				IBid bid = null;

				if (a == "p")
					board.auctionPass (board.ActiveAuctionPlayer);
				else if (a == "c") {
					Console.WriteLine ("Punti:");
					bid = new BidCarichi (board.ActiveAuctionPlayer, int.Parse (Console.ReadLine ()));
				} else {
					Console.WriteLine ("Numero[0=due,...,8=tre,9=asse]:");
					EnNumbers n = (EnNumbers) int.Parse (Console.ReadLine ());
					Console.WriteLine ("Punti:");
					int p = int.Parse (Console.ReadLine ());
					bid = new NormalBid (board.ActiveAuctionPlayer, n, p);
				}

				if (board.isBidBetter (bid)) {
					board.auctionPlaceABid (bid);
					wb = bid;	//la winning bid è sempre quella nuova oppure c'è il metodo che da fa la stessa cosa.
					//wb = board.currentAuctionWinningBid;
				}
			}

			if (wb == null)
				board.finalizeAuction (EnSemi.COPE);
			else {
				Console.WriteLine (wb.Bidder + " ha vinto l'asta e deve scegliere il seme[0=ori ,1=cope 2=bastoni , 3=spade]");
				board.finalizeAuction ((EnSemi) int.Parse (Console.ReadLine ()));
			}

			if (board.GameType == EnGameType.MONTE)
				Console.WriteLine ("Hanno passato tutti senza offerte e quindi la partita va a monte");
			else if (board.GameType == EnGameType.CARICHI)
				Console.WriteLine ("Partita a carichi, chiamante:" + board.PlayerChiamante.ToString ());
			else if (board.GameType == EnGameType.STANDARD) {
				Console.WriteLine ("Partita standard");
				Console.WriteLine ("Chiamante: " + board.PlayerChiamante.ToString ());
				if (board.isChiamataInMano)
					Console.WriteLine ("Chiamata in mano");
				else
					Console.WriteLine ("Socio: " + board.PlayerChiamante.ToString ());

				foreach (Player p in board.PlayerAltri)
					Console.WriteLine ("Altro: " + p.ToString ());
			}

			Console.WriteLine ("********************************");
			Console.WriteLine ("inizia la partita.");
			Console.WriteLine ("********************************");

			while (!board.isGameFinish) {

				Console.WriteLine ("------------------------------");

				Console.WriteLine ("carte in banco:");
				board.CardOnTheBoard.ForEach (delegate(Card c) {
					Console.WriteLine (c.ToString ());
				});
				Console.WriteLine ("");
				Console.WriteLine (board.ActivePlayer.ToString () + " deve giocare:");

				List<Card> mano = board.ActivePlayer.Hand;
				//List<Card> mano= board.getPlayerHand(board.ActivePlayer); sono equivalenti

				for (int i = 0; i < mano.Count; i++)
					Console.WriteLine ("premere " + i.ToString () + " per giocare " + mano [i].ToString ());

				board.PlayACard (board.ActivePlayer, mano [int.Parse (Console.ReadLine ())]);

				if (board.numberOfCardOnBoard == 0)
					Console.WriteLine (board.LastWinner.ToString () + " ha preso il turno");
			}

			Console.WriteLine ("Partita finita");
			Console.WriteLine ("Il chiamante ha fatto: " + board.getChiamantePointCount ().ToString () + " punti");
			Console.WriteLine ("gli altri hanno fatto: " + board.getAltriPointCount ().ToString () + " punti");

			Console.WriteLine ("Il chiamante deve fare " + board.WinningPoint.ToString () + " punti, quindi i vincitori sono:");
			board.Winner.ForEach (delegate(Player p) {
				Console.WriteLine (p.ToString ());
			});

		}
	}
}
