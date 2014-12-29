using System;

namespace BTLibrary
{
	public enum ConnectionState
	{
		//TODO : togliere "= numero"
		STATE_NONE = 1,
		STATE_LISTEN = 2,
		STATE_CONNECTING = 3,
		STATE_CONNECTED_SLAVE = 4,
		STATE_CONNECTED_MASTER = 5,

	}


}

