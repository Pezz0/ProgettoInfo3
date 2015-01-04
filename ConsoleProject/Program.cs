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
			else if (a == "2")
				mainXML ();
		}

		#region standard

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
					bid = new CarichiBid (Board.Instance.ActiveAuctionPlayer, int.Parse (Console.ReadLine ()));
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
				Console.WriteLine ("Partita a carichi, chiamante:" + Board.Instance.getChiamante ().ToString ());
			else if (Board.Instance.GameType == EnGameType.STANDARD) {
				Console.WriteLine ("Partita standard");
				Console.WriteLine ("Chiamante: " + Board.Instance.getChiamante ().ToString ());
				if (Board.Instance.isChiamataInMano)
					Console.WriteLine ("Chiamata in mano");
				else
					Console.WriteLine ("Socio: " + Board.Instance.getSocio ().ToString ());

				foreach (Player p in Board.Instance.getAltri())
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
//			Console.WriteLine ("Il chiamante ha fatto: " + Board.Instance.getChiamantePointCount ().ToString () + " punti");
//			Console.WriteLine ("gli altri hanno fatto: " + Board.Instance.getAltriPointCount ().ToString () + " punti");
//
//			Console.WriteLine ("Il chiamante deve fare " + Board.Instance.WinningPoint.ToString () + " punti, quindi i vincitori sono:");
//			Board.Instance.Winner.ForEach (delegate(Player p) {
//				Console.WriteLine (p.ToString ());
//			});

		}

		#endregion

		#region MainIA

		private static void mainIA ()
		{
			Board.Instance.initializeMaster (new string[]{ "A", "B", "C", "D", "E" }, 2);//il mazziere è C

			Board.Instance.eventSomeonePlaceABid += mssBid;
			Board.Instance.eventSomeonePlayACard += mssMove;
			Board.Instance.eventPickTheBoard += mssPick;
			Board.Instance.eventAuctionEnded += astaFine;
			Board.Instance.eventGameStarted += inizioGame;
			Board.Instance.eventGameEnded += fineGame;

			//IAIAuction iaa1 = new AIAuFixJump (Board.Instance.AllPlayers[1], true, 2);
			//IAIAuction iaa2 = new AIAuFixJump (Board.Instance.AllPlayers[2], true, 2);
			//IAIAuction iaa3 = new AIAuFixJump (Board.Instance.AllPlayers[3], true, 2);
			//IAIAuction iaa4 = new AIAuFixJump (Board.Instance.AllPlayers[4], true, 2);

			IAIAuction iaa1 = new AIAuMobileJump (Board.Instance.AllPlayers [1], true, 10, 1, 1);
			IAIAuction iaa2 = new AIAuMobileJump (Board.Instance.AllPlayers [2], true, 10, 1, 1);
			IAIAuction iaa3 = new AIAuMobileJump (Board.Instance.AllPlayers [3], true, 10, 1, 1);
			IAIAuction iaa4 = new AIAuMobileJump (Board.Instance.AllPlayers [4], true, 10, 1, 1);

			IAIPlayTime iap1 = new AIPtStupid (Board.Instance.AllPlayers [1]);
			IAIPlayTime iap2 = new AIPtStupid (Board.Instance.AllPlayers [2]);
			IAIPlayTime iap3 = new AIPtStupid (Board.Instance.AllPlayers [3]);
			IAIPlayTime iap4 = new AIPtStupid (Board.Instance.AllPlayers [4]);
	
			Board.Instance.AllPlayers.ForEach (delegate(Player p) {
				Console.WriteLine (p.ToString () + " possiede:");
				Board.Instance.getPlayerHand (p).ForEach (delegate(Card c) {
					Console.WriteLine (c.ToString ());
				});
			});

			Console.WriteLine ("premere per partire");
			Console.ReadLine ();
			Board.Instance.startGame ();

		}

		public static void mssBid (IBid bid)
		{
			Console.WriteLine ("**********************");
			Console.WriteLine ("Nuova bid:" + bid.ToString ());
			IBid wb = Board.Instance.currentAuctionWinningBid;

			Console.WriteLine ("**********************");
			if (wb == null)
				Console.WriteLine ("nessuna offerta");
			else
				Console.WriteLine ("Bid vincente:" + wb.ToString ());

			Console.WriteLine ("**********************");

			if (!Board.Instance.isAuctionClosed && Board.Instance.ActiveAuctionPlayer == Board.Instance.Me) {
				Console.WriteLine (Board.Instance.ActiveAuctionPlayer.ToString () + " deve fare una offerta [passo=p; carichi=c, normale=qualsiasi altra cosa]");

				string a = Console.ReadLine ();
				IBid newbid = null;

				if (a == "p")
					Board.Instance.auctionPass (Board.Instance.ActiveAuctionPlayer);
				else if (a == "c") {
					Console.Write ("Punti: ");
					newbid = new CarichiBid (Board.Instance.ActiveAuctionPlayer, int.Parse (Console.ReadLine ()));
				} else {
					Console.Write ("Numero[0=due,...,8=tre,9=asse]: ");
					EnNumbers n = (EnNumbers) int.Parse (Console.ReadLine ());
					Console.Write ("Punti: ");
					int p = int.Parse (Console.ReadLine ());
					newbid = new NormalBid (Board.Instance.ActiveAuctionPlayer, n, p);
				}
				Board.Instance.auctionPlaceABid (newbid);
			}

		}

		public static void mssMove (Move move)
		{
			Console.WriteLine ("**********************");
			Console.WriteLine ("Nuova mossa:" + move.ToString ());
			if (!Board.Instance.isGameFinish) {
				Console.WriteLine ("carte in banco:");
				Board.Instance.CardOnTheBoard.ForEach (delegate(Card c) {
					Console.WriteLine (c.ToString ());
				});
			}
			Console.WriteLine ("**********************");

			if (!Board.Instance.isGameFinish && Board.Instance.ActivePlayer == Board.Instance.Me) {
				List<Card> mano = Board.Instance.ActivePlayer.Hand;
				for (int i = 0; i < mano.Count; i++)
					Console.WriteLine ("premere " + i.ToString () + " per giocare " + mano [i].ToString ());

				Board.Instance.PlayACard (Board.Instance.ActivePlayer, mano [int.Parse (Console.ReadLine ())]);
			}
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

		public static void astaFine ()
		{
			if (Board.Instance.currentAuctionWinningBid.bidder == Board.Instance.Me) {
				Console.WriteLine ("hai vinto l'asta e deve scegliere il seme[0=ori ,1=cope 2=bastoni , 3=spade]");
				Board.Instance.finalizeAuction ((EnSemi) int.Parse (Console.ReadLine ()));
			}
		}

		public static void inizioGame ()
		{
			if (Board.Instance.GameType == EnGameType.MONTE)
				Console.WriteLine ("Hanno passato tutti senza offerte e quindi la partita va a monte");
			else if (Board.Instance.GameType == EnGameType.CARICHI)
				Console.WriteLine ("Partita a carichi, chiamante:" + Board.Instance.getChiamante ().ToString ());
			else if (Board.Instance.GameType == EnGameType.STANDARD) {
				Console.WriteLine ("Partita standard");
				Console.WriteLine ("Chiamante: " + Board.Instance.getChiamante ().ToString ());
				if (Board.Instance.isChiamataInMano)
					Console.WriteLine ("Chiamata in mano");
				else
					Console.WriteLine ("Socio: " + Board.Instance.getSocio ().ToString ());

				foreach (Player p in Board.Instance.getAltri())
					Console.WriteLine ("Altro: " + p.ToString ());
			}
		}

		public static void fineGame ()
		{

			Console.WriteLine ("Partita finita");
//			int i = 0;
//			Archive.Instance.forEach (delegate(GameData gm) {
//				gm.writeOnXML ("C:\\Users\\Matteo\\prova" + i.ToString () + ".xml");
//				i++;
//			});
//			Console.WriteLine ("Il chiamante ha fatto: " + Board.Instance.getChiamantePointCount ().ToString () + " punti");
//			Console.WriteLine ("gli altri hanno fatto: " + Board.Instance.getAltriPointCount ().ToString () + " punti");
//
//			Console.WriteLine ("Il chiamante deve fare " + Board.Instance.WinningPoint.ToString () + " punti, quindi i vincitori sono:");
//			Board.Instance.Winner.ForEach (delegate(Player p) {
//				Console.WriteLine (p.ToString ());
//			});
		}

		#endregion

		#region MainXML

		private static void mainXML ()
		{
			Board.Instance.initializeMaster (new string[]{ "A", "B", "C", "D", "E" }, 2);//il mazziere è C

			GameData gm = new GameData (DateTime.Now, null, Board.Instance.AllPlayers.ToArray (), null, EnGameType.MONTE, null, 61);

			gm.writeOnXML ("C:\\Users\\Matteo\\prova.xml");

		}

		#endregion
	
	}
}
