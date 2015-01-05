using System;

namespace ChiamataLibrary
{
	public class AIBMobileJump:IAIBCallEverything
	{
		private readonly int _maxJump;
		private readonly int _startJump;
		private readonly int _incr;

		public override void setup (Player me, EnSemi seme)
		{
			base.setup (me, seme);

			int currentMaxJump = _startJump;

			int cj = 0;
			for (int i = Board.Instance.nNumber - 1; i > 0; i--) {
				_lastBid = new NormalBid (me, (EnNumbers) i, 61);

				if (Board.Instance.getCard (_seme, (EnNumbers) i).initialPlayer == me) {
					cj = 0;
					currentMaxJump = currentMaxJump + _incr;
					if (currentMaxJump > _maxJump)
						currentMaxJump = _maxJump;
				} else if (cj < currentMaxJump)
					cj = cj + 1;
				else
					break;
			}

			while (Board.Instance.getCard (_seme, ( (NormalBid) _lastBid ).number).initialPlayer == me)
				_lastBid = new NormalBid (me, ( (NormalBid) _lastBid ).number + 1, 61);

		}

		public AIBMobileJump (int maxJump, int startJump, int incr) : base ()
		{
			this._incr = incr;
			this._startJump = startJump;
			this._maxJump = maxJump;
		}
	}
}

