using System;
using Android.OS;
using Android.Content.PM;
using Android.App;
using Android.Widget;
using Android.Util;
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
		private const float _FRACTION_WIDTH = 0.5f, _FRACTION_HEIGH = 0.75f;

		internal const int MAX_NAME_LENGHT = 10;

		private Button _create, _join, _settings;

		private RelativeLayout _textLayout, _buttonLayout, _imageLayout;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			RequestWindowFeature (WindowFeatures.NoTitle);
			SetContentView (Resource.Layout.Main);
			Window.SetFlags (WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

			DisplayMetrics metrics = Resources.DisplayMetrics;
			int widthInDp = metrics.WidthPixels;
			int heightInDp = metrics.HeightPixels;

			_create = FindViewById<Button> (Resource.Id.create);
			_join = FindViewById<Button> (Resource.Id.select);
			_settings = FindViewById<Button> (Resource.Id.settings);
			_textLayout = FindViewById<RelativeLayout> (Resource.Id.TitleLayout);
			_buttonLayout = FindViewById<RelativeLayout> (Resource.Id.ButtonLatout);
			_imageLayout = FindViewById<RelativeLayout> (Resource.Id.ImageLayout);

			_imageLayout.LayoutParameters.Width = (int) ( widthInDp * _FRACTION_WIDTH );
			_buttonLayout.LayoutParameters.Width = (int) ( widthInDp * _FRACTION_WIDTH );

			_imageLayout.LayoutParameters.Height = (int) ( heightInDp * _FRACTION_HEIGH );
			_buttonLayout.LayoutParameters.Height = (int) ( heightInDp * _FRACTION_HEIGH );

			_textLayout.LayoutParameters.Height = heightInDp - _buttonLayout.LayoutParameters.Height;

			_create.Enabled = true;
			_join.Enabled = true;
			_settings.Enabled = true;

			_create.Click += createClick;
			_join.Click += joinClick;
			_settings.Click += settingClick;

			BTManager.Instance.Initialize (this);




		}

		//		private int ConvertPixelsToDp (float pixelValue)
		//		{
		//			var dp = (int) ( pixelValue / Resources.DisplayMetrics.Density );
		//			return dp;
		//		}

		void createClick (object sender, EventArgs e)
		{
			_create.Enabled = false;
			//Toast.MakeText (this, "Create new tab", ToastLength.Long).Show ();
			var serverIntent = new Intent (this, typeof (CraeteTabActivity));
			StartActivityForResult (serverIntent, 1);
		}

		void joinClick (object sender, EventArgs e)
		{
			_join.Enabled = false;
			var serverIntent = new Intent (this, typeof (JoinTableActivity));
			StartActivityForResult (serverIntent, 1);
		}

		void settingClick (object sender, EventArgs e)
		{
			_settings.Enabled = false;

		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			_create.Enabled = true;
			_join.Enabled = true;
			_settings.Enabled = true;

			if (requestCode == 1 && resultCode == Result.Ok) {

				Intent returnIntent = data;
				SetResult (Result.Ok, returnIntent);
				Finish ();

			}

		}


	}
}


