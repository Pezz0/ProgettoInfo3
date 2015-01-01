using System;

namespace ChiamataLibrary
{
	public class NormalBid:NotPassBid
	{
		#region Information

		/// <summary>
		/// The bid's number.
		/// </summary>
		public readonly EnNumbers number;

		#endregion

		#region Bluetooth

		public override byte[] toByteArray ()
		{
			byte [] b = new Byte[3];
			b [0] = this.bidder.toByteArray () [0];
			b [1] = BitConverter.GetBytes (point) [0];
			b [2] = BitConverter.GetBytes ((int) number) [0];

			return b;
		}

		#endregion


		public override IBid changeBidder (Player newBidder)
		{
			return new NormalBid (newBidder, this.number, this.point);
		}

		public override NotPassBid getNext ()
		{
			if (this.number == 0)
				return new NormalBid (bidder, 0, this.point + 1);
			else
				return new NormalBid (bidder, this.number - 1, 61);
		}

		public NormalBid (Player bidder, EnNumbers number, int point) : base (bidder, point)
		{
			this.number = number;
		}

		public override string ToString ()
		{
			return string.Format ("[NormalBid: Player:{0}, Number={1}, Point={2}]", bidder, this.number, this.point);
		}

	}
}

