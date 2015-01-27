using System;
using CocosSharp;
using System.Collections.Generic;
using ChiamataLibrary;
using Android.App;
using Android.Content;
using MenuLayout;
using Java.IO;
using System.IO;
using System.Threading;




namespace Core
{
	/// <summary>
	/// Game scene.
	/// </summary>
	public class GameScene : CCScene,IPlayerController
	{

		/// <summary>
		/// The main layer. Will be the father of all the objects in the scene.
		/// </summary>
		private readonly CCLayer _mainLayer;
		/// <summary>
		/// The main window, used to get the window size and to terminate the game.
		/// </summary>
		private readonly CCWindow _window;


		#region Result board related variables

		/// <summary>
		/// Sprite for the result board at the end of the game.
		/// </summary>
		private readonly CCSprite _resultBoard;

		/// <summary>
		/// Sprite for the victory.
		/// </summary>
		private readonly CCSprite _endStatusSpriteWin;

		/// <summary>
		/// Sprite for the defeat.
		/// </summary>
		private readonly CCSprite _endStatusSpriteLoss;

		/// <summary>
		/// Boolean value that is used to decide wheter or not the end-game sprites are already been created.
		/// </summary>
		private bool written;

		#endregion

		#region Cards related variables

		/// <summary>
		/// List of CardData (contains sprite, base position and base rotation) representing the cards in the hand.
		/// </summary>
		private readonly List<CardData> _carte = new List<CardData> (8);

		/// <summary>
		/// The cards scale.
		/// </summary>
		private readonly float _cardScale;

		/// <summary>
		/// List of CardData (contains sprite, base position and base rotation) representing the cards dropped on the game field.
		/// </summary>
		private readonly List<CardData> _droppedCards = new List<CardData> (Board.PLAYER_NUMBER);

		/// <summary>
		/// Array of positions off screen for the 5 players (used to move the cards off-screen).
		/// </summary>
		private readonly CCPoint [] _offScreen = new CCPoint[5];

		/// <summary>
		/// Boolean variable that indicates whether or not the player has dropped a card on the game field.
		/// </summary>
		private bool _played;

		/// <summary>
		/// If >0, the game will pause and wait the time indicated by this variable (in seconds).
		/// </summary>
		private float _wait;

		/// <summary>
		/// The size of the window.
		/// </summary>
		private readonly CCSize _winSize;

		#endregion

		/// <summary>
		/// List of sprites for the turn lights.
		/// </summary>
		private readonly List<CCSprite> _turnLights = new List<CCSprite> (Board.PLAYER_NUMBER);

		#region Names and bids

		/// <summary>
		/// List of labels for the player names.
		/// </summary>
		private readonly List<CCLabel> _playerNames = new List<CCLabel> (Board.PLAYER_NUMBER);

		/// <summary>
		/// List of sprites for the player bids (numbers).
		/// </summary>
		private readonly List<CCSprite> _playerBids = new List<CCSprite> (Board.PLAYER_NUMBER);

		/// <summary>
		/// List of CCNode that will be the fathers of the playerBids.
		/// </summary>
		private readonly List<CCNode> _bidsFathers = new List<CCNode> (Board.PLAYER_NUMBER);

		#endregion

		#region Buttons

		/// <summary>
		/// Variable that stores the bid done by the player.
		/// </summary>
		private IBid _myBid;

		/// <summary>
		/// Variable that stores the seme choosen by the player (EnSemi is nullable).
		/// </summary>
		private EnSemi? _mySeme;

		/// <summary>
		/// Boolean variable that indicates whether or not the player has bidded.
		/// </summary>
		private bool _bidded;

		/// <summary>
		/// Boolean variable that indicates whether or not the initialization for the seme-choosing buttons has already taken place.
		/// </summary>
		private bool _initializedSeme;

		/// <summary>
		/// Scale of the buttons.
		/// </summary>
		private readonly float _scale;

		/// <summary>
		/// Array containing the path strings for the normal buttons sprites.
		/// </summary>
		private static readonly String [] _pathButtons = {
			"btnDue",
			"btnQuattro",
			"btnCinque",
			"btnSei",
			"btnSette",
			"btnOtto",
			"btnNove",
			"btnDieci",
			"btnTre",
			"btnAsse",
			"btnLascio",
			"btnCarichi"
		};

		/// <summary>
		/// Array containing the path strings for the pressed buttons sprites.
		/// </summary>
		private static readonly String [] _pathButtonsPressed = {
			"btnDuePressed",
			"btnQuattroPressed",
			"btnCinquePressed",
			"btnSeiPressed",
			"btnSettePressed",
			"btnOttoPressed",
			"btnNovePressed",
			"btnDieciPressed",
			"btnTrePressed",
			"btnAssePressed",
			"btnLascioPressed",
			"btnCarichiPressed"
		};

		/// <summary>
		/// Array of buttons used for the bids.
		/// </summary>
		private readonly Button [] _buttons = new Button[12];

		/// <summary>
		/// Button used to choose ori.
		/// </summary>
		private readonly Button _chooseOri;

		/// <summary>
		/// Button used to choose spade.
		/// </summary>
		private readonly Button _chooseSpade;

		/// <summary>
		/// Button used to choose bastoni.
		/// </summary>
		private readonly Button _chooseBastoni;

		/// <summary>
		/// Button used to choose coppe.
		/// </summary>
		private Button _chooseCoppe;

		/// <summary>
		/// Button used to exit the game.
		/// </summary>
		private Button _btnExit;

		/// <summary>
		/// Button used to start the next game.
		/// </summary>
		private Button _btnNext;

		/// <summary>
		/// Vertical spacing between the buttons.
		/// </summary>
		private const float _vertSpace = 0.05f * 58;

		/// <summary>
		/// Orizzontal spacing between the buttons.
		/// </summary>
		private const float _orzSpace = 0.04f * 115;

		/// <summary>
		/// List of methods linked to the buttons.
		/// </summary>
		private readonly List<TouchList.eventHandlerTouch> _actButtons = new List<TouchList.eventHandlerTouch> ();

		/// <summary>
		/// Slider used to modify the points
		/// </summary>
		private readonly Slider _slider;

		/// <summary>
		/// Disables all buttons.
		/// </summary>
		private void disableAllButtons ()
		{
			for (int i = 0; i < 12; i++)
				_buttons [i].Enabled = false;
		}

		/// <summary>
		/// Enables all the avaiable buttons.
		/// </summary>
		private void enableAvaiableButtons ()
		{
			//TODO : mettere anche la logica per i carichi
			//Reset all the buttons.
			disableAllButtons ();
			//If the winning bid is a normal bid, the avaiable buttons will be all the ones with number < winningNumber.
			if (Board.Instance.currentAuctionWinningBid is NormalBid) {
				NormalBid b = (NormalBid) ( Board.Instance.currentAuctionWinningBid );
				for (int i = (int) b.number - 1; i >= 0; i--)
					_buttons [i].Enabled = true;
				//Enable the lascio and due buttons.
				_buttons [0].Enabled = true;
				_buttons [10].Enabled = true;
				//If  the due is called, make the slider visible.
				if (b.number == 0) {
					_slider.visible = true;
					_slider.min = b.point + 1;
				}
			}



		}

