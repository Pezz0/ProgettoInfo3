using System;
using Java.Lang;
using System.Collections.Generic;
using Android.Bluetooth;
using System.IO;
using Android.OS;

namespace BTLibrary
{
	public class WriteThread:Thread
	{
		Stack<byte []> pippo;
		private static DateTime timer;

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

		private string Connected;


		public WriteThread (BluetoothSocket socket)
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

			pippo = new Stack<byte []> ();
			timer = DateTime.Now;
		}

		public override void Run ()
		{
			while (true) {
				if (pippo.Count > 0) {
					byte [] buffer = pippo.Pop (); 
					try {
						_OutStream.Write (buffer, 0, buffer.Length);
						// Share the sent message back to the UI Activity
						BTPlayService.Instance.forEachHandler (delegate(Handler h) {
							h.ObtainMessage ((int) MessageType.MESSAGE_WRITE, -1, -1, buffer).SendToTarget ();
						});
					} catch (System.Exception e) {
						//exception during write
						e.ToString ();
					}

					Sleep (100);
				} 
			}
		}

		public void addlist (byte [] a)
		{
			pippo.Push (a);

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

