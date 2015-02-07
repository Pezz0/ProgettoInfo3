using System;
using BTLibrary;
using NUnit.Framework;
using ChiamataLibrary;

namespace TestProject
{
	/// <summary>
	/// Test for the <see cref="BTLibrary.Package"/> class.
	/// </summary>
	[TestFixture ()]
	internal class PackageTest
	{
		/// <summary>
		/// Setup for this tests.
		/// </summary>
		[SetUp ()]
		public void setup ()
		{
			Board.Instance.Reset ();
			using (TestRandom rnd = new TestRandom ())
				Board.Instance.InitializeMaster (new string[]{ "A", "B", "C", "D", "E" }, 2, rnd);//il mazziere è C
		}

		#region Bid test

		/// <summary>
		/// First test for the <see cref="BTLibrary.PackageBid"/> class.
		/// Creating a new bid package from a new pass bid, to check if the created package is indeed a 
		/// <see cref="BTLibrary.EnPackageType.BID"/> and if the informations stored are the right ones (bidder and bid).
		/// </summary>
		[Test ()]
		public void packageBidPassTest ()
		{
			BidBase b = new PassBid (Board.Instance.Me);
			PackageBid pkgb = new PackageBid (b);

			byte [] bs = pkgb.getMessage ();

			PackageBid pkgbRec = new PackageBid (bs);

			Assert.AreEqual (EnPackageType.BID, pkgb.type);
			Assert.AreSame (Board.Instance.Me, pkgb.bid.bidder);
			Assert.True (b.Equals (pkgbRec.bid));
		}

		/// <summary>
		/// Second test for the <see cref="BTLibrary.PackageBid"/> class.
		/// Creating a new ACK package for a bid from a new pass bid, to check if the message stored in the ACK
		/// is indeed a <see cref="BTLibrary.EnPackageType.BID"/> and if the informations stored are the right ones (bidder and bid).
		/// </summary>
		[Test ()]
		public void packageBidPassAckTest ()
		{
			BidBase b = new PassBid (Board.Instance.Me);
			PackageBid pkgb = new PackageBid (b);
		
			byte [] bs = pkgb.getAckMessage ();
		
			PackageBid pkgbRec = new PackageBid (PackageBase.getMessageFromAck (bs));
		
			Assert.AreEqual (EnPackageType.BID, pkgb.type);
			Assert.AreSame (Board.Instance.Me, pkgb.bid.bidder);
			Assert.AreEqual (b, pkgbRec.bid);
		}

		/// <summary>
		/// Third test for the <see cref="BTLibrary.PackageBid"/> class.
		/// Creating a new bid package from a new carichi bid, to check if the created package is indeed a 
		/// <see cref="BTLibrary.EnPackageType.BID"/> and if the informations stored are the right ones (bidder and bid).
		/// </summary>
		[Test ()]
		public void packageBidCarichiTest ()
		{
			BidBase b = new CarichiBid (Board.Instance.Me, 61);
			PackageBid pkgb = new PackageBid (b);

			byte [] bs = pkgb.getMessage ();

			PackageBid pkgbRec = new PackageBid (bs);

			Assert.AreEqual (EnPackageType.BID, pkgb.type);
			Assert.AreSame (Board.Instance.Me, pkgb.bid.bidder);
			Assert.True (b.Equals (pkgbRec.bid));
		}

		/// <summary>
		/// Third test for the <see cref="BTLibrary.PackageBid"/> class.
		/// Creating a new ACK package for a bid from a new carichi bid, to check if the message stored in the ACK 
		/// is indeed a <see cref="BTLibrary.EnPackageType.BID"/> and if the informations stored are the right ones (bidder and bid).
		/// </summary>
		[Test ()]
		public void packageBidCarichiAckTest ()
		{
			BidBase b = new CarichiBid (Board.Instance.Me, 61);
			PackageBid pkgb = new PackageBid (b);
		
			byte [] bs = pkgb.getAckMessage ();
		
			PackageBid pkgbRec = new PackageBid (PackageBase.getMessageFromAck (bs));
		
			Assert.AreEqual (EnPackageType.BID, pkgb.type);
			Assert.AreSame (Board.Instance.Me, pkgb.bid.bidder);
			Assert.True (b.Equals (pkgbRec.bid));
		}

