using System;
using Android.Bluetooth;
using System.IO;
using System.Collections.Generic;
using Android.OS;
using System.Runtime.CompilerServices;
using System.Threading;
using Android.Util;

namespace BTLibrary
{
	/// <summary>
	/// Thread that is used to write messages via bluetooth.
	/// Has a buffer of messages to send. To send a message, simply use the add method to add the array of bytes representing the message to the buffer; the ACK messages will be 
	/// given the highest priority and will be inserted at the head of the list whereas the other messages will be inserted at the tail.
	/// When the buffer isn't empty, the thread will send the message at the head of the list. When an ACK is recived by the <see cref="BTlibrary.BTReadThread"/>, the method <see cref="Remove"/>
	/// is used to remove the corresponding message from the buffer.
	/// </summary>
	internal class BTWriteThread
	{
		/// <summary>
		/// The BluetoothSocket.
		/// </summary>
		internal BluetoothSocket _Socket;

		/// <summary>
		/// The output stream.
		/// </summary>
		private readonly Stream _OutStream;

		/// <summary>
		/// The connected device address.
		/// </summary>
		private readonly string _connected;

		/// <summary>
		/// Gets the connected device.
		/// </summary>
		/// <value>The connected device.</value>
		internal string Connected { get { return _connected; } }

		/// <summary>
		/// The sleep time of the thread.
		/// </summary>
		private const int _SLEEP_TIME = 50;

		/// <summary>
		/// The buffer to store out messages.
		/// </summary>
		private readonly BTBuffer _buffer = new BTBuffer ();

		/// <summary>
		/// The writer thread.
		/// </summary>
		private readonly Thread _writer;

		/// <summary>
		/// The monitor to signal that the terminate ACK is send.
		/// </summary>
		private readonly object _monitor;

		/// <summary>
		/// Initializes a new instance of the <see cref="BTLibrary.BTWriteThread"/> class.
		/// </summary>
		/// <param name="socket">Bluetooth socket.</param>
		internal BTWriteThread (BluetoothSocket socket, object monitor)
		{
			_Socket = socket;
			Stream tmpOut = null;

			// Get the BluetoothSocket input and output streams
			try {
				tmpOut = _Socket.OutputStream;
			} catch {
			}

			_OutStream = tmpOut;

			_monitor = monitor;
			//start the thread
			_connected = _Socket.RemoteDevice.Address;
			_writer = new Thread (Write);
			_writer.Name = "Writer";
			_writer.Start ();

		}

		internal BTWriteThread (BluetoothSocket socket)
		{
			_Socket = socket;
			Stream tmpOut = null;

			// Get the BluetoothSocket input and output streams
			try {
				tmpOut = _Socket.OutputStream;
			} catch {
			}

			_OutStream = tmpOut;

			_monitor = null;
			//start the thread
			_connected = _Socket.RemoteDevice.Address;
			_writer = new Thread (Write);
			_writer.Name = "Writer";
			_writer.Start ();

		}

		/// <summary>
		/// Starts executing the active part of the writer thread.
		/// </summary>
		private void Write ()
		{
			while (true) {
				lock (_buffer) {
					if (!_buffer.isEmpty) {
						byte [] msg = _buffer.getValue ();

						try {
							_OutStream.Write (msg, 0, msg.Length);
							// Share the sent message back 
							BTManager.Instance.ObtainMessage ((int) EnLocalMessageType.MESSAGE_WRITE, -1, -1, msg).SendToTarget ();

						} catch {
							BTManager.Instance.ObtainMessage ((int) EnLocalMessageType.MESSAGE_CONNECTION_LOST, _connected).SendToTarget ();
						}

						if (msg [0] == (int) EnPackageType.ACK) {
							_buffer.Remove (msg);
							if (msg [18] == (int) EnPackageType.TERMINATE && _monitor != null)
								lock (_monitor) {
									Monitor.Pulse (_monitor);
								}
						}
					} else
						Monitor.Wait (_buffer);
				}

				Thread.Sleep (_SLEEP_TIME);
			}
		}

		/// <summary>
		/// Add the specified message to the buffer.
		/// </summary>
		/// <param name="elem">Array of bytes representing the message to be sent.</param>
		internal void Add (byte [] elem)
		{
			lock (_buffer) {
				_buffer.Add (elem);
				Monitor.Pulse (_buffer);
			}
		}

		/// <summary>
		/// Removes the message from the buffer.
		/// </summary>
		/// <param name="elem">Array of bytes representing the message to be removed.</param>
		internal void Remove (byte [] elem)
		{
			lock (_buffer) {
				_buffer.Remove (elem);
			}
		}

		/// <summary>
		/// Nothings to send.
		/// </summary>
		/// <returns><c>true</c>, if there aren't message to send, <c>false</c> otherwise.</returns>
		internal bool NothingToSend ()
		{

			return _buffer.isEmpty;
		}

		/// <summary>
		/// Try to close the socket
		/// </summary>
		/// <returns><c>true</c> if this instance cancel ; otherwise, <c>false</c>.</returns>
		internal void Cancel ()
		{
			try {
				_OutStream.Close ();
				_Socket.Close ();
			} catch {
			}

			_writer.Abort ();
		}
	}

	/// <summary>
	/// Buffer object to store output messages.
	/// Implemented as a list. ACK are inserted on the head to give priority, the other messages are inserted on the tail.
	/// Removal is based on which ACK is recived (could be any index of the list). 
	/// </summary>
	internal class BTBuffer
	{
		/// <summary>
		/// The buffer.
		/// </summary>
		private readonly List<byte []> _buffer = new List<byte []> ();

		/// <summary>
		/// Gets a value indicating whether this <see cref="BTLibrary.BTBuffer"/> is empty.
		/// </summary>
		/// <value><c>true</c> if is empty; otherwise, <c>false</c>.</value>
		internal bool isEmpty{ get { return _buffer.Count == 0; } }

		/// <summary>
		/// Gets the first message in the buffer.
		/// </summary>
		/// <returns>The message.</returns>
		internal byte[] getValue ()
		{
			return _buffer [0];
		}

		/// <summary>
		/// Add the specified message to the buffer.
		/// </summary>
		/// <param name="elem">The message.</param>
		internal void Add (byte [] elem)
		{
			if (elem [0] == (int) EnPackageType.ACK)
				_buffer.Insert (0, elem);
			else
				_buffer.Add (elem);
		}

		/// <summary>
		/// Remove the specified message from the buffer.
		/// </summary>
		/// <param name="elem">The message.</param>
		internal void Remove (byte [] elem)
		{
			_buffer.RemoveAll (delegate(byte [] b) {
				bool temp = true;
				for (int i = 0; i < b.Length; i++)
					temp = temp && b [i] == elem [i];

				return temp;
			});

		}
	}


}

