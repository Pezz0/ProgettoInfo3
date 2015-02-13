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

		/// <summary>
		/// The total value of the cards on the table
		/// </summary>
		protected int _valueOnBoard;

		/// <summary>
		/// The number of card on the table
		/// </summary>
		protected int _nCardOnBoard;

		#endregion

		#region Key card

		/// <summary>
		/// The current winning card on the board
		/// </summary>
		protected Card _winnerOnBoard;

		/// <summary>
		/// The lowest briscola, in the hand of the player, that can take the board. 
		/// </summary>
		protected Card _lowBrisc;

		/// <summary>
		/// The highest briscola, in the hand of the player, that can take the board. 
		/// </summary>
		protected Card _highBrisc;

		/// <summary>
		/// The lowest not briscola, in the hand of the player, that can take the board.
		/// </summary>
		protected Card _lowNoBrisc;

		/// <summary>
		/// The highest not briscola, in the hand of the player, that can take the board.
		/// </summary>
		protected Card _highNoBrisc;

		/// <summary>
		/// The most valuable card, in the hand of the player, that cannot take the board.
		/// This field cannot be null.
		/// </summary>
		protected Card _lostCarico;

		/// <summary>
		/// A vestita card, in the hand of the player, that cannot take the board.
		/// This field cannot be null.
		/// </summary>
		protected Card _punticini;

		/// <summary>
		/// The least valueable card, in the hand of the player, that cannot take the board.
		/// This field cannot be null.
		/// </summary>
		protected Card _scartino;

		/// <summary>
		/// Gets a value indicating whether the controlled player can take the board using a briscola.
		/// </summary>
		/// <value><c>true</c> if this player can take using a briscola; otherwise, <c>false</c>.</value>
		protected bool TakeableWithBrisc{ get { return _lowBrisc != null; } }

		/// <summary>
		/// Gets a value indicating whether the controlled player can take the board not using a briscola.
		/// </summary>
		/// <value><c>true</c> if this player can take using a briscola; otherwise, <c>false</c>.</value>
		protected bool TakeableWithNoBrisc{ get { return _lowNoBrisc != null; } }

		/// <summary>
		/// Gets a value indicating whether the current winning card on the board is a briscola.
		/// </summary>
		/// <value><c>true</c> if the current winner on board is a briscola; otherwise, <c>false</c>.</value>
		protected bool isWinnerOnBoardBrisc{ get { return _winnerOnBoard != null && !_winnerOnBoard.IsBiscrola; } }

		#endregion

		#region Positinal information

		/// <summary>
		/// Integer representing the distance between this player and the player in the CHIAMANTE role.
		/// </summary>
		private int _deltaChiamante;

		/// <summary>
		/// Integer representing the distance between this player and the chiamante.
		/// this value is negative if the controlled player is before the chiamate
		/// this value is positive if the controlled player is after the chiamate
		/// </summary>
		protected int _currentDeltaChiamante;

		#endregion

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

			// set all the key card at null
			_lowBrisc = null;
			_highBrisc = null;
			_lowNoBrisc = null;
			_highNoBrisc = null;
			_lostCarico = null;
			_punticini = null;
			_scartino = null;

			//get the hand of the player
			List<Card> hand = _me.GetHand ();

			//cycle all the card in the hand for determinate the key card
			foreach (Card c in hand) {

				// the lost carico cannot be null so if it is null set it
				// if the card isn't a briscola and the card is more valuable than the current lostCarico set it.
				if (_lostCarico == null || ( !c.IsBiscrola && c.GetPoint () > _lostCarico.GetPoint () ))
					_lostCarico = c;

				// the scartino cannot be null so if it is null set it
				// if the card isn't a briscola and the card is less valuable than the current lostCarico set it.
				if (_scartino == null || ( !c.IsBiscrola && c.GetPoint () < _lostCarico.GetPoint () ))
					_scartino = c;

				// the punticini cannot be null so if it is null set it
				// if the card isn't a briscola and the card is a vestita set it.
				if (_punticini == null && !c.IsBiscrola && c.IsVestita)
					_punticini = c;

				//if the current card can take the board..
				if (c > _winnerOnBoard) {

					//the card in the hand are ordered by value.

					// if the card is a briscola and the current lowBrisc is null
					// set the lowNoBrisc
					if (c.IsBiscrola && _lowBrisc == null)
						_lowBrisc = c;

					// if the card is a briscola and is better than the current highbrisc
					// set the new highBrisc
					// everything is higher than null.
					if (c.IsBiscrola && c > _highBrisc)
						_highBrisc = c;

					// if the card isn't a briscola and the current lownoBrisc is null
					// set the lowNoBrisc
					if (!c.IsBiscrola && _highNoBrisc == null)
						_highNoBrisc = c;

					// if the card isn't a briscola and is better than the current highNoBrisc
					// set the new highNoBrisc
					// everything is higher than null.
					if (!c.IsBiscrola && c > _lowNoBrisc)
						_lowNoBrisc = c;
				}
			}

			//set the current delta chiamante
			_currentDeltaChiamante = _deltaChiamante + ( _nCardOnBoard < _deltaChiamante ? -5 : 0 );

			//ask at the child the card he want to play
			Card choosen = ChooseCardPrivate ();

			// if the choosen card is null return the scartino
			if (choosen == null)
				return _scartino;
			else
				return choosen;

		}

		/// <summary>
		/// Method that choose the card to play.
		/// </summary>
		/// <returns>The choosen card.</returns>
		protected abstract Card ChooseCardPrivate ();

		/// <summary>
		/// Setup this instance of the AI card chooser.
		/// </summary>
		/// <param name="me">Me.</param>
		public void Setup (Player me)
		{
			_deltaChiamante = ( me.order - Board.Instance.GetChiamante ().order + Board.PLAYER_NUMBER ) % Board.PLAYER_NUMBER;
			this._me = me;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AILibrary.AICInformation"/> class.
		/// </summary>
		/// <param name="thresholdH">The quantity of point that this player have to try to take.</param>
		/// <param name="thresholdL">The quantity of point that this player want to take.</param>
		/// <param name="pointsAfter">The opportunity value that this player give to the card not played yet.</param>
		protected AICInformation (int thresholdH, int thresholdL, int pointsAfter)
		{
			this._thresholdH = thresholdH;
			this._thresholdL = thresholdL;
			this._pointsAfter = pointsAfter;
		}
	}
}

