using System;

namespace ChiamataLibrary
{
	public class NormalBid:IBid
	{
		#region Information

		/// <summary>
		/// The bid's number.
		/// </summary>
		public readonly EnNumbers number;

		/// <summary>
		/// The bid's point.
		/// </summary>
		public readonly int point;


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

		public NormalBid (Player bidder, EnNumbers number, int point) : base (bidder)
		{
			this.number = number;
			this.point = point;
		}

		public override string ToString ()
		{
			return string.Format ("[NormalBid: Player:{0}, Number={1}, Point={2}]", bidder, this.number, this.point);
		}

	}
}

