using System;

namespace ChiamataLibrary
{
	public class AIAuStupid:IAIAuction
	{

		protected override IBid placeABid ()
		{
			return new PassBid (me);
		}

		protected override EnSemi chooseSeme ()
		{
			return EnSemi.SPADE;
		}

		public AIAuStupid (Player me) : base (me)
		{
		}
	}
}

