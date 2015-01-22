using System;
using ChiamataLibrary;
using Android.Content;

namespace MenuLayout
{
	public class GameProfile
	{
		private readonly string [] _playerName = new string[Board.PLAYER_NUMBER];
		private readonly string [] _playerAddress = new string[Board.PLAYER_NUMBER - 1];
		private readonly int _dealer;

		public string[] PlayerNames { get { return _playerName; } }

		public string[] PlayerAddress { get { return _playerAddress; } }

		public String getPlayerName (int order)
		{
			return _playerName [order];
		}

		public String getPlayerAddress (int order)
		{
			return _playerAddress [order];
		}

		public int Dealer{ get { return _dealer; } }

		public GameProfile (string [] playerName, string [] playerAddress, int dealer)
		{
			for (int i = 0; i < Board.PLAYER_NUMBER; ++i) {
				_playerName [i] = playerName [i];
				if (i < Board.PLAYER_NUMBER - 1)
					_playerAddress [i] = playerAddress [i];
			}
			_dealer = dealer;
		}

		public GameProfile (Intent intent)
		{
			_playerName = intent.GetStringArrayExtra ("Names");
			_playerAddress = intent.GetStringArrayExtra ("Address");
			_dealer = intent.GetIntExtra ("Dealer", 0);

		}

		public void setIntent (Intent intent)
		{
			intent.PutExtra ("Names", _playerName);
			intent.PutExtra ("Address", _playerAddress);
			intent.PutExtra ("Dealer", _dealer);
		}

		public GameProfile nextGame ()
		{
			return new GameProfile (_playerName, _playerAddress, ( _dealer + 1 ) % Board.PLAYER_NUMBER);
		}

	}
}

