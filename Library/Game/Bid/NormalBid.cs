using System;

namespace ChiamataLibrary
{
	public class NormalBid:IBid
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

		public NormalBid (Player bidder, EnNumbers number, int point) : base (bidder)
		{
			_number = number;
			_point = point;
		}

		public override string ToString ()
		{
			return string.Format ("[NormalBid: Number={0}, Point={1}]", Number, Point);
		}

	}
}

