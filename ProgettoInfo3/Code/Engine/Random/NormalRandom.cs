using System;

namespace MyRandom
{
	public class NormalRandom:IRandomGenerator
	{
		private Random _rnd;


		public int getRandomNumber (int maxValue)
		{
			return _rnd.Next (0, maxValue);
		}

		public NormalRandom ()
		{
			_rnd = new Random ();
		}
	}
}

