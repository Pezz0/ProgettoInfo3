using System;
using ChiamataLibrary;

namespace AILibrary
{
	#region Interfaces

	/// <summary>
	/// Interface implemented by the bid chooser.
	/// </summary>
	public interface IAIBidChooser
	{
		/// <summary>
		/// Chooses A bid.
		/// </summary>
		/// <returns>The bid.</returns>
		/// <param name="me">Me.</param>
		/// <param name="seme">Seme.</param>
		BidBase ChooseABid ();

		/// <summary>
		/// Setup this instance.
		/// </summary>
		void Setup (Player me, EnSemi seme);
	}

	/// <summary>
	///  Interface implemented by the seme chooser.
	/// </summary>
	public interface IAISemeChooser
	{
		/// <summary>
		/// Setup this instance.
		/// </summary>
		EnSemi ChooseSeme (Player me);
	}

	/// <summary>
	///  Interface implemented by the card chooser.
	/// </summary>
	public interface IAICardChooser
	{
		/// <summary>
		/// Chooses the card.
		/// </summary>
		/// <returns>The card.</returns>
		/// <param name="me">Me.</param>
		Card ChooseCard ();

		/// <summary>
		/// Setup this instance.
		/// </summary>
		void Setup (Player me);
	}

	#endregion

	#region Class
	/// <summary>
	/// Controller for a Bluetooth player.
	/// </summary>
	public class AIPlayerController:IPlayerController
	{
		/// <summary>
		/// The bid chooser.
		/// </summary>
		private readonly IAIBidChooser _bidChooser;

		/// <summary>
		/// The seme chooser.
		/// </summary>
		private readonly IAISemeChooser _semeChooser;

		/// <summary>
		/// The card chooser.
		/// </summary>
		private readonly IAICardChooser _cardChooser;

		/// <summary>
		/// The player that is controlled by this AI.
		/// </summary>
		public readonly Player me;

		private EnSemi _seme;

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.ArtificialIntelligence"/> class.
		/// </summary>
		/// <param name="me">The <see cref="ChiamataLibrary.Player"/> instance representing the AI.</param>
		/// <param name="bid">The implementation of <see cref="ChiamataLibrary.IAIBidChooser"/> that will be used to choose the bids for this AI.</param>
		/// <param name="seme">The implementation of <see cref="ChiamataLibrary.IAISemeChooser"/> that will be used to choose the seme for this AI.</param>
		/// <param name="card">The implementation of <see cref="ChiamataLibrary.IAICardChooser"/> that will be used to choose the cards played by this AI.</param>
		public AIPlayerController (Player me, IAIBidChooser bid, IAISemeChooser seme, IAICardChooser card)
		{
			this.me = me;
			this._cardChooser = card;
			this._semeChooser = seme;
			this._bidChooser = bid;

			me.Controller = this;

			Board.Instance.eventAuctionStart += StartAuction;
			Board.Instance.eventPlaytimeStart += StartGame;
		}

		/// <summary>
		/// Notifies the AI that the auction is starting and sets up the seme and bid choosers.
		/// </summary>
		private void StartAuction ()
		{
			_seme = _semeChooser.ChooseSeme (me);
			_bidChooser.Setup (me, _seme);
		}

		/// <summary>
		/// Notifies the AI that the game is starting and sets up the card chooser.
		/// </summary>
		private void StartGame ()
		{
			_cardChooser.Setup (me);
		}

		/// <summary>
		/// Method that returns which bid the AI wants to place in the auction.
		/// </summary>
		/// <returns>The bid.</returns>
		public BidBase ChooseBid ()
		{
			BidBase bid = _bidChooser.ChooseABid ();
			if (bid == null)
				return new PassBid (me);

			return  bid;
		}

		/// <summary>
		/// Method that returns which seme the AI wants to choose.
		/// </summary>
		/// <returns>The seme that will be chosen by this AI.</returns>
		public EnSemi? ChooseSeme ()
		{
			return _seme;
		}

		/// <summary>
		/// Method that returns which card the AI wants to play.
		/// </summary>
		/// <returns>The card.</returns>
		public Card ChooseCard ()
		{
			Card card = _cardChooser.ChooseCard ();
			if (card == null)
				return me.GetScartino ();

			return card;
		}
	}
	#endregion
}

