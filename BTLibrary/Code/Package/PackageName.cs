using System;
using System.Collections.Generic;
using System.Text;

namespace BTLibrary
{
	public class PackageName:Package
	{
		public readonly String name;

		public PackageName (String name) : base (EnPackageType.NAME)
		{
			this.name = name;
		}

		public PackageName (byte [] bs) : base (EnPackageType.NAME)
		{
			if (bs [0] == (byte) type)
				throw new Exception ("Wrong byte's sequence");

			List<byte> lb = new List<byte> ();
			for (int i = 1; bs [i] != 0; ++i)
				lb.Add (bs [i]);

			name = Encoding.ASCII.GetString (lb.ToArray ());
		}

		public override byte[] getMessage ()
		{
			List<Byte> msg = new List<byte> (1024);
			msg.Add ((byte) type);
			msg.AddRange (Encoding.ASCII.GetBytes (name));
			return msg.ToArray ();
		}

	}
}

