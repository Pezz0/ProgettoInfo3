using System;
using System.Collections.Generic;

namespace BTLibrary
{
	public class PackageTerminate:Package
	{
		public readonly int terminateSignal;

	
		public PackageTerminate (int terminateSignal) : base (EnPackageType.TERMINATE)
		{
			this.terminateSignal = terminateSignal;
		}

		public PackageTerminate (byte [] bs) : base (EnPackageType.SEME)
		{
			if (bs [0] != (byte) type)
				throw new Exception ("Wrong byte's sequence");
				
			terminateSignal = (int) bs [1];
		}

		public override byte[] getMessage ()
		{
			List<Byte> msg = new List<byte> (1024);
			msg.Add ((byte) type);
			msg.Add ((byte) terminateSignal);
			return msg.ToArray ();
		}


	}
}

