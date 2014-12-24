using System;
using NUnit.Framework;
using ChiamataLibrary;
using System.Collections.Generic;

namespace TestProject
{
	[TestFixture ()]
	public class AuctionSystemTest
	{
		private Board _board;
		private List<Player> _players;

		[SetUp ()]
		public void setup ()
		{
			_board = new Board ();

			_board.initialize (new string[]{ "A", "B", "C", "D", "E" }, 2);//il mazziere è C
			_players = _board.Players;
		}

		[Test ()]
		public void AuctionExample1 ()
		{
			NormalBid nb = new NormalBid (_players [3], EnNumbers.CAVALLO, 70);
		
			IBid wb = _board.currentAuctionWinningBid;

			Assert.IsNull (wb);
			Assert.AreSame (_players [3], _board.ActiveAuctionPlayer);

			_board.auctionPlaceABid (nb);
		
			Assert.AreSame (nb, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [4], _board.ActiveAuctionPlayer);
		
			_board.auctionPass (_players [4]);
		
			Assert.AreSame (nb, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [0], _board.ActiveAuctionPlayer);
		
			_board.auctionPass (_players [0]);
		
			Assert.AreSame (nb, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [1], _board.ActiveAuctionPlayer);
		
			_board.auctionPass (_players [1]);
		
			Assert.AreSame (nb, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [2], _board.ActiveAuctionPlayer);
		
			_board.auctionPass (_players [2]);
		
			Assert.True (_board.isAuctionClosed);
		
			_board.finalizeAuction (EnSemi.COPE);
		
			Assert.AreEqual (EnGameType.STANDARD, _board.GameType);
			Assert.AreSame (_players [3], _board.PlayerChiamante);
			Assert.True (_board.getCard (EnSemi.COPE, EnNumbers.CAVALLO).InitialPlayer.Role == EnRole.SOCIO);
		}

		[Test ()]
		public void AuctionExample2 ()
		{
			NormalBid nb1 = new NormalBid (_players [3], EnNumbers.CAVALLO, 61);
			NormalBid nb2 = new NormalBid (_players [4], EnNumbers.FANTE, 61);
			NormalBid nb3 = new NormalBid (_players [1], EnNumbers.SEI, 61);
			NormalBid nb4 = new NormalBid (_players [2], EnNumbers.DUE, 61);
			NormalBid nb5 = new NormalBid (_players [3], EnNumbers.DUE, 70);

			IBid wb = _board.currentAuctionWinningBid;

			Assert.IsNull (wb);
			Assert.AreSame (_players [3], _board.ActiveAuctionPlayer);

			_board.auctionPlaceABid (nb1);

			Assert.AreSame (nb1, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [4], _board.ActiveAuctionPlayer);

			_board.auctionPlaceABid (nb2);

			Assert.AreSame (nb2, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [0], _board.ActiveAuctionPlayer);

			_board.auctionPass (_players [0]);

			Assert.AreSame (nb2, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [1], _board.ActiveAuctionPlayer);

			_board.auctionPlaceABid (nb3);

			Assert.AreSame (nb3, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [2], _board.ActiveAuctionPlayer);

			_board.auctionPlaceABid (nb4);

			Assert.AreSame (nb4, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [3], _board.ActiveAuctionPlayer);

			_board.auctionPlaceABid (nb5);

			Assert.AreSame (nb5, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [4], _board.ActiveAuctionPlayer);

			_board.auctionPass (_players [4]);

			Assert.AreSame (nb5, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [1], _board.ActiveAuctionPlayer);

			_board.auctionPass (_players [1]);

			Assert.AreSame (nb5, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [2], _board.ActiveAuctionPlayer);

			_board.auctionPass (_players [2]);

			Assert.True (_board.isAuctionClosed);

			_board.finalizeAuction (EnSemi.COPE);

			Assert.AreEqual (EnGameType.STANDARD, _board.GameType);
			Assert.AreSame (_players [3], _board.PlayerChiamante);
			Assert.True (_board.getCard (EnSemi.COPE, EnNumbers.DUE).InitialPlayer.Role == EnRole.SOCIO);
		}

		[Test ()]
		public void auctionExample3AllPass ()
		{
			IBid wb = _board.currentAuctionWinningBid;

			Assert.IsNull (wb);
			Assert.AreSame (_players [3], _board.ActiveAuctionPlayer);

			_board.auctionPass (_players [3]);

			wb = _board.currentAuctionWinningBid;
			Assert.IsNull (wb);
			Assert.AreSame (_players [4], _board.ActiveAuctionPlayer);

			_board.auctionPass (_players [4]);

			wb = _board.currentAuctionWinningBid;
			Assert.IsNull (wb);
			Assert.AreSame (_players [0], _board.ActiveAuctionPlayer);

			_board.auctionPass (_players [0]);

			wb = _board.currentAuctionWinningBid;
			Assert.IsNull (wb);
			Assert.AreSame (_players [1], _board.ActiveAuctionPlayer);

			_board.auctionPass (_players [1]);

			wb = _board.currentAuctionWinningBid;
			Assert.IsNull (wb);
			Assert.AreSame (_players [2], _board.ActiveAuctionPlayer);

			_board.auctionPass (_players [2]);

			Assert.True (_board.isAuctionClosed);

			_board.finalizeAuction (EnSemi.COPE);

			Assert.AreEqual (EnGameType.MONTE, _board.GameType);
		}

		[Test ()]
		public void AuctionExample4 ()
		{
			NormalBid nb1 = new NormalBid (_players [3], EnNumbers.CAVALLO, 61);
			NormalBid nb2 = new NormalBid (_players [4], EnNumbers.FANTE, 61);
			BidCarichi bc1 = new BidCarichi (_players [0], 61);
			NormalBid nb3 = new NormalBid (_players [4], EnNumbers.DUE, 70);
			BidCarichi bc2 = new BidCarichi (_players [0], 80);

			IBid wb = _board.currentAuctionWinningBid;

			Assert.IsNull (wb);
			Assert.AreSame (_players [3], _board.ActiveAuctionPlayer);

			_board.auctionPlaceABid (nb1);

			Assert.AreSame (nb1, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [4], _board.ActiveAuctionPlayer);

			_board.auctionPlaceABid (nb2);

			Assert.AreSame (nb2, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [0], _board.ActiveAuctionPlayer);

			_board.auctionPlaceABid (bc1);

			Assert.AreSame (bc1, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [1], _board.ActiveAuctionPlayer);

			_board.auctionPass (_players [1]);

			Assert.AreSame (bc1, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [2], _board.ActiveAuctionPlayer);

			_board.auctionPass (_players [2]);

			Assert.AreSame (bc1, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [3], _board.ActiveAuctionPlayer);

			_board.auctionPass (_players [3]);

			Assert.AreSame (bc1, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [4], _board.ActiveAuctionPlayer);

			_board.auctionPlaceABid (nb3);

			Assert.AreSame (nb3, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [0], _board.ActiveAuctionPlayer);

			_board.auctionPlaceABid (bc2);

			Assert.AreSame (bc2, _board.currentAuctionWinningBid);
			Assert.AreSame (_players [4], _board.ActiveAuctionPlayer);

			_board.auctionPass (_players [4]);

			Assert.True (_board.isAuctionClosed);

			_board.finalizeAuction (EnSemi.COPE);

			Assert.AreEqual (EnGameType.CARICHI, _board.GameType);
			Assert.AreSame (_players [0], _board.PlayerChiamante);
		}
	
	}


}

