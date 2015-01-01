using System;

namespace ChiamataLibrary
{
	public class AIStupid
	{
		private readonly AIAuStupid _auction;
		private readonly AIPlStupid _playtime;

		public AIStupid (int me)
		{
			_auction = new AIAuStupid (me);
			_playtime = new AIPlStupid (me);
		}
	}
}

