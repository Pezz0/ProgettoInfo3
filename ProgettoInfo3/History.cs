
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
		Button back;
		private const float TAB_WIDTH = 0.7f;
		TabHost tabH;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			RequestWindowFeature (WindowFeatures.NoTitle);
			SetContentView (Resource.Layout.History);

			DisplayMetrics metrics = Resources.DisplayMetrics;
			int widthInDp = metrics.WidthPixels;

			tabH = FindViewById<TabHost> (Android.Resource.Id.TabHost);
			tabH.LayoutParameters.Width = (int) ( widthInDp * TAB_WIDTH );

			back = FindViewById<Button> (Resource.Id.backHistory);
			back.Click += Back;
			CreateTab (typeof (LastGame), "last_game", "Last Game");
			CreateTab (typeof (AllGames), "all_games", "All Games");

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

		public override void OnBackPressed ()
		{
			base.OnBackPressed ();
			Finish ();
		}
	}
}

