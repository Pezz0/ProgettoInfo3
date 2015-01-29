using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// Class representing a card.
	/// </summary>
	public class Card:IComparable<Card>,IEquatable<Card>
	{

		#region Card's information

		/// <summary>
		/// The seme.
		/// </summary>
		public readonly EnSemi seme;

		/// <summary>
		/// The number.
		/// </summary>
		public readonly EnNumbers number;

		/// <summary>
		/// Gets the value in points of this card.
		/// </summary>
		/// <returns>The card's value in points.</returns>
		public int getPoint ()
		{
			if (number == EnNumbers.ASSE)
				return 11;
			else if (number == EnNumbers.TRE)
				return 10;
			else if (number == EnNumbers.RE)
				return 4;
			else if (number == EnNumbers.CAVALLO)
				return 3;
			else if (number == EnNumbers.FANTE)
				return 2;
			else if (number == EnNumbers.SETTE && seme == EnSemi.ORI)
				return 1;
			else
				return 0;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="ChiamataLibrary.Card"/> is a carico.
		/// </summary>
		/// <value><c>true</c> if is carico; otherwise, <c>false</c>.</value>
		public bool isCarico{ get { return number == EnNumbers.ASSE || number == EnNumbers.TRE; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ChiamataLibrary.Card"/> is a biscola.
		/// </summary>
		/// <value><c>true</c> if is biscola; otherwise, <c>false</c>.</value>
		public bool isBiscola{ get { return seme == Board.Instance.Briscola; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ChiamataLibrary.Card"/> is a vestita.
		/// </summary>
		/// <value><c>true</c> if is vestita; otherwise, <c>false</c>.</value>
		public bool isVestita{ get { return number == EnNumbers.RE || number == EnNumbers.CAVALLO || number == EnNumbers.FANTE; } }

		#endregion

		#region Owner's information

		/// <summary>
		/// The player that owns this card at the start of the game.
		/// </summary>
		public readonly Player initialPlayer;

		/// <summary>
		/// The time at which the card has been played. A value of -1 means that the card hasn't been played yet.
		/// </summary>
		private int _playingTime = -1;

		/// <summary>
		/// Gets or sets the playing time. See <see cref="ChiamataLibrary.Card._playingTime"/> for more informations.
		/// </summary>
		/// <value>The playing time.</value>
		public int PlayingTime {
			get { return _playingTime; }
			set {
				if (!isPlayable)
					throw new Exception ("The card can't be played twice.");

				_playingTime = value;
			}
		}

		/// <summary>
		/// The player who owns this card at the end of the game.
		/// </summary>
		private Player _finalPlayer = null;

		/// <summary>
		/// The value that indicates if the card has already been taken by a player by winning an hand.
		/// </summary>
		private bool _finalPlayerAssigned = false;

		/// <summary>
		/// Gets or sets the player who owns this card at the end of the game.
		/// </summary>
		/// <value>The player who owns this card at the end of the game.</value>
		public Player FinalPlayer {
			get { return _finalPlayer; }
			set {
				if (_finalPlayerAssigned)
					throw new Exception ("The card can't be played twice.");

				if (isPlayable)
					throw new Exception ("The must be played to assign the final player");

				_finalPlayer = value;
			}
		}



		/// <summary>
		/// Gets a value indicating whether this <see cref="ChiamataLibrary.Card"/> is playable.
		/// </summary>
		/// <value><c>true</c> if is playable; otherwise, <c>false</c>.</value>
		public bool isPlayable{ get { return _playingTime == -1; } }

		#endregion

		#region Comparison

		/// <summary>
		/// Compares two instances of the <see cref="ChiamataLibrary.Card"/> class.
		/// </summary>
		/// <returns><c>1</c> if mainCard > other, <c>-0</c> if mainCard = other,<c>-1</c> otherwise.</returns>
		/// <param name="other">The card to compare the main one to.</param>
		public int CompareTo (Card other)
		{
			if (( (object) other ) == null)	 //null=null everything > null
				return 1;

			if (this.seme == other.seme)
				return (int) this.number - (int) other.number;

			if (Board.Instance.GameType != EnGameType.CARICHI) {
				if (this.seme == Board.Instance.CalledCard.seme)
					return 1;

				if (other.seme == Board.Instance.CalledCard.seme)
					return -1;
			}
			return 0;
		}

		/// <summary>
		/// Compares two instances of <see cref="ChiamataLibrary.Card"/>, returning true if are the same card.
		/// </summary>
		/// <returns><c>true</c> if are the same card, <c>false</c> otherwise.</returns>
		/// <param name="other">The <see cref="ChiamataLibrary.Card"/> to compare with the current <see cref="ChiamataLibrary.Card"/>.</param>
		public bool Equals (Card other)
		{
			return this.CompareTo (other) == 0;
		}


		/// <summary>
		/// Compares an instance of <see cref="ChiamataLibrary.Card"/>with an object, returning true if are the same card.
		/// </summary>
		/// <returns><c>true</c> if are the same card, <c>false</c> otherwise.</returns>
		/// <param name="other">The <see cref="System.Object"/> to compare with the current <see cref="ChiamataLibrary.Card"/>.</param>
		public override bool Equals (object other)
		{
			if (!( other is Card ))
				return false;

			return Equals ((Card) other);
		}

		/// <summary>Overrides the == operator</summary>
		/// <param name="b1">First card.</param>
		/// <param name="b2">Second card.</param>
		public static bool operator == (Card c1, Card c2)
		{
			if (( (object) c1 ) == null)
				return ( (object) c2 ) == null;

			return c1.CompareTo (c2) == 0;
		}

		/// <summary>Overrides the != operator</summary>
		/// <param name="b1">First card.</param>
		/// <param name="b2">Second card.</param>
		public static bool operator != (Card c1, Card c2)
		{
			if (( (object) c1 ) == null)
				return ( (object) c2 ) != null;

			return c1.CompareTo (c2) != 0;
		}

		/// <summary>Overrides the &gt= operator</summary>
		/// <param name="b1">First card.</param>
		/// <param name="b2">Second card.</param>
		public static bool operator >= (Card c1, Card c2)
		{
			if (( (object) c1 ) == null)	//null>=null  null < not null
				return  ( (object) c2 ) == null;

			return c1.CompareTo (c2) >= 0;
		}

		/// <summary>Overrides the &lt= operator</summary>
		/// <param name="b1">First card.</param>
		/// <param name="b2">Second card.</param>
		public static bool operator <= (Card c1, Card c2)
		{
			if (( (object) c1 ) == null)	//null <= everything
				return true;

			return c1.CompareTo (c2) <= 0;
		}

		/// <summary>Overrides the &gt operator</summary>
		/// <param name="b1">First card.</param>
		/// <param name="b2">Second card.</param>
		public static bool operator > (Card c1, Card c2)
		{
			if (( (object) c1 ) == null)	//null > nothing
				return false;

			return c1.CompareTo (c2) > 0;
		}

		/// <summary>Overrides the &lt operator</summary>
		/// <param name="b1">First card.</param>
		/// <param name="b2">Second card.</param>
		public static bool operator < (Card c1, Card c2)
		{
			if (( (object) c1 ) == null)	//null < not null
				return  !( ( (object) c2 ) == null );

			return c1.CompareTo (c2) < 0;
		}



		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.Card"/> class.
		/// </summary>
		/// <param name="number">The card's number.</param>
		/// <param name="seme">The card's seme.</param>
		/// <param name="iniziale">The player who initially owns the card.</param>
		public Card (EnNumbers number, EnSemi seme, Player initial)
		{
			if (!Board.Instance.isCreationPhase)
				throw new WrongPhaseException ("A player must be instantiated during the creation time", "Creation");
				
			this.seme = seme;	//set the seme
			this.number = number;	//set the number
			this.initialPlayer = initial;	//set the initial player
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.Card"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.Card"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("[Card: Seme={0}, Number={1}]", seme, number);
		}

		/// <summary>
		/// Override the GetHashCode method.
		/// </summary>
		/// <returns>The HashCode of this instance.</returns>
		public override int GetHashCode ()
		{//FIXME: To implement but never used
			return base.GetHashCode ();
		}



	}
}