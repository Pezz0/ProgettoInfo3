using System;
using NUnit.Framework;
using ChiamataLibrary;
using System.Collections.Generic;

namespace TestProject
{
	/// <summary>
	/// Test for the <see cref="ChiamatLibrary.Bid"/> class.
	/// </summary>
	[TestFixture ()]
	internal class BidTest
	{
		/// <summary>
		/// A null bid.
		/// </summary>
		private BidBase _null1;
		/// <summary>
		/// A null bid.
		/// </summary>
		private BidBase _null2;

		/// <summary>
		/// A pass bid.
		/// </summary>
		private PassBid _pass1;
		/// <summary>
		/// A pass bid.
		/// </summary>
		private PassBid _pass2;

		/// <summary>
		/// A normal bid.
		/// </summary>
		private NormalBid _normalBid1;
		/// <summary>
		/// A normal bid.
		/// </summary>
		private NormalBid _normalBid2;
		/// <summary>
		/// A normal bid.
		/// </summary>
		private NormalBid _normalBid3;
		/// <summary>
		/// A normal bid.
		/// </summary>
		private NormalBid _normalBid4;

		/// <summary>
		/// A carichi bid.
		/// </summary>
		private CarichiBid _carichi1;
		/// <summary>
		/// A carichi bid.
		/// </summary>
		private CarichiBid _carichi2;
		/// <summary>
		/// A carichi bid.
		/// </summary>
		private CarichiBid _carichi3;

		/// <summary>
		/// Setup for this tests.
		/// </summary>
		[SetUp ()]
		public void setup ()
		{
			Board.Instance.Reset ();

			using (TestRandom rnd = new TestRandom ())
				Board.Instance.InitializeMaster (new string[]{ "A", "B", "C", "D", "E" }, 2, rnd);//il mazziere è C

			_null1 = null;
			_null2 = null;

			_pass1 = new PassBid (Board.Instance.AllPlayers [0]);
			_pass2 = new PassBid (Board.Instance.AllPlayers [1]);

			_normalBid1 = new NormalBid (Board.Instance.AllPlayers [0], EnNumbers.ASSE, 70);
			_normalBid2 = new NormalBid (Board.Instance.AllPlayers [1], EnNumbers.DUE, 61);
			_normalBid3 = new NormalBid (Board.Instance.AllPlayers [2], EnNumbers.DUE, 70);
			_normalBid4 = new NormalBid (Board.Instance.AllPlayers [2], EnNumbers.DUE, 70);

			_carichi1 = new CarichiBid (Board.Instance.AllPlayers [0], 61);
			_carichi2 = new CarichiBid (Board.Instance.AllPlayers [0], 70);
			_carichi3 = new CarichiBid (Board.Instance.AllPlayers [0], 70);

		}

		#region Testing compareTo

		/// <summary>
		/// First test for the Compare To method.
		/// Comparing a pass to a null. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest01 ()
		{
			Assert.True (_pass1.CompareTo (_null1) > 0);
		}

		/// <summary>
		/// Second test for the Compare To method.
		/// Comparing a pass to a pass. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest02 ()
		{
			Assert.True (_pass1.CompareTo (_pass2) == 0);
		}

		/// <summary>
		/// Third test for the Compare To method.
		/// Comparing a pass to a normal. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest03 ()
		{
			Assert.True (_pass1.CompareTo (_normalBid1) < 0);
		}

		/// <summary>
		/// Fourth test for the Compare To method.
		/// Comparing a normal to a pass. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest04 ()
		{
			Assert.True (_normalBid1.CompareTo (_pass1) > 0);
		}

		/// <summary>
		/// Fifth test for the Compare To method.
		/// Comparing a normal to a normal. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest05 ()
		{
			Assert.True (_normalBid1.CompareTo (_normalBid2) < 0);
		}

		/// <summary>
		/// Sixth test for the Compare To method.
		/// Comparing a normal to a normal. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest06 ()
		{
			Assert.True (_normalBid2.CompareTo (_normalBid3) < 0);
		}

		/// <summary>
		/// Seventh test for the Compare To method.
		/// Comparing a normal to a normal. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest07 ()
		{
			Assert.True (_normalBid3.CompareTo (_normalBid4) == 0);
		}

		/// <summary>
		/// Eighth test for the Compare To method.
		/// Comparing a normal to a carichi. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest08 ()
		{
			Assert.True (_normalBid1.CompareTo (_carichi1) < 0);
		}

		/// <summary>
		/// Ninth test for the Compare To method.
		/// Comparing a normal to a carichi. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest09 ()
		{
			Assert.True (_normalBid2.CompareTo (_carichi2) < 0);
		}

		/// <summary>
		/// Fifteenth test for the Compare To method.
		/// Comparing a normal to a carichi. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest15 ()
		{
			Assert.True (_normalBid2.CompareTo (_carichi1) > 0);
		}

		/// <summary>
		/// Tenth test for the Compare To method.
		/// Comparing a carichi to a normal. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest10 ()
		{
			Assert.True (_carichi1.CompareTo (_normalBid1) > 0);
		}

		/// <summary>
		/// Eleventh test for the Compare To method.
		/// Comparing a carichi to a normal. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest11 ()
		{
			Assert.True (_carichi1.CompareTo (_normalBid2) < 0);
		}

		/// <summary>
		/// Twelfth test for the Compare To method.
		/// Comparing a carichi to a normal. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest12 ()
		{
			Assert.True (_carichi1.CompareTo (_normalBid3) < 0);
		}

