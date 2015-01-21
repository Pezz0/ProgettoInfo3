using System;
using ChiamataLibrary;
using System.Collections.Generic;

namespace BTLibrary
{
	public class BTPlayerController:IPlayerController
	{
		private readonly Player _player;
		private bool _ready = false;

		private bool _readyToStart = false;
		private IBid _bid;
		private EnSemi? _seme;
		private Card _card;

		public BTPlayerController (Player player)
		{
			_player = player;
			player.Controller = this;

			BTManager.Instance.eventPackageReceived += handleMessage;
		}

		private void handleMessage (Package pkg)
		{
			if (pkg == EnPackageType.READY) {
				Player p = ( (PackageReady) pkg ).player;
				_readyToStart = BTManager.Instance.isSlave () || p == _player;
			} else if (pkg == EnPackageType.BID) {
				PackageBid pkgb = (PackageBid) pkg;
				if (pkgb.bid.bidder == _player && pkgb.nOfBid > Board.Instance.NumberOfBid) {
					_ready = true;
					//recreate bid from message 
					_bid = pkgb.bid;
				}
			} else if (pkg == EnPackageType.SEME) {
				PackageSeme pkgs = (PackageSeme) pkg;
				if (pkgs.player == _player) {
					_ready = true;
					//recreate bid from message 
					_seme = pkgs.seme;
				}
			} else if (pkg == EnPackageType.MOVE) {
				PackageCard pkgm = (PackageCard) pkg;
				if (pkgm.move.player == _player && pkgm.time >= Board.Instance.Time) {
					_ready = true;
					//recreate bid from message 
					_card = pkgm.move.card;
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

