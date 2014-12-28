using System;
using Android.OS;
using ChiamataLibrary;

namespace BTLibrary
{
	public class BTHandler:Handler
	{
		private BluetoothPlayService _btps;

		public BTHandler (BluetoothPlayService btps)
		{
			this._btps = btps;

		}

		public override void HandleMessage (Message msg)
		{
			switch (msg.What) {

				case (int)MessageType.MESSAGE_READ:

					if (Board.Instance.isAuctionPhase)
						Board.Instance.auctionPlaceABid ((byte []) msg.Obj);

					if (Board.Instance.isPlayTime)
						Board.Instance.PlayACard ((byte []) msg.Obj);

					Player sender = Board.Instance.getPlayer ((byte []) msg.Obj);

					if (!_btps.isSlave ())
						//TODO: invia a tutti tranne a sender
						;

				break;

			}
		}

	}}

