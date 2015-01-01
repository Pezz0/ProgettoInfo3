using System;

namespace ChiamataLibrary
{
	public abstract class IAIPlayTime:IArtificialIntelligence
	{
		protected abstract Card PlayACard ();

		public void controlYourTurnPlayTime (Move move)
		{
			startPlayTime ();
		}

		public void startPlayTime ()
		{
			if (!Board.Instance.isGameFinish && Board.Instance.ActivePlayer == Me)
				Board.Instance.PlayACard (Me, PlayACard ());
		}


		public IAIPlayTime (int me) : base (me)
		{
			Board.Instance.eventGameStarted += startPlayTime;

			Board.Instance.eventSomeonePlayACard += controlYourTurnPlayTime;

			Board.Instance.eventIPlayACard += controlYourTurnPlayTime;
		}

	}
}

