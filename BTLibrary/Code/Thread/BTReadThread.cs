//using System;
using Java.Lang;
using Android.Bluetooth;
using System.IO;
using Android.OS;

namespace BTLibrary
{
	/// <summary>
	/// Connected thread class.
	/// </summary>
	internal class BTReadThread : Thread
	{
		/// <summary>
		/// The BluetoothSocket.
		/// </summary>
		public BluetoothSocket _socket;

		/// <summary>
		/// The input stream.
		/// </summary>
		private Stream _inStream;

		/// <summary>
		/// The output stream.
		/// </summary>
		private Stream _outStream;

		/// <summary>
		/// The connected device address.
		/// </summary>
		private string _connected;

		public BTReadThread (BluetoothSocket socket)
		{
			_socket = socket;
			Stream tmpIn = null;
			Stream tmpOut = null;

			// Get the BluetoothSocket input and output streams
			try {
				tmpIn = _socket.InputStream;
				tmpOut = _socket.OutputStream;
			} catch (Exception e) {
				//temp socket not created
				e.ToString ();
			}
			_inStream = tmpIn;
			_outStream = tmpOut;

			_connected = _socket.RemoteDevice.Address;
		}

		/// <summary>
		/// Starts executing the active part of the Connected thread.
		/// </summary>
		public override void Run ()
		{
			byte [] buffer = new byte[1024];
			int bytes;

			// Keep listening to the InputStream while connected
			while (true) {
				try {
					buffer = new byte[1024];
					// Read from the InputStream
					bytes = _inStream.Read (buffer, 0, buffer.Length);

					// Send the obtained bytes to the UI Activity

					BTPlayService.Instance.ObtainMessage ((int) MessageType.MESSAGE_DEVICE_READ, _connected).SendToTarget ();
					BTPlayService.Instance.ObtainMessage ((int) MessageType.MESSAGE_READ, bytes, -1, buffer).SendToTarget ();

				} catch (Exception e) {

					BTPlayService.Instance.ObtainMessage ((int) MessageType.MESSAGE_CONNECTION_LOST, _connected).SendToTarget ();

					break;
				}
			}
		}


		/// <summary>
		/// Try to close the socket
		/// </summary>
		/// <returns><c>true</c> if this instance cancel ; otherwise, <c>false</c>.</returns>
		public void Cancel ()
		{
			try {
				_socket.Close ();
			} catch (Exception e) {
				//close of connect socket failed
				e.ToString ();
			}
		}
	}
}
