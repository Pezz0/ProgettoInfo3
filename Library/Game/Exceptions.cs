using System;
using System.Runtime.Serialization;

namespace ChiamataLibrary
{
	#region WrongPhaseException

	[Serializable ()]
	public class WrongPhaseException:Exception
	{

		/// <summary>
		/// The expected phase.
		/// </summary>
		private string _expectedPhase;

		/// <summary>
		/// Gets the expected phase.
		/// </summary>
		/// <value>The expected phase.</value>
		public string ExpectedPhase { get { return _expectedPhase; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Library.WrongPhaseException"/> class.
		/// </summary>
		/// <param name="expected">Expected phase.</param>
		public WrongPhaseException (string expected) : base ()
		{
			_expectedPhase = expected;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Library.WrongPhaseException"/> class.
		/// </summary>
		/// <param name="message">Exception description</param>
		/// <param name="expected">Expected phase.</param>
		public WrongPhaseException (string message, string expected) : base (message)
		{
			_expectedPhase = expected;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Library.WrongPhaseException"/> class.
		/// </summary>
		/// <param name="message">Exception description</param>
		/// <param name="inner">Exception inner cause</param>
		/// <param name="expected">Expected phase.</param>
		public WrongPhaseException (string message, Exception inner, string expected) : base (message, inner)
		{
			_expectedPhase = expected;
		}

		/// <summary>
		/// Create the exception from serialized data.
		/// Usual scenario is when exception is occured somewhere on the remote workstation
		/// and we have to re-create/re-throw the exception on the local machine
		/// </summary>
		/// <param name="info">Serialization info</param>
		/// <param name="context">Serialization context</param>
		/// <param name="expected">Expected phase.</param>
		protected WrongPhaseException (SerializationInfo info, StreamingContext context, string actual, string expected)
			: base (info, context)
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

	[Serializable ()]
	public class WrongBiddingPlayerException:Exception
	{
		/// <summary>
		/// The player.
		/// </summary>
		private Player _player;

		/// <summary>
		/// Gets the player.
		/// </summary>
		/// <value>The player.</value>
		public Player Player { get { return _player; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.WrongBiddingPlayerException"/> class.
		/// </summary>
		/// <param name="player">The player who place the wrong bid</param>
		public WrongBiddingPlayerException (Player player) : base ()
		{
			_player = player;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.WrongBiddingPlayerException"/> class.
		/// </summary>
		/// <param name="message">Exception description</param>
		/// <param name="player">The player who place the wrong bid</param>
		public WrongBiddingPlayerException (string message, Player player) : base (message)
		{
			_player = player;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.WrongBiddingPlayerException"/> class.
		/// </summary>
		/// <param name="message">Exception description</param>
		/// <param name="inner">Exception inner cause</param>
		/// <param name="player">The player who place the wrong bid</param>
		public WrongBiddingPlayerException (string message, Exception inner, Player player) : base (message, inner)
		{
			_player = player;
		}

		/// <summary>
		/// Create the exception from serialized data.
		/// Usual scenario is when exception is occured somewhere on the remote workstation
		/// and we have to re-create/re-throw the exception on the local machine
		/// </summary>
		/// <param name="info">Serialization info</param>
		/// <param name="context">Serialization context</param>
		/// <param name="player">The player who place the wrong bid</param>
		protected WrongBiddingPlayerException (SerializationInfo info, StreamingContext context, Player player)
			: base (info, context)
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

	[Serializable ()]
	public class BidNotEnoughException:Exception
	{
		/// <summary>
		/// The bid.
		/// </summary>
		private IBid _bid;

		/// <summary>
		/// Gets the bid.
		/// </summary>
		/// <value>The bid.</value>
		public IBid Bid { get { return _bid; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.BidNotEnoughException"/> class.
		/// </summary>
		/// <param name="bid">Bid.</param>
		public BidNotEnoughException (IBid bid) : base ()
		{
			_bid = bid;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.BidNotEnoughException"/> class.
		/// </summary>
		/// <param name="message">Exception description</param>
		/// <param name="bid">Bid.</param>
		public BidNotEnoughException (string message, IBid bid) : base (message)
		{
			_bid = bid;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.BidNotEnoughException"/> class.
		/// </summary>
		/// <param name="message">Exception description</param>
		/// <param name="inner">Exception inner cause</param>
		/// <param name="bid">Bid.</param>
		public BidNotEnoughException (string message, Exception inner, IBid bid) : base (message, inner)
		{
			_bid = bid;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.BidNotEnoughException"/> class.
		/// </summary>
		/// <param name="info">Serialization info</param>
		/// <param name="context">Serialization context</param>
		/// <param name="bid">Bid.</param>
		protected BidNotEnoughException (SerializationInfo info, StreamingContext context, IBid bid)
			: base (info, context)
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

}

