
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
	[Activity (Label = "Points")]			
	public class Points : Activity
	{
		ArrayAdapter<string> play;

		GridView grid;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Grid);

			play = new ArrayAdapter<string> (this, Resource.Layout.Grid_elem);

			grid = FindViewById<GridView> (Resource.Id.gridView1);
			grid.Adapter = play;

			History.eventDelete += Delete;

			List<string> players = Archive.Instance.getAllPlayer ();

			if (players.Count > 0) {
				play.Add ("Player");
				play.Add ("Played");
				play.Add ("Points");

				string [][] values = new string[players.Count] [];

				for (int h = 0; h < players.Count; h++)
					values [h] = new string[3];

				int i = 0;
				players.ForEach (name => {
					values [i] [0] = name;
					values [i] [1] = Archive.Instance.getPlayed (name).ToString ();
					values [i] [2] = Archive.Instance.getTotalPoint (name).ToString ();
					i++;
					/*play.Add (name);
				play.Add (Archive.Instance.getPlayed (name).ToString ());
				int points = Archive.Instance.getTotalPoint (name);
				play.Add (points > 0 ? "+" + points : points.ToString ());*/
				});
				sort (values, 0, players.Count - 1);
				for (int j = 0; j < players.Count; j++) {
					for (int k = 0; k < 3; k++) {
						play.Add (values [j] [k]);
					}
				}
			} else
				play.Add ("No match found");
		}

		public void sort (string [][] matrix, int left, int right)
		{
			int i = left, j = right;
			int pivot = Convert.ToInt32 (matrix [( left + right ) / 2] [2]);

			while (i <= j) {
				while (Convert.ToInt32 (matrix [i] [2]) > pivot) {
					i++;
				}

				while (Convert.ToInt32 (matrix [j] [2]) < pivot) {
					j--;
				}

				if (i <= j) {
					// Swap
					string [] tmp = matrix [i];
					matrix [i] = matrix [j];
					matrix [j] = tmp;

					i++;
					j--;
				}
			}

			// Recursive calls
			if (left < j) {
				sort (matrix, left, j);
			}

			if (i < right) {
				sort (matrix, i, right);
			}
		}


		public void Delete ()
		{
			play.Clear ();
			play.Add ("No match found");
		}
	}
}

