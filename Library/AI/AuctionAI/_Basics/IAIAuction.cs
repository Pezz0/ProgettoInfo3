using System;
using System.Collections.Generic;

namespace ChiamataLibrary
{
	public abstract class IAIAuction :IArtificialIntelligence
	{
		protected List<Card> [] _onHand;

		protected override void setup ()
		{
			int nSemi = Enum.GetValues (typeof (EnSemi)).GetLength (0);

			_onHand = new List<Card>[nSemi];
			for (int i = 0; i < nSemi; i++)
				_onHand [i] = Board.Instance.getPlayerHand (me).FindAll (delegate(Card c) {
					return c.seme == (EnSemi) i;
				});
		}

		protected abstract IBid placeABid ();

		protected abstract EnSemi chooseSeme ();

		public void startAuction ()
		{
			setup ();

			if (Board.Instance.ActiveAuctionPlayer == me)
				Board.Instance.auctionPlaceABid (placeABid ());
		}

		public void controlYourTurnAuction (IBid bid)
		{
			if (Board.Instance.isAuctionClosed) {
				if (Board.Instance.currentAuctionWinningBid.bidder == me)
					Board.Instance.finalizeAuction (chooseSeme ());
			} else if (Board.Instance.ActiveAuctionPlayer == me)
				Board.Instance.auctionPlaceABid (placeABid ());

		}


		public IAIAuction (Player me) : base (me)
		{
			Board.Instance.eventAuctionStarted += startAuction;

			Board.Instance.eventSomeonePlaceABid += controlYourTurnAuction;

			Board.Instance.eventIPlaceABid += controlYourTurnAuction;

		}
	}
}

