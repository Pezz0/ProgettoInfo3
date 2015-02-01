using System;
using ChiamataLibrary;

namespace AILibrary
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
		public Card ChooseCard ()
		{
			int o = _deltaChiamante + ( Board.Instance.GetChiamante ().GetHand ().Count == _me.GetHand ().Count ? -5 : 0 );
			switch (o) {
				case -4:
					return _me.GetVestita ();

				case -3:
				case -2:
					Card strozzo = _me.GetStrozzoBasso (false);
					if (strozzo == null)
						return _me.GetVestita ();
					else
						return strozzo;

				case -1:
					return null;
				
				case 1:
					return _me.GetStrozzoBasso (false);
				
				case 2:
					return null;
				
				case 3:
					return _me.GetStrozzoBasso (false);
				
				case 4:

					foreach (Card c in Board.Instance.CardOnTheBoard)
						if (c.IsBiscrola)
							return  _me.GetVestita ();

					return _me.GetStrozzoAlto ();

			}

			throw new Exception ("some errore occur");
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="me">The <see cref="ChiamataLibrary.Player"/> instance representing the AI.</param>
		public void Setup (Player me)
		{
			this._me = me;
			_deltaChiamante = ( me.order - Board.Instance.GetChiamante ().order + Board.PLAYER_NUMBER ) % Board.PLAYER_NUMBER;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.AICSocioNascosto"/> class.
		/// </summary>
		public AICSocioNascosto ()
		{
		}
	}
}

