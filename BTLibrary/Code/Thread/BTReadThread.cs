﻿using System;
using Android.Bluetooth;
using System.IO;
using Android.OS;
using System.Threading;
using Android.Util;

namespace BTLibrary
{
	/// <summary>
	/// Thread that waits for incoming messages.
	/// </summary>
	internal class BTReadThread
	{
		/// <summary>
		/// The BluetoothSocket.
		/// </summary>
		internal BluetoothSocket _socket;

		/// <summary>
		/// The input stream.
		/// </summary>
		private readonly Stream _inStream;

		/// <summary>
		/// The connected device address.
		/// </summary>
		private readonly string _connected;

		/// <summary>
		/// The reader Thread.
		/// </summary>
		private readonly Thread _reader;

		/// <summary>
		/// Initializes a new instance of the <see cref="BTLibrary.BTReadThread"/> class.
		/// </summary>
		/// <param name="socket">Bluetooth socket.</param>
		internal BTReadThread (BluetoothSocket socket)
		{
			_socket = socket;
			Stream tmpIn = null;

			// Get the BluetoothSocket input stream.
			try {
				tmpIn = _socket.InputStream;
			} catch {
			}
			_inStream = tmpIn;

			_connected = _socket.RemoteDevice.Address;

			//start the reader thread
			_reader = new Thread (Read);
			_reader.Name = "Reader";
			_reader.Start ();
		}

		/// <summary>
		/// Starts executing the active part of the reader thread.
		/// </summary>
		private void Read ()
		{
			byte [] buffer = new byte[1024];
			int bytes;

			// Keep listening to the InputStream while connected
			while (true) {
				try {
					buffer = new byte[1024];
					// Read from the InputStream
					bytes = _inStream.Read (buffer, 0, buffer.Length);

					// Send the obtained bytes to the BTPlayservice
					BTManager.Instance.ObtainMessage ((int) EnLocalMessageType.MESSAGE_DEVICE_READ, _connected).SendToTarget ();
					BTManager.Instance.ObtainMessage ((int) EnLocalMessageType.MESSAGE_READ, bytes, -1, buffer).SendToTarget ();

				} catch {
					BTManager.Instance.ObtainMessage ((int) EnLocalMessageType.MESSAGE_CONNECTION_LOST, _connected).SendToTarget ();
					break;
				}
			}
		}


		/// <summary>
		/// Try to close the socket
		/// </summary>
		/// <returns><c>true</c> if this instance cancel ; otherwise, <c>false</c>.</returns>
		internal void Cancel ()
		{
			try {
				_inStream.Close ();
				_socket.Close ();

			} catch {
			}

			_reader.Abort ();
			Log.Debug ("Reader:", "Abort");
		}
	}
}
