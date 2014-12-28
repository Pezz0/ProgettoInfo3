using System;
using CocosSharp;
using System.Collections.Generic;

namespace Core
{
	public class TouchList
	{

		private CCEventListenerTouchAllAtOnce touchListener;
		public delegate void eventHandlerTouch (List<CCTouch> touches,CCEvent touchEvent);

		public event eventHandlerTouch eventTouchBegan;
		public event eventHandlerTouch eventTouchMoved;
		public event eventHandlerTouch eventTouchEnded;

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
			if(eventTouchBegan!=null)  {
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
			if(eventTouchMoved!=null)  {
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
			if(eventTouchEnded!=null)  {
				eventTouchEnded (touches, touchEvent);
			}
				
		}
	}
}

