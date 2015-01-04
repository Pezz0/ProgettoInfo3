using System;

namespace ChiamataLibrary
{
	public class AIPtProva:IArtificialIntelligence
	{
		private IAIPlayTime playtime;

		public AIPtProva (Player me) : base (me)
		{

		}

		protected override void setup ()
		{
			if (me.Role == EnRole.CHIAMANTE)
				playtime = new AIPtCTakeAllV3 (me, 20, 10, 3);
			else if (me.Role == EnRole.ALTRO)
				playtime = new AIPtACargaOsti (me, 20, 10);
			else
				playtime = new AIPtSNascosto (me);


		}


		#region implemented abstract members of IArtificialIntelligence

		public override bool Active {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		#endregion
	}
}

