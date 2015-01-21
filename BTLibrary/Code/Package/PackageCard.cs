using System;
using ChiamataLibrary;
using System.Collections.Generic;

namespace BTLibrary
{
	public class PackageCard:Package
	{
		public readonly int time;
		public readonly Move move;

		public PackageCard (Move move) : base (EnPackageType.MOVE)
		{
			time = Board.Instance.Time;
			this.move = move;
		}

		public PackageCard (byte [] bs) : base (EnPackageType.MOVE)
		{
			if (bs [0] == (byte) type)
				throw new Exception ("Wrong byte's sequence");

			EnSemi s = (EnSemi) ( bs [2] / Board.Instance.nNumber );
			EnNumbers n = (EnNumbers) ( bs [2] % Board.Instance.nNumber );

			move = new Move (Board.Instance.getCard (s, n), Board.Instance.getPlayer ((int) bs [1]));

			time = (int) bs [3];
		}

		public override byte[] getMessage ()
		{
			List<Byte> msg = new List<byte> (1024);
			msg.Add ((byte) type);
			msg.Add ((byte) move.player.order);
			msg.Add ((byte) ( ( (int) move.card.seme ) * Board.Instance.nNumber + ( (int) move.card.number ) ));
			msg.Add ((byte) time);
			return msg.ToArray ();

		}
	}
}

