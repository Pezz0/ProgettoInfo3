using System;
using ChiamataLibrary;

namespace AILibrary
{
	/// <summary>
	/// AI for play time, for players that are not in the SOCIO role or CHIAMANTE role.
	/// See the documentation for more informations about this AI.
	/// </summary>
	public class AICAltriCargaOsti:AICInformation
	{
		protected override Card ChooseCardPrivate ()
		{

			bool last = _nCardOnBoard == Board.PLAYER_NUMBER - 1;

			if (Board.Instance.isSocioReveal) {
				last = last || ( _nCardOnBoard == ( Board.PLAYER_NUMBER - 2 ) && ( _me + 1 ).Role == EnRole.ALTRO );
				last = last || ( _nCardOnBoard == ( Board.PLAYER_NUMBER - 3 ) && ( _me + 1 ).Role == EnRole.ALTRO && ( _me + 2 ).Role == EnRole.ALTRO );
			}


			bool wf = false;
			if (_winnerOnBoard != null) {
				wf = _winnerOnBoard.initialPlayer.Role != EnRole.CHIAMANTE;

				if (Board.Instance.isSocioReveal)
					wf = _winnerOnBoard.initialPlayer.Role == EnRole.ALTRO;
			}

			if (last) {
				//if I can pick up without using a briscola do it
				if (TakeableFromNoBrisc)
					return _highNoBrisc;
					
				//if there are enough point on the board pick up
				if (_valueOnBoard > _thresholdL && TakeableFromBrisc)
					return _lowBrisc;

				//scartino
				return _scartino;
			}

			switch (_currentDeltaChiamante) {
				case -4:
				case -3:
					return null;
				case -2:
				case -1:
					if (TakeableFromNoBrisc && !_lowNoBrisc.IsCarico)
						return _lowNoBrisc;

					if (!isWinnerOnBoardBrisc && TakeableFromBrisc)
						return _lowBrisc;

					if (_valueOnBoard > _thresholdL && TakeableFromBrisc)
						return _lowBrisc;

					return null;

				case 1:
				case 2:
				case 3:
					if (TakeableFromNoBrisc)
						return _highNoBrisc;

					if (( isWinnerOnBoardBrisc && wf ) || isWinnerOnBoardBrisc)
						return _lostCarico;

					return null;

			}

			throw new Exception ("some errore occur");
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.AICAltriCargaOsti"/> class.
		/// </summary>
		/// <param name="thresholdL">The lower points threshold. See <see cref="ChiamataLibrary.AICAltriCargaOsti._thresholdL"/> for more informations.</param>
		/// <param name="thresholdH">The higher points threshold. See <see cref="ChiamataLibrary.AICAltriCargaOsti._thresholdH"/> for more informations.</param>
		public AICAltriCargaOsti (int thresholdH, int thresholdL, int pointsAfter) : base (thresholdH, thresholdL, pointsAfter)
		{
		}
	}
}

