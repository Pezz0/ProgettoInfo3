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
			//a boolean that indicate if this player should consider himself last in the cycle
			bool last = _nCardOnBoard == Board.PLAYER_NUMBER - 1;

			// if the socio is reveal an altro can consider himself last in the cycle if the player after him are altro.
			if (Board.Instance.isSocioReveal) {
				last = last || ( _nCardOnBoard == ( Board.PLAYER_NUMBER - 2 ) && ( _me + 1 ).Role == EnRole.ALTRO );
				last = last || ( _nCardOnBoard == ( Board.PLAYER_NUMBER - 3 ) && ( _me + 1 ).Role == EnRole.ALTRO && ( _me + 2 ).Role == EnRole.ALTRO );
			}

			//a boolean that indicate if the current winner is a player that can be considered friend or not
			bool wf = false;
			//if the board is empty than wf=false
			if (_winnerOnBoard != null) {
				//a player can be considered friend if he isn't the chiamante
				wf = _winnerOnBoard.initialPlayer.Role != EnRole.CHIAMANTE;

				//if the socio is reveal than the only friend are the altri
				if (Board.Instance.isSocioReveal)
					wf = _winnerOnBoard.initialPlayer.Role == EnRole.ALTRO;
			}

			//if this player can consider himself last in the cycle than he can consider himself safe.
			if (last) {
				//if I can pick up without using a briscola do it
				if (TakeableWithNoBrisc)
					return _highNoBrisc;
					
				//if there are enough point on the board pick up
				if (_valueOnBoard > _thresholdL && TakeableWithBrisc)
					return _lowBrisc;

				//scartino
				return _scartino;
			}

			//evaluate the move based on the currenteDeltaChiamante
			switch (_currentDeltaChiamante) {
			//if the chiamate is last or second last
				case -4:
				case -3:
					//do a passive move->scartino
					return null;
				case -2:
				case -1:
					//if this player is right before the chiamante
					//the standard move is to try to 

					//if the player can take the board without a briscola and that card isn't a carico
					if (TakeableWithNoBrisc && !_lowNoBrisc.IsCarico)
						return _lowNoBrisc;	//play lowNoBrisc
						
					//if there isn't a briscola on the board and the controlled player can take whith a briscola
					if (!isWinnerOnBoardBrisc && TakeableWithBrisc)
						return _lowBrisc; //play a briscola->lowBrisc

					//it there is there is enough point ont the board
					if (_valueOnBoard > _thresholdL) {
						//if the player can take it
						if (TakeableWithBrisc)
							return _lowBrisc;
						else if (TakeableWithNoBrisc)
							return _lowNoBrisc;
					}

					return null;

			//if the player is after the chiamate 
			//the standard move is to put a carico on the board
				case 1:
				case 2:
				case 3:
					//if a player can take without using a brisc
					if (TakeableWithNoBrisc)
						//do it with the highest card -> hightNoBrisc
						return _highNoBrisc;

					//if there isn't a briscola on the board
					//or a friendly player is the current winner
					if (wf || !isWinnerOnBoardBrisc)
						//player che most valuable card on the board
						return _lostCarico;

					//otherwise play a scartino
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

