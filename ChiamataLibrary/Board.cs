using System;
using MyRandom;
using System.Collections.Generic;
using System.Text;

namespace ChiamataLibrary
{
	/// <summary>
	/// Board.
	/// </summary>
	public class Board:IBTSendable<Board>
	{
		/// <summary>
		/// The numbe of player
		/// </summary>
		public const int PLAYER_NUMBER = 5;

		/// <summary>
		/// The number of semi
		/// </summary>
		public readonly int nSemi = Enum.GetValues (typeof (EnSemi)).GetLength (0);

		/// <summary>
		/// The number of number
		/// </summary>
		public readonly int nNumber = Enum.GetValues (typeof (EnNumbers)).GetLength (0);

		#region Singleton implementation

		/// <summary>
		/// The singleton's instance.
		/// </summary>
		private static readonly Board _instance = new Board ();

		/// <summary>
		/// Gets the singleton's instance.
		/// </summary>
		/// <value>The instance.</value>
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
		/// Reset this instance.
		/// </summary>
		public void reset ()
		{
			eventSomeonePlaceABid = null;
			eventSomeonePlayACard = null;
			eventPickTheBoard = null;
			eventAuctionStart = null;
			eventPlaytimeStart = null;
			eventPlaytimeEnd = null;

			_t = -4;
		}

		#endregion

		#region Initilization

