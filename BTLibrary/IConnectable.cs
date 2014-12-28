using System;
using Android.Bluetooth;

namespace ProvaConnessioneBT
{
	public interface IConnectable
	{
		void ConnectAsMaster ();

		void ConnectAsSlave (BluetoothDevice device);

		bool existBluetooth ();

		bool isDiscovering ();

		void CancelDiscovery ();

		void UnregisterReciever ();

		bool isBTEnabled ();

		BluetoothDevice getRemoteDevice (string address);

	}
}

