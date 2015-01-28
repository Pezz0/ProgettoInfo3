using System;
using Android.Bluetooth;
using Java.Util;
using System.Threading;


namespace BTLibrary
{
	/// <summary>
	/// Listens for connection request (used by the master).
	/// </summary>
	internal class BTListenThread
	{
		/// <summary>
		/// The BluetoothServerSocket.
		/// </summary>
		private BluetoothServerSocket _serverSocket;

		/// <summary>
		/// Thread that waits for connection requests.
		/// </summary>
		private Thread _Listener;

		/// <summary>
		/// Initializes a new instance of the <see cref="BTLibrary.BTListenThread"/> class.
		/// </summary>
		/// <param name="NAME">Name.</param>
		/// <param name="MY_UUID">My UUID.</param>
		public BTListenThread (string NAME, UUID MY_UUID)
		{
			BluetoothServerSocket tmp = null;

			try {
				tmp = BTManager.Instance.getBTAdapter ().ListenUsingRfcommWithServiceRecord (NAME, MY_UUID);
			} catch {
			}

			//start the thread
			_serverSocket = tmp;
			_Listener = new Thread (Listen);
			_Listener.Name = "ListenThread";
			_Listener.Start ();
		}

		/// <summary>
		/// Starts executing the active part of ListenThread.
		/// </summary>
		private void Listen ()
		{ 
			BluetoothSocket socket = null;
			try {
				// This is a blocking call and will only return on a
				// successful connection or an exception
				socket = _serverSocket.Accept ();
			} catch (Exception e) {
				e.ToString ();
			}

			// If a connection was accepted
			if (socket != null) {
				lock (this) {
					if (BTManager.Instance.GetState () == EnConnectionState.STATE_LISTEN)
						BTManager.Instance.ConnectedToSlave (socket, socket.RemoteDevice);
					else {
						try {
							socket.Close ();
						} catch {
						}
					}
				}
			}
		}

		/// <summary>
		/// try to close the server socket  .
		/// </summary>
		public void Cancel ()
		{
			try {
				_serverSocket.Close ();
			} catch {
			}
		}
	}
}