		#region Buttons actions

		/// <summary>
		/// Action for the button lascio.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void actLascio (List<CCTouch> touches, CCEvent touchEvent)
		{
			_myBid = new PassBid ();
			_bidded = true;
			auctionEnded ();
		}

		/// <summary>
		/// Action for the button carichi.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void actCarichi (List<CCTouch> touches, CCEvent touchEvent)
		{
			disableAllButtons ();

			_myBid = new CarichiBid (Board.Instance.Me, _slider.currentValue);
			_bidded = true;

		}

		/// <summary>
		/// Action for the button asse.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void actAsse (List<CCTouch> touches, CCEvent touchEvent)
		{
			disableAllButtons ();

			_myBid = new NormalBid (Board.Instance.Me, EnNumbers.ASSE, 61);
			_bidded = true;
		}

		/// <summary>
		/// Action for the button tre.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void actTre (List<CCTouch> touches, CCEvent touchEvent)
		{


			disableAllButtons ();

			_myBid = new NormalBid (Board.Instance.Me, EnNumbers.TRE, 61);

			_bidded = true;
		}

		/// <summary>
		/// Action for the button dieci.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void actDieci (List<CCTouch> touches, CCEvent touchEvent)
		{

			disableAllButtons ();

			_myBid = new NormalBid (Board.Instance.Me, EnNumbers.RE, 61);

			_bidded = true;
		}

		/// <summary>
		/// Action for the button nove.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void actNove (List<CCTouch> touches, CCEvent touchEvent)
		{


			disableAllButtons ();

			_myBid = new NormalBid (Board.Instance.Me, EnNumbers.CAVALLO, 61);
			_bidded = true;
		}

		/// <summary>
		/// Action for the button otto.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void actOtto (List<CCTouch> touches, CCEvent touchEvent)
		{


			disableAllButtons ();

			_myBid = new NormalBid (Board.Instance.Me, EnNumbers.FANTE, 61);
			_bidded = true;
		}

		/// <summary>
		/// Action for the button sette.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void actSette (List<CCTouch> touches, CCEvent touchEvent)
		{


			disableAllButtons ();

			_myBid = new NormalBid (Board.Instance.Me, EnNumbers.SETTE, 61);
			_bidded = true;
		}

		/// <summary>
		/// Action for the button sei.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void actSei (List<CCTouch> touches, CCEvent touchEvent)
		{


			disableAllButtons ();

			_myBid = new NormalBid (Board.Instance.Me, EnNumbers.SEI, 61);
			_bidded = true;
		}

		/// <summary>
		/// Action for the button cinque.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void actCinque (List<CCTouch> touches, CCEvent touchEvent)
		{


			disableAllButtons ();

			_myBid = new NormalBid (Board.Instance.Me, EnNumbers.CINQUE, 61);
			_bidded = true;
		}

		/// <summary>
		/// Action for the button quattro.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void actQuattro (List<CCTouch> touches, CCEvent touchEvent)
		{


			disableAllButtons ();

			_myBid = new NormalBid (Board.Instance.Me, EnNumbers.QUATTRO, 61);
			_bidded = true;
		}

		/// <summary>
		/// Action for the button due.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void actDue (List<CCTouch> touches, CCEvent touchEvent)
		{


			disableAllButtons ();

			_myBid = new NormalBid (Board.Instance.Me, EnNumbers.DUE, _slider.currentValue);
			_bidded = true;
		}

		/// <summary>
		/// Action for the button ori.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void actOri (List<CCTouch> touches, CCEvent touchEvent)
		{
			disableAllButtons ();

			_mySeme = EnSemi.ORI;
			_bidded = true;
		}

		/// <summary>
		/// Action for the button bastoni.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void actBastoni (List<CCTouch> touches, CCEvent touchEvent)
		{
			disableAllButtons ();

			_mySeme = EnSemi.BASTONI;
			_bidded = true;
		}

		/// <summary>
		/// Action for the button coppe.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void actCoppe (List<CCTouch> touches, CCEvent touchEvent)
		{
			disableAllButtons ();

			_mySeme = EnSemi.COPE;
			_bidded = true;
		}

		/// <summary>
		/// Action for the button spade.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void actSpade (List<CCTouch> touches, CCEvent touchEvent)
		{
			disableAllButtons ();

			_mySeme = EnSemi.SPADE;
			_bidded = true;
		}

		/// <summary>
		/// Action for the button exit.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void actExit (List<CCTouch> touches, CCEvent touchEvent)
		{
			lock (_terminateMsg) {
				_terminateMsg.setAbort ();
				Monitor.Pulse (_terminateMsg);
			}

			new Thread (restart).Start ();
		}

		/// <summary>
		/// Action for the button next game.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void actNext (List<CCTouch> touches, CCEvent touchEvent)
		{
			lock (_terminateMsg) {
				_terminateMsg.setRestart ();
				Monitor.Pulse (_terminateMsg);
			}

			new Thread (restart).Start ();
		}

		/// <summary>
		/// Method called by the thread. Terminates the game and restarts another one.
		/// </summary>
		private void restart ()
		{
			lock (_terminateMsg) {
				Monitor.Wait (_terminateMsg);
			}
		
			Window.DefaultDirector.ReplaceScene (new GameScene (_window, _terminateMsg));
		}

		#endregion

		#region controller bid and seme

		public IBid chooseBid ()
		{
			turnLight (0);

			if (_bidded) {
				if (_myBid is NormalBid)
					_playerBids [0].Texture = new CCTexture2D ("cll_" + ( (NormalBid) _myBid ).number.ToString ());
				turnLight (1);
				_bidded = false;
				return _myBid;
			}
			return null;
		}

		public EnSemi? chooseSeme ()
		{
			turnLight (0);
			if (!_initializedSeme) {
				chooseSemeButtons ();
				_initializedSeme = true;
			}
			if (_bidded) {
				_bidded = false;
				_chooseOri.remove ();
				_chooseCoppe.remove ();
				_chooseBastoni.remove ();
				_chooseSpade.remove ();
				return _mySeme;
			}
			return null;
		}

		#endregion

		#endregion

		#region Touch

		/// <summary>
		/// Touch listener.
		/// </summary>
		private readonly TouchList _touch;

		/// <summary>
		/// Index that represent the selected card.
		/// </summary>
		private int _selected;

		/// <summary>
		/// Rectangle defining where the cards can be dropped.
		/// </summary>
		private readonly CCRect _dropField;

		/// <summary>
		/// Rectangle defining where the cards can be rearranged.
		/// </summary>
		private readonly CCRect _cardField;

		/// <summary>
		/// Number of cards currently in the hand.
		/// </summary>
		private int _inHand;

		/// <summary>
		/// Boolean variable that defines which touch must be used. 
		/// </summary>
		private bool _touchAsta;

		#region Touch listener asta

