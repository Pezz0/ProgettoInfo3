
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

namespace MenuLayout
{
	[Activity (Label = "History", ScreenOrientation = ScreenOrientation.ReverseLandscape)]			
	public class History : Activity
	{
		ArrayAdapter<string> play;

		GridView grid;

		Button back;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.History);

			play = new ArrayAdapter<string> (this, Resource.Layout.History_elem);

			grid = FindViewById<GridView> (Resource.Id.gridView1);
			grid.Adapter = play;

			back = FindViewById<Button> (Resource.Id.backHist);

			back.Click += Back;

			GameData gd = Archive.Instance.lastGame ();

			if (gd != null) {
				Player caller = gd.getChiamante ();

				Player socio = gd.getSocio ();

				for (int i = 0; i < Board.PLAYER_NUMBER; ++i) {
					Player player = gd.getPlayer (i);
					play.Add (player.name);
					if (player.name == caller.name)
						play.Add ("CHIAMANTE");
					else if (player.name == socio.name)
						play.Add ("SOCIO");
					else
						play.Add ("ALTRO");
					play.Add (gd.getAward (i));
				}
			} else
				play.Add ("No match found");
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

