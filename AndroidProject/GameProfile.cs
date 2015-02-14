using System;
using ChiamataLibrary;
using Android.Content;

namespace GUILayout
{
	// TODO : implementare parceable

	/// <summary>
	/// Contains the data of the game about to begin.
	/// </summary>
	internal class GameProfile
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
		internal string[] PlayerNames { get { return _playerName; } }

		/// <summary>
		/// Gets the players address.
		/// </summary>
		/// <value>The player address.</value>
		internal string[] PlayerAddress { get { return _playerAddress; } }

		/// <summary>
		/// A boolean that represent if the dealer can be change;
		/// </summary>
		private bool _dealerEnable;

		/// <summary>
		/// Gets the name of the player by index.
		/// </summary>
		/// <returns>The player name.</returns>
		/// <param name="order">Index of the player.</param>
		internal String getPlayerName (int order)
		{
			return _playerName [order];
		}

		/// <summary>
		/// Gets the player address by index.
		/// </summary>
		/// <returns>The player address.</returns>
		/// <param name="order">Index of the player.</param>
		internal String getPlayerAddress (int order)
		{
			return _playerAddress [order];
		}

		/// <summary>
		/// Gets the index of the dealer.
		/// </summary>
		/// <value>The index of the dealer.</value>
		internal int Dealer{ get { return _dealer; } }

		internal bool DealerEnabled{ get { return _dealerEnable; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="MenuLayout.GameProfile"/> class.
		/// </summary>
		/// <param name="playerName">Array of player names.</param>
		/// <param name="playerAddress">Array of player addresses.</param>
		/// <param name="dealer">Index of the dealer.</param>
		internal GameProfile (string [] playerName, string [] playerAddress, int dealer) : this (playerName, playerAddress, dealer, true)
		{
		

		}

		private GameProfile (string [] playerName, string [] playerAddress, int dealer, bool dealerEnable)
		{
			for (int i = 0; i < Board.PLAYER_NUMBER; ++i) {
				_playerName [i] = playerName [i];
				if (i < Board.PLAYER_NUMBER - 1)
					_playerAddress [i] = playerAddress [i];
			}
			_dealer = dealer;
			_dealerEnable = dealerEnable;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MenuLayout.GameProfile"/> class.
		/// </summary>
		/// <param name="intent">Intent.</param>
		internal GameProfile (Intent intent)
		{
			_playerName = intent.GetStringArrayExtra ("Names");
			_playerAddress = intent.GetStringArrayExtra ("Address");
			_dealer = intent.GetIntExtra ("Dealer", 0);
			_dealerEnable = intent.GetBooleanExtra ("DealerEnable", true);

		}

		/// <summary>
		/// Sets the intent to pass the data to the next activity.
		/// </summary>
		/// <param name="intent">Intent.</param>
		internal void setIntent (Intent intent)
		{
			intent.PutExtra ("Names", _playerName);
			intent.PutExtra ("Address", _playerAddress);
			intent.PutExtra ("Dealer", _dealer);
			intent.PutExtra ("DealerEnable", _dealerEnable);
		}

		/// <summary>
		/// Crates a new <see cref="MenuLayout.GameProfile"/> with the dealer incremented by one.
		/// </summary>
		/// <returns>Game profile of the next game.</returns>
		internal GameProfile nextGame ()
		{
			return new GameProfile (_playerName, _playerAddress, ( _dealer + 1 ) % Board.PLAYER_NUMBER, false);
		}

	}
}

