using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// A possibile winning bid
	/// </summary>
	public abstract class NotPassBid:IBid
	{
		/// <summary>
		/// The point.
		/// </summary>
		public readonly int point;

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.NotPassBid"/> class.
		/// </summary>
		/// <param name="bidder">Bidder.</param>
		/// <param name="point">Point.</param>
		public NotPassBid (Player bidder, int point) : base (bidder)
		{
			this.point = point;
		}

		/// <summary>
		/// Initializes a new naked instance of the <see cref="ChiamataLibrary.NotPassBid"/> class.
		/// </summary>
		/// <param name="point">Point.</param>
		public NotPassBid (int point) : base ()
		{
			this.point = point;
		}

		/// <summary>
		/// Gets the next bid.
		/// </summary>
		/// <returns>The next.</returns>
		public abstract NotPassBid getNext ();
	}
}

