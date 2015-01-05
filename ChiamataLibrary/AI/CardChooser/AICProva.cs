using System;

namespace ChiamataLibrary
{
	public class AICProva:IAICardChooser
	{
		private IAICardChooser _cardChooser;

		private Player _me;

		public AICProva ()
		{

		}

		public Card chooseCard ()
		{
			return _cardChooser.chooseCard ();
		}

		public void setup (Player me)
		{
			this._me = me;
			if (me.Role == EnRole.CHIAMANTE)
				_cardChooser = new AICChiamanteTakeAll (10, 20, 5);
			else if (me.Role == EnRole.SOCIO)
				_cardChooser = new AICSocioNascosto ();
			else
				_cardChooser = new AICAltriCargaOsti (10, 20);

			_cardChooser.setup (me);

		}

	}
}

