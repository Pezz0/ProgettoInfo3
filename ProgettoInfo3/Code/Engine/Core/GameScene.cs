using System;
using CocosSharp;

namespace ProgettoInfo3
{
	public class GameScene : CCScene
	{
		CCLayer mainLayer;
		CCSprite [] carte;
		CCEventListenerTouchAllAtOnce touchListener;

		// TODO : Sistemare questa winsize, non riesco a trovare un'altro modo per prenderla
		CCSize winSize;


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

			//Getting the window size
			winSize = mainWindow.WindowSizeInPixels;

			//Test card sprites creation
			for (int i = 0; i < 8; i++) {
				carte [i] = new CCSprite ("AsseBastoni");
				carte [i].Rotation = -90 - 4 * (i > 3 ? 4 - i - 1 : 4 - i);
				carte [i].Scale = 0.3f;
				//Positioning the cards in an arc shape, using a parabola constructed with the for index
				if (i == 0) {
					carte [i].PositionX = (int) (winSize.Width) - 100 + 3 * (i * i - 7 * i + 12);
					carte [i].PositionY = (int) (winSize.Height / 4);
				} else {
					carte [i].PositionX = (int) (winSize.Width) - 100 + 3 * (i * i - 7 * i + 12);
					carte [i].PositionY = carte [i - 1].PositionY + 50;
				}
				mainLayer.AddChild (carte [i]);
			}





			//Instancing the touch listener
			touchListener = new CCEventListenerTouchAllAtOnce ();
			//Instancing the event for the movement
			touchListener.OnTouchesMoved = touchMoved; 
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
		/// Function executed for the touch movement
		/// </summary>
		/// <param name="touches">List of touches</param>
		/// <param name="touchEvent">Touch event</param>
		void touchMoved (System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
		{
			//Moving the sprite following exactly the touch
			CCPoint pos = touches [0].LocationOnScreen;

			//Checking on wich card the touch is positioned
			//I'm doing this in reverse because the 7th card is the one in the foreground and the 0th card is the one in the background
			int i;
			bool touched = false;
			for (i = 7; i >= 0; i--) {
				if (carte [i].BoundingBoxTransformedToParent.ContainsPoint (pos)) {
					touched = true;
					break;
				}
					
			}
			if (touched) {
				//Inverting Y cuz the image is referred to the bottom-left corner and the touch is referred to the top-left corner
				carte [i].Position = new CCPoint (pos.X, winSize.Height - pos.Y);
			}


		}
	}
}
