using System;
using System.Collections.Generic;

namespace ChiamataLibrary
{

	/// <summary>
	/// AI for seme choosing, using as a figure of merit the quantity of the cards of the same SEME.
	/// </summary>
	public class AISQuantity:IAISemeChooser
	{
		/// <summary>
		/// The seme that will be chosen by this AI.
		/// </summary>
		private EnSemi _seme;

		/// <summary>
		/// Getter for the seme.
		/// </summary>
		/// <returns>The seme that will be chosen by this AI.</returns>
		public EnSemi? chooseSeme ()
		{
			return _seme;
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="me">The <see cref="ChiamataLibrary.Player"/> instance representing the AI.</param>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.AISQuantity"/> class.
		/// </summary>
		public AISQuantity ()
		{
		}
	}
}

