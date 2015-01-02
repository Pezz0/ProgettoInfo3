using System;

namespace ChiamataLibrary
{
	public class AIPtCTakeAll:IAIPlayTime
	{
		private readonly int _thresholdH;
		private readonly int _thresholdL;

		protected override Card playACard ()
		{
			int valueOnBoard = Board.Instance.getValueOnBoard ();
			int nCardOnBoard = Board.Instance.numberOfCardOnBoard;
			int turn = Board.Instance.Turn;
			bool revael = !Board.Instance.CalledCard.isPlayable;



			return null;
		}

		public AIPtCTakeAll (Player me, int thresholdH, int thresholdL) : base (me)
		{
			this._thresholdH = thresholdH;
			this._thresholdL = thresholdL;
		}
	}
}

