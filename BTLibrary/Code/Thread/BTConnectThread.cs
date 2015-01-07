﻿//using System;
using Java.Lang;
using Android.Bluetooth;
using Java.Util;

namespace BTLibrary
{
	/// <summary>
	/// Connect thread class.
	/// </summary>
	internal class BTConnectThread : Thread
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
		private BTPlayService _PlayService;

		public BTConnectThread (BluetoothDevice device, BTPlayService playService, UUID MY_UUID)
		{
			_device = device;
			_PlayService = playService;
			BluetoothSocket tmp = null;
			// Get a BluetoothSocket for a connection with the
			// given BluetoothDevice
			try {
				tmp = _device.CreateRfcommSocketToServiceRecord (MY_UUID);
			} catch (Exception e) {
				e.ToString ();
			}
			_socket = tmp;
		}

		/// <summary>
		/// Starts executing the active part of ConnectThread.
		/// </summary>
		public override void Run ()
		{
			Name = "ConnectThread";
		
			// Make a connection to the BluetoothSocket
			try {
				// This is a blocking call and will only return on a
				// successful connection or an exception
				_socket.Connect ();
			} catch (Exception e) {
				_PlayService.ConnectionFailed (e.Message);
				// Close the socket
				try {
					_socket.Close ();
				} catch (Exception e2) {
					//close fail
					e2.ToString ();
				}
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
			} catch (Exception e) {
				//close of connect socket failed
				e.ToString ();
			}
		}
	}
}

