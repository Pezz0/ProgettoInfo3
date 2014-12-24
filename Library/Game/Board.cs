using System;
using System.Collections.Generic;
using MyRandom;

namespace ChiamataLibrary
{
	/// <summary>
	/// This class rapresent the board used for this game
	/// </summary>
	public class Board
	{
		public const int PLAYER_NUMBER = 5;

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
		private int _t;

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
		private Card [,] _cardGrid;

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
		/// Gets the hand of a player.
		/// </summary>
		/// <returns>The hand.</returns>
		/// <param name="player">Player.</param>
		public List<Card> getPlayerHand (Player player)
		{
			List<Card> cl = new List<Card> ();
			foreach (Card c in _cardGrid)
				if (c.InitialPlayer == player && c.isPlayable)
					cl.Add (c);

			return cl;
		}

		/// <summary>
		/// Gets current chiamante's point count.
		/// </summary>
		/// <returns>The current chiamante's point count.</returns>
		public int GetChiamantePointCount ()
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
		public int GetAltriPointCount ()
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
		private Player [] _players;

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
		/// Gets the players.
		/// </summary>
		/// <value>The players.</value>
		public List<Player> Players { get { return  new List<Player> (_players); } }

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
				return  bid is PassBid && bid.Bidder == player;
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

				int active = ( wb.Bidder.Order + 1 ) % PLAYER_NUMBER;

				while (isPlayerPassed (_players [active]))
					active = ( active + 1 ) % PLAYER_NUMBER;

				return _players [active];

			}

		}

		#endregion

		#region Changing status

		/// <summary>
		/// Method for placing a bid in the auction.
		/// </summary>
		/// <param name="bid">The bid</param>
		private void auctionPlaceABid (IBid bid)
		{
			if (!isAuctionPhase || isAuctionClosed)
				throw new WrongPhaseException ("A player can pass only during the auction phase, when is open", "Auction open");

			if (ActiveAuctionPlayer != bid.Bidder)
				throw new WrongBiddingPlayerException ("This player cannot put a bid now", bid.Bidder);

			if (bid is PassBid)
				_bidList.Add (bid);
			else if (!isBidBetter (bid))
				throw new BidNotEnoughException ("The new bid is not enough to beat the winning one", bid);
			else
				_bidList.Add (bid);

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
		/// Method for placign a normal bid.
		/// </summary>
		/// <param name="nb">The normal bid.</param>
		public void auctionPlaceABid (NormalBid nb)
		{
			auctionPlaceABid ((IBid) nb);
		}

		/// <summary>
		///  Method for placign a carichi bid.
		/// </summary>
		/// <param name="bc">the carichi bid.</param>
		public void auctionPlaceABid (BidCarichi bc)
		{
			auctionPlaceABid ((IBid) bc);
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
		/// Finalize the auction, set the called card, the players roles and start the playtime
		/// </summary>
		/// <param name="seme">Seme.</param>
		public void finalizeAuction (EnSemi seme)
		{
			if (!isAuctionPhase && !isAuctionClosed)
				throw new WrongPhaseException ("The auction can be finalized only when the auction is close", "Auction closed");

			IBid wb = currentAuctionWinningBid;

			if (wb == null)
				_gameType = EnGameType.MONTE;
			else if (wb is BidCarichi) {	//carichi
				_gameType = EnGameType.CARICHI;

				wb.Bidder.Role = EnRole.CHIAMANTE;

				_point = ( (BidCarichi) wb ).Point;
			} else if (wb is NormalBid) {	//standard
				_gameType = EnGameType.STANDARD;

				_calledCard = getCard (seme, ( (NormalBid) wb ).Number);

				_point = ( (NormalBid) wb ).Point;

				//set the roles
				_calledCard.InitialPlayer.Role = EnRole.SOCIO;
				currentAuctionWinningBid.Bidder.Role = EnRole.CHIAMANTE;
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

		#endregion

		/// <summary>
		/// Gets the player that have to play.
		/// </summary>
		/// <value>The player that have to play.</value>
		public Player ActivePlayer{ get { return _players [( _lastWinner + _t ) % 5]; } }

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="Engine.Board"/> class.
		/// </summary>
		/// <param name="playerName">An array that contains the name of the five player.</param>
		/// <param name="indexDealer">Index of the dealer in the name's array.</param>
		public Board ()
		{
			_t = -2;	//set the time

		}

		public void initialize (string [] playerName, int indexDealer)
		{
			if (playerName.GetLength (0) != PLAYER_NUMBER)
				throw new Exception ("The number of player must be " + PLAYER_NUMBER);

			_players = new Player[PLAYER_NUMBER];	//set the players array
			for (int i = 0; i < PLAYER_NUMBER; i++)
				_players [i] = new Player (this, playerName [i], i);

			_lastWinner = indexDealer;	//the last winner is the player that have to play first in the next turn

			int nSemi = Enum.GetValues (typeof (EnSemi)).GetLength (0);	//the number of semi
			int nNumbers = Enum.GetValues (typeof (EnNumbers)).GetLength (0);	//the number of numbers
			int nCard = nSemi * nNumbers;	//the numbers of card
			int nCardForPlayer = nCard / PLAYER_NUMBER;	//the number of card for player

			_cardGrid = new Card[nSemi, nNumbers];	//set the card grid
			int [] cardAssign = { 0, 0, 0, 0, 0 };	//counter for the card distribution
			IRandomGenerator rand = new NormalRandom ();	//instantiate the random generator

			for (int i = 0; i < nSemi; i++)		//cycle all the possibible card
				for (int j = 0; j < nNumbers; j++) {
					int assignedPlayer = rand.getRandomNumber (PLAYER_NUMBER);	
					while (cardAssign [assignedPlayer] == nCardForPlayer)
						assignedPlayer = rand.getRandomNumber (PLAYER_NUMBER);	
					//assignedPlayer = ( assignedPlayer + 1 ) % PLAYER_NUMBER;	//continue to change the assigned player until isn't a full player

					_cardGrid [i, j] = new Card (this, (EnNumbers) j, (EnSemi) i, _players [assignedPlayer]);	//instantiate the card
					cardAssign [assignedPlayer]++;
				}

			_t = -1;	//start the auction
			_bidList = new List<IBid> ();
		}
	}
}

