
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
	[Activity (Label = "CraeteTabActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.ReverseLandscape)]			
	public class CraeteTabActivity : Activity
	{
		static Spinner spinner1;
		static Spinner spinner2;
		static Spinner spinner3;
		static Spinner spinner4;

		EditText pl1;
		EditText pl2;
		EditText pl3;
		EditText pl4;
		EditText pl5;


		static TextView add1;
		static TextView add2;
		static TextView add3;
		static TextView add4;

		Button start;
		Button back;

		static int counter = 0;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.CreateTable);
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

			pl1 = FindViewById<EditText> (Resource.Id.player1);
			pl2 = FindViewById<EditText> (Resource.Id.player2);
			pl3 = FindViewById<EditText> (Resource.Id.player3);
			pl4 = FindViewById<EditText> (Resource.Id.player4);
			pl5 = FindViewById<EditText> (Resource.Id.player5);


			add1 = FindViewById<TextView> (Resource.Id.addr2);
			add2 = FindViewById<TextView> (Resource.Id.addr3);
			add3 = FindViewById<TextView> (Resource.Id.addr4);
			add4 = FindViewById<TextView> (Resource.Id.addr5);

			start = FindViewById<Button> (Resource.Id.Start);
			back = FindViewById<Button> (Resource.Id.back);

			pl2.Text = Resources.GetText (Resource.String.Default1);
			pl3.Text = Resources.GetText (Resource.String.Default2);
			pl4.Text = Resources.GetText (Resource.String.Default3);
			pl5.Text = Resources.GetText (Resource.String.Default4);


			add1.Text = Resources.GetText (Resource.String.none_add);
			add2.Text = Resources.GetText (Resource.String.none_add);
			add3.Text = Resources.GetText (Resource.String.none_add);
			add4.Text = Resources.GetText (Resource.String.none_add);
		
			start.Click += Start_Game;
			back.Click += Back;

			BTPlayService.Instance.Initialize (this, new MyHandler ());
		}

		void spinner_Itemselected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			//se scelgo AI
			if (e.Id == 0) {
				if (counter - BTPlayService.Instance.getNumConnected () > 0) {
					counter--;
					if (sender.ToString () == spinner1.ToString () && add1.Text != Resources.GetText (Resource.String.none_add)) {
						BTPlayService.Instance.RemoveSlave (add1.Text);
					} else if (sender.ToString () == spinner2.ToString () && add2.Text != Resources.GetText (Resource.String.none_add)) {
						BTPlayService.Instance.RemoveSlave (add2.Text);
					} else if (sender.ToString () == spinner3.ToString () && add3.Text != Resources.GetText (Resource.String.none_add)) {
						BTPlayService.Instance.RemoveSlave (add3.Text);
					} else {
						BTPlayService.Instance.RemoveSlave (add4.Text);
					}
				} else
					BTPlayService.Instance.StopListen ();
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
				Intent returnIntent = new Intent ();
				SetResult (Result.Ok, returnIntent);
				BTPlayService.Instance.setHandler (new BTHandler ());
				BTPlayService.Instance.StopListen ();

				Finish ();
			} else
				Toast.MakeText (this, "Waiting for missing connection", ToastLength.Short).Show ();

		}

		void Back (object sender, EventArgs e)
		{
			Intent returnIntent = new Intent ();
			SetResult (Result.Canceled, returnIntent);

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
			public MyHandler ()
			{
			}

			public override void HandleMessage (Message msg)
			{

				if (msg.What == (int) MessageType.MESSAGE_DEVICE_ADDR) {
						
					if (spinner1.SelectedItem.ToString ().CompareTo ("BlueTooth") == 0 && add1.Text.CompareTo ("None") == 0)
						add1.Text = (string) msg.Obj;
					else if (spinner2.SelectedItem.ToString ().CompareTo ("BlueTooth") == 0 && add2.Text.CompareTo ("None") == 0)
						add2.Text = (string) msg.Obj;
					else if (spinner3.SelectedItem.ToString ().CompareTo ("BlueTooth") == 0 && add3.Text.CompareTo ("None") == 0)
						add3.Text = (string) msg.Obj;
					else if (spinner4.SelectedItem.ToString ().CompareTo ("BlueTooth") == 0 && add4.Text.CompareTo ("None") == 0)
						add4.Text = (string) msg.Obj;

				}

				if (counter - BTPlayService.Instance.getNumConnected () <= 0)
					BTPlayService.Instance.StopListen ();


			}
		}
			
	}
}

