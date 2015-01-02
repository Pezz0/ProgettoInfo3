using System;

namespace ChiamataLibrary
{
	public class AIStupid:IArtificialIntelligence
	{
		private readonly AIAuStupid _auction;
		private readonly AIPtStupid _playtime;

		public AIStupid (Player me) : base (me)
		{
			_auction = new AIAuStupid (me);
			_playtime = new AIPtStupid (me);
		}
	}
}