		/// <summary>
		/// Initializes the board in the master device.
		/// </summary>
		/// <param name="playerName">Players' name.</param>
		/// <param name="indexDealer">dealer's index.</param>
		public void initializeMaster (string [] playerName, int indexDealer, IRandomGenerator rnd)
		{
			if (!isCreationPhase)
				throw new WrongPhaseException ("The board must be initialized during the creation phase", "Creation phase");

			if (playerName.GetLength (0) != PLAYER_NUMBER)
				throw new Exception ("The number of player must be " + PLAYER_NUMBER);

			_me = 0;	

			_bytes = new List<byte> ();

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

		public void initializeMaster (string [] playerName, int indexDealer)
		{
			initializeMaster (playerName, indexDealer, new NormalRandom ());
		}

		/// <summary>
		/// Initializes the board in a slave device.
		/// </summary>
		/// <param name="me">Me.</param>
		public void initializeSlave (string me)
		{
			foreach (Player p in _players)
				if (p.name == me) {
					_me = p.order;
					return;
				}
		}

		#region Bluetooth

		private  List<Byte> _bytes = new List<byte> ();

		public byte[] toByteArray ()
		{
			return _bytes.ToArray ();
		}

		public Board recreateFromByteArray (byte [] bytes)
		{
			if (!isCreationPhase)
				throw new WrongPhaseException ("The board must be initialized during the creation phase", "Creation phase");

			reset ();
			_bytes = new List<Byte> (bytes);
			int index = 0;

			for (int i = 0; i < PLAYER_NUMBER; i++) {
			
				int lenght = BitConverter.ToInt16 (new byte[2]{ bytes [index], 0 }, 0);
				index = index + 1;

				byte [] bs = new byte[lenght];
				for (int j = 0; j < lenght; j++) {
					bs [j] = bytes [index + j];
				}

				index = index + lenght;
				_players [i] = new Player (Encoding.ASCII.GetString (bs), i);
			}

			_lastWinner = ( _bytes [index] + 1 ) % PLAYER_NUMBER;	//the last winner is the player that have to play first in the next turn
			index++;


			for (int i = 0; i < nSemi; i++)		//cycle all the possibible card
				for (int j = 0; j < nNumber; j++) {

					int assignedPlayer = BitConverter.ToInt16 (new byte[2]{ bytes [index], 0 }, 0);

					_cardGrid [i, j] = new Card ((EnNumbers) j, (EnSemi) i, _players [assignedPlayer]);	//instantiate the card
					index++;
				}
					
			return this;
		}

		public int ByteArrayLenght { get { return _bytes.Count; } }

		#endregion

		#endregion

		#region Time management

		/// <summary>
		/// Variable that rappresent the current discrete time
		/// 	-4 = creation time
		/// 	-3 = waiting phase
		/// 	-2 = auction time
		/// 	-1 = finalize
		/// 	 0 = first play
		/// 	...
		///  	 4 = last play of the first turn
		/// 	 5 = first play of the second turn
		/// 	...
		/// 	 39 = last play
		/// 	 40 = point counting e conclusion
		/// </summary>
		private int _t = -4;

		/// <summary>
		/// Gets a value indicating whether this <see cref="Engine.Board"/> is creating the cards and players.
		/// </summary>
		/// <value><c>true</c> if the board is creating; otherwise, <c>false</c>.</value>
		public bool isCreationPhase{ get { return _t == -4; } }

		/// <summary>
		/// /*Gets a value indicating whether this <see cref="ChiamataLibrary.Board"/> is waiting phase.*/
		/// </summary>
		/// <value><c>true</c> if is waiting phase; otherwise, <c>false</c>.</value>
		public bool isWaitingPhase{ get { return _t == -3; } }


		/// <summary>
		/// Gets a value indicating whether this <see cref="Engine.Board"/> is the time for the auction.
		/// </summary>
		/// <value><c>true</c> ifis the time for the auction; otherwise, <c>false</c>.</value>
		public bool isAuctionPhase{ get { return _t == -2; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ChiamataLibrary.Board"/> is finalize phase.
		/// </summary>
		/// <value><c>true</c> if is finalize phase; otherwise, <c>false</c>.</value>
		public bool isFinalizePhase{ get { return _t == -1; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="Engine.Board"/> is play time.
		/// </summary>
		/// <value><c>true</c> if is play time; otherwise, <c>false</c>.</value>
		public bool isPlayTime{ get { return _t >= 0 && _t <= nSemi * nNumber; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ChiamataLibrary.Board"/> is last turn.
		/// </summary>
		/// <value><c>true</c> if is last turn; otherwise, <c>false</c>.</value>
		public bool isLastTurn{ get { return _t == nSemi * nNumber; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ChiamataLibrary.Board"/> is game finish.
		/// </summary>
		/// <value><c>true</c> if is game finish; otherwise, <c>false</c>.</value>
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
		/// </summary>
		/// <value>The time.</value>
		public int Time { get { return _t; } }

		#endregion

		#region Card management

		/// <summary>
		/// The cards' grid.
		/// </summary>
		private readonly Card [,] _cardGrid;

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
		/// Gets the card from a byte's array.
		/// </summary>
		/// <returns>The card.</returns>
		/// <param name="bytes">The byte's array.</param>
		public Card getCard (byte [] bytes)
		{
			return _cardGrid [0, 0].recreateFromByteArray (bytes);
		}

		/// <summary>
		/// Gets the card.
		/// </summary>
		/// <returns>The card.</returns>
		/// <param name="bytes">Bytes.</param>
		public Card getCard (byte bytes)
		{
			return getCard (new Byte[1]{ bytes });
		}

		#endregion

		#region Player management

		/// <summary>
		/// The players.
		/// </summary>
		private readonly Player [] _players = new Player[PLAYER_NUMBER];

		/// <summary>
		/// The index of the player on this device
		/// </summary>
		private int _me;

		/// <summary>
		/// Gets the player.
		/// </summary>
		/// <returns>The player.</returns>
		/// <param name="order">Order.</param>
		public Player getPlayer (int order)
		{
			return _players [order];
		}

		/// <summary>
		/// Gets the player from a byte's array.
		/// </summary>
		/// <returns>The player.</returns>
		/// <param name="bytes">The byte's array.</param>
		public Player getPlayer (Byte [] bytes)
		{
			return _players [0].recreateFromByteArray (bytes);
		}

		/// <summary>
		/// Gets the player.
		/// </summary>
		/// <returns>The player.</returns>
		/// <param name="bytes">Bytes.</param>
		public Player getPlayer (Byte  bytes)
		{
			return getPlayer (new Byte[1]{ bytes });
		}

		/// <summary>
		/// Gets the player on this device.
		/// </summary>
		/// <value>Me.</value>
		public Player Me{ get { return _players [_me]; } }

		/// <summary>
		/// Gets the player's hand.
		/// </summary>
		/// <returns>The player hand.</returns>
		/// <param name="p">P.</param>
		public List<Card> getPlayerHand (Player p)
		{
			List<Card> cl = new List<Card> ();
			foreach (Card c in _cardGrid)
				if (c.initialPlayer == p && c.isPlayable)
					cl.Add (c);

			return cl;
		}

		public List<Card> getPlayerInitialHand (Player p)
		{
			List<Card> cl = new List<Card> ();
			foreach (Card c in _cardGrid)
				if (c.initialPlayer == p)
					cl.Add (c);

			return cl;
		}

		/// <summary>
		/// Gets all players.
		/// </summary>
		/// <value>All players.</value>
		public List<Player> AllPlayers{ get { return new List<Player> (_players); } }

		/// <summary>
		/// Gets the chiamante.
		/// </summary>
		/// <returns>The chiamante.</returns>
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
		/// Gets the socio.
		/// </summary>
		/// <returns>The socio.</returns>
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
		/// Gets the altri.
		/// </summary>
		/// <returns>The altri.</returns>
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
		/// Gets a value indicating whether this <see cref="ChiamataLibrary.Board"/> is socio reveal.
		/// </summary>
		/// <value><c>true</c> if is socio reveal; otherwise, <c>false</c>.</value>
		public bool isSocioReveal{ get { return !_calledCard.isPlayable; } }

		/// <summary>
		/// Gets the briscola.
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
		/// Ises the briscola.
		/// </summary>
		/// <returns><c>true</c>, if briscola was ised, <c>false</c> otherwise.</returns>
		/// <param name="c">C.</param>
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
		/// The default bid.
		/// </summary>
		private IBid _defBid = new PassBid ();

		/// <summary>
		/// Gets the default bid.
		/// </summary>
		/// <value>The default bid.</value>
		public IBid DefBid { get { return _defBid; } }

		/// <summary>
		/// The list bid.
		/// </summary>
		private List<IBid> _listBid;

		/// <summary>
		/// Gets the number of bid.
		/// </summary>
		/// <value>The number of bid.</value>
		public int NumberOfBid{ get { return _listBid.Count; } }

		/// <summary>
		///  Gets the player that have to do a bid or pass.
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
		/// The default move.
		/// </summary>
		private Move _defMove = new Move ();

		/// <summary>
		/// Gets the default move.
		/// </summary>
		/// <value>The default move.</value>
		public Move DefMove { get { return _defMove; } }

		/// <summary>
		/// The last winner.
		/// </summary>
		private int _lastWinner;

		/// <summary>
		/// The card on the board.
		/// </summary>
		private List<Card> _cardOnBoard = new List<Card> ();

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

		/// <summary>
		/// Gets the number of card on board.
		/// </summary>
		/// <value>The number of card on board.</value>
		public int numberOfCardOnBoard{ get { return _cardOnBoard.Count; } }

		/// <summary>
		/// Gets the card on the board.
		/// </summary>
		/// <value>The card on the board.</value>
		public List<Card> CardOnTheBoard { get { return _cardOnBoard; } }

		/// <summary>
		/// Gets the value on board.
		/// </summary>
		/// <value>The value on board.</value>
		public int ValueOnBoard {
			get {
				int v = 0;
				_cardOnBoard.ForEach (delegate(Card c) {
					v = v + c.getPoint ();
				});
				return v;
			}
		}

		#endregion

		#region Flux management

		#region Event declaration

		/// <summary>
		/// Event handler place A bid.
		/// </summary>
		public delegate void eventHandlerPlaceABid (IBid bid);

		/// <summary>
		/// Occurs when I place A bid.
		/// </summary>
		public event eventHandlerPlaceABid eventIPlaceABid;

		/// <summary>
		/// Occurs when someone else place A bid.
		/// </summary>
		public event eventHandlerPlaceABid eventSomeonePlaceABid;

		/// <summary>
		/// Event handler play A card.
		/// </summary>
		public delegate void eventHandlerPlayACard (Move move);

		/// <summary>
		/// Occurs when I play A card.
		/// </summary>
		public event eventHandlerPlayACard eventIPlayACard;

		/// <summary>
		/// Occurs when someone else play A card.
		/// </summary>
		public event eventHandlerPlayACard eventSomeonePlayACard;

		/// <summary>
		/// Event handler pick the board.
		/// </summary>
		public delegate void eventHandlerPickTheBoard (Player player, List<Card> board);

		/// <summary>
		/// Occurs when someone pick the board.
		/// </summary>
		public event eventHandlerPickTheBoard eventPickTheBoard;

		/// <summary>
		/// Event handler change phase.
		/// </summary>
		public delegate void eventHandlerChangePhase ();

		/// <summary>
		/// Occurs when i'm ready start.
		/// </summary>
		public event eventHandlerChangePhase eventImReady;

		/// <summary>
		/// Occurs when the auction start.
		/// </summary>
		public event eventHandlerChangePhase eventAuctionStart;

		/// <summary>
		/// Occurs when the playtime start.
		/// </summary>
		public event eventHandlerChangePhase eventPlaytimeStart;

		/// <summary>
		/// Occurs when the playtime end.
		/// </summary>
		public event eventHandlerChangePhase eventPlaytimeEnd;

		#endregion

		#region Start

		public void start ()
		{
			_t = -3;

				
			_listBid = new List<IBid> ();
			_activeAuctionPlayer = _lastWinner;	//dealer+1
			_currentWinningBid = null;
		}

		#endregion

		#region Update

		//private bool _imReady = false;

		public void update ()
		{
			if (isWaitingPhase) {	//waiting phase

				if (_me == 0) {	//master
					bool r = true;
					
					for (int i = 0; i < PLAYER_NUMBER && r; i++)
						r = r && _players [i].isReady;
					
					if (r) {
						_t = -2;
						if (eventAuctionStart != null)
							eventAuctionStart ();
					}
										
				} else {	//slave
					if (eventImReady != null)
						eventImReady ();


					_t = -2;
					if (eventAuctionStart != null)
						eventAuctionStart ();

				}


			} else if (isAuctionPhase) {	//auction
				IBid bid = _players [_activeAuctionPlayer].invokeChooseBid ();

				if (bid != null) {
					//place a bid
					_listBid.Add (bid);

					if (bid is NotPassBid)
						_currentWinningBid = (NotPassBid) bid;

					List<Player> pass = new List<Player> ();

					_listBid.ForEach (delegate(IBid b) {
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

					_cardOnBoard = new List<Card> ();

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

		private void addToArchive ()
		{
			Archive.Instance.add (new GameData (DateTime.Now, _cardGrid, _players, _listBid, _gameType, _calledCard, _winningPoint));
		}

		#endregion

	}
}
