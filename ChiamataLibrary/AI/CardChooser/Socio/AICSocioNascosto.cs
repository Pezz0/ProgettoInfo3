using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// AI for play time, for players that are in the SOCIO role.
	/// </summary>
	public class AICSocioNascosto:IAICardChooser
	{

		/// <summary>
		/// The <see cref="ChiamataLibrary.Player"/> instance representing the AI.
		/// </summary>
		private Player _me;
		/// <summary>
		/// Integer representing the distance between this player and the player in the CHIAMANTE role.
		/// </summary>
		private int _deltaChiamante;

		/// <summary>
		/// Method that returns which card the AI wants to play.
		/// </summary>
		/// <returns>The card.</returns>
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

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="me">The <see cref="ChiamataLibrary.Player"/> instance representing the AI.</param>
		public void setup (Player me)
		{
			this._me = me;
			_deltaChiamante = ( me.order - Board.Instance.getChiamante ().order + Board.PLAYER_NUMBER ) % Board.PLAYER_NUMBER;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.AICSocioNascosto"/> class.
		/// </summary>
		public AICSocioNascosto ()
		{
		}
	}
}

