using Java.Lang;
using Android.Bluetooth;
using Java.Util;


namespace BTLibrary
{
	internal class BTListenThread:Thread
	{
		/// <summary>
		/// The BluetoothServerSocket.
		/// </summary>
		private BluetoothServerSocket _serverSocket;

		/// <summary>
		/// The BluetoothPlayService.
		/// </summary>
		private BTPlayService _PlayService;

		public BTListenThread (BTPlayService playService, string NAME, UUID MY_UUID)
		{
			_PlayService = playService;
			BluetoothServerSocket tmp = null;

			try {
				tmp = _PlayService.getBTAdapter ().ListenUsingRfcommWithServiceRecord (NAME, MY_UUID);
			} catch (Exception e) {
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
					if (_PlayService.GetState () == ConnectionState.STATE_LISTEN)
						_PlayService.ConnectedToSlave (socket, socket.RemoteDevice);
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

