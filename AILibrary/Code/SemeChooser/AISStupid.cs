using System;
using ChiamataLibrary;

namespace AILibrary
{
	/// <summary>
	/// AI for seme choosing.
	/// </summary>
	public class AISStupid:IAISemeChooser
	{

		/// <summary>
		/// Getter for the seme.
		/// </summary>
		/// <returns>The seme that will be chosen by this AI.</returns>
		public EnSemi ChooseSeme (Player me)
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

