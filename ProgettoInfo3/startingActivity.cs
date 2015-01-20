
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

		private readonly List<IPlayerController> _PlayerControllerList = new List<IPlayerController> (Board.PLAYER_NUMBER - 1);

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			_PlayerControllerList.Clear ();

			if (requestCode == 2 && resultCode == Result.Ok) {

				if (BTPlayService.Instance.isSlave ()) {

					byte [] board = data.GetByteArrayExtra ("Board");

					List<byte> bs = new List<byte> ();

					for (int i = 1; i < board.GetLength (0); i++)
						bs.Add (board [i]);

					Board.Instance.recreateFromByteArray (bs.ToArray ());

					char [] Me = data.GetCharArrayExtra ("Name");

					Board.Instance.initializeSlave (new string (Me));

					BTManager.Instance.initialize ();
				
					for (int i = 0; i < Board.PLAYER_NUMBER; i++)
						if (Board.Instance.Me.order != i)
							_PlayerControllerList.Add (new BTPlayer (Board.Instance.getPlayer (i)));

				} else {
					string [] name = data.GetStringArrayExtra ("Names");

					ChiamataLibrary.Board.Instance.initializeMaster (name, data.GetIntExtra ("Dealer", 0));

					if (BTPlayService.Instance.getNumConnected () > 0)
						BTPlayService.Instance.WriteToAllSlave (EnContentType.BOARD, Board.Instance.toByteArray ());

					BTManager.Instance.initialize ();

					string [] type = data.GetStringArrayExtra ("types");

					for (int i = 1; i < Board.PLAYER_NUMBER; i++) {
						if (type [i - 1] == "AI")
							_PlayerControllerList.Add (new ArtificialIntelligence (Board.Instance.getPlayer (i), new AIBMobileJump (10, 1, 2), new AISQuality (), new AICProva ()));
						else if (type [i - 1] == "BlueTooth") {
							_PlayerControllerList.Add (new BTPlayer (Board.Instance.getPlayer (i)));
						
						}
					}
				}
					
				var application = new CCApplication ();
				application.ApplicationDelegate = new Core.GameAppDelegate ();
				SetContentView (application.AndroidContentView);
				application.StartGame ();

			}
		}
	}
}

