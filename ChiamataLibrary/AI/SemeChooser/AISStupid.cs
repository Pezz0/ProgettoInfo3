using System;

namespace ChiamataLibrary
{
	/// <summary>
	/// AI for seme choosing.
	/// </summary>
	public class AISStupid:IAISemeChooser
	{
		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="me">The <see cref="ChiamataLibrary.Player"/> instance representing the AI.</param>
		public void setup (Player me)
		{

		}

		/// <summary>
		/// Getter for the seme.
		/// </summary>
		/// <returns>The seme that will be chosen by this AI.</returns>
		public EnSemi? chooseSeme ()
		{
			return EnSemi.ORI;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChiamataLibrary.AISStupid"/> class.
		/// </summary>
		public AISStupid ()
		{
		}
	}
}

