using System;
using Android.OS;
using CocosSharp;
using Android.Content.PM;
using Microsoft.Xna.Framework;
using Android.App;
using Core;
using ChiamataLibrary;



namespace ProgettoInfo3
{
	[Activity(
		Label = "ProgettoInfo3",
		AlwaysRetainTaskState = true,
		Icon = "@drawable/icon",
		Theme = "@android:style/Theme.NoTitleBar",
		LaunchMode = LaunchMode.SingleInstance,
		ScreenOrientation = ScreenOrientation.Portrait,
		MainLauncher = true,
		ConfigurationChanges =  ConfigChanges.Keyboard | 
		ConfigChanges.KeyboardHidden)
	]
	public class MainActivity : AndroidGameActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate(bundle);

			#region Game setup
			//TODO : Passare un parametor alla gamescene per permettergli scegliere il mazziere
			Board.Instance.initializeMaster (new string[]{ "A", "B", "C", "D", "E" }, 2);//il mazziere è C
			#endregion

			#region IA setup
			IAIAuction iaa1 = new AIAuMobileJump (Board.Instance.AllPlayers [1], true, 10, 1, 1);
			IAIAuction iaa2 = new AIAuMobileJump (Board.Instance.AllPlayers [2], true, 10, 1, 1);
			IAIAuction iaa3 = new AIAuMobileJump (Board.Instance.AllPlayers [3], true, 10, 1, 1);
			IAIAuction iaa4 = new AIAuMobileJump (Board.Instance.AllPlayers [4], true, 10, 1, 1);

			AIPtStupid iap1 = new AIPtStupid (Board.Instance.AllPlayers [1]);
			AIPtStupid iap2 = new AIPtStupid (Board.Instance.AllPlayers [2]);
			AIPtStupid iap3 = new AIPtStupid (Board.Instance.AllPlayers [3]);
			AIPtStupid iap4 = new AIPtStupid (Board.Instance.AllPlayers [4]);
			#endregion

			starGame ();
		}

		protected void starGame()
		{
			var application = new CCApplication();


			application.ApplicationDelegate = new GameAppDelegate();
			SetContentView(application.AndroidContentView);
			application.StartGame();
		}
	}
}


