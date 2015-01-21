using System;
using ChiamataLibrary;
using System.Collections.Generic;

namespace BTLibrary
{
	public class PackageBoard:Package
	{
		public readonly byte [] bytes;

		public PackageBoard () : base (EnPackageType.BOARD)
		{
			bytes = Board.Instance.SendableBytes.ToArray ();
		}

		public PackageBoard (byte [] bs) : base (EnPackageType.BOARD)
		{
			if (bs [0] != (byte) type)
				throw new Exception ("Wrong byte's sequence");

			bytes = bs;
		}

		public override byte[] getMessage ()
		{
			List<Byte> msg = new List<byte> (1024);
			msg.Add ((byte) type);
			msg.AddRange (bytes);
			return msg.ToArray ();
		}

	}
}

