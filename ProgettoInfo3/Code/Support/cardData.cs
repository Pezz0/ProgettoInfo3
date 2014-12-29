using System;
using CocosSharp;

namespace Core
{
	public class CardData
	{
		public  CCSprite sprite;

		private CCPoint _posBase;

		public CCPoint posBase{ get { return _posBase; } set { _posBase = value; } }

		private float _rotation;

		public float rotation{ get { return _rotation; } set { _rotation = value; } }

		public CardData (CCSprite s, CCPoint p, float r)
		{
			sprite = s;
			_posBase = p;
			_rotation = r;
		}
	}
}

