using System;

namespace BTLibrary
{
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

		//ack
		ACK,
	}
}