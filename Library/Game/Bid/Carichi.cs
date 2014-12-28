using System;

namespace ChiamataLibrary
{
	public class BidCarichi:IBid
	{
		#region Information

		/// <summary>
		/// The bid's point.
		/// </summary>
		private int _point;

		/// <summary>
		/// Gets the bid's point.
		/// </summary>
		/// <value>The point.</value>
		public int Point { get { return _point; } }

		#endregion

		#region Bluetooth

		public override byte[] toByteArray ()
		{
			byte [] b = new Byte[3];
			b [0] = Bidder.toByteArray () [0];
			b [1] = BitConverter.GetBytes (_point) [0];
			b [2] = 255;	//default value

			return b;
		}

		#endregion

		public BidCarichi (Player bidder, int point) : base (bidder)
		{
			_point = point;
		}

		public override string ToString ()
		{
			return string.Format ("[BidCarichi:Player:{0}, Point={1}]", Bidder, Point);
		}
	}
}

