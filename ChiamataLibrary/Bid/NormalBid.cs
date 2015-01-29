using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// Class representing a normal bid.
	/// </summary>
	public class NormalBid:NotPassBid
	{
		#region Information

		/// <summary>
		/// The bid's number.
		/// </summary>
		public readonly EnNumbers number;

		#endregion

		/// <summary>
		/// Changes the bidder of a specified bid.
		/// </summary>
		/// <returns>The bid.</returns>
		/// <param name="newBidder">The new bidder.</param>
		public override Bid changeBidder (Player newBidder)
		{
			return new NormalBid (newBidder, this.number, this.point);
		}

		/// <summary>
		/// Gets the next avaiable bid.
		/// </summary>
		/// <returns>The next avaiable bid.</returns>
		public override NotPassBid getNext ()
		{
			if (this.number == 0)
				return new NormalBid (bidder, 0, this.point + 1);
			else
				return new NormalBid (bidder, this.number - 1, 61);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.NormalBid"/> class.
		/// </summary>
		/// <param name="bidder">Bidder.</param>
		/// <param name="number">Number.</param>
		/// <param name="point">Points.</param>
		public NormalBid (Player bidder, EnNumbers number, int point) : base (bidder, point)
		{
			this.number = number;
		}

		/// <summary>
		/// Initializes a new naked instance of the <see cref="ChiamataLibrary.NormalBid"/> class.
		/// </summary>
		/// <param name="number">Number.</param>
		/// <param name="point">Points.</param>
		public NormalBid (EnNumbers number, int point) : base (point)
		{
			this.number = number;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.NormalBid"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.NormalBid"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("[NormalBid: Player:{0}, Number={1}, Point={2}]", bidder, this.number, this.point);
		}

	}
}

