using System;

namespace ChiamataLibrary
{
	public class AIAuStupid:IAIAuction
	{

		protected override IBid PlaceABid ()
		{
			return new PassBid (Me);
		}

		protected override EnSemi chooseSeme ()
		{
			return EnSemi.SPADE;
		}

		public AIAuStupid (int me) : base (me)
		{
		}
	}
}

