using System;

namespace ChiamataLibrary
{
	public abstract class NotPassBid:IBid
	{
		public readonly int point;

		public NotPassBid (Player bidder, int point) : base (bidder)
		{
			this.point = point;
		}

		public abstract NotPassBid getNext ();
	}
}

