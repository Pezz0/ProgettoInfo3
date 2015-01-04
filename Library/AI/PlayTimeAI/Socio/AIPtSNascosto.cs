using System;
using System.Collections.Generic;

namespace ChiamataLibrary
{
	public class AIPtSNascosto:IAIPlayTime
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
					Card vestita = getVestita (cards);
					if (vestita == null)
						return getScartino (cards);
					else
						return vestita;
				

				case -3:
				case -2:
					Card strozzo = getStrozzoBassoNB (cards);
					if (strozzo == null) {
						vestita = getVestita (cards);
						if (vestita == null)
							return getScartino (cards);
						else
							return vestita;
					} else
						return strozzo;


				break;
				case -1:
					return getScartino (cards);
				break;
				case 1:
					temp = getStrozzoBassoNB (cards);
					if (temp == null)
						return getScartino (cards);
					else
						return temp;
				break;
				case 2:
					return getScartino (cards);
				break;
				case 3:
					strozzo = getStrozzoBassoNB (cards);
					if (strozzo == null)
						return getScartino (cards);
					else
						return strozzo;
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
						temp = getVestita (cards);
						if (temp == null)
							return getScartino (cards);
						else
							return temp;
					}
				break;
			}

			return null;


		}

		protected override void setup ()
		{
			base.setup ();
			deltaOrder = (me.order - Board.Instance.getChiamante ().order + Board.PLAYER_NUMBER) % Board.PLAYER_NUMBER;
		}



		public AIPtSNascosto (Player me) : base (me)
		{
		}


	

	}
}

