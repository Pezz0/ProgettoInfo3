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
			Board b = new Board ();

			b.initialize (new string[]{ "A", "B", "C", "D", "E" }, 2);//il mazziere è C

			NormalBid nb = new NormalBid (b.AllPlayers [3], EnNumbers.CAVALLO, 70);

			b.auctionPlaceABid (nb);
			b.auctionPass (b.AllPlayers [4]);
			b.auctionPass (b.AllPlayers [0]);
			b.auctionPass (b.AllPlayers [1]);
			b.auctionPass (b.AllPlayers [2]);

			b.finalizeAuction (EnSemi.COPE);

			_null = null;
			_noBrisc1 = b.getCard (EnSemi.BASTONI, EnNumbers.ASSE);
			_noBrisc2 = b.getCard (EnSemi.BASTONI, EnNumbers.CAVALLO);
			_noBrisc3 = b.getCard (EnSemi.SPADE, EnNumbers.TRE);
			_brisc = b.getCard (EnSemi.COPE, EnNumbers.SETTE);
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

