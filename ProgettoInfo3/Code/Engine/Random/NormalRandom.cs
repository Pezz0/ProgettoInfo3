using System;

namespace myRandom
{
	public class NormalRandom:RandomGenerator
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

