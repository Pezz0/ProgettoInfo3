using System;
using NUnit.Framework;
using Engine;
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
		public void oneOffertAllPass ()
		{
			Assert.AreSame (_players [3], _board.ActiveAuctionPlayer);

			NormalBid nb = new NormalBid (_players [3], EnNumbers.CAVALLO, 70);

			_board.auctionPutABid (nb);

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

			Assert.AreSame (_players [3], _board.PlayerChiamante);
		}
	}


}

