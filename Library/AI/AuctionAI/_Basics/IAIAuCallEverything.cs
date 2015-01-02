using System;

namespace ChiamataLibrary
{
	public class IAIAuCallEverything:IAIAuction
	{
			
		protected bool _allowChiamataInMano = false;
		protected IBid _lastBid;
		protected EnSemi _seme;

		protected override IBid placeABid ()
		{
			if (_lastBid is PassBid)
				return new PassBid (me);

			NotPassBid bid = new NormalBid (me, EnNumbers.ASSE, 61);

			if (Board.Instance.currentAuctionWinningBid != null)
				bid = Board.Instance.currentAuctionWinningBid.getNext ();

			if (!_allowChiamataInMano)
				bid = lowerTheBidUntilNotCIM (bid);

			if (_lastBid >= bid)
				return bid.changeBidder (me);

			return new PassBid (me);
		}

		protected override EnSemi chooseSeme ()
		{
			return _seme;
		}

		protected NotPassBid lowerTheBidUntilNotCIM (NotPassBid bid)
		{
			if (bid is CarichiBid)
				return bid;

			NormalBid nb = (NormalBid) bid;

			while (Board.Instance.getCard (_seme, nb.number).initialPlayer == me && nb.number == EnNumbers.DUE)
				nb = (NormalBid) nb.getNext ();

			return nb;
		}


		public IAIAuCallEverything (Player me) : base (me)
		{
		}
	}
}

