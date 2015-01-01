using System;

namespace ChiamataLibrary
{
	public class AIPlStupid:IAIPlayTime
	{
		protected override Card PlayACard ()
		{
			return Board.Instance.getPlayerHand (Me) [0];
		}

		public AIPlStupid (int me) : base (me)
		{
		}
	}
}

