using System;
using Android.OS;
using ChiamataLibrary;
using Android.Widget;
using Android.App;
using System.Collections.Generic;


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
			if (BTPlayService.Instance.isSlave ())
				BTPlayService.Instance.WriteToMaster (EnContentType.READY, Board.Instance.Me, new byte[0]);
			else
				BTPlayService.Instance.WriteToAllSlave (EnContentType.READY, Board.Instance.Me, new byte[0]);
				
		}

		public void bidPlaced (IBid bid)
		{
			List<byte> msg = new List<byte> ();

			msg.Add ((byte) Board.Instance.NumberOfBid);
			msg.AddRange (bid.toByteArray ());

			if (BTPlayService.Instance.isSlave ())
				BTPlayService.Instance.WriteToMaster (EnContentType.BID, bid.bidder, msg.ToArray ());
			else
				BTPlayService.Instance.WriteToAllSlave (EnContentType.BID, bid.bidder, msg.ToArray ());
		}

		public void semeChosen ()
		{
			if (BTPlayService.Instance.isSlave ()) {
				if (Board.Instance.Me.Role == EnRole.CHIAMANTE)
					BTPlayService.Instance.WriteToMaster (EnContentType.SEME, Board.Instance.getChiamante (), new byte[1]{ (byte) Board.Instance.CalledCard.seme });
			} else
				BTPlayService.Instance.WriteToAllSlave (EnContentType.SEME, Board.Instance.getChiamante (), new byte[1]{ (byte) Board.Instance.CalledCard.seme });
		}

		public void cardPlayed (Move move)
		{
			List<byte> msg = new List<byte> ();

			msg.Add ((byte) Board.Instance.Time);
			msg.AddRange (move.card.toByteArray ());

			if (BTPlayService.Instance.isSlave ())
				BTPlayService.Instance.WriteToMaster (EnContentType.MOVE, move.player, msg.ToArray ());
			else
				BTPlayService.Instance.WriteToAllSlave (EnContentType.MOVE, move.player, msg.ToArray ());
		}
	}
}

