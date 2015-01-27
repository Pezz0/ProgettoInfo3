using System;

namespace ChiamataLibrary
{
	public interface IPlayerController
	{

		IBid chooseBid ();

		EnSemi? chooseSeme ();

		Card chooseCard ();

	}
}

