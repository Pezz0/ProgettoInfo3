using System;
using Java.Lang;
using Android.Bluetooth;
using System.IO;

namespace BTLibrary
{
	/// <summary>
	/// Connected thread class.
	/// </summary>
	public class BTConnectedThread : Thread
	{
		/// <summary>
		/// The BluetoothSocket.
		/// </summary>
		public BluetoothSocket _Socket;

		/// <summary>
		/// The input stream.
		/// </summary>
		private Stream _InStream;

		/// <summary>
		/// The output stream.
		/// </summary>
		private Stream _OutStream;

		/// <summary>
		/// The BluetoothPlayService.
		/// </summary>
		private BTPlayService _PlayService;

		public BTConnectedThread (BluetoothSocket socket, BTPlayService playService)
		{
			_Socket = socket;
			_PlayService = playService;
			Stream tmpIn = null;
			Stream tmpOut = null;

			// Get the BluetoothSocket input and output streams
			try {
				tmpIn = _Socket.InputStream;
				tmpOut = _Socket.OutputStream;
			} catch (Java.IO.IOException e) {
				e.ToString ();
				//temp socket not created
			}
			_InStream = tmpIn;
			_OutStream = tmpOut;
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
					// Read from the InputStream
					bytes = _InStream.Read (buffer, 0, buffer.Length);

					// Send the obtained bytes to the UI Activity
					_PlayService.getHandler ().ObtainMessage ((int) MessageType.MESSAGE_READ, bytes, -1, buffer)
						.SendToTarget ();
				} catch (Java.IO.IOException e) {
					e.ToString ();
					//disconnected
					_PlayService.ConnectionLost ();
					break;
				}
			}
		}

		/// <summary>
		/// Write to the connected OutStream.
		/// </summary>
		/// <param name='buffer'>
		/// The bytes to write
		/// </param>
		public void Write (byte [] buffer)
		{
			try {
				_OutStream.Write (buffer, 0, buffer.Length);
				// Share the sent message back to the UI Activity
				_PlayService.getHandler ().ObtainMessage ((int) MessageType.MESSAGE_WRITE, -1, -1, buffer)
					.SendToTarget ();
			} catch (Java.IO.IOException e) {
				//exception during write
				e.ToString ();
			}
		}

		/// <summary>
		/// Try to close the socket
		/// </summary>
		/// <returns><c>true</c> if this instance cancel ; otherwise, <c>false</c>.</returns>
		public void Cancel ()
		{
			try {
				_Socket.Close ();
			} catch (Java.IO.IOException e) {
				e.ToString ();
				//close of connect socket failed
			}
		}
	}
}

