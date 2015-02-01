using System;
using System.Collections.Generic;
using System.Text;

namespace BTLibrary
{
	/// <summary>
	/// Package representing a name.
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
	/// <item><term>NAME</term></item>
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
	/// <item><term>NAME</term></item>
	/// </list>
	/// 
	/// 
	/// 
	/// The MOVE message is composed of:
	/// <list type="number">
	/// <item><term>Address</term><description>The address of the player changing their name.</description></item>
	/// <item><term>Name</term><description>The new name.</description></item>
	/// </list> 
	/// </summary>
	public class PackageName:PackageBase
	{
		/// <summary>
		/// Const representing the max character length for the name.
		/// </summary>
		public const int MAX_NAME_LENGHT = 10;
		/// <summary>
		/// The name.
		/// </summary>
		public readonly String name;
		/// <summary>
		/// The address of the bluetooth device connected to the name.
		/// </summary>
		public readonly String address;

		/// <summary>
		/// Initializes a new instance of the <see cref="BTLibrary.PackageName"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		public PackageName (String name) : base (EnPackageType.NAME)
		{
			this.name = name;
			address = BTManager.Instance.GetLocalAddress ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BTLibrary.PackageName"/> class.
		/// </summary>
		/// <param name="bs">Bytes array representing the name.</param>
		/// <exception cref="System.Exception">Thrown when the byte sequence doesen't represent a name.</exception>
		public PackageName (byte [] bs) : base (EnPackageType.NAME)
		{
			if (bs [0] != (byte) type)
				throw new Exception ("Wrong byte's sequence");

			char [] adr = new char[17];

			//the next 17 bytes indicete the address of the device who sends message
			for (int i = 1; i < 18; i++)
				adr [i - 1] = (char) bs [i];

			address = new string (adr);

			int len = (int) bs [18] + 19;
			List<byte> lb = new List<byte> ();

			for (int i = 19; i < len; ++i)
				lb.Add (bs [i]);
				
			name = Encoding.ASCII.GetString (lb.ToArray ());
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

			byte [] bAddress = Encoding.ASCII.GetBytes (address);
			msg.AddRange (bAddress);
			msg.Add ((byte) name.Length);
			msg.AddRange (Encoding.ASCII.GetBytes (name));
			return msg.ToArray ();
		}

	}
}

