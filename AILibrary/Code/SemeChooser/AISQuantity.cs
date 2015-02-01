using System;
using System.Collections.Generic;
using ChiamataLibrary;

namespace AILibrary
{

	/// <summary>
	/// AI for seme choosing, using as a figure of merit the quantity of the cards of the same SEME.
	/// </summary>
	public class AISQuantity:IAISemeChooser
	{
		/// <summary>
		/// Getter for the seme.
		/// </summary>
		/// <returns>The seme that will be chosen by this AI.</returns>
		public EnSemi ChooseSeme (Player me)
		{
			List<Card> hand = me.GetHand ();

			int [] onHandCount = new int[Board.Instance.nSemi]; 
			int maxLenght = 0;

			foreach (Card c in hand) {
				onHandCount [(int) c.seme]++;
				if (onHandCount [(int) c.seme] > onHandCount [maxLenght])
					maxLenght = (int) c.seme;
			}
				
			return (EnSemi) maxLenght;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.AISQuantity"/> class.
		/// </summary>
		public AISQuantity ()
		{
		}
	}
}

