using System;
using Android.Bluetooth;
using Java.Util;
using System.Threading;

namespace BTLibrary
{
	/// <summary>
	/// Used to connect to an open bluetooth slot (used by the slaves).
	/// </summary>
	internal class BTConnectThread
	{
		/// <summary>
		/// The BluetoothSocket.
		/// </summary>
		private readonly BluetoothSocket _socket;

		/// <summary>
		/// The BluetoothDevice we want to connect.
		/// </summary>
		private readonly BluetoothDevice _device;

		/// <summary>
		/// Thread that attempts to connect to the master.
		/// </summary>
		private readonly Thread _connecter;

		/// <summary>
		/// Initializes a new instance of the <see cref="BTLibrary.BTConnectThread"/> class.
		/// </summary>
		/// <param name="device">The device of the master we want to connect to.</param>
		/// <param name="MY_UUID">My UUID.</param>
		internal BTConnectThread (BluetoothDevice device, UUID MY_UUID)
		{
			_device = device;
			BluetoothSocket tmp = null;
			// Get a BluetoothSocket for a connection with the
			// given BluetoothDevice
			try {
				tmp = _device.CreateRfcommSocketToServiceRecord (MY_UUID);
			} catch {
			}
			_socket = tmp;
			_connecter = new Thread (Connect);
			_connecter.Name = "ConnectThread";
			_connecter.Start ();
		}

		/// <summary>
		/// Starts executing the active part of ConnectThread.
		/// </summary>
		private void Connect ()
		{
			// Make a connection to the BluetoothSocket
			try {
				// This is a blocking call and will only return on a
				// successful connection or an exception
				_socket.Connect ();
			} catch {
				BTManager.Instance.ObtainMessage ((int) EnLocalMessageType.MESSAGE_CONNECTION_FAILED, -1, -1).SendToTarget ();
				BTManager.Instance.ConnectionFailed ();

				// Close the socket
				try {
					_socket.Close ();
				} catch {
				}
				return;
			}

			BTManager.Instance.ConnectedToMaster (_socket, _device);
		}

		/// <summary>
		/// Try to close the socket
		/// </summary>
		internal void Cancel ()
		{
			try {
				_socket.Close ();
			} catch {
			}

			_connecter.Abort ();
		}
	}
}

