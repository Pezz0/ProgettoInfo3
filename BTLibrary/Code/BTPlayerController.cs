using System;
using ChiamataLibrary;
using System.Collections.Generic;
using Android.Util;

namespace BTLibrary
{
	/// <summary>
	/// Controller for a Bluetooth player.
	/// </summary>
	public class BTPlayerController:IPlayerController
	{
		/// <summary>
		/// The index of the player.
		/// </summary>
		private readonly int _player;

		/// <summary>
		/// The current bid made by the player.
		/// </summary>
		private Bid _bid;
		/// <summary>
		/// Boolean value indicating wheter or not the player has bidded.
		/// </summary>
		private bool _readyBid = false;

		/// <summary>
		/// The seme chosen by the player.
		/// </summary>
		/// <remarks>Nullable</remarks>
		private EnSemi? _seme;
		/// <summary>
		/// Boolean value indicating wheter or not the player has chosen a seme.
		/// </summary>
		private bool _readySeme = false;

		/// <summary>
		/// The current move made by the player.
		/// </summary>
		private Card _card;
		/// <summary>
		/// Boolean value indicating wheter or not the player has played a card.
		/// </summary>
		private bool _readyCard = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="BTLibrary.BTPlayerController"/> class.
		/// </summary>
		/// <param name="player">Index of the player contorlled by this bluetooth.</param>
		public BTPlayerController (int player)
		{
			_player = player;

			BTManager.Instance.eventPackageReceived += handleMessage;
		}

		/// <summary>
		/// Handles the bluetooth messages recived (only <see cref="BTLibrary.PackageBid"/>, <see cref="BTLibrary.PackageSeme"/> and <see cref="BTLibrary.PackageCard"/> packages will be accepted).
		/// </summary>
		/// <param name="pkg">Package.</param>
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
				if (pkgm.player.order == _player && pkgm.time >= Board.Instance.Time) {
					_readyCard = true;
					//recreate bid from message 
					_card = pkgm.card;
				}
			}
		}

		/// <summary>
		/// Method called by the <see cref="ChiamataLibrary.Board"/> to know which bid the player wants to place.
		/// Retuns null if no bid has been placed yet.
		/// </summary>
		/// <returns><c>The bid</c> if the message containing the bid has already arrived, <c>null</c> otherwise.</returns>
		public Bid ChooseBid ()
		{
			if (_readyBid) {
				_readyBid = false;
				return _bid;
			}
			return null;
		}

		/// <summary>
		/// Method called by the <see cref="ChiamataLibrary.Board"/> to know which seme the player wants to choose.
		/// Retuns null if no seme has been chosen yet.
		/// </summary>
		/// <returns><c>The seme</c> if the message containing the seme has already arrived, <c>null</c> otherwise.</returns>
		public EnSemi? ChooseSeme ()
		{
			if (_readySeme) {
				_readySeme = false;
				return _seme;
			}
			return null;
		}

		/// <summary>
		/// Method called by the <see cref="ChiamataLibrary.Board"/> to know which card the player wants to play.
		/// Returns null if no card has been played yet.
		/// </summary>
		/// <returns><c>The card</c> if the message containing the card has already arrived, <c>null</c> otherwise.</returns>
		public Card ChooseCard ()
		{
			if (_readyCard) {
				_readyCard = false;
				return _card;
			}
			return null;
		}

	}
}

