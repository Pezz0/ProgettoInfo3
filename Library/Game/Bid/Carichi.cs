using System;

namespace ChiamataLibrary
{
	public class BidCarichi:IBid
	{
		#region Information

		public readonly int point;

		#endregion

		#region Bluetooth

		public override byte[] toByteArray ()
		{
			byte [] b = new Byte[3];
			b [0] = bidder.toByteArray () [0];
			b [1] = BitConverter.GetBytes (point) [0];
			b [2] = 255;	//default value

			return b;
		}

		#endregion

		public BidCarichi (Player bidder, int point) : base (bidder)
		{
			this.point = point;
		}

		public override string ToString ()
		{
			return string.Format ("[BidCarichi:Player:{0}, Point={1}]", bidder, point);
		}
	}
}

