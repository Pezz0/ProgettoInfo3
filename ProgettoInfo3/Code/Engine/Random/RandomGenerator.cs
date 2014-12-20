using System;

namespace MyRandom
{
	public interface IRandomGenerator
	{
		/// <summary>
		/// Return a random integer number lesser or equal to MaxValue
		/// </summary>
		/// <param name="maxValue">Max value.</param>
		int getRandomNumber (int maxValue);
	}
}

