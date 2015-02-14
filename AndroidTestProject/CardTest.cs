using System;
using NUnit.Framework;
using ChiamataLibrary;
using AILibrary;

namespace TestProject
{
	/// <summary>
	/// Test for the <see cref="ChiamatLibrary.Card"/> class.
	/// </summary>
	[TestFixture ()]
	internal class CardTest
	{

		/// <summary>
		/// A not briscola card.
		/// </summary>
		private Card _noBrisc1;
		/// <summary>
		/// A not briscola card.
		/// </summary>
		private Card _noBrisc2;
		/// <summary>
		/// A not briscola card.
		/// </summary>
		private Card _noBrisc3;
		/// <summary>
		/// A briscola card.
		/// </summary>
		private Card _brisc;
		/// <summary>
		/// A null Card.
		/// </summary>
		private Card _null1 = null;
		/// <summary>
		/// A null Card.
		/// </summary>
		private Card _null2 = null;

		/// <summary>
		/// Setup for this tests.
		/// </summary>
		[SetUp ()]
		public void setup ()
		{

			Board.Instance.Reset ();
			using (TestRandom rnd = new TestRandom ())
				Board.Instance.InitializeMaster (new string[]{ "A", "B", "C", "D", "E" }, 2, rnd);//il mazziere è C

			AIPlayerController AI0 = new AIPlayerController ((Player) 0, new AIBMobileJump (10, 1, 1), new AISQuality (), new AICProva ());
			AIPlayerController AI1 = new AIPlayerController ((Player) 1, new AIBMobileJump (10, 1, 1), new AISQuality (), new AICProva ());
			AIPlayerController AI2 = new AIPlayerController ((Player) 2, new AIBMobileJump (10, 1, 1), new AISQuality (), new AICProva ());
			AIPlayerController AI3 = new AIPlayerController ((Player) 3, new AIBMobileJump (10, 1, 1), new AISQuality (), new AICProva ());
			AIPlayerController AI4 = new AIPlayerController ((Player) 4, new AIBMobileJump (10, 1, 1), new AISQuality (), new AICProva ());

			Board.Instance.Start ();

			while (!Board.Instance.IsPlayTime)
				Board.Instance.Update ();
				
			_brisc = Board.Instance.CalledCard;

			_noBrisc1 = Board.Instance.GetCard ((EnSemi) ( ( (int) _brisc.seme + 1 ) % Board.Instance.nSemi ), EnNumbers.ASSE);
			_noBrisc2 = Board.Instance.GetCard ((EnSemi) ( ( (int) _brisc.seme + 1 ) % Board.Instance.nSemi ), EnNumbers.CAVALLO);
			_noBrisc3 = Board.Instance.GetCard ((EnSemi) ( ( (int) _brisc.seme + 2 ) % Board.Instance.nSemi ), EnNumbers.TRE);

		}

		/// <summary>
		///First test for the > operator
		/// Comparing a null to a null. Expected <c>False</c>.
		/// </summary>
		[Test ()]
		public void testGr1 ()
		{
			Assert.False (_null1 > _null2);
		}

		/// <summary>
		///Second  test for the > operator
		/// Comparing a not null to a null. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void testGr2 ()
		{
			Assert.True (_noBrisc1 > _null1);
		}

		/// <summary>
		///Third test for the > operator
		/// Comparing a null to a not null. Expected <c>False</c>.
		/// </summary>
		[Test ()]
		public void testGr3 ()
		{
			Assert.False (_null1 > _noBrisc1);
		}

		/// <summary>
		/// Fourth test for the > operator
		/// Comparing a not null to a not null with same Seme and different number. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void testGr4 ()
		{
			Assert.True (_noBrisc1 > _noBrisc2);
		}

		/// <summary>
		/// Fifth test for the > operator
		///  Comparing a not null to a not null with same Seme and different number. Expected <c>False</c>.
		/// </summary>
		[Test ()]
		public void testGr5 ()
		{
			Assert.False (_noBrisc2 > _noBrisc1);
		}

		/// <summary>
		/// Sixth test for the > operator
		///  Comparing a not null to a not null with different Seme. Expected <c>False</c>.
		/// </summary>
		[Test ()]
		public void testGr6 ()
		{
			Assert.False (_noBrisc2 > _noBrisc3);
		}

	}
}

