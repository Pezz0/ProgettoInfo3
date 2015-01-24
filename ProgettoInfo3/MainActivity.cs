using System;
using Android.OS;
using Android.Content.PM;
using Android.App;
using Android.Widget;
using Android.Util;
using Android.Content;
using BTLibrary;
using Android.Views;
using ChiamataLibrary;

namespace MenuLayout
{
	[Activity (
		Label = "ProgettoInfo3",
		ScreenOrientation = ScreenOrientation.ReverseLandscape)
	]
	public class MainActivity : Activity
	{
		private const float _FRACTION_WIDTH = 0.5f, _FRACTION_HEIGH = 0.75f;

		internal const int MAX_NAME_LENGHT = 10;

		private Button _create, _join, _history;

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
			_history = FindViewById<Button> (Resource.Id.history);
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
			_history.Enabled = true;

			_create.Click += createClick;
			_join.Click += joinClick;
			_history.Click += settingClick;

			BTManager.Instance.Initialize (this);

		}

		void createClick (object sender, EventArgs e)
		{
			_create.Enabled = false;

			string [] names = new string[5];


			names [0] = BTManager.Instance.GetLocalName ();
			if (names [0].Length > MainActivity.MAX_NAME_LENGHT)
				names [0] = names [0].Substring (0, MainActivity.MAX_NAME_LENGHT);

			names [1] = Resources.GetText (Resource.String.Default1);
			names [2] = Resources.GetText (Resource.String.Default2);
			names [3] = Resources.GetText (Resource.String.Default3);
			names [4] = Resources.GetText (Resource.String.Default4);

			string [] address = new string[4];
			for (int i = 0; i < 4; ++i)
				address [i] = Resources.GetText (Resource.String.none_add);

			Intent inte = new Intent (this, typeof (CreateTabActivity));

			GameProfile gp = new GameProfile (names, address, 0);
			gp.setIntent (inte);

			StartActivityForResult (inte, 1);
		}

		void joinClick (object sender, EventArgs e)
		{
			_join.Enabled = false;
			var serverIntent = new Intent (this, typeof (JoinTableActivity));
			StartActivityForResult (serverIntent, 1);
		}

		void settingClick (object sender, EventArgs e)
		{
			var serverIntent = new Intent (this, typeof (History));
			StartActivity (serverIntent);

		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			_create.Enabled = true;
			_join.Enabled = true;
			_history.Enabled = true;

			if (requestCode == 1 && resultCode == Result.Ok) {

				Intent returnIntent = data;
				SetResult (Result.Ok, returnIntent);
				Finish ();

			}

		}


	}
}


