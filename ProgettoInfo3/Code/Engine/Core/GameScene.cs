using System;
using CocosSharp;

namespace ProgettoInfo3
{
	public class GameScene : CCScene
	{
		CCLayer mainLayer;
		CCSprite Jimbe;
		CCEventListenerTouchAllAtOnce touchListener;

		// TODO : Sistemare questa winsize, non riesco a trovare un'altro modo per prenderla
		CCSize winSize;


		/// <summary>
		/// Gamescene constructor, initializes sprites to their default position
		/// </summary>
		/// <param name="mainWindow">Main window.</param>
		public GameScene(CCWindow mainWindow) : base(mainWindow)
		{
			mainLayer = new CCLayer ();
			AddChild (mainLayer);

			//Test sprite creation
			Jimbe = new CCSprite ("Jimbe");
			Jimbe.PositionX = 100;
			Jimbe.PositionY = 100;
			mainLayer.AddChild (Jimbe);


			winSize = mainWindow.WindowSizeInPixels;

			//Instancing the touch listener
			touchListener = new CCEventListenerTouchAllAtOnce ();
			//Instancing the event for the movement
			touchListener.OnTouchesMoved = touchMoved; 
			AddEventListener (touchListener, this);
		}


		/// <summary>
		/// Game logic, will be executed every frame
		/// </summary>
		/// <param name="frameTimeInSeconds">Frame time in seconds.</param>
		void RunGameLogic(float frameTimeInSeconds)
		{

		}

		/// <summary>
		/// Function executed for the touch movement
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		void touchMoved (System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
		{
			//Moving the sprite following exactly the touch
			CCPoint pos = touches [0].LocationOnScreen;

			//Inverting Y cuz the image is referred to the bottom-left corner and the touch is referred to the top-left corner
			Jimbe.Position = new CCPoint (pos.X, winSize.Height-pos.Y);

		}
	}
}
