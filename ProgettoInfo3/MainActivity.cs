using System;
using Android.OS;
using CocosSharp;
using Android.Content.PM;
using Microsoft.Xna.Framework;
using Android.App;
using Core;



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

			var application = new CCApplication();


			application.ApplicationDelegate = new GameAppDelegate();
			SetContentView(application.AndroidContentView);
			application.StartGame();
		}
	}
}


