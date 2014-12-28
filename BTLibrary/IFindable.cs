using System;
using Android.Bluetooth;
using System.Collections.Generic;


namespace ProvaConnessioneBT
{
	public interface IFindable
	{

		bool isDiscovering ();

		void CancelDiscovery ();

		void UnregisterReciever ();

		List<string> GetPaired ();

		void Discovery ();

		void enableBluetooth ();

		void makeVisible (int amount);
	}
}

