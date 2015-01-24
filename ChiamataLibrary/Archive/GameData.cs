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
				if (p.Role == EnRole.SOCIO)
					return p;

			throw new Exception ("Chiamata in mano");
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
			else if (player.Role == EnRole.CHIAMANTE)
				award = 2;
			else
				award = 1;

			if (!w.Contains (player)) {
				award = -award;
			}
				
			award = award * ( 1 + ( ( winningPoint - 60 ) / 10 ) + ( isCapotto ? 1 : 0 ) );

			return award;
		}

		public int getAward (int i)
		{
			return getAward (_players [i]);
		}

		#endregion

		#region WriteXML

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
			writer.WriteAttributeString ("WinningPoint", winningPoint.ToString ());

			writer.WriteStartElement ("Players");

			for (int i = 0; i < Board.PLAYER_NUMBER; i++) {
				writer.WriteStartElement ("Player");

				writer.WriteStartElement ("Name");
				writer.WriteString (_players [i].name);
				writer.WriteEndElement ();	//name

				writer.WriteStartElement ("Role");
				writer.WriteString (_players [i].Role.ToString ());
				writer.WriteEndElement ();	//role

				writer.WriteStartElement ("Order");
				writer.WriteString (_players [i].order.ToString ());
				writer.WriteEndElement ();	//order

				writer.WriteEndElement ();	//player
			}

			writer.WriteEndElement ();	//end player

			writer.WriteStartElement ("BidList");
			writer.WriteAttributeString ("Number", _bids.Count.ToString ());

			foreach (IBid bid in _bids) {
				writer.WriteStartElement ("Bid");

				if (bid is PassBid) {
					writer.WriteAttributeString ("Type", "Pass");

					writer.WriteStartElement ("Bidder");
					writer.WriteString (bid.bidder.order.ToString ());
					writer.WriteEndElement ();	//bidder
				} else if (bid is CarichiBid) {
					writer.WriteAttributeString ("Type", "Carichi");

					writer.WriteStartElement ("Bidder");
					writer.WriteString (bid.bidder.order.ToString ());
					writer.WriteEndElement ();	//bidder

					writer.WriteStartElement ("Point");
					writer.WriteString (( (NotPassBid) bid ).point.ToString ());
					writer.WriteEndElement ();	//point
				} else if (bid is NormalBid) {
					writer.WriteAttributeString ("Type", "Normal");

					writer.WriteStartElement ("Bidder");
					writer.WriteString (bid.bidder.order.ToString ());
					writer.WriteEndElement ();	//bidder

					writer.WriteStartElement ("Point");
					writer.WriteString (( (NotPassBid) bid ).point.ToString ());
					writer.WriteEndElement ();	//point

					writer.WriteStartElement ("Number");
					writer.WriteString (( (NormalBid) bid ).number.ToString ());
					writer.WriteEndElement ();	//number7
				}
					
				writer.WriteEndElement ();	//bid
			}


			writer.WriteEndElement ();	//bid list

			writer.WriteStartElement ("Cards");

			for (int seme = 0; seme < Board.Instance.nSemi; seme++)
				for (int number = 0; number < Board.Instance.nNumber; number++) {
					writer.WriteStartElement ("Card");
					writer.WriteAttributeString ("Seme", ( (EnSemi) seme ).ToString ());
					writer.WriteAttributeString ("Number", ( (EnNumbers) number ).ToString ());
					writer.WriteAttributeString ("CalledCard", ( calledCard == _cards [seme, number] ).ToString ());

					writer.WriteStartElement ("InitialPlayer");
					writer.WriteString (_cards [seme, number].initialPlayer.order.ToString ());
					writer.WriteEndElement ();	//initialPlayer

					writer.WriteStartElement ("FinalPlayer");
					writer.WriteString (_cards [seme, number].FinalPlayer.order.ToString ());
					writer.WriteEndElement ();	//initialPlayer

					writer.WriteStartElement ("PlayingTime");
					writer.WriteString (_cards [seme, number].PlayingTime.ToString ());
					writer.WriteEndElement ();	//Playing time

					writer.WriteEndElement ();	//Card
				}

			writer.WriteEndElement ();	//end card

			writer.WriteEndElement ();	//end game

			writer.WriteEndDocument ();
			writer.Close ();
		}

		#endregion

		#region ReadXML

		public GameData (string path)
		{
			Board.Instance.reset ();
			_players = new Player[Board.PLAYER_NUMBER];
			_cards = new Card[Board.Instance.nSemi, Board.Instance.nNumber];
			_bids = new List<IBid> ();

			//create the xml reader
			XmlReaderSettings setting = new XmlReaderSettings ();
			setting.IgnoreComments = true;

			XmlReader reader = XmlReader.Create (path, setting);

			reader.ReadToFollowing ("Game");	//game

			reader.MoveToFirstAttribute ();	//datetime
			this.time = DateTime.Parse (reader.Value.ToString ());

			reader.MoveToNextAttribute ();	//gametype
			this.gameType = (EnGameType) Enum.Parse (typeof (EnGameType), reader.Value);

			reader.MoveToNextAttribute ();	//winningPoint
			this.winningPoint = int.Parse (reader.Value);

			//Players
			reader.ReadToFollowing ("Players");
			for (int i = 0; i < Board.PLAYER_NUMBER; i++) {
				reader.ReadToFollowing ("Player");

				reader.ReadToFollowing ("Name");	//name
				string name = reader.ReadElementContentAsString ();

				reader.ReadToFollowing ("Role");	//role
				EnRole role = (EnRole) Enum.Parse (typeof (EnRole), reader.ReadElementContentAsString ());

				reader.ReadToFollowing ("Order");	//order

				int order = reader.ReadElementContentAsInt ();

				_players [i] = new Player (name, order);
				_players [i].Role = role;
			}

			//bids
			reader.ReadToFollowing ("BidList");

			reader.MoveToFirstAttribute ();	//bid's number
			int nBids = int.Parse (reader.Value);

			for (int i = 0; i < nBids; i++) {
				reader.ReadToFollowing ("Bid");

				reader.MoveToFirstAttribute ();	//bid's type
				string type = reader.Value;

				reader.ReadToFollowing ("Bidder");	//bidder

				int bidder = reader.ReadElementContentAsInt ();

				if (type == "Pass")
					_bids.Add (new PassBid (_players [bidder]));
				else {
					reader.ReadToFollowing ("Point");	//point

					int point = reader.ReadElementContentAsInt ();

					if (type == "Carichi")
						_bids.Add (new CarichiBid (_players [bidder], point));
					else {

						reader.ReadToFollowing ("Number");	//number
						EnNumbers number = (EnNumbers) Enum.Parse (typeof (EnNumbers), reader.ReadElementContentAsString ());

						_bids.Add (new NormalBid (_players [bidder], number, point));
					}
				}
			}

			//Cards
			reader.ReadToFollowing ("Cards");

			for (int seme = 0; seme < Board.Instance.nSemi; seme++)
				for (int number = 0; number < Board.Instance.nNumber; number++) {
					reader.ReadToFollowing ("Card");
					reader.MoveToFirstAttribute ();	//seme
					reader.MoveToNextAttribute ();	//number
					reader.MoveToNextAttribute ();	//called card
					bool cc = bool.Parse (reader.Value);

					reader.ReadToFollowing ("InitialPlayer");	
					Player ip = _players [reader.ReadElementContentAsInt ()];

					_cards [seme, number] = new Card ((EnNumbers) number, (EnSemi) seme, ip);

					reader.ReadToFollowing ("FinalPlayer");	
					Player fp = _players [reader.ReadElementContentAsInt ()];

					reader.ReadToFollowing ("PlayingTime");	
					int tp = reader.ReadElementContentAsInt ();

					_cards [seme, number].PlayingTime = tp;
					_cards [seme, number].FinalPlayer = fp;

					if (cc)
						calledCard = _cards [seme, number];

				}

			reader.Close ();

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

