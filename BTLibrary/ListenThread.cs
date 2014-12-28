using System;
using Java.Lang;
using Android.Bluetooth;
using Java.Util;


namespace ProvaConnessioneBT
{
	public class ListenThread:Thread
	{
		/// <summary>
		/// The BluetoothServerSocket.
		/// </summary>
		private BluetoothServerSocket _serverSocket;

		/// <summary>
		/// The BluetoothPlayService.
		/// </summary>
		private BluetoothPlayService _PlayService;

		public ListenThread (BluetoothPlayService playService, string NAME, UUID MY_UUID)
		{
			_PlayService = playService;
			BluetoothServerSocket tmp = null;

			try {
				tmp = _PlayService.getBTAdapter ().ListenUsingRfcommWithServiceRecord (NAME, MY_UUID);
			} catch (Java.IO.IOException e) {
				//listen failed
				e.ToString ();

			}

			_serverSocket = tmp;
		}

		/// <summary>
		/// Starts executing the active part of ListenThread.
		/// </summary>
		public override void Run ()
		{
			Name = "ListenThread";

			BluetoothSocket socket = null;

			while (_PlayService.GetState () != ConnectionState.STATE_CONNECTED_MASTER) {
				try {
					// This is a blocking call and will only return on a
					// successful connection or an exception
					socket = _serverSocket.Accept ();
				} catch (Java.IO.IOException e) {
					e.ToString ();
					//accept failed
				}

				// If a connection was accepted
				if (socket != null) {
					lock (this) {
						if (_PlayService.GetState () == ConnectionState.STATE_LISTEN)
							_PlayService.ConnectedToSlave (socket, socket.RemoteDevice);
						else {
							try {
								socket.Close ();
							} catch (Java.IO.IOException e) {
								e.ToString ();
								//unable to colse unwanted socket
							}
						}
					}//end lock
				}//end if
			}//end while
		}
		//end run

		/// <summary>
		/// try to close the server socket  .
		/// </summary>
		public void Cancel ()
		{
			try {
				_serverSocket.Close ();
			} catch (Java.IO.IOException e) {
				e.ToString ();
				//close of server failed
			}
		}
	}
}