		/// <summary>
		/// Fourth test for the <see cref="BTLibrary.PackageBid"/> class.
		/// Creating a new bid package from a new normal bid, to check if the created package is indeed a 
		/// <see cref="BTLibrary.EnPackageType.BID"/> and if the informations stored are the right ones (bidder and bid).
		/// </summary>
		[Test ()]
		public void packageBidNormalTest ()
		{
			BidBase b = new NormalBid (Board.Instance.Me, EnNumbers.CAVALLO, 61);
			PackageBid pkgb = new PackageBid (b);

			byte [] bs = pkgb.getMessage ();

			PackageBid pkgbRec = new PackageBid (bs);

			Assert.AreEqual (EnPackageType.BID, pkgb.type);
			Assert.AreSame (Board.Instance.Me, pkgb.bid.bidder);
			Assert.True (b.Equals (pkgbRec.bid));
		}

		/// <summary>
		/// Fifth test for the <see cref="BTLibrary.PackageBid"/> class.
		/// Creating a new ACK package for a bid from a new normal bid, to check if the message stored in the ACK 
		/// is indeed a <see cref="BTLibrary.EnPackageType.BID"/> and if the informations stored are the right ones (bidder and bid).
		/// </summary>
		[Test ()]
		public void packageBidNormalAckTest ()
		{
			BidBase b = new NormalBid (Board.Instance.Me, EnNumbers.CAVALLO, 61);
			PackageBid pkgb = new PackageBid (b);
		
			byte [] bs = pkgb.getAckMessage ();
		
			PackageBid pkgbRec = new PackageBid (PackageBase.getMessageFromAck (bs));
		
			Assert.AreEqual (EnPackageType.BID, pkgb.type);
			Assert.AreSame (Board.Instance.Me, pkgb.bid.bidder);
			Assert.True (b.Equals (pkgbRec.bid));
		}

		#endregion

		#region Move

		/// <summary>
		/// First test for the <see cref="BTLibrary.PackageCard"/> class.
		/// Creating a new card package from a new move, to check if the message is indeed a 
		/// <see cref="BTLibrary.EnPackageType.MOVE"/> and if the informations stored are the right ones (player and card).
		/// </summary>
		[Test ()]
		public void packageCardTest ()
		{
			Player p = Board.Instance.Me;
			Card c = Board.Instance.GetCard (EnSemi.BASTONI, EnNumbers.CINQUE);

			PackageCard pkgc = new PackageCard (p, c);

			byte [] bs = pkgc.getMessage ();

			PackageCard pkgcRec = new PackageCard (bs);

			Assert.AreEqual (EnPackageType.MOVE, pkgcRec.type);
			Assert.AreSame (p, pkgcRec.player);
			Assert.AreSame (c, pkgcRec.card);
		}

		/// <summary>
		/// Second test for the <see cref="BTLibrary.PackageCard"/> class.
		/// Creating a new ACK package for a move from a new move, to check if the message stored in the ACK is indeed a 
		/// <see cref="BTLibrary.EnPackageType.MOVE"/> and if the informations stored are the right ones (player and card).
		/// </summary>
		[Test ()]
		public void packageCardAckTest ()
		{
			Player p = Board.Instance.Me;
			Card c = Board.Instance.GetCard (EnSemi.BASTONI, EnNumbers.CINQUE);
			PackageCard pkgc = new PackageCard (p, c);

			byte [] bs = pkgc.getAckMessage ();

			PackageCard pkgcRec = new PackageCard (PackageBase.getMessageFromAck (bs));

			Assert.AreEqual (EnPackageType.MOVE, pkgcRec.type);
			Assert.AreSame (p, pkgcRec.player);
			Assert.AreSame (c, pkgcRec.card);
		}

		#endregion

		#region Name

		/// <summary>
		/// First test for the <see cref="BTLibrary.PackageName"/> class.
		/// Creating a new name package from a new name, to check if the message is indeed a 
		/// <see cref="BTLibrary.EnPackageType.NAME"/> and if the name stored is the right one.
		/// </summary>
		[Test ()]
		public void packageNameTest ()
		{
			string name = "pippo";
			PackageName pkgn = new PackageName (name);

			byte [] bs = pkgn.getMessage ();

			PackageName pkgnRec = new PackageName (bs);

			Assert.AreEqual (EnPackageType.NAME, pkgnRec.type);
			Assert.AreEqual (name, pkgnRec.name);
		}

