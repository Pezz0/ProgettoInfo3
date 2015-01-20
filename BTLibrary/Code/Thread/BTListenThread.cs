using System;
using Android.Bluetooth;
using Java.Util;
using System.Threading;


namespace BTLibrary
{
	internal class BTListenThread
	{
		/// <summary>
		/// The BluetoothServerSocket.
		/// </summary>
		private BluetoothServerSocket _serverSocket;

		/// <summary>
		/// The listener thread.
		/// </summary>
		private Thread _Listener;

		public BTListenThread (string NAME, UUID MY_UUID)
		{
			BluetoothServerSocket tmp = null;

			try {
				tmp = BTPlayService.Instance.getBTAdapter ().ListenUsingRfcommWithServiceRecord (NAME, MY_UUID);
			} catch (Exception e) {
				e.ToString ();
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
					if (BTPlayService.Instance.GetState () == EnConnectionState.STATE_LISTEN)
						BTPlayService.Instance.ConnectedToSlave (socket, socket.RemoteDevice);
					else {
						try {
							socket.Close ();
						} catch (Exception e) {
							//unable to colse unwanted socket
							e.ToString ();
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
			} catch (Exception e) {
				e.ToString ();
			}
		}
	}
}

