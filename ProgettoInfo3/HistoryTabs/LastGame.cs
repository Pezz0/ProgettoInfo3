
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
	[Activity (Label = "LastGame")]			
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

			GameData gd = Archive.Instance.lastGame ();

			if (gd != null) {

				play.Add ("Played at: ");
				play.Add (gd.time.Day.ToString () + "/" + gd.time.Month.ToString () + "/" + gd.time.Year.ToString ());
				play.Add (gd.time.Hour.ToString () + ":" + gd.time.Minute.ToString () + ":" + gd.time.Second.ToString ());
				play.Add ("Called Card:");
				play.Add (gd.calledCard.number.ToString ());
				play.Add (gd.calledCard.seme.ToString ());
				play.Add ("Caller Points: ");
				play.Add (gd.getChiamantePointCount ());
				play.Add ("");
				play.Add ("Other's points: ");
				play.Add (gd.getAltriPointCount ());
				play.Add ("");

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
			
	}
}

