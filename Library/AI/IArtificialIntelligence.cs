using System;

namespace ChiamataLibrary
{
	public abstract class IArtificialIntelligence
	{
		public readonly Player me;

		protected virtual void setup ()
		{
		}

		public IArtificialIntelligence (Player me)
		{
			this.me = me;
		}
	}
}

