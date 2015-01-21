using System;
using ChiamataLibrary;
using System.Collections.Generic;

namespace BTLibrary
{
	public class PackageSeme:Package
	{
		public readonly Player player;
		public readonly EnSemi seme;

		public PackageSeme (Player player, EnSemi seme) : base (EnPackageType.SEME)
		{
			this.player = player;
			this.seme = seme;
		}

		public override byte[] getMessage ()
		{
			List<Byte> msg = new List<byte> (1024);
			msg.Add ((byte) type);
			msg.Add ((byte) player.order);
			msg.Add ((byte) seme);
			return msg.ToArray ();
		}

		public PackageSeme (byte [] bs) : base (EnPackageType.SEME)
		{
			if (bs [0] == (byte) type)
				throw new Exception ("Wrong byte's sequence");

			player = Board.Instance.getPlayer ((int) bs [1]);
			seme = (EnSemi) bs [2];
		}


	}
}

