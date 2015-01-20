using System;
using Android.OS;
using ChiamataLibrary;
using Android.Widget;
using Android.App;
using System.Collections.Generic;


namespace BTLibrary
{
	/// <summary>
	/// Class to perform communication beetween BlueTooth  and the board
	/// </summary>
	public class BTManager
	{
	
		#region Singleton

		private static readonly BTManager _instance = new BTManager ();

		public static BTManager Instance{ get { return _instance; } }

		static BTManager ()
		{

		}

		private BTManager ()
		{

		}

		#endregion

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

		//When the Board event eventImReady happens, write to master or to all slave the message
		public void imReady ()
		{
			//the message is only one byte because the ready event doesn't need any information
			if (BTPlayService.Instance.isSlave ())
				BTPlayService.Instance.WriteToMaster (EnContentType.READY, Board.Instance.Me, new byte[0]);
			else
				BTPlayService.Instance.WriteToAllSlave (EnContentType.READY, Board.Instance.Me, new byte[0]);
				
		}

		//When the Board event eventIPlaceABid or eventSomeonePlaceABid happens, write to master or to all slave the message
		public void bidPlaced (IBid bid)
		{
			//the message is compose of the nuber of bid to control that happens in the correct board time
			// then is added the information about the bid type 
			List<byte> msg = new List<byte> ();

			msg.Add ((byte) Board.Instance.NumberOfBid);
			msg.AddRange (bid.toByteArray ());

			if (BTPlayService.Instance.isSlave ())
				BTPlayService.Instance.WriteToMaster (EnContentType.BID, bid.bidder, msg.ToArray ());
			else
				BTPlayService.Instance.WriteToAllSlave (EnContentType.BID, bid.bidder, msg.ToArray ());
		}

		//When the Board event eventPlaytimeStart happens, write to master or to all slave the message
		public void semeChosen ()
		{
			//if this is the slave and is the caller send to master one byte that indicate the seme chosen 
			if (BTPlayService.Instance.isSlave ()) {
				if (Board.Instance.Me.Role == EnRole.CHIAMANTE)
					BTPlayService.Instance.WriteToMaster (EnContentType.SEME, Board.Instance.getChiamante (), new byte[1]{ (byte) Board.Instance.CalledCard.seme });
				//if this is the master send to all slave one byte that indicate the seme chosen
			} else
				BTPlayService.Instance.WriteToAllSlave (EnContentType.SEME, Board.Instance.getChiamante (), new byte[1]{ (byte) Board.Instance.CalledCard.seme });
		}

		//When the Board event eventIPlayACard or eventSomeonePlayACard happens, write to master or to all slave the message
		public void cardPlayed (Move move)
		{
			//the message is composed of the time where the card is played and then the information about the card
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

