using System;

namespace ChiamataLibrary
{
	public class NormalBid:IBid
	{
		#region Information

		/// <summary>
		/// The bid's number.
		/// </summary>
		private EnNumbers _number;

		/// <summary>
		/// The bid's point.
		/// </summary>
		private int _point;

		/// <summary>
		/// Gets the bid's number.
		/// </summary>
		/// <value>The number.</value>
		public EnNumbers Number { get { return _number; } }

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
			b [2] = BitConverter.GetBytes ((int) _number) [0];

			return b;
		}

		#endregion

		public NormalBid (Player bidder, EnNumbers number, int point) : base (bidder)
		{
			_number = number;
			_point = point;
		}

		public override string ToString ()
		{
			return string.Format ("[NormalBid: Player:{0}, Number={1}, Point={2}]", Bidder, Number, Point);
		}

	}
}

