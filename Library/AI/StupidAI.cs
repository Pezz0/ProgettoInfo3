using System;

namespace ChiamataLibrary
{
	public class StupidAI:IAIPlayer
	{

		protected override IBid PlaceABid ()
		{
			return new PassBid (Me);
		}

		protected override EnSemi chooseSeme ()
		{
			return EnSemi.SPADE;
		}

		protected override Card PlayACard ()
		{
			return Board.Instance.getPlayerHand (Me) [0];
		}

		public StupidAI (int me) : base (me)
		{
		}
	}
}

