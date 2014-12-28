using System;
using CocosSharp;
using System.Collections.Generic;
using ChiamataLibrary;

namespace Core
{
	public class GameScene : CCScene
	{
		//Core variables
		private CCLayer mainLayer;

		//Sprites and position variables
		private List<cardData> carte;
		private List<cardData> droppedCards;

		//Buttons
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
			"btnLascio"
		};
<<<<<<< HEAD
		Button [] buttons;
=======

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
			"btnLascioPressed"
		};
		Button[] buttons;
>>>>>>> Stash
		List<touchList.eventHandlerTouch> actButtons;


		//Touch helper variables
		private touchList touch;
		private int selected;
		private CCRect dropField;
		private CCRect cardField;
		private int inHand;

		//GameState variable
		//0 : Asta
		//10 : Game
		//20 : Punteggi
		private int _gameState;

		public int gameState{ get { return _gameState; } set { _gameState = value; } }

		// TODO : Sistemare questa winsize, non riesco a trovare un'altro modo per prenderla
		private CCSize winSize;


		//Debug
		CCLabel turn;
		CCLabel passed;



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
			touch = new touchList (this);
			touch.eventTouchBegan += touchBeganAsta;
			touch.eventTouchMoved += touchMovedAsta;
			touch.eventTouchEnded += touchEndedAsta;

			#region Card data initialization
			//Instancing the array of cards that will become childs of the mainLayer
			carte = new List<cardData> (8);
			droppedCards = new List<cardData> ();

			//Getting the window size
			winSize = mainWindow.WindowSizeInPixels;

			//Setting the area when cards can be dropped
			dropField = new CCRect (0, (int) ( winSize.Height / 4 ), (int) ( winSize.Width / 2 ), (int) ( winSize.Height / 2 ));

			//Setting the area where the cards can be re-arranged
			cardField = new CCRect ((int) ( winSize.Width * 3.5 / 5 ), (int) ( winSize.Height / 5 ), (int) ( winSize.Width * 1.5 / 5 ), (int) ( winSize.Height * 3 / 5 ));

			inHand = 8;
			#endregion

			#region Auction buttons initialization
			buttons = new Button[11];
			actButtons = new List<touchList.eventHandlerTouch> ();

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

