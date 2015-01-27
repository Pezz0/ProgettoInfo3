using System;
using System.Collections.Generic;

namespace ChiamataLibrary
{
	/// <summary>
	/// Player.
	/// </summary>
	public class Player
	{
		#region Basic informations

		public readonly string name;
		public readonly int order;
		private EnRole _role;

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

		public List<Card> Hand { get { return Board.Instance.getPlayerHand (this); } }

		public List<Card> InitialHand { get { return Board.Instance.getPlayerInitialHand (this); } }

		public List<Card> getCards (EnSemi seme)
		{
			return Hand.FindAll (delegate(Card c) {
				return c.seme == seme;
			});
		}

		public List<Card> getBriscole ()
		{
			return getCards (Board.Instance.Briscola);
		}

		public List<Card> getNoBriscole ()
		{
			return Hand.FindAll (delegate(Card c) {
				return c.seme != Board.Instance.Briscola;
			});
		}

		public Card getScartino ()
		{
			Card temp = Hand [0];

			for (int i = 1; i < Hand.Count; i++)
				if (Hand [i] < temp)
					temp = Hand [i];

			return temp;
		}

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

		public Card getBriscolaCarico ()
		{
			List<Card> temp = getBriscole ();
			if (temp.Count > 0)
				return temp [temp.Count - 1];
			else
				return null;

		}

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

		private IPlayerController _controller;

		public IPlayerController Controller { set { _controller = value; } }

		public void setController (IPlayerController controller)
		{
			_controller = controller;
		}

		//public bool isReady{ get { return _controller.isReady; } }


		public IBid invokeChooseBid ()
		{
			if (!Board.Instance.isAuctionPhase)
				throw new WrongPhaseException ("A player can place a bid only during the auction phase", "Auction");
				
			IBid bid = _controller.chooseBid ();

			if (bid != null && bid < Board.Instance.currentAuctionWinningBid && bid is NotPassBid)
				throw new BidNotEnoughException ("The new bid is not enough to beat the winning one", bid);

			if (bid == null)
				return null;
			return bid.changeBidder (this);
		}

		public EnSemi? invokeChooseSeme ()
		{
			if (!( Board.Instance.isAuctionPhase || Board.Instance.isFinalizePhase ))
				throw new WrongPhaseException ("A player can choose a seme only during the auction phase", "Auction");

			if (Board.Instance.currentAuctionWinningBid.bidder != this)
				throw new WrongPlayerException ("This player can't choose the seme because he doesn't win", Board.Instance.currentAuctionWinningBid.bidder);

			return _controller.chooseSeme ();
		}

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
		/// Initializes a new instance of the <see cref="Engine.Player"/> class.
		/// </summary>
		/// <param name="name">The player's name.</param>
		/// <param name="order">The player's order </param>
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
