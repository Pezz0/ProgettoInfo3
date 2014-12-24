using System;
using NUnit.Framework;
using ChiamataLibrary;
using System.Collections.Generic;

namespace TestProject
{
	[TestFixture ()]
	public class IBidTest
	{
		private IBid _null1;
		private IBid _null2;

		private PassBid _pass1;
		private PassBid _pass2;

		private NormalBid _normalBid1;
		private NormalBid _normalBid2;
		private NormalBid _normalBid3;
		private NormalBid _normalBid4;

		private BidCarichi _carichi1;
		private BidCarichi _carichi2;
		private BidCarichi _carichi3;


		[SetUp ()]
		public void setup ()
		{
			Board b = new Board ();

			b.initialize (new string[]{ "A", "B", "C", "D", "E" }, 2);//il mazziere è C

			_null1 = null;
			_null2 = null;


			_pass1 = new PassBid (b.Players [0]);
			_pass2 = new PassBid (b.Players [1]);

			_normalBid1 = new NormalBid (b.Players [0], EnNumbers.ASSE, 70);
			_normalBid2 = new NormalBid (b.Players [1], EnNumbers.DUE, 61);
			_normalBid3 = new NormalBid (b.Players [2], EnNumbers.DUE, 70);
			_normalBid4 = new NormalBid (b.Players [2], EnNumbers.DUE, 70);

			_carichi1 = new BidCarichi (b.Players [0], 61);
			_carichi2 = new BidCarichi (b.Players [0], 70);
			_carichi3 = new BidCarichi (b.Players [0], 70);

		}

		#region Testing compareTo

		[Test ()]
		public void compareToTest01 ()
		{
			Assert.True (_pass1.CompareTo (_null1) > 0);
		}

		[Test ()]
		public void compareToTest02 ()
		{
			Assert.True (_pass1.CompareTo (_pass2) == 0);
		}

		[Test ()]
		public void compareToTest03 ()
		{
			Assert.True (_pass1.CompareTo (_normalBid1) < 0);
		}

		[Test ()]
		public void compareToTest04 ()
		{
			Assert.True (_normalBid1.CompareTo (_pass1) > 0);
		}

		[Test ()]
		public void compareToTest05 ()
		{
			Assert.True (_normalBid1.CompareTo (_normalBid2) < 0);
		}

		[Test ()]
		public void compareToTest06 ()
		{
			Assert.True (_normalBid2.CompareTo (_normalBid3) < 0);
		}

		[Test ()]
		public void compareToTest07 ()
		{
			Assert.True (_normalBid3.CompareTo (_normalBid4) == 0);
		}

		[Test ()]
		public void compareToTest08 ()
		{
			Assert.True (_normalBid1.CompareTo (_carichi1) < 0);
		}

		[Test ()]
		public void compareToTest09 ()
		{
			Assert.True (_normalBid2.CompareTo (_carichi2) < 0);
		}

		[Test ()]
		public void compareToTest10 ()
		{
			Assert.True (_carichi1.CompareTo (_normalBid1) > 0);
		}

		[Test ()]
		public void compareToTest11 ()
		{
			Assert.True (_carichi1.CompareTo (_normalBid2) == 0);
		}

		[Test ()]
		public void compareToTest12 ()
		{
			Assert.True (_carichi1.CompareTo (_normalBid3) < 0);
		}

		[Test ()]
		public void compareToTest13 ()
		{
			Assert.True (_carichi1.CompareTo (_carichi2) < 0);
		}

		[Test ()]
		public void compareToTest14 ()
		{
			Assert.True (_carichi2.CompareTo (_carichi3) == 0);
		}




		#endregion

		#region Testing operator

		#region Testing >

		[Test ()]
		public void testOpGr1 ()
		{
			Assert.False (_null1 > _null2);
		}

		[Test ()]
		public void testOpGr2 ()
		{
			Assert.False (_null1 > _pass1);
		}

		[Test ()]
		public void testOpGr3 ()
		{
			Assert.True (_normalBid2 > _normalBid1);
		}

		#endregion

		#region Testing <

		[Test ()]
		public void testOpLe1 ()
		{
			Assert.False (_null1 < _null2);
		}

		[Test ()]
		public void testOpLe2 ()
		{
			Assert.True (_null1 < _pass1);
		}

		[Test ()]
		public void testOpLe3 ()
		{
			Assert.False (_normalBid2 < _normalBid1);
		}

		#endregion

		#region Testing >=

		[Test ()]
		public void testOpGrE1 ()
		{
			Assert.True (_null1 >= _null2);
		}

		[Test ()]
		public void testOpGrE2 ()
		{
			Assert.False (_null1 >= _pass1);
		}

		[Test ()]
		public void testOpGrE3 ()
		{
			Assert.True (_normalBid2 >= _normalBid1);
		}

		#endregion

		#region Testing <=

		[Test ()]
		public void testOpLeE1 ()
		{
			Assert.True (_null1 <= _null2);
		}

		[Test ()]
		public void testOpLeE2 ()
		{
			Assert.True (_null1 <= _pass1);
		}

		[Test ()]
		public void testOpLeE3 ()
		{
			Assert.False (_normalBid2 <= _normalBid1);
		}

		#endregion

		#region Testing ==

		[Test ()]
		public void testOpE1 ()
		{
			Assert.True (_null1 == _null2);
		}

		[Test ()]
		public void testOpE2 ()
		{
			Assert.False (_null1 == _pass1);
		}

		[Test ()]
		public void testOpE3 ()
		{
			Assert.False (_normalBid2 == _normalBid1);
		}

		#endregion

		#region Testing !=

		[Test ()]
		public void testOpNE1 ()
		{
			Assert.False (_null1 != _null2);
		}

		[Test ()]
		public void testOpNE2 ()
		{
			Assert.True (_null1 != _pass1);
		}

		[Test ()]
		public void testOpNE3 ()
		{
			Assert.True (_normalBid2 != _normalBid1);
		}

		#endregion

		#endregion

		#region Testing equals

		[Test ()]
		public void testEqual1 ()
		{
			Assert.True (_pass1.Equals (_pass2));
		}

		[Test ()]
		public void testEqual2 ()
		{
			Assert.False (_pass1.Equals (_null1));
		}

		[Test ()]
		public void testEqual3 ()
		{
			Assert.False (_pass1.Equals ("_null1"));
		}

		#endregion
	}

}

