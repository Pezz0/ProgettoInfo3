using System;
using Android.Bluetooth;
using Java.Util;
using System.Threading;

namespace BTLibrary
{
	/// <summary>
	/// Connect thread class.
	/// </summary>
	internal class BTConnectThread
	{
		/// <summary>
		/// The BluetoothSocket.
		/// </summary>
		private BluetoothSocket _socket;

		/// <summary>
		/// The BluetoothDevice we want to connect.
		/// </summary>
		private BluetoothDevice _device;

		private Thread _connecter;

		public BTConnectThread (BluetoothDevice device, UUID MY_UUID)
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
				BTPlayService.Instance.ObtainMessage ((int) EnLocalMessageType.MESSAGE_CONNECTION_FAILED, -1, -1).SendToTarget ();
				BTPlayService.Instance.ConnectionFailed ();

				// Close the socket
				try {
					_socket.Close ();
				} catch {
				}
				return;
			}

			// Start the connected thread
			BTPlayService.Instance.ConnectedToMaster (_socket, _device);
		}

		/// <summary>
		/// Try to close the socket
		/// </summary>
		public void Cancel ()
		{
			try {
				_socket.Close ();
			} catch {
			}

			_connecter.Abort ();
		}
	}
}

