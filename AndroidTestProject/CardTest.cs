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
	public class CardTest
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
		/// Setup for this tests.
		/// </summary>
		[SetUp ()]
		public void setup ()
		{

			Board.Instance.reset ();
			Board.Instance.InitializeMaster (new string[]{ "A", "B", "C", "D", "E" }, 2, new TestRandom ());//il mazziere è C

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

		//TODO: > test

	}
}

