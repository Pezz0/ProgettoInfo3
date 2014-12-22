using System;

namespace Engine
{
	public class NormalBid:IBid,IComparable<NormalBid>
	{
		#region Information

		/// <summary>
		/// The bid's number.
		/// </summary>
		private EnNumbers _number;

		/// <summary>
		/// The bid's point.
		/// </summary>
		private int _point;

		/// <summary>
		/// Gets the bid's number.
		/// </summary>
		/// <value>The number.</value>
		public EnNumbers Number { get { return _number; } }

		/// <summary>
		/// Gets the bid's point.
		/// </summary>
		/// <value>The point.</value>
		public int Point { get { return _point; } }

		#endregion

		#region Comparison

		public int CompareTo (NormalBid other)
		{
			if (( (object) other ) == null)
				return 1;	//everything is greater than null (at least I think so)

			if (this.Number != other.Number)	//the number is more important
				return other.Number - this.Number;	//the greater bid is the bid with the lesser number
			else
				return this.Point - other.Point;	//the greater bid is the with the greater amount of point
		
		}

		public bool Equals (NormalBid other)
		{
			//the null control is done by the compareTo
			return this.CompareTo (other) == 0;
		}

		public override bool Equals (object other)
		{
			//the null control is done by the compareTo
			if (!( other is NormalBid ))
				return false;

			return this.Equals ((NormalBid) other);
		}

		public static bool operator == (NormalBid bid1, NormalBid bid2)
		{
			if (( (object) bid1 ) == null)	//if bid1 is null we cannot invoke the compareTo but null is equals only with an other null
				return ( (object) bid2 ) == null;

			return bid1.CompareTo (bid2) == 0;
		}

		public static bool operator != (NormalBid bid1, NormalBid bid2)
		{
			if (( (object) bid1 ) == null)	//if bid1 is null we cannot invoke the compareTo but null is equals only with an other null
				return !( ( (object) bid2 ) == null );

			return bid1.CompareTo (bid2) != 0;
		}

		public static bool operator > (NormalBid bid1, NormalBid bid2)
		{
			if (( (object) bid1 ) == null)	//if bid1 is null we cannot invoke the compareTo but everything is greater than null, bid1=null -> bid2 > bid1
				return false;

			return bid1.CompareTo (bid2) > 0;
		}

		public static bool operator < (NormalBid bid1, NormalBid bid2)
		{
			if (( (object) bid1 ) == null)	//if bid1 is null we cannot invoke the compareTo but everything except null is greater than null
				return !( ( (object) bid2 ) == null );

			return bid1.CompareTo (bid2) < 0;
		}

		public static bool operator >= (NormalBid bid1, NormalBid bid2)
		{
			if (( (object) bid1 ) == null)	//if bid1 is null we cannot invoke the compareTo but everything except null is greater than null
				return  ( (object) bid2 ) == null;

			return bid1.CompareTo (bid2) >= 0;
		}

		public static bool operator <= (NormalBid bid1, NormalBid bid2)
		{
			if (( (object) bid1 ) == null)	//if bid1 is null we cannot invoke the compareTo but everything is greater than null, bid1=null -> bid1 <= bid2
				return true;

			return bid1.CompareTo (bid2) <= 0;
		}

		#endregion

		public NormalBid (Player bidder, EnNumbers number, int point) : base (bidder)
		{
			_number = number;
			_point = point;
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public override string ToString ()
		{
			return string.Format ("[NormalBid: Number={0}, Point={1}]", Number, Point);
		}

	}
}

