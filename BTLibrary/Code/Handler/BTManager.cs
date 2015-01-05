using System;
using Android.OS;
using ChiamataLibrary;
using Android.Widget;
using Android.App;
using BTLibrary;

namespace BTLibrary
{
	public class BTManager:Handler
	{
	

		private static readonly BTManager _instance = new BTManager ();

		public static BTManager Instance{ get { return _instance; } }

		static BTManager ()
		{
		}

		public BTManager ()
		{

		}

		public void WriteToAllSlave<T> (IBTSendable<T> bts)
		{
			BTPlayService.Instance.WriteToAllSlave (bts);
		}

		public void WriteToMaster<T> (IBTSendable<T> bts)
		{
			BTPlayService.Instance.WriteToMaster (bts);
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

					Toast.MakeText (Application.Context, (string) msg.Obj, ToastLength.Short);

				break;
				case (int)MessageType.MESSAGE_DEVICE_ADDR:
					Toast.MakeText (Application.Context, "Connected to " + BTPlayService.Instance.getRemoteDevice ((string) msg.Obj).Name, ToastLength.Short).Show ();

				break;
				case (int)MessageType.MESSAGE_TOAST:
					Toast.MakeText (Application.Context, (string) msg.Obj, ToastLength.Short).Show ();
				break;

			}
		}

	}
}

