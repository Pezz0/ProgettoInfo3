
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

namespace MenuLayout
{
	[Activity (Label = "History", ScreenOrientation = ScreenOrientation.ReverseLandscape)]			
	public class History : TabActivity
	{
		Button back, delete;
		private const float TAB_WIDTH = 0.7f;
		TabHost tabH;
		TabWidget tabW;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			RequestWindowFeature (WindowFeatures.NoTitle);
			SetContentView (Resource.Layout.History);

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

		private void CreateTab (Type activityType, string tag, string label)
		{
			var intent = new Intent (this, activityType);
			intent.AddFlags (ActivityFlags.NewTask);
			var spec = TabHost.NewTabSpec (tag);
			spec.SetIndicator (label);
			spec.SetContent (intent);

			TabHost.AddTab (spec);
		}

		public void Back (object sender, EventArgs e)
		{
			Finish ();
		}

		public delegate void eventHandlerDelete ();

		public static event eventHandlerDelete eventDelete;

		public void Delete (object sender, EventArgs e)
		{
			Archive.Instance.delete (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal));
			if (eventDelete != null)
				eventDelete ();
		}

		public override void OnBackPressed ()
		{
			base.OnBackPressed ();
			Finish ();
		}
	}
}

