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
using CocosSharp;
using Android.Content.PM;
using System.Runtime.CompilerServices;
using Android.Util;


namespace GUILayout
{
	/// <summary>
	/// Activity to create a new table.
	/// </summary>
	[Activity (Label = "CraeteTabActivity", ScreenOrientation = ScreenOrientation.ReverseLandscape)]			
	internal class CreateTabActivity : Activity
	{
		/// <summary>
		/// Spinner used to select if the slot is occupied by an AI or a Bluetooth
		/// </summary>
		private readonly Spinner [] _spinner = new Spinner[4];

		/// <summary>
		/// Player names
		/// </summary>
		private readonly EditText [] _pl = new EditText[5];

		/// <summary>
		/// Addresses of the bluetooth devices connected.
		/// </summary>
		private readonly string [] _addr = new string[4];

		/// <summary>
		/// Radio buttons used to decide to which slot will be assigned the dealer role.
		/// </summary>
		private readonly RadioButton [] _radioDealer = new RadioButton[5];
		/// <summary>
		/// Column titles.
		/// </summary>
		private TextView _titCol1, _titCol2, _titCol3;

		/// <summary>
		/// The Start and back buttons.
		/// </summary>
		private Button _start, _back;

		/// <summary>
		/// The adapter used for spinners.
		/// </summary>
		private ArrayAdapter adapter;

		/// <summary>
		/// Strings containing my bluetooth name and my bluetooth address.
		/// </summary>
		private string _name = "", _address = "";

		/// <summary>
		/// Width of the columns.
		/// </summary>
		private const float _FIRST_FRACTION_WIDTH = 0.15f, _SECOND_FRACTION_WIDTH = 0.2f, _THIRD_FRACTION_WIDTH = 0.3f;

		/// <summary>
		/// Called on activity creation.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			GameProfile gp = new GameProfile (Intent);

			SetContentView (Resource.Layout.CreateTable);
		
			Window.SetFlags (WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

			DisplayMetrics metrics = Resources.DisplayMetrics;
			int widthInDp = metrics.WidthPixels;

			#region Display Management
			_pl [0] = FindViewById<EditText> (Resource.Id.player1);
			_pl [1] = FindViewById<EditText> (Resource.Id.player2);
			_pl [2] = FindViewById<EditText> (Resource.Id.player3);
			_pl [3] = FindViewById<EditText> (Resource.Id.player4);
			_pl [4] = FindViewById<EditText> (Resource.Id.player5);

			_titCol1 = FindViewById<TextView> (Resource.Id.textView1);
			_titCol2 = FindViewById<TextView> (Resource.Id.textView7);
			_titCol3 = FindViewById<TextView> (Resource.Id.textView9);

			_radioDealer [0] = FindViewById<RadioButton> (Resource.Id.Dealer1);
			_radioDealer [1] = FindViewById<RadioButton> (Resource.Id.Dealer2);
			_radioDealer [2] = FindViewById<RadioButton> (Resource.Id.Dealer3);
			_radioDealer [3] = FindViewById<RadioButton> (Resource.Id.Dealer4);
			_radioDealer [4] = FindViewById<RadioButton> (Resource.Id.Dealer5);

			_spinner [0] = FindViewById<Spinner> (Resource.Id.spinner1);
			_spinner [1] = FindViewById<Spinner> (Resource.Id.spinner2);
			_spinner [2] = FindViewById<Spinner> (Resource.Id.spinner3);
			_spinner [3] = FindViewById<Spinner> (Resource.Id.spinner4);

			adapter = ArrayAdapter.CreateFromResource (this, Resource.Array.Type, Android.Resource.Layout.SimpleSpinnerItem);

			_start = FindViewById<Button> (Resource.Id.Start);
			_back = FindViewById<Button> (Resource.Id.back);

			_radioDealer [gp.Dealer].Checked = true;

			for (int i = 0; i < 5; ++i) {
				_pl [i].Text = gp.getPlayerName (i);
				_radioDealer [i].Enabled = gp.DealerEnabled;
				if (i < 4) {
					_addr [i] = gp.getPlayerAddress (i);
					_spinner [i].ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_Itemselected);
					_spinner [i].Adapter = adapter;
					_spinner [i].SetSelection (( _addr [i] == Resources.GetString (Resource.String.none_add) ) ? 0 : 1);
				}
			}


			_titCol1.LayoutParameters.Width = (int) ( widthInDp * _FIRST_FRACTION_WIDTH );
			_titCol2.LayoutParameters.Width = (int) ( widthInDp * _SECOND_FRACTION_WIDTH );
			_titCol3.LayoutParameters.Width = (int) ( widthInDp * _THIRD_FRACTION_WIDTH );
		
			_start.Click += Start_Game;
			_back.Click += Back;

			#endregion

			BTManager.Instance.setActivity (this);

			SetTitle (Resource.String.create_title);

			BTManager.Instance.eventLocalMessageReceived += handleLocalMessage;
			BTManager.Instance.eventPackageReceived += handlePackage;
			BTManager.Instance.RegisterReceivers ();
		}

		/// <summary>
		/// Gets the number of bluetooth-avaiable slots.
		/// </summary>
		/// <returns>The number of BT-avaiable slots.</returns>
		private int getBTNumber ()
		{
			int c = 0;
			for (int i = 0; i < 4; ++i)
				if (_spinner [i].SelectedItemPosition == 1)
					c++;
			return c;
		}

