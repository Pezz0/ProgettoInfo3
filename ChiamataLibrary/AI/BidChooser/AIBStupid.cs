using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// A stupid bid chooser
	/// </summary>
	public class AIBStupid:IAIBidChooser
	{
		public void setup (Player me, EnSemi seme)
		{

		}

		public IBid chooseABid ()
		{
			return new PassBid ();
		}

		public AIBStupid ()
		{
		}
	}
}

