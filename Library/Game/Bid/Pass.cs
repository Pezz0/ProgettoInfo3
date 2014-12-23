using System;

namespace Engine
{
	public class Pass:IBid
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Engine.Pass"/> class.
		/// </summary>
		/// <param name="bidder">Bidder.</param>
		public Pass (Player bidder) : base (bidder)
		{
		}
	}
}