		/// <summary>
		/// Second test for the <see cref="BTLibrary.PackageName"/> class.
		/// Creating a new ACK package for a name from a new name, to check if the message stored in the ACK is indeed a 
		/// <see cref="BTLibrary.EnPackageType.NAME"/> and if the name stored is the right one.
		/// </summary>
		[Test ()]
		public void packageNameAckTest ()
		{
			string name = "pippo";
			PackageName pkgn = new PackageName (name);

			byte [] bs = pkgn.getAckMessage ();

			PackageName pkgnRec = new PackageName (PackageBase.getMessageFromAck (bs));

			Assert.AreEqual (EnPackageType.NAME, pkgnRec.type);
			Assert.AreEqual (name, pkgnRec.name);
		}


		#endregion

		#region Seme

		/// <summary>
		/// First test for the <see cref="BTLibrary.PackageSeme"/> class.
		/// Creating a new seme package from a new seme, to check if the message is indeed a 
		/// <see cref="BTLibrary.EnPackageType.SEME"/> and if the informations stored are the right ones (player and seme).
		/// </summary>
		[Test ()]
		public void packageSemeTest ()
		{
			EnSemi s = EnSemi.COPE;
			PackageSeme pkgs = new PackageSeme (Board.Instance.Me, s);

			byte [] bs = pkgs.getMessage ();

			PackageSeme pkgsRec = new PackageSeme (bs);

			Assert.AreEqual (EnPackageType.SEME, pkgsRec.type);
			Assert.AreSame (Board.Instance.Me, pkgsRec.player);
			Assert.AreEqual (s, pkgsRec.seme);
		}

		/// <summary>
		/// Second test for the <see cref="BTLibrary.PackageSeme"/> class.
		/// Creating a new ACK package for a seme from a new seme, to check if the message stored in the ACK is indeed a 
		/// <see cref="BTLibrary.EnPackageType.SEME"/> and if the informations stored are the right ones (player and seme).
		/// </summary>
		[Test ()]
		public void packageSemeAckTest ()
		{
			EnSemi s = EnSemi.COPE;
			PackageSeme pkgs = new PackageSeme (Board.Instance.Me, s);

			byte [] bs = pkgs.getAckMessage ();

			PackageSeme pkgsRec = new PackageSeme (PackageBase.getMessageFromAck (bs));

			Assert.AreEqual (EnPackageType.SEME, pkgsRec.type);
			Assert.AreSame (Board.Instance.Me, pkgsRec.player);
			Assert.AreEqual (s, pkgsRec.seme);
		}

		#endregion

		#region Terminate

		/// <summary>
		/// First test for the <see cref="BTLibrary.PackageTerminate"/> class.
		/// Creating a new terminate package from a new terminate message, to check if the message is indeed a 
		/// <see cref="BTLibrary.EnPackageType.TERMINATE"/> and if the terminate signal is the right one.
		/// </summary>
		[Test ()]
		public void packageTerminateTest ()
		{
			PackageTerminate pkgt = new PackageTerminate (0);

			byte [] bs = pkgt.getMessage ();

			PackageTerminate pkgtRec = new PackageTerminate (bs);

			Assert.AreEqual (EnPackageType.TERMINATE, pkgtRec.type);
			Assert.AreEqual (0, pkgtRec.terminateSignal);

		}

		/// <summary>
		/// Second test for the <see cref="BTLibrary.PackageTerminate"/> class.
		/// Creating a new ACK package for a terminate message from a new terminate message, to check if the message stored in the ACK is indeed a 
		/// <see cref="BTLibrary.EnPackageType.TERMINATE"/> and if the terminate signal is the right one.
		/// </summary>
		[Test ()]
		public void packageTerminateAckTest ()
		{
			PackageTerminate pkgt = new PackageTerminate (0);

			byte [] bs = pkgt.getAckMessage ();

			PackageTerminate pkgtRec = new PackageTerminate (PackageBase.getMessageFromAck (bs));

			Assert.AreEqual (EnPackageType.TERMINATE, pkgtRec.type);
			Assert.AreEqual (0, pkgtRec.terminateSignal);

		}


		#endregion

	}
}

