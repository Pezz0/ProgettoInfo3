using System;
using Android.OS;
using ChiamataLibrary;
using Android.Widget;
using Android.App;


namespace BTLibrary
{
	public class BTManager
	{
	
		private static readonly BTManager _instance = new BTManager ();

		//public static BTManager Instance{ get { return _instance; } }

		static BTManager ()
		{

		}

		private BTManager ()
		{
			Board.Instance.eventImReady += imReady;

			Board.Instance.eventIPlaceABid += bidPlaced;

			if (!BTPlayService.Instance.isSlave ())
				Board.Instance.eventSomeonePlaceABid += bidPlaced;

			Board.Instance.eventPlaytimeStart += semeChosen;

			Board.Instance.eventIPlayACard += cardPlayed;

			if (!BTPlayService.Instance.isSlave ())
				Board.Instance.eventSomeonePlayACard += cardPlayed;
		}

		public void imReady ()
		{
			if (BTPlayService.Instance.isSlave ()) {
				BTPlayService.Instance.WriteToMaster (Board.Instance.Me);
			} else
				BTPlayService.Instance.WriteToAllSlave (Board.Instance.Me);
		}

		public void bidPlaced (IBid bid)
		{
			if (BTPlayService.Instance.isSlave ())
				BTPlayService.Instance.WriteToMaster (bid);
			else
				BTPlayService.Instance.WriteToAllSlave (bid);
		}

		public void semeChosen ()
		{
			Byte [] msg = new Byte[2];
			msg [0] = Board.Instance.getChiamante ().toByteArray () [0];
			msg [1] = (Byte) Board.Instance.Briscola;

			if (BTPlayService.Instance.isSlave ()) {
				if (Board.Instance.Me.Role == EnRole.CHIAMANTE)
					BTPlayService.Instance.WriteToMaster (msg);
			} else
				BTPlayService.Instance.WriteToAllSlave (msg);
		}

		public void cardPlayed (Move m)
		{

			if (BTPlayService.Instance.isSlave ())
				BTPlayService.Instance.WriteToMaster (m);
			else
				BTPlayService.Instance.WriteToAllSlave (m);
		}
	}
}

