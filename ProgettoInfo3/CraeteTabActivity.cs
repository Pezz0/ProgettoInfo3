
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
using CocosSharp;

namespace ProgettoInfo3
{
	[Activity (Label = "CraeteTabActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.ReverseLandscape)]			
	public class CraeteTabActivity : Activity
	{
		Spinner spinner1;
		Spinner spinner2;
		Spinner spinner3;
		Spinner spinner4;
		EditText pl1;
		EditText pl2;
		EditText pl3;
		EditText pl4;
		EditText pl5;
		Button start;

		int counter = 0;

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

			pl2.Text = Resources.GetText (Resource.String.Default1);
			pl3.Text = Resources.GetText (Resource.String.Default2);
			pl4.Text = Resources.GetText (Resource.String.Default3);
			pl5.Text = Resources.GetText (Resource.String.Default4);

			start = FindViewById<Button> (Resource.Id.Start);
			start.Click += Start_Game;

			BTPlayService.Instance.Initialize (this, new BTHandler ());
		}

		void spinner_Itemselected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			//se scelgo AI
			if (e.Id == 0) {
				if (counter - BTPlayService.Instance.getNumConnected () > 0) {
					counter--;
					if (sender.ToString () == spinner1.ToString ()) {
						Toast.MakeText (this, "spinner1", ToastLength.Short).Show ();
					} else if (sender.ToString () == spinner2.ToString ()) {
						Toast.MakeText (this, "spinner2", ToastLength.Short).Show ();
					} else if (sender.ToString () == spinner3.ToString ()) {
						Toast.MakeText (this, "spinner3", ToastLength.Short).Show ();
					} else {
						Toast.MakeText (this, "spinner4", ToastLength.Short).Show ();
					}
				} else
					BTPlayService.Instance.StopListen ();
				//se scelgo BT
			} else {
				if (counter == 0) {
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


			var application = new CCApplication ();


			application.ApplicationDelegate = new Core.GameAppDelegate ();
			SetContentView (application.AndroidContentView);
			application.StartGame ();

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
			
	}
}

