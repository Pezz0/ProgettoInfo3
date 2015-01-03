using System;
using Android.OS;
using CocosSharp;
using Android.Content.PM;
using Microsoft.Xna.Framework;
using Android.App;
using Android.Widget;

//using Core;
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

			//Toast.MakeText (this, "not implemented yet", ToastLength.Long).Show ();

		}


	}
}


