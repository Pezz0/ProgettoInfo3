using System;
using MyRandom;
using System.Collections.Generic;
using System.Text;

namespace ChiamataLibrary
{
	/// <summary>
	/// Class representing the game board. Everything game-related will go here.
	/// </summary>
	public class Board
	{
		/// <summary>
		/// The number of players.
		/// </summary>
		public const int PLAYER_NUMBER = 5;

		/// <summary>
		/// The number of semi.
		/// </summary>
		public readonly int nSemi = Enum.GetValues (typeof (EnSemi)).GetLength (0);

		/// <summary>
		/// The number of card numbers (usually 10in normal cards, 13 in french cards).
		/// </summary>
		public readonly int nNumber = Enum.GetValues (typeof (EnNumbers)).GetLength (0);

		#region Singleton implementation

		/// <summary>
		/// Instance of the <see cref="ChiamataLibrary.Board"/> singleton
		/// </summary>
		private static readonly Board _instance = new Board ();

		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		/// <value>The instance of the <see cref="ChiamataLibrary.Board"/>.</value>
		public static Board Instance{ get { return _instance; } }

		/// <summary>
		/// Initializes the <see cref="ChiamataLibrary.Board"/> class.
		/// </summary>
		static Board ()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.Board"/> class.
		/// </summary>
		private Board ()
		{
			_cardGrid = new Card[nSemi, nNumber];

			reset ();
		}

		/// <summary>
		/// Reset this instance, preparing for a new game.
		/// </summary>
		public void reset ()
		{
			eventSomeonePlaceABid = null;
			eventSomeonePlayACard = null;
			eventPickTheBoard = null;
			eventAuctionStart = null;
			eventPlaytimeStart = null;
			eventPlaytimeEnd = null;

			_listBid.Clear ();

			_t = -3;
		}

		#endregion

		#region Initilization

		/// <summary>
		/// Initializes the board for a master device.
		/// </summary>
		/// <param name="playerName">The Players' name.</param>
		/// <param name="indexDealer">The dealer's index.</param>
		/// <param name="rnd">The custom random number generator</param>
		public void initializeMaster (string [] playerName, int indexDealer, IRandomGenerator rnd)
		{
			if (!isCreationPhase)
				throw new WrongPhaseException ("The board must be initialized during the creation phase", "Creation phase");

			if (playerName.GetLength (0) != PLAYER_NUMBER)
				throw new Exception ("The number of player must be " + PLAYER_NUMBER);

			_me = 0;	

			_bytes.Clear ();

			for (int i = 0; i < PLAYER_NUMBER; i++) {
				_players [i] = new Player (playerName [i], i);

				//add the player's name at the bytes array
				byte [] bs = Encoding.ASCII.GetBytes (playerName [i]);
				_bytes.Add (( BitConverter.GetBytes (bs.GetLength (0)) ) [0]);
				_bytes.AddRange (bs);

			}

			_lastWinner = ( indexDealer + 1 ) % PLAYER_NUMBER;	//the last winner is the player that have to play first in the next turn
			_bytes.Add (BitConverter.GetBytes (indexDealer) [0]);	//add the dealer at the bytes array

			int nCard = nSemi * nNumber;	//the numbers of card
			int nCardForPlayer = nCard / PLAYER_NUMBER;	//the number of card for player

			int [] cardAssign = { 0, 0, 0, 0, 0 };	//counter for the card distribution

			for (int i = 0; i < nSemi; i++)		//cycle all the possibible card
				for (int j = 0; j < nNumber; j++) {
					int assignedPlayer = rnd.getRandomNumber (PLAYER_NUMBER);	
					while (cardAssign [assignedPlayer] == nCardForPlayer)
						assignedPlayer = rnd.getRandomNumber (PLAYER_NUMBER);	//continue to change the assigned player until isn't a full player

					_cardGrid [i, j] = new Card ((EnNumbers) j, (EnSemi) i, _players [assignedPlayer]);	//instantiate the card
					cardAssign [assignedPlayer]++;

					//add the assigned player to the bytes array
					_bytes.Add (BitConverter.GetBytes (assignedPlayer) [0]);
				}
		}

