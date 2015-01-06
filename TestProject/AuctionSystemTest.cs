using System;
using NUnit.Framework;
using ChiamataLibrary;
using System.Collections.Generic;

namespace TestProject
{
	[TestFixture ()]
	public class AuctionSystemTest
	{
		private List<Player> _players;

		[SetUp ()]
		public void setup ()
		{
			Board.Instance.reset ();
			Board.Instance.initializeMaster (new string[]{ "A", "B", "C", "D", "E" }, 2);//il mazziere è C

			_players = Board.Instance.AllPlayers;
		}

		[Test ()]
		public void AuctionExample1 ()
		{
			NormalBid nb = new NormalBid (_players [3], EnNumbers.CAVALLO, 70);
		
			IBid wb = Board.Instance.currentAuctionWinningBid;

			Assert.IsNull (wb);
			//Assert.AreSame (_players [3], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPlaceABid (nb);
		
			Assert.AreSame (nb, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [4], Board.Instance.ActiveAuctionPlayer);
		
			//Board.Instance.auctionPass (_players [4]);
		
			Assert.AreSame (nb, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [0], Board.Instance.ActiveAuctionPlayer);
		
			//Board.Instance.auctionPass (_players [0]);
		
			Assert.AreSame (nb, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [1], Board.Instance.ActiveAuctionPlayer);
		
			//Board.Instance.auctionPass (_players [1]);
		
			Assert.AreSame (nb, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [2], Board.Instance.ActiveAuctionPlayer);
		
			//Board.Instance.auctionPass (_players [2]);
		
			//Assert.True (Board.Instance.isAuctionClosed);
		
			//Board.Instance.finalizeAuction (EnSemi.COPE);
		
			Assert.AreEqual (EnGameType.STANDARD, Board.Instance.GameType);
			Assert.AreSame (_players [3], Board.Instance.getChiamante ());
		}

		[Test ()]
		public void AuctionExample2 ()
		{
			NormalBid nb1 = new NormalBid (_players [3], EnNumbers.CAVALLO, 61);
			NormalBid nb2 = new NormalBid (_players [4], EnNumbers.FANTE, 61);
			NormalBid nb3 = new NormalBid (_players [1], EnNumbers.SEI, 61);
			NormalBid nb4 = new NormalBid (_players [2], EnNumbers.DUE, 61);
			NormalBid nb5 = new NormalBid (_players [3], EnNumbers.DUE, 70);

			IBid wb = Board.Instance.currentAuctionWinningBid;

			Assert.IsNull (wb);
			//Assert.AreSame (_players [3], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPlaceABid (nb1);

			Assert.AreSame (nb1, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [4], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPlaceABid (nb2);

			Assert.AreSame (nb2, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [0], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPass (_players [0]);

			Assert.AreSame (nb2, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [1], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPlaceABid (nb3);

			Assert.AreSame (nb3, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [2], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPlaceABid (nb4);

			Assert.AreSame (nb4, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [3], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPlaceABid (nb5);

			Assert.AreSame (nb5, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [4], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPass (_players [4]);

			Assert.AreSame (nb5, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [1], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPass (_players [1]);

			Assert.AreSame (nb5, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [2], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPass (_players [2]);

			//Assert.True (Board.Instance.isAuctionClosed);

			//Board.Instance.finalizeAuction (EnSemi.COPE);

			Assert.AreEqual (EnGameType.STANDARD, Board.Instance.GameType);
			Assert.AreSame (_players [3], Board.Instance.getChiamante ());
		}

		[Test ()]
		public void auctionExample3AllPass ()
		{
			IBid wb = Board.Instance.currentAuctionWinningBid;

			Assert.IsNull (wb);
			//Assert.AreSame (_players [3], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPass (_players [3]);

			wb = Board.Instance.currentAuctionWinningBid;
			Assert.IsNull (wb);
			//Assert.AreSame (_players [4], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPass (_players [4]);

			wb = Board.Instance.currentAuctionWinningBid;
			Assert.IsNull (wb);
			//Assert.AreSame (_players [0], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPass (_players [0]);

			wb = Board.Instance.currentAuctionWinningBid;
			Assert.IsNull (wb);
			//Assert.AreSame (_players [1], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPass (_players [1]);

			wb = Board.Instance.currentAuctionWinningBid;
			Assert.IsNull (wb);
			//Assert.AreSame (_players [2], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPass (_players [2]);

			//Assert.True (Board.Instance.isAuctionClosed);

			//Board.Instance.finalizeAuction (EnSemi.COPE);

			Assert.AreEqual (EnGameType.MONTE, Board.Instance.GameType);
		}

		[Test ()]
		public void AuctionExample4 ()
		{
			NormalBid nb1 = new NormalBid (_players [3], EnNumbers.CAVALLO, 61);
			NormalBid nb2 = new NormalBid (_players [4], EnNumbers.FANTE, 61);
			CarichiBid bc1 = new CarichiBid (_players [0], 61);
			NormalBid nb3 = new NormalBid (_players [4], EnNumbers.DUE, 70);
			CarichiBid bc2 = new CarichiBid (_players [0], 80);

			IBid wb = Board.Instance.currentAuctionWinningBid;

			Assert.IsNull (wb);
			//Assert.AreSame (_players [3], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPlaceABid (nb1);

			Assert.AreSame (nb1, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [4], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPlaceABid (nb2);

			Assert.AreSame (nb2, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [0], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPlaceABid (bc1);

			Assert.AreSame (bc1, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [1], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPass (_players [1]);

			Assert.AreSame (bc1, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [2], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPass (_players [2]);

			Assert.AreSame (bc1, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [3], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPass (_players [3]);

			Assert.AreSame (bc1, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [4], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPlaceABid (nb3);

			Assert.AreSame (nb3, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [0], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPlaceABid (bc2);

			Assert.AreSame (bc2, Board.Instance.currentAuctionWinningBid);
			//Assert.AreSame (_players [4], Board.Instance.ActiveAuctionPlayer);

			//Board.Instance.auctionPass (_players [4]);

			//Assert.True (Board.Instance.isAuctionClosed);

			//Board.Instance.finalizeAuction (EnSemi.COPE);

			Assert.AreEqual (EnGameType.CARICHI, Board.Instance.GameType);
			Assert.AreSame (_players [0], Board.Instance.getChiamante ());
		}
	
	}


}

