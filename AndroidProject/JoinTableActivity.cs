﻿using System;
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

namespace GUILayout
{
	/// <summary>
	/// Activity to join an existing table.
	/// </summary>
	[Activity (Label = "JoinTableActivity", ScreenOrientation = ScreenOrientation.ReverseLandscape)]			
	internal class JoinTableActivity : Activity
	{
		/// <summary>
		/// The device address length.
		/// </summary>
		public const int _DEVICE_ADDRESS_LENGHT = 17;

		/// <summary>
		/// Width of the column.
		/// </summary>
		private const float _FRACTION_WIDTH = 0.3f;

		/// <summary>
		/// Buttons to scan for games, send updated name, close the activity or disconnect from current game.
		/// </summary>
		private Button _scan, _send, _back, _disconnect;

		/// <summary>
		/// Progress barr indicating the busy status
		/// </summary>
		private ProgressBar _pb;

		/// <summary>
		/// TextBox used to edit bluetooth name.
		/// </summary>
		private EditText _name;


		private ArrayAdapter<string> _pairedArrayList, _newArrayList;


		private bool _start, _normalEnd, _connecting;


		private string _address = "", _conn = "";


		private ListView _paired, _newdev;

		/// <summary>
		/// Relative layout.
		/// </summary>
		private RelativeLayout _newDeviceLayout;

		private AlertDialog.Builder _connect = null;

		/// <summary>
		/// Called on activity creation.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			_start = _normalEnd = true;
			_connecting = false;

			BTManager.Instance.RegisterReceivers ();

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
			_paired.ItemClick += pairedDevicelistClick;
			_paired.LayoutParameters.Width = (int) ( widthInDp * _FRACTION_WIDTH );
			_paired.Enabled = true;

			_newdev = FindViewById<ListView> (Resource.Id.newdev);
			_newArrayList = new ArrayAdapter<string> (this, Resource.Layout.device_name);
			_newdev.Adapter = _newArrayList;
			_newdev.ItemClick += newDevicelistClick;
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

			_playerControllerList = new List<BTPlayerController> (Board.PLAYER_NUMBER);
			for (int i = 0; i < Board.PLAYER_NUMBER; i++)
				_playerControllerList.Add (new BTPlayerController (i));

			BTManager.Instance.eventLocalMessageReceived += handleLocalMessage;
			BTManager.Instance.eventPackageReceived += handlePackage;

			if (_connect != null)
				_connect.Dispose ();

			_connect = new AlertDialog.Builder (this);


		}



		/// <summary>
		/// Called when the activity has detected the user's press of the back
		///  key.
		/// </summary>
		public override void OnBackPressed ()
		{
			base.OnBackPressed ();
			Back ();
		}

		/// <summary>
		/// Scan for nearby bluetooth devices.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
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

		/// <summary>
		/// Sends the name contained in the textbox to the paired bluetooth device.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
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

		/// <summary>
		/// Disconnect from the paired bluetooth device.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Disconnect (object sender, EventArgs e)
		{
			BTManager.Instance.Reset ();
			_send.Enabled = false;
			_name.Enabled = false;
			_paired.Enabled = true;
			_paired.Enabled = true;
			_scan.Enabled = true;
			_disconnect.Enabled = false;
			this.SetTitle (Resource.String.select);
		}

		/// <summary>
		/// Closes the activity.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Back (object sender, EventArgs e)
		{	
			Back ();
		}

		/// <summary>
		/// Performs back operations.
		/// </summary>
		private void Back ()
		{
			Intent returnIntent = new Intent ();
			SetResult (Result.Canceled, returnIntent);
			BTManager.Instance.Reset ();
			BTManager.Instance.UnregisterReceiver ();
			Finish ();
		}

		/// <summary>
		/// Method called when an item on the paired devices list is pressed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void pairedDevicelistClick (object sender, AdapterView.ItemClickEventArgs e)
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

