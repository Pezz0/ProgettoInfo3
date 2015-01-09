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


namespace ProgettoInfo3
{
	[Activity (Label = "CraeteTabActivity", ScreenOrientation = ScreenOrientation.ReverseLandscape)]			
	public class CraeteTabActivity : Activity
	{
		private static Spinner spinner1, spinner2, spinner3, spinner4;

		private static EditText pl0, pl1, pl2, pl3, pl4;

		private static TextView add1, add2, add3, add4;

		private Button start, back;

		static int counter;

		public const int MAX_NAME_LENGHT = 10;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.CreateTable);
		
			Window.SetFlags (WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

			spinner1 = FindViewById<Spinner> (Resource.Id.spinner1);
			spinner1.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_Itemselected);
			var adapter = ArrayAdapter.CreateFromResource (this, Resource.Array.Type, Android.Resource.Layout.SimpleSpinnerItem);
			spinner1.Adapter = adapter;

			spinner2 = FindViewById<Spinner> (Resource.Id.spinner2);
			spinner2.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_Itemselected);
			spinner2.Adapter = adapter;

			spinner3 = FindViewById<Spinner> (Resource.Id.spinner3);
			spinner3.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_Itemselected);
			spinner3.Adapter = adapter;

			spinner4 = FindViewById<Spinner> (Resource.Id.spinner4);
			spinner4.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_Itemselected);
			spinner4.Adapter = adapter;

			pl0 = FindViewById<EditText> (Resource.Id.player1);
			pl1 = FindViewById<EditText> (Resource.Id.player2);
			pl2 = FindViewById<EditText> (Resource.Id.player3);
			pl3 = FindViewById<EditText> (Resource.Id.player4);
			pl4 = FindViewById<EditText> (Resource.Id.player5);


			add1 = FindViewById<TextView> (Resource.Id.addr2);
			add2 = FindViewById<TextView> (Resource.Id.addr3);
			add3 = FindViewById<TextView> (Resource.Id.addr4);
			add4 = FindViewById<TextView> (Resource.Id.addr5);

			start = FindViewById<Button> (Resource.Id.Start);
			back = FindViewById<Button> (Resource.Id.back);


			pl1.Text = Resources.GetText (Resource.String.Default1);
			pl2.Text = Resources.GetText (Resource.String.Default2);
			pl3.Text = Resources.GetText (Resource.String.Default3);
			pl4.Text = Resources.GetText (Resource.String.Default4);

			add1.Text = Resources.GetText (Resource.String.none_add);
			add2.Text = Resources.GetText (Resource.String.none_add);
			add3.Text = Resources.GetText (Resource.String.none_add);
			add4.Text = Resources.GetText (Resource.String.none_add);

			add1.Visibility = ViewStates.Invisible;
			add2.Visibility = ViewStates.Invisible;
			add3.Visibility = ViewStates.Invisible;
			add4.Visibility = ViewStates.Invisible;
		
			start.Click += Start_Game;
			back.Click += Back;

			counter = 4;

			BTPlayService.Instance.AddHandler (new MyHandler (this));
			BTPlayService.Instance.setActivity (this);

			SetTitle (Resource.String.create_title);

			string name = BTPlayService.Instance.GetLocalName ();
			if (name.Length > MAX_NAME_LENGHT)
				pl0.Text = name.Substring (0, MAX_NAME_LENGHT);
			else
				pl0.Text = name;
		}

		void spinner_Itemselected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			//se scelgo AI
			if (e.Id == 0) {
				if (counter - BTPlayService.Instance.getNumConnected () >= 0) {
					counter--;
					if (sender.ToString () == spinner1.ToString () && add1.Text != Resources.GetText (Resource.String.none_add)) {
						BTPlayService.Instance.RemoveSlave (add1.Text);
						add1.Text = Resources.GetText (Resource.String.none_add);
						pl1.Text = Resources.GetText (Resource.String.Default1);
						pl1.InputType = Android.Text.InputTypes.TextVariationNormal;

					} else if (sender.ToString () == spinner2.ToString () && add2.Text != Resources.GetText (Resource.String.none_add)) {
						BTPlayService.Instance.RemoveSlave (add2.Text);
						add2.Text = Resources.GetText (Resource.String.none_add);
						pl2.Text = Resources.GetText (Resource.String.Default2);
						pl2.InputType = Android.Text.InputTypes.TextVariationNormal;

					} else if (sender.ToString () == spinner3.ToString () && add3.Text != Resources.GetText (Resource.String.none_add)) {
						BTPlayService.Instance.RemoveSlave (add3.Text);
						add3.Text = Resources.GetText (Resource.String.none_add);
						pl3.Text = Resources.GetText (Resource.String.Default3);
						pl3.InputType = Android.Text.InputTypes.TextVariationNormal;

					} else if (sender.ToString () == spinner4.ToString () && add4.Text != Resources.GetText (Resource.String.none_add)) {
						BTPlayService.Instance.RemoveSlave (add4.Text);
						add4.Text = Resources.GetText (Resource.String.none_add);
						pl4.Text = Resources.GetText (Resource.String.Default4);
						pl4.InputType = Android.Text.InputTypes.TextVariationNormal;
					}
					if (counter - BTPlayService.Instance.getNumConnected () <= 0)
						BTPlayService.Instance.StopListen ();
				}
				//se scelgo BT
			} else {
				if (counter - BTPlayService.Instance.getNumConnected () == 0) {
					if (BTPlayService.Instance.isBTEnabled ())
						BTPlayService.Instance.ConnectAsMaster ();
					else
						BTPlayService.Instance.enableBluetooth ();
				}
				counter++;



			}

		}

		void Start_Game (object sender, EventArgs e)
		{
			if (counter - BTPlayService.Instance.getNumConnected () == 0) {
				SetTitle (Resource.String.starting);

				Intent returnIntent = new Intent ();

				returnIntent.PutExtra ("Names", new string[5] {
					pl0.Text,
					pl1.Text,
					pl2.Text,
					pl3.Text,
					pl4.Text
				});

				returnIntent.PutExtra ("types", new string[4] {
					spinner1.SelectedItem.ToString (),
					spinner2.SelectedItem.ToString (),
					spinner3.SelectedItem.ToString (),
					spinner4.SelectedItem.ToString ()
				});

				SetResult (Result.Ok, returnIntent);
				//FIXME roba da mettere il BTPlayer
				BTPlayService.Instance.ResetHandler ();
				BTPlayService.Instance.StopListen ();
				Finish ();
			} else
				Toast.MakeText (this, "Waiting for missing connection", ToastLength.Short).Show ();

		}

		void Back (object sender, EventArgs e)
		{
			Intent returnIntent = new Intent ();
			SetResult (Result.Canceled, returnIntent);
			BTPlayService.Instance.Stop ();
			BTPlayService.Instance.ResetHandler ();
			Finish ();
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			switch (requestCode) {

				case (int)ActivityResultCode.REQUEST_ENABLE_BT:
					// When the request to enable Bluetooth returns
					if (resultCode == Result.Ok)
						// Bluetooth is now enabled, so set up a chat session
						BTPlayService.Instance.ConnectAsMaster ();
					else
						Finish ();
					
				break;
					
			}
		}


		private class MyHandler:Handler
		{
			string address = "";
			string name = "";
			private static Context _context;

			public MyHandler (Context context)
			{
				_context = context;
			}

			public override void HandleMessage (Message msg)
			{
				switch (msg.What) {
					case (int) MessageType.MESSAGE_DEVICE_ADDR:
						name = BTPlayService.Instance.getRemoteDevice ((string) msg.Obj).Name;
						if (name.Length > MAX_NAME_LENGHT)
							name = name.Substring (0, MAX_NAME_LENGHT);
						if (spinner1.SelectedItem.ToString ().CompareTo ("BlueTooth") == 0 && add1.Text.CompareTo ("None") == 0) {
							add1.Text = (string) msg.Obj;
							pl1.Text = name;
							pl1.InputType = Android.Text.InputTypes.Null;
						} else if (spinner2.SelectedItem.ToString ().CompareTo ("BlueTooth") == 0 && add2.Text.CompareTo ("None") == 0) {
							add2.Text = (string) msg.Obj;
							pl2.Text = name;
							pl2.InputType = Android.Text.InputTypes.Null;
						} else if (spinner3.SelectedItem.ToString ().CompareTo ("BlueTooth") == 0 && add3.Text.CompareTo ("None") == 0) {
							add3.Text = (string) msg.Obj;
							name = BTPlayService.Instance.getRemoteDevice ((string) msg.Obj).Name;
							pl3.Text = name;
							pl3.InputType = Android.Text.InputTypes.Null;
						} else if (spinner4.SelectedItem.ToString ().CompareTo ("BlueTooth") == 0 && add4.Text.CompareTo ("None") == 0) {
							add4.Text = (string) msg.Obj;
							name = BTPlayService.Instance.getRemoteDevice ((string) msg.Obj).Name;
							pl4.Text = name;
							pl4.InputType = Android.Text.InputTypes.Null;
						} 

					break;

					case (int)MessageType.MESSAGE_READ:
						string Player = Encoding.ASCII.GetString ((byte []) msg.Obj);
						if (Player.Length > MAX_NAME_LENGHT)
							Player = Player.Substring (0, MAX_NAME_LENGHT);
						if (address.CompareTo (add1.Text) == 0) {
							Toast.MakeText (Application.Context, pl1.Text + " changed his/her name to " + Player, ToastLength.Short).Show (); 
							pl1.Text = Player;
						}
						if (address.CompareTo (add2.Text) == 0) {
							Toast.MakeText (Application.Context, pl2.Text + " changed his/her name to " + Player, ToastLength.Short).Show (); 
							pl2.Text = Player;
						}
						if (address.CompareTo (add3.Text) == 0) {
							Toast.MakeText (Application.Context, pl3.Text + " changed his/her name to " + Player, ToastLength.Short).Show (); 
							pl3.Text = Player;
						}
						if (address.CompareTo (add4.Text) == 0) {
							Toast.MakeText (Application.Context, pl4.Text + " changed his/her name to " + Player, ToastLength.Short).Show (); 
							pl4.Text = Player;
						}
						address = "";
					break;

					case (int) MessageType.MESSAGE_DEVICE_READ:
						address = (string) msg.Obj;
					break;
					case (int) MessageType.MESSAGE_TOAST:
						Toast.MakeText (Application.Context, (string) msg.Obj, ToastLength.Short).Show ();
					break;
			
					case (int) MessageType.MESSAGE_STATE_CHANGE:
						if (msg.Arg1 == (int) ConnectionState.STATE_CONNECTED_MASTER)
							Toast.MakeText (Application.Context, "Connected to " + name, ToastLength.Short).Show ();

					break;
					case (int) MessageType.MESSAGE_CONNECTION_LOST:
						address = (string) msg.Obj;
						BTPlayService.Instance.RemoveSlave (address);
						if (address.CompareTo (add1.Text) == 0) {
							pl1.Text = _context.Resources.GetText (Resource.String.Default1);
							add1.Text = _context.Resources.GetText (Resource.String.none_add);
						}
						if (address.CompareTo (add2.Text) == 0) {
							pl2.Text = _context.Resources.GetText (Resource.String.Default2);
							add2.Text = _context.Resources.GetText (Resource.String.none_add);

						}
						if (address.CompareTo (add3.Text) == 0) {
							pl3.Text = _context.Resources.GetText (Resource.String.Default3);
							add3.Text = _context.Resources.GetText (Resource.String.none_add);


						}
						if (address.CompareTo (add4.Text) == 0) {
							pl4.Text = _context.Resources.GetText (Resource.String.Default4);
							add4.Text = _context.Resources.GetText (Resource.String.none_add);


						}
					break;
				}
				if (msg.What != (int) MessageType.MESSAGE_STATE_CHANGE) {
					if (counter - BTPlayService.Instance.getNumConnected () > 0)
						BTPlayService.Instance.ConnectAsMaster ();
					else
						BTPlayService.Instance.StopListen ();
				}
			}
		}
			
	}
}

