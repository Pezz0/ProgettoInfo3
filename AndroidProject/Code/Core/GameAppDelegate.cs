using System;
using CocosSharp;
using GUILayout;


namespace GUILayout
{
	/// <summary>
	/// Game app delegate.
	/// </summary>
	internal class GameAppDelegate : CCApplicationDelegate
	{
		/// <summary>
		/// The message used to decide whether or not the game should continue.
		/// </summary>
		private readonly TerminateMessage _terminateMsg;

	
		/// <summary>
		/// Initializes a new instance of the <see cref="Core.GameAppDelegate"/> class.
		/// </summary>
		/// <param name="terminateMsg">The message used to decide whether or not the game should continue.</param>
		internal GameAppDelegate (TerminateMessage terminateMsg)
		{
			this._terminateMsg = terminateMsg;
		}

		/// <summary>
		/// Called when the applicaiton finished launching.
		/// </summary>
		/// <param name="application">Application.</param>
		/// <param name="mainWindow">Main window.</param>
		public override void ApplicationDidFinishLaunching (CCApplication application, CCWindow mainWindow)
		{
			application.PreferMultiSampling = false;
			application.ContentRootDirectory = "Content";

			GameScene gameScene = new GameScene (mainWindow, _terminateMsg);

			mainWindow.RunWithScene (gameScene);


		}

		/// <summary>
		/// Called when the application enters background.
		/// </summary>
		/// <param name="application">Application.</param>
		public override void ApplicationDidEnterBackground (CCApplication application)
		{
		}

		/// <summary>
		/// Called when the application resumes.
		/// </summary>
		/// <param name="application">Application.</param>
		public override void ApplicationWillEnterForeground (CCApplication application)
		{
		}


	}

}