		/// <summary>
		/// Initializes the board for a master device.
		/// </summary>
		/// <param name="playerName">The Players' name.</param>
		/// <param name="indexDealer">The dealer's index.</param>
		public void initializeMaster (string [] playerName, int indexDealer)
		{
			initializeMaster (playerName, indexDealer, new NormalRandom ());
		}

		/// <summary>
		/// Initializes the board for a slave device.
		/// </summary>
		/// <param name="bytes">The sequence of bytes.</param>
		/// <param name="me">The name of the player on this device.</param>
		public void initializeSlave (byte [] bytes, string me)
		{
			reset ();

			_bytes.Clear ();
			_bytes.AddRange (bytes);

			int index = 0;

			//Player initialization
			for (int i = 0; i < PLAYER_NUMBER; i++) {

				int lenght = BitConverter.ToInt16 (new byte[2]{ _bytes [index], 0 }, 0);
				index = index + 1;

				byte [] bs = new byte[lenght];
				for (int j = 0; j < lenght; j++) {
					bs [j] = _bytes [index + j];
				}

				index = index + lenght;
				_players [i] = new Player (Encoding.ASCII.GetString (bs), i);
			}

			_lastWinner = ( _bytes [index] + 1 ) % PLAYER_NUMBER;	//the last winner is the player that have to play first in the next turn
			index++;


			//card initialization
			for (int i = 0; i < nSemi; i++)		//cycle all the possibible card
				for (int j = 0; j < nNumber; j++) {

					int assignedPlayer = BitConverter.ToInt16 (new byte[2] {
						_bytes [index],
						0
					}, 0);

					_cardGrid [i, j] = new Card ((EnNumbers) j, (EnSemi) i, _players [assignedPlayer]);	//instantiate the card
					index++;
				}

			foreach (Player p in _players)
				if (p.name == me) {
					_me = p.order;
					return;
				}

		}

		/// <summary>
		/// Array of bytes representing the board (used to send the BOARD message to the slaves devices).
		/// </summary>
		private readonly List<Byte> _bytes = new List<byte> ();

		/// <summary>
		/// Gets the sendable bytes.
		/// </summary>
		/// <value>The sendable bytes.</value>
		public List<Byte> SendableBytes { get { return _bytes; } }

		#endregion

		#region Time management

		/// <summary>
		/// Variable that rappresent the current discrete time
		/// 
		/// The BID message is composed of 4 Bytes:
		/// <list type="table">
		/// <listheader>
		/// <term>Time</term>
		/// <term>Description</term>
		/// </listheader>
		/// <item><term>-3</term><description>Creation time</description></item>
		/// <item><term>-2</term><description>Auction time</description></item>
		/// <item><term>-1</term><description>Finalize</description></item>
		/// <item><term>0</term><description>First play of the first turn</description></item>
		/// <item><term>...</term><description></description></item>
		/// <item><term>4</term><description>Last play of the first turn</description></item>
		/// <item><term>5</term><description>First play of the second turn</description></item>
		/// <item><term>...</term><description></description></item>
		/// <item><term>39</term><description>Last play of the last turn</description></item>
		/// <item><term>40</term><description>Points counting and conclusion</description></item>
		/// </list>
		/// </summary>
		private int _t = -3;

		/// <summary>
		/// Gets a value indicating whether this <see cref="ChiamataLibrary.Board"/> is creating the cards and players.
		/// </summary>
		/// <value><c>true</c> if the board is creating; otherwise, <c>false</c>.</value>
		public bool isCreationPhase{ get { return _t == -3; } }


