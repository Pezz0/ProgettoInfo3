using System;
using System.Collections.Generic;

namespace ChiamataLibrary
{
	public class Archive
	{
		#region Singleton implementation

		private static readonly Archive _instance = new Archive ();

		public static Archive Instance{ get { return _instance; } }

		static Archive ()
		{
		}

		private Archive ()
		{

		}

		#endregion

		private List<GameData> _listGames = new List<GameData> ();

		public void forEach (Action<GameData> action)
		{
			_listGames.ForEach (action);
		}

		public void forEach (Action<GameData> action, DateTime from)
		{
			_listGames.FindAll (delegate(GameData gm) {
				return gm.time > from;
			}).ForEach (action);
		}

		public void add (GameData gm)
		{
			_listGames.Add (gm);
		}

		#region Info

		public List<string> getAllPlayer ()
		{
			List<string> lp = new List<string> ();

			_listGames.ForEach (delegate(GameData gm) {
				for (int i = 0; i < Board.PLAYER_NUMBER; i++)
					if (!lp.Contains (gm.getPlayer (i).name))
						lp.Add (gm.getPlayer (i).name);
			});
				
			return lp;
		}

		public int getTotalPoint (String player)
		{
			int tp = 0;
			_listGames.ForEach (delegate(GameData gm) {
				for (int i = 0; i < Board.PLAYER_NUMBER; i++)
					if (gm.getPlayer (i).name == player)
						tp = tp + gm.getAward (1);
			});
			return tp;
		}


		#endregion

		public void AddFromXML (string path)
		{
			_listGames.Add (new GameData (path));
		}


	}
}

