using System;
using CocosSharp;
using System.Collections.Generic;

namespace Core
{
	/// <summary>
	/// Touch listener. Wrapper for <see cref="CCEventListenerTouchAllAtOnce"/>
	/// </summary>
	public class TouchList
	{
		/// <summary>
		/// The touch listener.
		/// </summary>
		private readonly CCEventListenerTouchAllAtOnce touchListener;

		/// <summary>
		/// Delegate for every method that will be called by the event.
		/// </summary>
		public delegate void eventHandlerTouch (List<CCTouch> touches, CCEvent touchEvent);

		/// <summary>
		/// Occurs when a touch begins.
		/// </summary>
		public event eventHandlerTouch eventTouchBegan;

		/// <summary>
		/// Occurs when a touch is dragged.
		/// </summary>
		public event eventHandlerTouch eventTouchMoved;

		/// <summary>
		/// Occurs when a touch is ended.
		/// </summary>
		public event eventHandlerTouch eventTouchEnded;

		/// <summary>
		/// Initializes a new instance of the <see cref="Core.TouchList"/> class.
		/// </summary>
		/// <param name="gScene">Game scene.</param>
		public TouchList (CCScene gScene)
		{
			touchListener = new CCEventListenerTouchAllAtOnce ();
			touchListener.OnTouchesBegan = touchBegan;
			touchListener.OnTouchesMoved = touchMoved; 
			touchListener.OnTouchesEnded = touchEnded;
			gScene.AddEventListener (touchListener, gScene);
		}


		/// <summary>
		/// Function executed on the starting touch
		/// </summary>
		/// <param name="touches">List of touches</param>
		/// <param name="touchEvent">Touch event</param>
		private void touchBegan (List<CCTouch> touches, CCEvent touchEvent)
		{
			if (eventTouchBegan != null) {
				eventTouchBegan (touches, touchEvent);
			}
		}

		/// <summary>
		/// Function executed on the touch movement
		/// </summary>
		/// <param name="touches">List of touches</param>
		/// <param name="touchEvent">Touch event</param>
		private void touchMoved (List<CCTouch> touches, CCEvent touchEvent)
		{
			if (eventTouchMoved != null) {
				eventTouchMoved (touches, touchEvent);
			}	
		}



		/// <summary>
		/// Function executed when the touch is released
		/// </summary>
		/// <param name="touches">List of touches</param>
		/// <param name="touchEvent">Touch event</param>
		void touchEnded (List<CCTouch> touches, CCEvent touchEvent)
		{	
			if (eventTouchEnded != null) {
				eventTouchEnded (touches, touchEvent);
			}
				
		}
	}
}

