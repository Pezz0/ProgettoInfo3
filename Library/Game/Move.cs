using System;

namespace ChiamataLibrary
{
	public class Move:IBTSendable<Move>
	{
		/// <summary>
		/// The played card.
		/// </summary>
		public readonly Card card;

		/// <summary>
		/// The player who play a card.
		/// </summary>
		public readonly Player player;

		#region Bluetooth

		public int ByteArrayLenght { get { return 2; } }

		public byte[] toByteArray ()
		{
			Byte [] b = new Byte[2];
			b [0] = player.toByteArray () [0];
			b [1] = card.toByteArray () [0];

			return b;
		}

		public Move ricreateFromByteArray (byte [] bytes)
		{
			return new Move (Board.Instance.getCard (bytes [1]), Board.Instance.getPlayer (bytes [0]));
		}

		#endregion

		public Move (Card card, Player player)
		{
			this.card = card;
			this.player = player;
		}

		public Move ()
		{
			this.card = null;
			this.player = null;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.Move"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="ChiamataLibrary.Move"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("[Move: ByteArrayLenght={0}]", ByteArrayLenght);
		}
	}
}

