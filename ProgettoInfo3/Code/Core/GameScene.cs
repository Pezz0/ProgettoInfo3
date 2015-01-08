using System;
using CocosSharp;
using System.Collections.Generic;
using ChiamataLibrary;

namespace Core
{
	public class GameScene : CCScene
	{
		//TODO: controllare i readonly

		//Core variables
		private CCLayer mainLayer;

		//Sprites and position variables
		private List<CardData> carte;
		private List<CardData> droppedCards;
		private CCPoint [] offScreen;
		private bool played;
		private float wait;
		private CCSize winSize;

		//Light
		private List<CCSprite> turnLights;

		//Names
		private List<CCLabel> playerNames;
		private List<CCLabel> playerBids;


		#region Buttons

		private IBid myBid;
		private EnSemi? mySeme;
		private bool bidded;
		private bool initializedSeme;
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
		private Button chooseOri;
		private Button chooseSpade;
		private Button chooseBastoni;
		private Button chooseCoppe;
		private const float vertSpace = 0.05f * 58;
		private const float orzSpace = 0.04f * 115;

		private List<TouchList.eventHandlerTouch> actButtons;

		private Slider slider;

		#region Buttons actions

		private void actLascio (List<CCTouch> touches, CCEvent touchEvent)
		{
			myBid = new PassBid ();
			bidded = true;
			auctionEnded ();
		}

		private void actCarichi (List<CCTouch> touches, CCEvent touchEvent)
		{
			myBid = new CarichiBid (Board.Instance.Me, slider.currentValue);
			bidded = true;

		}

		private void actAsse (List<CCTouch> touches, CCEvent touchEvent)
		{
			myBid = new NormalBid (Board.Instance.Me, EnNumbers.ASSE, slider.currentValue);
			bidded = true;
		}

		private void actTre (List<CCTouch> touches, CCEvent touchEvent)
		{
			myBid = new NormalBid (Board.Instance.Me, EnNumbers.TRE, slider.currentValue);
			bidded = true;
		}

		private void actDieci (List<CCTouch> touches, CCEvent touchEvent)
		{
			myBid = new NormalBid (Board.Instance.Me, EnNumbers.RE, slider.currentValue);
			bidded = true;
		}

		private void actNove (List<CCTouch> touches, CCEvent touchEvent)
		{
			myBid = new NormalBid (Board.Instance.Me, EnNumbers.CAVALLO, slider.currentValue);
			bidded = true;
		}

		private void actOtto (List<CCTouch> touches, CCEvent touchEvent)
		{
			myBid = new NormalBid (Board.Instance.Me, EnNumbers.FANTE, slider.currentValue);
			bidded = true;
		}

		private void actSette (List<CCTouch> touches, CCEvent touchEvent)
		{
			myBid = new NormalBid (Board.Instance.Me, EnNumbers.SETTE, slider.currentValue);
			bidded = true;
		}

		private void actSei (List<CCTouch> touches, CCEvent touchEvent)
		{
			myBid = new NormalBid (Board.Instance.Me, EnNumbers.SEI, slider.currentValue);
			bidded = true;
		}

		private void actCinque (List<CCTouch> touches, CCEvent touchEvent)
		{
			myBid = new NormalBid (Board.Instance.Me, EnNumbers.CINQUE, slider.currentValue);
			bidded = true;
		}

		private void actQuattro (List<CCTouch> touches, CCEvent touchEvent)
		{
			myBid = new NormalBid (Board.Instance.Me, EnNumbers.QUATTRO, slider.currentValue);
			bidded = true;
		}

		private void actDue (List<CCTouch> touches, CCEvent touchEvent)
		{
			myBid = new NormalBid (Board.Instance.Me, EnNumbers.DUE, slider.currentValue);
			bidded = true;
		}

		private void actOri (List<CCTouch> touches, CCEvent touchEvent)
		{
			mySeme = EnSemi.ORI;
			bidded = true;
		}

		private void actBastoni (List<CCTouch> touches, CCEvent touchEvent)
		{
			mySeme = EnSemi.BASTONI;
			bidded = true;
		}

