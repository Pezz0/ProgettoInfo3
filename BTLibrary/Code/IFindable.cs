using System;
using Android.Bluetooth;
using System.Collections.Generic;


namespace BTLibrary
{
	public interface IFindable
	{

		/// <summary>
		/// Performs discovery of new device
		/// </summary>
		void Discovery ();

		/// <summary>
		/// Cancel the discovery activity
		/// </summary>
		void CancelDiscovery ();

		/// <summary>
		/// Registers the receiver
		/// </summary>
		void RegisterReceiver ();

	}
}

