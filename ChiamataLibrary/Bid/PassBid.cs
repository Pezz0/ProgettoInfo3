using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// The pass
	/// </summary>
	public class PassBid:IBid
	{

		public override IBid changeBidder (Player newBidder)
		{
			return new PassBid (newBidder);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Engine.Pass"/> class.
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

