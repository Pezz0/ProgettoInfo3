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
		private string _connected;

		public string Connected { get { return _connected; } }

		public const int SLEEP_TIME = 50;

		BTBuffer _buffer = new BTBuffer ();

		public BTBuffer Buffer { get { return _buffer; } }

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

			_connected = _Socket.RemoteDevice.Address;
		}

		public override void Run ()
		{
			while (true) {
				if (!_buffer.isEmpty) {

					byte [] msg = _buffer.getValue ();

					try {
						_OutStream.Write (msg, 0, msg.Length);
						// Share the sent message back to the UI Activity
						BTPlayService.Instance.ObtainMessage ((int) MessageType.MESSAGE_WRITE, -1, -1, msg).SendToTarget ();
						
					} catch (System.Exception e) {
						//exception during write
						e.ToString ();
					}

					if (msg [0] == (int) EnContentType.ACK)
						_buffer.Remove (msg);
				}

				Sleep (SLEEP_TIME);
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
			} catch (System.Exception e) {
				//close of connect socket failed
				e.ToString ();
			}
		}
	}

	public class BTBuffer
	{
		private readonly List<byte []> _buffer = new List<byte []> ();

		public bool isEmpty{ get { return _buffer.Count == 0; } }

		[MethodImpl (MethodImplOptions.Synchronized)]
		public byte[] getValue ()
		{
			return _buffer [0];
		}

		[MethodImpl (MethodImplOptions.Synchronized)]
		public void Add (byte [] elem)
		{
			if (elem [0] == (int) EnContentType.ACK)
				_buffer.Insert (0, elem);
			else
				_buffer.Add (elem);
		}

		[MethodImpl (MethodImplOptions.Synchronized)]
		public void Remove (byte [] elem)
		{
			_buffer.RemoveAll (delegate(byte [] b) {
				bool temp = true;
				for (int i = 0; i < b.Length; i++)
					temp = temp && b [i] == elem [i];

				return temp;
			});

		}

		[MethodImpl (MethodImplOptions.Synchronized)]
		public void DiscardBuffer ()
		{
			_buffer.Clear ();
		}
	}


}

