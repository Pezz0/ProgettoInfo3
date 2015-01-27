using System;
using ChiamataLibrary;
using Android.Content;

namespace MenuLayout
{
	// TODO : implementare parceable

	/// <summary>
	/// Contains the data of the game about to begin.
	/// </summary>
	public class GameProfile
	{
		/// <summary>
		/// Array of player names.
		/// </summary>
		private readonly string [] _playerName = new string[Board.PLAYER_NUMBER];

		/// <summary>
		/// Array of player addresses.
		/// </summary>
		private readonly string [] _playerAddress = new string[Board.PLAYER_NUMBER - 1];

		/// <summary>
		/// Index of the dealer.
		/// </summary>
		private readonly int _dealer;

		/// <summary>
		/// Getter for the players name.
		/// </summary>
		/// <value>The players names.</value>
		public string[] PlayerNames { get { return _playerName; } }

		/// <summary>
		/// Gets the players address.
		/// </summary>
		/// <value>The player address.</value>
		public string[] PlayerAddress { get { return _playerAddress; } }

		/// <summary>
		/// Gets the name of the player by index.
		/// </summary>
		/// <returns>The player name.</returns>
		/// <param name="order">Index of the player.</param>
		public String getPlayerName (int order)
		{
			return _playerName [order];
		}

		/// <summary>
		/// Gets the player address by index.
		/// </summary>
		/// <returns>The player address.</returns>
		/// <param name="order">Index of the player.</param>
		public String getPlayerAddress (int order)
		{
			return _playerAddress [order];
		}

		/// <summary>
		/// Gets the index of the dealer.
		/// </summary>
		/// <value>The index of the dealer.</value>
		public int Dealer{ get { return _dealer; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="MenuLayout.GameProfile"/> class.
		/// </summary>
		/// <param name="playerName">Array of player names.</param>
		/// <param name="playerAddress">Array of player addresses.</param>
		/// <param name="dealer">Index of the dealer.</param>
		public GameProfile (string [] playerName, string [] playerAddress, int dealer)
		{
			for (int i = 0; i < Board.PLAYER_NUMBER; ++i) {
				_playerName [i] = playerName [i];
				if (i < Board.PLAYER_NUMBER - 1)
					_playerAddress [i] = playerAddress [i];
			}
			_dealer = dealer;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MenuLayout.GameProfile"/> class.
		/// </summary>
		/// <param name="intent">Intent.</param>
		public GameProfile (Intent intent)
		{
			_playerName = intent.GetStringArrayExtra ("Names");
			_playerAddress = intent.GetStringArrayExtra ("Address");
			_dealer = intent.GetIntExtra ("Dealer", 0);

		}

		/// <summary>
		/// Sets the intent to pass the data to the next activity.
		/// </summary>
		/// <param name="intent">Intent.</param>
		public void setIntent (Intent intent)
		{
			intent.PutExtra ("Names", _playerName);
			intent.PutExtra ("Address", _playerAddress);
			intent.PutExtra ("Dealer", _dealer);
		}

		/// <summary>
		/// Crates a new <see cref="MenuLayout.GameProfile"/> with the dealer incremented by one.
		/// </summary>
		/// <returns>Game profile of the next game.</returns>
		public GameProfile nextGame ()
		{
			return new GameProfile (_playerName, _playerAddress, ( _dealer + 1 ) % Board.PLAYER_NUMBER);
		}

	}
}

