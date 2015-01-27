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
	/// <summary>
	/// Title screen activity.
	/// </summary>
	[Activity (
		Label = "ProgettoInfo3",
		ScreenOrientation = ScreenOrientation.ReverseLandscape)
	]
	public class MainActivity : Activity
	{
		/// <summary>
		/// Width and height of the columns.
		/// </summary>
		private const float _FRACTION_WIDTH = 0.5f, _FRACTION_HEIGH = 0.75f;

		/// <summary>
		/// Maximum number of character accepted for the name.
		/// </summary>
		internal const int MAX_NAME_LENGHT = 10;

		/// <summary>
		/// Buttons to create or join a table and to view history of past games.
		/// </summary>
		private Button _create, _join, _history;


		private RelativeLayout _textLayout, _buttonLayout, _imageLayout;

		/// <summary>
		/// Called on activity creation.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
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
			_history.Click += historyClick;

			BTManager.Instance.Initialize (this);

		}

		/// <summary>
		/// Starts the create table activity.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
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

		/// <summary>
		/// Starts the join table activity.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void joinClick (object sender, EventArgs e)
		{
			_join.Enabled = false;
			var serverIntent = new Intent (this, typeof (JoinTableActivity));
			StartActivityForResult (serverIntent, 1);
		}

		/// <summary>
		/// Starts the history activity.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void historyClick (object sender, EventArgs e)
		{
			var serverIntent = new Intent (this, typeof (History));
			StartActivity (serverIntent);

		}

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


