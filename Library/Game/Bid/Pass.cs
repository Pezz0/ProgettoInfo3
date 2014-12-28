using System;

namespace ChiamataLibrary
{
	public class PassBid:IBid
	{
		#region Bluetooth

		public override byte[] toByteArray ()
		{
			byte [] b = new Byte[3];
			b [0] = Bidder.toByteArray () [0];
			b [1] = 255;	//default value
			b [2] = 255;	//default value

			return b;
		}

		#endregion


		/// <summary>
		/// Initializes a new instance of the <see cref="Engine.Pass"/> class.
		/// </summary>
		/// <param name="bidder">Bidder.</param>
		public PassBid (Player bidder) : base (bidder)
		{
		}

		public override string ToString ()
		{
			return string.Format ("[PassBid: Player:{0}", Bidder);
		}
	}
}

