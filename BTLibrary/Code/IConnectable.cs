using System;
using Android.Bluetooth;

namespace BTLibrary
{
	public interface IConnectable
	{
		/// <summary>
		/// Performs connection as master
		/// </summary>
		void ConnectAsMaster ();

		/// <summary>
		/// Performs connection as slave
		/// </summary>
		/// <param name="device">Device to connect.</param>
		void ConnectAsSlave (BluetoothDevice device);

	}
}

