using System;

namespace BTLibrary
{
	public enum ConnectionState
	{
		//TODO : togliere "= numero"
		STATE_NONE = 4,
		STATE_LISTEN = 5,
		STATE_CONNECTING = 6,
		STATE_CONNECTED_SLAVE = 7,
		STATE_CONNECTED_MASTER = 8,

	}


}

