using System;
using CocosSharp;
using System.Collections.Generic;

namespace Core
{
	public class Button
	{
		private bool pressed;
		private CCSprite spriteNorm;
		private CCSprite spritePressed;
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
					_Enabled = true;
				}
				else {
					//TODO : aggiungere la texture di disabled
					touch.eventTouchBegan -= touchBegan;
					touch.eventTouchEnded -= touchEnded;
					_Enabled = false;
				}
			}
		}

		public Button (CCLayer mainLayer,touchList tl, touchList.eventHandlerTouch method, string textureDefault,string texturePressed, CCPoint position,CCSize winSize, float rot=-90,float scale=0.55f)
		{

			//Defining the sprite
			spriteNorm = new CCSprite (textureDefault);
			spriteNorm.Position = position;
			spriteNorm.Rotation = rot;
			spriteNorm.Scale = scale;
			mainLayer.AddChild (spriteNorm);

			spritePressed = new CCSprite (texturePressed);
			spritePressed.Position = position;
			spritePressed.Rotation = rot;
			spritePressed.Scale = scale;
			mainLayer.AddChild (spritePressed);
			spritePressed.Visible = false;

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
			if (spriteNorm.BoundingBoxTransformedToParent.ContainsPoint (new CCPoint (touches [0].LocationOnScreen.X, winSize.Height - touches [0].LocationOnScreen.Y))){
				pressed = true;
				spritePressed.Visible = true;
				spriteNorm.Visible = false;
			}
				
		}

		private void touchEnded(List<CCTouch> touches, CCEvent touchEvent){
			spriteNorm.Visible = true;
			spritePressed.Visible = false;
			if (spriteNorm.BoundingBoxTransformedToParent.ContainsPoint (new CCPoint (touches [0].LocationOnScreen.X, winSize.Height - touches [0].LocationOnScreen.Y)) && pressed == true) {
				onButtonPressed(touches, touchEvent);
				pressed = false;

			}

		}


		public void remove(){
			touch.eventTouchBegan -= touchBegan;
			touch.eventTouchEnded -= touchEnded;
			mainLayer.RemoveChild (spriteNorm);
			mainLayer.RemoveChild (spritePressed);

		}
	}
}

