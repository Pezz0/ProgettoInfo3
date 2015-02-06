using System;
using Android.OS;
using Android.Content;
using Android.Bluetooth;
using Android.Widget;

namespace BTLibrary
{
	internal class BTReceiver : BroadcastReceiver
	{

		/// <summary>
		/// The counter that indicates the number of new device discovered.
		/// </summary>
		private int count = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="BTLibrary.BTReceiver"/> class.
		/// </summary>
		public BTReceiver ()
		{
		}

		/// <param name="context">The Context in which the receiver is running.</param>
		/// <param name="intent">The Intent being received.</param>
		/// <summary>
		/// This method is called when the BroadcastReceiver is receiving an Intent broadcast.
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
					BTManager.Instance.ObtainMessage ((int) EnLocalMessageType.NEW_DEVICE, device.Address).SendToTarget ();
					count++;
				}

				//When discovery finish
			} else if (action == BluetoothAdapter.ActionDiscoveryFinished) {
				//send to the Activity a message that indicate that the discovery is finished
				BTManager.Instance.ObtainMessage ((int) EnLocalMessageType.END_SCANNING, -1, -1).SendToTarget ();
				//if no device were found
				if (count == 0)
					//send to the activity a message that no Device are found
					BTManager.Instance.ObtainMessage ((int) EnLocalMessageType.NONE_FOUND, -1, -1).SendToTarget ();
			} else if (action == BluetoothDevice.ActionBondStateChanged) {
				int state = intent.GetIntExtra (BluetoothDevice.ExtraBondState, BluetoothDevice.Error);
				int prevState = intent.GetIntExtra (BluetoothDevice.ExtraPreviousBondState, BluetoothDevice.Error);
				BluetoothDevice extradev = (BluetoothDevice) intent.GetParcelableExtra (BluetoothDevice.ExtraDevice);

				if (state == (int) Bond.Bonded && prevState == (int) Bond.Bonding) {
					BTManager.Instance.ObtainMessage ((int) EnLocalMessageType.PAIRING_SUCCESS, extradev.Address).SendToTarget ();
				} 
			} else if (action == BluetoothDevice.ActionPairingRequest) {
				intent.SetFlags (ActivityFlags.NewTask);
				context.StartActivity (intent);
				//BTManager.Instance.ObtainMessage ((int) EnLocalMessageType.PAIRING_REQUEST, inten).SendToTarget ();
			}
		}
	}
}

