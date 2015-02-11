using System;

namespace GUILayout
{
	public class PointsInfo
	{
		private string _player;
		private int _played;
		private int _points;

		public string Player { get { return _player; } }

		public int Played { get { return _played; } }

		public int Points { get { return _points; } }

		public PointsInfo (string player, int played, int points)
		{
			_player = player;
			_played = played;
			_points = points;
		}
	}
}

