using System;
using System.Collections.Generic;
using ChiamataLibrary;
using AILibrary;

namespace ConsoleProject
{
	class ConsoleController:IPlayerController
	{
		public BidBase ChooseBid ()
		{
			Console.WriteLine ("******************************");

			BidBase wb = Board.Instance.currentAuctionWinningBid;

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

		public EnSemi? ChooseSeme ()
		{
			Console.WriteLine ("******************************");

			Console.WriteLine ("Hai vinto l'asta e devi scegliere il seme[0=ori ,1=cope 2=bastoni , 3=spade]");
			return (EnSemi) int.Parse (Console.ReadLine ());
		}

		public Card ChooseCard ()
		{

			Console.WriteLine ("******************************");

			Console.WriteLine ("carte in banco:");
			Board.Instance.CardOnTheBoard.ForEach (delegate(Card c) {
				Console.WriteLine (c.ToString ());
			});
			Console.WriteLine ("");

			List<Card> mano = Board.Instance.Me.GetHand ();

			for (int i = 0; i < mano.Count; i++)
				Console.WriteLine ("premere " + i.ToString () + " per giocare " + mano [i].ToString ());

			return mano [int.Parse (Console.ReadLine ())];
		}

	}

	class MainClass
	{

		public static void Main (string [] args)
		{

			Archive.Instance.AddFromFolder ();

			Board.Instance.Reset ();
			Board.Instance.InitializeMaster (new string[]{ "A", "B", "C", "D", "E" }, 2, new NormalRandom ());	//il mazziere è C

			foreach (Player p in Board.Instance.AllPlayers) {
				new AIPlayerController (p, new AIBMobileJump (10, 1, 1), new AISQuality (), new AICProva ());
				Console.WriteLine (p.ToString () + " possiede:");
				foreach (Card c in p.GetHand())
					Console.WriteLine (c.ToString ());
			}

			//setto me
			//Board.Instance.Me.Controller = new ConsoleController ();

			//setto gli eventi
			Board.Instance.eventSomeonePlaceABid += someonePlaceABid;
			Board.Instance.eventSomeonePlayACard += someonePlayACard;
			Board.Instance.eventPickTheBoard += someonePickUp;
			Board.Instance.eventAuctionStart += auctionStarting;
			Board.Instance.eventPlaytimeStart += gameStarting;
			Board.Instance.eventPlaytimeEnd += gameFinish;

			Console.WriteLine ("premere per partire...");
			Console.ReadLine ();

			Board.Instance.Start ();

			while (Board.Instance.Time < 41)
				Board.Instance.Update ();

			Archive.Instance.SaveLastGame ();

			Console.WriteLine ("premere per finire...");
			Console.ReadLine ();

		}

		public static void someonePlaceABid (BidBase bid)
		{
			Console.WriteLine ("Nuova bid:" + bid.ToString ());
		}

		public static void someonePlayACard (Player p, Card c)
		{
			Console.WriteLine (string.Format ("{0} gioca {1}", p.ToString (), c.ToString ()));
		}

		public static void someonePickUp (Player player, List<Card> board)
		{
			Console.WriteLine (player.ToString () + " ha preso il turno.");
			Console.WriteLine ("**********************");
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
				Console.WriteLine ("Partita a carichi, chiamante:" + Board.Instance.GetChiamante ().ToString ());
			else if (Board.Instance.GameType == EnGameType.STANDARD) {
				Console.WriteLine ("Partita standard");
				Console.WriteLine ("Chiamante: " + Board.Instance.GetChiamante ().ToString ());
				if (Board.Instance.isChiamataInMano)
					Console.WriteLine ("Chiamata in mano");
				else
					Console.WriteLine ("Socio: " + Board.Instance.GetSocio ().ToString ());

				foreach (Player p in Board.Instance.GetAltri())
					Console.WriteLine ("Altro: " + p.ToString ());
			}
		}

		public static void gameFinish ()
		{
			Console.WriteLine ("Partita finita");
		}
	}
}
