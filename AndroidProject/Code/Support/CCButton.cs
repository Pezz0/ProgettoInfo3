using System;
using CocosSharp;
using System.Collections.Generic;

namespace GUILayout
{
	/// <summary>
	/// Basic button.
	/// </summary>
	internal class CCButton
	{
		/// <summary>
		/// Boolean value that indicates if the button has been pressed.
		/// </summary>
		private bool _pressed;

		/// <summary>
		/// The sprite that the button shows when it's in the normal state.
		/// </summary>
		private readonly CCSprite _spriteNorm;

		/// <summary>
		/// The sprite that the button shows when it's in the pressed state.
		/// </summary>
		private readonly CCSprite _spritePressed;

		/// <summary>
		/// The size of the window.
		/// </summary>
		private readonly CCSize _winSize;

		/// <summary>
		/// The method which will be called when the button is pressed. Must be coherent with the delegate <see cref="Core.touchList.eventHandlerTouch"/>.
		/// </summary>
		private readonly TouchList.eventHandlerTouch _onButtonPressed;

		/// <summary>
		/// Touch listener.
		/// </summary>
		private readonly TouchList _touch;

		/// <summary>
		/// Father node for the button (usually the mainlayer).
		/// </summary>
		private readonly CCNode _father;

		/// <summary>
		/// Boolean value that indicates wheter or not the button can be pressed.
		/// </summary>
		private bool _Enabled;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Core.Button"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		internal bool Enabled {
			get{ return _Enabled; }
			set {
				if (value == true) {
					_spriteNorm.Color = CCColor3B.White;
					_touch.eventTouchBegan += touchBegan;
					_touch.eventTouchEnded += touchEnded;
					_Enabled = true;
				} else {
					_spriteNorm.Color = CCColor3B.Gray;
					_touch.eventTouchBegan -= touchBegan;
					_touch.eventTouchEnded -= touchEnded;
					_Enabled = false;
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Core.Button"/> class.
		/// </summary>
		/// <param name="father">Father node for the button.</param>
		/// <param name="tl">Touch listener.</param>
		/// <param name="method">Method to call when the button is pressed. Must be coherent with the delegate <see cref="Core.touchList.eventHandlerTouch"/>.</param>
		/// <param name="textureDefault">Texture for normal state.</param>
		/// <param name="texturePressed">Texture for pressed state.</param>
		/// <param name="position">Position of the button.</param>
		/// <param name="winSize">Window size.</param>
		/// <param name="rot">Rotation of the button.</param>
		/// <param name="scale">Scale of the button.</param>
		/// <param name="order">ZOrder (depth).</param>
		internal CCButton (CCNode father, TouchList tl, TouchList.eventHandlerTouch method, string textureDefault, string texturePressed, CCPoint position, CCSize winSize, float rot = -90, float scale = 0.55f, int order = 0)
		{

			//Defining the sprite
			_spriteNorm = new CCSprite (textureDefault);
			_spriteNorm.Position = position;
			_spriteNorm.Rotation = rot;
			_spriteNorm.Scale = scale;
			father.AddChild (_spriteNorm);

			_spritePressed = new CCSprite (texturePressed);
			_spritePressed.Position = position;
			_spritePressed.Rotation = rot;
			_spritePressed.Scale = scale;
			father.AddChild (_spritePressed);
			_spritePressed.Visible = false;

			this._winSize = winSize;
			this._father = father;

			//Defining the event variables
			_onButtonPressed = method;
			_pressed = false;
			_touch = tl;
			_touch.eventTouchBegan += touchBegan;
			_touch.eventTouchEnded += touchEnded;
		}

		/// <summary>
		/// Method added to the touch listener. Will be executed every time a touch is detected.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void touchBegan (List<CCTouch> touches, CCEvent touchEvent)
		{
			if (_spriteNorm.BoundingBoxTransformedToWorld.ContainsPoint (new CCPoint (touches [0].LocationOnScreen.X, _winSize.Height - touches [0].LocationOnScreen.Y))) {
				_pressed = true;
				_spritePressed.Visible = true;
				_spriteNorm.Visible = false;
			}
				
		}

		/// <summary>
		/// Method added to the touch listener. Will be executed every time a touch ends.
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="touchEvent">Touch event.</param>
		private void touchEnded (List<CCTouch> touches, CCEvent touchEvent)
		{
			_spriteNorm.Visible = true;
			_spritePressed.Visible = false;
			if (_spriteNorm.BoundingBoxTransformedToWorld.ContainsPoint (new CCPoint (touches [0].LocationOnScreen.X, _winSize.Height - touches [0].LocationOnScreen.Y)) && _pressed == true) {
				_onButtonPressed (touches, touchEvent);
				_pressed = false;

			}

		}

		/// <summary>
		/// Remove the instance of the <see cref="Core.Button"/>.
		/// </summary>
		internal void remove ()
		{
			_touch.eventTouchBegan -= touchBegan;
			_touch.eventTouchEnded -= touchEnded;
			_father.RemoveChild (_spriteNorm);
			_father.RemoveChild (_spritePressed);

		}
	}
}

