using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// Abstract class representing a general bid.
	/// </summary>
	public abstract class Bid: IComparable<Bid>,IEquatable<Bid>
	{
		/// <summary>
		/// The bidder.
		/// </summary>
		public readonly Player bidder;

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.Bid"/> class.
		/// </summary>
		/// <param name="bidder">Bidder.</param>
		public Bid (Player bidder)
		{
			this.bidder = bidder;
		}

		/// <summary>
		/// Initializes a new naked instance of the <see cref="ChiamataLibrary.Bid"/> class.
		/// </summary>
		public Bid ()
		{
			this.bidder = null;
		}

		/// <summary>
		/// Changes the bidder of a specified bid.
		/// </summary>
		/// <returns>The bid.</returns>
		/// <param name="newBidder">The new bidder.</param>
		public abstract Bid changeBidder (Player newBidder);

		#region Comparison

		/// <summary>
		/// Compares two instances of the <see cref="ChiamataLibrary.IBid"/> class.
		/// </summary>
		/// <returns><c>1</c> if mainBid > other, <c>-0</c> if mainBid = other,<c>-1</c> otherwise.</returns>
		/// <param name="other">The bid to compare the main one to.</param>
		public int CompareTo (Bid other)
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

		/// <summary>
		/// Compares two instances of <see cref="ChiamataLibrary.IBid"/>, returning true if are the same bid.
		/// </summary>
		/// <returns><c>true</c> if are the same bid, <c>false</c> otherwise.</returns>
		/// <param name="other">The <see cref="ChiamataLibrary.IBid"/> to compare with the current <see cref="ChiamataLibrary.IBid"/>.</param>
		public bool Equals (Bid other)
		{
			return this.CompareTo (other) == 0;
		}


		/// <summary>
		/// Compares an instance of <see cref="ChiamataLibrary.IBid"/>with an object, returning true if are the same bid.
		/// </summary>
		/// <returns><c>true</c> if are the same bid, <c>false</c> otherwise.</returns>
		/// <param name="other">The <see cref="System.Object"/> to compare with the current <see cref="ChiamataLibrary.IBid"/>.</param>
		public override bool Equals (object other)
		{
			if (!( other is Bid ))
				return false;

			return Equals ((Bid) other);
		}

		/// <summary>Overrides the == operator</summary>
		/// <param name="b1">First bid.</param>
		/// <param name="b2">Second bid.</param>
		public static bool operator == (Bid b1, Bid b2)
		{
			if (( (object) b1 ) == null)
				return ( (object) b2 ) == null;

			return b1.CompareTo (b2) == 0;
		}

		/// <summary>Overrides the != operator</summary>
		/// <param name="b1">First bid.</param>
		/// <param name="b2">Second bid.</param>
		public static bool operator != (Bid b1, Bid b2)
		{
			if (( (object) b1 ) == null)
				return ( (object) b2 ) != null;

			return b1.CompareTo (b2) != 0;
		}

		/// <summary>Overrides the &gt= operator</summary>
		/// <param name="b1">First bid.</param>
		/// <param name="b2">Second bid.</param>
		public static bool operator >= (Bid b1, Bid b2)
		{
			if (( (object) b1 ) == null)	//null>=null  null < not null
				return  ( (object) b2 ) == null;

			return b1.CompareTo (b2) >= 0;
		}

		/// <summary>Overrides the &lt= operator</summary>
		/// <param name="b1">First bid.</param>
		/// <param name="b2">Second bid.</param>
		public static bool operator <= (Bid b1, Bid b2)
		{
			if (( (object) b1 ) == null)	//null <= everything
				return true;

			return b1.CompareTo (b2) <= 0;
		}

		/// <summary>Overrides the &gt operator</summary>
		/// <param name="b1">First bid.</param>
		/// <param name="b2">Second bid.</param>
		public static bool operator > (Bid b1, Bid b2)
		{
			if (( (object) b1 ) == null)	//null > nothing
				 return false;

			return b1.CompareTo (b2) > 0;
		}

		/// <summary>Overrides the &lt operator</summary>
		/// <param name="b1">First bid.</param>
		/// <param name="b2">Second bid.</param>
		public static bool operator < (Bid b1, Bid b2)
		{
			if (( (object) b1 ) == null)	//null < not null
				return  !( ( (object) b2 ) == null );

			return b1.CompareTo (b2) < 0;
		}



		#endregion

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


