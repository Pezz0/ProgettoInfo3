using System;
using ChiamataLibrary;
using System.Collections.Generic;

namespace BTLibrary
{
	public class PackageReady:Package
	{
		public readonly Player player;

		public override byte[] getMessage ()
		{
			List<Byte> msg = new List<byte> (1024);
			msg.Add ((byte) type);	//0
			msg.Add ((byte) player.order);	//1
			return msg.ToArray ();
		}

		public PackageReady () : base (EnPackageType.READY)
		{
			player = Board.Instance.Me;
		}

		public PackageReady (byte [] bs) : base (EnPackageType.READY)
		{
			if (bs [0] != (byte) type)
				throw new Exception ("Wrong byte's sequence");

			player = Board.Instance.getPlayer ((int) bs [1]);
		}
	}
}

