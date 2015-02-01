using System;
using System.Collections.Generic;
using System.IO;
using PCLStorage;
using System.Threading.Tasks;

namespace ChiamataLibrary
{
	/// <summary>
	/// Contains all the data about past games and offers methods to save or load those data from XML files.
	/// </summary>
	public sealed class Archive
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
		public void ForEach (Action<GameData> action)
		{
			foreach (GameData gd in _listGames)
				action (gd);
		}

		/// <summary>
		/// Applies the method passed as an argument to each game saved after the date provided as the second argument.
		/// </summary>
		/// <param name="action">The method that must be execuded ofr each game. Must be a delegate of <see cref="Action<ChiamataLibrary.GameData>"/>.</param>
		/// <param name="from">The date before which the games will be ignored.</param>
		public void ForEach (Action<GameData> action, DateTime from)
		{
			foreach (GameData gd in _listGames)
				if (gd.time > from)
					action (gd);

		}

		/// <summary>
		/// Add the specified <see cref="ChiamataLibrary.GameData"/> to the archive.
		/// </summary>
		/// <param name="gm">The instance of <see cref="ChiamataLibrary.GameData"/> to be added.</param>
		internal void Add (GameData gm)
		{
			_listGames.Add (gm);
		}


		private IFolder GetFolder ()
		{
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			return  rootFolder.CreateFolderAsync ("Games", CreationCollisionOption.OpenIfExists).Result;
		}

		private string GetFileName (int i)
		{
			return "Partita" + i.ToString () + ".xml";
		}

		/// <summary>
		/// Adds <see cref="ChiamataLibrary.GameData"/> to the archive from the XML files found in the specified folder.
		/// </summary>
		/// <param name="path">The path of the XML files.</param>
		public void AddFromFolder ()
		{
			int c = 0;

			IFolder folder = GetFolder ();

			ExistenceCheckResult exist = folder.CheckExistsAsync (GetFileName (c)).Result;

			if (exist == ExistenceCheckResult.FileExists) {

				IFile file = folder.GetFileAsync (GetFileName (c)).Result;
				Stream s = file.OpenAsync (FileAccess.Read).Result;

				Add (new GameData (s));
				++c;
				exist = folder.CheckExistsAsync (GetFileName (c)).Result;
			}
		}

		/// <summary>
		/// Saves the <see cref="ChiamataLibrary.GameData"/> in XML files and puts them in the specified folder.
		/// </summary>
		/// <param name="path">The path of the XML files.</param>
		public void SaveAllInFolder ()
		{
			IFolder folder = GetFolder ();

			for (int i = 0; i < _listGames.Count; ++i) {
				IFile file = folder.CreateFileAsync (GetFileName (i), CreationCollisionOption.ReplaceExisting).Result;
				Stream s = file.OpenAsync (FileAccess.ReadAndWrite).Result;
				_listGames [i].WriteOnXML (s);
			}

		}

		/// <summary>
		/// saves only the last game on XML.
		/// </summary>
		/// <param name="path">The path of the XML file.</param>
		public void SaveLastGame ()
		{
			IFolder folder = GetFolder ();

			IFile file = folder.CreateFileAsync (GetFileName (_listGames.Count - 1), CreationCollisionOption.ReplaceExisting).Result;
			Stream s = file.OpenAsync (FileAccess.ReadAndWrite).Result;
			_listGames [_listGames.Count - 1].WriteOnXML (s);
		
		}

		/// <summary>
		/// Deletes all the saved XML files found in the specified folder.
		/// </summary>
		/// <param name="path">The path of the XML files.</param>
		public void DeleteAll ()
		{
			IFolder folder = GetFolder ();

			folder.DeleteAsync ().Wait ();

			_listGames.Clear ();
		}

		/// <summary>
		/// Returns the last <see cref="ChiamataLibrary.GameData"/> saved.
		/// </summary>
		/// <returns>The last <see cref="ChiamataLibrary.GameData"/> saved.</returns>

		public GameData LastGame {
			get {
				if (_listGames.Count > 0)
					return _listGames [_listGames.Count - 1];
				return null;
			}
		}

		#region Info

		/// <summary>
		/// Returns a list containint the name of every player that has ever played in the saved games, no matter how many times.
		/// </summary>
		/// <returns>The names of all players.</returns>
		public List<string> GetAllPlayer ()
		{
			List<string> lp = new List<string> ();
			foreach (GameData gd in _listGames)
				for (int i = 0; i < Board.PLAYER_NUMBER; i++)
					if (!lp.Contains (gd.GetPlayer (i).name))
						lp.Add (gd.GetPlayer (i).name);
				
			return lp;
		}

		/// <summary>
		/// Retuns how many times  a player with the specified name has played in the saved games.
		/// </summary>
		/// <returns>The number of times the provided player played.</returns>
		/// <param name="name">Name of the player.</param>
		public int GetPlayed (string name)
		{
			int count = 0;
			foreach (GameData gd in _listGames)
				for (int i = 0; i < Board.PLAYER_NUMBER; ++i)
					if (gd.GetPlayer (i).name == name)
						++count;

		
			return count;
		}

		/// <summary>
		/// Gets the total points made by the player provided.
		/// </summary>
		/// <returns>The total points of the player.</returns>
		/// <param name="player">Name of the player.</param>
		public int GetTotalAward (string player)
		{
			int tp = 0;
			foreach (GameData gd in _listGames)
				for (int i = 0; i < Board.PLAYER_NUMBER; i++)
					if (gd.GetPlayer (i).name == player)
						tp = tp + gd.GetAward (i);

			return tp;
		}


		#endregion

	}
}

