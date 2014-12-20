using System;

namespace Engine
{
	public abstract class IBid
	{
		/// <summary>
		/// The bidder.
		/// </summary>
		private Player _bidder;

		/// <summary>
		/// Gets the bidder.
		/// </summary>
		/// <value>The bidder.</value>
		public Player Bidder { get { return _bidder; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Engine.IBid"/> class.
		/// </summary>
		/// <param name="bidder">Bidder.</param>
		public IBid (Player bidder)
		{
			_bidder = bidder;
		}
	}
}

