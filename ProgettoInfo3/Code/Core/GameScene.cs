using System;
using CocosSharp;
using System.Collections.Generic;
using ChiamataLibrary;

namespace Core
{
	public class GameScene : CCScene
	{
		//TODO: iscriversi ai vbellissimi eventi di scandi
		//TODO: fare arrivare le carte dal nulla
		//TODO: indicatore di a chi tocca
		//TODO: carte visibili sulla board
		//TODO: tasti su due righe e aggiungere carichi
		//TODO: controllare i readonly

		//Core variables
		private CCLayer mainLayer;

		//Sprites and position variables
		private List<CardData> carte;
		private List<CardData> droppedCards;
		private CCSize winSize;

		#region Buttons
		private static String [] pathButtons = {
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


		private static String [] pathButtonsPressed = {
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

		private Button [] buttons;
		private const float vertSpace = 0.05f * 58;
		private const float orzSpace = 0.04f * 115;

		private List<TouchList.eventHandlerTouch> actButtons;

		private Slider slider;

		#region Buttons actions

		private void actLascio (List<CCTouch> touches, CCEvent touchEvent)
		{

			Board.Instance.auctionPass (Board.Instance.ActiveAuctionPlayer);
			if (Board.Instance.isAuctionClosed)
				switchState ();
		}

		private void actCarichi(List<CCTouch> touches, CCEvent touchEvent){
			BidCarichi cb = new BidCarichi (Board.Instance.ActiveAuctionPlayer,slider.currentValue);
			Board.Instance.auctionPlaceABid (cb);
		}

		private void actAsse (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.ASSE, slider.currentValue);
			Board.Instance.auctionPlaceABid (nb);
		}

		private void actTre (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.TRE, slider.currentValue);
			Board.Instance.auctionPlaceABid (nb);
		}

		private void actDieci (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.RE, slider.currentValue);
			Board.Instance.auctionPlaceABid (nb);
		}

