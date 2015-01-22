using System;
using System.Collections.Generic;
using System.IO;

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
			_listGames.FindAll (gm => gm.time > from).ForEach (action);
		}

		public void add (GameData gm)
		{
			_listGames.Add (gm);
		}

		public void addFromFolder (String path, String fileName)
		{
			int i = 0;
			String completePath = Path.Combine (path, fileName + i.ToString () + ".xml");

			while (File.Exists (completePath)) {
				add (new GameData (completePath));
				++i;
				completePath = Path.Combine (path, fileName + i.ToString () + ".xml");
			}

//			for (i = 0; File.Exists (completePath); completePath = Path.Combine (path, fileName + ( ++i ).ToString () + ".xml"))
//				add (new GameData (completePath));

		}

		public void saveInFolder (String path, String fileName)
		{
			int i = 0;
			_listGames.ForEach (delegate(GameData gd) {
				gd.writeOnXML (Path.Combine (path, fileName + i.ToString () + ".xml"));
				i++;
			});


			//_listGames.ForEach (gd => gd.writeOnXML (Path.Combine (path, fileName + ( i++ ).ToString () + ".xml")));
		}

		public GameData lastGame ()
		{
			return _listGames [_listGames.Count - 1];
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

