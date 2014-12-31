using System;
using ChiamataLibrary;
using System.Collections.Generic;

namespace ConsolePerDebug
{
	class MainClass
	{
		public static void Main (string [] args)
		{
			Console.Write ("Scegliere la modalità[0=normale,1=tutti tranne te IA]: ");

			string a = Console.ReadLine ();

			if (a == "0")
				mainStandard ();
			else if (a == "1")
				mainIA ();
		}

	
		private static void mainStandard ()
		{
			Board.Instance.initializeMaster (new string[]{ "A", "B", "C", "D", "E" }, 2);//il mazziere è C

			Board.Instance.AllPlayers.ForEach (delegate(Player p) {
				Console.WriteLine (p.ToString () + " possiede:");
				Board.Instance.getPlayerHand (p).ForEach (delegate(Card c) {
					Console.WriteLine (c.ToString ());
				});
			});


			IBid wb = null;

			while (!Board.Instance.isAuctionClosed) {

				Console.WriteLine ("------------------------------");
				if (wb == null)
					Console.WriteLine ("Non c'è nessuna bid");
				else
					Console.WriteLine ("Offerta vincente:" + wb.ToString ());

				Console.WriteLine (Board.Instance.ActiveAuctionPlayer.ToString () + " deve fare una offerta [passo=p; carichi=c, normale=qualsiasi altra cosa]");

				string a = Console.ReadLine ();
				IBid bid = null;

				if (a == "p")
					Board.Instance.auctionPass (Board.Instance.ActiveAuctionPlayer);
				else if (a == "c") {
					Console.Write ("Punti: ");
					bid = new BidCarichi (Board.Instance.ActiveAuctionPlayer, int.Parse (Console.ReadLine ()));
				} else {
					Console.Write ("Numero[0=due,...,8=tre,9=asse]: ");
					EnNumbers n = (EnNumbers) int.Parse (Console.ReadLine ());
					Console.Write ("Punti: ");
					int p = int.Parse (Console.ReadLine ());
					bid = new NormalBid (Board.Instance.ActiveAuctionPlayer, n, p);
				}

				if (Board.Instance.isBidBetter (bid)) {
					Board.Instance.auctionPlaceABid (bid);
					wb = bid;	//la winning bid è sempre quella nuova oppure c'è il metodo che da fa la stessa cosa.
					//wb = board.currentAuctionWinningBid;
				}
			}

			if (wb == null)
				Board.Instance.finalizeAuction (EnSemi.COPE);
			else {
				Console.WriteLine (wb.bidder + " ha vinto l'asta e deve scegliere il seme[0=ori ,1=cope 2=bastoni , 3=spade]");
				Board.Instance.finalizeAuction ((EnSemi) int.Parse (Console.ReadLine ()));
			}

			if (Board.Instance.GameType == EnGameType.MONTE)
				Console.WriteLine ("Hanno passato tutti senza offerte e quindi la partita va a monte");
			else if (Board.Instance.GameType == EnGameType.CARICHI)
				Console.WriteLine ("Partita a carichi, chiamante:" + Board.Instance.PlayerChiamante.ToString ());
			else if (Board.Instance.GameType == EnGameType.STANDARD) {
				Console.WriteLine ("Partita standard");
				Console.WriteLine ("Chiamante: " + Board.Instance.PlayerChiamante.ToString ());
				if (Board.Instance.isChiamataInMano)
					Console.WriteLine ("Chiamata in mano");
				else
					Console.WriteLine ("Socio: " + Board.Instance.PlayerChiamante.ToString ());

				foreach (Player p in Board.Instance.PlayerAltri)
					Console.WriteLine ("Altro: " + p.ToString ());
			}

			Console.WriteLine ("********************************");
			Console.WriteLine ("inizia la partita.");
			Console.WriteLine ("********************************");

			while (!Board.Instance.isGameFinish) {

				Console.WriteLine ("------------------------------");

				Console.WriteLine ("carte in banco:");
				Board.Instance.CardOnTheBoard.ForEach (delegate(Card c) {
					Console.WriteLine (c.ToString ());
				});
				Console.WriteLine ("");
				Console.WriteLine (Board.Instance.ActivePlayer.ToString () + " deve giocare:");

				List<Card> mano = Board.Instance.ActivePlayer.Hand;
				//List<Card> mano= board.getPlayerHand(board.ActivePlayer); sono equivalenti

				for (int i = 0; i < mano.Count; i++)
					Console.WriteLine ("premere " + i.ToString () + " per giocare " + mano [i].ToString ());

				Board.Instance.PlayACard (Board.Instance.ActivePlayer, mano [int.Parse (Console.ReadLine ())]);

				if (Board.Instance.numberOfCardOnBoard == 0)
					Console.WriteLine (Board.Instance.LastWinner.ToString () + " ha preso il turno");
			}

			Console.WriteLine ("Partita finita");
			Console.WriteLine ("Il chiamante ha fatto: " + Board.Instance.getChiamantePointCount ().ToString () + " punti");
			Console.WriteLine ("gli altri hanno fatto: " + Board.Instance.getAltriPointCount ().ToString () + " punti");

			Console.WriteLine ("Il chiamante deve fare " + Board.Instance.WinningPoint.ToString () + " punti, quindi i vincitori sono:");
			Board.Instance.Winner.ForEach (delegate(Player p) {
				Console.WriteLine (p.ToString ());
			});

		}

