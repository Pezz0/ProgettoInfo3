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
		public EnRole Role {
			get { return _role; }
			set {
				if (!( Board.Instance.isFinalizePhase || Board.Instance.isCreationPhase ))
					throw new WrongPhaseException ("The role is assigned at the end of the auction", "Auction closed");

				_role = value;
			}
		}

		#endregion

		#region Derived informations

		/// <summary>
		/// Gets the hand.
		/// </summary>
		/// <value>The list of cards representing the hand.</value>
		public List<Card> Hand { get { return Board.Instance.getPlayerHand (this); } }

		/// <summary>
		/// Gets the initial hand.
		/// </summary>
		/// <value>The list of cards representing the initial hand.</value>
		public List<Card> InitialHand { get { return Board.Instance.getPlayerInitialHand (this); } }

		/// <summary>
		/// Gets the cards with the same seme as the one provided as argument.
		/// </summary>
		/// <returns>The list of cards that has the same seme as the one provided.</returns>
		/// <param name="seme">The seme.</param>
		public List<Card> getCards (EnSemi seme)
		{
			return Hand.FindAll (delegate(Card c) {
				return c.seme == seme;
			});
		}

		/// <summary>
		/// Gets the briscole in the hand.
		/// </summary>
		/// <returns>The list of cards in the hand that are briscole.</returns>
		public List<Card> getBriscole ()
		{
			return getCards (Board.Instance.Briscola);
		}

		/// <summary>
		/// Gets the cards that are not briscole.
		/// </summary>
		/// <returns>The list of cards in the hand that are not briscole.</returns>
		public List<Card> getNoBriscole ()
		{
			return Hand.FindAll (delegate(Card c) {
				return c.seme != Board.Instance.Briscola;
			});
		}

		/// <summary>
		/// Gets the a scartino from the player's hand.
		/// </summary>
		/// <remarks>Basically returns the card in the hand that has the lowest value. A scartino is a card that has no value; 
		/// ideally, a card that isn't briscola and is worth 0 points is a good scartino.
		/// This means that this method will try to find any card that has no value and isn't briscola (basically every card between DUE and SETTE).</remarks>
		/// <returns>The scartino.</returns>
		public Card getScartino ()
		{
			Card temp = Hand [0];

			for (int i = 1; i < Hand.Count; i++)
				if (Hand [i] < temp)
					temp = Hand [i];

			return temp;
		}

		/// <summary>
		/// Gets a carico from the player's hand.
		/// </summary>
		/// <remarks>Basically returns the card in the hand that has the highest value. A carico is a card that has the greatest value; 
		/// ideally, a card that isn't briscola and has a value of 10 or 11 points is a good carico. 
		/// This means that this method will try to find any card that is a TRE or ASSE and isn't briscola</remarks>
		/// <returns>The carico.</returns>
		public Card getCarico ()
		{
			Card temp;
			List<Card> listCarichi = new List<Card> ();

			int i = 9;
			while (listCarichi.Count == 0 && i > 4) {
				listCarichi = Hand.FindAll (delegate(Card c) {
					return c.seme != Board.Instance.CalledCard.seme && c.number == (EnNumbers) i;
				});

				i--;
			}

			temp = listCarichi.Find (delegate (Card c) {
				return c.seme == Board.Instance.CardOnTheBoard [0].seme;
			});

			if (listCarichi.Count == 0)
				return getScartino ();
			else if (temp == null)
				return listCarichi [0];
			else
				return temp;
		}

		/// <summary>
		/// Gets the lowest card that can win the hand.
		/// </summary>
		/// <returns>The strozzo basso if a card fitting the description exists in the hand, <c>null</c> otherwise.</returns>
		/// <param name="briscola">If set to <c>true</c>, briscole will be considered in the search, otherwise they won't be considered. This argument default value is true.</param>
		public Card getStrozzoBasso (bool briscola = true)
		{

			if (Board.Instance.numberOfCardOnBoard == 0)
				return null;

			List<Card> temp = Hand.FindAll (delegate (Card c) {
				bool a = c > Board.Instance.CardOnTheBoard [Board.Instance.numberOfCardOnBoard - 1] && !c.isCarico;

				if (briscola)
					return a && c.isBiscola;
				else
					return a;
			});

			if (temp.Count == 0)
				return null;
			else
				return new SortedSet<Card> (temp).Min;
		}

		/// <summary>
		/// Gets the highest card that can win the hand.
		/// </summary>
		/// <returns>The strozzo alto if a card fitting the description exists in the hand, <c>null</c> otherwise.</returns>
		public Card getStrozzoAlto ()
		{
			Card temp = null;
			int i = 9;
			while (temp == null) {
				temp = Hand.Find (delegate(Card c) {
					return !c.isBiscola && c.number == (EnNumbers) i && c.seme == Board.Instance.CardOnTheBoard [0].seme;
				});

				i--;
			}
			return temp;
		}

		/// <summary>
		/// Gets a briscola that isn't a carico.
		/// </summary>
		/// <remarks>This means that TRE or ASSE won't be considered in this search.
		/// See the <see cref="ChiamataLibrary.Player.getCarico"/> method for more informations about carichi.</remarks>
		/// <returns>The briscola not carico if a card fitting the description exists in the hand, <c>null</c> otherwise.</returns>
		public Card getBriscolaNotCarico ()
		{
			List<Card> temp = Hand.FindAll (delegate (Card c) {
				return c.isBiscola && !c.isCarico;
			});
			if (temp.Count == 0)
				return null;
			else
				return temp [temp.Count - 1];
		}

		/// <summary>
		/// Gets the briscola carico.
		/// </summary>
		/// <remarks>Basically returns the briscola with the highest value.</remarks>
		/// <returns>The briscola carico if a card fitting the description exists in the hand, <c>null</c> otherwise.</returns>
		public Card getBriscolaCarico ()
		{
			List<Card> temp = getBriscole ();
			if (temp.Count > 0)
				return temp [temp.Count - 1];
			else
				return null;

		}

		/// <summary>
		/// Gets the vestita.
		/// </summary>
		/// <remarks>Basically returns a card with a value between 2 and 4 points. A vestita is a card that has an average value; 
		/// ideally, a card that isn't briscola and has a value between 2 and 4 points is a good vestita. 
		/// This means that this method will try to find any card that is a FANTE, CAVALLO or RE and isn't briscola</remarks>
		/// <returns>The vestita if a card fitting the description exists in the hand, <c>null</c> otherwise.</returns>
		public Card getVestita ()
		{
			Card vestita = null;
			int i = 7;
			while (vestita == null && i >= 0) {
				vestita = Hand.Find (delegate(Card c) {
					return !c.isBiscola && c.number == (EnNumbers) i;
				});

				i--;
			}

			return vestita;
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
		public void setController (IPlayerController controller)
		{
			_controller = controller;
		}


		/// <summary>
		/// Invokes the method that chooses the bid.
		/// </summary>
		/// <returns>The bid chosen.</returns>
		public Bid invokeChooseBid ()
		{
			if (!Board.Instance.isAuctionPhase)
				throw new WrongPhaseException ("A player can place a bid only during the auction phase", "Auction");
				
			Bid bid = _controller.chooseBid ();

			if (bid != null && bid < Board.Instance.currentAuctionWinningBid && bid is NotPassBid)
				throw new BidNotEnoughException ("The new bid is not enough to beat the winning one", bid);

			if (bid == null)
				return null;
			return bid.changeBidder (this);
		}

		/// <summary>
		/// Invokes the method that chooses the seme.
		/// </summary>
		/// <returns>The seme chosen.</returns>
		public EnSemi? invokeChooseSeme ()
		{
			if (!( Board.Instance.isAuctionPhase || Board.Instance.isFinalizePhase ))
				throw new WrongPhaseException ("A player can choose a seme only during the auction phase", "Auction");

			if (Board.Instance.currentAuctionWinningBid.bidder != this)
				throw new WrongPlayerException ("This player can't choose the seme because he didn't win", Board.Instance.currentAuctionWinningBid.bidder);

			return _controller.chooseSeme ();
		}

		/// <summary>
		/// Invokes the method that chooses the card.
		/// </summary>
		/// <returns>The card chosen.</returns>
		public Move invokeChooseCard ()
		{
			if (!Board.Instance.isPlayTime)
				throw new WrongPhaseException ("A player can play a card only during the playtime phase", "Playtime");

			Card card = _controller.chooseCard ();
			if (card == null)
				return null;
			return new Move (card, this);
		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.Player"/> class.
		/// </summary>
		/// <param name="name">The player's name.</param>
		/// <param name="order">The player's order (aka index).</param>
		public Player (string name, int order)
		{
			if (!Board.Instance.isCreationPhase)
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
