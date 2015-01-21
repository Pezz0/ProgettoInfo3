using System;
using System.Collections.Generic;
using System.Text;

namespace BTLibrary
{
	public class PackageName:Package
	{
		public const int MAX_NAME_LENGHT = 10;

		public readonly String name;
		public readonly String address;

		public PackageName (String name) : base (EnPackageType.NAME)
		{
			this.name = name;
			address = BTManager.Instance.GetLocalAddress ();
		}

		public PackageName (byte [] bs) : base (EnPackageType.NAME)
		{
			if (bs [0] != (byte) type)
				throw new Exception ("Wrong byte's sequence");

			char [] adr = new char[17];

			//the next 17 bytes indicete the address of the device who sends message
			for (int i = 1; i < 18; i++)
				adr [i - 1] = (char) bs [i];

			address = new string (adr);

			List<byte> lb = new List<byte> ();

			for (int i = 18; bs [i] != 0; ++i)
				lb.Add (bs [i]);
				
			name = Encoding.ASCII.GetString (lb.ToArray ());
		}

		public override byte[] getMessage ()
		{
			List<Byte> msg = new List<byte> (1024);
			msg.Add ((byte) type);

			byte [] bAddress = Encoding.ASCII.GetBytes (address);
			msg.AddRange (bAddress);

			msg.AddRange (Encoding.ASCII.GetBytes (name));
			return msg.ToArray ();
		}

	}
}