		/// <summary>
		/// Thirteenth test for the Compare To method.
		/// Comparing a carichi to a carichi. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest13 ()
		{
			Assert.True (_carichi1.CompareTo (_carichi2) < 0);
		}

		/// <summary>
		/// Fourteenth test for the Compare To method.
		/// Comparing a carichi to a carichi. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest14 ()
		{
			Assert.True (_carichi2.CompareTo (_carichi3) == 0);
		}




		#endregion

		#region Testing operator

		#region Testing >

		/// <summary>
		/// First test for the &gt operator
		/// Comparing a null to a null. Expected <c>False</c>.
		/// </summary>
		[Test ()]
		public void testOpGr1 ()
		{
			Assert.False (_null1 > _null2);
		}

		/// <summary>
		/// Second test for the &gt operator
		/// Comparing a null to a pass. Expected <c>False</c>.
		/// </summary>
		[Test ()]
		public void testOpGr2 ()
		{
			Assert.False (_null1 > _pass1);
		}

		/// <summary>
		/// Third test for the &gt operator
		/// Comparing a normal to a normal. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void testOpGr3 ()
		{
			Assert.True (_normalBid2 > _normalBid1);
		}

		#endregion

		#region Testing <

		/// <summary>
		/// First test for the &lt operator
		/// Comparing a null to a null. Expected <c>False</c>.
		/// </summary>
		[Test ()]
		public void testOpLe1 ()
		{
			Assert.False (_null1 < _null2);
		}

		/// <summary>
		/// Second test for the &lt operator
		/// Comparing a null to a pass. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void testOpLe2 ()
		{
			Assert.True (_null1 < _pass1);
		}


		/// <summary>
		/// Third test for the &lt operator
		/// Comparing a normal to a normal. Expected <c>False</c>.
		/// </summary>
		[Test ()]
		public void testOpLe3 ()
		{
			Assert.False (_normalBid2 < _normalBid1);
		}

		#endregion

		#region Testing >=

		/// <summary>
		/// First test for the &gt= operator
		/// Comparing a null to a null. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void testOpGrE1 ()
		{
			Assert.True (_null1 >= _null2);
		}

		/// <summary>
		/// Second test for the &gt= operator
		/// Comparing a null to a pass. Expected <c>False</c>.
		/// </summary>
		[Test ()]
		public void testOpGrE2 ()
		{
			Assert.False (_null1 >= _pass1);
		}

		/// <summary>
		/// Third test for the &gt= operator
		/// Comparing a normal to a normal. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void testOpGrE3 ()
		{
			Assert.True (_normalBid2 >= _normalBid1);
		}

		#endregion

		#region Testing <=

		/// <summary>
		/// First test for the &lt= operator
		/// Comparing a null to a null. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void testOpLeE1 ()
		{
			Assert.True (_null1 <= _null2);
		}

		/// <summary>
		/// Second test for the &lt= operator
		/// Comparing a null to a pass. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void testOpLeE2 ()
		{
			Assert.True (_null1 <= _pass1);
		}

		/// <summary>
		/// Third test for the &lt= operator
		/// Comparing a normal to a normal. Expected <c>False</c>.
		/// </summary>
		[Test ()]
		public void testOpLeE3 ()
		{
			Assert.False (_normalBid2 <= _normalBid1);
		}

		#endregion

		#region Testing ==

		/// <summary>
		/// First test for the == operator
		/// Comparing a null to a null. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void testOpE1 ()
		{
			Assert.True (_null1 == _null2);
		}

		/// <summary>
		/// Second test for the == operator
		/// Comparing a null to a pass. Expected <c>False</c>.
		/// </summary>
		[Test ()]
		public void testOpE2 ()
		{
			Assert.False (_null1 == _pass1);
		}

		/// <summary>
		/// Third test for the == operator
		/// Comparing a normal to a normal. Expected <c>False</c>.
		/// </summary>
		[Test ()]
		public void testOpE3 ()
		{
			Assert.False (_normalBid2 == _normalBid1);
		}

		#endregion

		#region Testing !=

		/// <summary>
		/// First test for the != operator
		/// Comparing a null to a null. Expected <c>False</c>.
		/// </summary>
		[Test ()]
		public void testOpNE1 ()
		{
			Assert.False (_null1 != _null2);
		}

		/// <summary>
		/// Second test for the != operator
		/// Comparing a null to a pass. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void testOpNE2 ()
		{
			Assert.True (_null1 != _pass1);
		}

		/// <summary>
		/// Third test for the != operator
		/// Comparing a normal to a normal. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void testOpNE3 ()
		{
			Assert.True (_normalBid2 != _normalBid1);
		}

		#endregion

		#endregion

		#region Testing equals

		/// <summary>
		/// First test for the equals method
		/// Comparing a pass to a pass. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void testEqual1 ()
		{
			Assert.True (_pass1.Equals (_pass2));
		}

		/// <summary>
		/// Second test for the equals method
		/// Comparing a pass to a null. Expected <c>False</c>.
		/// </summary>
		[Test ()]
		public void testEqual2 ()
		{
			Assert.False (_pass1.Equals (_null1));
		}

		/// <summary>
		/// Third test for the equals method
		/// Comparing a pass to a string. Expected <c>False</c>.
		/// </summary>
		[Test ()]
		public void testEqual3 ()
		{
			Assert.False (_pass1.Equals ("_null1"));
		}

		#endregion
	}

}

