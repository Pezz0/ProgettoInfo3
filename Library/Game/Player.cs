using System;
using System.Collections.Generic;
using BTLibrary;

namespace ChiamataLibrary
{
	public class Player:IBTSendable<Player>
	{
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
				if (!Board.Instance.isAuctionPhase && Board.Instance.isAuctionClosed)
					throw new WrongPhaseException ("The role is assigned at the end of the auction", "Auction closed");
					
				_role = value;
			}
		}

		/// <summary>
		/// Gets the player's hand.
		/// </summary>
		/// <value>The player's hand.</value>
		public List<Card> Hand { get { return Board.Instance.getPlayerHand (this); } }

		#region Bluetooth

		public byte[] toByteArray ()
		{
			return BitConverter.GetBytes (order);
		}

		public Player ricreateFromByteArray (byte [] bytes)
		{
			return Board.Instance.AllPlayers [(int) bytes [0]];
		}

		public int ByteArrayLenght { get { return 1; } }

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

		public override string ToString ()
		{
			return string.Format ("[Player: Name={0}, Role={1}, Order={2}]", name, Role, order);
		}
	}
}
