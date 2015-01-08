﻿using System;
using Android.OS;
using ChiamataLibrary;
using Android.Widget;
using Android.App;

namespace BTLibrary
{
	public class BTPlayer:Handler
	{
		private readonly Player _player;
		private bool _ready;
		private IBid _bid;
		private EnSemi? _seme;
		private Card _card;

		public BTPlayer (Player player)
		{
			_player = player;
			_ready = false;
			player.setAuctionControl (placeABid, chooseSeme);
			player.setPlaytimeControl (playACard);


		}

		public override void HandleMessage (Message msg)
		{
			if (msg.What == (int) MessageType.MESSAGE_READ) {
				byte [] data = (byte []) msg.Obj;
				Player sender = Board.Instance.getPlayer (data [0]);

				if (sender == _player) {
					if (Board.Instance.isAuctionPhase) {
						_ready = true;
						_bid = Board.Instance.DefBid.ricreateFromByteArray (data);
					}
					if (Board.Instance.isFinalizePhase) {
						_ready = true;
						_seme = (EnSemi) ( data [1] );
					}
					if (Board.Instance.isPlayTime) {
						_ready = true;
						_card = Board.Instance.getCard (data [1]);
					}
				}
			}

		}

		public IBid placeABid ()
		{
			if (_ready) {
				_ready = false;
				return _bid;
			}
			return null;
		}

		public EnSemi? chooseSeme ()
		{
			if (_ready) {
				_ready = false;
				return _seme;
			}
			return null;
		}

		public Card playACard ()
		{
			if (_ready) {
				_ready = false;
				return _card;
			}
			return null;
		}
	}
}

