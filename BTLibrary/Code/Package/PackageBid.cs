using System;
using ChiamataLibrary;
using System.Collections.Generic;

namespace BTLibrary
{
	public class PackageBid:Package
	{
		public readonly int nOfBid;
		public readonly IBid bid;

		public PackageBid (IBid bid) : base (EnPackageType.BID)
		{
			this.bid = bid;
			nOfBid = Board.Instance.NumberOfBid;
		}

		public PackageBid (byte [] bs) : base (EnPackageType.BID)
		{
			if (bs [0] != (byte) type)
				throw new Exception ("Wrong byte's sequence");

			Player bidder = Board.Instance.getPlayer ((int) bs [1]);

			if (bs [2] == 255)
				bid = new PassBid (bidder);
			else if (bs [3] == 255)
				bid = new CarichiBid (bidder, (int) bs [2]);
			else
				bid = new NormalBid (bidder, (EnNumbers) bs [3], (int) bs [2]);

			nOfBid = (int) bs [4];
		}

		public override byte[] getMessage ()
		{
			List<Byte> msg = new List<byte> (1024);
			msg.Add ((byte) type);	//0
			msg.Add ((byte) bid.bidder.order);	//1

			if (bid is NotPassBid)	//2
				msg.Add ((byte) ( (NotPassBid) bid ).point);
			else
				msg.Add (255);

			if (bid is NormalBid)	//3
				msg.Add ((byte) ( (NormalBid) bid ).number);
			else
				msg.Add (255);

			msg.Add ((byte) nOfBid);	//4

			return msg.ToArray ();
		}

	}
}

