using System;

namespace ChiamataLibrary
{
	public class PassBid:IBid
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Engine.Pass"/> class.
		/// </summary>
		/// <param name="bidder">Bidder.</param>
		public PassBid (Player bidder) : base (bidder)
		{
		}

		public override string ToString ()
		{
			return string.Format ("[PassBid: Player:{0}", Bidder);
		}
	}
}

