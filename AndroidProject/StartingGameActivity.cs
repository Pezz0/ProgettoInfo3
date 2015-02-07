
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
using AILibrary;

namespace GUILayout
{
	/// <summary>
	/// First activity to be created on the app launch.
	/// </summary>
	[Activity (Label = "Briscola Chiamata",
		AlwaysRetainTaskState = true,
		Icon = "@drawable/icon",
		Theme = "@android:style/Theme.NoTitleBar",
		LaunchMode = LaunchMode.SingleTop,
		ScreenOrientation = ScreenOrientation.Portrait,
		MainLauncher = true,
		ConfigurationChanges = ConfigChanges.Keyboard |
		ConfigChanges.KeyboardHidden)]	

	public class StartingGameActivity : AndroidGameActivity
	{
		/// <summary>
		/// Contains the data of the game.
		/// </summary>
		private GameProfile _gameProfile;

		/// <summary>
		/// Called on activity creation.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			BTManager.Instance.Initialize (this);
			var serverIntent = new Intent (this, typeof (MainActivity));
			serverIntent.PutExtra ("NewGame", 0);
			StartActivityForResult (serverIntent, 2);

		}

		/// <summary>
		/// Boolean value indicating if this is the first game since the app launch (used to skip some initializations).
		/// </summary>
		private bool primo = true;

		/// <summary>
		/// The player controllers list.
		/// </summary>
		private List<IPlayerController> _playerControllerList = new List<IPlayerController> (Board.PLAYER_NUMBER);

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
			_playerControllerList.Clear ();

			if (requestCode == 2 && resultCode == Result.Ok) {

				if (!BTManager.Instance.isSlave ()) {

					_gameProfile = new GameProfile (data);

					Board.Instance.InitializeMaster (_gameProfile.PlayerNames, _gameProfile.Dealer, new CriptoRandom ());

					BTManager.Instance.initializeCommunication ();

					if (BTManager.Instance.getNumConnected () > 0)
						BTManager.Instance.WriteToAllSlave (new PackageBoard ());

					_playerControllerList = new List<IPlayerController> (Board.PLAYER_NUMBER);

					for (int i = 1; i < Board.PLAYER_NUMBER; i++) {
						if (_gameProfile.getPlayerAddress (i - 1) == Resources.GetString (Resource.String.none_add))
							_playerControllerList.Add (new AIPlayerController ((Player) i, new AIBMobileJump (10, 1, 2), new AISQuality (), new AICProva ()));
						else {
							BTPlayerController bt = new BTPlayerController (i);
							_playerControllerList.Add (bt);
							( (Player) i ).SetController (bt);
						}
					}
				}


				if (primo) {
					var application = new CCApplication ();
					application.ApplicationDelegate = new GameAppDelegate (_terminateMsg);

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

		/// <summary>
		/// Method that waits the signal from the game scene to decide if a new game needs to be started [only master].
		/// </summary>
		private void finisher ()
		{

			lock (_terminateMsg) {
				Monitor.Wait (_terminateMsg);
			}

			BTManager.Instance.eventPackageReceived -= terminateHandle;
			List<string> addresses = new List<string> (_gameProfile.PlayerAddress);
			Board.Instance.Reset ();
			var serverIntent = new Intent (this, typeof (MainActivity));

			if (addresses.Exists (pla => pla != Resources.GetText (Resource.String.none_add))) {
				BTManager.Instance.WriteToAllSlave (new PackageTerminate (_terminateMsg.Signal));

			} 


			switch (_terminateMsg.Signal) {
				case 0:
					serverIntent.PutExtra ("NewGame", 0);
				break;
				case 1:
					serverIntent.PutExtra ("NewGame", 1);
					_gameProfile.nextGame ().setIntent (serverIntent);
				break;
			}

			StartActivityForResult (serverIntent, 2);


		}

		/// <summary>
		///  Handles the bluetooth messages recived (only terminate packages will be accepted) [only slaves].
		/// </summary>
		/// <param name="pkg">Package.</param>
		private void terminateHandle (PackageBase pkg)
		{
			if (pkg == EnPackageType.TERMINATE) {

				PackageTerminate pkgt = (PackageTerminate) pkg;
				Board.Instance.Reset ();
				Intent serverIntent = new Intent (this, typeof (MainActivity));
				if (pkgt.terminateSignal == 0) {

					serverIntent.PutExtra ("NewGame", 0);


				} else if (pkgt.terminateSignal == 1) {
					serverIntent.PutExtra ("NewGame", 1);
				}
				BTManager.Instance.eventPackageReceived -= terminateHandle;
				StartActivityForResult (serverIntent, 2);
			}
		}
	}

	/// <summary>
	/// Message used to signal the activity if a new game needs to be started after finishing the previous one.
	/// </summary>
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
