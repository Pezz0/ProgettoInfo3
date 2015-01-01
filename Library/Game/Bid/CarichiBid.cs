using System;

namespace ChiamataLibrary
{
	public class CarichiBid:NotPassBid
	{
		public override NotPassBid getNext ()
		{
			return new CarichiBid (bidder, point + 1);
		}

		public override IBid changeBidder (Player newBidder)
		{
			return new CarichiBid (newBidder, this.point);
		}

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

		public CarichiBid (Player bidder, int point) : base (bidder, point)
		{
		}

		public override string ToString ()
		{
			return string.Format ("[BidCarichi:Player:{0}, Point={1}]", bidder, point);
		}
	}
}

