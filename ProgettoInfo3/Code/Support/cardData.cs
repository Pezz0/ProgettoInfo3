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

		private int _index;

		public int index { get { return _index; } set { _index = value; } }

		public CardData (CCSprite s, CCPoint p, float r, int ind)
		{
			sprite = s;
			_posBase = p;
			_rotation = r;
			_index = ind;
		}
	}
}

