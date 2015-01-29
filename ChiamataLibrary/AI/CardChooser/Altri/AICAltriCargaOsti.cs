﻿using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// AI for play time, for players that are not in the SOCIO role or CHIAMANTE role.
	/// See the documentation for more informations about this AI.
	/// </summary>
	public class AICAltriCargaOsti:IAICardChooser
	{
		/// <summary>
		/// The <see cref="ChiamataLibrary.Player"/> instance representing the AI.
		/// </summary>
		private Player _me;
		/// <summary>
		/// Integer representing the distance between this player and the player in the CHIAMANTE role.
		/// </summary>
		private int _deltaChiamante;
		/// <summary>
		/// The lower threshold of points. If the points on the field are below this threshold, the AI won't even attempt to win the hand.
		/// </summary>
		private readonly int _thresholdL;
		/// <summary>
		/// The higher threshold of points. If the points on the field are above this threshold, the AI will try its best to win the hand.
		/// </summary>
		private readonly int _thresholdH;

		/// <summary>
		/// Method that returns which card the AI wants to play.
		/// </summary>
		/// <returns>The card.</returns>
		public Card chooseCard ()
		{
			Card temp;
			int boardValue = Board.Instance.ValueOnBoard;
			int o = _deltaChiamante + ( Board.Instance.getChiamante ().Hand.Count == _me.Hand.Count ? -5 : 0 );
			switch (o) {
				case -4:
				case -3:
					return null;
				case -2:
				case -1:
					if (Board.Instance.CardOnTheBoard.Exists (c => c.isBiscola))
						return null;
					else
						return _me.getStrozzoBasso ();
				
				case 1:
				case 2:
					return _me.getCarico ();
				
				case 3:
					if (boardValue < _thresholdL)
						temp = _me.getStrozzoBasso ();
					else if (boardValue < _thresholdH) {
						temp = _me.getBriscolaNotCarico ();
					} else
						temp = _me.getBriscolaCarico ();

					if (Board.Instance.CardOnTheBoard.FindAll (c => c.isBiscola).TrueForAll (c => !( c < temp )))
						return temp;
					else
						return null;
						
				case 4:

					if (Board.Instance.CardOnTheBoard.Exists (c => c.isBiscola)) {
						if (boardValue < _thresholdL)
							temp = _me.getStrozzoBasso ();
						else if (boardValue < _thresholdH) {
							temp = _me.getBriscolaNotCarico ();
						} else
							temp = _me.getBriscolaCarico ();

						if (Board.Instance.CardOnTheBoard.FindAll (c => c.isBiscola).TrueForAll (c => !( c < temp )))
							return temp;
						else
							return null;
					} else
						return  _me.getStrozzoAlto ();

			}

			throw new Exception ("some errore occur");
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="me">The <see cref="ChiamataLibrary.Player"/> instance representing the AI.</param>
		public void setup (Player me)
		{
			this._me = me;
			_deltaChiamante = ( me.order - Board.Instance.getChiamante ().order + Board.PLAYER_NUMBER ) % Board.PLAYER_NUMBER;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.AICAltriCargaOsti"/> class.
		/// </summary>
		/// <param name="thresholdL">The lower points threshold. See <see cref="ChiamataLibrary.AICAltriCargaOsti._thresholdL"/> for more informations.</param>
		/// <param name="thresholdH">The higher points threshold. See <see cref="ChiamataLibrary.AICAltriCargaOsti._thresholdH"/> for more informations.</param>
		public AICAltriCargaOsti (int thresholdL, int thresholdH)
		{
			this._thresholdL = thresholdL;
			this._thresholdH = thresholdH;
		}
	}
}

