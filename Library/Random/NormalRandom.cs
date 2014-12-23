using System;
using System.Security.Cryptography;

namespace MyRandom
{
	public class NormalRandom:IRandomGenerator
	{
		private RandomNumberGenerator _rnd;


		public int getRandomNumber (int maxValue)
		{
			byte [] b = new byte[4];
			_rnd.GetBytes (b);
			double d = (double) BitConverter.ToUInt32 (b, 0) / UInt32.MaxValue;

			return (int) Math.Round (d * ( maxValue - 1 ));

		}

		public NormalRandom ()
		{
			_rnd = RandomNumberGenerator.Create ();
		}
	}
}

