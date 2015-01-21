using System;
using Android.Bluetooth;
using System.IO;
using System.Collections.Generic;
using Android.OS;
using System.Runtime.CompilerServices;
using System.Threading;

namespace BTLibrary
{
	/// <summary>
	/// BT write thread.
	/// </summary>
	public class BTWriteThread
	{
		/// <summary>
		/// The BluetoothSocket.
		/// </summary>
		public BluetoothSocket _Socket;

		/// <summary>
		/// The output stream.
		/// </summary>
		private Stream _OutStream;

		/// <summary>
		/// The connected device address.
		/// </summary>
		private string _connected;

		public string Connected { get { return _connected; } }

		/// <summary>
		/// The sleep time of the thread.
		/// </summary>
		private const int _SLEEP_TIME = 100;

		/// <summary>
		/// The buffer to store out messages.
		/// </summary>
		private BTBuffer _buffer = new BTBuffer ();

		/// <summary>
		/// The writer thread.
		/// </summary>
		private Thread _writer;

		public BTWriteThread (BluetoothSocket socket)
		{
			_Socket = socket;
			Stream tmpOut = null;

			// Get the BluetoothSocket input and output streams
			try {
				tmpOut = _Socket.OutputStream;
			} catch {
			}

			_OutStream = tmpOut;

			//start the thread
			_connected = _Socket.RemoteDevice.Address;
			_writer = new Thread (Write);
			_writer.Name = "Writer";
			_writer.Start ();
		}

		private void Write ()
		{
			while (true) {
				lock (_buffer) {
					if (!_buffer.isEmpty) {
						byte [] msg = _buffer.getValue ();

						try {
							_OutStream.Write (msg, 0, msg.Length);
							// Share the sent message back 
							//BTPlayService.Instance.ObtainMessage ((int) EnLocalMessageType.MESSAGE_WRITE, -1, -1, msg).SendToTarget ();
						
						} catch {
							BTPlayService.Instance.ObtainMessage ((int) EnLocalMessageType.MESSAGE_CONNECTION_LOST, _connected).SendToTarget ();
							BTPlayService.Instance.ConnectionLost ();

						}

						if (msg [0] == (int) EnPackageType.ACK/* || msg [0] == (int) EnPackageType.NAME*/)
							_buffer.Remove (msg);
					} else
						Monitor.Wait (_buffer);
				}

				Thread.Sleep (_SLEEP_TIME);
			}
		}

		public void Add (byte [] elem)
		{
			lock (_buffer) {
				_buffer.Add (elem);
				Monitor.Pulse (_buffer);
			}
		}

		public void Remove (byte [] elem)
		{
			lock (_buffer) {
				_buffer.Remove (elem);
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
			} catch {
			}

			_writer.Abort ();
		}
	}

	/// <summary>
	/// Buffer object to store output messages.
	/// </summary>
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
			if (elem [0] == (int) EnPackageType.ACK)
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

