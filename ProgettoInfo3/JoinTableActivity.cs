using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
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
		public const int MAX_NAME_LENGHT = 10, DEVICE_ADDRESS_LENGHT = 17;

		private static Button scan, send, back;

		private static ProgressBar pb;

		private static EditText name;

		private static ArrayAdapter<string> pairedArrayList, newArrayList;

		private static bool start, normalEnd, connecting;

		private static string address = "";

		private static ListView paired, newdev;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			start = normalEnd = true;
			connecting = false;

			//BTPlayService.Instance.AddHandler (new BTAckHandler ());
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

			pb = FindViewById<ProgressBar> (Resource.Id.progress);
			pb.Visibility = ViewStates.Invisible;

			pairedArrayList.Clear ();
			paired.Adapter = pairedArrayList; 

			paired.ItemClick += devicelistClick;

			newdev.Adapter = newArrayList;
			newdev.ItemClick += devicelistClick;

			scan = FindViewById<Button> (Resource.Id.scan);
			scan.Click += scanDevice;
			scan.Enabled = true;

			send = FindViewById<Button> (Resource.Id.Sendname);
			send.Enabled = false;
			send.Click += SendName;

			back = FindViewById<Button> (Resource.Id.back);
			back.Click += Back;

			name = FindViewById<EditText> (Resource.Id.MyName);


			string namedev = BTPlayService.Instance.GetLocalName ();
			if (namedev.Length > MAX_NAME_LENGHT)
				name.Text = namedev.Substring (0, MAX_NAME_LENGHT);
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

			BTPlayService.Instance.eventMsgInitilizationRecieved += handleMessage;
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
			normalEnd = true;
			scan.Enabled = false;
			if (BTPlayService.Instance.isBTEnabled ()) {
				SetTitle (Resource.String.scanning);
				newArrayList.Clear ();
				BTPlayService.Instance.Discovery ();
				pb.Visibility = ViewStates.Visible;
			} else
				BTPlayService.Instance.enableBluetooth ();

			
		}

		private void SendName (object sender, EventArgs e)
		{
			if (name.Text.CompareTo ("") != 0) {
				if (name.Text.Length > MAX_NAME_LENGHT) {
					string sub = name.Text.Substring (0, MAX_NAME_LENGHT);
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
			connecting = true;
			var info = ( e.View as TextView ).Text.ToString ();
			address = info.Substring (info.Length - DEVICE_ADDRESS_LENGHT);

			if (BTPlayService.Instance.isDiscovering ()) {
				BTPlayService.Instance.CancelDiscovery ();
				normalEnd = false;
			}

			if (BTPlayService.Instance.isBTEnabled ()) {
				connection ();
			} else {
				BTPlayService.Instance.enableBluetooth ();
			}
		}

		private void connection ()
		{

			AlertDialog.Builder connect = new AlertDialog.Builder (this);
			connect.SetTitle ("Connection");
			connect.SetMessage ("Do you really want to connect with " + BTPlayService.Instance.getRemoteDevice (address).Name + "?");
			connect.SetPositiveButton ("YES", delegate {
				SetTitle (Resource.String.connecting);
				pb.Visibility = ViewStates.Visible;
				BTPlayService.Instance.ConnectAsSlave (BTPlayService.Instance.getRemoteDevice (address));
				address = "";
				connecting = false;
			});
			connect.SetNegativeButton ("NO", delegate {
				address = "";
				connecting = false;
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
						if (connecting)
							connection ();
					}
				break;

			}
		}

		private void handleMessage (Message msg)
		{
			string conn = "";

			switch (msg.What) {

				case (int)MessageType.NEW_DEVICE:
					string address = (string) msg.Obj;
					newArrayList.Add (BTPlayService.Instance.getRemoteDevice (address).Name + "\n" + address);

				break;

				case (int) MessageType.END_SCANNING:
					if (normalEnd) {
						AlertDialog.Builder connect = new AlertDialog.Builder (this);
						connect.SetTitle ("End Scanning");
						connect.SetMessage ("Scanning For New Device Finished");
						connect.SetNeutralButton ("OK", delegate {
						});
						connect.Show ();

					}
					scan.Enabled = true;
					pb.Visibility = ViewStates.Invisible;
					this.SetTitle (Resource.String.select);
				break;
				case (int)MessageType.NONE_FOUND:
					if (normalEnd) {
						newArrayList.Add ("No Device Found");
					}
				break;
				case (int) MessageType.MESSAGE_STATE_CHANGE:
					if (msg.Arg1 == (int) ConnectionState.STATE_CONNECTED_SLAVE) {
						pb.Visibility = ViewStates.Invisible;
						send.Enabled = true;
						Toast.MakeText (Application.Context, "Connected to " + conn, ToastLength.Short).Show ();
						this.SetTitle (Resource.String.change_name);
					}

				break;
				case (int)MessageType.MESSAGE_READ:
					this.SetTitle (Resource.String.starting);
					send.Enabled = false;
					Intent returnIntent = new Intent ();
					returnIntent.PutExtra ("Board", (byte []) msg.Obj);
					returnIntent.PutExtra ("Name", name.Text.ToCharArray ());
					this.SetResult (Result.Ok, returnIntent);
					this.Finish ();
				break;
				case (int)MessageType.MESSAGE_TOAST:
					pb.Visibility = ViewStates.Invisible;
					Toast.MakeText (Application.Context, (string) msg.Obj, ToastLength.Long).Show ();
					this.SetTitle (Resource.String.select);
				break;
				case (int) MessageType.MESSAGE_DEVICE_ADDR:
					conn = BTPlayService.Instance.getRemoteDevice ((string) msg.Obj).Name;
				break;

			}
		}

	}
}

