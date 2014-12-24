using System;

namespace ChiamataLibrary
{
	public class Card
	{
		/// <summary>
		/// The board used by this card.
		/// </summary>
		private Board _board;

		#region Card's information

		/// <summary>
		/// The card's seme.
		/// </summary>
		private EnSemi _seme;

		/// <summary>
		/// The card's number.
		/// </summary>
		private EnNumbers _number;

		/// <summary>
		/// Gets the card's seme.
		/// </summary>
		/// <value>The card's seme.</value>
		public EnSemi Seme { get { return _seme; } }

		/// <summary>
		/// Gets card's number.
		/// </summary>
		/// <value>The card's number.</value>
		public EnNumbers Number { get { return _number; } }

		/// <summary>
		/// Gets the value in point of this card.
		/// </summary>
		/// <returns>the value in point.</returns>
		public int getPoint ()
		{
			if (_number == EnNumbers.ASSE)
				return 11;
			else if (_number == EnNumbers.TRE)
				return 10;
			else if (_number == EnNumbers.RE)
				return 4;
			else if (_number == EnNumbers.CAVALLO)
				return 3;
			else if (_number == EnNumbers.FANTE)
				return 2;
			else if (_number == EnNumbers.SETTE && _seme == EnSemi.ORI)
				return 1;
			else
				return 0;
		}

		#endregion

		#region Owner's information

		/// <summary>
		/// The player that have this card in his hand at the start of the game.
		/// </summary>
		private Player _plIni;

		/// <summary>
		/// The player that have this card in his pocket at the end of the game.
		/// </summary>
		private Player _plFin;

		/// <summary>
		/// The time in whith this card is played.
		/// </summary>
		private int _playingTime;

		/// <summary>
		/// Gets the player that have this card in his hand at the start of the game.
		/// </summary>
		/// <value>The initial player.</value>
		public Player InitialPlayer { get { return _plIni; } }

		/// <summary>
		/// Gets the player that have this card in his pocket at the end of the game.
		/// </summary>
		/// <value>The final player.</value>
		public Player FinalPlayer { get { return _plFin; } }

		/// <summary>
		/// Gets the time in whith this card is played.
		/// </summary>
		/// <value>The playing time.</value>
		public int PlayingTime { get { return _playingTime; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="Engine.Card"/> is playable.
		/// </summary>
		/// <value><c>true</c> if is playable; otherwise, <c>false</c>.</value>
		public bool isPlayable{ get { return _playingTime == -1; } }

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="Engine.Card"/> class.
		/// </summary>
		/// <param name="board">The board used by this card.</param>
		/// <param name="number">The card's number.</param>
		/// <param name="seme">The card's seme.</param>
		/// <param name="iniziale">Initial player.</param>
		public Card (Board board, EnNumbers number, EnSemi seme, Player initial)
		{
			if (!board.isCreationPhase)
				throw new WrongPhaseException ("A player must be instantiated during the creation time", "Creation");

			_board = board;	//set the board
			_seme = seme;	//set the seme
			_number = number;	//set the number
			_plIni = initial;	//set the initial player

			_plFin = null;		//the card is playable, so there isn't any final player yet.
			_playingTime = -1;
		}

		public override string ToString ()
		{
			return string.Format ("[Card: Seme={0}, Number={1}]", Seme, Number);
		}


	}
}