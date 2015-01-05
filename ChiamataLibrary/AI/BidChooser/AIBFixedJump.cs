using System;

namespace ChiamataLibrary
{
	public class AIBFixedJump:IAIBCallEverything
	{
		private readonly int _maxJump;

		public override void setup (Player me, EnSemi seme)
		{
			base.setup (me, seme);

			int cj = 0;
			for (int i = Board.Instance.nNumber - 1; i > 0; i--) {
				_lastBid = new NormalBid (me, (EnNumbers) i, 61);

				if (Board.Instance.getCard (_seme, (EnNumbers) i).initialPlayer == me)
					cj = 0;
				else if (cj < _maxJump)
					cj = cj + 1;
				else
					break;
			}

			while (Board.Instance.getCard (_seme, ( (NormalBid) _lastBid ).number).initialPlayer == me)
				_lastBid = new NormalBid (me, ( (NormalBid) _lastBid ).number + 1, 61);

		}

		public AIBFixedJump (int maxJump) : base ()
		{
			this._maxJump = maxJump;
		}
	}
}

