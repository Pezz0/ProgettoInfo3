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
				if (type != EnContentType.BOARD && type != EnContentType.ACK && type != EnContentType.NONE) {

					Player sender = Board.Instance.getPlayer (data [1]);

					if (type == EnContentType.READY && BTPlayService.Instance.isSlave ())
						_readyToStart = true;
					
					if (sender == _player) {
						if (type == EnContentType.READY && !BTPlayService.Instance.isSlave ())
							_readyToStart = true;

						if (type == EnContentType.BID && data [2] > Board.Instance.NumberOfBid) {
							_ready = true;

							_bid = Board.Instance.DefBid.recreateFromByteArray (new byte[3] {
								data [1],
								data [3],
								data [4]
							});
						}
						if (type == EnContentType.SEME) {
							_ready = true;
							_seme = (EnSemi) ( data [2] );
						}
						if (type == EnContentType.MOVE && data [2] >= Board.Instance.Time) {
							_ready = true;
							_card = Board.Instance.getCard (data [3]);
						}
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

