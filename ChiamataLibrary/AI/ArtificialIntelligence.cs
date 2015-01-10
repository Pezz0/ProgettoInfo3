using System;

namespace ChiamataLibrary
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
		/// <returns>The A bid.</returns>
		/// <param name="me">Me.</param>
		/// <param name="seme">Seme.</param>
		IBid chooseABid ();

		/// <summary>
		/// Setup this instance.
		/// </summary>
		void setup (Player me, EnSemi seme);
	}

	/// <summary>
	///  Interface implemented by the seme chooser.
	/// </summary>
	public interface IAISemeChooser
	{
		/// <summary>
		/// Chooses the seme.
		/// </summary>
		/// <returns>The seme.</returns>
		/// <param name="me">Me.</param>
		EnSemi? chooseSeme ();

		/// <summary>
		/// Setup this instance.
		/// </summary>
		void setup (Player me);
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
		Card chooseCard ();

		/// <summary>
		/// Setup this instance.
		/// </summary>
		void setup (Player me);
	}

	#endregion

	#region Class
	/// <summary>
	/// Artificial intelligence.
	/// </summary>
	public class ArtificialIntelligence:IPlayerController
	{
		/// <summary>
		/// The bid chooser.
		/// </summary>
		private IAIBidChooser _bidChooser;

		/// <summary>
		/// The seme chooser.
		/// </summary>
		private IAISemeChooser _semeChooser;

		/// <summary>
		/// The card chooser.
		/// </summary>
		private IAICardChooser _cardChooser;

		/// <summary>
		/// The player that is controlled by this AI.
		/// </summary>
		public readonly Player me;

		private EnSemi _seme;

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.ArtificialIntelligence"/> class.
		/// </summary>
		/// <param name="me">Me.</param>
		/// <param name="bid">Bid.</param>
		/// <param name="seme">Seme.</param>
		/// <param name="card">Card.</param>
		public ArtificialIntelligence (Player me, IAIBidChooser bid, IAISemeChooser seme, IAICardChooser card)
		{
			this.me = me;
			this._cardChooser = card;
			this._semeChooser = seme;
			this._bidChooser = bid;

			me.Controller = this;

			Board.Instance.eventAuctionStart += startAuction;
			Board.Instance.eventPlaytimeStart += startGame;
		}

		public bool isReady { get { return true; } }

		public bool isActive {
			get {
				return true;
			}
		}

		private void startAuction ()
		{
			_semeChooser.setup (me);
			_seme = _semeChooser.chooseSeme ().Value;
			_bidChooser.setup (me, _seme);
		}

		private void startGame ()
		{
			_cardChooser.setup (me);
		}


		public IBid chooseBid ()
		{
			IBid bid = _bidChooser.chooseABid ();
			if (bid == null)
				throw new Exception ("Null bid");

			return bid;
		}

		public EnSemi? chooseSeme ()
		{
			return _semeChooser.chooseSeme ();
		}

		public Card chooseCard ()
		{
			Card card = _cardChooser.chooseCard ();
			if (card == null)
				throw new Exception ("Null card");

			return card;
		}
	}
	#endregion
}

