using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// An AI that call every card > lastbid
	/// </summary>
	public abstract class IAIBCallEverything:IAIBidChooser
	{
		protected bool _allowChiamataInMano = false;
		protected IBid _lastBid;

		protected Player _me;
		protected EnSemi _seme;

		public IBid chooseABid ()
		{
			if (_lastBid is PassBid)
				return new PassBid (_me);

			NotPassBid bid = new NormalBid (_me, EnNumbers.ASSE, 61);

			if (Board.Instance.currentAuctionWinningBid != null)
				bid = Board.Instance.currentAuctionWinningBid.getNext ();

			if (!_allowChiamataInMano)
				bid = lowerTheBidUntilNotCIM (bid);

			if (_lastBid >= bid)
				return bid.changeBidder (_me);

			return new PassBid (_me);
		}

		public virtual void setup (Player me, EnSemi seme)
		{
			this._me = me;
			this._seme = seme;
		}

		protected NotPassBid lowerTheBidUntilNotCIM (NotPassBid bid)
		{
			if (bid is CarichiBid)
				return bid;

			NormalBid nb = (NormalBid) bid;

			while (Board.Instance.getCard (_seme, nb.number).initialPlayer == _me && nb.number != EnNumbers.DUE)
				nb = (NormalBid) nb.getNext ();

			return nb;
		}


		public IAIBCallEverything ()
		{
		}
	}
}

