
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
using Android.Content.PM;
using Microsoft.Xna.Framework;
using CocosSharp;
using ChiamataLibrary;
using BTLibrary;

namespace ProgettoInfo3
{
	[Activity (Label = "ProgettoInfo3",
		AlwaysRetainTaskState = true,
		Icon = "@drawable/icon",
		Theme = "@android:style/Theme.NoTitleBar",
		LaunchMode = LaunchMode.SingleTop,
		ScreenOrientation = ScreenOrientation.Portrait,
		MainLauncher = true,
		ConfigurationChanges = ConfigChanges.Keyboard |
		ConfigChanges.KeyboardHidden)]	

	public class startingActivity : AndroidGameActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			var serverIntent = new Intent (this, typeof (MainActivity));
			StartActivityForResult (serverIntent, 2);

		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == 2 && resultCode == Result.Ok) {

				var application = new CCApplication ();
				application.ApplicationDelegate = new Core.GameAppDelegate ();
				SetContentView (application.AndroidContentView);

				if (BTPlayService.Instance.isSlave ()) {

					for (int i = 0; i < Board.PLAYER_NUMBER; i++) {
						if (Board.Instance.Me.order != i) {
							BTPlayer bt = new BTPlayer (Board.Instance.getPlayer (i));
							BTPlayService.Instance.AddHandler (bt);
						}
					}

				} else {
					string [] name = data.GetStringArrayExtra ("Names");
					List<ArtificialIntelligence> AIs = new List<ArtificialIntelligence> (4);
					ChiamataLibrary.Board.Instance.initializeMaster (name, 2);
					if (BTPlayService.Instance.getNumConnected () > 0)
						BTPlayService.Instance.WriteToAllSlave<Board> (Board.Instance);

					string [] type = data.GetStringArrayExtra ("types");

					for (int i = 1; i < Board.PLAYER_NUMBER; i++) {
						if (type [i - 1] == "AI")
							AIs.Add (new ArtificialIntelligence (Board.Instance.getPlayer (i), new AIBMobileJump (10, 1, 2), new AISQuality (), new AICProva ()));
						else if (type [i - 1] == "BlueTooth") {
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

