using System;
using CocosSharp;
using System.Collections.Generic;

namespace Core
{
	public class GameScene : CCScene
	{
		//Core variables
		private CCLayer mainLayer;

		//Sprites and position variables
		private List<cardData> carte;
		private List<cardData> droppedCards;


		//Touch helper variables
		private CCEventListenerTouchAllAtOnce touchListener;
		private int selected;
		private CCRect dropField;
		private CCRect cardField;
		private int inHand;

		// TODO : Sistemare questa winsize, non riesco a trovare un'altro modo per prenderla
		private CCSize winSize;


		/// <summary>
		/// Gamescene constructor, initializes sprites to their default position
		/// </summary>
		/// <param name="mainWindow">Main window.</param>
		public GameScene (CCWindow mainWindow) : base (mainWindow)
		{

			//Instancing the layer and setting him as a child of the mainWindow
			mainLayer = new CCLayer ();
			AddChild (mainLayer);

			//Instancing the array of cards that will become childs of the mainLayer
			carte = new List<cardData>(8);
			droppedCards = new List<cardData> ();

			//Getting the window size
			winSize = mainWindow.WindowSizeInPixels;

			//Setting the area when cards can be dropped
			dropField = new CCRect (0, (int)(winSize.Height/4), (int)(winSize.Width/2), (int)(winSize.Height/2));

			//Setting the area where the cards can be re-arranged
			cardField = new CCRect ((int)(winSize.Width*4/5),(int)(winSize.Height/5), (int)(winSize.Width/5), (int)(winSize.Height*3/5));

			inHand = 8;


			#region Sprites creation and positioning
			//Sprites creation
			CCPoint posBase;
			float rotation;
 			for (int i = 0; i < 8; i++) {


				if (i == 0){
					posBase=new CCPoint (winSize.Width - 100 + 3 * (i * i - 7 * i + 12), winSize.Height / 4);

				}else{
					posBase=new CCPoint (winSize.Width - 100 + 3 * (i * i - 7 * i + 12),carte [i - 1].posBase.Y + 50);
				}
				rotation=-90 - 4 * (i > 3 ? 4 - i - 1 : 4 - i);
				carte.Add(new cardData(new CCSprite ("AsseBastoni"), posBase, rotation));
					
				//Positioning the cards in an arc shape, using a parabola constructed with the for index
				carte [i].sprite.Position = carte [i].posBase;
				carte [i].sprite.Rotation = carte [i].rotation;
				carte [i].sprite.Scale = 0.3f;

				mainLayer.AddChild (carte [i].sprite,i);
			}
			#endregion





			//Instancing the touch listener
			touchListener = new CCEventListenerTouchAllAtOnce ();
			//Instancing the event for the movement
			touchListener.OnTouchesBegan = touchBegan;
			touchListener.OnTouchesMoved = touchMoved; 
			touchListener.OnTouchesEnded = touchEnded;

			AddEventListener (touchListener, this);
		}


		/// <summary>
		/// Game logic, will be executed every frame
		/// </summary>
		/// <param name="frameTimeInSeconds">Frame time in seconds</param>
		void RunGameLogic (float frameTimeInSeconds)
		{

		}




