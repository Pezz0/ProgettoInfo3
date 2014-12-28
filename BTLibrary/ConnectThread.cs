using System;
using Java.Lang;
using Android.Bluetooth;
using Java.Util;

namespace BTLibrary
{
	/// <summary>
	/// Connect thread class.
	/// </summary>
	public class ConnectThread : Thread
	{
		/// <summary>
		/// The BluetoothSocket.
		/// </summary>
		private BluetoothSocket _socket;

		/// <summary>
		/// The BluetoothDevice we want to connect.
		/// </summary>
		private BluetoothDevice _device;

		/// <summary>
		/// The BluetoothPlayService
		/// </summary>
		private BluetoothPlayService _PlayService;

		public ConnectThread (BluetoothDevice device, BluetoothPlayService playService, UUID MY_UUID)
		{
			_device = device;
			_PlayService = playService;
			BluetoothSocket tmp = null;
			// Get a BluetoothSocket for a connection with the
			// given BluetoothDevice
			try {
				tmp = _device.CreateRfcommSocketToServiceRecord (MY_UUID);
			} catch (Java.IO.IOException e) {
				e.ToString ();//create fail
			}
			_socket = tmp;
		}

		/// <summary>
		/// Starts executing the active part of ConnectThread.
		/// </summary>
		public override void Run ()
		{
			Name = "ConnectThread";

			// Always cancel discovery because it will slow down a connection
			_PlayService.getBTAdapter ().CancelDiscovery ();

			// Make a connection to the BluetoothSocket
			try {
				// This is a blocking call and will only return on a
				// successful connection or an exception
				_socket.Connect ();
			} catch (Java.IO.IOException e) {
				e.ToString ();
				//connection failure
				_PlayService.ConnectionFailed ();
				// Close the socket
				try {
					_socket.Close ();
				} catch (Java.IO.IOException e2) {
					e2.ToString ();
					//not able to close socket during connection failure
				}

				// Start the service over to restart listening mode
				_PlayService.ConnectAsMaster ();
				return;
			}

			// Reset the ConnectThread because we're done
			lock (this) {
				_PlayService.connectThread = null;
			}

			// Start the connected thread
			_PlayService.ConnectedToMaster (_socket, _device);
		}

		/// <summary>
		/// Try to close the socket
		/// </summary>
		public void Cancel ()
		{
			try {
				_socket.Close ();
			} catch (Java.IO.IOException e) {
				e.ToString ();
				//close of connect socket failed
			}
		}
	}
}

