using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// A stupid card chooser
	/// </summary>
	public class AICStupid:IAICardChooser
	{
		private Player _me;

		public void setup (Player me)
		{
			this._me = me;
		}

		public Card chooseCard ()
		{
			return _me.Hand [0];
		}

		public AICStupid ()
		{
		}
	}
}

