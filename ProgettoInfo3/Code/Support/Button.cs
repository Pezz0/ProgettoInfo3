using System;
using CocosSharp;
using System.Collections.Generic;

namespace Core
{
	public class Button
	{
		private bool pressed;
		private CCSprite sprite;
		private CCSize winSize;
		private touchList.eventHandlerTouch onButtonPressed;
		private touchList touch;
		private CCLayer mainLayer;

		private bool _Enabled;
		public bool Enabled {
			get{return _Enabled;}
			set{
				if(value==true){
					//TODO : aggiungere la texture di disabled
					touch.eventTouchBegan += touchBegan;
					touch.eventTouchEnded += touchEnded;
				}
				else {
					//TODO : aggiungere la texture di disabled
					touch.eventTouchBegan -= touchBegan;
					touch.eventTouchEnded -= touchEnded;
				}
			}
		}

		public Button (CCLayer mainLayer,touchList tl, touchList.eventHandlerTouch method, CCSprite sprite, CCPoint position,CCSize winSize, float rot=-90,float scale=0.55f)
		{

			//Defining the sprite
			this.sprite = sprite;
			sprite.Position = position;
			sprite.Rotation = rot;
			sprite.Scale = scale;
			mainLayer.AddChild (sprite);

			this.winSize = winSize;
			this.mainLayer = mainLayer;

			//Defining the event variables
			onButtonPressed = method;
			pressed = false;
			touch = tl;
			touch.eventTouchBegan += touchBegan;
			touch.eventTouchEnded += touchEnded;
		}

		private void touchBegan (List<CCTouch> touches, CCEvent touchEvent)
		{
			if (sprite.BoundingBoxTransformedToParent.ContainsPoint (new CCPoint (touches [0].LocationOnScreen.X, winSize.Height - touches [0].LocationOnScreen.Y)))
				pressed = true;
		}

		private void touchEnded(List<CCTouch> touches, CCEvent touchEvent){
			if (sprite.BoundingBoxTransformedToParent.ContainsPoint (new CCPoint (touches [0].LocationOnScreen.X, winSize.Height - touches [0].LocationOnScreen.Y)) && pressed == true) {
				onButtonPressed(touches, touchEvent);
				pressed = false;
			}

		}


		public void remove(){
			touch.eventTouchBegan -= touchBegan;
			touch.eventTouchEnded -= touchEnded;
			mainLayer.RemoveChild (sprite);

		}
	}
}

