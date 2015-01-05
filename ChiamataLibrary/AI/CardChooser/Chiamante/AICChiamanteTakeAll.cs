using System;
using System.Collections.Generic;

namespace ChiamataLibrary
{
	public class AICChiamanteTakeAll:IAICardChooser
	{
		private Player _me;
		private readonly int _thresholdH;
		private readonly int _thresholdL;
		private readonly int _pointsAfter;

		public Card chooseCard ()
		{
			int nCardOnBoard = Board.Instance.numberOfCardOnBoard;
			int valueOnBoard = Board.Instance.ValueOnBoard + _pointsAfter * ( Board.PLAYER_NUMBER - nCardOnBoard - 1 );
			int turn = Board.Instance.Turn;


			List<Card> briscole = _me.getBriscole ();

			if (valueOnBoard < _thresholdL)
				return _me.getScartino ();
			else if (valueOnBoard < _thresholdH) {
				if (briscole.Count == 0)
					return _me.getScartino ();

				return briscole [( ( valueOnBoard - _thresholdL ) / ( _thresholdH - _thresholdL ) ) * briscole.Count];
			}

			if (briscole.Count == 0)
				return _me.getScartino ();

			return briscole [briscole.Count - 1];
		}

		public void setup (Player me)
		{
			this._me = me;
		}


		public AICChiamanteTakeAll (int thresholdH, int thresholdL, int pointsAfter)
		{
			this._thresholdH = thresholdH;
			this._thresholdL = thresholdL;
			this._pointsAfter = pointsAfter;
		}
	}
}

