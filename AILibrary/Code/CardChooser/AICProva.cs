using System;
using ChiamataLibrary;

namespace AILibrary
{
	/// <summary>
	/// Test AI for play time.
	/// </summary>
	public class AICProva:IAICardChooser
	{
		/// <summary>
		/// The card chooser.
		/// </summary>
		private IAICardChooser _cardChooser;
		/// <summary>
		/// The <see cref="ChiamataLibrary.Player"/> instance representing the AI.
		/// </summary>
		private Player _me;

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.AICProva"/> class.
		/// </summary>
		public AICProva ()
		{

		}

		/// <summary>
		/// Method that returns which card the AI wants to play.
		/// </summary>
		/// <returns>The card.</returns>
		public Card ChooseCard ()
		{
			return _cardChooser.ChooseCard ();
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="me">The <see cref="ChiamataLibrary.Player"/> instance representing the AI.</param>
		public void Setup (Player me)
		{
			this._me = me;
			if (me.Role == EnRole.CHIAMANTE)
				_cardChooser = new AICChiamanteTakeAll (10, 25, 3);
			else if (me.Role == EnRole.SOCIO)
				_cardChooser = new AICSocioNascosto ();
			else
				_cardChooser = new AICAltriCargaOsti (10, 20);

			_cardChooser.Setup (me);

		}

	}
}

