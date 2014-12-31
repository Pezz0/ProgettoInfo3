using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// This abstract class rappresent an AI player
	/// </summary>
	public abstract class IAIPlayer
	{
		/// <summary>
		/// The player that is IA is using.
		/// </summary>
		private readonly int _me;

		public Player Me{ get { return Board.Instance.AllPlayers [_me]; } }

		/// <summary>
		/// The method that decide what bid have to be placed
		/// </summary>
		/// <returns>The A bid.</returns>
		protected abstract IBid PlaceABid ();

		/// <summary>
		/// The method that decide what card have to be played
		/// </summary>
		/// <returns>The A card.</returns>
		protected abstract Card PlayACard ();

		/// <summary>
		/// Chooses the seme when the IA win.
		/// </summary>
		/// <returns>The seme.</returns>
		protected abstract EnSemi chooseSeme ();

		public void startAuction ()
		{
			if (Board.Instance.ActiveAuctionPlayer == Me)
				Board.Instance.auctionPlaceABid (PlaceABid ());
		}

		public void controlYourTurnAuction (IBid bid)
		{
			 
			if (Board.Instance.isAuctionClosed) {
				if (Board.Instance.currentAuctionWinningBid.bidder == Me)
					Board.Instance.finalizeAuction (chooseSeme ());
			} else
				startAuction ();
			 
		}

		public void controlYourTurnPlayTime (Move move)
		{
			startPlayTime ();
		}

		public void startPlayTime ()
		{
			if (Board.Instance.ActivePlayer == Me)
				Board.Instance.PlayACard (Me, PlayACard ());
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.IAIPlayer"/> class.
		/// </summary>
		/// <param name="me">Me.</param>
		public IAIPlayer (int me)
		{
			this._me = me;
			Board.Instance.eventAuctionStarted += startAuction;
			Board.Instance.eventGameStarted += startPlayTime;
			 
			Board.Instance.eventSomeonePlaceABid += controlYourTurnAuction;
			Board.Instance.eventSomeonePlayACard += controlYourTurnPlayTime;
			 
			Board.Instance.eventIPlaceABid += controlYourTurnAuction;
			Board.Instance.eventIPlayACard += controlYourTurnPlayTime;
		}
		 
	}
}