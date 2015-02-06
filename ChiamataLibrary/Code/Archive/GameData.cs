using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;

namespace ChiamataLibrary
{
	/// <summary>
	/// Contains all the useful informations about a past game.
	/// </summary>
	public class GameData
	{
		/// <summary>
		/// The date on which the game was played.
		/// </summary>
		public readonly DateTime time;

		#region Card

		/// <summary>
		/// The matrix containing all the 40 instances of <see cref="ChiamataLibrary.Card"/>.
		/// </summary>
		private readonly Card [,] _cards = new Card[Board.Instance.nSemi, Board.Instance.nNumber];

		/// <summary>
		/// Getter for the card. Must provide SEME and NUMBER as arguments.
		/// </summary>
		/// <returns>The instance of <see cref="ChiamataLibrary.Card"/>.</returns>
		/// <param name="seme">Seme.</param>
		/// <param name="number">Number.</param>
		public Card GetCard (EnSemi seme, EnNumbers number)
		{
			return _cards [(int) seme, (int) number];
		}

		#endregion

		#region Player

		/// <summary>
		/// Array of <see cref="ChiamataLibrary.Player"/> that were in the game.
		/// </summary>
		private readonly Player [] _players = new Player[Board.PLAYER_NUMBER];

		/// <summary>
		/// Getter for the player. Must provide his hindex as argument.
		/// </summary>
		/// <returns>The instance of <see cref="ChiamataLibrary.Player"/>.</returns>
		/// <param name="order">The index of the instances of <see cref="ChiamataLibrary.Player"/>.</param>
		public Player GetPlayer (int order)
		{
			return _players [order];
		}

		#region Team

		/// <summary>
		/// Gets the <see cref="ChiamataLibrary.Player"/> in the CHIAMANTE role.
		/// </summary>
		/// <returns>The <see cref="ChiamataLibrary.Player"/> in the CHIAMANTE role.</returns>
		public Player GetChiamante ()
		{
			foreach (Player p in _players)
				if (p.Role == EnRole.CHIAMANTE)
					return p;

			throw new Exception ("Some error occur, this path shoudn't be executed");
		}

		/// <summary>
		/// Gets the <see cref="ChiamataLibrary.Player"/> in the SOCIO role.
		/// </summary>
		/// <returns>The <see cref="ChiamataLibrary.Player"/> in the SOCIO role.</returns>
		public Player GetSocio ()
		{
			foreach (Player p in _players)
				if (p.Role == EnRole.SOCIO)
					return p;

			throw new Exception ("Chiamata in mano");
		}

		/// <summary>
		/// Gets the list of <see cref="ChiamataLibrary.Player"/> in the ALTRI role.
		/// </summary>
		/// <returns>The list of <see cref="ChiamataLibrary.Player"/> in the ALTRI role.</returns>
		public List<Player> GetAltri ()
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

		/// <summary>
		/// The list of the bids put in the auction.
		/// </summary>
		private readonly List<BidBase> _bids = new List<BidBase> ();

		#endregion

		#region Winning condition

		/// <summary>
		/// The type of the game. See<see cref="ChiamataLibrary.EnGameType"/>for the various types of games.
		/// </summary>
		public readonly EnGameType gameType;
		/// <summary>
		/// The<see cref="ChiamataLibrary.Card"/>that has been called.
		/// </summary>
		public readonly Card calledCard;
		/// <summary>
		/// The points that the team composed of CHIAMANTE and SOCIO had to do in order to win the game.
		/// </summary>
		public readonly int winningPoint;

