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


namespace ProgettoInfo3
{
	[Activity (Label = "CraeteTabActivity", ScreenOrientation = ScreenOrientation.ReverseLandscape)]			
	public class CraeteTabActivity : Activity
	{
		private Spinner _spinner1, _spinner2, _spinner3, _spinner4;

		private EditText _pl0, _pl1, _pl2, _pl3, _pl4;

		private TextView _add1, _add2, _add3, _add4, _titCol1, _titCol2, _titCol3;

		private Button _start, _back;

		private RadioButton _dealer0, _dealer1, _dealer2, _dealer3, _dealer4;

		private ArrayAdapter adapter;

		private int _counter, _dealer;

		private string _name = "", _address = "";

		private float _FIRST_FRACTION_WIDTH = 0.15f, _SECOND_FRACTION_WIDTH = 0.2f, _THIRD_FRACTION_WIDTH = 0.3f;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.CreateTable);
		
			Window.SetFlags (WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

			DisplayMetrics metrics = Resources.DisplayMetrics;
			int widthInDp = metrics.WidthPixels;

			#region Display Management

			_spinner1 = FindViewById<Spinner> (Resource.Id.spinner1);
			_spinner1.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_Itemselected);
			adapter = ArrayAdapter.CreateFromResource (this, Resource.Array.Type, Android.Resource.Layout.SimpleSpinnerItem);
			_spinner1.Adapter = adapter;

