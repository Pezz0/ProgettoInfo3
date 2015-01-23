using System;
using CocosSharp;
using System.Collections.Generic;

namespace Core
{
	public class Slider
	{
		private CCSprite spriteBar;
		private CCSprite spritePoint;
		private CCLabel lblValue;
		private CCSize winSize;
		private TouchList touch;
		private CCLayer mainLayer;

		private float scale;
		private int _min;

		public int min {
			set {
				if (_currentValue == _min) {
					_currentValue = value;
					_min = value;
					lblValue.Text = value.ToString ();
					//(value.ToString (), true);

				}
			}
		}

		private bool _visible;

		public bool visible {
			get{ return _visible; }
			set {
				_visible = value;
				if (value) {
					spriteBar.Visible = true;
					spritePoint.Visible = true;
					touch.eventTouchBegan += touchBegan;
					touch.eventTouchMoved += touchMoved;

				} else {
					spriteBar.Visible = false;
					spritePoint.Visible = false;
					touch.eventTouchBegan -= touchBegan;
					touch.eventTouchMoved -= touchMoved;
				}
			}
		}

		private int _max;
		private int _currentValue;

		public void resetRange ()
		{
			_min = 61;
			_currentValue = 61;
			lblValue.SetString ("61", true);
		}

		public int currentValue{ get { return _currentValue; } }


		public Slider (CCLayer mainLayer, TouchList tl, string textureBar, string texturePoint, CCPoint position, CCSize winSize, int min, int max, float rot = -90, float scale = 0.8f)
		{
			//Defining the sprite
			spriteBar = new CCSprite (textureBar);
			spriteBar.AnchorPoint = new CCPoint (0, 0.5f);
			spriteBar.Position = position;
			spriteBar.Rotation = rot;
			spriteBar.Scale = scale;
			spriteBar.BlendFunc = CCBlendFunc.AlphaBlend;
			mainLayer.AddChild (spriteBar);

			spritePoint = new CCSprite (texturePoint);
			spriteBar.AddChild (spritePoint);
			spritePoint.Position = new CCPoint (0, spriteBar.ContentSize.Height / 2);
			spritePoint.Scale = scale * 0.06f;
			spritePoint.BlendFunc = CCBlendFunc.AlphaBlend;

			_min = min;
			_max = max;

			_currentValue = _min;


			lblValue = new CCLabel (_currentValue.ToString (), "Arial", 15);
			lblValue.Position = new CCPoint (spriteBar.ContentSize.Width / 2, -0.05f * spriteBar.ContentSize.Height);
			spriteBar.AddChild (lblValue);

			this.winSize = winSize;
			this.mainLayer = mainLayer;



			this.scale = scale;

			//Defining the event variables
			touch = tl;
			touch.eventTouchBegan += touchBegan;
			touch.eventTouchMoved += touchMoved;
		}

		private void touchBegan (List<CCTouch> touches, CCEvent touchEvent)
		{
			if (spriteBar.BoundingBoxTransformedToParent.ContainsPoint (new CCPoint (touches [0].LocationOnScreen.X, winSize.Height - touches [0].LocationOnScreen.Y))) {
				spritePoint.PositionX = ( winSize.Height - touches [0].LocationOnScreen.Y - spriteBar.PositionY ) / scale;
				_currentValue = _min + (int) Math.Round (( spritePoint.PositionX / spriteBar.ContentSize.Width ) * ( _max - _min ));
				lblValue.Text = _currentValue.ToString ();
			}
		}

		private void touchMoved (List<CCTouch> touches, CCEvent touchEvent)
		{
			if (spriteBar.BoundingBoxTransformedToParent.ContainsPoint (new CCPoint (touches [0].LocationOnScreen.X, winSize.Height - touches [0].LocationOnScreen.Y))) {
				spritePoint.PositionX = ( winSize.Height - touches [0].LocationOnScreen.Y - spriteBar.PositionY ) / scale;
				_currentValue = _min + (int) Math.Round (( spritePoint.PositionX / spriteBar.ContentSize.Width ) * ( _max - _min ));
				lblValue.Text = _currentValue.ToString ();
			}
		}

		public void remove ()
		{
			touch.eventTouchBegan -= touchBegan;
			touch.eventTouchEnded -= touchMoved;
			mainLayer.RemoveChild (spriteBar);
			mainLayer.RemoveChild (spritePoint);
		}
			
	}
}