		private void actCoppe (List<CCTouch> touches, CCEvent touchEvent)
		{
			mySeme = EnSemi.COPE;
			bidded = true;
		}

		private void actSpade (List<CCTouch> touches, CCEvent touchEvent)
		{
			mySeme = EnSemi.SPADE;
			bidded = true;
		}

		#endregion

		#region Delegate controller bid and seme

		public IBid bidController ()
		{
			if (bidded) {
				bidded = false;
				return myBid;
			}
			return null;
		}

		public EnSemi? semiController ()
		{
			if (!initializedSeme) {
				chooseSemeButtons ();
				initializedSeme = true;
			}
			if (bidded) {
				bidded = false;
				chooseOri.remove ();
				chooseCoppe.remove ();
				chooseBastoni.remove ();
				chooseSpade.remove ();
				return mySeme;
			}
			return null;
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

					if (selected < inHand / 2 && selected > 0) {
						for (int i = 0; i <= selected - 1; i++) {
							carte [i].posBase = carte [i + 1].posBase;
							carte [i].rotation = carte [i + 1].rotation;
							carte [i].sprite.ZOrder++;
							moveSprite (carte [i].posBase, carte [i].sprite, 0.3f, carte [i].rotation);
						}
					} else if (selected >= inHand / 2 && selected < inHand - 1) {
						for (int i = inHand - 1; i >= selected + 1; i--) {
							carte [i].posBase = carte [i - 1].posBase;
							carte [i].rotation = carte [i - 1].rotation;
							carte [i].sprite.ZOrder--;
							moveSprite (carte [i].posBase, carte [i].sprite, 0.3f, carte [i].rotation);
						}
					}
					carte [selected].posBase = new CCPoint (dropField.MaxX, winSize.Height - dropField.Center.Y);
					moveSprite (new CCPoint (carte [selected].posBase.X, carte [selected].posBase.Y), carte [selected].sprite);
					droppedCards.Add (carte [selected]);
					played = true;

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


			//Instancing the layer and setting him as a child of the mainWindow
			mainLayer = new CCLayer ();
			AddChild (mainLayer);


			//Instancing the touch listener
			touch = new TouchList (this);


			#region Card data initialization
			//Instancing the array of cards that will become childs of the mainLayer
			carte = new List<CardData> (8);
			droppedCards = new List<CardData> (5);

			//Getting the window size
			winSize = mainWindow.WindowSizeInPixels;

			//Setting the area when cards can be dropped
			dropField = new CCRect (0, (int) ( winSize.Height / 4 ), (int) ( winSize.Width / 2 ), (int) ( winSize.Height / 2 ));

			//Setting the area where the cards can be re-arranged
			cardField = new CCRect ((int) ( winSize.Width * 3.5 / 5 ), (int) ( winSize.Height / 5 ), (int) ( winSize.Width * 1.5 / 5 ), (int) ( winSize.Height * 3 / 5 ));

			//Setting the "off-Screen" position for the cards
			offScreen = new CCPoint[5];
			offScreen [0] = new CCPoint (winSize.Width + 200, winSize.Height / 2);
			offScreen [1] = new CCPoint (winSize.Width / 2, winSize.Height + 100);
			offScreen [2] = new CCPoint (-100, winSize.Height * 3 / 4);
			offScreen [3] = new CCPoint (-100, winSize.Height / 4);
			offScreen [4] = new CCPoint (winSize.Width / 2, -100);

			inHand = 8;
			#endregion

			#region Events subscriptions
			Board.Instance.eventAuctionStart += auctionStarted;
			Board.Instance.eventSomeonePlaceABid += bidPlaced;
			Board.Instance.eventPlaytimeStart += startPlaytime;
			Board.Instance.Me.setPlaytimeControl (iPlayCard);
			Board.Instance.eventSomeonePlayACard += playCard;
			Board.Instance.eventPickTheBoard += clearBoard;
			#endregion

			#region Light initialization
			turnLights = new List<CCSprite> (5);
			turnLights.Add (new CCSprite ("turnLight2"));
			turnLights [0].Position = new CCPoint (winSize.Width - turnLights [0].ContentSize.Height / 2, winSize.Height / 2);
			turnLights [0].BlendFunc = CCBlendFunc.Additive;
			turnLights [0].Rotation = -90;
			turnLights [0].Color = CCColor3B.Green;
			turnLights [0].ScaleX = winSize.Height / turnLights [0].ContentSize.Width;
			mainLayer.AddChild (turnLights [0]);
			turnLights [0].ZOrder = -1;
			turnLights [0].Visible = false;

			turnLights.Add (new CCSprite ("turnLight2"));
			turnLights [1].Position = new CCPoint (winSize.Width / 2, winSize.Height + 5 - turnLights [1].ContentSize.Height / 2);
			turnLights [1].BlendFunc = CCBlendFunc.Additive;
			turnLights [1].Rotation = 180;
			turnLights [1].Color = CCColor3B.Red;
			turnLights [1].ScaleX = winSize.Width / turnLights [1].ContentSize.Width;
			mainLayer.AddChild (turnLights [1]);
			turnLights [1].ZOrder = 20;
			turnLights [1].Visible = false;

			turnLights.Add (new CCSprite ("turnLight2"));
			turnLights [2].Position = new CCPoint (-5 + turnLights [2].ContentSize.Height / 2, winSize.Height * 3 / 4);
			turnLights [2].BlendFunc = CCBlendFunc.Additive;
			turnLights [2].Rotation = 90;
			turnLights [2].Color = CCColor3B.Yellow;
			turnLights [2].ScaleX = ( winSize.Height / 2 ) / turnLights [2].ContentSize.Width;
			mainLayer.AddChild (turnLights [2]);
			turnLights [2].ZOrder = 20;
			turnLights [2].Visible = false;

			turnLights.Add (new CCSprite ("turnLight2"));
			turnLights [3].Position = new CCPoint (-5 + turnLights [3].ContentSize.Height / 2, winSize.Height / 4);
			turnLights [3].BlendFunc = CCBlendFunc.Additive;
			turnLights [3].Rotation = 90;
			turnLights [3].Color = CCColor3B.Blue;
			turnLights [3].ScaleX = ( winSize.Height / 2 ) / turnLights [3].ContentSize.Width;
			mainLayer.AddChild (turnLights [3]);
			turnLights [3].ZOrder = 20;
			turnLights [3].Visible = false;

			turnLights.Add (new CCSprite ("turnLight2"));
			turnLights [4].Position = new CCPoint (winSize.Width / 2, -5 + turnLights [1].ContentSize.Height / 2);
			turnLights [4].BlendFunc = CCBlendFunc.Additive;
			turnLights [4].Color = CCColor3B.Orange;
			turnLights [4].ScaleX = winSize.Width / turnLights [4].ContentSize.Width;
			mainLayer.AddChild (turnLights [4]);
			turnLights [4].ZOrder = 20;
			turnLights [4].Visible = false;

			#endregion

			#region Names initialization
			playerNames = new List<CCLabel> (5);

			playerNames.Add (new CCLabel ("", "Arial", 12));

			playerNames.Add (new CCLabel (Board.Instance.AllPlayers [1].name, "Arial", 20));
			playerNames [1].Position = new CCPoint (winSize.Width / 2, winSize.Height - 15);
			playerNames [1].Rotation = 180;
			mainLayer.AddChild (playerNames [1]);

			playerNames.Add (new CCLabel (Board.Instance.AllPlayers [2].name, "Arial", 20));
			playerNames [2].Position = new CCPoint (15, winSize.Height * 3 / 4);
			playerNames [2].Rotation = -90;
			mainLayer.AddChild (playerNames [2]);

			playerNames.Add (new CCLabel (Board.Instance.AllPlayers [3].name, "Arial", 20));
			playerNames [3].Position = new CCPoint (15, winSize.Height / 4);
			playerNames [3].Rotation = -90;
			mainLayer.AddChild (playerNames [3]);

			playerNames.Add (new CCLabel (Board.Instance.AllPlayers [4].name, "Arial", 20));
			playerNames [4].Position = new CCPoint (winSize.Width / 2, 15);
			playerNames [4].Rotation = 180;
			mainLayer.AddChild (playerNames [4]);
			#endregion

			#region Debug bids labels
			playerBids = new List <CCLabel> (5);

			playerBids.Add (new CCLabel ("", "Arial", 12));

			playerBids.Add (new CCLabel ("", "Arial", 20));
			playerBids [1].Position = new CCPoint (playerNames [1].PositionX, playerNames [1].PositionY - 15);
			playerBids [1].Rotation = 180;
			mainLayer.AddChild (playerBids [1]);

			playerBids.Add (new CCLabel ("", "Arial", 20));
			playerBids [2].Position = new CCPoint (playerNames [2].PositionX + 15, playerNames [2].PositionY);
			playerBids [2].Rotation = -90;
			mainLayer.AddChild (playerBids [2]);

			playerBids.Add (new CCLabel ("", "Arial", 20));
			playerBids [3].Position = new CCPoint (playerNames [3].PositionX + 15, playerNames [3].PositionY);
			playerBids [3].Rotation = -90;
			mainLayer.AddChild (playerBids [3]);

			playerBids.Add (new CCLabel ("", "Arial", 20));
			playerBids [4].Position = new CCPoint (playerNames [4].PositionX, playerNames [4].PositionY + 15);
			playerBids [4].Rotation = 180;
			mainLayer.AddChild (playerBids [4]);
			#endregion

			#region Card sprites creation and positioning
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
				carte.Add (new CardData (new CCSprite (Board.Instance.Me.Hand [i].number.ToString () + "_" + Board.Instance.Me.Hand [i].seme.ToString ()), posBase, rotation, i));

				//Positioning the cards in an arc shape, using a parabola constructed with the for index
				carte [i].sprite.Position = carte [i].posBase;
				carte [i].sprite.Rotation = carte [i].rotation;
				carte [i].sprite.Scale = 0.3f;

				mainLayer.AddChild (carte [i].sprite, i);
			}
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