		/// <summary>
		/// Gets a value indicating whether this <see cref="ChiamataLibrary.Board"/> is doing the auction.
		/// </summary>
		/// <value><c>true</c> if is the auction is ongoing; otherwise, <c>false</c>.</value>
		public bool isAuctionPhase{ get { return _t == -2; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ChiamataLibrary.Board"/> is in the finalize phase.
		/// </summary>
		/// <value><c>true</c> if the finalize phase is ongoing; otherwise, <c>false</c>.</value>
		public bool isFinalizePhase{ get { return _t == -1; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ChiamataLibrary.Board"/> is in play time.
		/// </summary>
		/// <value><c>true</c> if is in play time; otherwise, <c>false</c>.</value>
		public bool isPlayTime{ get { return _t >= 0 && _t <= nSemi * nNumber; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ChiamataLibrary.Board"/> is on the last turn.
		/// </summary>
		/// <value><c>true</c> if is on the last turn; otherwise, <c>false</c>.</value>
		public bool isLastTurn{ get { return _t == nSemi * nNumber; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ChiamataLibrary.Board"/> has finished the game.
		/// </summary>
		/// <value><c>true</c> if the game is finished; otherwise, <c>false</c>.</value>
		public bool isGameFinish{ get { return _t > nSemi * nNumber; } }

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
		/// See <see cref="ChiamataLibrary.Board._t"/> for more informations.
		/// </summary>
		/// <value>The time.</value>
		public int Time { get { return _t; } }

		#endregion

		#region Card management

		/// <summary>
		/// The cards' matrix.
		/// </summary>
		private readonly Card [,] _cardGrid;

		// <summary>
		/// Getter for the card. Must provide SEME and NUMBER as arguments.
		/// </summary>
		/// <returns>The instance of <see cref="ChiamataLibrary.Card"/>.</returns>
		/// <param name="seme">Seme.</param>
		/// <param name="number">Number.</param>
		public Card getCard (EnSemi seme, EnNumbers number)
		{
			return _cardGrid [(int) seme, (int) number];
		}

		#endregion

		#region Player management

		/// <summary>
		/// The array of <see cref="ChiamataLibrary.Player"/> currently in the game.
		/// </summary>
		private readonly Player [] _players = new Player[PLAYER_NUMBER];

		/// <summary>
		/// The index of the player on this device.
		/// </summary>
		private int _me;

		/// <summary>
		/// Getter for the <see cref="ChiamataLibrary.Player"/>. Must provide the index as an argument.
		/// </summary>
		/// <returns>The instance of <see cref="ChiamataLibrary.Player"/> that has the index provided.</returns>
		/// <param name="order">Index of the player.</param>
		public Player getPlayer (int order)
		{
			return _players [order];
		}

		/// <summary>
		/// Gets the <see cref="ChiamataLibrary.Player"/> on this device.
		/// </summary>
		/// <value>The instance of <see cref="ChiamataLibrary.Player"/> representing myself.</value>
		public Player Me{ get { return _players [_me]; } }

		/// <summary>
		/// Gets the player's hand.
		/// </summary>
		/// <returns>The list oc <see cref="ChiamataLibrary.Card"/> representing the hand.</returns>
		/// <param name="p">The instance of <see cref="ChiamataLibrary.Player"/>.</param>
		public List<Card> getPlayerHand (Player p)
		{
			List<Card> cl = new List<Card> ();
			foreach (Card c in _cardGrid)
				if (c.initialPlayer == p && c.isPlayable)
					cl.Add (c);

			return cl;
		}

		/// <summary>
		/// Gets the player initial hand.
		/// </summary>
		/// <returns>The list oc <see cref="ChiamataLibrary.Card"/> representing the initial hand.</returns>
		/// <param name="p">he instance of <see cref="ChiamataLibrary.Player"/>.</param>
		public List<Card> getPlayerInitialHand (Player p)
		{
			List<Card> cl = new List<Card> ();
			foreach (Card c in _cardGrid)
				if (c.initialPlayer == p)
					cl.Add (c);

			return cl;
		}

		/// <summary>
		/// Gets a list of all the <see cref="ChiamataLibrary.Player"/> in the game.
		/// </summary>
		/// <returns>A list of all the <see cref="ChiamataLibrary.Player"/> in the game.</returns>
		public List<Player> AllPlayers{ get { return new List<Player> (_players); } }

		/// <summary>
		/// Gets the <see cref="ChiamataLibrary.Player"/> in the CHIAMANTE role.
		/// </summary>
		/// <returns>The <see cref="ChiamataLibrary.Player"/> in the CHIAMANTE role.</returns>
		public Player getChiamante ()
		{

			if (!isPlayTime)
				throw new WrongPhaseException ("Roles not assigned yet", "Playtime");

			foreach (Player p in _players)
				if (p.Role == EnRole.CHIAMANTE)
					return p;

			throw new Exception ("Some error occur, this path shoudn't be executed");
		}

		/// <summary>
		/// Gets the <see cref="ChiamataLibrary.Player"/> in the SOCIO role.
		/// </summary>
		/// <returns>The <see cref="ChiamataLibrary.Player"/> in the SOCIO role.</returns>
		public Player getSocio ()
		{
			if (!isPlayTime)
				throw new WrongPhaseException ("Roles not assigned yet", "Playtime");

			if (isChiamataInMano)
				return null;

			foreach (Player p in _players)
				if (p.Role == EnRole.SOCIO)
					return p;
			throw new Exception ("Some error occur, this path shoudn't be executed");
		}

		/// <summary>
		/// Gets a list of the <see cref="ChiamataLibrary.Player"/> in the ALTRI role.
		/// </summary>
		/// <returns>A list of the <see cref="ChiamataLibrary.Player"/> in the ALTRI role.</returns>
		public List<Player> getAltri ()
		{
			if (!isPlayTime)
				throw new WrongPhaseException ("Roles not assigned yet", "Playtime");

			List<Player> pl = new List<Player> ();

			foreach (Player p in _players)
				if (p.Role == EnRole.ALTRO)
					pl.Add (p);

			return pl;
		}

		#endregion

		#region Property

		#region Basic properties

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
		/// The minimum point necessary for a chimante's win.
		/// </summary>
		private int _winningPoint;

		/// <summary>
		/// Gets the minimum point necessary for a chimante's win.
		/// </summary>
		/// <value>The winning point.</value>
		public int WinningPoint { get { return _winningPoint; } }

		/// <summary>
		/// The called card.
		/// </summary>
		private Card _calledCard;

		/// <summary>
		/// Gets the called card.
		/// </summary>
		/// <value>The called card.</value>
		public Card CalledCard { get { return _calledCard; } }

		#endregion

		#region derived properties

		/// <summary>
		/// Gets a value indicating whether this <see cref="ChiamataLibrary.Board"/> is chiamata in mano.
		/// </summary>
		/// <value><c>true</c> if is chiamata in mano; otherwise, <c>false</c>.</value>
		public bool isChiamataInMano{ get { return _gameType == EnGameType.STANDARD && _calledCard.initialPlayer.Role == EnRole.CHIAMANTE; } }

		/// <summary>
		/// Gets a value indicating whether in this <see cref="ChiamataLibrary.Board"/> the socio has already revealed himself.
		/// </summary>
		/// <value><c>true</c> if the socio has revealed himself; otherwise, <c>false</c>.</value>
		public bool isSocioReveal{ get { return !_calledCard.isPlayable; } }

		/// <summary>
		/// Gets the <see cref="ChiamataLibrary.EnSemi"/> representing the briscola.
		/// </summary>
		/// <value>The briscola.</value>
		public EnSemi Briscola {
			get { 
				if (_gameType != EnGameType.STANDARD)
					throw new Exception ("In this game there isn't a briscola");

				return _calledCard.seme;
			}
		}

		/// <summary>
		/// Returns true if the card provided as argument is briscola
		/// </summary>
		/// <returns><c>true</c>, if the card provided as argument is briscola, <c>false</c> otherwise.</returns>
		/// <param name="c">The card to check.</param>
		public bool isBriscola (Card c)
		{
			if (_gameType != EnGameType.STANDARD)
				throw new Exception ("In this game there isn't a briscola");

			return c.seme == Briscola;
		}

		#endregion

		#endregion

		#region Auction management

		/// <summary>
		/// The list of bids in the auction.
		/// </summary>
		private readonly List<Bid> _listBid = new List<Bid> ();

		/// <summary>
		/// Gets the number of bids currently in the auction.
		/// </summary>
		/// <value>The number of bids.</value>
		public int NumberOfBid{ get { return _listBid.Count; } }

		/// <summary>
		/// Gets the player whose turn is it during the auction.
		/// </summary>
		private int _activeAuctionPlayer;

		public Player ActiveAuctionPlayer{ get { return _players [_activeAuctionPlayer]; } }

		/// <summary>
		/// The current winning bid.
		/// </summary>
		private NotPassBid _currentWinningBid;

		/// <summary>
		/// Gets the current auction winning bid.
		/// </summary>
		/// <value>The current auction winning bid.</value>
		public NotPassBid currentAuctionWinningBid {
			get {
				if (!( isAuctionPhase || isFinalizePhase ))
					throw new WrongPhaseException ("This information isn't relevant outside the auction", "Open/closed auction");

				return _currentWinningBid;
			}
		}

		#endregion

		#region Playtime management

		/// <summary>
		/// The index of the player that won the last hand.
		/// </summary>
		private int _lastWinner;

		/// <summary>
		/// The cards on the board.
		/// </summary>
		private readonly List<Card> _cardOnBoard = new List<Card> ();

		/// <summary>
		/// Gets the player who won the last hand.
		/// </summary>
		/// <value>The player who won the last hand.</value>
		public Player LastWinner{ get { return _players [_lastWinner]; } }

		/// <summary>
		/// Gets the player that has do his turn.
		/// </summary>
		/// <value>The player that has do his turn.</value>
		public Player ActivePlayer{ get { return _players [( _lastWinner + _t ) % PLAYER_NUMBER]; } }

		/// <summary>
		/// Gets the number of cards on board.
		/// </summary>
		/// <value>The number of cards on board.</value>
		public int numberOfCardOnBoard{ get { return _cardOnBoard.Count; } }

		/// <summary>
		/// Gets the cards on the board.
		/// </summary>
		/// <value>The list of cards on the board.</value>
		public List<Card> CardOnTheBoard { get { return _cardOnBoard; } }

		/// <summary>
		/// Gets the value on the board (points wise).
		/// </summary>
		/// <value>The value on board (points wise).</value>
		public int ValueOnBoard {
			get {
				int v = 0;
				_cardOnBoard.ForEach (c => v = v + c.getPoint ());
				return v;
			}
		}

		#endregion

		#region Flux management

		#region Event declaration

		/// <summary>
		/// Delegate for the event that occours when a bid is placed.
		/// </summary>
		public delegate void eventHandlerPlaceABid (Bid bid);

		/// <summary>
		/// Occurs when i place a bid.
		/// </summary>
		public event eventHandlerPlaceABid eventIPlaceABid;

		/// <summary>
		/// Occurs when someone else places a bid.
		/// </summary>
		public event eventHandlerPlaceABid eventSomeonePlaceABid;

		/// <summary>
		/// Delegate for the event that occours when a card is played.
		/// </summary>
		public delegate void eventHandlerPlayACard (Move move);

		/// <summary>
		/// Occurs when i play a card.
		/// </summary>
		public event eventHandlerPlayACard eventIPlayACard;

		/// <summary>
		/// Occurs when someone else plays a card.
		/// </summary>
		public event eventHandlerPlayACard eventSomeonePlayACard;

		/// <summary>
		/// Delegate for the event that occours when a player wins the hand.
		/// </summary>
		public delegate void eventHandlerPickTheBoard (Player player, List<Card> board);

		/// <summary>
		/// Occurs when someone wins the hand.
		/// </summary>
		public event eventHandlerPickTheBoard eventPickTheBoard;

		/// <summary>
		/// Delegate for the event that occours when the phase must be changed.
		/// </summary>
		public delegate void eventHandlerChangePhase ();

		/// <summary>
		/// Occurs when the auction starts.
		/// </summary>
		public event eventHandlerChangePhase eventAuctionStart;

		/// <summary>
		/// Occurs when the playtime starts.
		/// </summary>
		public event eventHandlerChangePhase eventPlaytimeStart;

		/// <summary>
		/// Occurs when the playtime ends.
		/// </summary>
		public event eventHandlerChangePhase eventPlaytimeEnd;

		#endregion

		#region Start

		/// <summary>
		/// Starts the game.
		/// </summary>
		public void start ()
		{
			_listBid.Clear ();
			_activeAuctionPlayer = _lastWinner;	//dealer+1
			_currentWinningBid = null;

			_t = -2;
			if (eventAuctionStart != null)
				eventAuctionStart ();

		}

		#endregion

		#region Update

		/// <summary>
		/// Updates the game. must be continuously called in order to continue with the game. Stopping would mean a pause in the game.
		/// </summary>
		public void update ()
		{

			if (isAuctionPhase) {	//auction
				Bid bid = _players [_activeAuctionPlayer].invokeChooseBid ();

				if (bid != null) {
					//place a bid
					_listBid.Add (bid);

					if (bid is NotPassBid)
						_currentWinningBid = (NotPassBid) bid;

					List<Player> pass = new List<Player> ();

					_listBid.ForEach (delegate(Bid b) {
						if (b is PassBid)
							pass.Add (b.bidder);
					});
						
					//find the next bidder
					_activeAuctionPlayer = ( _activeAuctionPlayer + 1 ) % PLAYER_NUMBER;

					while (pass.Contains (_players [_activeAuctionPlayer]))
						_activeAuctionPlayer = ( _activeAuctionPlayer + 1 ) % PLAYER_NUMBER;

					//event place a bid.
					if (eventIPlaceABid != null && bid.bidder == Me)
						eventIPlaceABid (bid);

					if (eventSomeonePlaceABid != null && bid.bidder != Me)
						eventSomeonePlaceABid (bid);



					//check if the auction is still open
					if (pass.Count >= PLAYER_NUMBER - 1 && _listBid.Count >= PLAYER_NUMBER) {
						_t = -1;
						if (_currentWinningBid == null) {
							_gameType = EnGameType.MONTE;
							_t = 41;
						} else if (_currentWinningBid is CarichiBid) {	//carichi
							_gameType = EnGameType.CARICHI;

							_currentWinningBid.bidder.Role = EnRole.CHIAMANTE;
							_winningPoint = ( (CarichiBid) _currentWinningBid ).point;

							_t = 0;

							if (eventPlaytimeStart != null)
								eventPlaytimeStart ();

						} 
					}
				}


			} else if (isFinalizePhase) {	//finalize
				EnSemi? seme = _currentWinningBid.bidder.invokeChooseSeme ();

				if (seme.HasValue) {

					_gameType = EnGameType.STANDARD;

					_calledCard = getCard (seme.Value, ( (NormalBid) _currentWinningBid ).number);

					_winningPoint = ( (NormalBid) _currentWinningBid ).point;

					//set the roles
					_calledCard.initialPlayer.Role = EnRole.SOCIO;
					currentAuctionWinningBid.bidder.Role = EnRole.CHIAMANTE;

					_t = 0;

					if (eventPlaytimeStart != null)
						eventPlaytimeStart ();

				}


			} else if (isPlayTime) {	//playtime

				if (numberOfCardOnBoard == PLAYER_NUMBER) {
					Card max = _cardOnBoard [0];
					for (int i = 1; i < PLAYER_NUMBER; i++)
						if (_cardOnBoard [i] > max)
							max = _cardOnBoard [i];

					_lastWinner = max.initialPlayer.order;

					_cardOnBoard.ForEach (delegate(Card c) {
						c.FinalPlayer = max.initialPlayer;
					});

					//event pick up
					if (eventPickTheBoard != null)
						eventPickTheBoard (_players [_lastWinner], _cardOnBoard);

					_cardOnBoard.Clear ();

					if (isLastTurn) {
						if (eventPlaytimeEnd != null)
							eventPlaytimeEnd ();

						addToArchive ();
						_t++;
					}

				} else {
					Move move = ActivePlayer.invokeChooseCard ();
					if (move != null) {
					
						move.card.PlayingTime = _t;
						_cardOnBoard.Add (move.card);

						//Events play a card
						if (eventIPlayACard != null && move.player == Me)
							eventIPlayACard (move);

						if (eventSomeonePlayACard != null && move.player != Me)
							eventSomeonePlayACard (move);

						_t++;
					}
				}
			} 

		}

		#endregion

		/// <summary>
		/// Adds this game to the archive.
		/// </summary>
		private void addToArchive ()
		{
			Archive.Instance.add (new GameData (DateTime.Now, _cardGrid, _players, _listBid, _gameType, _calledCard, _winningPoint));
		}

		#endregion

	}
}