		private void actNove (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.CAVALLO, slider.currentValue);
			Board.Instance.auctionPlaceABid (nb);
		}

		private void actOtto (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.FANTE, slider.currentValue);
			Board.Instance.auctionPlaceABid (nb);
		}

		private void actSette (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.SETTE, slider.currentValue);
			Board.Instance.auctionPlaceABid (nb);
		}

		private void actSei (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.SEI, slider.currentValue);
			Board.Instance.auctionPlaceABid (nb);
		}

		private void actCinque (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.CINQUE, slider.currentValue);
			Board.Instance.auctionPlaceABid (nb);
		}

		private void actQuattro (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.QUATTRO, slider.currentValue);
			Board.Instance.auctionPlaceABid (nb);
		}

		private void actDue (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.DUE, slider.currentValue);
			Board.Instance.auctionPlaceABid (nb);
		}

		#endregion
		#endregion

		#region Touch
		//Touch Listener
		private TouchList touch;
		//Index of the selected card
		private int selected;
		//Rectangle defining where the cards can be dropped
		private CCRect dropField;
		//Rectangle defining where the cards can be rearranged
		private CCRect cardField;
		//Number of cards in hand
		private int inHand;

		#region Touch listener asta

		/// <summary>
		/// Function executed on the starting touch
		/// </summary>
		/// <param name="touches">List of touches</param>
		/// <param name="touchEvent">Touch event</param>
		void touchBeganAsta (List<CCTouch> touches, CCEvent touchEvent)
		{


			CCPoint pos = touches [0].LocationOnScreen;
			CCPoint posToParent = new CCPoint (pos.X, winSize.Height - pos.Y);

			//Checking on wich card the touch is positioned
			//I'm doing this in reverse because the 7th card is the one in the foreground and the 0th card is the one in the background
			int i;
			selected = -1;
			for (i = inHand - 1; i >= 0; i--) {
				if (carte [i].sprite.BoundingBoxTransformedToParent.ContainsPoint (posToParent)) {
					selected = i;
					break;
				}

			}
		}

		/// <summary>
		/// Function executed on the touch movement
		/// </summary>
		/// <param name="touches">List of touches</param>
		/// <param name="touchEvent">Touch event</param>
		void touchMovedAsta (List<CCTouch> touches, CCEvent touchEvent)
		{
			//Moving the sprite following exactly the touch
			CCPoint pos = touches [0].LocationOnScreen;

			//Returning the card to their place if the bound of the rectangle are surpassed
			if (!cardField.ContainsPoint (pos) && selected >= 0) {
				moveSprite (carte [selected].posBase, carte [selected].sprite);
				selected = -1;
			}

			//Only if a touch has begun and a card has been selected
			if (selected >= 0) {
				//Inverting Y cuz the image is referred to the bottom-left corner and the touch is referred to the top-left corner
				carte [selected].sprite.Position = new CCPoint (pos.X, winSize.Height - pos.Y);

				#region Card rearrangement
				//Checking if player wants to rearrange the cards

				//Every case has the same pattern :
				//1 - Check if the card has been moved beyond the next one : player wants to swap those cards
				//2 - Switch che posBase values
				//3 - If card has not moved yet, make it move
				//4 - Swap depth
				//5 - Swap cardData elements
				if (cardField.ContainsPoint (pos) && inHand > 1) {
					if (selected == 0) {
						if (( winSize.Height - pos.Y ) > carte [1].posBase.Y + 8) {
							//Swap base position
							CCPoint tempP = carte [1].posBase;
							carte [1].posBase = carte [0].posBase;
							carte [0].posBase = tempP;

							//Swap base rotation
							float tempR = carte [1].rotation;
							carte [1].rotation = carte [0].rotation;
							carte [0].rotation = tempR;

							//Move and rotate sprite
							if (!carte [1].posBase.Equals (carte [1].sprite.Position)) {
								moveSprite (carte [0].sprite, carte [0].rotation);
								moveSprite (carte [1].posBase, carte [1].sprite, 0.3f, carte [1].rotation);
							}


							//Swap zOrder
							carte [0].sprite.ZOrder = 1;
							carte [1].sprite.ZOrder = 0;

							//Swap cardData entry
							CardData tempC = carte [1];
							carte [1] = carte [0];
							carte [0] = tempC;

							//Now the selected card is in another position
							selected = 1;
						}
					} else if (selected == inHand - 1) {
						if (( winSize.Height - pos.Y ) < carte [selected - 1].posBase.Y - 8) {
							//Swap base position
							CCPoint tempP = carte [inHand - 2].posBase;
							carte [inHand - 2].posBase = carte [inHand - 1].posBase;
							carte [inHand - 1].posBase = tempP;

							//Swap base rotation
							float tempR = carte [inHand - 2].rotation;
							carte [inHand - 2].rotation = carte [inHand - 1].rotation;
							carte [inHand - 1].rotation = tempR;

							//Move and rotate sprite
							if (!carte [inHand - 2].posBase.Equals (carte [inHand - 2].sprite.Position)) {
								moveSprite (carte [inHand - 1].sprite, carte [inHand - 1].rotation);
								moveSprite (carte [inHand - 2].posBase, carte [inHand - 2].sprite, 0.3f, carte [inHand - 2].rotation);
							}


							//Swap zOrder
							carte [inHand - 1].sprite.ZOrder = inHand - 2;
							carte [inHand - 2].sprite.ZOrder = inHand - 1;

							//Swap cardData entry
							CardData tempC = carte [inHand - 2];
							carte [inHand - 2] = carte [inHand - 1];
							carte [inHand - 1] = tempC;

							//Now the selected card is in another position
							selected = inHand - 2;
						}
					} else {
						if (( winSize.Height - pos.Y ) < carte [selected - 1].posBase.Y - 8) {
							//Swap base position
							CCPoint tempP = carte [selected - 1].posBase;
							carte [selected - 1].posBase = carte [selected].posBase;
							carte [selected].posBase = tempP;

							//Swap base rotation
							float tempR = carte [selected - 1].rotation;
							carte [selected - 1].rotation = carte [selected].rotation;
							carte [selected].rotation = tempR;

							//Move and rotate sprite
							if (!carte [selected - 1].posBase.Equals (carte [selected - 1].sprite.Position)) {
								moveSprite (carte [selected].sprite, carte [selected].rotation);
								moveSprite (carte [selected - 1].posBase, carte [selected - 1].sprite, 0.3f, carte [selected - 1].rotation);
							}

							//Swap zOrder
							carte [selected].sprite.ZOrder = selected - 1;
							carte [selected - 1].sprite.ZOrder = selected;

							//Swap cardData entry
							CardData tempC = carte [selected - 1];
							carte [selected - 1] = carte [selected];
							carte [selected] = tempC;

							//Now the selected card is in another position
							selected = selected - 1;
						}

						if (( winSize.Height - pos.Y ) > carte [selected + 1].posBase.Y + 8) {
							//Swap base position
							CCPoint tempP = carte [selected + 1].posBase;
							carte [selected + 1].posBase = carte [selected].posBase;
							carte [selected].posBase = tempP;

							//Swap base rotation
							float tempR = carte [selected + 1].rotation;
							carte [selected + 1].rotation = carte [selected].rotation;
							carte [selected].rotation = tempR;

							//Move and rotate sprite
							if (!carte [selected + 1].posBase.Equals (carte [selected + 1].sprite.Position)) {
								moveSprite (carte [selected].sprite, carte [selected].rotation);
								moveSprite (carte [selected + 1].posBase, carte [selected + 1].sprite, 0.3f, carte [selected + 1].rotation);
							}


							//Swap zOrder
							carte [selected + 1].sprite.ZOrder = selected;
							carte [selected].sprite.ZOrder = selected + 1;

							//Swap cardData entry
							CardData tempC = carte [selected + 1];
							carte [selected + 1] = carte [selected];
							carte [selected] = tempC;

							//Now the selected card is in another position
							selected = selected + 1;
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
		void touchEndedAsta (List<CCTouch> touches, CCEvent touchEvent)
		{
			CCPoint pos = touches [0].LocationOnScreen;
			if (selected >= 0) {
				if (dropField.ContainsPoint (pos)) {
					carte [selected].posBase = new CCPoint (dropField.Center.X, winSize.Height - dropField.Center.Y);
					moveSprite (new CCPoint (carte [selected].posBase.X, carte [selected].posBase.Y), carte [selected].sprite);
					droppedCards.Add (carte [selected]);
					carte.RemoveAt (selected);
					inHand--;

				} else {
					moveSprite (new CCPoint (carte [selected].posBase.X, carte [selected].posBase.Y), carte [selected].sprite);
				}
				selected = -1;
			}



		}

		#endregion

		#region Touch listener current player turn

		/// <summary>
		/// Function executed on the starting touch
		/// </summary>
		/// <param name="touches">List of touches</param>
		/// <param name="touchEvent">Touch event</param>
		void touchBeganGame (System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
		{


			CCPoint pos = touches [0].LocationOnScreen;
			CCPoint posToParent = new CCPoint (pos.X, winSize.Height - pos.Y);

			//Checking on wich card the touch is positioned
			//I'm doing this in reverse because the 7th card is the one in the foreground and the 0th card is the one in the background
			int i;
			selected = -1;
			for (i = inHand - 1; i >= 0; i--) {
				if (carte [i].sprite.BoundingBoxTransformedToParent.ContainsPoint (posToParent)) {
					selected = i;
					break;
				}

			}
		}

		/// <summary>
		/// Function executed on the touch movement
		/// </summary>
		/// <param name="touches">List of touches</param>
		/// <param name="touchEvent">Touch event</param>
		void touchMovedGame (System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
		{
			//Moving the sprite following exactly the touch
			CCPoint pos = touches [0].LocationOnScreen;

			//Only if a touch has begun and a card has been selected
			if (selected >= 0) {
				//Inverting Y cuz the image is referred to the bottom-left corner and the touch is referred to the top-left corner
				carte [selected].sprite.Position = new CCPoint (pos.X, winSize.Height - pos.Y);

				#region Card rearrangement
				//Checking if player wants to rearrange the cards

				//Every case has the same pattern :
				//1 - Check if the card has been moved beyond the next one : player wants to swap those cards
				//2 - Switch che posBase values
				//3 - If card has not moved yet, make it move
				//4 - Swap depth
				//5 - Swap cardData elements
				if (cardField.ContainsPoint (pos) && inHand > 1) {
					if (selected == 0) {
						if (( winSize.Height - pos.Y ) > carte [1].posBase.Y + 8) {
							//Swap base position
							CCPoint tempP = carte [1].posBase;
							carte [1].posBase = carte [0].posBase;
							carte [0].posBase = tempP;

							//Swap base rotation
							float tempR = carte [1].rotation;
							carte [1].rotation = carte [0].rotation;
							carte [0].rotation = tempR;

							//Move and rotate sprite
							if (!carte [1].posBase.Equals (carte [1].sprite.Position)) {
								moveSprite (carte [0].sprite, carte [0].rotation);
								moveSprite (carte [1].posBase, carte [1].sprite, 0.3f, carte [1].rotation);
							}


							//Swap zOrder
							carte [0].sprite.ZOrder = 1;
							carte [1].sprite.ZOrder = 0;

							//Swap cardData entry
							CardData tempC = carte [1];
							carte [1] = carte [0];
							carte [0] = tempC;

							//Now the selected card is in another position
							selected = 1;
						}
					} else if (selected == inHand - 1) {
						if (( winSize.Height - pos.Y ) < carte [selected - 1].posBase.Y - 8) {
							//Swap base position
							CCPoint tempP = carte [inHand - 2].posBase;
							carte [inHand - 2].posBase = carte [inHand - 1].posBase;
							carte [inHand - 1].posBase = tempP;

							//Swap base rotation
							float tempR = carte [inHand - 2].rotation;
							carte [inHand - 2].rotation = carte [inHand - 1].rotation;
							carte [inHand - 1].rotation = tempR;

							//Move and rotate sprite
							if (!carte [inHand - 2].posBase.Equals (carte [inHand - 2].sprite.Position)) {
								moveSprite (carte [inHand - 1].sprite, carte [inHand - 1].rotation);
								moveSprite (carte [inHand - 2].posBase, carte [inHand - 2].sprite, 0.3f, carte [inHand - 2].rotation);
							}


							//Swap zOrder
							carte [inHand - 1].sprite.ZOrder = inHand - 2;
							carte [inHand - 2].sprite.ZOrder = inHand - 1;

							//Swap cardData entry
							CardData tempC = carte [inHand - 2];
							carte [inHand - 2] = carte [inHand - 1];
							carte [inHand - 1] = tempC;

							//Now the selected card is in another position
							selected = inHand - 2;
						}
					} else {
						if (( winSize.Height - pos.Y ) < carte [selected - 1].posBase.Y - 8) {
							//Swap base position
							CCPoint tempP = carte [selected - 1].posBase;
							carte [selected - 1].posBase = carte [selected].posBase;
							carte [selected].posBase = tempP;

							//Swap base rotation
							float tempR = carte [selected - 1].rotation;
							carte [selected - 1].rotation = carte [selected].rotation;
							carte [selected].rotation = tempR;

							//Move and rotate sprite
							if (!carte [selected - 1].posBase.Equals (carte [selected - 1].sprite.Position)) {
								moveSprite (carte [selected].sprite, carte [selected].rotation);
								moveSprite (carte [selected - 1].posBase, carte [selected - 1].sprite, 0.3f, carte [selected - 1].rotation);
							}

							//Swap zOrder
							carte [selected].sprite.ZOrder = selected - 1;
							carte [selected - 1].sprite.ZOrder = selected;

							//Swap cardData entry
							CardData tempC = carte [selected - 1];
							carte [selected - 1] = carte [selected];
							carte [selected] = tempC;

							//Now the selected card is in another position
							selected = selected - 1;
						}

						if (( winSize.Height - pos.Y ) > carte [selected + 1].posBase.Y + 8) {
							//Swap base position
							CCPoint tempP = carte [selected + 1].posBase;
							carte [selected + 1].posBase = carte [selected].posBase;
							carte [selected].posBase = tempP;

							//Swap base rotation
							float tempR = carte [selected + 1].rotation;
							carte [selected + 1].rotation = carte [selected].rotation;
							carte [selected].rotation = tempR;

							//Move and rotate sprite
							if (!carte [selected + 1].posBase.Equals (carte [selected + 1].sprite.Position)) {
								moveSprite (carte [selected].sprite, carte [selected].rotation);
								moveSprite (carte [selected + 1].posBase, carte [selected + 1].sprite, 0.3f, carte [selected + 1].rotation);
							}


							//Swap zOrder
							carte [selected + 1].sprite.ZOrder = selected;
							carte [selected].sprite.ZOrder = selected + 1;

							//Swap cardData entry
							CardData tempC = carte [selected + 1];
							carte [selected + 1] = carte [selected];
							carte [selected] = tempC;

							//Now the selected card is in another position
							selected = selected + 1;
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
		void touchEndedGame (System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
		{
			CCPoint pos = touches [0].LocationOnScreen;
			if (selected >= 0) {
				if (dropField.ContainsPoint (pos)) {
					carte [selected].posBase = new CCPoint (dropField.Center.X, winSize.Height - dropField.Center.Y);
					moveSprite (new CCPoint (carte [selected].posBase.X, carte [selected].posBase.Y), carte [selected].sprite);
					droppedCards.Add (carte [selected]);
					carte.RemoveAt (selected);
					inHand--;

				} else {
					moveSprite (new CCPoint (carte [selected].posBase.X, carte [selected].posBase.Y), carte [selected].sprite);
				}
				selected = -1;
			}



		}

		#endregion

		#endregion

		//GameState variable
		//0 : Asta
		//10 : Game
		//20 : Punteggi
		private int _gameState;
		public int gameState{ get { return _gameState; } set { _gameState = value; } }

		#region Bluetooth



		#endregion

		/// <summary>
		/// Gamescene constructor, initializes sprites to their default position
		/// </summary>
		/// <param name="mainWindow">Main window.</param>
		public GameScene (CCWindow mainWindow) : base (mainWindow)
		{

			#region Game setup
			_gameState = 0;

			//TODO : Passare un parametor alla gamescene per permettergli scegliere il mazziere
			Board.Instance.initialize (new string[]{ "A", "B", "C", "D", "E" }, 2);//il mazziere è C
			#endregion


			//Instancing the layer and setting him as a child of the mainWindow
			mainLayer = new CCLayer ();
			AddChild (mainLayer);


			//Instancing the touch listener
			touch = new TouchList (this);
			touch.eventTouchBegan += touchBeganAsta;
			touch.eventTouchMoved += touchMovedAsta;
			touch.eventTouchEnded += touchEndedAsta;

			#region Card data initialization
			//Instancing the array of cards that will become childs of the mainLayer
			carte = new List<CardData> (8);
			droppedCards = new List<CardData> ();

			//Getting the window size
			winSize = mainWindow.WindowSizeInPixels;

			//Setting the area when cards can be dropped
			dropField = new CCRect (0, (int) ( winSize.Height / 4 ), (int) ( winSize.Width / 2 ), (int) ( winSize.Height / 2 ));

			//Setting the area where the cards can be re-arranged
			cardField = new CCRect ((int) ( winSize.Width * 3.5 / 5 ), (int) ( winSize.Height / 5 ), (int) ( winSize.Width * 1.5 / 5 ), (int) ( winSize.Height * 3 / 5 ));

			inHand = 8;
			#endregion

			#region Auction buttons initialization
			buttons = new Button[12];
			actButtons = new List<TouchList.eventHandlerTouch> ();

			actButtons.Add (actDue);
			actButtons.Add (actQuattro);
			actButtons.Add (actCinque);
			actButtons.Add (actSei);
			actButtons.Add (actSette);
			actButtons.Add (actOtto);
			actButtons.Add (actNove);
			actButtons.Add (actDieci);
			actButtons.Add (actTre);
			actButtons.Add (actAsse);
			actButtons.Add (actLascio);
			actButtons.Add (actCarichi);

			int textWidth =new CCTexture2D("btnLascio").PixelsWide;

			float scale =((winSize.Height/2)-orzSpace*4)/(4*textWidth);

			//FIXME : controllare le posizioni dei bottoni nel caso si cambiassero le textures
			for (int i = 4; i > -1; i--) {
				buttons[i]= new Button(mainLayer,touch,actButtons[i],pathButtons[i],pathButtonsPressed[i],new CCPoint(3*vertSpace+3*58*scale,winSize.Height/4+(textWidth*scale+orzSpace)*((i-4)*-1)),winSize,-90,scale);
			}
			for (int i = 9; i>4; i--) {
				buttons[i]= new Button(mainLayer,touch,actButtons[i],pathButtons[i],pathButtonsPressed[i],new CCPoint(2*vertSpace+2*58*scale,winSize.Height/4+(textWidth*scale+orzSpace)*((i-9)*-1)),winSize,-90,scale);
			}

			buttons[10]= new Button(mainLayer,touch,actButtons[10],pathButtons[10],pathButtonsPressed[10],new CCPoint(vertSpace+58*scale,winSize.Height/2-orzSpace/2-(textWidth*scale)/2),winSize,-90,scale);
			buttons[11]= new Button(mainLayer,touch,actButtons[11],pathButtons[11],pathButtonsPressed[11],new CCPoint(vertSpace+58*scale,winSize.Height/2+orzSpace/2+(textWidth*scale)/2),winSize,-90,scale);

			slider = new Slider(mainLayer,touch,"sliderBar","sliderBall",new CCPoint(5*vertSpace+4*58*scale,winSize.Height/4-115*scale),winSize,61,120);


			#endregion

			#region Sprites creation and positioning
			//Sprites creation
			CCPoint posBase;
			float rotation;
			for (int i = 0; i < 8; i++) {


				if (i == 0) {
					posBase = new CCPoint (winSize.Width - 50 + 3 * ( i * i - 7 * i + 12 ), winSize.Height / 4);

				} else {
					posBase = new CCPoint (winSize.Width - 50 + 3 * ( i * i - 7 * i + 12 ), carte [i - 1].posBase.Y + 50);
				}
				rotation = -90 - 4 * ( i > 3 ? 4 - i - 1 : 4 - i );
				carte.Add (new CardData (new CCSprite ("AsseBastoni"), posBase, rotation));
					
				//Positioning the cards in an arc shape, using a parabola constructed with the for index
				carte [i].sprite.Position = carte [i].posBase;
				carte [i].sprite.Rotation = carte [i].rotation;
				carte [i].sprite.Scale = 0.3f;

				mainLayer.AddChild (carte [i].sprite, i);
			}
			#endregion







	
		}


		/// <summary>
		/// Game logic, will be executed every frame
		/// </summary>
		/// <param name="frameTimeInSeconds">Frame time in seconds</param>
		void RunGameLogic (float frameTimeInSeconds)
		{
		}



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

		private void switchState ()
		{
			switch (_gameState) {
				case 0:
					touch.eventTouchBegan -= touchBeganAsta;
					touch.eventTouchMoved -= touchMovedAsta;
					touch.eventTouchEnded -= touchEndedAsta;
					touch.eventTouchBegan += touchBeganGame;
					touch.eventTouchMoved += touchMovedGame;
					touch.eventTouchEnded += touchEndedGame;
					for (int i = 0; i < 12; i++) {
						buttons [i].remove ();
					}
					_gameState = 10;
				break;
				case 10:
				break;
			}

		}

		#region Debug
		#endregion

	}




}
