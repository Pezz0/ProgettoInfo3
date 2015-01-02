using System;
using Android.OS;
using CocosSharp;
using Android.Content.PM;
using Microsoft.Xna.Framework;
using Android.App;
using Android.Widget;
using Core;
using Android.Content;



namespace ProgettoInfo3
{
	[Activity (
		Label = "ProgettoInfo3",
		AlwaysRetainTaskState = true,
		Icon = "@drawable/icon",
		Theme = "@android:style/Theme.NoTitleBar",
		LaunchMode = LaunchMode.SingleInstance,
		ScreenOrientation = ScreenOrientation.ReverseLandscape,
		MainLauncher = true,
		ConfigurationChanges = ConfigChanges.Keyboard |
		ConfigChanges.KeyboardHidden)
	]
	public class MainActivity : AndroidGameActivity
	{
		Android.Widget.Button create;
		Android.Widget.Button join;
		Android.Widget.Button settings;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			create = FindViewById<Android.Widget.Button> (Resource.Id.create);
			join = FindViewById<Android.Widget.Button> (Resource.Id.select);

			settings = FindViewById<Android.Widget.Button> (Resource.Id.settings);

			create.Click += (sender, e) => createClick (sender, e);
			join.Click += (sender, e) => joinClick (sender, e);
			settings.Click += (sender, e) => settingClick (sender, e);


		}

		void createClick (object sender, EventArgs e)
		{
			//Toast.MakeText (this, "Create new tab", ToastLength.Long).Show ();
			var serverIntent = new Intent (this, typeof (CraeteTabActivity));
			StartActivity (serverIntent);

		}

		void joinClick (object sender, EventArgs e)
		{
			var serverIntent = new Intent (this, typeof (JoinTableActivity));
			StartActivity (serverIntent);
		}

		void settingClick (object sender, EventArgs e)
		{
			starGame ();
			//Toast.MakeText (this, "not implemented yet", ToastLength.Long).Show ();

		}

		protected void starGame ()
		{
			var application = new CCApplication ();


			application.ApplicationDelegate = new GameAppDelegate ();
			SetContentView (application.AndroidContentView);
			application.StartGame ();
		}
	}
}


