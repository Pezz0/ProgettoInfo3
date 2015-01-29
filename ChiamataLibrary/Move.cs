﻿using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// Class representing a move.
	/// </summary>
	public class Move
	{
		/// <summary>
		/// The played card.
		/// </summary>
		public readonly Card card;

		/// <summary>
		/// The player who played the card.
		/// </summary>
		public readonly Player player;

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.Move"/> class.
		/// </summary>
		/// <param name="card">The played card.</param>
		/// <param name="player">The player that played the card.</param>
		public Move (Card card, Player player)
		{
			this.card = card;
			this.player = player;
		}

		/// <summary>
		/// Initializes a new default instance of the <see cref="ChiamataLibrary.Move"/> class.
		/// </summary>
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
			return string.Format ("[Move: Card={0}, Player={1}]", card, player);

		}
	}
}

