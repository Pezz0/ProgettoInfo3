using System;

namespace ChiamataLibrary
{
	public interface IPlayerController
	{
		bool isReady{ get; }

		IBid chooseBid ();

		EnSemi? chooseSeme ();

		Card chooseCard ();

	}
}

