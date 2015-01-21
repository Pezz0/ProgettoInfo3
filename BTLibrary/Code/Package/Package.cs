using System;
using ChiamataLibrary;
using System.Collections.Generic;
using System.Text;

namespace BTLibrary
{
	public abstract class Package
	{

		public readonly EnPackageType type;

		public Package (EnPackageType type)
		{
			this.type = type;
		}

		public static Package createPackage (byte [] b)
		{
			EnPackageType t = (EnPackageType) b [0];
			switch (t) {
				case EnPackageType.BID:
					return new PackageBid (b);
				case EnPackageType.BOARD:
					return new PackageBoard (b);
				case EnPackageType.MOVE:
					return new PackageCard (b);
				case EnPackageType.NAME:
					return new PackageName (b);
				case EnPackageType.READY:
					return new PackageReady (b);
				case EnPackageType.SEME:
					return new PackageSeme (b);
				default:
					throw new Exception ("Wrong byte's sequence");
			}

		}

		public static bool operator == (Package package, EnPackageType type)
		{
			return package.type == type;
		}

		public static bool operator != (Package package, EnPackageType type)
		{
			return package.type != type;
		}

		public abstract byte[] getMessage ();

		public byte[] getAckMessage ()
		{
			List<Byte> msg = new List<byte> (1024);

			msg.Add ((byte) EnPackageType.ACK);

			byte [] bAddress = Encoding.ASCII.GetBytes (BTPlayService.Instance.GetLocalAddress ());
			msg.AddRange (bAddress);

			msg.AddRange (getMessage ());

			return msg.ToArray ();
		}


	}
}

