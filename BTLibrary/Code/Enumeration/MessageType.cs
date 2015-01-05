using System;

namespace BTLibrary
{
	public enum MessageType
	{
		//TODO : togliere "= numero"
		MESSAGE_STATE_CHANGE = 9,
		MESSAGE_READ = 10,
		MESSAGE_WRITE = 11,
		MESSAGE_DEVICE_ADDR = 12,
		MESSAGE_TOAST = 13,
		NEW_DEVICE = 14,
		END_SCANNING = 15,
		NONE_FOUND = 16,
	}
}

