using System;
using Android.OS;
using ChiamataLibrary;

namespace BTLibrary
{
	public class BTHandler:Handler
	{
	
		public BTHandler ()
		{

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

				break;

			}
		}

	}
}

