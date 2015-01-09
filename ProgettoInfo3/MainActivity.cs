using System;
using Android.OS;
using Android.Content.PM;
using Android.App;
using Android.Widget;
using Android.Content;
using BTLibrary;
using Android.Views;



namespace ProgettoInfo3
{
	[Activity (
		Label = "ProgettoInfo3",
		ScreenOrientation = ScreenOrientation.ReverseLandscape)
	]
	public class MainActivity : Activity
	{
		Button create, join, settings;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			RequestWindowFeature (WindowFeatures.NoTitle);
			SetContentView (Resource.Layout.Main);
			Window.SetFlags (WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

			create = FindViewById<Button> (Resource.Id.create);
			join = FindViewById<Button> (Resource.Id.select);
			settings = FindViewById<Button> (Resource.Id.settings);

			create.Enabled = true;
			join.Enabled = true;
			settings.Enabled = true;

			create.Click += createClick;
			join.Click += joinClick;
			settings.Click += settingClick;

			BTPlayService.Instance.Initialize (this);



		}

		void createClick (object sender, EventArgs e)
		{
			create.Enabled = false;
			//Toast.MakeText (this, "Create new tab", ToastLength.Long).Show ();
			var serverIntent = new Intent (this, typeof (CraeteTabActivity));
			StartActivityForResult (serverIntent, 1);
		}

		void joinClick (object sender, EventArgs e)
		{
			join.Enabled = false;
			var serverIntent = new Intent (this, typeof (JoinTableActivity));
			StartActivityForResult (serverIntent, 1);
		}

		void settingClick (object sender, EventArgs e)
		{
			settings.Enabled = false;

		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			create.Enabled = true;
			join.Enabled = true;
			settings.Enabled = true;

			if (requestCode == 1 && resultCode == Result.Ok) {

				Intent returnIntent = data;
				SetResult (Result.Ok, returnIntent);
				Finish ();

			}

		}


	}
}


