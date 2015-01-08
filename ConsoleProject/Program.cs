using System;
using System.Collections.Generic;

namespace ChiamataLibrary
{
	class MainClass
	{
		public static void Main (string [] args)
		{
			string a = Console.ReadLine ();

			if (a == "x") {
				GameData gm = new GameData ("C:\\Users\\Matteo\\Documents\\prova.xml");
			} else if (a == "b") {
				Board.Instance.initializeMaster (new string[]{ "A", "B", "C", "D", "E" }, 2);

				byte [] bs = Board.Instance.toByteArray ();
				Board.Instance.recreateFromByteArray (bs);

			} else {
				Board.Instance.initializeMaster (new string[]{ "A", "B", "C", "D", "E" }, 2);	//il mazziere è C

				Board.Instance.AllPlayers.ForEach (delegate(Player p) {
					Console.WriteLine (p.ToString () + " possiede:");
					Board.Instance.getPlayerHand (p).ForEach (delegate(Card c) {
						Console.WriteLine (c.ToString ());
					});
				});

				//setto me
				Board.Instance.Me.setAuctionControl (bidChooser, semeChooser);
				Board.Instance.Me.setPlaytimeControl (cardChooser);

				//setto le IA
				ArtificialIntelligence AI1 = new ArtificialIntelligence (Board.Instance.getPlayer (1), new AIBMobileJump (10, 1, 2), new AISQuality (), new AICStupid ());
				ArtificialIntelligence AI2 = new ArtificialIntelligence (Board.Instance.getPlayer (2), new AIBMobileJump (10, 1, 2), new AISQuality (), new AICStupid ());
				ArtificialIntelligence AI3 = new ArtificialIntelligence (Board.Instance.getPlayer (3), new AIBMobileJump (10, 1, 2), new AISQuality (), new AICStupid ());
				ArtificialIntelligence AI4 = new ArtificialIntelligence (Board.Instance.getPlayer (4), new AIBMobileJump (10, 1, 2), new AISQuality (), new AICStupid ());

				//setto gli eventi
				Board.Instance.eventSomeonePlaceABid += someonePlaceABid;
				Board.Instance.eventSomeonePlayACard += someonePlayACard;
				Board.Instance.eventPickTheBoard += someonePickUp;
				Board.Instance.eventAuctionStart += auctionStarting;
				Board.Instance.eventPlaytimeStart += gameStarting;
				Board.Instance.eventPlaytimeEnd += gameFinish;

				Console.WriteLine ("premere per partire...");
				Console.ReadLine ();

				Board.Instance.start ();

				while (Board.Instance.Time < 41)
					Board.Instance.update ();

				Archive.Instance.forEach (delegate(GameData gd) {
					gd.writeOnXML ("C:\\Users\\Matteo\\Documents\\prova.xml");
				});

			}

		}


		public static IBid bidChooser ()
		{
			Console.WriteLine ("******************************");

			IBid wb = Board.Instance.currentAuctionWinningBid;

			if (wb == null)
				Console.WriteLine ("Non c'è nessuna bid");
			else
				Console.WriteLine ("Offerta vincente:" + wb.ToString ());

			Console.WriteLine ("Devi fare una offerta [passo=p; carichi=c, normale=qualsiasi altra cosa]");

			string a = Console.ReadLine ();

			if (a == "p")
				return new PassBid ();
			else if (a == "c") {
				Console.Write ("Punti: ");
				return  new CarichiBid (int.Parse (Console.ReadLine ()));
			} else {
				Console.Write ("Numero[0=due,...,8=tre,9=asse]: ");
				EnNumbers n = (EnNumbers) int.Parse (Console.ReadLine ());
				Console.Write ("Punti: ");
				int p = int.Parse (Console.ReadLine ());
				return new NormalBid (n, p);
			}
		}

		public static EnSemi? semeChooser ()
		{
			Console.WriteLine ("******************************");

			Console.WriteLine ("Hai vinto l'asta e devi scegliere il seme[0=ori ,1=cope 2=bastoni , 3=spade]");
			return (EnSemi) int.Parse (Console.ReadLine ());

		}

		public static Card cardChooser ()
		{
			Console.WriteLine ("******************************");

			Console.WriteLine ("carte in banco:");
			Board.Instance.CardOnTheBoard.ForEach (delegate(Card c) {
				Console.WriteLine (c.ToString ());
			});
			Console.WriteLine ("");

			List<Card> mano = Board.Instance.Me.Hand;

			for (int i = 0; i < mano.Count; i++)
				Console.WriteLine ("premere " + i.ToString () + " per giocare " + mano [i].ToString ());

			return mano [int.Parse (Console.ReadLine ())];

		}

		public static void someonePlaceABid (IBid bid)
		{
			Console.WriteLine ("**********************");
			Console.WriteLine ("Nuova bid:" + bid.ToString ());
		}

		public static void someonePlayACard (Move move)
		{
			Console.WriteLine ("**********************");
			Console.WriteLine ("Nuova move:" + move.ToString ());
		}

		public static void someonePickUp (Player player, List<Card> board)
		{
			Console.WriteLine ("**********************");
			Console.WriteLine (player.ToString () + " ha preso il turno.");
		}


		public static void auctionStarting ()
		{
			Console.WriteLine ("**********************");
			Console.WriteLine ("Inizio asta");
		}

		public static void gameStarting ()
		{
			Console.WriteLine ("**********************");
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

		public static void gameFinish ()
		{
			Console.WriteLine ("Partita finita");
		}
	}
}
