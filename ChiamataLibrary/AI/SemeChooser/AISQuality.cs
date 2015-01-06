using System;
using System.Collections.Generic;

namespace ChiamataLibrary
{
	public class AISQuality:IAISemeChooser
	{
		private EnSemi _seme;

		public EnSemi? chooseSeme ()
		{
			return _seme;
		}

		public void setup (Player me)
		{
			List<Card> [] onHand = new List<Card>[Board.Instance.nSemi]; 
			for (int i = 0; i < Board.Instance.nSemi; i++)
				onHand [i] = Board.Instance.getPlayerHand (me).FindAll (delegate(Card c) {
					return c.seme == (EnSemi) i;
				});

			int maxValue = 0;
			int [] p = new int[Board.Instance.nSemi];

			for (int i = 0; i < Board.Instance.nSemi; i++) {
				p [i] = 0;
				onHand [i].ForEach (delegate(Card c) {
					p [i] = p [i] + c.getPoint ();
				});

				if (p [i] > p [maxValue])
					maxValue = i;
			}
				
			_seme = (EnSemi) maxValue;
		}

		public AISQuality ()
		{
		}
	}
}