		/// <summary>
		/// Function executed on the starting touch
		/// </summary>
		/// <param name="touches">List of touches</param>
		/// <param name="touchEvent">Touch event</param>
		void touchBegan (List<CCTouch> touches, CCEvent touchEvent)
		{


			CCPoint pos = touches [0].LocationOnScreen;
			CCPoint posToParent = new CCPoint (pos.X, _winSize.Height - pos.Y);

			//Checking on wich card the touch is positioned
			//I'm doing this in reverse because the 7th card is the one in the foreground and the 0th card is the one in the background
			int i;
			_selected = -1;
			for (i = _inHand - 1; i >= 0; i--) {
				if (_carte [i].sprite.BoundingBoxTransformedToParent.ContainsPoint (posToParent)) {
					_selected = i;
					break;
				}

			}
				
		}

		/// <summary>
		/// Function executed on the touch movement
		/// </summary>
		/// <param name="touches">List of touches</param>
		/// <param name="touchEvent">Touch event</param>
		void touchMoved (List<CCTouch> touches, CCEvent touchEvent)
		{
			//Moving the sprite following exactly the touch
			CCPoint pos = touches [0].LocationOnScreen;
			if (_touchAsta) {
				//Returning the card to their place if the bound of the rectangle are surpassed
				if (!_cardField.ContainsPoint (pos) && _selected >= 0) {
					moveSprite (_carte [_selected].posBase, _carte [_selected].sprite);
					_selected = -1;
				}
			}
			//Only if a touch has begun and a card has been selected
			if (_selected >= 0) {
				//Inverting Y cuz the image is referred to the bottom-left corner and the touch is referred to the top-left corner
				_carte [_selected].sprite.Position = new CCPoint (pos.X, _winSize.Height - pos.Y);

				#region Card rearrangement
				//Checking if player wants to rearrange the cards

				//Every case has the same pattern :
				//1 - Check if the card has been moved beyond the next one : player wants to swap those cards
				//2 - Switch che posBase values
				//3 - If card has not moved yet, make it move
				//4 - Swap depth
				//5 - Swap cardData elements
				if (_cardField.ContainsPoint (pos) && _inHand > 1) {
					if (_selected == 0) {
						if (( _winSize.Height - pos.Y ) > _carte [1].posBase.Y + 8) {
							//Swap base position
							CCPoint tempP = _carte [1].posBase;
							_carte [1].posBase = _carte [0].posBase;
							_carte [0].posBase = tempP;

							//Swap base rotation
							float tempR = _carte [1].rotation;
							_carte [1].rotation = _carte [0].rotation;
							_carte [0].rotation = tempR;

							//Move and rotate sprite
							if (!_carte [1].posBase.Equals (_carte [1].sprite.Position)) {
								moveSprite (_carte [0].sprite, _carte [0].rotation);
								moveSprite (_carte [1].posBase, _carte [1].sprite, 0.3f, _carte [1].rotation);
							}


							//Swap zOrder
							_carte [0].sprite.ZOrder = 1;
							_carte [1].sprite.ZOrder = 0;

							//Swap cardData entry
							CardData tempC = _carte [1];
							_carte [1] = _carte [0];
							_carte [0] = tempC;

							//Now the selected card is in another position
							_selected = 1;
						}
					} else if (_selected == _inHand - 1) {
						if (( _winSize.Height - pos.Y ) < _carte [_selected - 1].posBase.Y - 8) {
							//Swap base position
							CCPoint tempP = _carte [_inHand - 2].posBase;
							_carte [_inHand - 2].posBase = _carte [_inHand - 1].posBase;
							_carte [_inHand - 1].posBase = tempP;

							//Swap base rotation
							float tempR = _carte [_inHand - 2].rotation;
							_carte [_inHand - 2].rotation = _carte [_inHand - 1].rotation;
							_carte [_inHand - 1].rotation = tempR;

							//Move and rotate sprite
							if (!_carte [_inHand - 2].posBase.Equals (_carte [_inHand - 2].sprite.Position)) {
								moveSprite (_carte [_inHand - 1].sprite, _carte [_inHand - 1].rotation);
								moveSprite (_carte [_inHand - 2].posBase, _carte [_inHand - 2].sprite, 0.3f, _carte [_inHand - 2].rotation);
							}


							//Swap zOrder
							_carte [_inHand - 1].sprite.ZOrder = _inHand - 2;
							_carte [_inHand - 2].sprite.ZOrder = _inHand - 1;

							//Swap cardData entry
							CardData tempC = _carte [_inHand - 2];
							_carte [_inHand - 2] = _carte [_inHand - 1];
							_carte [_inHand - 1] = tempC;

							//Now the selected card is in another position
							_selected = _inHand - 2;
						}
					} else {
						if (( _winSize.Height - pos.Y ) < _carte [_selected - 1].posBase.Y - 8) {
							//Swap base position
							CCPoint tempP = _carte [_selected - 1].posBase;
							_carte [_selected - 1].posBase = _carte [_selected].posBase;
							_carte [_selected].posBase = tempP;

							//Swap base rotation
							float tempR = _carte [_selected - 1].rotation;
							_carte [_selected - 1].rotation = _carte [_selected].rotation;
							_carte [_selected].rotation = tempR;

							//Move and rotate sprite
							if (!_carte [_selected - 1].posBase.Equals (_carte [_selected - 1].sprite.Position)) {
								moveSprite (_carte [_selected].sprite, _carte [_selected].rotation);
								moveSprite (_carte [_selected - 1].posBase, _carte [_selected - 1].sprite, 0.3f, _carte [_selected - 1].rotation);
							}

							//Swap zOrder
							_carte [_selected].sprite.ZOrder = _selected - 1;
							_carte [_selected - 1].sprite.ZOrder = _selected;

							//Swap cardData entry
							CardData tempC = _carte [_selected - 1];
							_carte [_selected - 1] = _carte [_selected];
							_carte [_selected] = tempC;

							//Now the selected card is in another position
							_selected = _selected - 1;
						}

						if (( _winSize.Height - pos.Y ) > _carte [_selected + 1].posBase.Y + 8) {
							//Swap base position
							CCPoint tempP = _carte [_selected + 1].posBase;
							_carte [_selected + 1].posBase = _carte [_selected].posBase;
							_carte [_selected].posBase = tempP;

							//Swap base rotation
							float tempR = _carte [_selected + 1].rotation;
							_carte [_selected + 1].rotation = _carte [_selected].rotation;
							_carte [_selected].rotation = tempR;

							//Move and rotate sprite
							if (!_carte [_selected + 1].posBase.Equals (_carte [_selected + 1].sprite.Position)) {
								moveSprite (_carte [_selected].sprite, _carte [_selected].rotation);
								moveSprite (_carte [_selected + 1].posBase, _carte [_selected + 1].sprite, 0.3f, _carte [_selected + 1].rotation);
							}


							//Swap zOrder
							_carte [_selected + 1].sprite.ZOrder = _selected;
							_carte [_selected].sprite.ZOrder = _selected + 1;

							//Swap cardData entry
							CardData tempC = _carte [_selected + 1];
							_carte [_selected + 1] = _carte [_selected];
							_carte [_selected] = tempC;

							//Now the selected card is in another position
							_selected = _selected + 1;
						}
					}
				}
				#endregion
			}


		}



