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

		public static BTManager Instance{ get { return _instance; } }

		static BTManager ()
		{

		}

		private BTManager ()
		{

		}

		public void initialize ()
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
			byte [] msg = new byte[2];
			msg [0] = (int) EnContentType.READY;
			msg [1] = Board.Instance.Me.toByteArray () [0];

			if (BTPlayService.Instance.isSlave ())
				BTPlayService.Instance.WriteToMaster (msg);
			else
				BTPlayService.Instance.WriteToAllSlave (msg);
				
		}

		public void bidPlaced (IBid bid)
		{
			byte [] msg = new byte[5];
			msg [0] = (int) EnContentType.BID;

			byte [] bs = bid.toByteArray ();

			msg [1] = bs [0];
			msg [3] = bs [1];
			msg [4] = bs [2];

			msg [2] = (byte) Board.Instance.NumberOfBid;

			if (BTPlayService.Instance.isSlave ())
				BTPlayService.Instance.WriteToMaster (msg);
			else
				BTPlayService.Instance.WriteToAllSlave (msg);
		}

		public void semeChosen ()
		{
			Byte [] msg = new Byte[3];
			msg [0] = (int) EnContentType.SEME;
			msg [1] = Board.Instance.getChiamante ().toByteArray () [0];
			msg [2] = (Byte) Board.Instance.Briscola;

			if (BTPlayService.Instance.isSlave ()) {
				if (Board.Instance.Me.Role == EnRole.CHIAMANTE)
					BTPlayService.Instance.WriteToMaster (msg);
			} else
				BTPlayService.Instance.WriteToAllSlave (msg);
		}

		public void cardPlayed (Move move)
		{
			byte [] msg = new byte[4];
			msg [0] = (int) EnContentType.MOVE;

			byte [] ms = move.toByteArray ();
			msg [1] = ms [0];
			msg [3] = ms [1];

			msg [2] = (byte) Board.Instance.Time;

			if (BTPlayService.Instance.isSlave ())
				BTPlayService.Instance.WriteToMaster (msg);
			else
				BTPlayService.Instance.WriteToAllSlave (msg);
		}
	}
}

