using System;
using System.Collections.Generic;

namespace ChiamataLibrary
{
	public class Player
	{
		/// <summary>
		/// The board used by this player.
		/// </summary>
		private readonly Board _board;

		/// <summary>
		/// The player's name.
		/// </summary>
		public readonly string name;

		/// <summary>
		/// The player's order.
		/// </summary>
		public readonly int order;

		/// <summary>
		/// The player's role.
		/// </summary>
		private EnRole _role;

		/// <summary>
		/// Gets or sets the player's role, the role can't be change during the play time.
		/// </summary>
		/// <value>The role.</value>
		public EnRole Role {
			get { return _role; }
			set {
				if (!_board.isAuctionPhase && _board.isAuctionClosed)
					throw new WrongPhaseException ("The role is assigned at the end of the auction", "Auction closed");
					
				_role = value;
			}
		}

		/// <summary>
		/// Gets the player's hand.
		/// </summary>
		/// <value>The player's hand.</value>
		public List<Card> Hand { get { return _board.getPlayerHand (this); } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Engine.Player"/> class.
		/// </summary>
		/// <param name="board">The board used by this player.</param>
		/// <param name="name">The player's name.</param>
		/// <param name="order">The player's order </param>
		public Player (Board board, string name, int order)
		{
			if (!board.isCreationPhase)
				throw new WrongPhaseException ("A player must be instantiated during the creation time", "Creation");

			this._role = EnRole.ALTRO;
			this._board = board;
			this.name = name;
			this.order = order;
		}

		public override string ToString ()
		{
			return string.Format ("[Player: Name={0}, Role={1}, Order={2}]", name, Role, order);
		}
	}
}
