using System;
using System.Collections.Generic;

namespace ChiamataLibrary
{
	public abstract class IAIAuction :IArtificialIntelligence
	{
		protected List<Card> [] _onHand;

		private bool _active;

		public override bool Active {
			get { return _active; }
			set {
				_active = value;
				if (_active) {
					Board.Instance.eventAuctionStarted += startAuction;
					Board.Instance.eventSomeonePlaceABid += controlYourTurnAuction;
					Board.Instance.eventIPlaceABid += controlYourTurnAuction;
				} else {
					Board.Instance.eventAuctionStarted -= startAuction;
					Board.Instance.eventSomeonePlaceABid -= controlYourTurnAuction;
					Board.Instance.eventIPlaceABid -= controlYourTurnAuction;
				}
			}
		}

		protected override void setup ()
		{
			_onHand = new List<Card>[Board.Instance.nSemi];
			for (int i = 0; i < Board.Instance.nSemi; i++)
				_onHand [i] = Board.Instance.getPlayerHand (me).FindAll (delegate(Card c) {
					return c.seme == (EnSemi) i;
				});
		}

		protected abstract IBid placeABid ();

		protected abstract EnSemi chooseSeme ();

		public void startAuction ()
		{
			setup ();

			if (Board.Instance.ActiveAuctionPlayer == me) {
				IBid bid = placeABid ();
				Board.Instance.auctionPlaceABid (placeABid ());
				if (bid is PassBid)
					Active = false;
			}
		}

		public void controlYourTurnAuction (IBid bid)
		{
			if (Board.Instance.isAuctionClosed) {
				if (Board.Instance.currentAuctionWinningBid.bidder == me)
					Board.Instance.finalizeAuction (chooseSeme ());
			} else if (Board.Instance.ActiveAuctionPlayer == me)
				Board.Instance.auctionPlaceABid (placeABid ());

		}

		public void finish ()
		{
			Active = false;
		}


		public IAIAuction (Player me) : base (me)
		{
			Active = true;
			Board.Instance.eventAuctionEnded += finish;
		}



	}
}

