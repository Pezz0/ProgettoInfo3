using System;
using Android.Bluetooth;
using System.Collections.Generic;


namespace BTLibrary
{
	public interface IFindable
	{

		bool isDiscovering ();

		void CancelDiscovery ();

		List<string> GetPaired ();

		void Discovery ();

		void enableBluetooth ();

		void makeVisible (int amount);
	}
}

