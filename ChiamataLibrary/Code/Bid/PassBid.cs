using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// Class representing a pass bid.
	/// </summary>
	public class PassBid:BidBase
	{
		/// <summary>
		/// Changes the bidder of a specified bid.
		/// </summary>
		/// <returns>The bid.</returns>
		/// <param name="newBidder">The new bidder.</param>
		public override BidBase ChangeBidder (Player newBidder)
		{
			return new PassBid (newBidder);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.PassBid"/> class.
		/// </summary>
		/// <param name="bidder">Bidder.</param>
		public PassBid (Player bidder) : base (bidder)
		{
		}

		/// <summary>
		/// Initializes a new naked instance of the <see cref="ChiamataLibrary.PassBid"/> class.
		/// </summary>
		public PassBid () : base ()
		{
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.PassBid"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.PassBid"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("[PassBid: Player:{0}", bidder);
		}
	}
}

