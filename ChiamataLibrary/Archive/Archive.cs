using System;
using System.Collections.Generic;
using System.IO;

namespace ChiamataLibrary
{
	/// <summary>
	/// Contains all the data about past games and offers methods to save or load those data from XML files.
	/// </summary>
	public class Archive
	{
		#region Singleton implementation

		/// <summary>
		/// Instance of the <see cref="ChiamataLibrary.Archive"/> singleton
		/// </summary>
		private static readonly Archive _instance = new Archive ();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance of the <see cref="ChiamataLibrary.Archive"/>.</value>
		public static Archive Instance{ get { return _instance; } }

		static Archive ()
		{
		}

		private Archive ()
		{

		}

		#endregion

		/// <summary>
		/// The list of previous games saved in the archive.
		/// </summary>
		private readonly List<GameData> _listGames = new List<GameData> ();

		/// <summary>
		/// Applies the method passed as an argument to each one of the games saved in the archive.
		/// </summary>
		/// <param name="action">The method that must be execuded for each game. Must be a delegate of <see cref="Action<ChiamataLibrary.GameData>"/>.</param>
		public void forEach (Action<GameData> action)
		{
			_listGames.ForEach (action);
		}

		/// <summary>
		/// Applies the method passed as an argument to each game saved after the date provided as the second argument.
		/// </summary>
		/// <param name="action">The method that must be execuded ofr each game. Must be a delegate of <see cref="Action<ChiamataLibrary.GameData>"/>.</param>
		/// <param name="from">The date before which the games will be ignored.</param>
		public void forEach (Action<GameData> action, DateTime from)
		{
			_listGames.FindAll (gm => gm.time > from).ForEach (action);
		}

		/// <summary>
		/// Add the specified <see cref="ChiamataLibrary.GameData"/> to the archive.
		/// </summary>
		/// <param name="gm">The instance of <see cref="ChiamataLibrary.GameData"/> to be added.</param>
		public void add (GameData gm)
		{
			_listGames.Add (gm);
		}

		/// <summary>
		/// The index of the last game saved.
		/// </summary>
		private int _lastGameCount;
		/// <summary>
		/// Constant string representing the base file name for oll the XML files that will be saved.
		/// </summary>
		public const string FILE_NAME = "Partita";

		/// <summary>
		/// Adds <see cref="ChiamataLibrary.GameData"/> to the archive from the XML files found in the specified folder.
		/// </summary>
		/// <param name="path">The path of the XML files.</param>
		public void addFromFolder (string path)
		{
			_lastGameCount = 0;
			String completePath = Path.Combine (path, FILE_NAME + _lastGameCount.ToString () + ".xml");

			while (File.Exists (completePath)) {
				add (new GameData (completePath));
				++_lastGameCount;
				completePath = Path.Combine (path, FILE_NAME + _lastGameCount.ToString () + ".xml");
			}


		}

		/// <summary>
		/// Saves the <see cref="ChiamataLibrary.GameData"/> in XML files and puts them in the specified folder.
		/// </summary>
		/// <param name="path">The path of the XML files.</param>
		public void saveInFolder (string path)
		{
			int i = 0;
			_listGames.ForEach (delegate(GameData gd) {
				gd.writeOnXML (Path.Combine (path, FILE_NAME + i.ToString () + ".xml"));
				++i;
			});
		}

		/// <summary>
		/// saves only the last game on XML.
		/// </summary>
		/// <param name="path">The path of the XML file.</param>
		public void saveLastGame (string path)
		{
			lastGame ().writeOnXML (Path.Combine (path, FILE_NAME + _lastGameCount.ToString () + ".xml"));
			++_lastGameCount;
		}

		/// <summary>
		/// Deletes all the saved XML files found in the specified folder.
		/// </summary>
		/// <param name="path">The path of the XML files.</param>
		public void delete (string path)
		{
			string completePath = "";
			for (int i = 0; i < _listGames.Count; ++i) {
				completePath = Path.Combine (path, FILE_NAME + i.ToString () + ".xml");
				new FileInfo (completePath).Delete ();
			}
			_listGames.Clear ();
		}

		/// <summary>
		/// Returns the last <see cref="ChiamataLibrary.GameData"/> saved.
		/// </summary>
		/// <returns>The last <see cref="ChiamataLibrary.GameData"/> saved.</returns>
		public GameData lastGame ()
		{
			if (_listGames.Count > 0)
				return _listGames [_listGames.Count - 1];
			return null;
		}

		#region Info

		/// <summary>
		/// Returns a list containint the name of every player that has ever played in the saved games, no matter how many times.
		/// </summary>
		/// <returns>The names of all players.</returns>
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

		/// <summary>
		/// Retuns how many times  a player with the specified name has played in the saved games.
		/// </summary>
		/// <returns>The number of times the provided player played.</returns>
		/// <param name="name">Name of the player.</param>
		public int getPlayed (string name)
		{
			int count = 0;
			_listGames.ForEach (delegate(GameData gm) {
				for (int i = 0; i < Board.PLAYER_NUMBER; ++i)
					if (gm.getPlayer (i).name == name)
						++count;

			});
			return count;
		}

		/// <summary>
		/// Gets the total points made by the player provided.
		/// </summary>
		/// <returns>The total points of the player.</returns>
		/// <param name="player">Name of the player.</param>
		public int getTotalPoint (string player)
		{
			int tp = 0;
			_listGames.ForEach (delegate(GameData gm) {
				for (int i = 0; i < Board.PLAYER_NUMBER; i++)
					if (gm.getPlayer (i).name == player)
						tp = tp + gm.getAward (i);
			});
			return tp;
		}


		#endregion

		/// <summary>
		/// Adds a single <see cref="ChiamataLibrary.GameData"/> from a single XML file.
		/// </summary>
		/// <param name="path">Path of the XML file.</param>
		public void AddFromXML (string path)
		{
			_listGames.Add (new GameData (path));
		}


	}
}

