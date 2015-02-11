
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

namespace GUILayout
{
	[Activity (Label = "AllGames")]			
	internal class AllGames : Activity
	{
		private ArrayAdapter<string> _play;
		private ArrayAdapter _dateFilter;

		private GridView _grid;
		private Spinner _date;

		private const float SLIDER_WIDTH = 0.5f;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.GridAndFilters);

			HistoryActivity.eventDelete += Delete;

			DisplayMetrics metrics = Resources.DisplayMetrics;
			int widthInDp = metrics.WidthPixels;

			_play = new ArrayAdapter<string> (this, Resource.Layout.Grid_elem);
			_grid = FindViewById<GridView> (Resource.Id.gridView1);
			_date = FindViewById<Spinner> (Resource.Id.Spinner);
			_grid.Adapter = _play;
			_dateFilter = ArrayAdapter.CreateFromResource (this, Resource.Array.Date, Android.Resource.Layout.SimpleSpinnerItem);
			_date.Adapter = _dateFilter;
			_date.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_Itemselected);
			_date.LayoutParameters.Width = (int) ( widthInDp * SLIDER_WIDTH );


		}

		void spinner_Itemselected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			DateTime date = DateTime.Now;
			switch (e.Id) {
				case 0:
					LoadAll ();
				break;
				case 1:
					DateTime day = date.Subtract (new TimeSpan (1, 0, 0, 0));
					LoadPartial (day);
				break;
				case 2:
					DateTime week = date.Subtract (new TimeSpan (7, 0, 0, 0));
					LoadPartial (week);
				break;
				case 3:
					DateTime month = date.Subtract (new TimeSpan (30, 0, 0, 0));
					LoadPartial (month);
				break;
			}

		}

		private void LoadAll ()
		{
			_play.Clear ();
			int i = 1;
			Archive.Instance.ForEach (delegate(GameData gd) {
				AggiungiPartita (gd, i);
				++i;
			});

			if (_play.Count == 0)
				_play.Add ("No matches found"); 
		}

		private void LoadPartial (DateTime Time)
		{
			_play.Clear ();
			int i = 1;
			Archive.Instance.ForEach (delegate(GameData gd) {
				AggiungiPartita (gd, i);
				++i;
			}, Time);

			if (_play.Count == 0)
				_play.Add ("No matches found"); 
		}

		private void AggiungiPartita (GameData gd, int i)
		{
			_play.Add ("Game " + i + ":");
			_play.Add ("");
			_play.Add ("");

			if (gd != null) {
				_play.Add ("Date:");
				_play.Add (gd.time.Day.ToString () + "/" + gd.time.Month.ToString () + "/" + gd.time.Year.ToString ());
				_play.Add ("");
				_play.Add ("Time:");
				_play.Add (gd.time.Hour.ToString () + ":" + gd.time.Minute.ToString () + ":" + gd.time.Second.ToString ());
				_play.Add ("");
				_play.Add ("Type:");
				_play.Add (gd.gameType.ToString ());
				_play.Add ("");
				if (gd.gameType != EnGameType.CARICHI) {
					_play.Add ("Called Card:");
					_play.Add (gd.calledCard.number.ToString ());
					_play.Add (gd.calledCard.seme.ToString ());
				}
				_play.Add ("Caller Points: ");
				_play.Add (gd.GetChiamantePointCount ());
				_play.Add ("");
				_play.Add ("Other's points: ");
				_play.Add (gd.GetAltriPointCount ());
				_play.Add ("");

				Player caller = gd.GetChiamante ();

				Player socio = null;
				if (!gd.IsChiamataInMano)
					socio = gd.GetSocio ();

				for (int j = 0; j < Board.PLAYER_NUMBER; ++j) {
					Player player = gd.GetPlayer (j);
					_play.Add (player.name);
					if (player.name == caller.name)
						_play.Add ("CHIAMANTE");
					else if (!gd.IsChiamataInMano && player.name == socio.name)
						_play.Add ("SOCIO");
					else
						_play.Add ("ALTRO");
					int award = gd.GetAward (player);
					_play.Add (award > 0 ? "+" + award.ToString () : award.ToString ());
				}
			} else {
				_play.Add ("Game not Found");
				_play.Add ("");
				_play.Add ("");
			}
			_play.Add ("");
			_play.Add ("");
			_play.Add ("");
		}

		private void Delete ()
		{
			_play.Clear ();
			_play.Add ("No matches found");
		}
	}

}

