using System;

namespace ChiamataLibrary
{
	public class AIStupid:IArtificialIntelligence
	{
		private readonly AIAuStupid _auction;
		private readonly AIPtStupid _playtime;


		public override bool Active {
			get { return _auction.Active; }
			set {
				_auction.Active = value;
				_playtime.Active = value;
			}
		}


		public AIStupid (Player me) : base (me)
		{
			_auction = new AIAuStupid (me);
			_playtime = new AIPtStupid (me);
		}
	}
}

