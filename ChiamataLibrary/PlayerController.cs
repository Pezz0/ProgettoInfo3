﻿using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// Interface for a player controller. Must implement the methods to choose a bid, a seme and cards to play.
	/// </summary>
	public interface IPlayerController
	{
		/// <summary>
		/// Method to choose the bid.
		/// </summary>
		/// <returns>The bid.</returns>
		Bid chooseBid ();

		/// <summary>
		/// Method to choose the seme.
		/// </summary>
		/// <returns>The seme.</returns>
		EnSemi? chooseSeme ();

		/// <summary>
		/// Method to choose the card to play.
		/// </summary>
		/// <returns>The card.</returns>
		Card chooseCard ();

	}
}

