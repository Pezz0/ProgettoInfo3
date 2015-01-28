using System;

namespace BTLibrary
{
	/// <summary>
	/// Enumeration for the package type.
	/// </summary>
	public enum EnPackageType
	{
		NONE,

		//init
		NAME,
		BOARD,

		//playtime
		READY,
		BID,
		SEME,
		MOVE,

		//terminate
		TERMINATE,

		//ack
		ACK,
	}
}