		/// <summary>
		/// Function executed when the touch is released
		/// </summary>
		/// <param name="touches">List of touches</param>
		/// <param name="touchEvent">Touch event</param>
		void touchEnded (List<CCTouch> touches, CCEvent touchEvent)
		{
			CCPoint pos = touches [0].LocationOnScreen;
			if (_selected >= 0) {
				if (!_touchAsta) {
					if (_dropField.ContainsPoint (pos)) {

						if (_selected < _inHand / 2 && _selected > 0) {
							for (int i = 0; i <= _selected - 1; i++) {
								_carte [i].posBase = _carte [i + 1].posBase;
								_carte [i].rotation = _carte [i + 1].rotation;
								_carte [i].sprite.ZOrder++;
								moveSprite (_carte [i].posBase, _carte [i].sprite, 0.3f, _carte [i].rotation);
							}
						} else if (_selected >= _inHand / 2 && _selected < _inHand - 1) {
							for (int i = _inHand - 1; i >= _selected + 1; i--) {
								_carte [i].posBase = _carte [i - 1].posBase;
								_carte [i].rotation = _carte [i - 1].rotation;
								_carte [i].sprite.ZOrder--;
								moveSprite (_carte [i].posBase, _carte [i].sprite, 0.3f, _carte [i].rotation);
							}
						}
						_carte [_selected].sprite.Scale = _cardScale * 0.75f;
						_carte [_selected].posBase = new CCPoint (_dropField.MaxX, _winSize.Height - _dropField.Center.Y);
						moveSprite (new CCPoint (_carte [_selected].posBase.X, _carte [_selected].posBase.Y), _carte [_selected].sprite);
						_droppedCards.Add (_carte [_selected]);
						_played = true;

						_carte.RemoveAt (_selected);
						_inHand--;
					


					} else {
						moveSprite (new CCPoint (_carte [_selected].posBase.X, _carte [_selected].posBase.Y), _carte [_selected].sprite);
					}
				} else
					moveSprite (new CCPoint (_carte [_selected].posBase.X, _carte [_selected].posBase.Y), _carte [_selected].sprite);
				_selected = -1;
			}



		}

		#endregion

		#endregion

		public bool isReady { get { return true; } }



		//Boolean value that says if the debug label has already been written.
		//private bool written;

		/// <summary>
		/// Message that contains the information about continue or quit the game.
		/// </summary>
		private TerminateMessage _terminateMsg;


