using ChiamataLibrary;
using System.Collections.Generic;

namespace BTLibrary
{
	/// <summary>
	/// Class to handle playtime event messages
	/// </summary>
	public class BTPlayer:IPlayerController
	{
		private readonly Player _player;
		private bool _ready = false;

		private bool _readyToStart = false;
		private IBid _bid;
		private EnSemi? _seme;
		private Card _card;

		public BTPlayer (Player player)
		{
			_player = player;
			player.Controller = this;

			BTPlayService.Instance.eventMsgPlaytimeReceived += handleMessage;
		}

		private void handleMessage (EnContentType type, Player sender, List<byte> msg)
		{
			if (type == EnContentType.READY && BTPlayService.Instance.isSlave ())
				_readyToStart = true;

			if (sender == _player) {

				if (type == EnContentType.READY && !BTPlayService.Instance.isSlave ())
					_readyToStart = true;

				if (type == EnContentType.BID && msg [0] > Board.Instance.NumberOfBid) {
					_ready = true;
					//recreate bid from message 
					_bid = Board.Instance.DefBid.recreateFromByteArray (new byte[2] {
						msg [1],
						msg [2]
					}).changeBidder (sender);
				}
				if (type == EnContentType.SEME) {
					//recreate seme from message
					_ready = true;
					_seme = (EnSemi) ( msg [0] );
				}
				if (type == EnContentType.MOVE && msg [0] >= Board.Instance.Time) {
					//recreate card from message
					_ready = true;
					_card = Board.Instance.getCard (msg [1]);
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

