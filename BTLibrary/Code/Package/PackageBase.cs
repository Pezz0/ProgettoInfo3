using System;
using ChiamataLibrary;
using System.Collections.Generic;
using System.Text;

namespace BTLibrary
{
	/// <summary>
	/// Used to contain messages.
	/// 
	/// 
	/// This package will be similar to:
	/// <list type="table">
	/// <listheader>
	/// <term>Type</term>
	/// <term>Message</term>
	/// </listheader>
	/// <item><term>4</term><description>1 Byte</description></item>
	/// <item><term>MESSAGE</term></item>
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
	/// <item><term>MESSAGE</term></item>
	/// </list>
	/// </summary>
	public abstract class PackageBase : IEquatable<EnPackageType>
	{
		/// <summary>
		/// <see cref="Enumeration.EnPackageType"/> representing the type of the package.
		/// </summary>
		public readonly EnPackageType type;

		/// <summary>
		/// Initializes a new instance of the <see cref="BTLibrary.Package"/> class.
		/// </summary>
		/// <param name="type">The Type of the package.</param>
		protected PackageBase (EnPackageType type)
		{
			this.type = type;
		}

		/// <summary>
		/// Creates the package adding the message to it.
		/// </summary>
		/// <returns>The complete package.</returns>
		/// <param name="b">The bytes array containing the message.</param>
		/// <exception cref="System.Exception">Thrown when the byte sequence doesen't represent a bid, board, move, name, seme or terminate.</exception>
		internal static PackageBase createPackage (byte [] b)
		{
			EnPackageType t = (EnPackageType) b [0];
			switch (t) {
				case EnPackageType.BID:
					return new PackageBid (b);
				case EnPackageType.BOARD:
					return new PackageBoard (b);
				case EnPackageType.MOVE:
					return new PackageCard (b);
				case EnPackageType.NAME:
					return new PackageName (b);
				case EnPackageType.SEME:
					return new PackageSeme (b);
				case EnPackageType.TERMINATE:
					return new PackageTerminate (b);
				default:
					throw new Exception ("Wrong byte's sequence");
			}

		}

		///<summary>
		/// Overload for the == operator.
		/// </summary>
		/// <param name="package">Package.</param>
		/// <param name="type">Type of package.</param>
		public static bool operator == (PackageBase package, EnPackageType type)
		{
			return package.type == type;
		}

		///<summary>
		/// Overload for the != operator.
		/// </summary>
		/// <param name="package">Package.</param>
		/// <param name="type">Type of package.</param>
		public static bool operator != (PackageBase package, EnPackageType type)
		{
			return package.type != type;
		}

		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="BTLibrary.Package"/>.</param>
		/// <summary>
		/// Compares an object with a <see cref="BTLibrary.Package"/>.
		/// </summary>
		/// <returns><c>false</c> if the object is not a <see cref="BTLibrary.EnPackageType"/> or is not the same type as the current <see cref="BTLibrary.Package"/>, otherwise <c>true</c>.</returns>
		/// <remarks>Overrides Object's equals.</remarks>
		public override bool Equals (object obj)
		{
			if (obj is EnPackageType)
				return this == (EnPackageType) obj;
			else
				return false;
		}

		/// <summary>
		/// Overload for the != operator.
		/// </summary>
		/// <returns>The hashcode of the class.</returns>
		public override int GetHashCode ()
		{
			//FIXME: to implement but never used
			return base.GetHashCode ();
		}

		/// <param name="other"><see cref="BTLibrary.Package"/> to compare.</param>
		/// <summary>
		/// Compares two instances of <see cref="BTLibrary.Package"/>.
		/// </summary>
		/// <returns><c>false</c> if the <see cref="BTLibrary.Package"/> is not the same type as the current <see cref="BTLibrary.Package"/>, otherwise <c>true</c>.</returns>
		/// <param name="other">The <see cref="BTLibrary.EnPackageType"/> to compare with the current <see cref="BTLibrary.Package"/>.</param>
		public bool Equals (EnPackageType other)
		{
			return type == other;
		}


		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <returns>The message.</returns>
		/// <remarks>Implemented in the subclasses.</remarks>
		public abstract byte[] getMessage ();

		/// <summary>
		/// Create an ACK message based on the current instance of <see cref="BTLibrary.Package"/>.
		/// </summary>
		/// <returns>The ACK message.</returns>
		public byte[] getAckMessage ()
		{
			List<Byte> msg = new List<byte> (1024);

			msg.Add ((byte) EnPackageType.ACK);

			byte [] bAddress = Encoding.ASCII.GetBytes (BTManager.Instance.GetLocalAddress ());
			msg.AddRange (bAddress);

			msg.AddRange (getMessage ());

			return msg.ToArray ();
		}

		/// <summary>
		/// Gets the message from  an ACK package.
		/// </summary>
		/// <returns>The message.</returns>
		/// <param name="bs">The bytes array representing the package.</param>
		public static byte[] getMessageFromAck (byte [] bs)
		{
			List<byte> msg = new List<byte> ();
			//the other bytes indicate the message (normal or playtime)
			for (int i = 18; i < bs.GetLength (0); i++)
				msg.Add (bs [i]);

			return msg.ToArray ();
		}

		/// <summary>
		/// Gets the address from an ACK package.
		/// </summary>
		/// <returns>The address from which came the package.</returns>
		/// <param name="bs">The bytes array representing the package.</param>
		internal static string getAddressFromAck (byte [] bs)
		{
			char [] adr = new char[17];

			//the next 17 bytes indicete the address of the device who sends message
			for (int i = 1; i < 18; i++)
				adr [i - 1] = (char) bs [i];

			return new string (adr);
		}


	}
}