			int textWidth = new CCTexture2D ("btnLascio").PixelsWide;

			float scale = ( ( winSize.Height / 2 ) - orzSpace * 4 ) / ( 4 * textWidth );

			//FIXME : controllare le posizioni dei bottoni nel caso si cambiassero le textures
			for (int i = 4; i > -1; i--) {
				buttons [i] = new Button (mainLayer, touch, actButtons [i], pathButtons [i], pathButtonsPressed [i], new CCPoint (3 * vertSpace + 3 * 58 * scale, winSize.Height / 4 + ( textWidth * scale + orzSpace ) * ( ( i - 4 ) * -1 )), winSize, -90, scale);
			}
			for (int i = 9; i > 4; i--) {
				buttons [i] = new Button (mainLayer, touch, actButtons [i], pathButtons [i], pathButtonsPressed [i], new CCPoint (2 * vertSpace + 2 * 58 * scale, winSize.Height / 4 + ( textWidth * scale + orzSpace ) * ( ( i - 9 ) * -1 )), winSize, -90, scale);
			}

			buttons [10] = new Button (mainLayer, touch, actButtons [10], pathButtons [10], pathButtonsPressed [10], new CCPoint (vertSpace + 58 * scale, winSize.Height / 2 - orzSpace / 2 - ( textWidth * scale ) / 2), winSize, -90, scale);
			buttons [11] = new Button (mainLayer, touch, actButtons [11], pathButtons [11], pathButtonsPressed [11], new CCPoint (vertSpace + 58 * scale, winSize.Height / 2 + orzSpace / 2 + ( textWidth * scale ) / 2), winSize, -90, scale);

