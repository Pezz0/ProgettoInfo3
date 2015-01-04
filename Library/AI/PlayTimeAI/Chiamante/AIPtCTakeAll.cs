﻿using System;
using System.Collections.Generic;

namespace ChiamataLibrary
{
	public class AIPtCTakeAll:IAIPlayTime
	{
		//FIXME : 
		//Non strozza mai e poi mai
		//Reagisce alle carte in banco e non prende iniziativa
		//Gioca peggio di haxhi
		private readonly int _thresholdH;
		private readonly int _thresholdL;
		private const string NAME = "Haxhi";

		protected override Card playACard ()
		{
			int valueOnBoard = Board.Instance.getValueOnBoard ();
			int nCardOnBoard = Board.Instance.numberOfCardOnBoard;
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

		public AIPtCTakeAll (Player me, int thresholdH, int thresholdL) : base (me)
		{
			this._thresholdH = thresholdH;
			this._thresholdL = thresholdL;
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

