
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
using ChiamataLibrary;
using Android.Content.PM;
using Android.Util;

namespace GUILayout
{
	/// <summary>
	/// History of stats and past games.
	/// </summary>
	[Activity (Label = "History", ScreenOrientation = ScreenOrientation.ReverseLandscape)]			
	public class HistoryActivity : TabActivity
	{
		/// <summary>
		/// Buttons to exit the activity and to delete the game data.
		/// </summary>
		private Button back, delete;

		/// <summary>
		/// Tab width.
		/// </summary>
		private const float TAB_WIDTH = 0.7f;

		/// <summary>
		/// The tab host.
		/// </summary>
		private TabHost tabH;

		/// <summary>
		/// The widgets in tab host.
		/// </summary>
		private TabWidget tabW;

		/// <summary>
		/// Called on activity creation.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			RequestWindowFeature (WindowFeatures.NoTitle);
			SetContentView (Resource.Layout.History);
			Window.SetFlags (WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

			DisplayMetrics metrics = Resources.DisplayMetrics;
			int widthInDp = metrics.WidthPixels;

			tabH = FindViewById<TabHost> (Android.Resource.Id.TabHost);
			tabH.LayoutParameters.Width = (int) ( widthInDp * TAB_WIDTH );

			tabW = FindViewById<TabWidget> (Android.Resource.Id.Tabs);

			back = FindViewById<Button> (Resource.Id.backHistory);
			back.Click += Back;

			delete = FindViewById<Button> (Resource.Id.Delete);
			delete.Click += Delete;

			CreateTab (typeof (LastGame), "last_game", "Last Game");
			CreateTab (typeof (AllGames), "all_games", "All Games");
			CreateTab (typeof (Points), "points", "Points");

		}

		/// <summary>
		/// Helper metod that creates a new tab.
		/// </summary>
		/// <param name="activityType">Activity type.</param>
		/// <param name="tag">Tag of the tab.</param>
		/// <param name="label">Title of the tab.</param>
		private void CreateTab (Type activityType, string tag, string label)
		{
			var intent = new Intent (this, activityType);
			intent.AddFlags (ActivityFlags.NewTask);
			var spec = TabHost.NewTabSpec (tag);
			spec.SetIndicator (label);
			spec.SetContent (intent);

			TabHost.AddTab (spec);
		}

		/// <summary>
		/// Closes the activity.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Back (object sender, EventArgs e)
		{
			Finish ();
		}

		/// <summary>
		/// Delegate for the delete event.
		/// </summary>
		public delegate void eventHandlerDelete ();

		/// <summary>
		/// Delete event.
		/// </summary>
		public static event eventHandlerDelete eventDelete;

		/// <summary>
		/// Deletes the currently selected game from the archive.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Delete (object sender, EventArgs e)
		{
			Archive.Instance.DeleteAll ();
			if (eventDelete != null)
				eventDelete ();
		}

		/// <summary>
		/// Called when the activity has detected the user's press of the back
		///  key.
		/// </summary>
		public override void OnBackPressed ()
		{
			base.OnBackPressed ();
			Finish ();
		}
	}
}

