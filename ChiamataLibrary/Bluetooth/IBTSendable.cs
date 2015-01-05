using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// The interface implemented by the class that can be sended trough bluetooth
	/// </summary>
	public interface IBTSendable<T>
	{
		/// <summary>
		/// Gets the byte array lenght.
		/// </summary>
		/// <value>The byte array lenght.</value>
		int ByteArrayLenght{ get; }

		/// <summary>
		/// Returns a <see cref="Byte[]"/> that represents the current <see cref="BTLibrary.IBTSendable"/>.
		/// </summary>
		/// <returns>A <see cref="Byte[]"/> that represents the current <see cref="BTLibrary.IBTSendable"/>.</returns>
		Byte[] toByteArray ();

		/// <summary>
		/// Return a class that will generate this byte array
		/// </summary>
		/// <returns>The class.</returns>
		/// <param name="bytes">the bytes array.</param>
		T ricreateFromByteArray (Byte [] bytes);
	}
}

