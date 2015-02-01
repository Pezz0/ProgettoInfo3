using System;
using System.Runtime.Serialization;

namespace ChiamataLibrary
{
	#region WrongPhaseException
	/// <summary>
	/// Thrown when a different phase was expected.
	/// </summary>
	internal class WrongPhaseException:Exception
	{

		/// <summary>
		/// The expected phase.
		/// </summary>
		private readonly string _expectedPhase;

		/// <summary>
		/// Gets the expected phase.
		/// </summary>
		/// <value>The expected phase.</value>
		public string ExpectedPhase { get { return _expectedPhase; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.WrongPhaseException"/> class.
		/// </summary>
		/// <param name="expected">Expected phase.</param>
		public WrongPhaseException (string expected) : base ()
		{
			_expectedPhase = expected;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.WrongPhaseException"/> class.
		/// </summary>
		/// <param name="message">Exception description</param>
		/// <param name="expected">Expected phase.</param>
		public WrongPhaseException (string message, string expected) : base (message)
		{
			_expectedPhase = expected;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.WrongPhaseException"/> class.
		/// </summary>
		/// <param name="message">Exception description</param>
		/// <param name="inner">Exception inner cause</param>
		/// <param name="expected">Expected phase.</param>
		public WrongPhaseException (string message, Exception inner, string expected) : base (message, inner)
		{
			_expectedPhase = expected;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.WrongPhaseException"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.WrongPhaseException"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("[WrongPhaseException(" + Message + "): ExpectedPhase={0}]", ExpectedPhase);
		}

	}

	#endregion

	#region WrongBiddingPlayerException
	/// <summary>
	/// Thrown when an unexpected bid is presented in the auction.
	/// </summary>
	internal class WrongBiddingPlayerException:Exception
	{
		/// <summary>
		/// The player making the bid.
		/// </summary>
		private readonly Player _player;

		/// <summary>
		/// Gets the player.
		/// </summary>
		/// <value>The player.</value>
		public Player Player { get { return _player; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.WrongBiddingPlayerException"/> class.
		/// </summary>
		/// <param name="player">The player who placed the wrong bid</param>
		public WrongBiddingPlayerException (Player player) : base ()
		{
			_player = player;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.WrongBiddingPlayerException"/> class.
		/// </summary>
		/// <param name="message">Exception description</param>
		/// <param name="player">The player who placed the wrong bid</param>
		public WrongBiddingPlayerException (string message, Player player) : base (message)
		{
			_player = player;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.WrongBiddingPlayerException"/> class.
		/// </summary>
		/// <param name="message">Exception description</param>
		/// <param name="inner">Exception inner cause</param>
		/// <param name="player">The player who placed the wrong bid</param>
		public WrongBiddingPlayerException (string message, Exception inner, Player player) : base (message, inner)
		{
			_player = player;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.WrongBiddingPlayerException"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.WrongBiddingPlayerException"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("[WrongBiddingPlayerException(" + Message + "): Player={0}]", Player);
		}

	}

	#endregion

	#region BidNotEnoughException
	/// <summary>
	/// Thrown when the bid is superior to the currently winning one.
	/// </summary>
	internal class BidNotEnoughException:Exception
	{
		/// <summary>
		/// The bid.
		/// </summary>
		private readonly BidBase _bid;

		/// <summary>
		/// Gets the bid.
		/// </summary>
		/// <value>The bid.</value>
		public BidBase Bid { get { return _bid; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.BidNotEnoughException"/> class.
		/// </summary>
		/// <param name="bid">The bid.</param>
		public BidNotEnoughException (BidBase bid) : base ()
		{
			_bid = bid;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.BidNotEnoughException"/> class.
		/// </summary>
		/// <param name="message">Exception description</param>
		/// <param name="bid">The bid.</param>
		public BidNotEnoughException (string message, BidBase bid) : base (message)
		{
			_bid = bid;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.BidNotEnoughException"/> class.
		/// </summary>
		/// <param name="message">Exception description</param>
		/// <param name="inner">Exception inner cause</param>
		/// <param name="bid">The bid.</param>
		public BidNotEnoughException (string message, Exception inner, BidBase bid) : base (message, inner)
		{
			_bid = bid;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.BidNotEnoughException"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.BidNotEnoughException"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("[BidNotEnoughException(" + Message + "): Bid={0}]", Bid);
		}
	}

	#endregion

	#region WrongPlayerException
	/// <summary>
	/// Thrown when a player plays in the wrong turn.
	/// </summary>
	internal class WrongPlayerException:Exception
	{
		/// <summary>
		/// The player.
		/// </summary>
		private readonly Player _player;

		/// <summary>
		/// Gets the player.
		/// </summary>
		/// <value>The player.</value>
		public Player Player { get { return _player; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.WrongPlayerException"/> class.
		/// </summary>
		/// <param name="player">The player who played in the wrong turn</param>
		public WrongPlayerException (Player player) : base ()
		{
			_player = player;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.WrongPlayerException"/> class.
		/// </summary>
		/// <param name="message">Exception description</param>
		/// <param name="player">The player who played in the wrong turn</param>
		public WrongPlayerException (string message, Player player) : base (message)
		{
			_player = player;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.WrongPlayerException"/> class.
		/// </summary>
		/// <param name="message">Exception description</param>
		/// <param name="inner">Exception inner cause</param>
		/// <param name="player">The player who played in the wrong turn</param>
		public WrongPlayerException (string message, Exception inner, Player player) : base (message, inner)
		{
			_player = player;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.WrongPlayerException"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.WrongPlayerException"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("[WrongPlayerException: Player={0}]", Player);
		}

	}

	#endregion

	#region WrongCardException
	/// <summary>
	/// Wrong card exception.
	/// </summary>
	internal class WrongCardException:Exception
	{
		/// <summary>
		/// The wrong card.
		/// </summary>
		private readonly Card _card;

		/// <summary>
		/// Gets the wrong card.
		/// </summary>
		/// <value>The card.</value>
		public Card Card { get { return _card; } }


		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.WrongCardException"/> class.
		/// </summary>
		/// <param name="card">The wrong card</param>
		public WrongCardException (Card card) : base ()
		{
			_card = card;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.WrongCardException"/> class.
		/// </summary>
		/// <param name="message">Exception description</param>
		/// <param name="card">The wrong card</param>
		public WrongCardException (string message, Card card) : base (message)
		{
			_card = card;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.WrongCardException"/> class.
		/// </summary>
		/// <param name="message">Exception description</param>
		/// <param name="inner">Exception inner cause</param>
		/// <param name="card">The wrong card</param>
		public WrongCardException (string message, Exception inner, Card card) : base (message, inner)
		{
			_card = card;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.WrongCardException"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.WrongCardException"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("[WrongCardException: Card={0}]", Card);
		}


	}

	#endregion

}

