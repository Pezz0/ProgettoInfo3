using System;
using NUnit.Framework;
using ChiamataLibrary;

namespace TestProject
{
	/// <summary>
	/// Test for the <see cref="ChiamatLibrary.Card"/> class.
	/// </summary>
	[TestFixture ()]
	public class CardTest
	{
		/// <summary>
		/// A null card.
		/// </summary>
		private Card _null;

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
		/// Setup for this tests.
		/// </summary>
		[SetUp ()]
		public void setup ()
		{

			Board.Instance.reset ();
			Board.Instance.initializeMaster (new string[]{ "A", "B", "C", "D", "E" }, 2);//il mazziere è C

			AIPlayerController AI0 = new AIPlayerController (Board.Instance.getPlayer (0), new AIBMobileJump (10, 1, 1), new AISQuality (), new AICProva ());
			AIPlayerController AI1 = new AIPlayerController (Board.Instance.getPlayer (1), new AIBMobileJump (10, 1, 1), new AISQuality (), new AICProva ());
			AIPlayerController AI2 = new AIPlayerController (Board.Instance.getPlayer (2), new AIBMobileJump (10, 1, 1), new AISQuality (), new AICProva ());
			AIPlayerController AI3 = new AIPlayerController (Board.Instance.getPlayer (3), new AIBMobileJump (10, 1, 1), new AISQuality (), new AICProva ());
			AIPlayerController AI4 = new AIPlayerController (Board.Instance.getPlayer (4), new AIBMobileJump (10, 1, 1), new AISQuality (), new AICProva ());

			Board.Instance.start ();

			while (!Board.Instance.isPlayTime)
				Board.Instance.update ();
				
			_brisc = Board.Instance.CalledCard;

			_null = null;
			_noBrisc1 = Board.Instance.getCard ((EnSemi) ( ( (int) _brisc.seme + 1 ) % Board.Instance.nSemi ), EnNumbers.ASSE);
			_noBrisc2 = Board.Instance.getCard ((EnSemi) ( ( (int) _brisc.seme + 1 ) % Board.Instance.nSemi ), EnNumbers.CAVALLO);
			_noBrisc3 = Board.Instance.getCard ((EnSemi) ( ( (int) _brisc.seme + 2 ) % Board.Instance.nSemi ), EnNumbers.TRE);

		}

		#region compareTo test

		/// <summary>
		/// First test for the Compare To method.
		/// Comparing a not briscola to a null. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest01 ()
		{
			Assert.True (_noBrisc1.CompareTo (_null) > 0);
		}

		/// <summary>
		/// Second test for the Compare To method.
		/// Comparing a not briscola to a not briscola. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest02 ()
		{
			Assert.True (_noBrisc1.CompareTo (_noBrisc2) > 0);
		}

		/// <summary>
		/// Third test for the Compare To method.
		/// Comparing a briscola to a not briscola. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest03 ()
		{
			Assert.True (_brisc.CompareTo (_noBrisc2) > 0);
		}

		/// <summary>
		/// Fourth test for the Compare To method.
		/// Comparing a not briscola to a briscola. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest04 ()
		{
			Assert.True (_noBrisc1.CompareTo (_brisc) < 0);
		}

		/// <summary>
		/// Fifth test for the Compare To method.
		/// Comparing a not briscola to a not briscola. Expected <c>True</c>.
		/// </summary>
		[Test ()]
		public void compareToTest05 ()
		{
			Assert.True (_noBrisc1.CompareTo (_noBrisc3) == 0);
		}

		#endregion

	}
}

