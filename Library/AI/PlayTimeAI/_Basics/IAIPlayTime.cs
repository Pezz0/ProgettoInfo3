using System;

namespace ChiamataLibrary
{
	public abstract class IAIPlayTime:IArtificialIntelligence
	{
		protected abstract Card playACard ();

		public void controlYourTurnPlayTime (Move move)
		{
			if (!Board.Instance.isGameFinish && Board.Instance.ActivePlayer == me)
				Board.Instance.PlayACard (me, playACard ());
		}

		public void startPlayTime ()
		{
			setup ();

			if (!Board.Instance.isGameFinish && Board.Instance.ActivePlayer == me)
				Board.Instance.PlayACard (me, playACard ());
		}


		public IAIPlayTime (Player me) : base (me)
		{
			Board.Instance.eventGameStarted += startPlayTime;

			Board.Instance.eventSomeonePlayACard += controlYourTurnPlayTime;

			Board.Instance.eventIPlayACard += controlYourTurnPlayTime;
		}

	}
}

