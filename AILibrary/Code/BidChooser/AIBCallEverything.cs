using System;
using ChiamataLibrary;

namespace AILibrary
{
	/// <summary>
	/// Template for every AI that will call every time the bid is superior to his _lastBid
	/// See the documentation for major informations.
	/// </summary>
	public abstract class AIBCallEverything:IAIBidChooser
	{
		/// <summary>
		/// Boolean value that indicates wheter or not the AI can place a bid for a card that he has in its hand.
		/// </summary>
		protected bool _allowChiamataInMano = false;
		/// <summary>
		/// The lowest bid the AI will do in the auction. If the auction goes below this, the AI will do a <see cref="ChiamataLibrary.PassBid"/>.
		/// </summary>
		private Bid _lastBid;

		/// <summary>
		/// The <see cref="ChiamataLibrary.Player"/> instance representing the AI.
		/// </summary>
		protected Player _me;
		/// <summary>
		/// The seme that will be chosen by the AI if it wins the auction.
		/// </summary>
		protected EnSemi _seme;

		/// <summary>
		/// Method that returns which bid the AI wants to place in the auction.
		/// </summary>
		/// <returns>The bid.</returns>
		public Bid ChooseABid ()
		{
			NotPassBid bid = new NormalBid (EnNumbers.ASSE, 61);

			if (Board.Instance.currentAuctionWinningBid != null)
				bid = Board.Instance.currentAuctionWinningBid.GetNext ();

			if (!_allowChiamataInMano)
				bid = lowerTheBidUntilNotCIM (bid);

			if (_lastBid >= bid)
				return bid.ChangeBidder (_me);

			return new PassBid (_me);
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="me">The <see cref="ChiamataLibrary.Player"/> instance representing the AI.</param>
		/// <param name="seme">The seme that will be chosen by the AI if it wins the auction.</param>
		public void Setup (Player me, EnSemi seme)
		{
			this._me = me;
			this._seme = seme;
			_lastBid = SetLastBid ();
		}

		protected abstract NotPassBid SetLastBid ();

		/// <summary>
		/// Lowers the bid without calling a card that this player has in its hand.
		/// </summary>
		/// <returns>The lowered bid.</returns>
		/// <param name="bid">The current bid.</param>
		private NotPassBid lowerTheBidUntilNotCIM (NotPassBid bid)
		{
			if (bid is CarichiBid)
				return bid;

			NormalBid nb = (NormalBid) bid;

			while (Board.Instance.GetCard (_seme, nb.number).initialPlayer == _me && nb.number != EnNumbers.DUE && nb.point < 62)
				nb = (NormalBid) nb.GetNext ();

			return nb;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.IAIBCallEverything"/> class.
		/// </summary>
		public AIBCallEverything ()
		{
		}
	}
}

