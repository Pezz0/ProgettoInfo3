using System;
using ChiamataLibrary;
using System.Collections.Generic;

namespace BTLibrary
{
	/// <summary>
	/// Package representing a played card.
	///
	/// 
	/// 
	/// This package will be similar to:
	/// <list type="table">
	/// <listheader>
	/// <term>Field</term>
	/// <term>Length</term>
	/// </listheader>
	/// <item><term>6</term><description>1 Byte</description></item>
	/// <item><term>MOVE</term><description>3 Bytes</description></item>
	/// </list>
	///
	/// 
	/// or similar to the following if it's an ACK package:
	/// <list type="table">
	/// <listheader>
	/// <term>Field</term>
	/// <term>Length</term>
	/// </listheader>
	/// <item><term>8</term><description>1 Byte</description></item>
	/// <item><term>ADDRESS</term><description>17 Bytes</description></item>
	/// <item><term>MOVE</term><description>3 Bytes</description></item>
	/// </list>
	/// 
	/// 
	/// 
	/// The MOVE message is composed of 3 Bytes:
	/// <list type="number">
	/// <item><term>Player</term><description>The index of the player making the move.</description></item>
	/// <item><term>Card</term><description>The index of the card played. The cards are ordeder in ascending order of points, following this seme order : ORI, COPPE, BASTONI, SPADE.</description></item>
	/// <item><term>Time</term><description>The value of the time variable in the <see cref="Chiamatalibrary.Board"/>, used to ignore messages that have already been recieved.</description></item>
	/// </list> 
	/// </summary>
	public class PackageCard:PackageBase
	{
		/// <summary>
		/// The time of the play. The global time is stored in <see cref="ChiamataLibrary.Board"/>.
		/// </summary>
		public readonly int time;

		public readonly Player player;

		public readonly Card card;


		/// <summary>
		/// Initializes a new instance of the <see cref="BTLibrary.PackageCard"/> class.
		/// </summary>
		/// <param name="move">Move.</param>
		public PackageCard (Player player, Card card) : base (EnPackageType.MOVE)
		{
			time = Board.Instance.Time;
			this.player = player;
			this.card = card;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BTLibrary.PackageCard"/> class.
		/// </summary>
		/// <param name="bs">Byte array representing the move.</param>
		/// <exception cref="System.Exception">Thrown when the byte sequence doesen't represent a move.</exception>
		public PackageCard (byte [] bs) : base (EnPackageType.MOVE)
		{
			if (bs [0] != (byte) type)
				throw new Exception ("Wrong byte's sequence");

			EnSemi s = (EnSemi) ( bs [2] / Board.Instance.nNumber );
			EnNumbers n = (EnNumbers) ( bs [2] % Board.Instance.nNumber );

			card = Board.Instance.GetCard (s, n); 
			player = (Player) bs [1];

			time = (int) bs [3];
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
			msg.Add ((byte) ( ( (int) card.seme ) * Board.Instance.nNumber + ( (int) card.number ) ));
			msg.Add ((byte) time);
			return msg.ToArray ();

		}
	}
}

