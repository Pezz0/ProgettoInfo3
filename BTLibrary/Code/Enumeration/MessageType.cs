using System;

namespace BTLibrary
{
	public enum MessageType
	{
		//TODO : togliere "= numero" DONE
		MESSAGE_STATE_CHANGE,
		MESSAGE_READ,
		MESSAGE_DEVICE_READ,
		MESSAGE_WRITE,
		MESSAGE_DEVICE_ADDR,
		MESSAGE_CONNECTION_LOST,
		MESSAGE_TOAST,
		NEW_DEVICE,
		END_SCANNING,
		NONE_FOUND,
	}
}