		/// <summary>
		/// Gamescene constructor, initializes sprites to their default position
		/// </summary>
		/// <param name="mainWindow">Main window.</param>
		public GameScene (CCWindow mainWindow, TerminateMessage terminateMsg) : base (mainWindow)
		{
			this._terminateMsg = terminateMsg;

			//Instancing the layer and setting him as a child of the mainWindow.
			_mainLayer = new CCLayerColor ();
			AddChild (_mainLayer);

			//Saving the window for later use.
			_window = mainWindow;


			//Instancing the touch listener.
			_touch = new TouchList (this);


			#region Card data initialization
			//Getting the window size.
			_winSize = mainWindow.WindowSizeInPixels;

			//Setting the area when cards can be dropped.
			_dropField = new CCRect (0, (int) ( _winSize.Height / 5 ), (int) ( _winSize.Width / 2 ), (int) ( _winSize.Height * 3 / 5 ));

			//Setting the area where the cards can be re-arranged.
			_cardField = new CCRect ((int) ( _winSize.Width * 3.5 / 5 ), (int) ( _winSize.Height / 5 ), (int) ( _winSize.Width * 1.5 / 5 ), (int) ( _winSize.Height * 3 / 5 ));

			//Setting the "off-Screen" position for the cards.
			_offScreen [0] = new CCPoint (_winSize.Width + 200, _winSize.Height / 2);
			_offScreen [1] = new CCPoint (_winSize.Width / 2, _winSize.Height + 100);
			_offScreen [2] = new CCPoint (-100, _winSize.Height * 3 / 4);
			_offScreen [3] = new CCPoint (-100, _winSize.Height / 4);
			_offScreen [4] = new CCPoint (_winSize.Width / 2, -100);

			//Initializing the number of cards in-hand.
			_inHand = 8;
			#endregion

			#region Events subscriptions
			//Initializing the events of the touch listener.
			_touch.eventTouchBegan += touchBegan;
			_touch.eventTouchMoved += touchMoved;
			_touch.eventTouchEnded += touchEnded;

			//Initializing the methods for the events provided by the board.
			Board.Instance.eventAuctionStart += auctionStarted;
			Board.Instance.eventSomeonePlaceABid += bidPlaced;
			Board.Instance.eventPlaytimeStart += startPlaytime;
			Board.Instance.eventSomeonePlayACard += playCard;
			Board.Instance.eventPickTheBoard += clearBoard;
			#endregion

			#region Light initialization
			_turnLights.Add (new CCSprite ("turnLight"));						//Adding the sprite to the list.
			_turnLights [0].Position = new CCPoint (_winSize.Width - _turnLights [0].ContentSize.Height / 2, _winSize.Height / 2);	//Setting the position.
			_turnLights [0].BlendFunc = CCBlendFunc.NonPremultiplied;				//Setting the blend function to show the transparency.
			_turnLights [0].Rotation = -90;								//Setting the rotation.	
			_turnLights [0].Color = CCColor3B.Red;							//Setting the color for the light (default is white).
			_turnLights [0].ScaleX = _winSize.Height / _turnLights [0].ContentSize.Width;		//Setting the scale.
			_mainLayer.AddChild (_turnLights [0]);							//Setting the sprite to be a child of the mainlayer.
			_turnLights [0].ZOrder = 0;								//Setting the ZOrder (depth).
			_turnLights [0].Visible = false;							//Making the sprite invisible for now.

			_turnLights.Add (new CCSprite ("turnLight"));
			_turnLights [1].Position = new CCPoint (_winSize.Width / 2, _winSize.Height + 5 - _turnLights [1].ContentSize.Height / 2);
			_turnLights [1].BlendFunc = CCBlendFunc.NonPremultiplied;
			_turnLights [1].Rotation = 180;
			_turnLights [1].Color = CCColor3B.Red;
			_turnLights [1].ScaleX = _winSize.Width / _turnLights [1].ContentSize.Width;
			_mainLayer.AddChild (_turnLights [1]);
			_turnLights [1].ZOrder = 20;
			_turnLights [1].Visible = false;

			_turnLights.Add (new CCSprite ("turnLight"));
			_turnLights [2].Position = new CCPoint (-5 + _turnLights [2].ContentSize.Height / 2, _winSize.Height * 3 / 4);
			_turnLights [2].BlendFunc = CCBlendFunc.NonPremultiplied;
			_turnLights [2].Rotation = 90;
			_turnLights [2].Color = CCColor3B.Red;
			_turnLights [2].ScaleX = ( _winSize.Height / 2 ) / _turnLights [2].ContentSize.Width;
			_mainLayer.AddChild (_turnLights [2]);
			_turnLights [2].ZOrder = 20;
			_turnLights [2].Visible = false;

			_turnLights.Add (new CCSprite ("turnLight"));
			_turnLights [3].Position = new CCPoint (-5 + _turnLights [3].ContentSize.Height / 2, _winSize.Height / 4);
			_turnLights [3].BlendFunc = CCBlendFunc.NonPremultiplied;
			_turnLights [3].Rotation = 90;
			_turnLights [3].Color = CCColor3B.Red;
			_turnLights [3].ScaleX = ( _winSize.Height / 2 ) / _turnLights [3].ContentSize.Width;
			_mainLayer.AddChild (_turnLights [3]);
			_turnLights [3].ZOrder = 20;
			_turnLights [3].Visible = false;

			_turnLights.Add (new CCSprite ("turnLight"));
			_turnLights [4].Position = new CCPoint (_winSize.Width / 2, -5 + _turnLights [1].ContentSize.Height / 2);
			_turnLights [4].BlendFunc = CCBlendFunc.NonPremultiplied;
			_turnLights [4].Color = CCColor3B.Red;
			_turnLights [4].ScaleX = _winSize.Width / _turnLights [4].ContentSize.Width;
			_mainLayer.AddChild (_turnLights [4]);
			_turnLights [4].ZOrder = 20;
			_turnLights [4].Visible = false;

			#endregion

			#region Names initialization
			_playerNames.Add (new CCLabel ("", "Arial", 12));			//My name.

			//Add a new label with the player name, then set the color to black and the position to be near the side of the window.
			_playerNames.Add (new CCLabel (Board.Instance.AllPlayers [( Board.Instance.Me.order + 1 ) % Board.PLAYER_NUMBER].name, "Arial", ( _winSize.Width / 12 ) * 0.5f));
			_playerNames [1].Position = new CCPoint (_winSize.Width / 2, _winSize.Height - _winSize.Height / 40);
			_playerNames [1].Color = CCColor3B.Black;
			_mainLayer.AddChild (_playerNames [1]);

			_playerNames.Add (new CCLabel (Board.Instance.AllPlayers [( Board.Instance.Me.order + 2 ) % Board.PLAYER_NUMBER].name, "Arial", ( _winSize.Width / 12 ) * 0.5f));
			_playerNames [2].Position = new CCPoint (_winSize.Height / 40, _winSize.Height * 3 / 4);
			_playerNames [2].Rotation = -90;
			_playerNames [2].Color = CCColor3B.Black;
			_mainLayer.AddChild (_playerNames [2]);

			_playerNames.Add (new CCLabel (Board.Instance.AllPlayers [( Board.Instance.Me.order + 3 ) % Board.PLAYER_NUMBER].name, "Arial", ( _winSize.Width / 12 ) * 0.5f));
			_playerNames [3].Position = new CCPoint (_winSize.Height / 40, _winSize.Height / 4);
			_playerNames [3].Rotation = -90;
			_playerNames [3].Color = CCColor3B.Black;
			_mainLayer.AddChild (_playerNames [3]);

			_playerNames.Add (new CCLabel (Board.Instance.AllPlayers [( Board.Instance.Me.order + 4 ) % Board.PLAYER_NUMBER].name, "Arial", ( _winSize.Width / 12 ) * 0.5f));
			_playerNames [4].Position = new CCPoint (_winSize.Width / 2, _winSize.Height / 40);
			_playerNames [4].Rotation = 180;
			_playerNames [4].Color = CCColor3B.Black;
			_mainLayer.AddChild (_playerNames [4]);
			#endregion

			#region Card sprites creation and positioning
			CCPoint posBase;
			float rotation;
			for (int i = 0; i < 8; i++) {


				if (i == 0) 	//First card.
					posBase = new CCPoint (_winSize.Width - 50 + 3 * ( i * i - 7 * i + 12 ), _winSize.Height / 4);
				else 		//All the other cards (Positioning the cards in an arc shape, using a parabola constructed with the for index).
					posBase = new CCPoint (_winSize.Width - 50 + 3 * ( i * i - 7 * i + 12 ), _carte [i - 1].posBase.Y + ( _winSize.Height / _carte [0].sprite.Texture.PixelsWide ) * 21f);

				rotation = -90 - 4 * ( i > 3 ? 4 - i - 1 : 4 - i );
				_carte.Add (new CardData (new CCSprite (Board.Instance.Me.Hand [i].number.ToString () + "_" + Board.Instance.Me.Hand [i].seme.ToString ()), posBase, rotation, i));


				_carte [i].sprite.Position = _carte [i].posBase;					//Set the position.
				_carte [i].sprite.Rotation = _carte [i].rotation;					//Set the rotation.
				_cardScale = ( _winSize.Height / _carte [i].sprite.Texture.PixelsWide ) * 0.12f;	//Set the scale.
				_carte [i].sprite.Scale = _cardScale;

				_mainLayer.AddChild (_carte [i].sprite, i);						//Add the sprite as a child of the mainlayer.
			}	
			#endregion

			#region Auction buttons initialization
			//Initializing the list of methods for the button actions.
			_actButtons.Add (actDue);
			_actButtons.Add (actQuattro);
			_actButtons.Add (actCinque);
			_actButtons.Add (actSei);
			_actButtons.Add (actSette);
			_actButtons.Add (actOtto);
			_actButtons.Add (actNove);
			_actButtons.Add (actDieci);
			_actButtons.Add (actTre);
			_actButtons.Add (actAsse);
			_actButtons.Add (actLascio);
			_actButtons.Add (actCarichi);


			int textWidth = new CCTexture2D ("btnLascio").PixelsWide;

			//Setting a common value for the scale.
			_scale = ( ( _winSize.Height / 2 ) - _orzSpace * 4 ) / ( 4 * textWidth );


			for (int i = 4; i > -1; i--) {
				_buttons [i] = new Button (_mainLayer, _touch, _actButtons [i], _pathButtons [i], _pathButtonsPressed [i], new CCPoint (3 * _vertSpace + 3 * 58 * _scale, _winSize.Height / 4 + ( textWidth * _scale + _orzSpace ) * ( ( i - 4 ) * -1 )), _winSize, -90, _scale);
			}
			for (int i = 9; i > 4; i--) {
				_buttons [i] = new Button (_mainLayer, _touch, _actButtons [i], _pathButtons [i], _pathButtonsPressed [i], new CCPoint (2 * _vertSpace + 2 * 58 * _scale, _winSize.Height / 4 + ( textWidth * _scale + _orzSpace ) * ( ( i - 9 ) * -1 )), _winSize, -90, _scale);
			}

			_buttons [10] = new Button (_mainLayer, _touch, _actButtons [10], _pathButtons [10], _pathButtonsPressed [10], new CCPoint (_vertSpace + 58 * _scale, _winSize.Height / 2 - _orzSpace / 2 - ( textWidth * _scale ) / 2), _winSize, -90, _scale);
			_buttons [11] = new Button (_mainLayer, _touch, _actButtons [11], _pathButtons [11], _pathButtonsPressed [11], new CCPoint (_vertSpace + 58 * _scale, _winSize.Height / 2 + _orzSpace / 2 + ( textWidth * _scale ) / 2), _winSize, -90, _scale);

			_slider = new Slider (_mainLayer, _touch, "sliderBar", "sliderBall", new CCPoint (5 * _vertSpace + 4 * 58 * _scale, _winSize.Height / 4 - 115 * _scale), _winSize, 61, 120, -90, _cardScale);
			_slider.visible = false;
			disableAllButtons ();


			#endregion

			#region Bid sprites initialization
			CCNode bid0 = new CCNode ();								//Instancing the father.
			bid0.Position = new CCPoint (_winSize.Width * 35 / 40, _winSize.Height * 21 / 24);	//Setting the father position to be the same as the name label position.
			bid0.Rotation = -90;									//Setting the father rotation to be the same as the name label rotation.
			_mainLayer.AddChild (bid0);								//Adding the father as a child of the mainlayer.
			_bidsFathers.Add (bid0);								//Adding the father to the list containing all the fathers.	
			_playerBids.Add (new CCSprite (""));							//Instancing a blank sprite.
			_playerBids [0].Position = new CCPoint (-45, -75);					//Setting the position.
			_playerBids [0].Scale = _cardScale * 0.65f;						//Setting the scale.
			bid0.AddChild (_playerBids [0]);							//Adding the sprite as a child of the father instanciated before.


			CCNode bid1 = new CCNode ();
			bid1.Position = _playerNames [1].Position;
			_mainLayer.AddChild (bid1);
			_bidsFathers.Add (bid1);
			_playerBids.Add (new CCSprite (""));
			_playerBids [1].Position = new CCPoint (-45, -75);
			_playerBids [1].Scale = _cardScale * 0.65f;
			_playerBids [1].Rotation = -90;
			bid1.AddChild (_playerBids [1]);

			CCNode bid2 = new CCNode ();
			bid2.Position = _playerNames [2].Position;
			bid2.Rotation = -90;
			_mainLayer.AddChild (bid2);
			_bidsFathers.Add (bid2);
			_playerBids.Add (new CCSprite (""));
			_playerBids [2].Position = new CCPoint (-45, -75);
			_playerBids [2].Scale = _cardScale * 0.65f;
			bid2.AddChild (_playerBids [2]);

			CCNode bid3 = new CCNode ();
			bid3.Position = _playerNames [3].Position;
			bid3.Rotation = -90;
			_mainLayer.AddChild (bid3);
			_bidsFathers.Add (bid3);
			_playerBids.Add (new CCSprite (""));
			_playerBids [3].Position = new CCPoint (-45, -75);
			_playerBids [3].Scale = _cardScale * 0.65f;
			bid3.AddChild (_playerBids [3]);

			CCNode bid4 = new CCNode ();
			bid4.Position = _playerNames [4].Position;
			bid4.Rotation = 180;
			_mainLayer.AddChild (bid4);
			_bidsFathers.Add (bid4);
			_playerBids.Add (new CCSprite (""));
			_playerBids [4].Position = new CCPoint (-45, -75);
			_playerBids [4].Scale = _cardScale * 0.65f;
			_playerBids [4].Rotation = -270;
			bid4.AddChild (_playerBids [4]);

			#endregion

			#region Debug bids labels
//			playerBids = new List <CCLabel> (5);
//
//			playerBids.Add (new CCLabel ("", "Arial", 12));
//
//			playerBids.Add (new CCLabel ("", "Arial", ( winSize.Width / 12 ) * 0.5f));
//			playerBids [1].Position = new CCPoint (playerNames [1].PositionX, playerNames [1].PositionY - 30);
//			mainLayer.AddChild (playerBids [1]);
//
//			playerBids.Add (new CCLabel ("", "Arial", ( winSize.Width / 12 ) * 0.5f));
//			playerBids [2].Position = new CCPoint (playerNames [2].PositionX + 30, playerNames [2].PositionY);
//			playerBids [2].Rotation = -90;
//			mainLayer.AddChild (playerBids [2]);
//
//			playerBids.Add (new CCLabel ("", "Arial", ( winSize.Width / 12 ) * 0.5f));
//			playerBids [3].Position = new CCPoint (playerNames [3].PositionX + 30, playerNames [3].PositionY);
//			playerBids [3].Rotation = -90;
//			mainLayer.AddChild (playerBids [3]);
//
//			playerBids.Add (new CCLabel ("", "Arial", ( winSize.Width / 12 ) * 0.5f));
//			playerBids [4].Position = new CCPoint (playerNames [4].PositionX, playerNames [4].PositionY + 30);
//			playerBids [4].Rotation = 180;
//			mainLayer.AddChild (playerBids [4]);
			#endregion

			#region End game label, buttons and sprites initialization
			_resultBoard = new CCSprite ("resultBoard");
			_resultBoard.Position = new CCPoint (_winSize.Width / 2, _winSize.Height / 2);
			_resultBoard.Scale = _scale * 0.38f;
			_resultBoard.Rotation = -90;
			_resultBoard.Visible = false;
			_mainLayer.AddChild (_resultBoard, 15);

			_endStatusSpriteWin = new CCSprite ("spriteVictory");
			_endStatusSpriteWin.Position = new CCPoint (_resultBoard.BoundingBox.Size.Width / 2, _resultBoard.BoundingBox.Size.Height * 4 / 5);
			_endStatusSpriteWin.Scale = _cardScale;
			_endStatusSpriteWin.Visible = false;
			_resultBoard.AddChild (_endStatusSpriteWin, 1);

			_endStatusSpriteLoss = new CCSprite ("spriteDefeat");
			_endStatusSpriteLoss.Position = new CCPoint (_resultBoard.BoundingBox.Size.Width / 2, _resultBoard.BoundingBox.Size.Height * 4 / 5);
			_endStatusSpriteLoss.Scale = _cardScale;
			_endStatusSpriteLoss.Visible = false;
			_resultBoard.AddChild (_endStatusSpriteLoss, 1);

			_btnExit = new Button (_resultBoard, _touch, actExit, "btnExit", "btnExitPressed", new CCPoint (_resultBoard.BoundingBox.Size.Width / 4, _resultBoard.BoundingBox.Size.Height / 5), _winSize, 0, 2.6f);
			_btnNext = new Button (_resultBoard, _touch, actNext, "btnNext", "btnNextPressed", new CCPoint (_resultBoard.BoundingBox.Size.Width * 3 / 4, _resultBoard.BoundingBox.Size.Height / 5), _winSize, 0, 2.6f);
			_btnExit.Enabled = false;
			_btnNext.Enabled = false;
			#endregion


			_wait = 0;			//Initializing the wait.
			_touchAsta = true;		//Setting the touch to be the asta's.

			Schedule (RunGameLogic);	//Start the method that will be called every frame.
			Board.Instance.start ();	//Start the board.

			_mainLayer.Color = new CCColor3B (6, 117, 21);	//Set the background color to green.
			_mainLayer.Opacity = 255;			//Alpha = 1.

			written = false;


			Board.Instance.Me.Controller = this;	//Set the controller for my player.
		}

