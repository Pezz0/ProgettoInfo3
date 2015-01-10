using System;

namespace ChiamataLibrary
{
	public class AICAltriCargaOsti:IAICardChooser
	{
		private Player _me;
		private int _deltaChiamante;
		private readonly int _thresholdL;
		private readonly int _thresholdH;

		public Card chooseCard ()
		{
			Card temp;
			int boardValue = Board.Instance.ValueOnBoard;
			int o = _deltaChiamante + ( Board.Instance.getChiamante ().Hand.Count == _me.Hand.Count ? -5 : 0 );
			switch (o) {
				case -4:
				case -3:
					return _me.getScartino ();
				
				case -2:
				case -1:
					if (Board.Instance.CardOnTheBoard.Exists (delegate (Card c) {
						return c.seme == Board.Instance.CalledCard.seme;
					})) {
						return _me.getScartino ();
					} else {
						temp = _me.getStrozzoBasso ();

						if (temp == null)
							return _me.getScartino ();
						else
							return temp;
					}
				
				case 1:
				case 2:
					temp = _me.getCarico ();
					if (temp == null)
						return _me.getScartino ();
					else
						return temp;
				
				case 3:
					if (boardValue < _thresholdL) {
						temp = _me.getStrozzoBasso ();
						if (temp == null)
							return _me.getScartino ();
						else
							return temp;
					} else if (boardValue < _thresholdH) {
						temp = _me.getBriscolaNotCarico ();
					} else
						temp = _me.getBriscolaCarico ();

					if (temp != null && ( Board.Instance.CardOnTheBoard.FindAll (delegate (Card c) {
						return c.seme == Board.Instance.CalledCard.seme;
					}).TrueForAll (delegate (Card c) {
						return temp >= c;
					}) ))
						return temp;
					else
						return _me.getScartino ();
						
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

						if (boardValue < _thresholdL) {
							temp = _me.getStrozzoBasso ();
							if (temp == null)
								return _me.getScartino ();
							else
								return temp;
						} else if (boardValue < _thresholdH) {
							temp = _me.getBriscolaNotCarico ();
						} else
							temp = _me.getBriscolaCarico ();

						if (temp != null && ( Board.Instance.CardOnTheBoard.FindAll (delegate (Card c) {
							return c.seme == Board.Instance.CalledCard.seme;
						}).TrueForAll (delegate (Card c) {
							return temp >= c;
						}) ))
							return temp;
						else
							return _me.getScartino ();
					}
			}

			throw new Exception ("some errore occur");
		}

		public void setup (Player me)
		{
			this._me = me;
			_deltaChiamante = ( me.order - Board.Instance.getChiamante ().order + Board.PLAYER_NUMBER ) % Board.PLAYER_NUMBER;
		}


		public AICAltriCargaOsti (int thresholdL, int thresholdH)
		{
			this._thresholdL = thresholdL;
			this._thresholdH = thresholdH;
		}
	}
}

