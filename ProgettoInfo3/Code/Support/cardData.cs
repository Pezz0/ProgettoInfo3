using System;
using CocosSharp;

namespace Core
{
	/// <summary>
	/// Stores the sprite, position and rotation data for a card.
	/// </summary>
	public class CardData
	{
		/// <summary>
		/// The card sprite.
		/// </summary>
		public  CCSprite sprite;

		/// <summary>
		/// The base position of the sprite (if the card is dropped in a "wrong" zone, it will return to the base position).
		/// </summary>
		private CCPoint _posBase;

		/// <summary>
		/// Gets or sets the base position.
		/// </summary>
		/// <value>The base position.</value>
		public CCPoint posBase{ get { return _posBase; } set { _posBase = value; } }

		/// <summary>
		/// The rotation of the card.
		/// </summary>
		private float _rotation;

		/// <summary>
		/// Gets or sets the rotation.
		/// </summary>
		/// <value>The rotation of the card.</value>
		public float rotation{ get { return _rotation; } set { _rotation = value; } }

		/// <summary>
		/// The index the card had in the starting hand.
		/// </summary>
		private int _index;

		/// <summary>
		/// Gets or sets the index.
		/// </summary>
		/// <value>The index the card had in the starting hand.</value>
		public int index { get { return _index; } set { _index = value; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Core.CardData"/> class.
		/// </summary>
		/// <param name="s">Card sprite.</param>
		/// <param name="p">Card position.</param>
		/// <param name="r">Card rotation.</param>
		/// <param name="ind">Card index.</param>
		public CardData (CCSprite s, CCPoint p, float r, int ind)
		{
			sprite = s;
			_posBase = p;
			_rotation = r;
			_index = ind;
		}
	}
}

