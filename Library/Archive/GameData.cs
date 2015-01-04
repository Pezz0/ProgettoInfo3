using System;
using System.Collections.Generic;
using System.Xml;

namespace ChiamataLibrary
{
	public class GameData
	{
		public readonly DateTime time;

		#region Card

		private readonly Card [,] _cards;

		public Card getCard (EnSemi seme, EnNumbers number)
		{
			return _cards [(int) seme, (int) number];
		}

		#endregion

		#region Player

		private readonly Player [] _players;

		public Player getPlayer (int order)
		{
			return _players [order];
		}

		#region Team

		public Player getChiamante ()
		{
			foreach (Player p in _players)
				if (p.Role == EnRole.CHIAMANTE)
					return p;

			throw new Exception ("Some error occur, this path shoudn't be executed");
		}

		public Player getSocio ()
		{
			foreach (Player p in _players)
				if (p.Role == EnRole.CHIAMANTE)
					return p;

			throw new Exception ("Some error occur, this path shoudn't be executed");
		}

		public List<Player> getAltri ()
		{
			List<Player> pl = new List<Player> ();

			foreach (Player p in _players)
				if (p.Role == EnRole.ALTRO)
					pl.Add (p);

			return pl;
		}

		#endregion

		#endregion

		#region Auction

		private readonly List<IBid> _bids;

		#endregion

		#region Winning condition

		public readonly EnGameType gameType;
		public readonly Card calledCard;
		public readonly int winningPoint;

		public bool isChiamataInMano{ get { return gameType == EnGameType.STANDARD && calledCard.initialPlayer.Role == EnRole.CHIAMANTE; } }

		public bool isCapotto{ get { return getChiamantePointCount () % 121 == 0; } }

		public int getChiamantePointCount ()
		{
			int count = 0;
			foreach (Card c in _cards)
				if (( c.FinalPlayer.Role == EnRole.CHIAMANTE || c.FinalPlayer.Role == EnRole.SOCIO ) && !c.isPlayable)
					count = count + c.getPoint ();

			return count;
		}

		public int getAltriPointCount ()
		{
			int count = 0;
			foreach (Card c in _cards)
				if (c.FinalPlayer.Role == EnRole.ALTRO && !c.isPlayable)
					count = count + c.getPoint ();

			return count;
		}

		public List<Player> getWinners ()
		{
			List<Player> w = new List<Player> ();
			if (getChiamantePointCount () >= winningPoint) {
				w.Add (getChiamante ());
				if (!isChiamataInMano)
					w.Add (getSocio ());
			} else
				w = getAltri ();

			return w;
		}

		public int getAward (Player player)
		{
			List<Player> w = getWinners ();
			int award = 0;
			if (w.Count == 1)
				award = 4;
			else if (w.Count == 2 && player.Role == EnRole.CHIAMANTE)
				award = 2;
			else if (w.Count == 2 && player.Role == EnRole.SOCIO)
				award = 1;
			else
				award = 1;

			if (!w.Contains (player)) {
				int nLoser = Board.PLAYER_NUMBER - w.Count;
				award = -( award * w.Count ) / nLoser;
			}
				
			award = award * ( 1 + ( ( winningPoint - 60 ) / 10 ) + ( isCapotto ? 1 : 0 ) );

			return award;
		}

		public int getAward (int i)
		{
			return getAward (_players [i]);
		}

		#endregion

		#region XML

		private void writePlayer (int i, XmlWriter w)
		{
			w.WriteStartElement ("Player", _players [i].name);

			w.WriteStartElement ("Role");
			w.WriteString (_players [i].Role.ToString ());
			w.WriteEndElement ();	//role

			w.WriteStartElement ("Order");
			w.WriteString (_players [i].order.ToString ());
			w.WriteEndElement ();	//order

			w.WriteEndElement ();	//player
		}

		private void writeCard (int seme, int number, XmlWriter w)
		{
			w.WriteStartElement ("Card");
			w.WriteAttributeString ("Seme", ( (int) seme ).ToString ());
			w.WriteAttributeString ("Number", ( (int) seme ).ToString ());
			w.WriteAttributeString ("CalledCard", ( calledCard == _cards [seme, number] ).ToString ());

			w.WriteStartElement ("InitialPlayer");
			w.WriteString (_cards [seme, number].initialPlayer.name);
			w.WriteEndElement ();	//initialPlayer

			w.WriteStartElement ("FinalPlayer");
			w.WriteString (_cards [seme, number].FinalPlayer.name);
			w.WriteEndElement ();	//initialPlayer

			w.WriteStartElement ("Playing time");
			w.WriteString (_cards [seme, number].PlayingTime.ToString ());
			w.WriteEndElement ();	//Playing time

			w.WriteEndElement ();	//Card
		}

		public void writeOnXML (string path)
		{

			XmlWriterSettings setting = new XmlWriterSettings ();
			setting.Indent = true;
			setting.IndentChars = "\t";

			XmlWriter writer = XmlWriter.Create (path, setting);

			writer.WriteStartDocument ();

			writer.WriteComment ("This XML document contains the data for a game");

			writer.WriteStartElement ("Game");
			writer.WriteAttributeString ("DateTime", time.ToString ());
			writer.WriteAttributeString ("GameType", gameType.ToString ());

			writer.WriteStartElement ("Players");

			for (int i = 0; i < Board.PLAYER_NUMBER; i++)
				writePlayer (i, writer);

			writer.WriteEndElement ();	//end player

			writer.WriteStartElement ("Cards");

			for (int seme = 0; seme < Board.Instance.nSemi; seme++) {
				writer.WriteStartElement ("Seme", ( (int) seme ).ToString ());

				for (int number = 0; number < Board.Instance.nNumber; number++)
					writeCard (seme, number, writer);

				writer.WriteEndElement ();	//end seme
			}

			writer.WriteEndElement ();	//end card

			writer.WriteEndElement ();	//end game

			writer.WriteEndDocument ();
			writer.Close ();
		}

		#endregion


		public GameData (DateTime time, Card [,] cards, Player [] players, List<IBid> bids, EnGameType type, Card calledCard, int winningPoint)
		{
			this.time = time;
			this._cards = cards;
			this._players = players;
			this._bids = bids;
			this.gameType = type;
			this.calledCard = calledCard;
			this.winningPoint = winningPoint;
		}
	}
}

