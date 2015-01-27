using System;
using CocosSharp;
using System.Collections.Generic;

namespace Core
{
	/// <summary>
	/// Basic slider.
	/// </summary>
	public class Slider
	{
		/// <summary>
		/// The sprite for the slider bar.
		/// </summary>
		private readonly CCSprite spriteBar;

		/// <summary>
		/// The sprite for the slider point.
		/// </summary>
		private readonly CCSprite spritePoint;

		/// <summary>
		/// The label indicating the current value of the slider.
		/// </summary>
		private CCLabel lblValue;

		/// <summary>
		/// The size of the window.
		/// </summary>
		private readonly CCSize winSize;

		/// <summary>
		/// The touhc listener.
		/// </summary>
		private readonly TouchList touch;

		/// <summary>
		/// The main layer (will be father of the slider).
		/// </summary>
		private readonly CCLayer mainLayer;

		/// <summary>
		/// The scale fo the slider.
		/// </summary>
		private readonly float scale;

		/// <summary>
		/// The minimum value of the slider.
		/// </summary>
		private int _min;

		/// <summary>
		/// Sets the minimum value for the slider.
		/// </summary>
		/// <value>The minimum value.</value>
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

		/// <summary>
		/// Boolean value that indicates if the slider is visible or not.
		/// </summary>
		private bool _visible;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Core.Slider"/> is visible.
		/// </summary>
		/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
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

		/// <summary>
		/// The maximum value of the slider.
		/// </summary>
		private int _max;

		/// <summary>
		/// The current value of the slider.
		/// </summary>
		private int _currentValue;

		/// <summary>
		/// Resets the range of the slider (sets the minimum to 61).
		/// </summary>
		public void resetRange ()
		{
			_min = 61;
			_currentValue = 61;
			lblValue.SetString ("61", true);
		}

		/// <summary>
		/// Gets the current value of the slider.
		/// </summary>
		/// <value>The current value of the slider.</value>
		public int currentValue{ get { return _currentValue; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Core.Slider"/> class.
		/// </summary>
		/// <param name="mainLayer">Main layer.</param>
		/// <param name="tl">Touch listener.</param>
		/// <param name="textureBar">Texture of the slider bar.</param>
		/// <param name="texturePoint">Texture of the slider point.</param>
		/// <param name="position">Position of the slider.</param>
		/// <param name="winSize">Window size.</param>
		/// <param name="min">Minimum value of the slider.</param>
		/// <param name="max">Maximum value of the slider.</param>
		/// <param name="rot">Rotation of the slider sprite (default -90).</param>
		/// <param name="scale">Scale of the slider sprite (default 0.8f).</param>
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

		/// <summary>
		/// Method added to the touch listener. Will be executed every time a touch is detected.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void touchBegan (List<CCTouch> touches, CCEvent touchEvent)
		{
			if (spriteBar.BoundingBoxTransformedToParent.ContainsPoint (new CCPoint (touches [0].LocationOnScreen.X, winSize.Height - touches [0].LocationOnScreen.Y))) {
				spritePoint.PositionX = ( winSize.Height - touches [0].LocationOnScreen.Y - spriteBar.PositionY ) / scale;
				_currentValue = _min + (int) Math.Round (( spritePoint.PositionX / spriteBar.ContentSize.Width ) * ( _max - _min ));
				lblValue.Text = _currentValue.ToString ();
			}
		}

		/// <summary>
		/// Method added to the touch listener. Will be executed every time a touch is dragged.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void touchMoved (List<CCTouch> touches, CCEvent touchEvent)
		{
			if (spriteBar.BoundingBoxTransformedToParent.ContainsPoint (new CCPoint (touches [0].LocationOnScreen.X, winSize.Height - touches [0].LocationOnScreen.Y))) {
				spritePoint.PositionX = ( winSize.Height - touches [0].LocationOnScreen.Y - spriteBar.PositionY ) / scale;
				_currentValue = _min + (int) Math.Round (( spritePoint.PositionX / spriteBar.ContentSize.Width ) * ( _max - _min ));
				lblValue.Text = _currentValue.ToString ();
			}
		}

		/// <summary>
		/// Remove the instance of the <see cref="Core.Slider"/>.
		/// </summary>
		public void remove ()
		{
			touch.eventTouchBegan -= touchBegan;
			touch.eventTouchEnded -= touchMoved;
			mainLayer.RemoveChild (spriteBar);
			mainLayer.RemoveChild (spritePoint);
		}
			
	}
}

