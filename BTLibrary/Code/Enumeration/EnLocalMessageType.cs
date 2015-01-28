using System;

namespace BTLibrary
{
	/// <summary>
	/// Enumeration for the local messages.
	/// </summary>
	public enum EnLocalMessageType
	{
		MESSAGE_STATE_CHANGE,
		MESSAGE_READ,
		MESSAGE_DEVICE_READ,
		MESSAGE_WRITE,
		MESSAGE_DEVICE_ADDR,
		MESSAGE_CONNECTION_LOST,
		MESSAGE_CONNECTION_FAILED,
		NEW_DEVICE,
		END_SCANNING,
		NONE_FOUND,
	}
}