		/// <summary>
		/// Called when the selected item of the spinner changes.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void spinner_Itemselected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			//se scelgo AI
			if (e.Id == 0) {
				if (getBTNumber () - BTManager.Instance.getNumConnected () >= 0) {

					for (int i = 0; i < 4; ++i)
						if (sender == _spinner [i] && _addr [i] != Resources.GetText (Resource.String.none_add)) {
							BTManager.Instance.RemoveSlave (_addr [i]);
							_addr [i] = Resources.GetText (Resource.String.none_add);
							_pl [i + 1].Text = Resources.GetText (Resource.String.Default1);
							_pl [i + 1].InputType = Android.Text.InputTypes.TextVariationNormal;
							break;
						}

					//se scelgo BT
					if (getBTNumber () - BTManager.Instance.getNumConnected () <= 0)
						BTManager.Instance.StopListen ();
				}

			} else if (BTManager.Instance.isBTEnabled ())
				BTManager.Instance.ConnectAsMaster ();
			else
				BTManager.Instance.enableBluetooth ();
		}

		/// <summary>
		/// Starts the game.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Start_Game (object sender, EventArgs e)
		{
			if (getBTNumber () - BTManager.Instance.getNumConnected () == 0) {
				SetTitle (Resource.String.starting);

				BTManager.Instance.eventLocalMessageReceived -= handleLocalMessage;
				BTManager.Instance.eventPackageReceived -= handlePackage;

				string [] names = new string[5];
				for (int i = 0; i < 5; ++i)
					names [i] = _pl [i].Text;

				int dealer = 0;
				for (int i = 0; i < 5; ++i)
					if (_radioDealer [i].Checked) {
						dealer = i;
						break;
					}

				Intent inte = new Intent ();
				GameProfile gp = new GameProfile (names, _addr, dealer);

				gp.setIntent (inte);

				SetResult (Result.Ok, inte);
			
				BTManager.Instance.StopListen ();
				Finish ();
			} else
				Toast.MakeText (this, "Waiting for missing connection", ToastLength.Short).Show ();

		}

		/// <summary>
		/// Close the activity and returns to the main activity.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Back (object sender, EventArgs e)
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
		/// Called when the activity has detected the user's press of the back
		///  key.
		/// </summary>
		public override void OnBackPressed ()
		{
			base.OnBackPressed ();
			Back ();
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
					if (resultCode == Result.FirstUser)
						// Bluetooth is now enabled, so set up a chat session
						BTManager.Instance.ConnectAsMaster ();
					else if (resultCode == Result.Canceled) {
						BTManager.Instance.DisableBluetooth ();
						Finish ();
					}
					
				break;
					
			}

		}

		/// <summary>
		/// Handles the bluetooth messages recived (only name-change packages will be accepted).
		/// </summary>
		/// <param name="pkg">Package containing the message.</param>
		private void handlePackage (PackageBase pkg)
		{
			if (pkg == EnPackageType.NAME) {
				PackageName pkgn = (PackageName) pkg;

				for (int i = 0; i < 4; ++i)
					if (pkgn.address == _addr [i]) {
						Toast.MakeText (Application.Context, _pl [i + 1].Text + " changed his/her name to " + pkgn.name, ToastLength.Short).Show (); 
						_pl [i + 1].Text = pkgn.name;
						break;
					}
			}

		}

		/// <summary>
		/// Handles the local messages.
		/// </summary>
		/// <param name="msg">Message.</param>
		private void handleLocalMessage (Message msg)
		{

			switch (msg.What) {
				case (int) EnLocalMessageType.MESSAGE_DEVICE_ADDR:
					if (msg.Arg1 == (int) EnConnectionState.STATE_CONNECTED_MASTER) {

						_name = BTManager.Instance.getRemoteDevice ((string) msg.Obj).Name;
						Toast.MakeText (Application.Context, "Connected to " + _name, ToastLength.Short).Show ();
						if (_name.Length > MainActivity.MAX_NAME_LENGHT)
							_name = _name.Substring (0, MainActivity.MAX_NAME_LENGHT);

						for (int i = 0; i < 4; ++i)
							if (_spinner [i].SelectedItem.ToString () == (string) ( adapter.GetItem (1) ) && _addr [i] == Resources.GetText (Resource.String.none_add)) {
								_addr [i] = (string) msg.Obj;
								_pl [i + 1].Text = _name;
								_pl [i + 1].InputType = Android.Text.InputTypes.Null;
								break;
							} 
					}

				break;
				
				case (int) EnLocalMessageType.MESSAGE_CONNECTION_LOST:
					_address = (string) msg.Obj;
					BTManager.Instance.RemoveSlave (_address);

					for (int i = 0; i < 4; ++i)
						if (_address == _addr [i]) {
							_pl [i + 1].Text = this.Resources.GetText (Resource.String.Default1);
							_addr [i] = this.Resources.GetText (Resource.String.none_add);
							break;
						}

					Toast.MakeText (Application.Context, "Device connection lost", ToastLength.Short).Show ();
				break;
			
			}
			if (msg.What != (int) EnLocalMessageType.MESSAGE_STATE_CHANGE) {
				if (getBTNumber () - BTManager.Instance.getNumConnected () > 0)
					BTManager.Instance.ConnectAsMaster ();
				else
					BTManager.Instance.StopListen ();
			}
		}

	}
}