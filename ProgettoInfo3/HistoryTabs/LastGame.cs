
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
using Android.Util;
using Android.Content.PM;

namespace MenuLayout
{
	[Activity (Label = "LastGame", ScreenOrientation = ScreenOrientation.ReverseLandscape)]			
	public class LastGame : Activity
	{
		ArrayAdapter<string> play;

		GridView grid;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			// Create your application here

			SetContentView (Resource.Layout.Grid);

			play = new ArrayAdapter<string> (this, Resource.Layout.Grid_elem);

			grid = FindViewById<GridView> (Resource.Id.gridView1);
			grid.Adapter = play;

			History.eventDelete += Delete;

			GameData gd = Archive.Instance.lastGame ();

			if (gd != null) {

				play.Add ("Date:");
				play.Add (gd.time.Day.ToString () + "/" + gd.time.Month.ToString () + "/" + gd.time.Year.ToString ());
				play.Add ("");
				play.Add ("Time:");
				play.Add (gd.time.Hour.ToString () + ":" + gd.time.Minute.ToString () + ":" + gd.time.Second.ToString ());
				play.Add ("");
				play.Add ("Type:");
				play.Add (gd.gameType.ToString ());
				play.Add ("");
				if (gd.gameType != EnGameType.CARICHI) {
					play.Add ("Called Card:");
					play.Add (gd.calledCard.number.ToString ());
					play.Add (gd.calledCard.seme.ToString ());
				}
				play.Add ("Caller Points: ");
				play.Add (gd.getChiamantePointCount ());
				play.Add ("");
				play.Add ("Other's points: ");
				play.Add (gd.getAltriPointCount ());
				play.Add ("");

				Player caller = gd.getChiamante ();
				Player socio = null;
				if (!gd.isChiamataInMano)
					socio = gd.getSocio ();
				for (int i = 0; i < Board.PLAYER_NUMBER; ++i) {
					Player player = gd.getPlayer (i);
					play.Add (player.name);
					if (player.name == caller.name)
						play.Add ("CHIAMANTE");
					else if (!gd.isChiamataInMano && player.name == socio.name)
						play.Add ("SOCIO");
					else
						play.Add ("ALTRO");
					int award = gd.getAward (player);
					play.Add (award > 0 ? "+" + award.ToString () : award.ToString ());
				}
			} else
				play.Add ("No match found");
		}

		private void Delete ()
		{
			play.Clear ();
			play.Add ("No match found");
		}
			
	}
}

