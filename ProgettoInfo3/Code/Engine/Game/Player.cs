using System;

namespace Engine
{
	public class Player
	{
		/// <summary>
		/// The board used by this player.
		/// </summary>
		private Board _board;

		#region order management

		/// <summary>
		/// The order.
		/// </summary>
		private int _order;

		/// <summary>
		/// Gets the order.
		/// </summary>
		/// <value>The order.</value>
		public int Order { get { return _order; } }

		#endregion

		#region Name management

		/// <summary>
		/// The player's name.
		/// </summary>
		private string _name;

		/// <summary>
		/// Gets the player's name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get { return _name; } }

		#endregion

		#region Role management

		/// <summary>
		/// The player's role.
		/// </summary>
		private EnRole _role;

		/// <summary>
		/// Gets or sets the player's role, the role can't be change during the play time.
		/// </summary>
		/// <value>The role.</value>
		public EnRole Role {
			get { return _role; }
			set {
				if (_board.isPlayTime)
					throw new Exception ("The role can't be change during the play time");
				_role = value;
			}
		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="Engine.Player"/> class.
		/// </summary>
		/// <param name="board">The board used by this player.</param>
		/// <param name="name">The player's name.</param>
		/// <param name="order">The player's order </param>
		public Player (Board board, string name, int order)
		{
			if (!_board.isCreationPhase)
				throw new Exception ("A player must be instantiated during the creation time");
			_board = board;
			_name = name;
			_order = order;
		}

		public override string ToString ()
		{
			return string.Format ("[Player: Order={0}, Name={1}, Role={2}]", Order, Name, Role);
		}
	}
}
