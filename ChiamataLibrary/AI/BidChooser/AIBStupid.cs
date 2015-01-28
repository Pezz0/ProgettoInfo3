using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// AI for auction.
	/// </summary>
	public class AIBStupid:IAIBidChooser
	{
		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="me">The <see cref="ChiamataLibrary.Player"/> instance representing the AI.</param>
		/// <param name="seme">The seme that will be chosen if this AI wins the auction.</param>
		public void setup (Player me, EnSemi seme)
		{

		}

		/// <summary>
		/// Always returns a <see cref="ChiamataLibrary.PassBid"/>.
		/// </summary>
		/// <returns>The bid.</returns>
		/// <param name="me">Me.</param>
		/// <param name="seme">Seme.</param>
		public IBid chooseABid ()
		{
			return new PassBid ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.AIBStupid"/> class.
		/// </summary>
		public AIBStupid ()
		{
		}
	}
}