		#region Gamelogic

		/// <summary>
		/// Game logic, will be executed every frame
		/// </summary>
		/// <param name="frameTimeInSeconds">Frame time in seconds</param>
		void RunGameLogic (float frameTimeInSeconds)
		{
			if (_wait > 0) {
				_wait -= frameTimeInSeconds;
			} else {
				_wait = 0;
				Board.Instance.update ();
			}

			if (Board.Instance.isGameFinish && !written) {
				written = true;
				_resultBoard.Visible = true;

				if (Board.Instance.Me.order != 0) {
					_btnExit.remove ();
					_btnNext.remove ();
					new Thread (restart).Start ();
				}

				_btnExit.Enabled = true;
				_btnNext.Enabled = true;
				bool inMano = false;
				GameData gd = Archive.Instance.lastGame ();
				if (gd.getWinners ().Contains (Board.Instance.Me))
					_endStatusSpriteWin.Visible = true;
				else
					_endStatusSpriteLoss.Visible = true;

				if (gd.isChiamataInMano) {
					inMano = true;
				}

				turnLight (-1);

				String str = "Il chiamante era " + gd.getChiamante ().name;
				str += inMano ? ( " e si è chiamato in mano:" ) : ( " e il socio era " + gd.getSocio ().name + ": " );
				str += inMano ? ( "ha " ) : ( "hanno " );
				str += "fatto " + gd.getChiamantePointCount () + " punti." + Environment.NewLine;
				str += "Gli altri giocatori (" + gd.getAltri () [0].name + ", " + gd.getAltri () [1].name + " e " + gd.getAltri () [2].name + ")" + Environment.NewLine + "hanno fatto " + gd.getAltriPointCount () + " punti.";
				CCLabel resultLbl = new CCLabel (str, "Roboto", _cardScale * 80f, new CCSize (_resultBoard.BoundingBox.Size.Width * 4 / 5, -1), CCTextAlignment.Center);

				resultLbl.Position = new CCPoint (_resultBoard.BoundingBox.Size.Width / 2, _resultBoard.BoundingBox.Size.Height * 43 / 80);
				_resultBoard.AddChild (resultLbl);
			}
					
			
		}

