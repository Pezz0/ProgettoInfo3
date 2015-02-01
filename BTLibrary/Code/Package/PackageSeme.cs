using System;
using ChiamataLibrary;
using System.Collections.Generic;

namespace BTLibrary
{
	/// <summary>
	/// Package representing a seme.
	///
	/// 
	/// 
	/// This package will be similar to:
	/// <list type="table">
	/// <listheader>
	/// <term>Type</term>
	/// <term>Message</term>
	/// </listheader>
	/// <item><term>1</term><description>1 Byte</description></item>
	/// <item><term>SEME</term><description>2 Bytes</description></item>
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
	/// <item><term>SEME</term><description>2 Bytes</description></item>
	/// </list>
	/// 
	/// 
	/// 
	/// The SEME message is composed of:
	/// <list type="number">
	/// <item><term>Player</term><description>The index of the player choosing the seme.</description></item>
	/// <item><term>Seme</term><description>The index of the seme (0 is ORI, 1 si COPPE, 2 is BASTONI, 3 is SPADE).</description></item>
	/// </list> 
	/// </summary>
	public class PackageSeme:PackageBase
	{
		/// <summary>
		/// The player that choose the seme.
		/// </summary>
		public readonly Player player;
		/// <summary>
		/// The seme.
		/// </summary>
		public readonly EnSemi seme;

		/// <summary>
		/// Initializes a new instance of the <see cref="BTLibrary.PackageSeme"/> class.
		/// </summary>
		/// <param name="player">The player that choose the seme.</param>
		/// <param name="seme">Seme.</param>
		public PackageSeme (Player player, EnSemi seme) : base (EnPackageType.SEME)
		{
			this.player = player;
			this.seme = seme;
		}

		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <returns>The message.</returns>
		/// <remarks>Implementation for the abstract method in <see cref="BTLibrary.Package"/>.</remarks>
		public override byte[] getMessage ()
		{
			List<Byte> msg = new List<byte> (1024);
			msg.Add ((byte) type);
			msg.Add ((byte) player.order);
			msg.Add ((byte) seme);
			return msg.ToArray ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BTLibrary.PackageSeme"/> class.
		/// </summary>
		/// <param name="bs">Bytes array representing the seme.</param>
		/// <exception cref="System.Exception">Thrown when the byte sequence doesen't represent a seme.</exception>
		public PackageSeme (byte [] bs) : base (EnPackageType.SEME)
		{
			if (bs [0] != (byte) type)
				throw new Exception ("Wrong byte's sequence");

			player = (Player) bs [1];
			seme = (EnSemi) bs [2];
		}


	}
}

