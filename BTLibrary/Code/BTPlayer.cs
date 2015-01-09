using System;
using Android.OS;
using ChiamataLibrary;
using Android.Widget;
using Android.App;

namespace BTLibrary
{
	public class BTPlayer:Handler,IPlayerController
	{
		private readonly Player _player;
		private bool _ready;

		private bool _readyToStart;
		private IBid _bid;
		private EnSemi? _seme;
		private Card _card;

		public BTPlayer (Player player)
		{
			_player = player;
			_ready = false;
			_readyToStart = false;
			player.Controller = this;
		}

		public override void HandleMessage (Message msg)
		{
			if (msg.What == (int) MessageType.MESSAGE_READ) {

				byte [] data = (byte []) msg.Obj;

				EnContentType type = (EnContentType) data [0];
				Player sender = Board.Instance.getPlayer (data [1]);

				if (type == EnContentType.READY && BTPlayService.Instance.isSlave ())
					_readyToStart = true;
					
				if (sender == _player) {
					if (type == EnContentType.READY && !BTPlayService.Instance.isSlave ())
						_readyToStart = true;

					if (type == EnContentType.BID) {
						_ready = true;

						_bid = Board.Instance.DefBid.recreateFromByteArray (new byte[3] {
							data [1],
							data [2],
							data [3]
						});
					}
					if (type == EnContentType.SEME) {
						_ready = true;
						_seme = (EnSemi) ( data [2] );
					}
					if (type == EnContentType.MOVE) {
						_ready = true;
						_card = Board.Instance.getCard (data [2]);
					}
				}
			}

		}

		public bool isReady { get { return _readyToStart; } }


		public IBid chooseBid ()
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

		public Card chooseCard ()
		{
			if (_ready) {
				_ready = false;
				return _card;
			}
			return null;
		}


	}
}

