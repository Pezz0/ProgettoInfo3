using System;
using System.Collections.Generic;

namespace ChiamataLibrary
{
	public class AIPtCTakeAllV2:IAIPlayTime
	{
		//FIXME : 
		//Non strozza mai e poi mai
		//Reagisce alle carte in banco e non prende iniziativa
		//Gioca peggio di haxhi
		private readonly int _thresholdH;
		private readonly int _thresholdL;
		private readonly int _pointsAfter;
		private const string NAME = "Haxhi";

		protected override Card playACard ()
		{


			int nCardOnBoard = Board.Instance.numberOfCardOnBoard;
			int valueOnBoard = Board.Instance.getValueOnBoard ()+_pointsAfter*(Board.PLAYER_NUMBER-nCardOnBoard-1);
			int turn = Board.Instance.Turn;
			bool reveal = !Board.Instance.CalledCard.isPlayable;
			List<Card> cards = Board.Instance.getPlayerHand (me);
			List<Card> briscole = getBriscole (cards);
			if(valueOnBoard<_thresholdL){
				return getScartino (cards);
			}
			else if( valueOnBoard<_thresholdH){
				if (briscole.Count == 0)
					return getScartino (cards);
				return briscole [0];
			}
			if (briscole.Count == 0)
				return getScartino (cards);
			return briscole [briscole.Count-1];


		}

		public AIPtCTakeAllV2 (Player me, int thresholdH, int thresholdL,int pointsAfter) : base (me)
		{
			this._thresholdH = thresholdH;
			this._thresholdL = thresholdL;
			this._pointsAfter = pointsAfter;
		}

		private Card getScartino(List<Card> mano){
			Card temp = mano [0];

			for (int i = 1; i < mano.Count; i++)
				if (mano [i] < temp)
					temp = mano [i];

			return temp;
		}
		private List<Card> getBriscole(List<Card> mano){
			List<Card> temp = new List<Card> ();
			mano.ForEach (delegate (Card c) {
				if (c.seme == Board.Instance.CalledCard.seme)
					temp.Add (c);
			});
			return temp;
		}
}
}

