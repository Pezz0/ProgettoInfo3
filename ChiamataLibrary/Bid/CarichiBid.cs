using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// Carichi bid.
	/// </summary>
	public class CarichiBid:NotPassBid
	{
		public override NotPassBid getNext ()
		{
			return new CarichiBid (bidder, point + 1);
		}

		public override IBid changeBidder (Player newBidder)
		{
			return new CarichiBid (newBidder, this.point);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.CarichiBid"/> class.
		/// </summary>
		/// <param name="bidder">Bidder.</param>
		/// <param name="point">Point.</param>
		public CarichiBid (Player bidder, int point) : base (bidder, point)
		{
		}

		/// <summary>
		/// Initializes a new naked instance of the <see cref="ChiamataLibrary.CarichiBid"/> class.
		/// </summary>
		/// <param name="point">Point.</param>
		public CarichiBid (int point) : base (point)
		{
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.CarichiBid"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.CarichiBid"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("[BidCarichi:Player:{0}, Point={1}]", bidder, point);
		}
	}
}

