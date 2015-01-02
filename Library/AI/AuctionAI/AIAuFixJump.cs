using System;
using System.Collections.Generic;

namespace ChiamataLibrary
{
	//vuole chiamare il seme in cui più ha punti
	//lascia al primo buco lungo maxJump
	public class AIAuFixJump:IAIAuCallEverything
	{
		private readonly int _maxJump;

		private readonly bool _politic;

		protected override void setup ()
		{
			base.setup ();
			int nSemi = Enum.GetValues (typeof (EnSemi)).GetLength (0);
			int nNumber = Enum.GetValues (typeof (EnNumbers)).GetLength (0);

			int maxValue = 0;
			int maxLenght = 0;
			int [] p = new int[nSemi];

			for (int i = 0; i < nSemi; i++) {
				if (_onHand [i].Count > _onHand [maxLenght].Count)
					maxLenght = i;

				p [i] = 0;
				_onHand [i].ForEach (delegate(Card c) {
					p [i] = p [i] + c.getPoint ();
				});

				if (p [i] > p [maxValue])
					maxValue = i;
			}
			if (_politic)//quality
				_seme = (EnSemi) maxValue;
			else 	//quantity
				_seme = (EnSemi) maxLenght;

			int cj = 0;
			for (int i = nNumber - 1; i > 0; i--) {
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

//			Console.WriteLine ("LAST BID: " + _lastBid.ToString ());

		}

		public AIAuFixJump (Player me, bool politic, int maxJump) : base (me)
		{
			this._politic = politic;
			this._maxJump = maxJump;
		}
	}
}