		/// <summary>
		/// Gets a value indicating whether this <see cref="ChiamataLibrary.GameData"/> is a chiamata in mano.
		/// </summary>
		/// <value><c>true</c> if is chiamata in mano; otherwise, <c>false</c>.</value>
		public bool IsChiamataInMano{ get { return gameType == EnGameType.STANDARD && calledCard.initialPlayer.Role == EnRole.CHIAMANTE; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ChiamataLibrary.GameData"/> is capotto.
		/// </summary>
		/// <value><c>true</c> if is capotto; otherwise, <c>false</c>.</value>
		public bool IsCapotto{ get { return GetChiamantePointCount () % 121 == 0; } }

		/// <summary>
		/// Gets the chiamante points.
		/// </summary>
		/// <returns>The chiamante points.</returns>
		public int GetChiamantePointCount ()
		{
			int count = 0;
			foreach (Card c in _cards)
				if (( c.FinalPlayer.Role == EnRole.CHIAMANTE || c.FinalPlayer.Role == EnRole.SOCIO ))
					count = count + c.GetPoint ();

			return count;
		}

		/// <summary>
		/// Gets the altri points.
		/// </summary>
		/// <returns>The altri points.</returns>
		public int GetAltriPointCount ()
		{
			int count = 0;
			foreach (Card c in _cards)
				if (c.FinalPlayer.Role == EnRole.ALTRO)
					count = count + c.GetPoint ();

			return count;
		}

		/// <summary>
		/// Gets the list of winning<see cref="ChiamataLibrary.Player"/>.
		/// </summary>
		/// <returns>The list of winners.</returns>
		public List<Player> GetWinners ()
		{
			List<Player> w = new List<Player> ();
			if (GetChiamantePointCount () >= winningPoint) {
				w.Add (GetChiamante ());
				if (!IsChiamataInMano)
					w.Add (GetSocio ());
			} else
				w = GetAltri ();

			return w;
		}

		/// <summary>
		/// Gets the award for the<see cref="ChiamataLibrary.Player"/>provided as argument.
		/// </summary>
		/// <returns>The award.</returns>
		/// <param name="player">The<see cref="ChiamataLibrary.Player"/>we want to calculate the award for.</param>
		public int GetAward (Player player)
		{
			List<Player> w = GetWinners ();
			int award = 0;

			if (w.Count == 1 && player.Role == EnRole.CHIAMANTE)
				award = 4;
			else if (player.Role == EnRole.CHIAMANTE)
				award = 2;
			else
				award = 1;

			if (!w.Contains (player))
				award = -award;
				
			award = award * ( 1 + ( ( winningPoint - 60 ) / 10 ) + ( IsCapotto ? 1 : 0 ) );

			return award;
		}

		/// <summary>
		/// Gets the award for the<see cref="ChiamataLibrary.Player"/>with the index provided as argument.
		/// </summary>
		/// <returns>The award.</returns>
		/// <param name="i">The index of the<see cref="ChiamataLibrary.Player"/>.</param>
		public int GetAward (int i)
		{
			return GetAward (_players [i]);
		}

		#endregion

		#region WriteXML

		/// <summary>
		/// Method that writes this <see cref="ChiamataLibrary.GameData"/> on an XML file.
		/// </summary>
		/// <param name="path">The path for the XML file.</param>
		internal void WriteOnXML (Stream s)
		{

			XmlWriterSettings setting = new XmlWriterSettings ();
			setting.Indent = true;
			setting.IndentChars = "\t";

			using (XmlWriter writer = XmlWriter.Create (s, setting)) {

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

				foreach (BidBase bid in _bids) {
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
						writer.WriteString (( (NotPassBidBase) bid ).point.ToString ());
						writer.WriteEndElement ();	//point
					} else if (bid is NormalBid) {
						writer.WriteAttributeString ("Type", "Normal");

						writer.WriteStartElement ("Bidder");
						writer.WriteString (bid.bidder.order.ToString ());
						writer.WriteEndElement ();	//bidder

						writer.WriteStartElement ("Point");
						writer.WriteString (( (NotPassBidBase) bid ).point.ToString ());
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
			}
		}

		#endregion

		#region ReadXML

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.GameData"/> class.
		/// </summary>
		/// <param name="path">The path of the XML file from which will be reading the <see cref="ChiamataLibrary.GameData"/>.</param>
		internal GameData (Stream s)
		{
			Board.Instance.Reset ();
			_players = new Player[Board.PLAYER_NUMBER];
			_cards = new Card[Board.Instance.nSemi, Board.Instance.nNumber];
			_bids = new List<BidBase> ();

			//create the xml reader
			XmlReaderSettings setting = new XmlReaderSettings ();
			setting.IgnoreComments = true;

			using (XmlReader reader = XmlReader.Create (s, setting)) {

				reader.ReadToFollowing ("Game");	//game

				reader.MoveToFirstAttribute ();	//datetime
				this.time = DateTime.Parse (reader.Value.ToString ());

				reader.MoveToNextAttribute ();	//gametype
				this.gameType = (EnGameType) Enum.Parse (typeof (EnGameType), reader.Value, true);

				reader.MoveToNextAttribute ();	//winningPoint
				this.winningPoint = int.Parse (reader.Value);

				//Players
				reader.ReadToFollowing ("Players");
				for (int i = 0; i < Board.PLAYER_NUMBER; i++) {
					reader.ReadToFollowing ("Player");

					reader.ReadToFollowing ("Name");	//name
					string name = reader.ReadElementContentAsString ();

					reader.ReadToFollowing ("Role");	//role
					EnRole role = (EnRole) Enum.Parse (typeof (EnRole), reader.ReadElementContentAsString (), true);

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
							EnNumbers number = (EnNumbers) Enum.Parse (typeof (EnNumbers), reader.ReadElementContentAsString (), true);

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
					
			}

		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.GameData"/> class.
		/// </summary>
		/// <param name="time">The date on which the game was played.</param>
		/// <param name="cards">The matrix of <see cref="ChiamataLibrary.Card"/> of the game.</param>
		/// <param name="players">The array of <see cref="ChiamataLibrary.Player"/>.</param>
		/// <param name="bids">The list of bids.</param>
		/// <param name="type">The type of game.</param>
		/// <param name="calledCard">The called <see cref="ChiamataLibrary.Card"/>.</param>
		/// <param name="winningPoint">The points that the team composed of CHIAMANTE and SOCIO had to do in order to win the game.</param>
		internal GameData (DateTime time, Card [,] cards, Player [] players, List<BidBase> bids, EnGameType type, Card calledCard, int winningPoint)
		{
			this.time = time;

			for (int i = 0; i < Board.Instance.nSemi; ++i)
				for (int j = 0; j < Board.Instance.nNumber; ++j)
					this._cards [i, j] = cards [i, j];

			for (int i = 0; i < Board.PLAYER_NUMBER; ++i)
				this._players [i] = players [i];

			this._bids = new List<BidBase> (bids);

			this.gameType = type;
			this.calledCard = calledCard;
			this.winningPoint = winningPoint;
		}
	}
}

