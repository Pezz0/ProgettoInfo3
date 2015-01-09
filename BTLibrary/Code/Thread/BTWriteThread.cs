using System;
using Android.Bluetooth;
using System.IO;
using System.Collections.Generic;
using Android.OS;
using Java.Lang;
using System.Runtime.CompilerServices;

namespace BTLibrary
{
	public class BTWriteThread:Thread
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

		private List<byte []> _buffer;

		public const int SLEEP_TIME = 500;

		public BTWriteThread (BluetoothSocket socket)
		{
			_Socket = socket;
			Stream tmpIn = null;
			Stream tmpOut = null;

			// Get the BluetoothSocket input and output streams
			try {
				tmpIn = _Socket.InputStream;
				tmpOut = _Socket.OutputStream;
			} catch (System.Exception e) {
				//temp socket not created
				e.ToString ();
			}
			_InStream = tmpIn;
			_OutStream = tmpOut;

			Connected = _Socket.RemoteDevice.Address;

			_buffer = new List<byte []> ();

		}


		public override void Run ()
		{
			while (true) {
				if (_buffer.Count > 0) {

					byte [] msg = _buffer [0];
					try {
						_OutStream.Write (msg, 0, msg.Length);
						// Share the sent message back to the UI Activity
						BTPlayService.Instance.forEachHandler (delegate(Handler h) {
							h.ObtainMessage ((int) MessageType.MESSAGE_WRITE, -1, -1, msg).SendToTarget ();
						});
					} catch (System.Exception e) {
						//exception during write
						e.ToString ();
					}

					Sleep (SLEEP_TIME);
				}
				
			}
		}

		[MethodImpl (MethodImplOptions.Synchronized)]
		public void Add (byte [] elem)
		{
			_buffer.Add (elem);
		}

		[MethodImpl (MethodImplOptions.Synchronized)]
		public void Remove (byte [] elem)
		{
			_buffer.Remove (elem);
		}

		/// <summary>
		/// Try to close the socket
		/// </summary>
		/// <returns><c>true</c> if this instance cancel ; otherwise, <c>false</c>.</returns>
		public void Cancel ()
		{
			try {
				_Socket.Close ();
			} catch (System.Exception e) {
				//close of connect socket failed
				e.ToString ();
			}
		}
	}
}