<<<<<<< HEAD
			for (int i = 0; i < 11; i++) {
				buttons [i] = new Button (mainLayer, touch, actButtons [i], new CCSprite (pathButtons [i]), new CCPoint (winSize.Width / 7, winSize.Height * ( 11 - i ) / 12), winSize);
=======
			for (int i=0;i<11;i++){
				buttons[i]= new Button(mainLayer,touch,actButtons[i],pathButtons[i],pathButtonsPressed[i],new CCPoint(winSize.Width/7,winSize.Height*(11-i)/12),winSize);
>>>>>>> Stash
			}

			#endregion

			#region Sprites creation and positioning
			//Sprites creation
			CCPoint posBase;
			float rotation;
			for (int i = 0; i < 8; i++) {


				if (i == 0) {
					posBase = new CCPoint (winSize.Width - 100 + 3 * ( i * i - 7 * i + 12 ), winSize.Height / 4);

				} else {
					posBase = new CCPoint (winSize.Width - 100 + 3 * ( i * i - 7 * i + 12 ), carte [i - 1].posBase.Y + 50);
				}
				rotation = -90 - 4 * ( i > 3 ? 4 - i - 1 : 4 - i );
				carte.Add (new cardData (new CCSprite ("AsseBastoni"), posBase, rotation));
					
				//Positioning the cards in an arc shape, using a parabola constructed with the for index
				carte [i].sprite.Position = carte [i].posBase;
				carte [i].sprite.Rotation = carte [i].rotation;
				carte [i].sprite.Scale = 0.3f;

				mainLayer.AddChild (carte [i].sprite, i);
			}
			#endregion

			passed = new CCLabel ("Passed: ", "Arial", 12);
			passed.Position = new CCPoint (winSize.Width / 2, winSize.Height / 2);
			passed.Rotation = -90;
			mainLayer.AddChild (passed);

			turn = new CCLabel ("Turno: " + Board.Instance.ActiveAuctionPlayer.ToString (), "Arial", 12);
			turn.Position = new CCPoint (passed.Position.X + 15, passed.Position.Y);
			turn.Rotation = -90;
			mainLayer.AddChild (turn);





	
		}


		/// <summary>
		/// Game logic, will be executed every frame
		/// </summary>
		/// <param name="frameTimeInSeconds">Frame time in seconds</param>
		void RunGameLogic (float frameTimeInSeconds)
		{
		}

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
							cardData tempC = carte [1];
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
							cardData tempC = carte [inHand - 2];
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
							cardData tempC = carte [selected - 1];
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
							cardData tempC = carte [selected + 1];
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

		#region Touch listener game

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
							cardData tempC = carte [1];
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
							cardData tempC = carte [inHand - 2];
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
							cardData tempC = carte [selected - 1];
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
							cardData tempC = carte [selected + 1];
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

		#region Buttons actions

		//TODO : Cambiare i punti dentro queste funzioni con i punti presi dalla slider
		private void actLascio (List<CCTouch> touches, CCEvent touchEvent)
		{
			passed.Text = passed.Text + Board.Instance.ActiveAuctionPlayer + ", ";
			Board.Instance.auctionPass (Board.Instance.ActiveAuctionPlayer);
			if (Board.Instance.isAuctionClosed)
				switchState ();
			else
				turn.Text = "Turno: " + Board.Instance.ActiveAuctionPlayer.ToString ();
		}

		private void actAsse (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.ASSE, 61);
			Board.Instance.auctionPlaceABid (nb);
			turn.Text = "Turno: " + Board.Instance.ActiveAuctionPlayer.ToString ();
		}

		private void actTre (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.TRE, 61);
			Board.Instance.auctionPlaceABid (nb);
			turn.Text = "Turno: " + Board.Instance.ActiveAuctionPlayer.ToString ();
		}

		private void actDieci (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.RE, 61);
			Board.Instance.auctionPlaceABid (nb);
			turn.Text = "Turno: " + Board.Instance.ActiveAuctionPlayer.ToString ();
		}

		private void actNove (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.CAVALLO, 61);
			Board.Instance.auctionPlaceABid (nb);
			turn.Text = "Turno: " + Board.Instance.ActiveAuctionPlayer.ToString ();
		}

		private void actOtto (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.FANTE, 61);
			Board.Instance.auctionPlaceABid (nb);
			turn.Text = "Turno: " + Board.Instance.ActiveAuctionPlayer.ToString ();
		}

		private void actSette (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.SETTE, 61);
			Board.Instance.auctionPlaceABid (nb);
			turn.Text = "Turno: " + Board.Instance.ActiveAuctionPlayer.ToString ();
		}

		private void actSei (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.SEI, 61);
			Board.Instance.auctionPlaceABid (nb);
			turn.Text = "Turno: " + Board.Instance.ActiveAuctionPlayer.ToString ();
		}

		private void actCinque (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.CINQUE, 61);
			Board.Instance.auctionPlaceABid (nb);
			turn.Text = "Turno: " + Board.Instance.ActiveAuctionPlayer.ToString ();
		}

		private void actQuattro (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.QUATTRO, 61);
			Board.Instance.auctionPlaceABid (nb);
			turn.Text = "Turno: " + Board.Instance.ActiveAuctionPlayer.ToString ();
		}

		private void actDue (List<CCTouch> touches, CCEvent touchEvent)
		{
			NormalBid nb = new NormalBid (Board.Instance.ActiveAuctionPlayer, EnNumbers.DUE, 61);
			Board.Instance.auctionPlaceABid (nb);
			turn.Text = "Turno: " + Board.Instance.ActiveAuctionPlayer.ToString ();
		}

		#endregion

		private void switchState ()
		{
			switch (_gameState) {
				case 0:
					turn.Text = "Vincitore: " + Board.Instance.currentAuctionWinningBid.Bidder.ToString ();
					mainLayer.RemoveChild (passed);
					touch.eventTouchBegan -= touchBeganAsta;
					touch.eventTouchMoved -= touchMovedAsta;
					touch.eventTouchEnded -= touchEndedAsta;
					touch.eventTouchBegan += touchBeganGame;
					touch.eventTouchMoved += touchMovedGame;
					touch.eventTouchEnded += touchEndedGame;
					for (int i = 0; i < 11; i++) {
						buttons [i].remove ();
					}
					_gameState = 10;
				break;
				case 10:
				break;
			}

		}

	}


	class cardData
	{

		public  CCSprite sprite;

		private CCPoint _posBase;

		public CCPoint posBase{ get { return _posBase; } set { _posBase = value; } }

		private float _rotation;

		public float rotation{ get { return _rotation; } set { _rotation = value; } }

		public cardData (CCSprite s, CCPoint p, float r)
		{
			sprite = s;
			_posBase = p;
			_rotation = r;
		}
			
	}


}
