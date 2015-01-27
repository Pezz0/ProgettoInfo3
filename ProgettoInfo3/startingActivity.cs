
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
using System.Threading;

namespace MenuLayout
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

		private GameProfile _gameProfile;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			Archive.Instance.addFromFolder (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal));
			var serverIntent = new Intent (this, typeof (MainActivity));
			StartActivityForResult (serverIntent, 2);

		}

		private bool primo = true;
		private List<IPlayerController> _playerControllerList = new List<IPlayerController> (Board.PLAYER_NUMBER);

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			_playerControllerList.Clear ();

			if (requestCode == 2 && resultCode == Result.Ok) {

				if (BTManager.Instance.isSlave ()) {
				

				} else {

					_gameProfile = new GameProfile (data);

					ChiamataLibrary.Board.Instance.initializeMaster (_gameProfile.PlayerNames, _gameProfile.Dealer);

					BTManager.Instance.initializeComunication ();

					if (BTManager.Instance.getNumConnected () > 0)
						BTManager.Instance.WriteToAllSlave (new PackageBoard ());

					_playerControllerList = new List<IPlayerController> (Board.PLAYER_NUMBER);

					for (int i = 1; i < Board.PLAYER_NUMBER; i++) {
						if (_gameProfile.getPlayerAddress (i - 1) == Resources.GetString (Resource.String.none_add))
							_playerControllerList.Add (new AIPlayerController (Board.Instance.getPlayer (i), new AIBMobileJump (10, 1, 2), new AISQuality (), new AICProva ()));
						else {
							BTPlayerController bt = new BTPlayerController (i);
							_playerControllerList.Add (bt);
							Board.Instance.getPlayer (i).setController (bt);
						}
					}
				}
					

				if (primo) {
					var application = new CCApplication ();
					application.ApplicationDelegate = new Core.GameAppDelegate (_terminateMsg);

					SetContentView (application.AndroidContentView);

					application.StartGame ();
					primo = false;
				} else {
					lock (_terminateMsg) {
						Monitor.Pulse (_terminateMsg);
					}
				}

				if (BTManager.Instance.isSlave ())
					BTManager.Instance.eventPackageReceived += terminateHandle;
				else
					new Thread (finisher).Start ();

			}
		}

		private readonly TerminateMessage _terminateMsg = new TerminateMessage (0);

		private void finisher ()
		{
		
			lock (_terminateMsg) {
				Monitor.Wait (_terminateMsg);
			}
		
			Archive.Instance.saveLastGame (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal));
			BTManager.Instance.eventPackageReceived -= terminateHandle;
			switch (_terminateMsg.Signal) {
				case 0:
					Board.Instance.reset ();
					List<string> addresses = new List<string> (_gameProfile.PlayerAddress);
					if (addresses.Exists (pla => pla != Resources.GetText (Resource.String.none_add))) {
						BTManager.Instance.WriteToAllSlave (new PackageTerminate (_terminateMsg.Signal));
						var serverIntent = new Intent (this, typeof (MainActivity));
						StartActivityForResult (serverIntent, 2);

					} else {
						var serverIntent = new Intent (this, typeof (MainActivity));
						StartActivityForResult (serverIntent, 2);
					}
				break;
				case 1:
		
					//string [] address = new string[4];
					//for (int i = 0; i < 4; ++i)
						//address [i] = Resources.GetText (Resource.String.none_add);
					Intent inte = new Intent (this, typeof (CreateTabActivity));
					_gameProfile.nextGame ().setIntent (inte);

					Board.Instance.reset ();

					BTManager.Instance.WriteToAllSlave (new PackageTerminate (_terminateMsg.Signal));

					StartActivityForResult (inte, 2);

				break;
			}

		
		}


		private void terminateHandle (Package pkg)
		{
			if (pkg == EnPackageType.TERMINATE) {
				PackageTerminate pkgt = (PackageTerminate) pkg;
				if (pkgt.terminateSignal == 0) {
					Board.Instance.reset ();
					var serverIntent = new Intent (this, typeof (MainActivity));
					StartActivityForResult (serverIntent, 2);
				} else if (pkgt.terminateSignal == 1) {
					Board.Instance.reset ();
					var serverIntent = new Intent (this, typeof (JoinTableActivity));
					StartActivityForResult (serverIntent, 2);
				}

				BTManager.Instance.eventPackageReceived -= terminateHandle;
			}
		}
	}

	public class TerminateMessage
	{
		private int _signal;

		public int Signal { get { return _signal; } }

		public void setAbort ()
		{
			_signal = 0;
		}

		public void setRestart ()
		{
			_signal = 1;
		}

		public TerminateMessage (int signal)
		{
			this._signal = signal;
		}


	}
}

