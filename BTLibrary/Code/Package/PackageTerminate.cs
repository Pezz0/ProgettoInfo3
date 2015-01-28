using System;
using System.Collections.Generic;

namespace BTLibrary
{
	/// <summary>
	/// Package representing a terminate message.
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
	/// <item><term>TERMINATE</term><description>1 Byte</description></item>
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
	/// <item><term>TERMINATE</term><description>1 Byte</description></item>
	/// </list>
	/// 
	/// 
	/// 
	/// The TERMINATE message is composed of:
	/// <list type="number">
	/// <item><term>Terminate signal</term><description>An integer indicating if the new game should be started or not (0 means abort and 1 start next game).</description></item>
	/// </list> 
	/// </summary>
	public class PackageTerminate:Package
	{
		/// <summary>
		/// Code for the terminate signal.
		/// </summary>
		/// <remarks>0 to abort, 1 to continue</remarks>
		public readonly int terminateSignal;

		/// <summary>
		/// Initializes a new instance of the <see cref="BTLibrary.PackageTerminate"/> class.
		/// </summary>
		/// <param name="terminateSignal">Terminate signal. 0 to abort, 1 to continue.</param>
		public PackageTerminate (int terminateSignal) : base (EnPackageType.TERMINATE)
		{
			this.terminateSignal = terminateSignal;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BTLibrary.PackageTerminate"/> class.
		/// </summary>
		/// <param name="bs">Bytes array representing the terminate signal. 0 to abort, 1 to continue.</param>
		/// <exception cref="System.Exception">Thrown when the byte sequence doesen't represent a terminate message.</exception>
		public PackageTerminate (byte [] bs) : base (EnPackageType.TERMINATE)
		{
			if (bs [0] != (byte) type)
				throw new Exception ("Wrong byte's sequence");
				
			terminateSignal = (int) bs [1];
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
			msg.Add ((byte) terminateSignal);
			return msg.ToArray ();
		}


	}
}