			slider = new Slider (mainLayer, touch, "sliderBar", "sliderBall", new CCPoint (5 * vertSpace + 4 * 58 * scale, winSize.Height / 4 - 115 * scale), winSize, 61, 120);


			#endregion


			wait = 0;

			Schedule (RunGameLogic);

			Board.Instance.start ();




		}


		/// <summary>
		/// Game logic, will be executed every frame
		/// </summary>
		/// <param name="frameTimeInSeconds">Frame time in seconds</param>
		void RunGameLogic (float frameTimeInSeconds)
		{
			if (wait > 0) {
				wait -= frameTimeInSeconds;
			} else {
				wait = 0;
				Board.Instance.update ();
			}
					
			
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

		#region Debug

		#endregion

		#region Let there be light

		private void turnLight (int playerIndex)
		{
			for (int i = 0; i < 5; i++) {
				turnLights [i].Visible = false;
			}
			turnLights [playerIndex].Visible = true;
		}

		#endregion

		#region Event responses

		#region Auction started

		public void auctionStarted ()
		{

			touch.eventTouchBegan += touchBeganAsta;
			touch.eventTouchMoved += touchMovedAsta;
			touch.eventTouchEnded += touchEndedAsta;

			bidded = false;
			initializedSeme = false;

			Board.Instance.Me.setAuctionControl (bidController, semiController);
			turnLight (( Board.Instance.ActiveAuctionPlayer.order - Board.Instance.Me.order + 5 ) % 5);

			if (Board.Instance.ActiveAuctionPlayer != Board.Instance.Me)
				for (int i = 0; i < 12; i++)
					buttons [i].Enabled = false;

		}

		#endregion

		#region Auction turn changed

		public void bidPlaced (IBid bid)
		{
			//TODO : disabilitare i pulsanti che non posso cliccare perchè puntano roba troppo alta
			if (!Board.Instance.isAuctionPhase || Board.Instance.ActiveAuctionPlayer != Board.Instance.Me) {
				for (int i = 0; i < 12; i++)
					buttons [i].Enabled = false;
			} else {
				for (int i = 0; i < 12; i++)
					buttons [i].Enabled = true;
			}

			playerBids [( bid.bidder.order - Board.Instance.Me.order + 5 ) % 5].Text = bidToString (bid);
			turnLight (( !Board.Instance.isAuctionPhase ? Board.Instance.currentAuctionWinningBid.bidder.order : Board.Instance.ActiveAuctionPlayer.order - Board.Instance.Me.order + 5 ) % 5);

		}

		private string bidToString (IBid bid)
		{
			if (bid is PassBid)
				return "Passo";
			else if (bid is CarichiBid)
				return "Carichi al " + ( (CarichiBid) bid ).point.ToString ();
			else
				return ( (NormalBid) bid ).number.ToString () + " al " + ( (NormalBid) bid ).point.ToString ();
		}

		#endregion

		#region Auction ended

		//TODO: i touch del game non vanno qui
		public void auctionEnded ()
		{
			touch.eventTouchBegan -= touchBeganAsta;
			touch.eventTouchMoved -= touchMovedAsta;
			touch.eventTouchEnded -= touchEndedAsta;
			for (int i = 0; i < 12; i++) {
				buttons [i].remove ();
			}
			slider.remove ();
		}

		#endregion

		#region Seme choosing

		public void chooseSemeButtons ()
		{

			auctionEnded ();
			chooseOri = new Button (mainLayer, touch, actOri, ( (NormalBid) Board.Instance.currentAuctionWinningBid ).number.ToString () + "_" + "ORI", ( (NormalBid) Board.Instance.currentAuctionWinningBid ).number.ToString () + "_" + "ORI", new CCPoint (winSize.Width / 2, winSize.Height / 2 + 150), winSize, -90, 0.3f);
			chooseCoppe = new Button (mainLayer, touch, actCoppe, ( (NormalBid) Board.Instance.currentAuctionWinningBid ).number.ToString () + "_" + "COPE", ( (NormalBid) Board.Instance.currentAuctionWinningBid ).number.ToString () + "_" + "COPE", new CCPoint (winSize.Width / 2, winSize.Height / 2 + 50), winSize, -90, 0.3f);
			chooseBastoni = new Button (mainLayer, touch, actBastoni, ( (NormalBid) Board.Instance.currentAuctionWinningBid ).number.ToString () + "_" + "BASTONI", ( (NormalBid) Board.Instance.currentAuctionWinningBid ).number.ToString () + "_" + "BASTONI", new CCPoint (winSize.Width / 2, winSize.Height / 2 - 50), winSize, -90, 0.3f);
			chooseSpade = new Button (mainLayer, touch, actSpade, ( (NormalBid) Board.Instance.currentAuctionWinningBid ).number.ToString () + "_" + "SPADE", ( (NormalBid) Board.Instance.currentAuctionWinningBid ).number.ToString () + "_" + "SPADE", new CCPoint (winSize.Width / 2, winSize.Height / 2 - 150), winSize, -90, 0.3f);
		}

		#endregion

		#region Playtime started

		public void startPlaytime ()
		{
			touch.eventTouchBegan += touchBeganGame;
			touch.eventTouchMoved += touchMovedGame;
			touch.eventTouchEnded += touchEndedGame;

			for (int i = 1; i < 5; i++) {
				if (Board.Instance.getPlayer (i).Role == EnRole.CHIAMANTE)
					playerBids [i].SetString (playerBids [i].Text + " di " + Board.Instance.Briscola.ToString (), true);
				else
					playerBids [i].SetString ("", true);
			}
		}

		#endregion

		#region I play a card

		public Card iPlayCard ()
		{
			if (played) {
				played = false;
				Card temp = Board.Instance.Me.InitialHand [droppedCards [Board.Instance.numberOfCardOnBoard].index];
				if (Board.Instance.numberOfCardOnBoard != 4)
					turnLight (1);
				return Board.Instance.getCard (temp.seme, temp.number);
			}
			return null;
		}

		#endregion

		#region Play a card

		public void playCard (Move m)
		{
			int localIndex = m.player.order;
			CCSprite cardSprite = new CCSprite (m.card.number.ToString () + "_" + m.card.seme.ToString ());
			cardSprite.Scale = 0.25f;
			CardData cd;
			switch (localIndex) {
				case 1:
					cardSprite.Position = offScreen [1];
					cardSprite.Rotation = 180;
					mainLayer.AddChild (cardSprite);
					cd = new CardData (cardSprite, new CCPoint (dropField.MaxX * 3 / 4, winSize.Height - dropField.MinY), 180, -1);
					droppedCards.Add (cd);
					moveSprite (cd.posBase, cardSprite);
					wait = 0.5f;
				break;
				case 2:
					cardSprite.Position = offScreen [2];
					cardSprite.Rotation = 270;
					mainLayer.AddChild (cardSprite);
					cd = new CardData (cardSprite, new CCPoint (dropField.MaxX * 2 / 5, dropField.MidY + 100), 270, -1);
					droppedCards.Add (cd);
					moveSprite (cd.posBase, cardSprite);
					wait = 0.5f;
				break;

				case 3:
					cardSprite.Position = offScreen [3];
					cardSprite.Rotation = 270;
					mainLayer.AddChild (cardSprite);
					cd = new CardData (cardSprite, new CCPoint (dropField.MaxX * 2 / 5, dropField.MidY - 100), 270, -1);
					droppedCards.Add (cd);
					moveSprite (cd.posBase, cardSprite);
					wait = 0.5f;
				break;

				case 4:
					cardSprite.Position = offScreen [4];
					mainLayer.AddChild (cardSprite);
					cd = new CardData (cardSprite, new CCPoint (dropField.MaxX * 3 / 4, winSize.Height - dropField.MaxY), 0, -1);
					droppedCards.Add (cd);
					moveSprite (cd.posBase, cardSprite);
					wait = 0.5f;
				break;
			}

			if (Board.Instance.numberOfCardOnBoard == 5) {
				wait += 2;
			} else
				turnLight (( localIndex + 1 ) % 5);
				
				
				




		}

		#endregion

		#region Fine giro

		public void clearBoard (Player player, List<Card> board)
		{
			for (int i = 4; i > -1; i--) {
				CCMoveTo move = new CCMoveTo (0.5f, offScreen [player.order]);
				CCRemoveSelf delete = new CCRemoveSelf ();
				droppedCards [i].sprite.RunActions (move, delete);
				droppedCards.RemoveAt (i);
			}
			turnLight (player.order);


		}

		#endregion

		#endregion

	}




}
