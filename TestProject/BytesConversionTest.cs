using System;
using NUnit.Framework;
using ChiamataLibrary;

namespace TestProject
{
	[TestFixture ()]
	public class BytesConversionTest
	{

		[SetUp ()]
		public void setup ()
		{
//			Board.Instance.reset ();
//			Board.Instance.initializeMaster (new string[]{ "A", "B", "C", "D", "E" }, 2);//il mazziere è C
//
//			_players = Board.Instance.AllPlayers;
		}

		[Test ()]
		public void Card ()
		{
			Board.Instance.reset ();
			Board.Instance.initializeMaster (new string[]{ "A", "B", "C", "D", "E" }, 2);//il mazziere è C

			byte [] convertedCard = Board.Instance.getCard (EnSemi.ORI, EnNumbers.CAVALLO).toByteArray ();

			Assert.AreEqual (6, convertedCard [0]);

			Card recreatedCard = Board.Instance.getCard (convertedCard);

			Assert.AreSame (Board.Instance.getCard (EnSemi.ORI, EnNumbers.CAVALLO), recreatedCard);


		}
	
	
	
	}
}