		#endregion

		#region Move and rotate methods

		/// <summary>
		/// Auxiliary method to move and rotate a sprite
		/// </summary>
		/// <param name="destination">Point of destination</param>
		/// <param name="sprite">The sprite to be moved and rotated</param>
		/// <param name="time">Animation time</param>
		/// <param name="rotation">Rotation of the sprite</param>
		private void moveSprite (CCPoint destination, CCSprite sprite, float time, float rotation)
		{

			CCRotateTo rotate = new CCRotateTo (time, rotation);
			CCMoveTo move = new CCMoveTo (time, new CCPoint (destination.X, destination.Y));
			CCSpawn actions = new CCSpawn (move, rotate);
			sprite.RunAction (actions);
		}

		/// <summary>
		/// Auxiliary method to only move a sprite
		/// </summary>
		/// <param name="destination">Point of destination</param>
		/// <param name="sprite">The sprite to be moved</param>
		/// <param name="time">Animation time</param>
		private void moveSprite (CCPoint destination, CCSprite sprite, float time = 0.3f)
		{
			CCMoveTo move = new CCMoveTo (time, new CCPoint (destination.X, destination.Y));
			sprite.RunAction (move);
		}

		/// <summary>
		/// Auxiliary method to only rotate a sprite
		/// </summary>
		/// <param name="sprite">The sprite to be rotated.</param>
		/// <param name="Rotation">Rotation of the sprite</param>
		/// <param name="time">Animation time</param>
		private void moveSprite (CCSprite sprite, float rotation, float time = 0.3f)
		{
			CCRotateTo rotate = new CCRotateTo (time, rotation);
			sprite.RunAction (rotate);
		}

		#endregion

		#region Debug

		#endregion

		#region Let there be light

		/// <summary>
		/// Method that shows the turn light of the player indicated by the argument.
		/// </summary>
		/// <param name="playerIndex">Local player index (0 is me,1 the guy to my left...).</param>
		private void turnLight (int playerIndex)
		{
			for (int i = 0; i < 5; i++) {
				_turnLights [i].Visible = false;
			}
			if (playerIndex >= 0)
				_turnLights [playerIndex].Visible = true;
		}

		#endregion

		#region Convert to localIndex

		/// <summary>
		/// Converts global index to local index.
		/// </summary>
		/// <returns>The local order of the player.</returns>
		/// <param name="p">The player.</param>
		private int playerToOrder (Player p)
		{
			return ( p.order - Board.Instance.Me.order + Board.PLAYER_NUMBER ) % Board.PLAYER_NUMBER;
		}

		/// <summary>
		/// Converts global index to local index.
		/// </summary>
		/// <returns>The local order of the player.</returns>
		/// <param name="i">The global index.</param>
		private int playerToOrder (int i)
		{
			return( Board.Instance.getPlayer (i).order - Board.Instance.Me.order + Board.PLAYER_NUMBER ) % Board.PLAYER_NUMBER;
		}

		#endregion

		#region Event responses

		#region Auction started

		/// <summary>
		/// Method to initialize the auction.
		/// </summary>
		private void auctionStarted ()
		{
			_touchAsta = true;

			_bidded = false;
			_initializedSeme = false;

			turnLight (playerToOrder (Board.Instance.ActiveAuctionPlayer));

			if (Board.Instance.ActiveAuctionPlayer != Board.Instance.Me)
				disableAllButtons ();
			else
				for (int i = 0; i < 12; i++)
					_buttons [i].Enabled = true;


		}

		#endregion

		#region Auction turn changed

		/// <summary>
		/// Method that shows a bid near the player.
		/// </summary>
		/// <param name="bid">The bid.</param>
		private void bidPlaced (IBid bid)
		{
			//TODO : disabilitare i pulsanti che non posso cliccare perchè puntano roba troppo alta
			if (!Board.Instance.isAuctionPhase || Board.Instance.ActiveAuctionPlayer != Board.Instance.Me) {
				disableAllButtons ();
			} else {
				enableAvaiableButtons ();
			}
			if (bid is PassBid)
				_playerBids [playerToOrder (bid.bidder.order)].Visible = false;
			else
				_playerBids [playerToOrder (bid.bidder.order)].Texture = new CCTexture2D ("cll_" + bidToString (bid));
			_wait = 0.4f;
			turnLight (( !Board.Instance.isAuctionPhase ? playerToOrder (Board.Instance.currentAuctionWinningBid.bidder) : playerToOrder (Board.Instance.ActiveAuctionPlayer) ));

		}

		private string bidToString (IBid bid)
		{
			if (bid is PassBid)
				return "PASSO";
			else if (bid is CarichiBid)
				return "Carichi al " + ( (CarichiBid) bid ).point.ToString ();
			else
				return ( (NormalBid) bid ).number.ToString ();
		}

		#endregion

		#region Auction ended

