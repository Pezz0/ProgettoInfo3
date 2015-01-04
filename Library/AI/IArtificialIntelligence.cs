using System;
using System.Collections.Generic;

namespace ChiamataLibrary
{
	public abstract class IArtificialIntelligence
	{
		public readonly Player me;

		public abstract bool Active{ get; set; }

		protected virtual void setup ()
		{
		}

		public IArtificialIntelligence (Player me)
		{
			this.me = me;
		}

		protected Card getCarico (List<Card> mano)
		{
			Card temp;
			List<Card> listCarichi = null;
			int i = 9;
			while (listCarichi.Count == 0 && i > 4) {


				listCarichi = mano.FindAll (delegate(Card c) {
					return c.seme != Board.Instance.CalledCard.seme && c.number == (EnNumbers) i;
				});

				i--;
			}
			temp = listCarichi.Find (delegate (Card c) {
				return c.seme == Board.Instance.CardOnTheBoard [0].seme;
			});
			if (temp == null)
				return listCarichi [0];
			else
				return temp;

		}

		protected Card getScartino (List<Card> mano)
		{
			Card temp = mano [0];

			for (int i = 1; i < mano.Count; i++)
				if (mano [i] < temp)
					temp = mano [i];

			return temp;
		}

		protected List<Card> getBriscole (List<Card> mano)
		{
			List<Card> temp = new List<Card> ();
			mano.ForEach (delegate (Card c) {
				if (c.seme == Board.Instance.CalledCard.seme)
					temp.Add (c);
			});
			return temp;
		}

		protected Card getStrozzoBasso (List<Card> mano)
		{
			List<Card> temp = mano.FindAll (delegate (Card c) {
				return c > Board.Instance.CardOnTheBoard [Board.Instance.numberOfCardOnBoard - 1] && c.getPoint () < 10;
			});
			if (temp.Count == 0)
				return null;
			else
				return new SortedSet<Card> (temp).Min;
		}

		protected Card getStrozzoAlto (List<Card> mano)
		{
			Card temp = null;
			int i = 9;
			while (temp == null) {


				temp = mano.Find (delegate(Card c) {
					return c.seme != Board.Instance.CalledCard.seme && c.number == (EnNumbers) i && c.seme == Board.Instance.CardOnTheBoard [0].seme;
				});

				i--;
			}
			return temp;
		}

		protected Card getBriscolaNotCarico (List<Card> mano)
		{
			List<Card> temp = mano.FindAll (delegate (Card c) {
				return c.seme == Board.Instance.CalledCard.seme && c.getPoint () < 10;
			});
			if (temp.Count == 0)
				return null;
			else
				return temp [temp.Count - 1];
		}

		protected Card getBriscolaCarico (List<Card> mano)
		{
			List<Card> temp = getBriscole (mano);
			return temp [temp.Count - 1];

		}

		protected Card getVestita (List<Card> mano)
		{
			Card vestita = null;
			int i = 7;
			while (vestita == null && i >= 0) {
				vestita = mano.Find (delegate(Card c) {
					return c.seme != Board.Instance.CalledCard.seme && c.number == (EnNumbers) i;
				});

				i--;
			}

			return vestita;
		}

		protected Card getStrozzoBassoNB (List<Card> mano)
		{

			List<Card> temp = mano.FindAll (delegate(Card c) {
				return c.seme != Board.Instance.CalledCard.seme && c.seme == Board.Instance.CardOnTheBoard [0].seme && c.getPoint () < 10;
			});
			if (temp.Count == 0)
				return null;
			else
				return new SortedSet<Card> (temp).Max;
		}

	}
}

