using System;
using System.Collections.Generic;

namespace ChiamataLibrary
{
	public class AIPtACargaOsti:IAIPlayTime
	{
		private int deltaOrder;
		private int _thresholdL;
		private int _thresholdH;

		protected override Card playACard ()
		{
			List<Card> cards = Board.Instance.getPlayerHand (me);
			int boardValue = Board.Instance.getValueOnBoard ();
			Card temp;
			int o = deltaOrder + (Board.Instance.getChiamante ().Hand.Count == cards.Count ? -5 : 0);
			switch (o) {
				case -4:
				case -3:
					return getScartino (cards);
				break;
				case -2:
				case -1:
					if (Board.Instance.CardOnTheBoard.Exists (delegate (Card c) {
						return c.seme == Board.Instance.CalledCard.seme;
					})) {
						return getScartino (cards);
					} else
						return getStrozzoBasso (cards);
				break;
				case 1:

					temp = getCarico (cards);
					if (temp == null)
						return getScartino (cards);
					else
						return temp;
				break;
				case 2:
					temp = getCarico (cards);
					if (temp == null)
						return getScartino (cards);
					else
						return temp;
				break;
				case 3:



					if (boardValue < _thresholdL) {
						temp = getStrozzoBasso (cards);
						if (temp == null)
							return getScartino (cards);
						else
							return temp;
					} else if (boardValue < _thresholdH) {
						temp=getBriscolaNotCarico (cards);
					} else
						temp= getBriscolaCarico (cards);
					if (Board.Instance.CardOnTheBoard.FindAll (delegate (Card c) {
						return c.seme == Board.Instance.CalledCard.seme;
					}).TrueForAll (delegate (Card c) {
						return temp > c;
					})) {
						return temp;
					} else
				return getScartino (cards);


				break;
				case 4:
					if (!Board.Instance.CardOnTheBoard.Exists (delegate (Card c) {
						return c.seme == Board.Instance.CalledCard.seme;
					})) {
						temp = getStrozzoAlto (cards);
						if (temp == null)
							return getScartino (cards);
						else
							return temp;
					} else {

						if (boardValue < _thresholdL) {
							temp = getStrozzoBasso (cards);
							if (temp == null)
								return getScartino (cards);
							else
								return temp;
						} else if (boardValue < _thresholdH) {
							temp=getBriscolaNotCarico (cards);
						} else
							temp= getBriscolaCarico (cards);
						if (Board.Instance.CardOnTheBoard.FindAll (delegate (Card c) {
							return c.seme == Board.Instance.CalledCard.seme;
						}).TrueForAll (delegate (Card c) {
							return temp > c;
						})) {
							return temp;
						} else
							return getScartino (cards);
					}

				break;
			}

			return null;


		}


		public AIPtACargaOsti (Player me,int thresholdH,int thresholdL) : base (me)
		{
			_thresholdH = thresholdH;
			_thresholdL = thresholdL;
		}

		protected override void setup ()
		{
			base.setup ();
			deltaOrder = (me.order - Board.Instance.getChiamante ().order + Board.PLAYER_NUMBER) % Board.PLAYER_NUMBER;
		}

	
	}
}

