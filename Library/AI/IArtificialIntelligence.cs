using System;

namespace ChiamataLibrary
{
	public abstract class IArtificialIntelligence
	{
		public readonly Player me;

		public abstract bool Active{ get; set; }

		protected virtual void setup ()
		{
		}

		public IArtificialIntelligence (Player me)
		{
			this.me = me;
		}
	}
}

