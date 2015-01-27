using System;
using ChiamataLibrary;
using System.Collections.Generic;
using Android.Util;

namespace BTLibrary
{
	public class BTPlayerController:IPlayerController
	{
		private readonly int _player;

		private IBid _bid;
		private bool _readyBid = false;

		private EnSemi? _seme;
		private bool _readySeme = false;

		private Card _card;
		private bool _readyCard = false;

		public BTPlayerController (int player)
		{
			_player = player;

			BTManager.Instance.eventPackageReceived += handleMessage;
		}

		private void handleMessage (Package pkg)
		{
			if (pkg == EnPackageType.BID) {
				PackageBid pkgb = (PackageBid) pkg;
				if (pkgb.bid.bidder.order == _player && pkgb.nOfBid > Board.Instance.NumberOfBid) {
					_readyBid = true;
					//recreate bid from message 
					_bid = pkgb.bid;
				}
			} else if (pkg == EnPackageType.SEME) {
				PackageSeme pkgs = (PackageSeme) pkg;
				if (pkgs.player.order == _player) {
					_readySeme = true;
					//recreate bid from message 
					_seme = pkgs.seme;
				}
			} else if (pkg == EnPackageType.MOVE) {
				PackageCard pkgm = (PackageCard) pkg;
				if (pkgm.move.player.order == _player && pkgm.time >= Board.Instance.Time) {
					Log.Debug ("da BTPlayer", pkgm.move.ToString () + pkgm.time + " " + Board.Instance.Time);
					_readyCard = true;
					//recreate bid from message 
					_card = pkgm.move.card;
				}
			}
		}

		public IBid chooseBid ()
		{
			if (_readyBid) {
				_readyBid = false;
				return _bid;
			}
			return null;
		}

		public EnSemi? chooseSeme ()
		{
			if (_readySeme) {
				_readySeme = false;
				return _seme;
			}
			return null;
		}

		public Card chooseCard ()
		{
			if (_readyCard) {
				_readyCard = false;
				return _card;
			}
			return null;
		}

	}
}

