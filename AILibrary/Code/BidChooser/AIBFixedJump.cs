using System;
using ChiamataLibrary;

namespace AILibrary
{
	/// <summary>
	/// AI for auction, Fixed Jump algorithm.
	/// See the documentation for more informations about this AI.
	/// </summary>
	public class AIBFixedJump:AIBCallEverything
	{
		/// <summary>
		/// Maximum jump.
		/// </summary>
		/// <remarks>
		/// Represents the maximum number of consecutive cards that can be missing from a player hand.
		/// For example, a value of 0 means that in order to continue bidding, the AI must have all the cards from ASSE to CINQUE of one SEME
		/// A Value of 1 lets the AI bid even if has some missing cards (the AI still cannot bid if two consecutive cards are missing)
		/// </remarks>
		private readonly int _maxJump;

		protected override NotPassBid SetLastBid ()
		{
			int cj = 0;
			for (int i = Board.Instance.nNumber - 1; i > 0; --i) {
				if (Board.Instance.GetCard (_seme, (EnNumbers) i).initialPlayer == _me)
					cj = 0;
				else if (cj < _maxJump)
					cj = cj + 1;
				else
					return new NormalBid (_me, (EnNumbers) i, 61);
			}

			return new NormalBid (_me, EnNumbers.DUE, 61);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.AIBFixedJump"/> class.
		/// </summary>
		/// <param name="maxJump">Value of max jump. See <see cref="ChiamataLibrary.AIBFixedJump._maxJump"/> for more informations.</param>
		public AIBFixedJump (int maxJump) : base ()
		{
			this._maxJump = maxJump;
		}
	}
}

