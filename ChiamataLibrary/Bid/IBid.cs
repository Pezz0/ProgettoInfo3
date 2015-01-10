using System;

namespace ChiamataLibrary
{
	public abstract class IBid: IComparable<IBid>,IEquatable<IBid>,IBTSendable<IBid>
	{
		/// <summary>
		/// The bidder.
		/// </summary>
		public readonly Player bidder;

		#region Bluetooth

		public int ByteArrayLenght { get { return 3; } }

		public abstract byte[] toByteArray ();

		public IBid recreateFromByteArray (byte [] bytes)
		{
			if (bytes [0] == 255)
				return new PassBid ();

			if (bytes [1] == 255)
				return new CarichiBid ((int) bytes [1]);

			return new NormalBid ((EnNumbers) bytes [1], (int) bytes [0]);
		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="Engine.IBid"/> class.
		/// </summary>
		/// <param name="bidder">Bidder.</param>
		public IBid (Player bidder)
		{
			this.bidder = bidder;
		}

		/// <summary>
		/// Initializes a new naked instance of the <see cref="ChiamataLibrary.IBid"/> class.
		/// </summary>
		public IBid ()
		{
			this.bidder = null;
		}

		/// <summary>
		/// Changes the bidder.
		/// </summary>
		/// <returns>The bidder.</returns>
		/// <param name="newBidder">New bidder.</param>
		public abstract IBid changeBidder (Player newBidder);

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

				if (b1.number != b2.number)	//the number is more important
					return b2.number - b1.number;	//the greater bid is the bid with the lesser number
				else
					return b1.point - b2.point;	//the greater bid is the with the greater amount of point
			}

			if (this is NormalBid && other is CarichiBid) {
				NormalBid b1 = (NormalBid) this;
				CarichiBid b2 = (CarichiBid) other;

				if (b1.number != EnNumbers.DUE)	//a normalBid can compete with a CarichiBid only if the number is two
					return -1;
				else
					return b1.point - b2.point;	//the greater bid is the with the greater amount of point
			}

			if (this is CarichiBid && other is NormalBid) {
				CarichiBid b1 = (CarichiBid) this;
				NormalBid b2 = (NormalBid) other;

				if (b2.number != EnNumbers.DUE)	//a normalBid can compete with a CarichiBid only if the number is two
					return 1;
				else
					return b1.point - b2.point;	//the greater bid is the with the greater amount of point
			}

			if (this is CarichiBid && other is CarichiBid) {
				CarichiBid b1 = (CarichiBid) this;
				CarichiBid b2 = (CarichiBid) other;

				return b1.point - b2.point;	//the greater bid is the with the greater amount of point
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


