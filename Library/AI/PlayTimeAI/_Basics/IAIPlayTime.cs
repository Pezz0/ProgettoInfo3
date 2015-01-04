using System;

namespace ChiamataLibrary
{
	public abstract class IAIPlayTime:IArtificialIntelligence
	{
		protected abstract Card playACard ();

		private bool _active;

		public override bool Active {
			get { return _active; }
			set {
				_active = value;
				if (_active) {
					Board.Instance.eventGameStarted += startPlayTime;
					Board.Instance.eventSomeonePlayACard += controlYourTurnPlayTime;
					Board.Instance.eventIPlayACard += controlYourTurnPlayTime;
				} else {
					Board.Instance.eventGameStarted -= startPlayTime;
					Board.Instance.eventSomeonePlayACard -= controlYourTurnPlayTime;
					Board.Instance.eventIPlayACard -= controlYourTurnPlayTime;
				}
			}
		}


		public void controlYourTurnPlayTime (Move move)
		{
			if (!Board.Instance.isGameFinish && Board.Instance.ActivePlayer == me)
				Board.Instance.PlayACard (me, playACard ());

			if (Board.Instance.getPlayerHand (me).Count == 0)
				Active = false;

		}

		public void startPlayTime ()
		{
			Board.Instance.eventSomeonePlayACard += controlYourTurnPlayTime;
			Board.Instance.eventIPlayACard += controlYourTurnPlayTime;
			_active = true;
			setup ();

			if (!Board.Instance.isGameFinish && Board.Instance.ActivePlayer == me)
				Board.Instance.PlayACard (me, playACard ());
		}


		public IAIPlayTime (Player me) : base (me)
		{
			Active = false;
			Board.Instance.eventGameStarted += startPlayTime;
		}



	}
}