		/// <summary>
		/// Function executed on the starting touch
		/// </summary>
		/// <param name="touches">List of touches</param>
		/// <param name="touchEvent">Touch event</param>
		void touchBegan (System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
		{


			CCPoint pos = touches [0].LocationOnScreen;
			CCPoint posToParent = new CCPoint (pos.X, winSize.Height - pos.Y);

			//Checking on wich card the touch is positioned
			//I'm doing this in reverse because the 7th card is the one in the foreground and the 0th card is the one in the background
			int i;
			selected = -1;
			for (i = inHand-1; i >= 0; i--) {
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
		void touchMoved (System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
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
				if (cardField.ContainsPoint (pos)&&inHand>1) {
					if (selected == 0) {
						if ((winSize.Height - pos.Y) > carte [1].posBase.Y + 8) {
							//Swap base position
							CCPoint tempP = carte [1].posBase;
							carte [1].posBase = carte [0].posBase;
							carte [0].posBase = tempP;

							//Swap base rotation
							float tempR=carte[1].rotation;
							carte[1].rotation=carte[0].rotation;
							carte[0].rotation=tempR;

							//Move and rotate sprite
							if(!carte[1].posBase.Equals(carte[1].sprite.Position)){
								moveSprite(carte[0].sprite,carte[0].rotation);
								moveSprite (carte [1].posBase, carte [1].sprite,0.3f,carte[1].rotation);
							}
								

							//Swap zOrder
							carte[0].sprite.ZOrder=1;
							carte[1].sprite.ZOrder=0;

							//Swap cardData entry
							cardData tempC =carte[1];
							carte[1]=carte[0];
							carte[0]=tempC;

							//Now the selected card is in another position
							selected=1;
						}
					} else if (selected == inHand-1) {
						if ((winSize.Height - pos.Y) < carte [selected - 1].posBase.Y - 8) {
							//Swap base position
							CCPoint tempP = carte [inHand-2].posBase;
							carte [inHand-2].posBase = carte [inHand-1].posBase;
							carte [inHand-1].posBase = tempP;

							//Swap base rotation
							float tempR=carte[inHand-2].rotation;
							carte[inHand-2].rotation=carte[inHand-1].rotation;
							carte[inHand-1].rotation=tempR;

							//Move and rotate sprite
							if(!carte[inHand-2].posBase.Equals(carte[inHand-2].sprite.Position)){
								moveSprite(carte[inHand-1].sprite,carte[inHand-1].rotation);
								moveSprite (carte [inHand-2].posBase, carte [inHand-2].sprite,0.3f,carte[inHand-2].rotation);
							}
								

							//Swap zOrder
							carte[inHand-1].sprite.ZOrder=inHand-2;
							carte[inHand-2].sprite.ZOrder=inHand-1;

							//Swap cardData entry
							cardData tempC =carte[inHand-2];
							carte[inHand-2]=carte[inHand-1];
							carte[inHand-1]=tempC;

							//Now the selected card is in another position
							selected=inHand-2;
						}
					}else {
						if ((winSize.Height - pos.Y) < carte [selected - 1].posBase.Y - 8) {
							//Swap base position
							CCPoint tempP = carte [selected - 1].posBase;
							carte [selected - 1].posBase = carte [selected].posBase;
							carte [selected].posBase = tempP;

							//Swap base rotation
							float tempR=carte[selected - 1].rotation;
							carte[selected - 1].rotation=carte[selected].rotation;
							carte[selected].rotation=tempR;

							//Move and rotate sprite
							if(!carte[selected-1].posBase.Equals(carte[selected-1].sprite.Position)){
								moveSprite(carte[selected].sprite,carte[selected].rotation);
								moveSprite (carte [selected - 1].posBase, carte [selected - 1].sprite,0.3f,carte[selected-1].rotation);
							}

							//Swap zOrder
							carte[selected].sprite.ZOrder=selected-1;
							carte[selected-1].sprite.ZOrder=selected;

							//Swap cardData entry
							cardData tempC =carte[selected-1];
							carte[selected-1]=carte[selected];
							carte[selected]=tempC;

							//Now the selected card is in another position
							selected=selected-1;
						}

						if ((winSize.Height - pos.Y) > carte [selected + 1].posBase.Y + 8) {
							//Swap base position
							CCPoint tempP = carte [selected + 1].posBase;
							carte [selected + 1].posBase = carte [selected].posBase;
							carte [selected].posBase = tempP;

							//Swap base rotation
							float tempR=carte[selected + 1].rotation;
							carte[selected + 1].rotation=carte[selected].rotation;
							carte[selected].rotation=tempR;

							//Move and rotate sprite
							if(!carte[selected+1].posBase.Equals(carte[selected+1].sprite.Position)){
								moveSprite(carte[selected].sprite,carte[selected].rotation);
								moveSprite (carte [selected + 1].posBase, carte [selected + 1].sprite,0.3f,carte[selected+1].rotation);
							}
								

							//Swap zOrder
							carte[selected+1].sprite.ZOrder=selected;
							carte[selected].sprite.ZOrder=selected+1;

							//Swap cardData entry
							cardData tempC =carte[selected+1];
							carte[selected+1]=carte[selected];
							carte[selected]=tempC;

							//Now the selected card is in another position
							selected=selected+1;
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
		void touchEnded (System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
		{
			CCPoint pos = touches [0].LocationOnScreen;
			if(selected>=0){
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
			CCSpawn actions = new CCSpawn(move,rotate);
			sprite.RunAction (actions);
		}

		/// <summary>
		/// Auxiliary method to only move a sprite
		/// </summary>
		/// <param name="destination">Point of destination</param>
		/// <param name="sprite">The sprite to be moved</param>
		/// <param name="time">Animation time</param>
		private void moveSprite(CCPoint destination, CCSprite sprite,float time =0.3f){
			CCMoveTo move = new CCMoveTo (time, new CCPoint (destination.X, destination.Y));
			sprite.RunAction (move);
		}

		/// <summary>
		/// Auxiliary method to only rotate a sprite
		/// </summary>
		/// <param name="sprite">The sprite to be rotated.</param>
		/// <param name="Rotation">Rotation of the sprite</param>
		/// <param name="time">Animation time</param>
		private void moveSprite(CCSprite sprite, float rotation, float time =0.3f){
			CCRotateTo rotate = new CCRotateTo (time, rotation);
			sprite.RunAction (rotate);
		}

		#endregion
	}

	class cardData
	{

		public  CCSprite sprite;

		private CCPoint _posBase;

		public CCPoint posBase{ get { return _posBase; } set{ _posBase=value; } }

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
