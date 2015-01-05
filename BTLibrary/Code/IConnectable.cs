using System;
using Android.Bluetooth;

namespace BTLibrary
{
	public interface IConnectable
	{
		void ConnectAsMaster ();

		void ConnectAsSlave (BluetoothDevice device);

		bool existBluetooth ();

		bool isDiscovering ();

		void CancelDiscovery ();

		void UnregisterReceiever ();

		bool isBTEnabled ();

		BluetoothDevice getRemoteDevice (string address);

	}
}

