using System;
using System.Security.Cryptography;

namespace MyRandom
{
	public class NormalRandom:IRandomGenerator
	{
		private RNGCryptoServiceProvider _rnd;

		public int getRandomNumber (int maxValue)
		{
			byte [] b = new byte[2];
			_rnd.GetBytes (b);
			int d = ( (int) BitConverter.ToUInt16 (b, 0) ) % maxValue;

			return d;

		}

		public NormalRandom ()
		{
			_rnd = new RNGCryptoServiceProvider ();
		}
	}
}

