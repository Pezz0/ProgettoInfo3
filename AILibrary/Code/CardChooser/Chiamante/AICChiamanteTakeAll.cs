using System;
using System.Collections.Generic;
using ChiamataLibrary;

namespace AILibrary
{
	/// <summary>
	/// AI for play time, for players that are in the CHIAMANTE role.
	/// </summary>
	public class AICChiamanteTakeAll:AICInformation
	{
		protected override Card ChooseCardPrivate ()
		{
			// is this player last in this cycle?
			bool last = _nCardOnBoard == ( Board.PLAYER_NUMBER - 1 );
			// if the next player is the socio than is like to be last
			last = last || ( _nCardOnBoard == ( Board.PLAYER_NUMBER - 2 ) && Board.Instance.isSocioReveal && ( _me + 1 ).Role == EnRole.SOCIO );

			if (last) {

				if (Board.Instance.isSocioReveal && _winnerOnBoard.initialPlayer.Role == EnRole.SOCIO)
					return _lostCarico;

				//if I can pick up without using a briscola do it
				if (TakeableFromNoBrisc)
					return _highNoBrisc;

				//if there are enough point on the board pick up
				if (_valueOnBoard > _thresholdL && TakeableFromBrisc)
					return _lowBrisc;

				//scartino
				return null;
			}

			return TakeIfPossibile ();

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.AICChiamanteTakeAll"/> class.
		/// </summary>
		/// <param name="thresholdL">The lower points threshold. See <see cref="ChiamataLibrary.AICChiamanteTakeAll._thresholdL"/> for more informations.</param>
		/// <param name="thresholdH">The higher points threshold. See <see cref="ChiamataLibrary.AICChiamanteTakeAll._thresholdH"/> for more informations.</param>
		/// <param name="pointsAfter">An estimate of the points that can be added to the hand after this AI has already played. See <see cref="ChiamataLibrary.AICChiamanteTakeAll._pointsAfter"/> for more informations.</param>
		public AICChiamanteTakeAll (int thresholdH, int thresholdL, int pointsAfter) : base (thresholdH, thresholdL, pointsAfter)
		{
		}
	}
}

