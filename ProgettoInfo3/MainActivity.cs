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

//using System.Collections.Generic;
using Android.Views;



namespace ProgettoInfo3
{
	[Activity (
		Label = "ProgettoInfo3",
		Theme = "@android:style/Theme.NoTitleBar",
		ScreenOrientation = ScreenOrientation.ReverseLandscape)
	]
	public class MainActivity : Activity
	{
		Button create;
		Button join;
		Button settings;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);
			Window.SetFlags (WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

			create = FindViewById<Button> (Resource.Id.create);
			join = FindViewById<Button> (Resource.Id.select);

			settings = FindViewById<Button> (Resource.Id.settings);

			create.Click += (sender, e) => createClick (sender, e);
			join.Click += (sender, e) => joinClick (sender, e);
			settings.Click += (sender, e) => settingClick (sender, e);


			BTPlayService.Instance.Initialize (this);



		}

		private int ConvertPixelsToDp (float pixelValue)
		{
			var dp = (int) ( ( pixelValue ) / Resources.DisplayMetrics.Density );
			return dp;
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
			StartActivityForResult (serverIntent, 1);
		}

		void settingClick (object sender, EventArgs e)
		{
		

		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{

			if (requestCode == 1 && resultCode == Result.Ok) {

				Intent returnIntent = data;
				SetResult (Result.Ok, returnIntent);
				Finish ();

			}

		}


	}
}


