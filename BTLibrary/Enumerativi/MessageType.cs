using System;

namespace ProvaConnessioneBT
{
	public enum MessageType
	{
		MESSAGE_STATE_CHANGE = 1,
		MESSAGE_READ = 2,
		MESSAGE_WRITE = 3,
		MESSAGE_DEVICE_NAME = 4,
		MESSAGE_TOAST = 5,
		NEW_DEVICE = 6,
		END_SCANNING = 7,
		NONE_FOUND = 8,
	}
}

