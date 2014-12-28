using System;
using Android.OS;
using Android.Content;
using Android.Bluetooth;

namespace ProvaConnessioneBT
{
	public class Receiver : BroadcastReceiver
	{
		/// <summary>
		/// The handler to communicate to activity the result of a receiving Intent.
		/// </summary>
		Handler _handler;

		/// <summary>
		/// the counter that inicate if any new device is dicovered
		/// </summary>
		private static int cont = 0;

		public Receiver (Handler handler)
		{
			_handler = handler;
		}

		/// <param name="context">The Context in which the receiver is running.</param>
		/// <param name="intent">The Intent being received.</param>
		/// <summary>
		/// This method is called when the BroadcastReceiver is receiving an Intent
		///  broadcast.
		/// </summary>
		public override void OnReceive (Context context, Intent intent)
		{ 
			string action = intent.Action;

			// When discovery finds a device
			if (action == BluetoothDevice.ActionFound) {

				// Get the BluetoothDevice object from the Intent
				BluetoothDevice device = (BluetoothDevice) intent.GetParcelableExtra (BluetoothDevice.ExtraDevice);

				// If it's already paired, skip it, because it's been listed already
				if (device.BondState != Bond.Bonded) {
					_handler.ObtainMessage ((int) MessageType.NEW_DEVICE, device.Address).SendToTarget ();
					cont++;
				}

				//When discovery finish
			} else if (action == BluetoothAdapter.ActionDiscoveryFinished) {
				//send to the Activity a message that indicate that the discovery is finished
				_handler.ObtainMessage ((int) MessageType.END_SCANNING, -1, -1).SendToTarget ();
				//if no device were found
				if (cont == 0)
					//send to the activity a message that no Device are found
					_handler.ObtainMessage ((int) MessageType.NONE_FOUND, -1, -1).SendToTarget ();
			}
		}
	}
}

