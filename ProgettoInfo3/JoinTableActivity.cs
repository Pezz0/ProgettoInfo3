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
using System.Runtime.CompilerServices;
using Android.Util;

namespace ProgettoInfo3
{
	[Activity (Label = "JoinTableActivity", ScreenOrientation = ScreenOrientation.ReverseLandscape)]			
	public class JoinTableActivity : Activity
	{
		public const int _DEVICE_ADDRESS_LENGHT = 17;

		private const float _FRACTION_WIDTH = 0.3f;

		private Button _scan, _send, _back, _disconnect;

		private ProgressBar _pb;

		private EditText _name;

		private ArrayAdapter<string> _pairedArrayList, _newArrayList;

		private bool _start, _normalEnd, _connecting;

		private string _address = "", _conn = "";

		private ListView _paired, _newdev;

		private RelativeLayout _newDeviceLayout;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			_start = _normalEnd = true;
			_connecting = false;

			BTManager.Instance.RegisterReceiver ();

			BTManager.Instance.setActivity (this);

			SetContentView (Resource.Layout.DeviceList);

			Window.SetFlags (WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
			SetTitle (Resource.String.select);

			DisplayMetrics metrics = Resources.DisplayMetrics;
			int widthInDp = metrics.WidthPixels;

			#region Display Management
			_paired = FindViewById<ListView> (Resource.Id.paired);
			_pairedArrayList = new ArrayAdapter<string> (this, Resource.Layout.device_name);
			_pairedArrayList.Clear ();
			_paired.Adapter = _pairedArrayList; 
			_paired.ItemClick += devicelistClick;
			_paired.LayoutParameters.Width = (int) ( widthInDp * _FRACTION_WIDTH );
			_paired.Enabled = true;

			_newdev = FindViewById<ListView> (Resource.Id.newdev);
			_newArrayList = new ArrayAdapter<string> (this, Resource.Layout.device_name);
			_newdev.Adapter = _newArrayList;
			_newdev.ItemClick += devicelistClick;
			_newDeviceLayout = FindViewById<RelativeLayout> (Resource.Id.Layout);
			_newDeviceLayout.LayoutParameters.Width = (int) ( widthInDp * _FRACTION_WIDTH );
			_newdev.Enabled = false;

			_pb = FindViewById<ProgressBar> (Resource.Id.progress);
			_pb.Visibility = ViewStates.Invisible;

			_scan = FindViewById<Button> (Resource.Id.scan);
			_scan.Click += scanDevice;
			_scan.Enabled = true;

			_send = FindViewById<Button> (Resource.Id.Sendname);
			_send.Enabled = false;
			_send.Click += SendName;

			_disconnect = FindViewById<Button> (Resource.Id.Disconnect);
			_disconnect.Enabled = false;
			_disconnect.Click += Disconnect;

			_back = FindViewById<Button> (Resource.Id.back);
			_back.Click += Back;

			_name = FindViewById<EditText> (Resource.Id.MyName);
			_name.Enabled = false;

			#endregion

			string namedev = BTManager.Instance.GetLocalName ();
			if (namedev.Length > MainActivity.MAX_NAME_LENGHT)
				_name.Text = namedev.Substring (0, MainActivity.MAX_NAME_LENGHT);
			else
				_name.Text = namedev;

			if (!BTManager.Instance.existBluetooth ()) {
				Toast.MakeText (this, "BlueTooth not supported", ToastLength.Short);
				Finish ();
			}

			if (!BTManager.Instance.isBTEnabled ())
				BTManager.Instance.enableBluetooth ();
			else {
				List<string> address = BTManager.Instance.GetPaired ();
				foreach (string addr in address)
					_pairedArrayList.Add (BTManager.Instance.getRemoteDevice (addr).Name + "\n" + addr);
			}

			BTManager.Instance.eventLocalMessageReceived += handleLocalMessage;
			BTManager.Instance.eventPackageReceived += handlePackage;
		}

		public override void OnBackPressed ()
		{
			base.OnBackPressed ();
			BTManager.Instance.Stop ();
			Finish ();
		}


		private void scanDevice (object sender, EventArgs e)
		{
			_start = false;
			_normalEnd = true;
			_scan.Enabled = false;
			if (BTManager.Instance.isBTEnabled ()) {
				SetTitle (Resource.String.scanning);
				_newArrayList.Clear ();
				BTManager.Instance.Discovery ();
				_pb.Visibility = ViewStates.Visible;
			} else
				BTManager.Instance.enableBluetooth ();

			
		}

		private void SendName (object sender, EventArgs e)
		{
			if (_name.Text.CompareTo ("") != 0) {
				if (_name.Text.Length > MainActivity.MAX_NAME_LENGHT) {
					string sub = _name.Text.Substring (0, MainActivity.MAX_NAME_LENGHT);
					AlertDialog.Builder setName = new AlertDialog.Builder (this);
					setName.SetTitle ("Name Too Long");
					setName.SetMessage ("Your name is too long\nDo you want to be registered on master with this name: " + sub + "?");
					setName.SetPositiveButton ("YES", delegate {
						BTManager.Instance.WriteToMaster (new PackageName (sub));

					});
					setName.SetNegativeButton ("NO", delegate {
						return;
					});
					setName.Show ();

				} else
					BTManager.Instance.WriteToMaster (new PackageName (_name.Text));

			} else
				Toast.MakeText (this, "Insert a valid name", ToastLength.Short).Show ();
			
		}

