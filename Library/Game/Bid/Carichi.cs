using System;

namespace ChiamataLibrary
{
	public class BidCarichi:IBid
	{
		#region Information

		/// <summary>
		/// The bid's point.
		/// </summary>
		private int _point;

		/// <summary>
		/// Gets the bid's point.
		/// </summary>
		/// <value>The point.</value>
		public int Point { get { return _point; } }

		#endregion

		public BidCarichi (Player bidder, int point) : base (bidder)
		{
			_point = point;
		}
	}
}

