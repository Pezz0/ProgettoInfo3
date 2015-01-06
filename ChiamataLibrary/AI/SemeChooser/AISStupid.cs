using System;

namespace ChiamataLibrary
{
	/// <summary>
	///  A stupid seme chooser
	/// </summary>
	public class AISStupid:IAISemeChooser
	{
		public void setup (Player me)
		{

		}

		public EnSemi? chooseSeme ()
		{
			return EnSemi.ORI;
		}

		public AISStupid ()
		{
		}
	}
}

