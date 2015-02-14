using System;

namespace GUILayout
{
	public class PointsInfo
	{
		public readonly string player;
		public readonly int played;
		public readonly int points;

		public PointsInfo (string player, int played, int points)
		{
			this.player = player;
			this.played = played;
			this.points = points;
		}
	}
}

