using System;
using System.Collections.Generic;
using ChiamataLibrary;

namespace AILibrary
{
	/// <summary>
	/// AI for play time, for players that are in the CHIAMANTE role.
	/// </summary>
	public class AICChiamanteTakeAll:IAICardChooser
	{
		/// <summary>
		/// The <see cref="ChiamataLibrary.Player"/> instance representing the AI.
		/// </summary>
		private Player _me;
		/// <summary>
		/// The higher threshold of points. If the points on the field are above this threshold, the AI will try its best to win the hand.
		/// </summary>
		private readonly int _thresholdH;
		/// <summary>
		/// The lower threshold of points. If the points on the field are below this threshold, the AI won't even attempt to win the hand.
		/// </summary>
		private readonly int _thresholdL;
		/// <summary>
		/// An estimate of the points that players playing after this AI can add to the current hand.
		/// This is used to estimate the desirability of the current hand. If the current points are below the lower threshold but we think
		/// that the players playing after this AI will add more points to the hand, it is worthwhile to try and win the hand.
		/// </summary>
		private readonly int _pointsAfter;

		/// <summary>
		/// Method that returns which card the AI wants to play.
		/// </summary>
		/// <returns>The card.</returns>
		public Card ChooseCard ()
		{
			int nCardOnBoard = Board.Instance.numberOfCardOnBoard;
			int valueOnBoard = Board.Instance.ValueOnBoard + _pointsAfter * ( Board.PLAYER_NUMBER - nCardOnBoard - 1 );
			int turn = Board.Instance.Turn;

			List<Card> briscole = _me.GetBriscole ();

			if (valueOnBoard < _thresholdL)
				return _me.GetScartino ();
			else if (valueOnBoard < _thresholdH) {
				if (briscole.Count == 0)
					return _me.GetScartino ();

				return briscole [( ( valueOnBoard - _thresholdL ) / ( _thresholdH - _thresholdL ) ) * briscole.Count];
			}

			if (briscole.Count == 0)
				return null;

			return briscole [briscole.Count - 1];
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="me">The <see cref="ChiamataLibrary.Player"/> instance representing the AI.</param>
		public void Setup (Player me)
		{
			this._me = me;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.AICChiamanteTakeAll"/> class.
		/// </summary>
		/// <param name="thresholdL">The lower points threshold. See <see cref="ChiamataLibrary.AICChiamanteTakeAll._thresholdL"/> for more informations.</param>
		/// <param name="thresholdH">The higher points threshold. See <see cref="ChiamataLibrary.AICChiamanteTakeAll._thresholdH"/> for more informations.</param>
		/// <param name="pointsAfter">An estimate of the points that can be added to the hand after this AI has already played. See <see cref="ChiamataLibrary.AICChiamanteTakeAll._pointsAfter"/> for more informations.</param>
		public AICChiamanteTakeAll (int thresholdH, int thresholdL, int pointsAfter)
		{
			this._thresholdH = thresholdH;
			this._thresholdL = thresholdL;
			this._pointsAfter = pointsAfter;
		}
	}
}