		/// <summary>
		/// Method called when an item on the new devices list is pressed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void newDevicelistClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			_connecting = true;
			var info = ( e.View as TextView ).Text.ToString ();
			_address = info.Substring (info.Length - _DEVICE_ADDRESS_LENGHT);

			if (BTManager.Instance.isDiscovering ()) {
				BTManager.Instance.CancelDiscovery ();
				_normalEnd = false;
			}
			SetTitle (Resource.String.pairing);
			BTManager.Instance.CreateBond (_address);
		}

		/// <summary>
		/// Connects with the selected bluetooth device.
		/// </summary>
		private void connection ()
		{
			AlertDialog.Builder connect = new AlertDialog.Builder (this);
			connect.SetTitle ("Connection");
			connect.SetMessage ("Do you really want to connect with " + BTManager.Instance.getRemoteDevice (_address).Name + "?");
			connect.SetCancelable (true);
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

		/// <param name="requestCode">The integer request code originally supplied to
		///  startActivityForResult(), allowing you to identify who this
		///  result came from.</param>
		/// <param name="resultCode">The integer result code returned by the child activity
		///  through its setResult().</param>
		/// <param name="data">An Intent, which can return result data to the caller
		///  (various data can be attached to Intent "extras").</param>
		/// <summary>
		/// Called when an activity you launched exits, giving you the requestCode
		///  you started it with, the resultCode it returned, and any additional
		///  data from it.
		/// </summary>
		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			switch (requestCode) {

				case (int)EnActivityResultCode.VISIBILITY_REQUEST:
					// When the request to enable Bluetooth returns
					if (resultCode == Result.FirstUser) {
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
					} else if (resultCode == Result.Canceled)
						BTManager.Instance.DisableBluetooth ();
				break;

			}
		}

		private List<BTPlayerController> _playerControllerList;

		/// <summary>
		/// Handles the bluetooth messages recived (only board packages will be accepted).
		/// </summary>
		/// <param name="pkg">Package containing the message.</param>
		private void handlePackage (PackageBase pkg)
		{
			if (pkg == EnPackageType.BOARD) {
				this.SetTitle (Resource.String.starting);
				_send.Enabled = false;
				_name.Enabled = false;

				Intent returnIntent = new Intent ();
				this.SetResult (Result.Ok, returnIntent);

				Board.Instance.InitializeSlave (( (PackageBoard) pkg ).bytes, _name.Text);

				BTManager.Instance.initializeCommunication ();


				for (int i = 0; i < Board.PLAYER_NUMBER; ++i)
					if (Board.Instance.Me.order != i)
						( (Player) i ).SetController (_playerControllerList [i]);



				BTManager.Instance.eventLocalMessageReceived -= handleLocalMessage;
				BTManager.Instance.eventPackageReceived -= handlePackage;
		
				Finish ();
			}
		}

		/// <summary>
		/// Handles the local messages.
		/// </summary>
		/// <param name="msg">Message.</param>
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

						if (!IsFinishing) {
							_connect.SetTitle ("End Scanning");
							_connect.SetMessage ("Scanning For New Device Finished");
							_connect.SetNeutralButton ("OK", delegate {
							});
							_connect.Show ();
							this.SetTitle (Resource.String.select);
						}

					}
					_scan.Enabled = true;
					_pb.Visibility = ViewStates.Invisible;

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
					BTManager.Instance.RemoveMaster ();
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
				case (int) EnLocalMessageType.PAIRING_SUCCESS:
					string addr = (string) msg.Obj;
					string device = BTManager.Instance.getRemoteDevice (addr).Name + "\n" + addr;
					_newArrayList.Remove (device);
					_pairedArrayList.Add (device);
					SetTitle (Resource.String.connecting);
					_pb.Visibility = ViewStates.Visible;
					BTManager.Instance.ConnectAsSlave (BTManager.Instance.getRemoteDevice (addr));
					_connecting = false;

				break;

			}

		}

	}
		
}

