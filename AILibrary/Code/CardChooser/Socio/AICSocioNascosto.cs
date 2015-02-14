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
			//a boolean that indicate if this player should consider himself last in the cycle
			bool last = _nCardOnBoard == ( Board.PLAYER_NUMBER - 1 );

			// if the socio is reveal an altro can consider himself last in the cycle if the player after him is the chiamante
			last = last || ( _nCardOnBoard == ( Board.PLAYER_NUMBER - 2 ) && ( _me + 1 ).Role == EnRole.CHIAMANTE );


			//a boolean that indicate if the current winner is a player that can be considered friend or not
			bool wf = false;
			//if the board is empty than wf=false
			if (_winnerOnBoard != null)
				wf = _winnerOnBoard.initialPlayer.Role == EnRole.CHIAMANTE;

			//if this player can consider himself last in the cycle than he can consider himself safe.
			if (last) {
				//if I can pick up without using a briscola do it
				if (TakeableWithNoBrisc)
					return _highNoBrisc;

				//if the chiamante is the winner
				if (wf) {
					//and the socio is revealed
					if (Board.Instance.isSocioReveal)
						return _lostCarico;	//play a carico
					else
						return _punticini;	//play the punticini
				}

				//scartino
				return null;
			}

			//evaluate the move based on the currenteDeltaChiamante
			switch (_currentDeltaChiamante) {
			//if the chiamante is the last put some point on the board
			//there is an high possibility that this point go the chiamante
			//prevents the altri to change the order in which the cycle is play
				case -4:
					return _punticini;

				case -3:
				case -2:
					//if the socio is revealed he can put a high value card
					//without problem so if he can take the board do it
					if (TakeableWithNoBrisc && Board.Instance.isSocioReveal)
						return _highNoBrisc;

					//if the socio isn't revealed is better to be safer
					if (TakeableWithNoBrisc)
						return _lowNoBrisc;

					return null;

				case -1:
					//if the socio is right before the chiamate 
					//the standard move is to not interfere to the chiamante play
					//so play a scartino
					return null;
				
				case 1:
					//if the socio is right after the chiamante the standard move
					//is to try to take the board and put the chiamante last in the next cycle

					//if he can take the board without using a briscola
					//do it unless he have to use a carico
					if (TakeableWithNoBrisc && !_lowNoBrisc.IsCarico)
						return _lowBrisc;

					return null;
				
				case 2:
				case 3:
					return null;

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

