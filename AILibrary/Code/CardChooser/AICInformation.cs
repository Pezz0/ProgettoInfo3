using System;
using ChiamataLibrary;
using System.Collections.Generic;

namespace AILibrary
{
	public abstract class AICInformation:IAICardChooser
	{
		/// <summary>
		/// The <see cref="ChiamataLibrary.Player"/> instance representing the AI.
		/// </summary>
		protected Player _me;

		#region Value information

		/// <summary>
		/// The higher threshold of points. If the points on the field are above this threshold, the AI will try its best to win the hand.
		/// </summary>
		protected readonly int _thresholdH;
		/// <summary>
		/// The lower threshold of points. If the points on the field are below this threshold, the AI won't even attempt to win the hand.
		/// </summary>
		protected readonly int _thresholdL;
		/// <summary>
		/// An estimate of the points that players playing after this AI can add to the current hand.
		/// This is used to estimate the desirability of the current hand. If the current points are below the lower threshold but we think
		/// that the players playing after this AI will add more points to the hand, it is worthwhile to try and win the hand.
		/// </summary>
		protected readonly int _pointsAfter;

		protected int _valueOnBoard;

		protected int _nCardOnBoard;

		#endregion

		#region Key card

		protected Card _winnerOnBoard;

		protected Card _lowBrisc;

		protected Card _highBrisc;

		protected Card _lowNoBrisc;

		protected Card _highNoBrisc;

		protected Card _lostCarico;

		protected Card _punticini;

		protected Card _scartino;

		protected bool TakeableFromBrisc{ get { return _lowBrisc != null; } }

		protected bool TakeableFromNoBrisc{ get { return _lowNoBrisc != null; } }

		protected bool isWinnerOnBoardBrisc{ get { return _winnerOnBoard != null && !_winnerOnBoard.IsBiscrola; } }

		#endregion

		#region Positinal information

		/// <summary>
		/// Integer representing the distance between this player and the player in the CHIAMANTE role.
		/// </summary>
		private int _deltaChiamante;

		protected int _currentDeltaChiamante;

		#endregion

		protected Card TakeIfPossibile ()
		{
			//if the point on the board are enough
			//take it at all cost
			if (_valueOnBoard > _thresholdH && TakeableFromBrisc)
				return _highBrisc;

			//add the opportunity cost
			_valueOnBoard = _valueOnBoard + _pointsAfter * ( Board.PLAYER_NUMBER - _nCardOnBoard - 1 );

			Card lowPoint = _lowNoBrisc;

			if (!TakeableFromNoBrisc)
				lowPoint = _scartino;

			if (_valueOnBoard > _thresholdH || lowPoint.IsCarico)
				return lowPoint;

			return _lowNoBrisc;
		}


		/// <summary>
		/// Method that returns which card the AI wants to play.
		/// </summary>
		/// <returns>The card.</returns>
		public Card ChooseCard ()
		{
			//point value on the board
			_valueOnBoard = Board.Instance.ValueOnBoard;

			//stronger card on the board
			_winnerOnBoard = null;
			foreach (Card c in Board.Instance.CardOnTheBoard)
				if (c > _winnerOnBoard)
					_winnerOnBoard = c;

			//number of card on the board
			_nCardOnBoard = Board.Instance.numberOfCardOnBoard;

			_lowBrisc = null;
			_highBrisc = null;
			_lowNoBrisc = null;
			_highNoBrisc = null;
			_lostCarico = null;
			_punticini = null;
			_scartino = null;

			List<Card> hand = _me.GetHand ();
			foreach (Card c in hand) {

				if (_lostCarico == null || ( !c.IsBiscrola && c.GetPoint () > _lostCarico.GetPoint () ))
					_lostCarico = c;

				if (_scartino == null || ( !c.IsBiscrola && c.GetPoint () < _lostCarico.GetPoint () ))
					_scartino = c;

				if (_punticini == null && !c.IsBiscrola && c.IsVestita)
					_punticini = c;

				if (c > _winnerOnBoard) {

					if (c.IsBiscrola && _lowBrisc == null)
						_lowBrisc = c;
					if (c.IsBiscrola && c > _highBrisc)
						_highBrisc = c;

					if (!c.IsBiscrola && _highNoBrisc == null)
						_highNoBrisc = c;
					if (!c.IsBiscrola && c > _lowNoBrisc)
						_lowNoBrisc = c;
				}
			}

			_currentDeltaChiamante = _deltaChiamante + ( _nCardOnBoard < _deltaChiamante ? -5 : 0 );

			Card choosen = ChooseCardPrivate ();

			if (choosen == null)
				return _scartino;
			else
				return choosen;

		}

		protected abstract Card ChooseCardPrivate ();

		public void Setup (Player me)
		{
			_deltaChiamante = ( me.order - Board.Instance.GetChiamante ().order + Board.PLAYER_NUMBER ) % Board.PLAYER_NUMBER;
			this._me = me;
		}


		protected AICInformation (int thresholdH, int thresholdL, int pointsAfter)
		{
			this._thresholdH = thresholdH;
			this._thresholdL = thresholdL;
			this._pointsAfter = pointsAfter;
		}
	}
}

