using System;
using Android.OS;
using CocosSharp;
using Android.Content.PM;
using Android.App;
using Android.Widget;
using Android.Content;
using ChiamataLibrary;
using Microsoft.Xna.Framework;
using BTLibrary;
using System.Collections.Generic;



namespace ProgettoInfo3
{
	[Activity (
		Label = "ProgettoInfo3",
		AlwaysRetainTaskState = true,
		Icon = "@drawable/icon",
		Theme = "@android:style/Theme.NoTitleBar",
		LaunchMode = LaunchMode.SingleTop,
		ScreenOrientation = ScreenOrientation.Portrait,
		MainLauncher = true,
		ConfigurationChanges = ConfigChanges.Keyboard |
		ConfigChanges.KeyboardHidden)
	]
	public class MainActivity : AndroidGameActivity
	{
		Button create;
		Button join;
		Button settings;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			create = FindViewById<Button> (Resource.Id.create);
			join = FindViewById<Button> (Resource.Id.select);

			settings = FindViewById<Button> (Resource.Id.settings);

			create.Click += (sender, e) => createClick (sender, e);
			join.Click += (sender, e) => joinClick (sender, e);
			settings.Click += (sender, e) => settingClick (sender, e);


			BTPlayService.Instance.Initialize (this);



		}

		void createClick (object sender, EventArgs e)
		{
			//Toast.MakeText (this, "Create new tab", ToastLength.Long).Show ();
			var serverIntent = new Intent (this, typeof (CraeteTabActivity));
			StartActivityForResult (serverIntent, 1);
		}

		void joinClick (object sender, EventArgs e)
		{
			var serverIntent = new Intent (this, typeof (JoinTableActivity));
			StartActivity (serverIntent);
		}

		void settingClick (object sender, EventArgs e)
		{
		

		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == 1 && resultCode == Result.Ok) {
				string [] name = data.GetStringArrayExtra ("Names");
				var application = new CCApplication ();
				application.ApplicationDelegate = new Core.GameAppDelegate ();
				SetContentView (application.AndroidContentView);

				ChiamataLibrary.Board.Instance.initializeMaster (name, 2);
				if (BTPlayService.Instance.getNumConnected () > 0)
					BTPlayService.Instance.WriteToAllSlave<Board> (Board.Instance);

				List<ArtificialIntelligence> AIs = new List<ArtificialIntelligence> (4);

				if (BTPlayService.Instance.isSlave ()) {

					for (int i = 0; i < Board.PLAYER_NUMBER; i++) {
						if (Board.Instance.Me.order != i) {
							BTPlayer bt = new BTPlayer (Board.Instance.getPlayer (i));
							BTPlayService.Instance.AddHandler (bt);
						}
					}

				} else {
					string [] type = data.GetStringArrayExtra ("types");

					for (int i = 1; i < Board.PLAYER_NUMBER; i++) {
						if (type [i] == "AI")
							AIs.Add (new ArtificialIntelligence (Board.Instance.getPlayer (i), new AIBMobileJump (10, 1, 2), new AISQuality (), new AICStupid ()));
						else if (type [i] == "BlueTooth") {
							BTPlayer bt = new BTPlayer (Board.Instance.getPlayer (i));
							BTPlayService.Instance.AddHandler (bt);
						}
					}
				}

				application.StartGame ();

			}




		}


	}
}


