using System;

namespace ChiamataLibrary
{
	public abstract class IArtificialIntelligence
	{
		private readonly int _me;

		public Player Me{ get { return Board.Instance.AllPlayers [_me]; } }

		public IArtificialIntelligence (int me)
		{
			_me = me;
		}
	}
}

