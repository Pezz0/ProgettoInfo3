using System;
using System.Collections.Generic;
using MyRandom;

namespace ChiamataLibrary
{
	/// <summary>
	/// This class rapresent the board used for this game
	/// </summary>
	public sealed class Board:IBTSendable<Board>
	{
		public const int PLAYER_NUMBER = 5;
		public const int MAX_NAME_LENGHT = 10;

		#region Singleton implementation

		private static readonly Board _instance = new Board ();

		public static Board Instance{ get { return _instance; } }

		static Board ()
		{
		}

		private Board ()
		{
		}

		#endregion

		#region Time management

		/// <summary>
		/// Variable that rappresent the current discrete time
		/// 	-2 = creation time
		/// 	-1 = auction time
		/// 	 0 = first play
		/// 	...
		///  	 4 = last play of the first turn
		/// 	 5 = first play of the second turn
		/// 	...
		/// 	 39 = last play
		/// 	 40 = point counting e conclusion
		/// </summary>
		private int _t = -2;

		/// <summary>
		/// Gets a value indicating whether this <see cref="Engine.Board"/> is creating the cards and players.
		/// </summary>
		/// <value><c>true</c> if the board is creating; otherwise, <c>false</c>.</value>
		public bool isCreationPhase{ get { return _t == -2; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="Engine.Board"/> is the time for the auction.
		/// </summary>
		/// <value><c>true</c> ifis the time for the auction; otherwise, <c>false</c>.</value>
		public bool isAuctionPhase{ get { return _t == -1; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="Engine.Board"/> is play time.
		/// </summary>
		/// <value><c>true</c> if is play time; otherwise, <c>false</c>.</value>
		public bool isPlayTime{ get { return _t >= 0; } }

		/// <summary>
		/// Gets the current turn.
		/// </summary>
		/// <value>The current turn.</value>
		public int Turn {
			get {
				if (!isPlayTime)
					throw new WrongPhaseException ("The turn doesn't exist outside the play time", "Playtime");

				return _t / PLAYER_NUMBER;
			}
		}

		/// <summary>
		/// Gets the current discrete time.
		/// </summary>
		/// <value>The time.</value>
		public int Time { get { return _t; } }

		#endregion

		#region Card management

		/// <summary>
		/// The card grid.
		/// </summary>
		private readonly Card [,] _cardGrid = new Card[Enum.GetValues (typeof (EnSemi)).GetLength (0), Enum.GetValues (typeof (EnNumbers)).GetLength (0)];

		/// <summary>
		/// Gets the card.
		/// </summary>
		/// <returns>The card.</returns>
		/// <param name="seme">Seme.</param>
		/// <param name="number">Number.</param>
		public Card getCard (EnSemi seme, EnNumbers number)
		{
			return _cardGrid [(int) seme, (int) number];
		}

		/// <summary>
		/// Gets the card.
		/// </summary>
		/// <returns>The card.</returns>
		/// <param name="bytes">the bytes array that rapresent the class.</param>
		public Card getCard (byte [] bytes)
		{
			return _cardGrid [0, 0].ricreateFromByteArray (bytes);
		}

		/// <summary>
		/// Gets the card from a byte.
		/// </summary>
		/// <returns>The card.</returns>
		/// <param name="bytes">Bytes.</param>
		public Card getCard (byte bytes)
		{
			return getCard (new Byte[1]{ bytes });
		}

		/// <summary>
		/// Gets the hand of a player.
		/// </summary>
		/// <returns>The hand.</returns>
		/// <param name="player">Player.</param>
		public List<Card> getPlayerHand (Player player)
		{
			List<Card> cl = new List<Card> ();
			foreach (Card c in _cardGrid)
				if (c.initialPlayer == player && c.isPlayable)
					cl.Add (c);

			return cl;
		}

		/// <summary>
		/// Gets current chiamante's point count.
		/// </summary>
		/// <returns>The current chiamante's point count.</returns>
		public int getChiamantePointCount ()
		{
			int count = 0;
			foreach (Card c in _cardGrid)
				if (( c.FinalPlayer.Role == EnRole.CHIAMANTE || c.FinalPlayer.Role == EnRole.SOCIO ) && !c.isPlayable)
					count = count + c.getPoint ();

			return count;
		}

		/// <summary>
		/// Gets current altri's point count.
		/// </summary>
		/// <returns>The current chiamante's point count.</returns>
		public int getAltriPointCount ()
		{
			int count = 0;
			foreach (Card c in _cardGrid)
				if (c.FinalPlayer.Role == EnRole.ALTRO && !c.isPlayable)
					count = count + c.getPoint ();

			return count;
		}

		#endregion

		#region Players management

		/// <summary>
		/// The array of players.
		/// </summary>
		private readonly Player [] _players = new Player[PLAYER_NUMBER];

		/// <summary>
		/// That order of the player on this device.
		/// </summary>
		private int _me;

		/// <summary>
		/// Gets the player on this device.
		/// </summary>
		/// <value>Me.</value>
		public Player Me{ get { return _players [_me]; } }

		/// <summary>
		/// Gets all players.
		/// </summary>
		/// <value>All players.</value>
		public List<Player> AllPlayers{ get { return new List<Player> (_players); } }

		/// <summary>
		/// Gets the player from his order.
		/// </summary>
		/// <returns>The player.</returns>
		/// <param name="order">Order.</param>
		public Player getPlayer (int order)
		{
			return _players [order];
		}

		/// <summary>
		/// Gets the player from a byte array.
		/// </summary>
		/// <returns>The player.</returns>
		/// <param name="bytes">The Bytes' array.</param>
		public Player getPlayer (Byte [] bytes)
		{
			return _players [0].ricreateFromByteArray (bytes);
		}

		/// <summary>
		/// Gets the player from a byte.
		/// </summary>
		/// <returns>The player.</returns>
		/// <param name="bytes">Bytes.</param>
		public Player getPlayer (Byte  bytes)
		{
			return getPlayer (new Byte[1]{ bytes });
		}

		/// <summary>
		/// Gets the player chiamante.
		/// </summary>
		/// <value>The player chiamante.</value>
		public Player PlayerChiamante {
			get {
				if (!isPlayTime)
					throw new WrongPhaseException ("Roles not assigned yet", "Playtime");

				foreach (Player p in _players)
					if (p.Role == EnRole.CHIAMANTE)
						return p;

				throw new Exception ("Some error occur, this path shoudn't be executed");
			}
		}

		/// <summary>
		/// Gets the player socio.
		/// </summary>
		/// <value>The player socio.</value>
		public Player PlayerSocio {
			get {
				if (!isPlayTime)
					throw new WrongPhaseException ("Roles not assigned yet", "Playtime");

				foreach (Player p in _players)
					if (p.Role == EnRole.SOCIO)
						return p;
				throw new Exception ("Some error occur, this path shoudn't be executed");
			}
		}

		/// <summary>
		/// Gets the player altri.
		/// </summary>
		/// <value>The player altri.</value>
		public List<Player> PlayerAltri {
			get {
				if (!isPlayTime)
					throw new WrongPhaseException ("Roles not assigned yet", "Playtime");

				List<Player> pl = new List<Player> ();

				foreach (Player p in _players)
					if (p.Role == EnRole.ALTRO)
						pl.Add (p);

				return pl;
			}
		}

		#endregion

		#region Auction management

		/// <summary>
		/// The bid list.
		/// </summary>
		private List<IBid> _bidList;

		#region Status control

		/// <summary>
		/// Control if a player have passed in the auction phase
		/// </summary>
		/// <returns><c>true</c>, if the player passed, <c>false</c> otherwise.</returns>
		/// <param name="p">the player.</param>
		public bool isPlayerPassed (Player player)
		{
			if (!isAuctionPhase || isAuctionClosed)
				throw new WrongPhaseException ("This information isn't relevant outside the open auction", "Open auction");

			return _bidList.Exists (delegate(IBid bid) {
				return  bid is PassBid && bid.bidder == player;
			});
		}

		/// <summary>
		/// Gets the current auction winning bid.
		/// </summary>
		/// <value>The current auction winning bid. If null all the player have passed for now</value>
		public IBid currentAuctionWinningBid {
			get {
				if (!isAuctionPhase)
					throw new WrongPhaseException ("This information isn't relevant outside the auction", "Open/closed auction");
					
				return _bidList.FindLast (delegate(IBid bid) {
					return !( bid is PassBid );
				});
			}
		}


		/// <summary>
		/// Gets a value indicating whether the auction is closed.
		/// </summary>
		/// <value><c>true</c> if is auction closed; otherwise, <c>false</c>.</value>
		public bool isAuctionClosed {
			get {
				if (!isAuctionPhase)
					throw new WrongPhaseException ("This information isn't relevant outside the auction", "Open auction");
					
				List<IBid> passedBid = _bidList.FindAll (delegate(IBid bid) {
					return  bid is PassBid;
				});

				//if PLAYER_NUMBER - 1(4) players have passed then someone win e the auction is closed
				return passedBid.Count >= PLAYER_NUMBER - 1 && _bidList.Count >= PLAYER_NUMBER;
			}
		}

		/// <summary>
		/// Gets the player that have to do a bid or pass.
		/// </summary>
		/// <value>The the player that have to do a bid or pass.</value>
		public Player ActiveAuctionPlayer {
			get {
				if (!isAuctionPhase || isAuctionClosed)
					throw new WrongPhaseException ("This information isn't relevant outside the open auction", "Open auction");

				if (_bidList.Count == 0)
					return _players [_lastWinner + 1];
					
				IBid wb = currentAuctionWinningBid;
				if (wb == null)
					return _players [( _lastWinner + 1 + _bidList.Count ) % PLAYER_NUMBER];

				int active = ( wb.bidder.order + 1 ) % PLAYER_NUMBER;

				while (isPlayerPassed (_players [active]))
					active = ( active + 1 ) % PLAYER_NUMBER;

				return _players [active];

			}

		}

		#endregion

		#region Changing status

		/// <summary>
		/// Event handler place A bid.
		/// </summary>
		public delegate void eventHandlerPlaceABid (IBid bid);

		/// <summary>
		/// Occurs when event I place A bid.
		/// </summary>
		public event eventHandlerPlaceABid eventIPlaceABid;

		/// <summary>
		/// Occurs when event someone place A bid.
		/// </summary>
		public event eventHandlerPlaceABid eventSomeonePlaceABid;

		/// <summary>
		/// Method for placing a bid in the auction.
		/// </summary>
		/// <param name="bid">The bid</param>
		public void auctionPlaceABid (IBid bid)
		{
			if (!isAuctionPhase || isAuctionClosed)
				throw new WrongPhaseException ("A player can place a bid only during the auction phase, when is open", "Auction open");

			if (ActiveAuctionPlayer != bid.bidder)
				throw new WrongBiddingPlayerException ("This player cannot place a bid now", bid.bidder);

			if (bid is PassBid)
				_bidList.Add (bid);
			else if (!isBidBetter (bid))
				throw new BidNotEnoughException ("The new bid is not enough to beat the winning one", bid);
			else
				_bidList.Add (bid);

			if (eventIPlaceABid != null && bid.bidder == Me)
				eventIPlaceABid (bid);
				
			if (eventSomeonePlaceABid != null && bid.bidder != Me)
				eventSomeonePlaceABid (bid);

		}

		/// <summary>
		/// Method for placing a bid in the auction.
		/// </summary>
		/// <param name="bytes">The Bytes' array that rappresent a bid.</param>
		public void auctionPlaceABid (byte [] bytes)
		{
			auctionPlaceABid (new PassBid (_players [0]).ricreateFromByteArray (bytes));
		}

		/// <summary>
		/// Method for passing during the auction
		/// </summary>
		/// <param name="player">Player who want to pass.</param>
		public void auctionPass (Player player)
		{
			auctionPlaceABid ((IBid) new PassBid (player));
		}

		/// <summary>
		/// Return a value that indicate if the passed bid is bettere than the last one
		/// </summary>
		/// <returns><c>true</c>, if bid is better, <c>false</c> otherwise.</returns>
		/// <param name="nb">Nb.</param>
		public bool isBidBetter (IBid nb)
		{
			return nb > currentAuctionWinningBid;
		}

		/// <summary>
		///  Finalize the auction, set the called card, the players roles and start the playtime
		/// </summary>
		/// <param name="seme">Seme, if "chiamata a carichi" o "monte" thi parameter is avoidable.</param>
		public void finalizeAuction (EnSemi seme = EnSemi.COPE)
		{
			if (!isAuctionPhase && !isAuctionClosed)
				throw new WrongPhaseException ("The auction can be finalized only when the auction is close", "Auction closed");

			IBid wb = currentAuctionWinningBid;

			if (wb == null)
				_gameType = EnGameType.MONTE;
			else if (wb is BidCarichi) {	//carichi
				_gameType = EnGameType.CARICHI;

				wb.bidder.Role = EnRole.CHIAMANTE;

				_point = ( (BidCarichi) wb ).point;
			} else if (wb is NormalBid) {	//standard
				_gameType = EnGameType.STANDARD;

				_calledCard = getCard (seme, ( (NormalBid) wb ).number);

				_point = ( (NormalBid) wb ).point;

				//set the roles
				_calledCard.initialPlayer.Role = EnRole.SOCIO;
				currentAuctionWinningBid.bidder.Role = EnRole.CHIAMANTE;
			}
				
			//time for the first turn
			_t = 0;
		}


		#endregion

		#endregion

		#region Playtime management

		#region Win condition

		/// <summary>
		/// The type of the game.
		/// </summary>
		private EnGameType _gameType;

		/// <summary>
		/// Gets the type of the game.
		/// </summary>
		/// <value>The type of the game.</value>
		public EnGameType GameType { get { return _gameType; } }

		/// <summary>
		/// The necessary point to win.
		/// </summary>
		private int _point;

		/// <summary>
		/// Gets the winning point.
		/// </summary>
		/// <value>The winning point.</value>
		public int WinningPoint { get { return _point; } }

		/// <summary>
		/// The called card.
		/// </summary>
		private Card _calledCard;

		/// <summary>
		/// Gets the called card.
		/// </summary>
		/// <value>The called card.</value>
		public Card CalledCard { get { return _calledCard; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ChiamataLibrary.Board"/> is a chiamata in mano.
		/// </summary>
		/// <value><c>true</c> if is a chiamata in mano; otherwise, <c>false</c>.</value>
		public bool isChiamataInMano{ get { return _gameType == EnGameType.STANDARD && _calledCard.initialPlayer == PlayerChiamante; } }


		#endregion

		/// <summary>
		/// The index of the player who won the last turn.
		/// </summary>
		private int _lastWinner;

		/// <summary>
		/// Gets the last winner.
		/// </summary>
		/// <value>The last winner.</value>
		public Player LastWinner{ get { return _players [_lastWinner]; } }

		/// <summary>
		/// Gets the player that have to play.
		/// </summary>
		/// <value>The player that have to play.</value>
		public Player ActivePlayer{ get { return _players [( _lastWinner + _t ) % PLAYER_NUMBER]; } }

		private List<Card> _lastCycle = new List<Card> ();

		/// <summary>
		/// Gets the card on the board.
		/// </summary>
		/// <value>The card on the board.</value>
		public  List<Card> CardOnTheBoard { get { return _lastCycle; } }

		/// <summary>
		/// Event handler play A card.
		/// </summary>
		public delegate void eventHandlerPlayACard (Move move);

		/// <summary>
		/// Occurs when event I play A card.
		/// </summary>
		public event eventHandlerPlayACard eventIPlayACard;

		/// <summary>
		/// Occurs when event someone play A card.
		/// </summary>
		public event eventHandlerPlayACard eventSomeonePlayACard;


		/// <summary>
		/// A method that allow a player to play a card.
		/// </summary>
		/// <param name="move">The move.</param>
		public void PlayACard (Move move)
		{
			if (!isPlayTime)
				throw new WrongPhaseException ("A player can play a card if and only if during the playtime", "Playtime");

			if (ActivePlayer != move.player)
				throw new WrongPlayerException ("This player cannot play now", move.player);

			if (!move.card.isPlayable || move.card.initialPlayer != move.player)
				throw new WrongCardException ("This player cannot play this card", move.card);


			move.card.PlayingTime = _t;
			_lastCycle.Add (move.card);

			if (numberOfCardOnBoard == PLAYER_NUMBER - 1) {
				Card max = _lastCycle [0];
				for (int i = 1; i < PLAYER_NUMBER; i++)
					if (_lastCycle [i] > max)
						max = _lastCycle [i];

				_lastWinner = max.initialPlayer.order;

				_lastCycle.ForEach (delegate(Card c) {
					c.FinalPlayer = max.initialPlayer;
				});

				_lastCycle = new List<Card> ();
			}
			_t++;

			if (eventIPlayACard != null && move.player == Me)
				eventIPlayACard (move);

			if (eventSomeonePlaceABid != null && move.player != Me)
				eventSomeonePlayACard (move);

		}

		/// <summary>
		/// A method that allow a player to play a card.
		/// </summary>
		/// <param name="player">Player.</param>
		/// <param name="card">Card.</param>
		public void PlayACard (Player player, Card card)
		{
			PlayACard (new Move (card, player));
		}

		/// <summary>
		/// A method that allow a player to play a card.
		/// </summary>
		/// <param name="bytes">Bytes.</param>
		public void PlayACard (Byte [] bytes)
		{
			PlayACard (new Move ().ricreateFromByteArray (bytes));
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="ChiamataLibrary.Board"/> game is ended.
		/// </summary>
		/// <value><c>true</c> if this game is ended; otherwise, <c>false</c>.</value>
		public bool isGameFinish{ get { return _t == Enum.GetValues (typeof (EnSemi)).GetLength (0) * Enum.GetValues (typeof (EnNumbers)).GetLength (0); } }

		/// <summary>
		/// Gets the number of card on board.
		/// </summary>
		/// <value>The number of card on board.</value>
		public int numberOfCardOnBoard{ get { return _t % PLAYER_NUMBER; } }

		/// <summary>
		/// Gets the winner.
		/// </summary>
		/// <value>The winner.</value>
		public List<Player> Winner {
			get {
				List<Player> w = new List<Player> ();
				if (getChiamantePointCount () >= WinningPoint) {
					w.Add (PlayerChiamante);
					if (!isChiamataInMano)
						w.Add (PlayerSocio);
				} else
					w = PlayerAltri;

				return w;
			}
		}

		#endregion

		/// <summary>
		/// Initialize with the specified name's players e dealer.
		/// </summary>
		/// <param name="playerName">Player's name.</param>
		/// <param name="indexDealer">Index's dealer.</param>
		public void initializeMaster (string [] playerName, int indexDealer)
		{
			if (!isCreationPhase)
				throw new WrongPhaseException ("The board must be initialized during the creation phase", "Creation phase");

			if (playerName.GetLength (0) != PLAYER_NUMBER)
				throw new Exception ("The number of player must be " + PLAYER_NUMBER);

			_me = 0;	

			for (int i = 0; i < PLAYER_NUMBER; i++) {
				_players [i] = new Player (playerName [i], i);

				//add the player's name at the bytes array
				char [] n = playerName [i].ToCharArray ();
				for (int j = 0; j < MAX_NAME_LENGHT; j++)
					if (j < n.Length)
						_bytes.AddRange (BitConverter.GetBytes (n [j]));
					else
						_bytes.Add (0);
			}

			_lastWinner = indexDealer;	//the last winner is the player that have to play first in the next turn
			_bytes.Add (BitConverter.GetBytes (indexDealer) [0]);	//add the dealer at the bytes array

			int nSemi = Enum.GetValues (typeof (EnSemi)).GetLength (0);	//the number of semi
			int nNumbers = Enum.GetValues (typeof (EnNumbers)).GetLength (0);	//the number of numbers
			int nCard = nSemi * nNumbers;	//the numbers of card
			int nCardForPlayer = nCard / PLAYER_NUMBER;	//the number of card for player
		
			int [] cardAssign = { 0, 0, 0, 0, 0 };	//counter for the card distribution
			IRandomGenerator rand = new NormalRandom ();	//instantiate the random generator

			for (int i = 0; i < nSemi; i++)		//cycle all the possibible card
				for (int j = 0; j < nNumbers; j++) {
					int assignedPlayer = rand.getRandomNumber (PLAYER_NUMBER);	
					while (cardAssign [assignedPlayer] == nCardForPlayer)
						assignedPlayer = rand.getRandomNumber (PLAYER_NUMBER);	//continue to change the assigned player until isn't a full player

					_cardGrid [i, j] = new Card ((EnNumbers) j, (EnSemi) i, _players [assignedPlayer]);	//instantiate the card
					cardAssign [assignedPlayer]++;

					//add the assigned player to the bytes array
					_bytes.Add (BitConverter.GetBytes (assignedPlayer) [0]);
				}

			_t = -1;	//start the auction
			_bidList = new List<IBid> ();
		}

		#region Bluetooth

		private  List<Byte> _bytes = new List<byte> ();

		public byte[] toByteArray ()
		{
			return _bytes.ToArray ();
		}

		public Board ricreateFromByteArray (byte [] bytes)
		{
			if (!isCreationPhase)
				throw new WrongPhaseException ("The board must be initialized during the creation phase", "Creation phase");
		
			reset ();
			_bytes = new List<Byte> (bytes);
			int index = 0;

			for (int i = 0; i < PLAYER_NUMBER; i++) {
				char [] c = new char[MAX_NAME_LENGHT];
				for (int j = 0; j < MAX_NAME_LENGHT; j++) {
					c [j] = BitConverter.ToChar (new byte[]{ bytes [index] }, 0);
					index++;
				}
				_players [i] = new Player (new string (c), i);
			}

			_lastWinner = _bytes [index];	//the last winner is the player that have to play first in the next turn
			index++;
	
			int nSemi = Enum.GetValues (typeof (EnSemi)).GetLength (0);	//the number of semi
			int nNumbers = Enum.GetValues (typeof (EnNumbers)).GetLength (0);	//the number of numbers

			for (int i = 0; i < nSemi; i++)		//cycle all the possibible card
				for (int j = 0; j < nNumbers; j++) {

					int assignedPlayer = BitConverter.ToInt16 (new Byte[] { bytes [index], 0 }, 0);

					_cardGrid [i, j] = new Card ((EnNumbers) j, (EnSemi) i, _players [assignedPlayer]);	//instantiate the card
					index++;
				}

			_t = -1;	//start the auction
			_bidList = new List<IBid> ();

			return this;
		}

		public int ByteArrayLenght { get { return _bytes.Count; } }

		public void initializeSlave (string me)
		{
			foreach (Player p in _players)
				if (p.name == me) {
					_me = p.order;
					return;
				}
		}

		#endregion

		public void reset ()
		{
			_t = -2;
			//_instance = new Board ();
		}
	}
}

