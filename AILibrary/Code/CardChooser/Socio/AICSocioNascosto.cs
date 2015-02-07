using System;
using ChiamataLibrary;
using System.Collections.Generic;

namespace AILibrary
{
	/// <summary>
	/// AI for play time, for players that are in the SOCIO role.
	/// </summary>
	public class AICSocioNascosto:AICInformation
	{
		protected override Card ChooseCardPrivate ()
		{
			// is this player last in this cycle?
			bool last = _nCardOnBoard == ( Board.PLAYER_NUMBER - 1 );

			last = last || ( _nCardOnBoard == ( Board.PLAYER_NUMBER - 2 ) && Board.Instance.isSocioReveal && ( _me + 1 ).Role == EnRole.CHIAMANTE );


			bool wf = false;
			if (_winnerOnBoard != null)
				wf = _winnerOnBoard.initialPlayer.Role == EnRole.CHIAMANTE;

			if (last) {
				//if I can pick up without using a briscola do it
				if (TakeableFromNoBrisc)
					return _highNoBrisc;

				if (_winnerOnBoard.initialPlayer.Role == EnRole.CHIAMANTE) {
					if (Board.Instance.isSocioReveal)
						return _lostCarico;
					else
						return _punticini;
				}

				//scartino
				return null;
			}


			switch (_currentDeltaChiamante) {
				case -4:
					return _punticini;

				case -3:
				case -2:
					if (TakeableFromNoBrisc && Board.Instance.isSocioReveal)
						return _highNoBrisc;

					if (TakeableFromNoBrisc)
						return _lowNoBrisc;

					return null;

				case -1:
					return null;
				
				case 1:
					if (TakeableFromBrisc)
						return _lowBrisc;
					return null;
				
				case 2:
					return null;
				
				case 3:
					return _punticini;

			}

			throw new Exception ("some errore occur");
		}



		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.AICSocioNascosto"/> class.
		/// </summary>
		public AICSocioNascosto (int thresholdH, int thresholdL, int pointsAfter) : base (thresholdH, thresholdL, pointsAfter)
		{
		}
	}
}

