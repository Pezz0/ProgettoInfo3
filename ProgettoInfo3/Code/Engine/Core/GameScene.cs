using System;
using CocosSharp;

namespace Core
{
	public class GameScene : CCScene
	{
		//Core variables
		private CCLayer mainLayer;

		//Sprites and position variables
		private CCSprite [] carte;
		private CCPoint3 [] posBase;
		//Using a 3D point to store the rotation, too

		//Touch helper variables
		private CCEventListenerTouchAllAtOnce touchListener;
		private int selected;
		private CCRect dropField;

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
			carte = new CCSprite[8];
			posBase = new CCPoint3[8];

			//Setting the area when cards can be dropped
			dropField = new CCRect (127, 454, 300, 400);

			//Getting the window size
			winSize = mainWindow.WindowSizeInPixels;

			//Test card sprites creation
			for (int i = 0; i < 8; i++) {

				//Positioning the cards in an arc shape, using a parabola constructed with the for index
				posBase [i].X = winSize.Width - 100 + 3 * (i * i - 7 * i + 12);
				posBase [i].Y = i == 0 ? winSize.Height / 4 : posBase [i - 1].Y + 50;
				posBase [i].Z = -90 - 4 * (i > 3 ? 4 - i - 1 : 4 - i);

				carte [i] = new CCSprite ("AsseBastoni");
				carte [i].PositionX = posBase [i].X;
				carte [i].PositionY = posBase [i].Y;
				carte [i].Rotation = posBase [i].Z;
				carte [i].Scale = 0.3f;


				mainLayer.AddChild (carte [i]);
			}





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
		/// Function executed on the touch movement
		/// </summary>
		/// <param name="touches">List of touches</param>
		/// <param name="touchEvent">Touch event</param>
		void touchMoved (System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
		{
			//Moving the sprite following exactly the touch
			CCPoint pos = touches [0].LocationOnScreen;


			if (selected >= 0) {
				//Inverting Y cuz the image is referred to the bottom-left corner and the touch is referred to the top-left corner
				carte [selected].Position = new CCPoint (pos.X, winSize.Height - pos.Y);
			}


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
			for (i = 7; i >= 0; i--) {
				if (carte [i].BoundingBoxTransformedToParent.ContainsPoint (posToParent)) {

					selected = i;
					break;
				}

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

			if (dropField.ContainsPoint (pos)) {
				posBase [selected] = new CCPoint3 (dropField.Center.X, winSize.Height -dropField.Center.Y,posBase [selected].Z);
				CCMoveTo c = new CCMoveTo (0.3f, new CCPoint (posBase [selected].X, posBase [selected].Y));
				carte [selected].RunAction (c);
			}
			selected = -1;


		}
	}


}
