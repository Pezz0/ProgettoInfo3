using System;

namespace ChiamataLibrary
{
	public class AIPtStupid:IAIPlayTime
	{
		protected override Card playACard ()
		{
			return Board.Instance.getPlayerHand (me) [0];
		}

		public AIPtStupid (Player me) : base (me)
		{
		}
	}
}
