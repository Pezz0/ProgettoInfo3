using System;
using NUnit.Framework;
using ChiamataLibrary;

namespace TestProject
{
	[TestFixture ()]
	public class CardTest
	{
		private Card _null;

		private Card _noBrisc1;
		private Card _noBrisc2;
		private Card _noBrisc3;
		private Card _brisc;

		[SetUp ()]
		public void setup ()
		{

			Board.Instance.reset ();
			Board.Instance.initialize (new string[]{ "A", "B", "C", "D", "E" }, 2);//il mazziere è C

			NormalBid nb = new NormalBid (Board.Instance.AllPlayers [3], EnNumbers.CAVALLO, 70);

			Board.Instance.auctionPlaceABid (nb);
			Board.Instance.auctionPass (Board.Instance.AllPlayers [4]);
			Board.Instance.auctionPass (Board.Instance.AllPlayers [0]);
			Board.Instance.auctionPass (Board.Instance.AllPlayers [1]);
			Board.Instance.auctionPass (Board.Instance.AllPlayers [2]);

			Board.Instance.finalizeAuction (EnSemi.COPE);

			_null = null;
			_noBrisc1 = Board.Instance.getCard (EnSemi.BASTONI, EnNumbers.ASSE);
			_noBrisc2 = Board.Instance.getCard (EnSemi.BASTONI, EnNumbers.CAVALLO);
			_noBrisc3 = Board.Instance.getCard (EnSemi.SPADE, EnNumbers.TRE);
			_brisc = Board.Instance.getCard (EnSemi.COPE, EnNumbers.SETTE);
		}

		#region compareTo test

		[Test ()]
		public void compareToTest01 ()
		{
			Assert.True (_noBrisc1.CompareTo (_null) > 0);
		}

		[Test ()]
		public void compareToTest02 ()
		{
			Assert.True (_noBrisc1.CompareTo (_noBrisc2) > 0);
		}

		[Test ()]
		public void compareToTest03 ()
		{
			Assert.True (_brisc.CompareTo (_noBrisc2) > 0);
		}

		[Test ()]
		public void compareToTest04 ()
		{
			Assert.True (_noBrisc1.CompareTo (_brisc) < 0);
		}

		[Test ()]
		public void compareToTest05 ()
		{
			Assert.True (_noBrisc1.CompareTo (_noBrisc3) == 0);
		}

		#endregion

		//devo testare gli operatori? sono glis stessi delle bid..
	}
}

