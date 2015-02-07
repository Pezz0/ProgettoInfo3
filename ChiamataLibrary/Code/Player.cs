using System;
using System.Collections.Generic;

namespace ChiamataLibrary
{
	/// <summary>
	/// Class representing a player.
	/// </summary>
	public class Player
	{
		#region Basic informations

		/// <summary>
		/// The name of the player.
		/// </summary>
		public readonly string name;
		/// <summary>
		/// The index of the player.
		/// </summary>
		public readonly int order;
		/// <summary>
		/// The role of the player.
		/// </summary>
		private EnRole _role;

		/// <summary>
		/// Gets or sets the role.
		/// </summary>
		/// <value>The role of the player.</value>
		public EnRole Role { get { return _role; } internal set { _role = value; } }

		#endregion

		#region Derived informations

		/// <summary>
		/// Gets the cards with the same seme as the one provided as argument.
		/// </summary>
		/// <returns>The list of cards that has the same seme as the one provided.</returns>
		/// <param name="seme">The seme.</param>

		public List<Card> GetHand (bool complete = false)
		{
			List<Card> hand = new List<Card> ();
			foreach (Card c in Board.Instance._cardGrid)
				if (c.initialPlayer == this && ( complete || c.IsPlayable ))
					hand.Add (c);

			return hand;
		}


		public List<Card> GetHand (EnSemi seme, bool complete = false)
		{
			List<Card> hand = new List<Card> ();
			for (int n = 0; n < Board.Instance.nNumber; ++n) {
				Card c = Board.Instance._cardGrid [(int) seme, n];
				if (c.initialPlayer == this && ( complete || c.IsPlayable ))
					hand.Add (c);
			}

			return hand;

		}


		#endregion

		#region Operation

		public static Player operator + (Player p, int o)
		{
			return Board.Instance._players [( p.order + o ) % Board.PLAYER_NUMBER];
		}

		public static Player operator - (Player p, int o)
		{
			return Board.Instance._players [( p.order + 5 - o ) % Board.PLAYER_NUMBER];
		}

		public static Player operator ++ (Player p)
		{
			return Board.Instance._players [( p.order + 1 ) % Board.PLAYER_NUMBER];
		}

		public static Player operator -- (Player p)
		{
			return Board.Instance._players [( p.order - 1 ) % Board.PLAYER_NUMBER];
		}

		public static explicit operator Player (int order)
		{
			return Board.Instance._players [order];
		}

		public static explicit operator Player (byte order)
		{
			return Board.Instance._players [(int) order];
		}

		#endregion

		#region Control

		/// <summary>
		/// The controller for this player (could be bluetooth or AI).
		/// See <see cref="BTLibrary.BTPlayerController"/> and <see cref="ChiamataLibrary.AIPlayerController"/> for more informations.
		/// </summary>
		private IPlayerController _controller;

		/// <summary>
		/// Sets the controller for this player.
		/// </summary>
		/// <value>The controller.</value>
		public IPlayerController Controller { set { _controller = value; } }

		/// <summary>
		/// Sets the controller for this player.
		/// </summary>
		/// <param name="controller">Controller.</param>
		public void SetController (IPlayerController controller)
		{
			_controller = controller;
		}


		/// <summary>
		/// Invokes the method that chooses the bid.
		/// </summary>
		/// <returns>The bid chosen.</returns>
		internal BidBase InvokeChooseBid ()
		{
			if (!Board.Instance.IsAuctionPhase)
				throw new WrongPhaseException ("A player can place a bid only during the auction phase", "Auction");
				
			BidBase bid = _controller.ChooseBid ();

			if (bid != null && bid < Board.Instance.currentAuctionWinningBid && bid is NotPassBidBase)
				throw new BidNotEnoughException ("The new bid is not enough to beat the winning one", bid);

			if (bid == null)
				return null;
			return bid.ChangeBidder (this);
		}

		/// <summary>
		/// Invokes the method that chooses the seme.
		/// </summary>
		/// <returns>The seme chosen.</returns>
		internal EnSemi? InvokeChooseSeme ()
		{
			if (!( Board.Instance.IsAuctionPhase || Board.Instance.IsFinalizePhase ))
				throw new WrongPhaseException ("A player can choose a seme only during the auction phase", "Auction");

			if (Board.Instance.currentAuctionWinningBid.bidder != this)
				throw new WrongPlayerException ("This player can't choose the seme because he didn't win", Board.Instance.currentAuctionWinningBid.bidder);

			return _controller.ChooseSeme ();
		}

		/// <summary>
		/// Invokes the method that chooses the card.
		/// </summary>
		/// <returns>The card chosen.</returns>
		internal Card InvokeChooseCard ()
		{
			if (!Board.Instance.IsPlayTime)
				throw new WrongPhaseException ("A player can play a card only during the playtime phase", "Playtime");

			Card c = _controller.ChooseCard ();

			if (c == null)
				return null;

			if (c.initialPlayer != this || !c.IsPlayable)
				throw new WrongCardException ("This can't play this card", c);

			return c;

		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.Player"/> class.
		/// </summary>
		/// <param name="name">The player's name.</param>
		/// <param name="order">The player's order (aka index).</param>
		internal Player (string name, int order)
		{
			if (!Board.Instance.IsCreationPhase)
				throw new WrongPhaseException ("A player must be instantiated during the creation time", "Creation");

			this._role = EnRole.ALTRO;
			this.name = name;
			this.order = order;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.Player"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.Player"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("[Player: Name={0}, Role={1}, Order={2}]", name, Role, order);
		}
	}
}
