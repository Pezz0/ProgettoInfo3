
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

namespace MenuLayout
{
	[Activity (Label = "AllGames")]			
	public class AllGames : Activity
	{
		ArrayAdapter<string> play;

		GridView grid;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Grid);

			History.eventDelete += Delete;

			play = new ArrayAdapter<string> (this, Resource.Layout.Grid_elem);
			grid = FindViewById<GridView> (Resource.Id.gridView1);
			grid.Adapter = play;
			int i = 1;

			Archive.Instance.ForEach (delegate(GameData gd) {

				play.Add ("Game " + i + ":");
				play.Add ("");
				play.Add ("");

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
					play.Add (gd.GetChiamantePointCount ());
					play.Add ("");
					play.Add ("Other's points: ");
					play.Add (gd.GetAltriPointCount ());
					play.Add ("");

					Player caller = gd.GetChiamante ();

					Player socio = null;
					if (!gd.IsChiamataInMano)
						socio = gd.GetSocio ();

					for (int j = 0; j < Board.PLAYER_NUMBER; ++j) {
						Player player = gd.GetPlayer (j);
						play.Add (player.name);
						if (player.name == caller.name)
							play.Add ("CHIAMANTE");
						else if (!gd.IsChiamataInMano && player.name == socio.name)
							play.Add ("SOCIO");
						else
							play.Add ("ALTRO");
						int award = gd.GetAward (player);
						play.Add (award > 0 ? "+" + award.ToString () : award.ToString ());
					}
				} else {
					play.Add ("Game not Found");
					play.Add ("");
					play.Add ("");
				}
				play.Add ("");
				play.Add ("");
				play.Add ("");

				++i;

			});

			if (play.Count == 0)
				play.Add ("No matches found"); 
		}

		private void Delete ()
		{
			play.Clear ();
			play.Add ("No matches found");
		}
	}

}

