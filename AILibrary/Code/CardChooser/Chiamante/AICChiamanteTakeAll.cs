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
			//a boolean that indicate if this player should consider himself last in the cycle
			bool last = _nCardOnBoard == ( Board.PLAYER_NUMBER - 1 );

			// if the socio is reveal an altro can consider himself last in the cycle if the player after him is the socio and he is reveal.
			last = last || ( _nCardOnBoard == ( Board.PLAYER_NUMBER - 2 ) && Board.Instance.isSocioReveal && ( _me + 1 ).Role == EnRole.SOCIO );

			//a boolean that indicate if the current winner is a player that can be considered friend or not
			bool wf = false;
			//if the board is empty than wf=false
			if (_winnerOnBoard != null)
				//the chiamate should't trust anyone before the reveal
				wf = Board.Instance.isSocioReveal && _winnerOnBoard.initialPlayer.Role == EnRole.SOCIO;

			//if this player can consider himself last in the cycle than he can consider himself safe.
			if (last) {

				//if the current winner is the socio give him the lostCarico
				if (wf)
					return _lostCarico;

				//if I can pick up without using a briscola do it
				if (TakeableWithNoBrisc)
					return _highNoBrisc;

				//if there are enough point on the board pick up
				if (_valueOnBoard > _thresholdL && TakeableWithBrisc)
					return _lowBrisc;

				//scartino
				return null;
			}

			//if the point on the board are enough
			//take it at all cost
			if (_valueOnBoard > _thresholdH && TakeableWithBrisc)
				return _highBrisc;

			//add the opportunity cost
			_valueOnBoard = _valueOnBoard + _pointsAfter * ( Board.PLAYER_NUMBER - _nCardOnBoard - 1 );

			// the lowpoint card is the card to play if you don't care to take the board
			Card lowPoint = _scartino;

			// if you can take the board without using a briscola change the lowpoint
			if (TakeableWithNoBrisc)
				lowPoint = _highNoBrisc;

			// if the board is takeable with the briscola and the value on board is enough take it
			// if the lowpoint card is a carico then sacrifice the briscola
			if (TakeableWithBrisc && ( _valueOnBoard > _thresholdH || lowPoint.IsCarico ))
				return _lowBrisc;
				
			return lowPoint;

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