		private void Disconnect (object sender, EventArgs e)
		{
			BTManager.Instance.Stop ();
			_send.Enabled = false;
			_name.Enabled = false;
			_paired.Enabled = true;
			_paired.Enabled = true;
			_scan.Enabled = true;
			_disconnect.Enabled = false;
			this.SetTitle (Resource.String.select);
		}

		private void Back (object sender, EventArgs e)
		{
			BTManager.Instance.Stop ();
			Finish ();
		}

		private void devicelistClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			_connecting = true;
			var info = ( e.View as TextView ).Text.ToString ();
			_address = info.Substring (info.Length - _DEVICE_ADDRESS_LENGHT);

			if (BTManager.Instance.isDiscovering ()) {
				BTManager.Instance.CancelDiscovery ();
				_normalEnd = false;
			}

			if (BTManager.Instance.isBTEnabled ()) {
				connection ();
			} else {
				BTManager.Instance.enableBluetooth ();
			}
		}

		private void connection ()
		{

			AlertDialog.Builder connect = new AlertDialog.Builder (this);
			connect.SetTitle ("Connection");
			connect.SetMessage ("Do you really want to connect with " + BTManager.Instance.getRemoteDevice (_address).Name + "?");
			connect.SetPositiveButton ("YES", delegate {
				SetTitle (Resource.String.connecting);
				_pb.Visibility = ViewStates.Visible;
				BTManager.Instance.ConnectAsSlave (BTManager.Instance.getRemoteDevice (_address));
				_address = "";
				_connecting = false;
			});
			connect.SetNegativeButton ("NO", delegate {
				_address = "";
				_connecting = false;
			});
			connect.Show ();
		}


		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			switch (requestCode) {

				case (int)EnActivityResultCode.REQUEST_ENABLE_BT:
					// When the request to enable Bluetooth returns
					if (resultCode == Result.Ok) {
						// Bluetooth is now enabled, so set up a chat session
						List<string> address = BTManager.Instance.GetPaired ();
						foreach (string addr in address)
							_pairedArrayList.Add (BTManager.Instance.getRemoteDevice (addr).Name + "\n" + addr);

						if (!_start) {
							SetTitle (Resource.String.scanning);
							_newArrayList.Clear ();
							BTManager.Instance.Discovery ();
						}
						if (_connecting)
							connection ();
					}
				break;

			}
		}

		private void handlePackage (Package pkg)
		{
			if (pkg == EnPackageType.BOARD) {
				this.SetTitle (Resource.String.starting);
				_send.Enabled = false;
				_name.Enabled = false;

				Intent returnIntent = new Intent ();
				returnIntent.PutExtra ("Board", ( (PackageBoard) pkg ).bytes);
				returnIntent.PutExtra ("Name", _name.Text.ToCharArray ());
				this.SetResult (Result.Ok, returnIntent);

				BTManager.Instance.eventLocalMessageReceived -= handleLocalMessage;
				BTManager.Instance.eventPackageReceived -= handlePackage;

				Finish ();
			}
		}

		[MethodImpl (MethodImplOptions.Synchronized)]
		private void handleLocalMessage (Message msg)
		{
			switch (msg.What) {

				case (int)EnLocalMessageType.NEW_DEVICE:
					string address = (string) msg.Obj;
					_newArrayList.Add (BTManager.Instance.getRemoteDevice (address).Name + "\n" + address);
					_newdev.Enabled = true;

				break;

				case (int) EnLocalMessageType.END_SCANNING:
					if (_normalEnd) {
						AlertDialog.Builder connect = new AlertDialog.Builder (this);
						connect.SetTitle ("End Scanning");
						connect.SetMessage ("Scanning For New Device Finished");
						connect.SetNeutralButton ("OK", delegate {
						});
						connect.Show ();

					}
					_scan.Enabled = true;
					_pb.Visibility = ViewStates.Invisible;
					this.SetTitle (Resource.String.select);
				break;
				case (int)EnLocalMessageType.NONE_FOUND:
					if (_normalEnd) {
						_newArrayList.Add ("No Device Found");
						_newdev.Enabled = false;
					}
				break;

				case (int) EnLocalMessageType.MESSAGE_CONNECTION_LOST:
					_send.Enabled = false;
					_name.Enabled = false;
					_paired.Enabled = true;
					_paired.Enabled = true;
					_scan.Enabled = true;
					_disconnect.Enabled = false;
					this.SetTitle (Resource.String.select);
					Toast.MakeText (Application.Context, "Device connection lost", ToastLength.Short).Show ();
				break;
				case (int) EnLocalMessageType.MESSAGE_CONNECTION_FAILED:
					Toast.MakeText (Application.Context, "I don't find any BlueTooth game opened on this device", ToastLength.Short).Show ();
					_pb.Visibility = ViewStates.Invisible;
					this.SetTitle (Resource.String.select);
				break;
				case (int) EnLocalMessageType.MESSAGE_DEVICE_ADDR:
					if (msg.Arg1 == (int) EnConnectionState.STATE_CONNECTED_SLAVE) {
						_conn = BTManager.Instance.getRemoteDevice ((string) msg.Obj).Name;
						_pb.Visibility = ViewStates.Invisible;
						_send.Enabled = true;
						_name.Enabled = true;
						_paired.Enabled = false;
						_paired.Enabled = false;
						_scan.Enabled = false;
						_disconnect.Enabled = true;
						Toast.MakeText (Application.Context, "Connected to " + _conn, ToastLength.Short).Show ();
						this.SetTitle (Resource.String.change_name);
					}

				break;

			}
		}

	}
}

