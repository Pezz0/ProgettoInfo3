using System;
using ChiamataLibrary;
using System.Collections.Generic;

namespace BTLibrary
{
	/// <summary>
	/// Package representing the board.
	///
	/// 
	/// 
	/// This package will be similar to:
	/// <list type="table">
	/// <listheader>
	/// <term>Type</term>
	/// <term>Message</term>
	/// </listheader>
	/// <item><term>2</term><description>1 Byte</description></item>
	/// <item><term>BOARD</term></item>
	/// </list>
	///
	/// 
	/// or similar to the following if it's an ACK package:
	/// <list type="table">
	/// <listheader>
	/// <term>Type</term>
	/// <term>Address</term>
	/// <term>Message</term>
	/// </listheader>
	/// <item><term>8</term><description>1 Byte</description></item>
	/// <item><term>ADDRESS</term><description>17 Bytes</description></item>
	/// <item><term>BOARD</term></item>
	/// </list>
	/// 
	/// 
	/// 
	/// The BOARD message is composed of (lenght of the message will vary because of the variable lengths of the player names):
	/// <list type="number">
	/// <item><term>Names</term><description>The player names, following the pattern name length (1 Byte) - Name. </description></item>
	/// <item><term>Dealer</term><description>The index of the dealer.</description></item>
	/// <item><term>Cards</term><description>All the 40 cards are assigned an integer between 0 and 4 indicating the index of the player owning that card.</description></item>
	/// </list> 
	/// </summary>
	public class PackageBoard:PackageBase
	{
		/// <summary>
		/// Byte array representing the board.
		/// </summary>
		private byte [] _bytes;

		/// <summary>
		/// Gets the bytes representing the board.
		/// </summary>
		/// <value>The bytes.</value>
		public byte[] bytes {
			get {
				return _bytes;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BTLibrary.PackageBoard"/> class.
		/// </summary>
		public PackageBoard () : base (EnPackageType.BOARD)
		{
			_bytes = Board.Instance.SendableBytes.ToArray ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BTLibrary.PackageBoard"/> class.
		/// </summary>
		/// <param name="bs">The bytes array representing the board.</param>
		/// <exception cref="System.Exception">Thrown when the byte sequence doesen't represent a board.</exception>
		public PackageBoard (byte [] bs) : base (EnPackageType.BOARD)
		{
			if (bs [0] != (byte) type)
				throw new Exception ("Wrong byte's sequence");

			_bytes = new byte[1024];
			for (int i = 1; i < bs.GetLength (0); ++i)
				_bytes [i - 1] = bs [i];

		}

		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <returns>The message.</returns>
		/// <remarks>Implementation for the abstract method in <see cref="BTLibrary.Package"/>.</remarks>
		public override byte[] getMessage ()
		{
			List<Byte> msg = new List<byte> (1024);
			msg.Add ((byte) type);
			msg.AddRange (_bytes);
			return msg.ToArray ();
		}

	}
}

