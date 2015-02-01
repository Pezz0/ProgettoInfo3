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

		/// <summary>
		/// Gets the briscole in the hand.
		/// </summary>
		/// <returns>The list of cards in the hand that are briscole.</returns>
		public List<Card> GetBriscole (bool complete = false)
		{
			return GetHand (Board.Instance.Briscola, complete);
		}

		/// <summary>
		/// Gets the cards that are not briscole.
		/// </summary>
		/// <returns>The list of cards in the hand that are not briscole.</returns>
		public List<Card> GetNoBriscole (bool complete = false)
		{
			List<Card> hand = new List<Card> ();
			foreach (Card c in Board.Instance._cardGrid)
				if (!c.IsBiscrola && c.initialPlayer == this && ( complete || c.IsPlayable ))
					hand.Add (c);

			return hand;
		}


		/// <summary>
		/// Gets a briscola that isn't a carico.
		/// </summary>
		/// <remarks>This means that TRE or ASSE won't be considered in this search.
		/// See the <see cref="ChiamataLibrary.Player.getCarico"/> method for more informations about carichi.</remarks>
		/// <returns>The briscola not carico if a card fitting the description exists in the hand, <c>null</c> otherwise.</returns>
		public Card GetBriscolaNotCarico (bool complete = false)
		{
			foreach (Card c in Board.Instance._cardGrid)
				if (c.IsBiscrola && c.initialPlayer == this && !c.IsCarico && ( complete || c.IsPlayable ))
					return c;

			return null;

		}

		/// <summary>
		/// Gets the briscola carico.
		/// </summary>
		/// <remarks>Basically returns the briscola with the highest value.</remarks>
		/// <returns>The briscola carico if a card fitting the description exists in the hand, <c>null</c> otherwise.</returns>
		public Card GetBriscolaCarico (bool complete = false)
		{
			foreach (Card c in Board.Instance._cardGrid)
				if (c.IsBiscrola && c.initialPlayer == this && c.IsCarico && ( complete || c.IsPlayable ))
					return c;

			return null;
		}



		/// <summary>
		/// Gets the a scartino from the player's hand.
		/// </summary>
		/// <remarks>Basically returns the card in the hand that has the lowest value. A scartino is a card that has no value; 
		/// ideally, a card that isn't briscola and is worth 0 points is a good scartino.
		/// This means that this method will try to find any card that has no value and isn't briscola (basically every card between DUE and SETTE).</remarks>
		/// <returns>The scartino.</returns>
		public Card GetScartino ()
		{
			List<Card> h = GetHand ();
			Card temp = h [0];

			for (int i = 1; i < h.Count; ++i)
				if (h [i] < temp)
					temp = h [i];

			return temp;
		}

		/// <summary>
		/// Gets a carico from the player's hand.
		/// </summary>
		/// <remarks>Basically returns the card in the hand that has the highest value. A carico is a card that has the greatest value; 
		/// ideally, a card that isn't briscola and has a value of 10 or 11 points is a good carico. 
		/// This means that this method will try to find any card that is a TRE or ASSE and isn't briscola</remarks>
		/// <returns>The carico.</returns>
		public Card GetCarico ()
		{
			if (Board.Instance.numberOfCardOnBoard != 0) {

				int onBoard = (int) Board.Instance.CardOnTheBoard [0].seme;

				if (Board.Instance._cardGrid [onBoard, 9].initialPlayer == this && Board.Instance._cardGrid [onBoard, 9].IsPlayable)
					return Board.Instance._cardGrid [onBoard, 9];

				if (Board.Instance._cardGrid [onBoard, 8].initialPlayer == this && Board.Instance._cardGrid [onBoard, 8].IsPlayable)
					return Board.Instance._cardGrid [onBoard, 8];
			}

			foreach (Card c in Board.Instance._cardGrid)
				if (!c.IsBiscrola && c.initialPlayer == this && c.IsPlayable && c.IsCarico)
					return c;

			return GetVestita ();

		}

		/// <summary>
		/// Gets the vestita.
		/// </summary>
		/// <remarks>Basically returns a card with a value between 2 and 4 points. A vestita is a card that has an average value; 
		/// ideally, a card that isn't briscola and has a value between 2 and 4 points is a good vestita. 
		/// This means that this method will try to find any card that is a FANTE, CAVALLO or RE and isn't briscola</remarks>
		/// <returns>The vestita if a card fitting the description exists in the hand, <c>null</c> otherwise.</returns>
		public Card GetVestita ()
		{
			if (Board.Instance.numberOfCardOnBoard != 0) {
				int onBoard = (int) Board.Instance.CardOnTheBoard [0].seme;

				if (Board.Instance._cardGrid [onBoard, 7].initialPlayer == this && Board.Instance._cardGrid [onBoard, 7].IsPlayable)
					return Board.Instance._cardGrid [onBoard, 7];

				if (Board.Instance._cardGrid [onBoard, 6].initialPlayer == this && Board.Instance._cardGrid [onBoard, 6].IsPlayable)
					return Board.Instance._cardGrid [onBoard, 6];

				if (Board.Instance._cardGrid [onBoard, 5].initialPlayer == this && Board.Instance._cardGrid [onBoard, 5].IsPlayable)
					return Board.Instance._cardGrid [onBoard, 5];

			}

			foreach (Card c in Board.Instance._cardGrid)
				if (!c.IsBiscrola && c.initialPlayer == this && c.IsPlayable && c.IsVestita)
					return c;

			return GetScartino ();
		}

		/// <summary>
		/// Gets the lowest card that can win the hand.
		/// </summary>
		/// <returns>The strozzo basso if a card fitting the description exists in the hand, <c>null</c> otherwise.</returns>
		/// <param name="briscola">If set to <c>true</c>, briscole will be considered in the search, otherwise they won't be considered. This argument default value is true.</param>
		public Card GetStrozzoBasso (bool briscola = true)
		{

			if (Board.Instance.numberOfCardOnBoard == 0)
				return null;

			Card onBoard = Board.Instance.CardOnTheBoard [0];
			for (int i = 1; i < Board.Instance.numberOfCardOnBoard; ++i)
				if (Board.Instance.CardOnTheBoard [i] > onBoard)
					onBoard = Board.Instance.CardOnTheBoard [i];

			foreach (Card c in Board.Instance._cardGrid)
				if (!c.IsBiscrola && c.initialPlayer == this && c.IsPlayable && c > onBoard)
					return c;

			if (!briscola)
				return GetScartino ();

			foreach (Card c in Board.Instance._cardGrid)
				if (c.IsBiscrola && c.initialPlayer == this && c.IsPlayable && c > onBoard)
					return c;

			return GetScartino ();
		}

		/// <summary>
		/// Gets the highest card that can win the hand.
		/// </summary>
		/// <returns>The strozzo alto if a card fitting the description exists in the hand, <c>null</c> otherwise.</returns>
		public Card GetStrozzoAlto (bool briscola = false)
		{
			if (Board.Instance.numberOfCardOnBoard == 0)
				return null;

			Card onBoard = Board.Instance.CardOnTheBoard [0];
			for (int i = 1; i < Board.Instance.numberOfCardOnBoard; ++i)
				if (Board.Instance.CardOnTheBoard [i] > onBoard)
					onBoard = Board.Instance.CardOnTheBoard [i];

			foreach (Card c in Board.Instance._cardGrid)
				if (!c.IsBiscrola && c.initialPlayer == this && c.IsPlayable && c > onBoard)
					onBoard = c;

			if (onBoard.initialPlayer == this)
				return onBoard;

			if (!briscola)
				return GetScartino ();

			foreach (Card c in Board.Instance._cardGrid)
				if (c.IsBiscrola && c.initialPlayer == this && c.IsPlayable && c > onBoard)
					return c;

			return GetScartino ();
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