		private static void mainIA ()
		{
			Board.Instance.eventSomeonePlaceABid += mssBid;
			Board.Instance.eventSomeonePlayACard += mssMove;
			Board.Instance.eventPickTheBoard += mssPick;

			IAIPlayer ia1 = new StupidAI (1);
			IAIPlayer ia2 = new StupidAI (2);
			IAIPlayer ia3 = new StupidAI (3);
			IAIPlayer ia4 = new StupidAI (4);

			Board.Instance.initializeMaster (new string[]{ "A", "B", "C", "D", "E" }, 2);//il mazziere è C
	
			Console.WriteLine ("possiedi:");
			Board.Instance.getPlayerHand (Board.Instance.Me).ForEach (delegate(Card c) {
				Console.WriteLine (c.ToString ());
			});


			while (!Board.Instance.isAuctionClosed) {
				if (Board.Instance.ActiveAuctionPlayer == Board.Instance.Me) {
					Console.WriteLine (Board.Instance.ActiveAuctionPlayer.ToString () + " deve fare una offerta [passo=p; carichi=c, normale=qualsiasi altra cosa]");

					string a = Console.ReadLine ();
					IBid bid = null;

					if (a == "p")
						Board.Instance.auctionPass (Board.Instance.ActiveAuctionPlayer);
					else if (a == "c") {
						Console.Write ("Punti: ");
						bid = new BidCarichi (Board.Instance.ActiveAuctionPlayer, int.Parse (Console.ReadLine ()));
					} else {
						Console.Write ("Numero[0=due,...,8=tre,9=asse]: ");
						EnNumbers n = (EnNumbers) int.Parse (Console.ReadLine ());
						Console.Write ("Punti: ");
						int p = int.Parse (Console.ReadLine ());
						bid = new NormalBid (Board.Instance.ActiveAuctionPlayer, n, p);
						Board.Instance.auctionPlaceABid (bid);
					}
				}
			}

			if (Board.Instance.currentAuctionWinningBid.bidder == Board.Instance.Me) {
				Console.WriteLine ("hai vinto l'asta e deve scegliere il seme[0=ori ,1=cope 2=bastoni , 3=spade]");
				Board.Instance.finalizeAuction ((EnSemi) int.Parse (Console.ReadLine ()));
			}

			if (Board.Instance.GameType == EnGameType.MONTE)
				Console.WriteLine ("Hanno passato tutti senza offerte e quindi la partita va a monte");
			else if (Board.Instance.GameType == EnGameType.CARICHI)
				Console.WriteLine ("Partita a carichi, chiamante:" + Board.Instance.PlayerChiamante.ToString ());
			else if (Board.Instance.GameType == EnGameType.STANDARD) {
				Console.WriteLine ("Partita standard");
				Console.WriteLine ("Chiamante: " + Board.Instance.PlayerChiamante.ToString ());
				if (Board.Instance.isChiamataInMano)
					Console.WriteLine ("Chiamata in mano");
				else
					Console.WriteLine ("Socio: " + Board.Instance.PlayerChiamante.ToString ());

				foreach (Player p in Board.Instance.PlayerAltri)
					Console.WriteLine ("Altro: " + p.ToString ());
			}

			Console.WriteLine ("********************************");
			Console.WriteLine ("inizia la partita.");
			Console.WriteLine ("********************************");

			while (!Board.Instance.isGameFinish) {

				if (Board.Instance.ActivePlayer == Board.Instance.Me) {
					List<Card> mano = Board.Instance.ActivePlayer.Hand;
					for (int i = 0; i < mano.Count; i++)
						Console.WriteLine ("premere " + i.ToString () + " per giocare " + mano [i].ToString ());

					Board.Instance.PlayACard (Board.Instance.ActivePlayer, mano [int.Parse (Console.ReadLine ())]);
				}
			}

			Console.WriteLine ("Partita finita");
			Console.WriteLine ("Il chiamante ha fatto: " + Board.Instance.getChiamantePointCount ().ToString () + " punti");
			Console.WriteLine ("gli altri hanno fatto: " + Board.Instance.getAltriPointCount ().ToString () + " punti");

			Console.WriteLine ("Il chiamante deve fare " + Board.Instance.WinningPoint.ToString () + " punti, quindi i vincitori sono:");
			Board.Instance.Winner.ForEach (delegate(Player p) {
				Console.WriteLine (p.ToString ());
			});



		}

		public static void mssBid (IBid bid)
		{
			Console.WriteLine ("**********************");
			Console.WriteLine ("Nuova bid:" + bid.ToString ());
			IBid wb = Board.Instance.currentAuctionWinningBid;

			if (wb == null)
				Console.WriteLine ("nessuna offerta");
			else
				Console.WriteLine ("Bid vincente:" + wb.ToString ());

			Console.WriteLine ("**********************");
		}

		public static void mssMove (Move move)
		{
			Console.WriteLine ("**********************");
			Console.WriteLine ("Nuova mossa:" + move.ToString ());
			Console.WriteLine ("carte in banco:");
			Board.Instance.CardOnTheBoard.ForEach (delegate(Card c) {
				Console.WriteLine (c.ToString ());
			});
			Console.WriteLine ("**********************");
		}

		public static void mssPick (Player p, List<Card> lc)
		{
			Console.WriteLine ("**********************");
			Console.WriteLine (p.ToString () + " ha preso la board composta da:");
			lc.ForEach (delegate(Card c) {
				Console.WriteLine (c.ToString ());
			});
			Console.WriteLine ("**********************");
		}

	}
}
