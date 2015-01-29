using System;
using ChiamataLibrary;
using System.Collections.Generic;

namespace BTLibrary
{
	/// <summary>
	/// Package representing a bid.
	///
	/// 
	/// 
	/// This package will be similar to:
	/// <list type="table">
	/// <listheader>
	/// <term>Type</term>
	/// <term>Message</term>
	/// </listheader>
	/// <item><term>4</term><description>1 Byte</description></item>
	/// <item><term>BID</term><description>4 Bytes</description></item>
	/// </list>
	///
	/// 
	/// or similar to the following if it's an ACK package:
	/// <list type="table">
	/// <listheader>
	/// <term>Type</term>
	/// <term>Address</term>
	/// <term>Message</term>
	/// </listheader>
	/// <item><term>8</term><description>1 Byte</description></item>
	/// <item><term>ADDRESS</term><description>17 Bytes</description></item>
	/// <item><term>BID</term><description>4 Bytes</description></item>
	/// </list>
	/// 
	/// 
	/// 
	/// The BID message is composed of 4 Bytes:
	/// <list type="number">
	/// <item><term>Bidder</term><description>The player placing the bid (represented by the index in the board).</description></item>
	/// <item><term>Points</term><description>The points related to bid or 255 to indicate a <see cref="ChiamataLibrary.PassBid"/>.</description></item>
	/// <item><term>Number</term><description>The number of the card related to bid or 255 to indicate a <see cref="ChiamataLibrary.CarichiBid"/>.</description></item>
	/// <item><term>Number of bid</term><description>The number of the bid.</description></item>
	/// </list> 
	/// </summary>
	public class PackageBid:Package
	{
		/// <summary>
		/// The current index of the bid.
		/// </summary>
		public readonly int nOfBid;
		/// <summary>
		/// The bid.
		/// </summary>
		public readonly Bid bid;

		/// <summary>
		/// Initializes a new instance of the <see cref="BTLibrary.PackageBid"/> class.
		/// </summary>
		/// <param name="bid">The bid that will be inserted in the message.</param>
		public PackageBid (Bid bid) : base (EnPackageType.BID)
		{
			this.bid = bid;
			nOfBid = Board.Instance.NumberOfBid;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BTLibrary.PackageBid"/> class.
		/// </summary>
		/// <param name="bs">The byte array representing the bid.</param>
		/// <exception cref="System.Exception">Thrown when the byte sequence doesen't represent a bid.</exception>
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

		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <returns>The message.</returns>
		/// <remarks>Implementation for the abstract method in <see cref="BTLibrary.Package"/>.</remarks>
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

