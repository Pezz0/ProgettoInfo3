using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// Abstract class representing a possible winning bid.
	/// </summary>
	public abstract class NotPassBidBase:BidBase
	{
		/// <summary>
		/// The point.
		/// </summary>
		public readonly int point;

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.NotPassBid"/> class.
		/// </summary>
		/// <param name="bidder">Bidder.</param>
		/// <param name="point">Points.</param>
		protected NotPassBidBase (Player bidder, int point) : base (bidder)
		{
			this.point = point;
		}

		/// <summary>
		/// Initializes a new naked instance of the <see cref="ChiamataLibrary.NotPassBid"/> class.
		/// </summary>
		/// <param name="point">Points.</param>
		protected NotPassBidBase (int point) : base ()
		{
			this.point = point;
		}

		/// <summary>
		/// Gets the next avaiable bid.
		/// </summary>
		/// <returns>The next avaiable bid.</returns>
		public abstract NotPassBidBase GetNext ();
	}
}

