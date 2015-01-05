
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BTLibrary;

namespace ProgettoInfo3
{
	[Activity (Label = "JoinTableActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.ReverseLandscape)]			
	public class JoinTableActivity : Activity
	{
		private Button scan;
		private ArrayAdapter<string> pairedArrayList;
		private static ArrayAdapter<string> newArrayList;
		BTReceiver receiver;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.DeviceList);

			SetTitle (Resource.String.select);
			scan = FindViewById<Button> (Resource.Id.scan);
			ListView paired = FindViewById<ListView> (Resource.Id.paired);
			ListView newdev = FindViewById<ListView> (Resource.Id.newdev);
			pairedArrayList = new ArrayAdapter<string> (this, Resource.Layout.device_name);
			newArrayList = new ArrayAdapter<string> (this, Resource.Layout.device_name);

			paired.Adapter = pairedArrayList; //NON VANNO GLI ADAPTER
			paired.ItemClick += (sender, e) => devicelistClick (sender, e);

			newdev.Adapter = newArrayList;
			newdev.ItemClick += (sender, e) => devicelistClick (sender, e);

			scan = FindViewById<Button> (Resource.Id.scan);
			scan.Click += (sender, e) => scanDevice (sender, e);

			receiver = new BTReceiver (new BTConnHandler (this, this));
			var filter = new IntentFilter (BTPlayService.found);
			RegisterReceiver (receiver, filter);

			// Register for broadcasts when discovery has finished
			filter = new IntentFilter (BTPlayService.endscan);
			RegisterReceiver (receiver, filter);


			BTPlayService.Instance.Initialize (this, new BTHandler ());

			if (!BTPlayService.Instance.existBluetooth ()) {
				Toast.MakeText (this, "BlueTooth not supported", ToastLength.Short);
				Finish ();
			}

			if (!BTPlayService.Instance.isBTEnabled ())
				BTPlayService.Instance.enableBluetooth ();
			else {
				List<string> address = BTPlayService.Instance.GetPaired ();
				foreach (string addr in address)
					pairedArrayList.Add (BTPlayService.Instance.getRemoteDevice (addr).Name + "\n" + addr);

			}

		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			UnregisterReceiver (receiver);
		}

		void scanDevice (object sender, EventArgs e)
		{
			SetTitle (Resource.String.scanning);
			newArrayList.Clear ();
			BTPlayService.Instance.Discovery ();
		}

		void devicelistClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			if (BTPlayService.Instance.isDiscovering ())
				BTPlayService.Instance.CancelDiscovery ();
				
			var info = ( e.View as TextView ).Text.ToString ();
			var address = info.Substring (info.Length - 17);

			AlertDialog.Builder connect = new AlertDialog.Builder (this);
			connect.SetTitle ("Connection");
			connect.SetMessage ("Do you really want to connect with " + BTPlayService.Instance.getRemoteDevice (address).Name + "?");
			connect.SetPositiveButton ("YES", delegate {
				BTPlayService.Instance.ConnectAsSlave (BTPlayService.Instance.getRemoteDevice (address));
			});
			connect.SetNegativeButton ("NO", delegate {
				Finish ();
			});
			connect.Show ();


		

		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			switch (requestCode) {

				case (int)ActivityResultCode.REQUEST_ENABLE_BT:
					// When the request to enable Bluetooth returns
					if (resultCode == Result.Ok) {
						// Bluetooth is now enabled, so set up a chat session
						List<string> address = BTPlayService.Instance.GetPaired ();
						foreach (string addr in address)
							pairedArrayList.Add (BTPlayService.Instance.getRemoteDevice (addr).Name + "\n" + addr);

					}
				break;

			}
		}

		private class BTConnHandler:Handler
		{
			Context c;
			Activity a;

			public BTConnHandler (Context co, Activity ac)
			{
				c = co;
				a = ac;
			}

			public override void HandleMessage (Message msg)
			{
				switch (msg.What) {

					case (int)MessageType.NEW_DEVICE:
						string address = (string) msg.Obj;
						newArrayList.Add (BTPlayService.Instance.getRemoteDevice (address) + "\n" + address);

					break;

					case (int) MessageType.END_SCANNING:
						AlertDialog.Builder connect = new AlertDialog.Builder (c);
						connect.SetTitle ("End Scanning");
						connect.SetMessage ("Scanning For New Device Finished");
						connect.SetNeutralButton ("OK", delegate {
						});
						connect.Show ();
						a.SetTitle (Resource.String.select);
					break;
					case (int)MessageType.NONE_FOUND:
						newArrayList.Add ("No Device Found");
					break;

				}
			}
		
		}
	}
}

