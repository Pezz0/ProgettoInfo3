using System;
using ChiamataLibrary;

namespace AILibrary
{
	/// <summary>
	/// AI for play time.
	/// </summary>
	public class AICStupid:IAICardChooser
	{
		/// <summary>
		/// The <see cref="ChiamataLibrary.Player"/> instance representing the AI.
		/// </summary>
		private Player _me;

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="me">The <see cref="ChiamataLibrary.Player"/> instance representing the AI.</param>
		public void Setup (Player me)
		{
			this._me = me;
		}

		/// <summary>
		/// Method that returns which card the AI wants to play.
		/// </summary>
		/// <returns>The card.</returns>
		public Card ChooseCard ()
		{
			return _me.GetHand (false) [0];
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.AICStupid"/> class.
		/// </summary>
		public AICStupid ()
		{
		}
	}
}

