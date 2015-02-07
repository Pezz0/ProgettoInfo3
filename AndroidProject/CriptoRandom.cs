using System;
using System.Security.Cryptography;
using ChiamataLibrary;

namespace GUILayout
{
	/// <summary>
	/// Class used for a normal random.
	/// </summary>
	internal class CriptoRandom:IRandomGenerator,IDisposable
	{
		/// <summary>
		/// The random number generator.
		/// </summary>
		private readonly RNGCryptoServiceProvider _rnd;

		/// <summary>
		/// Return a random integer number lesser or equal to MaxValue
		/// </summary>
		/// <param name="maxValue">Max value.</param>
		/// <returns>The random number.</returns>
		public int GetRandomNumber (int maxValue)
		{
			byte [] b = new byte[2];
			_rnd.GetBytes (b);
			int d = ( (int) BitConverter.ToUInt16 (b, 0) ) % maxValue;

			return d;

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MyRandom.NormalRandom"/> class.
		/// </summary>
		public CriptoRandom ()
		{
			_rnd = new RNGCryptoServiceProvider ();
		}


		#region IDisposable implementation

		bool _disposed;

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		~CriptoRandom ()
		{
			Dispose (false);
		}

		private void Dispose (bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
				_rnd.Dispose ();

			_disposed = true;
		}

		#endregion
	}
}

