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
	internal class BTConnectedThread : Thread
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
		/// The connected device address.
		/// </summary>
		private string Connected;

		public BTConnectedThread (BluetoothSocket socket)
		{
			_Socket = socket;
			Stream tmpIn = null;
			Stream tmpOut = null;

			// Get the BluetoothSocket input and output streams
			try {
				tmpIn = _Socket.InputStream;
				tmpOut = _Socket.OutputStream;
			} catch (Exception e) {
				//temp socket not created
				e.ToString ();
			}
			_InStream = tmpIn;
			_OutStream = tmpOut;

			Connected = _Socket.RemoteDevice.Address;
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
					bytes = _InStream.Read (buffer, 0, buffer.Length);

					// Send the obtained bytes to the UI Activity

					BTPlayService.Instance.forEachHandler (delegate(Handler h) {

						h.ObtainMessage ((int) MessageType.MESSAGE_DEVICE_READ, Connected).SendToTarget ();
						h.ObtainMessage ((int) MessageType.MESSAGE_READ, bytes, -1, buffer).SendToTarget ();


					});


				} catch (Exception e) {
					//disconnected
					BTPlayService.Instance.ConnectionLost (e.Message);
					BTPlayService.Instance.forEachHandler (delegate(Handler h) {
						h.ObtainMessage ((int) MessageType.MESSAGE_CONNECTION_LOST, Connected).SendToTarget ();
					});
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
				BTPlayService.Instance.forEachHandler (delegate(Handler h) {
					h.ObtainMessage ((int) MessageType.MESSAGE_WRITE, -1, -1, buffer).SendToTarget ();
				});
			} catch (Exception e) {
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
			} catch (Exception e) {
				//close of connect socket failed
				e.ToString ();
			}
		}
	}
}