		/// <summary>
		/// Method to finalize the auction.
		/// </summary>
		private void auctionEnded ()
		{
			_touchAsta = false;
			for (int i = 0; i < 12; i++) {
				_buttons [i].remove ();
			}
			_slider.remove ();
		}

		#endregion

		#region Seme choosing

		/// <summary>
		/// Method to show the seme-choosing buttons.
		/// </summary>
		private void chooseSemeButtons ()
		{

			auctionEnded ();
			_chooseOri = new Button (_mainLayer, _touch, actOri, ( (NormalBid) Board.Instance.currentAuctionWinningBid ).number.ToString () + "_" + "ORI", ( (NormalBid) Board.Instance.currentAuctionWinningBid ).number.ToString () + "_" + "ORI", new CCPoint (_winSize.Width / 2, _winSize.Height / 2 + _cardScale * _carte [0].sprite.Texture.PixelsWide * 1.5f), _winSize, -90, _cardScale * 0.7f);
			_chooseCoppe = new Button (_mainLayer, _touch, actCoppe, ( (NormalBid) Board.Instance.currentAuctionWinningBid ).number.ToString () + "_" + "COPE", ( (NormalBid) Board.Instance.currentAuctionWinningBid ).number.ToString () + "_" + "COPE", new CCPoint (_winSize.Width / 2, _winSize.Height / 2 + _cardScale * _carte [0].sprite.Texture.PixelsWide * 0.5f), _winSize, -90, _cardScale * 0.7f);
			_chooseBastoni = new Button (_mainLayer, _touch, actBastoni, ( (NormalBid) Board.Instance.currentAuctionWinningBid ).number.ToString () + "_" + "BASTONI", ( (NormalBid) Board.Instance.currentAuctionWinningBid ).number.ToString () + "_" + "BASTONI", new CCPoint (_winSize.Width / 2, _winSize.Height / 2 - _cardScale * _carte [0].sprite.Texture.PixelsWide * 0.5f), _winSize, -90, _cardScale * 0.7f);
			_chooseSpade = new Button (_mainLayer, _touch, actSpade, ( (NormalBid) Board.Instance.currentAuctionWinningBid ).number.ToString () + "_" + "SPADE", ( (NormalBid) Board.Instance.currentAuctionWinningBid ).number.ToString () + "_" + "SPADE", new CCPoint (_winSize.Width / 2, _winSize.Height / 2 - _cardScale * _carte [0].sprite.Texture.PixelsWide * 1.5f), _winSize, -90, _cardScale * 0.7f);
		}

		#endregion

		#region Playtime started

		/// <summary>
		/// Method to initialize play time
		/// </summary>
		private void startPlaytime ()
		{
			if (Board.Instance.ActivePlayer == Board.Instance.Me)
				_touchAsta = false;
			else
				_touchAsta = true;


			for (int i = 0; i < Board.PLAYER_NUMBER; i++) {
				if (Board.Instance.getPlayer (i).Role == EnRole.CHIAMANTE) {
					CCSprite seme = new CCSprite ("cll_" + Board.Instance.Briscola.ToString ());
					seme.Position = new CCPoint (45, -75);
					seme.Scale = _cardScale * 0.65f;
					_bidsFathers [playerToOrder (i)].AddChild (seme);
				} else
					_playerBids [playerToOrder (i)].Visible = false;
			}

			turnLight (playerToOrder (Board.Instance.ActivePlayer.order));
		}

		#endregion

		#region I play a card

		/// <summary>
		/// Method to notify the board that i played a card.
		/// </summary>
		/// <returns>The card.</returns>
		public Card chooseCard ()
		{
			turnLight (0);
			if (_played) {
				_played = false;
				Card temp = Board.Instance.Me.InitialHand [_droppedCards [Board.Instance.numberOfCardOnBoard].index];
				_touchAsta = true;
				turnLight (1);
				_wait += 0.5f;
				return Board.Instance.getCard (temp.seme, temp.number);
			}

			_touchAsta = false;
			return null;
		}

		#endregion

		#region Play a card

		/// <summary>
		/// Method to show a card played by others.
		/// </summary>
		/// <param name="m">M.</param>
		private void playCard (Move m)
		{
			int localIndex = playerToOrder (m.player);
			CCSprite cardSprite = new CCSprite (m.card.number.ToString () + "_" + m.card.seme.ToString ());
			cardSprite.Scale = _cardScale * 0.7f;
			CardData cd;
			switch (localIndex) {
				case 1:
					cardSprite.Position = _offScreen [1];
					cardSprite.Rotation = 180;
					_mainLayer.AddChild (cardSprite);
					cd = new CardData (cardSprite, new CCPoint (_dropField.MaxX * 3 / 4, _winSize.Height - _dropField.MinY), 180, -1);
					_droppedCards.Add (cd);
					moveSprite (cd.posBase, cardSprite);
					_wait = 0.5f;
				break;
				case 2:
					cardSprite.Position = _offScreen [2];
					cardSprite.Rotation = 270;
					_mainLayer.AddChild (cardSprite);
					cd = new CardData (cardSprite, new CCPoint (_dropField.MaxX * 2 / 5, _dropField.MidY + 100), 270, -1);
					_droppedCards.Add (cd);
					moveSprite (cd.posBase, cardSprite);
					_wait = 0.5f;

				break;

				case 3:
					cardSprite.Position = _offScreen [3];
					cardSprite.Rotation = 270;
					_mainLayer.AddChild (cardSprite);
					cd = new CardData (cardSprite, new CCPoint (_dropField.MaxX * 2 / 5, _dropField.MidY - 100), 270, -1);
					_droppedCards.Add (cd);
					moveSprite (cd.posBase, cardSprite);
					_wait = 0.5f;

				break;

				case 4:
					cardSprite.Position = _offScreen [4];
					_mainLayer.AddChild (cardSprite);
					cd = new CardData (cardSprite, new CCPoint (_dropField.MaxX * 3 / 4, _winSize.Height - _dropField.MaxY), 0, -1);
					_droppedCards.Add (cd);
					moveSprite (cd.posBase, cardSprite);
					_wait = 0.5f;


				break;
			}

			if (Board.Instance.numberOfCardOnBoard == Board.PLAYER_NUMBER) {
				_wait += 1.5f;
			} else
				turnLight (( localIndex + 1 ) % Board.PLAYER_NUMBER);
				
				
				




		}

		#endregion

		#region Fine giro

		/// <summary>
		/// Clears the board.
		/// </summary>
		/// <param name="player">Player that wins the hand.</param>
		/// <param name="board">List of cards on the board.</param>
		public void clearBoard (Player player, List<Card> board)
		{
			for (int i = 4; i > -1; i--) {
				CCMoveTo move = new CCMoveTo (0.5f, _offScreen [playerToOrder (player)]);
				CCRemoveSelf delete = new CCRemoveSelf ();
				_droppedCards [i].sprite.RunActions (move, delete);
				_droppedCards.RemoveAt (i);
			}
			turnLight (playerToOrder (player));

			if (player == Board.Instance.Me)
				_touchAsta = false;
		}

		#endregion

		#endregion

	}




}
