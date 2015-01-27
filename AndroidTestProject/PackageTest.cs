using System;
using BTLibrary;
using NUnit.Framework;
using ChiamataLibrary;

namespace TestProject
{
	[TestFixture ()]
	public class PackageTest
	{
		[SetUp ()]
		public void setup ()
		{
			Board.Instance.reset ();
			Board.Instance.initializeMaster (new string[]{ "A", "B", "C", "D", "E" }, 2);//il mazziere è C
		}

		#region Bid test

		[Test ()]
		public void packageBidPassTest ()
		{
			IBid b = new PassBid (Board.Instance.Me);
			PackageBid pkgb = new PackageBid (b);

			byte [] bs = pkgb.getMessage ();

			PackageBid pkgbRec = new PackageBid (bs);

			Assert.AreEqual (EnPackageType.BID, pkgb.type);
			Assert.AreSame (Board.Instance.Me, pkgb.bid.bidder);
			Assert.True (b.Equals (pkgbRec.bid));
		}
		//
		//		[Test ()]
		//		public void packageBidPassAckTest ()
		//		{
		//			IBid b = new PassBid (Board.Instance.Me);
		//			PackageBid pkgb = new PackageBid (b);
		//
		//			byte [] bs = pkgb.getAckMessage ();
		//
		//			PackageBid pkgbRec = new PackageBid (Package.getMessageFromHack (bs));
		//
		//			Assert.AreEqual (EnPackageType.BID, pkgb.type);
		//			Assert.AreSame (Board.Instance.Me, pkgb.bid.bidder);
		//			Assert.AreEqual (b, pkgbRec.bid);
		//		}

		[Test ()]
		public void packageBidCarichiTest ()
		{
			IBid b = new CarichiBid (Board.Instance.Me, 61);
			PackageBid pkgb = new PackageBid (b);

			byte [] bs = pkgb.getMessage ();

			PackageBid pkgbRec = new PackageBid (bs);

			Assert.AreEqual (EnPackageType.BID, pkgb.type);
			Assert.AreSame (Board.Instance.Me, pkgb.bid.bidder);
			Assert.True (b.Equals (pkgbRec.bid));
		}

		//		[Test ()]
		//		public void packageBidCarichiAckTest ()
		//		{
		//			IBid b = new CarichiBid (Board.Instance.Me, 61);
		//			PackageBid pkgb = new PackageBid (b);
		//
		//			byte [] bs = pkgb.getAckMessage ();
		//
		//			PackageBid pkgbRec = new PackageBid (Package.getMessageFromHack (bs));
		//
		//			Assert.AreEqual (EnPackageType.BID, pkgb.type);
		//			Assert.AreSame (Board.Instance.Me, pkgb.bid.bidder);
		//			Assert.True (b.Equals (pkgbRec.bid));
		//		}

		[Test ()]
		public void packageBidNormalTest ()
		{
			IBid b = new NormalBid (Board.Instance.Me, EnNumbers.CAVALLO, 61);
			PackageBid pkgb = new PackageBid (b);

			byte [] bs = pkgb.getMessage ();

			PackageBid pkgbRec = new PackageBid (bs);

			Assert.AreEqual (EnPackageType.BID, pkgb.type);
			Assert.AreSame (Board.Instance.Me, pkgb.bid.bidder);
			Assert.True (b.Equals (pkgbRec.bid));
		}

		//		[Test ()]
		//		public void packageBidNormalAckTest ()
		//		{
		//			IBid b = new NormalBid (Board.Instance.Me, EnNumbers.CAVALLO, 61);
		//			PackageBid pkgb = new PackageBid (b);
		//
		//			byte [] bs = pkgb.getAckMessage ();
		//
		//			PackageBid pkgbRec = new PackageBid (Package.getMessageFromHack (bs));
		//
		//			Assert.AreEqual (EnPackageType.BID, pkgb.type);
		//			Assert.AreSame (Board.Instance.Me, pkgb.bid.bidder);
		//			Assert.True (b.Equals (pkgbRec.bid));
		//		}

		#endregion


		#region Move

		[Test ()]
		public void packageCardTest ()
		{
			Move m = new Move (Board.Instance.getCard (EnSemi.BASTONI, EnNumbers.CINQUE), Board.Instance.Me);
			PackageCard pkgc = new PackageCard (m);

			byte [] bs = pkgc.getMessage ();

			PackageCard pkgcRec = new PackageCard (bs);

			Assert.AreEqual (EnPackageType.MOVE, pkgcRec.type);
			Assert.AreSame (Board.Instance.Me, pkgcRec.move.player);
			Assert.AreSame (Board.Instance.getCard (EnSemi.BASTONI, EnNumbers.CINQUE), pkgcRec.move.card);
		}

		#endregion

		#region Name

		[Test ()]
		public void packageNameTest ()
		{
			string name = "pippo";
			PackageName pkgn = new PackageName (name);

			byte [] bs = pkgn.getMessage ();

			PackageName pkgnRec = new PackageName (bs);

			Assert.AreEqual (EnPackageType.NAME, pkgnRec.type);
			Assert.AreSame (name, pkgnRec.name);
		}

		#endregion

		#region Seme

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

		#endregion

		#region Terminate

		[Test ()]
		public void packageTerminateTest ()
		{
			PackageTerminate pkgt = new PackageTerminate (0);

			byte [] bs = pkgt.getMessage ();

			PackageTerminate pkgtRec = new PackageTerminate (bs);

			Assert.AreEqual (EnPackageType.TERMINATE, pkgtRec.type);
			Assert.AreEqual (0, pkgtRec.terminateSignal);

		}


		#endregion

	}
}

