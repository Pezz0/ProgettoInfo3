using System;
using System.Collections.Generic;
using ChiamataLibrary;

namespace AILibrary
{
	/// <summary>
	/// AI for seme choosing, using as a figure of merit the quality of the cards of the same SEME.
	/// </summary>
	public class AISQuality:IAISemeChooser
	{
		/// <summary>
		/// Getter for the seme.
		/// </summary>
		/// <returns>The seme that will be chosen by this AI.</returns>
		public EnSemi ChooseSeme (Player me)
		{
			List<Card> hand = me.GetHand ();

			int [] onHandPoint = new int[Board.Instance.nSemi]; 
			int maxValue = 0;

			foreach (Card c in hand) {
				onHandPoint [(int) c.seme] += c.GetPoint ();
				if (onHandPoint [(int) c.seme] > onHandPoint [maxValue])
					maxValue = (int) c.seme;
			}

			return (EnSemi) maxValue;

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.AISQuality"/> class.
		/// </summary>
		public AISQuality ()
		{
		}
	}
}

