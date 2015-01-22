using System;
using ChiamataLibrary;
using System.Collections.Generic;
using System.Text;

namespace BTLibrary
{
	public abstract class Package : IEquatable<EnPackageType>
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

		public override bool Equals (object obj)
		{
			if (obj is EnPackageType)
				return this == (EnPackageType) obj;
			else
				return false;
		}

		public override int GetHashCode ()
		{
			//FIXME: to implement but never used
			return base.GetHashCode ();
		}

		public bool Equals (EnPackageType other)
		{
			return type == other;
		}


		public abstract byte[] getMessage ();

		public byte[] getAckMessage ()
		{
			List<Byte> msg = new List<byte> (1024);

			msg.Add ((byte) EnPackageType.ACK);

			byte [] bAddress = Encoding.ASCII.GetBytes (BTManager.Instance.GetLocalAddress ());
			msg.AddRange (bAddress);

			msg.AddRange (getMessage ());

			return msg.ToArray ();
		}

		public static byte[] getMessageFromHack (byte [] bs)
		{
			List<byte> msg = new List<byte> ();
			//the other bytes indicate the message (normal or playtime)
			for (int i = 18; i < bs.GetLength (0); i++)
				msg.Add (bs [i]);

			return msg.ToArray ();
		}

		public static string getAddressFromHack (byte [] bs)
		{
			char [] adr = new char[17];

			//the next 17 bytes indicete the address of the device who sends message
			for (int i = 1; i < 18; i++)
				adr [i - 1] = (char) bs [i];

			return new string (adr);
		}


	}
}

