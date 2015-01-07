
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
using ChiamataLibrary;
using Android.Content.PM;

namespace ProgettoInfo3
{
	[Activity (Label = "JoinTableActivity", ScreenOrientation = ScreenOrientation.ReverseLandscape)]			
	public class JoinTableActivity : Activity
	{
		private static Button scan;
		private static Button send;
		private static Button back;

		private static EditText name;

		private ArrayAdapter<string> pairedArrayList;
		private static ArrayAdapter<string> newArrayList;
		private bool start;

		ListView paired, newdev;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			start = true;

			BTPlayService.Instance.setHandler (new BTConnHandler (this, this));
			BTPlayService.Instance.RegisterReceiver ();

			BTPlayService.Instance.setActivity (this);

			SetContentView (Resource.Layout.DeviceList);

			Window.SetFlags (WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

			SetTitle (Resource.String.select);
			scan = FindViewById<Button> (Resource.Id.scan);
			paired = FindViewById<ListView> (Resource.Id.paired);
			newdev = FindViewById<ListView> (Resource.Id.newdev);
			pairedArrayList = new ArrayAdapter<string> (this, Resource.Layout.device_name);
			newArrayList = new ArrayAdapter<string> (this, Resource.Layout.device_name);

			pairedArrayList.Clear ();
			paired.Adapter = pairedArrayList; 

			paired.ItemClick += devicelistClick;

			newdev.Adapter = newArrayList;
			newdev.ItemClick += devicelistClick;

			scan = FindViewById<Button> (Resource.Id.scan);
			scan.Click += scanDevice;

			send = FindViewById<Button> (Resource.Id.Sendname);
			send.Enabled = false;
			send.Click += SendName;

			back = FindViewById<Button> (Resource.Id.back);
			back.Click += Back;

			name = FindViewById<EditText> (Resource.Id.MyName);


			string namedev = BTPlayService.Instance.GetLocalName ();
			if (namedev.Length > 10)
				name.Text = namedev.Substring (0, 10);
			else
				name.Text = namedev;

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
			BTPlayService.Instance.UnregisterReceiever ();
		}

		public override void OnBackPressed ()
		{
			base.OnBackPressed ();
			BTPlayService.Instance.Stop ();
			Finish ();
		}


		private void scanDevice (object sender, EventArgs e)
		{
			start = false;
			if (BTPlayService.Instance.isBTEnabled ()) {
				SetTitle (Resource.String.scanning);
				newArrayList.Clear ();
				BTPlayService.Instance.Discovery ();
			} else
				BTPlayService.Instance.enableBluetooth ();

			
		}

		private void SendName (object sender, EventArgs e)
		{
			if (name.Text.CompareTo ("") != 0) {
				if (name.Text.Length > 10) {
					string sub = name.Text.Substring (0, 10);
					AlertDialog.Builder setName = new AlertDialog.Builder (this);
					setName.SetTitle ("Name Too Long");
					setName.SetMessage ("Your name is too long\nDo you want to be registered on master with this name: " + sub + "?");
					setName.SetPositiveButton ("YES", delegate {
						BTPlayService.Instance.WriteToMaster (Encoding.ASCII.GetBytes (sub));

					});
					setName.SetNegativeButton ("NO", delegate {
						return;
					});
					setName.Show ();

				} else
					BTPlayService.Instance.WriteToMaster (Encoding.ASCII.GetBytes (name.Text));

			} else
				Toast.MakeText (this, "Insert a valid name", ToastLength.Short).Show ();
			
		}

		private void Back (object sender, EventArgs e)
		{
			BTPlayService.Instance.Stop ();
			Finish ();
		}

		private void devicelistClick (object sender, AdapterView.ItemClickEventArgs e)
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
				//Finish ();
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

						if (!start) {
							SetTitle (Resource.String.scanning);
							newArrayList.Clear ();
							BTPlayService.Instance.Discovery ();
						}
					}
				break;

			}
		}

		private class BTConnHandler:Handler
		{
			Context c;
			Activity a;
			string conn = "";

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
						newArrayList.Add (BTPlayService.Instance.getRemoteDevice (address).Name + "\n" + address);

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
					case (int) MessageType.MESSAGE_STATE_CHANGE:
						if (msg.Arg1 == (int) ConnectionState.STATE_CONNECTED_SLAVE) {
							send.Enabled = true;
							Toast.MakeText (Application.Context, "Connected to " + conn, ToastLength.Short).Show ();
							a.SetTitle (Resource.String.change_name);
						}
					break;
					case (int)MessageType.MESSAGE_READ:
						send.Enabled = false;
						Board.Instance.ricreateFromByteArray ((Byte []) msg.Obj);
						Board.Instance.initializeSlave (name.Text);
					break;
					case (int)MessageType.MESSAGE_TOAST:
						Toast.MakeText (Application.Context, (string) msg.Obj, ToastLength.Short).Show ();
					break;
					case (int) MessageType.MESSAGE_DEVICE_ADDR:
						conn = BTPlayService.Instance.getRemoteDevice ((string) msg.Obj).Name;
					break;
					
				}
			}
		
		}
	}
}