			_spinner2 = FindViewById<Spinner> (Resource.Id.spinner2);
			_spinner2.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_Itemselected);
			_spinner2.Adapter = adapter;

			_spinner3 = FindViewById<Spinner> (Resource.Id.spinner3);
			_spinner3.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_Itemselected);
			_spinner3.Adapter = adapter;

			_spinner4 = FindViewById<Spinner> (Resource.Id.spinner4);
			_spinner4.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_Itemselected);
			_spinner4.Adapter = adapter;

			_pl0 = FindViewById<EditText> (Resource.Id.player1);
			_pl1 = FindViewById<EditText> (Resource.Id.player2);
			_pl2 = FindViewById<EditText> (Resource.Id.player3);
			_pl3 = FindViewById<EditText> (Resource.Id.player4);
			_pl4 = FindViewById<EditText> (Resource.Id.player5);

			_add1 = FindViewById<TextView> (Resource.Id.addr2);
			_add2 = FindViewById<TextView> (Resource.Id.addr3);
			_add3 = FindViewById<TextView> (Resource.Id.addr4);
			_add4 = FindViewById<TextView> (Resource.Id.addr5);
			_titCol1 = FindViewById<TextView> (Resource.Id.textView1);
			_titCol2 = FindViewById<TextView> (Resource.Id.textView7);
			_titCol3 = FindViewById<TextView> (Resource.Id.textView9);

			_dealer0 = FindViewById<RadioButton> (Resource.Id.Dealer1);
			_dealer1 = FindViewById<RadioButton> (Resource.Id.Dealer2);
			_dealer2 = FindViewById<RadioButton> (Resource.Id.Dealer3);
			_dealer3 = FindViewById<RadioButton> (Resource.Id.Dealer4);
			_dealer4 = FindViewById<RadioButton> (Resource.Id.Dealer5);

			_start = FindViewById<Button> (Resource.Id.Start);
			_back = FindViewById<Button> (Resource.Id.back);

			_pl1.Text = Resources.GetText (Resource.String.Default1);
			_pl2.Text = Resources.GetText (Resource.String.Default2);
			_pl3.Text = Resources.GetText (Resource.String.Default3);
			_pl4.Text = Resources.GetText (Resource.String.Default4);

			_add1.Text = Resources.GetText (Resource.String.none_add);
			_add2.Text = Resources.GetText (Resource.String.none_add);
			_add3.Text = Resources.GetText (Resource.String.none_add);
			_add4.Text = Resources.GetText (Resource.String.none_add);

			_add1.Visibility = ViewStates.Invisible;
			_add2.Visibility = ViewStates.Invisible;
			_add3.Visibility = ViewStates.Invisible;
			_add4.Visibility = ViewStates.Invisible;

			_titCol1.LayoutParameters.Width = (int) ( widthInDp * _FIRST_FRACTION_WIDTH );
			_titCol2.LayoutParameters.Width = (int) ( widthInDp * _SECOND_FRACTION_WIDTH );
			_titCol3.LayoutParameters.Width = (int) ( widthInDp * _THIRD_FRACTION_WIDTH );
		
			_start.Click += Start_Game;
			_back.Click += Back;

			_dealer0.Click += RadioClick;
			_dealer1.Click += RadioClick;
			_dealer2.Click += RadioClick;
			_dealer3.Click += RadioClick;
			_dealer4.Click += RadioClick;

			#endregion

			_counter = 4;
			_dealer = 0;
		
			BTManager.Instance.setActivity (this);

			SetTitle (Resource.String.create_title);

			string name = BTManager.Instance.GetLocalName ();
			if (name.Length > MainActivity.MAX_NAME_LENGHT)
				_pl0.Text = name.Substring (0, MainActivity.MAX_NAME_LENGHT);
			else
				_pl0.Text = name;

			BTManager.Instance.eventLocalMessageReceived += handleLocalMessage;
			BTManager.Instance.eventPackageReceived += handlePackage;
		}

		void spinner_Itemselected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			//se scelgo AI
			if (e.Id == 0) {
				if (_counter - BTManager.Instance.getNumConnected () >= 0) {
					_counter--;
					if (sender.ToString () == _spinner1.ToString () && _add1.Text != Resources.GetText (Resource.String.none_add)) {
						BTManager.Instance.RemoveSlave (_add1.Text);
						_add1.Text = Resources.GetText (Resource.String.none_add);
						_pl1.Text = Resources.GetText (Resource.String.Default1);
						_pl1.InputType = Android.Text.InputTypes.TextVariationNormal;

					} else if (sender.ToString () == _spinner2.ToString () && _add2.Text != Resources.GetText (Resource.String.none_add)) {
						BTManager.Instance.RemoveSlave (_add2.Text);
						_add2.Text = Resources.GetText (Resource.String.none_add);
						_pl2.Text = Resources.GetText (Resource.String.Default2);
						_pl2.InputType = Android.Text.InputTypes.TextVariationNormal;

					} else if (sender.ToString () == _spinner3.ToString () && _add3.Text != Resources.GetText (Resource.String.none_add)) {
						BTManager.Instance.RemoveSlave (_add3.Text);
						_add3.Text = Resources.GetText (Resource.String.none_add);
						_pl3.Text = Resources.GetText (Resource.String.Default3);
						_pl3.InputType = Android.Text.InputTypes.TextVariationNormal;

					} else if (sender.ToString () == _spinner4.ToString () && _add4.Text != Resources.GetText (Resource.String.none_add)) {
						BTManager.Instance.RemoveSlave (_add4.Text);
						_add4.Text = Resources.GetText (Resource.String.none_add);
						_pl4.Text = Resources.GetText (Resource.String.Default4);
						_pl4.InputType = Android.Text.InputTypes.TextVariationNormal;
					}
					if (_counter - BTManager.Instance.getNumConnected () <= 0)
						BTManager.Instance.StopListen ();
				}
				//se scelgo BT
			} else {
				if (_counter - BTManager.Instance.getNumConnected () == 0) {
					if (BTManager.Instance.isBTEnabled ())
						BTManager.Instance.ConnectAsMaster ();
					else
						BTManager.Instance.enableBluetooth ();
				}
				_counter++;
			}
		}

		void RadioClick (object sender, EventArgs e)
		{
			if (sender.ToString () == _dealer0.ToString ())
				_dealer = 0;
			if (sender.ToString () == _dealer1.ToString ())
				_dealer = 1;
			if (sender.ToString () == _dealer2.ToString ())
				_dealer = 2;
			if (sender.ToString () == _dealer3.ToString ())
				_dealer = 3;
			if (sender.ToString () == _dealer4.ToString ())
				_dealer = 4;
		}

		void Start_Game (object sender, EventArgs e)
		{
			if (_counter - BTManager.Instance.getNumConnected () == 0) {
				SetTitle (Resource.String.starting);

				BTManager.Instance.eventLocalMessageReceived += handleLocalMessage;
				BTManager.Instance.eventPackageReceived += handlePackage;

				Intent returnIntent = new Intent ();

				returnIntent.PutExtra ("Names", new string[5] {
					_pl0.Text,
					_pl1.Text,
					_pl2.Text,
					_pl3.Text,
					_pl4.Text
				});

				returnIntent.PutExtra ("types", new string[4] {
					_spinner1.SelectedItem.ToString (),
					_spinner2.SelectedItem.ToString (),
					_spinner3.SelectedItem.ToString (),
					_spinner4.SelectedItem.ToString ()
				});

				returnIntent.PutExtra ("Dealer", _dealer);

				SetResult (Result.Ok, returnIntent);
			
				BTManager.Instance.StopListen ();
				Finish ();
			} else
				Toast.MakeText (this, "Waiting for missing connection", ToastLength.Short).Show ();

		}

		void Back (object sender, EventArgs e)
		{
			Intent returnIntent = new Intent ();
			SetResult (Result.Canceled, returnIntent);
			BTManager.Instance.Stop ();
			Finish ();
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			switch (requestCode) {

				case (int)EnActivityResultCode.REQUEST_ENABLE_BT:
					// When the request to enable Bluetooth returns
					if (resultCode == Result.Ok)
						// Bluetooth is now enabled, so set up a chat session
						BTManager.Instance.ConnectAsMaster ();
					else
						Finish ();
					
				break;
					
			}

		}

		private void handlePackage (Package pkg)
		{
			if (pkg == EnPackageType.NAME) {
				PackageName pkgn = (PackageName) pkg;

				if (pkgn.address == _add1.Text) {
					Toast.MakeText (Application.Context, _pl1.Text + " changed his/her name to " + pkgn.name, ToastLength.Short).Show (); 
					_pl1.Text = pkgn.name;
				} else if (pkgn.address == _add2.Text) {
					Toast.MakeText (Application.Context, _pl2.Text + " changed his/her name to " + pkgn.name, ToastLength.Short).Show (); 
					_pl2.Text = pkgn.name;
				} else if (pkgn.address == _add3.Text) {
					Toast.MakeText (Application.Context, _pl3.Text + " changed his/her name to " + pkgn.name, ToastLength.Short).Show (); 
					_pl3.Text = pkgn.name;
				} else if (pkgn.address == _add4.Text) {
					Toast.MakeText (Application.Context, _pl4.Text + " changed his/her name to " + pkgn.name, ToastLength.Short).Show (); 
					_pl4.Text = pkgn.name;
				}
			}

		}


		private void handleLocalMessage (Message msg)
		{

			switch (msg.What) {
				case (int) EnLocalMessageType.MESSAGE_DEVICE_ADDR:
					if (msg.Arg1 == (int) EnConnectionState.STATE_CONNECTED_MASTER) {

						_name = BTManager.Instance.getRemoteDevice ((string) msg.Obj).Name;
						Toast.MakeText (Application.Context, "Connected to " + _name, ToastLength.Short).Show ();
						if (_name.Length > MainActivity.MAX_NAME_LENGHT)
							_name = _name.Substring (0, MainActivity.MAX_NAME_LENGHT);
						if (_spinner1.SelectedItem.ToString ().CompareTo ((string) ( adapter.GetItem (1) )) == 0 && _add1.Text.CompareTo (Resources.GetText (Resource.String.none_add)) == 0) {
							_add1.Text = (string) msg.Obj;
							_pl1.Text = _name;
							_pl1.InputType = Android.Text.InputTypes.Null;
						} else if (_spinner2.SelectedItem.ToString ().CompareTo ((string) ( adapter.GetItem (1) )) == 0 && _add2.Text.CompareTo (Resources.GetText (Resource.String.none_add)) == 0) {
							_add2.Text = (string) msg.Obj;
							_pl2.Text = _name;
							_pl2.InputType = Android.Text.InputTypes.Null;
						} else if (_spinner3.SelectedItem.ToString ().CompareTo ((string) ( adapter.GetItem (1) )) == 0 && _add3.Text.CompareTo (Resources.GetText (Resource.String.none_add)) == 0) {
							_add3.Text = (string) msg.Obj;
							_pl3.Text = _name;
							_pl3.InputType = Android.Text.InputTypes.Null;
						} else if (_spinner4.SelectedItem.ToString ().CompareTo ((string) ( adapter.GetItem (1) )) == 0 && _add4.Text.CompareTo (Resources.GetText (Resource.String.none_add)) == 0) {
							_add4.Text = (string) msg.Obj;
							_pl4.Text = _name;
							_pl4.InputType = Android.Text.InputTypes.Null;
						} 
					}

				break;
				
				case (int) EnLocalMessageType.MESSAGE_CONNECTION_LOST:
					_address = (string) msg.Obj;
					BTManager.Instance.RemoveSlave (_address);
					if (_address == _add1.Text) {
						_pl1.Text = this.Resources.GetText (Resource.String.Default1);
						_add1.Text = this.Resources.GetText (Resource.String.none_add);
					} else if (_address == _add2.Text) {
						_pl2.Text = this.Resources.GetText (Resource.String.Default2);
						_add2.Text = this.Resources.GetText (Resource.String.none_add);

					} else if (_address == _add3.Text) {
						_pl3.Text = this.Resources.GetText (Resource.String.Default3);
						_add3.Text = this.Resources.GetText (Resource.String.none_add);


					} else if (_address == _add4.Text) {
						_pl4.Text = this.Resources.GetText (Resource.String.Default4);
						_add4.Text = this.Resources.GetText (Resource.String.none_add);
					}
					Toast.MakeText (Application.Context, "Device connection lost", ToastLength.Short).Show ();
				break;
			}
			if (msg.What != (int) EnLocalMessageType.MESSAGE_STATE_CHANGE) {
				if (_counter - BTManager.Instance.getNumConnected () > 0)
					BTManager.Instance.ConnectAsMaster ();
				else
					BTManager.Instance.StopListen ();
			}
		}

	}
}