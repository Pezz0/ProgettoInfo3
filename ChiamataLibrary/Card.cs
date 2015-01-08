using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// Card.
	/// </summary>
	public class Card:IComparable<Card>,IEquatable<Card>,IBTSendable<Card>
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
		/// Gets the value in point of this card.
		/// </summary>
		/// <returns>the value in point.</returns>
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

		public bool isCarico{ get { return number == EnNumbers.ASSE || number == EnNumbers.TRE; } }

		public bool isBiscola{ get { return seme == Board.Instance.Briscola; } }

		public bool isVestita{ get { return number == EnNumbers.RE || number == EnNumbers.CAVALLO || number == EnNumbers.FANTE; } }

		#endregion

		#region Owner's information

		/// <summary>
		/// The player who have this card at the start of the game.
		/// </summary>
		public readonly Player initialPlayer;

		/// <summary>
		/// The playing time.
		/// </summary>
		private int _playingTime = -1;

		/// <summary>
		/// Gets or sets the playing time.
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
		/// The player who have this card at the end of the game.
		/// </summary>
		private Player _finalPlayer = null;

		/// <summary>
		/// The value that indicate if the final player is decided.
		/// </summary>
		private bool _finalPlayerAssigned = false;

		/// <summary>
		/// Gets or sets the player who have this card at the end of the game.
		/// </summary>
		/// <value>The player who have this card at the end of the game.</value>
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
		/// Gets a value indicating whether this <see cref="Engine.Card"/> is playable.
		/// </summary>
		/// <value><c>true</c> if is playable; otherwise, <c>false</c>.</value>
		public bool isPlayable{ get { return _playingTime == -1; } }

		#endregion

		#region Comparison

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

		public bool Equals (Card other)
		{
			return this.CompareTo (other) == 0;
		}

		public override bool Equals (object other)
		{
			if (!( other is Card ))
				return false;

			return Equals ((Card) other);
		}

		public static bool operator == (Card c1, Card c2)
		{
			if (( (object) c1 ) == null)
				return ( (object) c2 ) == null;

			return c1.CompareTo (c2) == 0;
		}

		public static bool operator != (Card c1, Card c2)
		{
			if (( (object) c1 ) == null)
				return ( (object) c2 ) != null;

			return c1.CompareTo (c2) != 0;
		}

		public static bool operator >= (Card c1, Card c2)
		{
			if (( (object) c1 ) == null)	//null>=null  null < not null
				return  ( (object) c2 ) == null;

			return c1.CompareTo (c2) >= 0;
		}

		public static bool operator <= (Card c1, Card c2)
		{
			if (( (object) c1 ) == null)	//null <= everything
				return true;

			return c1.CompareTo (c2) <= 0;
		}

		public static bool operator > (Card c1, Card c2)
		{
			if (( (object) c1 ) == null)	//null > nothing
				return false;

			return c1.CompareTo (c2) > 0;
		}

		public static bool operator < (Card c1, Card c2)
		{
			if (( (object) c1 ) == null)	//null < not null
				return  !( ( (object) c2 ) == null );

			return c1.CompareTo (c2) < 0;
		}



		#endregion

		#region Bluetooth

		public int ByteArrayLenght { get { return 1; } }

		public byte[] toByteArray ()
		{
			return BitConverter.GetBytes (( (int) seme ) * Enum.GetValues (typeof (EnNumbers)).GetLength (0) + ( (int) number ));
		}

		public Card recreateFromByteArray (byte [] bytes)
		{
			EnSemi s = (EnSemi) ( BitConverter.ToChar (bytes, 0) / Enum.GetValues (typeof (EnNumbers)).GetLength (0) );
			EnNumbers n = (EnNumbers) ( BitConverter.ToChar (bytes, 1) % Enum.GetValues (typeof (EnNumbers)).GetLength (0) );

			return Board.Instance.getCard (s, n);
		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="Engine.Card"/> class.
		/// </summary>
		/// <param name="number">The card's number.</param>
		/// <param name="seme">The card's seme.</param>
		/// <param name="iniziale">Initial player.</param>
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

		public override int GetHashCode ()
		{//TODO: but is not necessary
			return base.GetHashCode ();
		}



	}
}