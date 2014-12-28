using System;

namespace ChiamataLibrary
{
	public abstract class IBid: IComparable<IBid>,IEquatable<IBid>,IBTSendable<IBid>
	{
		/// <summary>
		/// The bidder.
		/// </summary>
		private Player _bidder;

		/// <summary>
		/// Gets the bidder.
		/// </summary>
		/// <value>The bidder.</value>
		public Player Bidder { get { return _bidder; } }

		#region Bluetooth

		public  int ByteArrayLenght { get { return 3; } }

		public abstract byte[] toByteArray ();

		public IBid ricreateFromByteArray (byte [] bytes)
		{
			if (bytes [1] == 255)
				return new PassBid (Board.Instance.getPlayer (bytes [0]));

			if (bytes [2] == 255)
				return new BidCarichi (Board.Instance.getPlayer (bytes [0]), (int) bytes [1]);

			return new NormalBid (Board.Instance.getPlayer (bytes [0]), (EnNumbers) bytes [2], (int) bytes [1]);
		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="Engine.IBid"/> class.
		/// </summary>
		/// <param name="bidder">Bidder.</param>
		public IBid (Player bidder)
		{
			_bidder = bidder;
		}

		#region Comparison

		public int CompareTo (IBid other)
		{
			if (( (object) other ) == null)	 //null=null everything > null
				return 1;

			if (this is PassBid)	//pass=pass pass<everything
				return ( other is PassBid ) ? 0 : -1;
				
			if (other is PassBid)
				return 1;
				
			if (this is NormalBid && other is NormalBid) {
				NormalBid b1 = (NormalBid) this;
				NormalBid b2 = (NormalBid) other;

				if (b1.Number != b2.Number)	//the number is more important
					return b2.Number - b1.Number;	//the greater bid is the bid with the lesser number
				else
					return b1.Point - b2.Point;	//the greater bid is the with the greater amount of point
			}

			if (this is NormalBid && other is BidCarichi) {
				NormalBid b1 = (NormalBid) this;
				BidCarichi b2 = (BidCarichi) other;

				if (b1.Number != EnNumbers.DUE)	//a normalBid can compete with a CarichiBid only if the number is two
					return -1;
				else
					return b1.Point - b2.Point;	//the greater bid is the with the greater amount of point
			}

			if (this is BidCarichi && other is NormalBid) {
				BidCarichi b1 = (BidCarichi) this;
				NormalBid b2 = (NormalBid) other;

				if (b2.Number != EnNumbers.DUE)	//a normalBid can compete with a CarichiBid only if the number is two
					return 1;
				else
					return b1.Point - b2.Point;	//the greater bid is the with the greater amount of point
			}

			if (this is BidCarichi && other is BidCarichi) {
				BidCarichi b1 = (BidCarichi) this;
				BidCarichi b2 = (BidCarichi) other;

				return b1.Point - b2.Point;	//the greater bid is the with the greater amount of point
			}

			throw new Exception ("Some error occur, this path shoudn't be executed");
		}

		public bool Equals (IBid other)
		{
			return this.CompareTo (other) == 0;
		}

		public override bool Equals (object other)
		{
			if (!( other is IBid ))
				return false;

			return Equals ((IBid) other);
		}

		public static bool operator == (IBid b1, IBid b2)
		{
			if (( (object) b1 ) == null)
				return ( (object) b2 ) == null;

			return b1.CompareTo (b2) == 0;
		}

		public static bool operator != (IBid b1, IBid b2)
		{
			if (( (object) b1 ) == null)
				return ( (object) b2 ) != null;

			return b1.CompareTo (b2) != 0;
		}

		public static bool operator >= (IBid b1, IBid b2)
		{
			if (( (object) b1 ) == null)	//null>=null  null < not null
				return  ( (object) b2 ) == null;

			return b1.CompareTo (b2) >= 0;
		}

		public static bool operator <= (IBid b1, IBid b2)
		{
			if (( (object) b1 ) == null)	//null <= everything
				return true;

			return b1.CompareTo (b2) <= 0;
		}

		public static bool operator > (IBid b1, IBid b2)
		{
			if (( (object) b1 ) == null)	//null > nothing
				 return false;

			return b1.CompareTo (b2) > 0;
		}

		public static bool operator < (IBid b1, IBid b2)
		{
			if (( (object) b1 ) == null)	//null < not null
				return  !( ( (object) b2 ) == null );

			return b1.CompareTo (b2) < 0;
		}



		#endregion

		public override int GetHashCode ()
		{//TODO: but is not necessary
			return base.GetHashCode ();
		}

	}
}


