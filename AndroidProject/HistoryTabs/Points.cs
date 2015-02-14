
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
	[Activity (Label = "Points")]			
	internal class Points : Activity
	{
		ArrayAdapter<string> _play;
		private ArrayAdapter _dateFilter;
		private TextView _label;
		private GridView _grid;
		private Spinner _date;
		private const float SLIDER_WIDTH = 0.5f;
		private List<string> _players;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.GridAndFilters);

			DisplayMetrics metrics = Resources.DisplayMetrics;
			int widthInDp = metrics.WidthPixels;

			_play = new ArrayAdapter<string> (this, Resource.Layout.Grid_elem);
			_grid = FindViewById<GridView> (Resource.Id.gridView1);
			_grid.Adapter = _play;
			_label = FindViewById<TextView> (Resource.Id.label);
			_label.Text = "Sort: ";
			_date = FindViewById<Spinner> (Resource.Id.Spinner);
			_dateFilter = ArrayAdapter.CreateFromResource (this, Resource.Array.Sort, Android.Resource.Layout.SimpleSpinnerItem);
			_date.Adapter = _dateFilter;
			_date.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_Itemselected);
			_date.LayoutParameters.Width = (int) ( widthInDp * SLIDER_WIDTH );

			HistoryActivity.eventDelete += Delete;

			_players = Archive.Instance.GetAllPlayer ();

		}

		void spinner_Itemselected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			_play.Clear ();
			if (_players.Count > 0) {
				_play.Add ("Player");
				_play.Add ("Played");
				_play.Add ("Points");

				//string [][] values = new string[players.Count] [];

				PointsInfo [] values = new PointsInfo[_players.Count];

				int i = 0;

				foreach (string name in _players) {
					values [i] = new PointsInfo (name, Archive.Instance.GetPlayed (name), Archive.Instance.GetTotalAward (name));
					++i;
				}
			
				switch (e.Id) {
					case 0:
						SortByPointDescend (values, _players.Count);
					break;
					case 1:
						SortByPointAscend (values, _players.Count);
					break;
					case 2:
						SortByPlayedDescend (values, _players.Count);
					break;
					case 3:
						SortByPlayedAscend (values, _players.Count);
					break;
					case 4:
						SortByNameDescend (values, 0, _players.Count - 1);
					break;
					case 5:
						SortByNameAscend (values, 0, _players.Count - 1);
					break;
				}
				for (int j = 0; j < _players.Count; j++) {
					PointsInfo val = values [j];
					_play.Add (val.player);
					_play.Add (val.played.ToString ());
					_play.Add (( val.points > 0 ? "+" : "" ) + val.points.ToString ()); 
				}
			} else
				_play.Add ("No match found");

		}

		private void SortByPointDescend (PointsInfo [] data, int size)
		{
			int max = data [0].points;
			int min = data [0].points;

			for (int i = 1; i < size; ++i) {
				if (data [i].points < min)
					min = data [i].points;
				if (data [i].points > max)
					max = data [i].points;
			}

			Queue<PointsInfo> [] point = new Queue<PointsInfo>[( max - min ) + 1];
			for (int i = 0; i < ( max - min ) + 1; ++i)
				point [i] = new Queue<PointsInfo> ();

			for (int i = 0; i < size; ++i)
				point [max - data [i].points].Enqueue (data [i]);

			int j = 0;
			for (int i = 0; i < ( max - min ) + 1; ++i)
				while (point [i].Count > 0) {
					data [j] = point [i].Dequeue ();
					j++;
				}
		}

		private void SortByPointAscend (PointsInfo [] data, int size)
		{
			int max = data [0].points;
			int min = data [0].points;

			for (int i = 1; i < size; ++i) {
				if (data [i].points < min)
					min = data [i].points;
				if (data [i].points > max)
					max = data [i].points;
			}

			Queue<PointsInfo> [] point = new Queue<PointsInfo>[( max - min ) + 1];
			for (int i = 0; i < ( max - min ) + 1; ++i)
				point [i] = new Queue<PointsInfo> ();

			for (int i = 0; i < size; ++i)
				point [data [i].points - min].Enqueue (data [i]);

			int j = 0;
			for (int i = 0; i < ( max - min ) + 1; ++i)
				while (point [i].Count > 0) {
					data [j] = point [i].Dequeue ();
					j++;
				}
		}

		private void SortByPlayedDescend (PointsInfo [] data, int size)
		{
			int max = data [0].played;
			int min = data [0].played;

			for (int i = 1; i < size; ++i) {
				if (data [i].played < min)
					min = data [i].played;
				if (data [i].played > max)
					max = data [i].played;
			}

			Queue<PointsInfo> [] played = new Queue<PointsInfo>[( max - min ) + 1];
			for (int i = 0; i < ( max - min ) + 1; ++i)
				played [i] = new Queue<PointsInfo> ();

			for (int i = 0; i < size; ++i)
				played [max - data [i].played].Enqueue (data [i]);

			int j = 0;
			for (int i = 0; i < ( max - min ) + 1; ++i)
				while (played [i].Count > 0) {
					data [j] = played [i].Dequeue ();
					j++;
				}
		}

		private void SortByPlayedAscend (PointsInfo [] data, int size)
		{
			int max = data [0].played;
			int min = data [0].played;

			for (int i = 1; i < size; ++i) {
				if (data [i].played < min)
					min = data [i].played;
				if (data [i].played > max)
					max = data [i].played;
			}

			Queue<PointsInfo> [] played = new Queue<PointsInfo>[( max - min ) + 1];
			for (int i = 0; i < ( max - min ) + 1; ++i)
				played [i] = new Queue<PointsInfo> ();

			for (int i = 0; i < size; ++i)
				played [data [i].played - min].Enqueue (data [i]);

			int j = 0;
			for (int i = 0; i < ( max - min ) + 1; ++i)
				while (played [i].Count > 0) {
					data [j] = played [i].Dequeue ();
					j++;
				}
		}

		private void SortByNameAscend (PointsInfo [] data, int left, int right)
		{
			int i = left, j = right;
			PointsInfo pivotInfo = data [new Random ().Next (left, right)];
			string pivotName = pivotInfo.player;

			while (i <= j) {
				while (data [i].player.CompareTo (pivotName) < 0) {
					i++;
				}

				while (data [j].player.CompareTo (pivotName) > 0) {
					j--;
				}

				if (i <= j) {
					// Swap
					PointsInfo tmp = data [i];
					data [i] = data [j];
					data [j] = tmp;

					i++;
					j--;
				}
			}

			// Recursive calls
			if (left < j) {
				SortByNameAscend (data, left, j);
			}

			if (i < right) {
				SortByNameAscend (data, i, right);
			}
		}

		private void SortByNameDescend (PointsInfo [] data, int left, int right)
		{
			int i = left, j = right;
			PointsInfo pivotInfo = data [new Random ().Next (left, right)];
			string pivotName = pivotInfo.player;

			while (i <= j) {
				while (data [i].player.CompareTo (pivotName) > 0) {
					i++;
				}

				while (data [j].player.CompareTo (pivotName) < 0) {
					j--;
				}

				if (i <= j) {
					// Swap
					PointsInfo tmp = data [i];
					data [i] = data [j];
					data [j] = tmp;

					i++;
					j--;
				}
			}

			// Recursive calls
			if (left < j) {
				SortByNameDescend (data, left, j);
			}

			if (i < right) {
				SortByNameDescend (data, i, right);
			}
		}

		private void Delete ()
		{
			_play.Clear ();
			_play.Add ("No match found");
		}
	}
}

