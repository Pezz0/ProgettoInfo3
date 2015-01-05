using System;

namespace ChiamataLibrary
{
	public class AICSocioNascosto:IAICardChooser
	{

		private Player _me;
		private int _deltaChiamante;

		public Card chooseCard ()
		{
			Card temp;
			int o = _deltaChiamante + ( Board.Instance.getChiamante ().Hand.Count == _me.Hand.Count ? -5 : 0 );
			switch (o) {
				case -4:
					Card vestita = _me.getVestita ();
					if (vestita == null)
						return _me.getScartino ();
					else
						return vestita;


				case -3:
				case -2:
					Card strozzo = _me.getStrozzoBasso (false);
					if (strozzo == null) {
						vestita = _me.getVestita ();
						if (vestita == null)
							return _me.getScartino ();
						else
							return vestita;
					} else
						return strozzo;


				case -1:
					return _me.getScartino ();
				
				case 1:
					temp = _me.getStrozzoBasso (false);
					if (temp == null)
						return _me.getScartino ();
					return temp;
				
				case 2:
					return _me.getScartino ();
				
				case 3:
					strozzo = _me.getStrozzoBasso (false);
					if (strozzo == null)
						return _me.getScartino ();
					else
						return strozzo;
				
				case 4:
					if (!Board.Instance.CardOnTheBoard.Exists (delegate (Card c) {
						return c.seme == Board.Instance.CalledCard.seme;
					})) {
						temp = _me.getStrozzoAlto ();
						if (temp == null)
							return _me.getScartino ();
						else
							return temp;
					} else {
						temp = _me.getVestita ();
						if (temp == null)
							return _me.getScartino ();
						else
							return temp;
					}
			}

			throw new Exception ("some errore occur");
		}

		public void setup (Player me)
		{
			this._me = me;
			_deltaChiamante = ( me.order - Board.Instance.getChiamante ().order + Board.PLAYER_NUMBER ) % Board.PLAYER_NUMBER;
		}


		public AICSocioNascosto ()
		{
		}
	}
}

