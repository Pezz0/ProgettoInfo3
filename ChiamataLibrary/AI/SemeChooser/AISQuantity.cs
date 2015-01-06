using System;
using System.Collections.Generic;

namespace ChiamataLibrary
{
	namespace ChiamataLibrary
	{
		public class AISQuantity:IAISemeChooser
		{
			private EnSemi _seme;

			public EnSemi? chooseSeme ()
			{
				return _seme;
			}

			public void setup (Player me)
			{
				System.Collections.Generic.List<Card> [] onHand = new List<Card>[Board.Instance.nSemi]; 
				for (int i = 0; i < Board.Instance.nSemi; i++)
					onHand [i] = Board.Instance.getPlayerHand (me).FindAll (delegate(Card c) {
						return c.seme == (EnSemi) i;
					});
						
				int maxLenght = 0;
				int [] p = new int[Board.Instance.nSemi];

				for (int i = 0; i < Board.Instance.nSemi; i++)
					if (onHand [i].Count > onHand [maxLenght].Count)
						maxLenght = i;

				_seme = (EnSemi) maxLenght;
			}

			public AISQuantity ()
			{
			}
		}
	}
}

