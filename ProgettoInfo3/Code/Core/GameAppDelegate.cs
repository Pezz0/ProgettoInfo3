using System;
using CocosSharp;
using MenuLayout;


namespace Core
{

	public class GameAppDelegate : CCApplicationDelegate
	{
		private TerminateMessage _terminateMsg;

	

		public GameAppDelegate (TerminateMessage terminateMsg)
		{

			this._terminateMsg = terminateMsg;
		}


		public override void ApplicationDidFinishLaunching (CCApplication application, CCWindow mainWindow)
		{
			application.PreferMultiSampling = false;
			application.ContentRootDirectory = "Content";

			GameScene gameScene = new GameScene (mainWindow, _terminateMsg);

			mainWindow.RunWithScene (gameScene);


		}

		public override void ApplicationDidEnterBackground (CCApplication application)
		{
		}

		public override void ApplicationWillEnterForeground (CCApplication application)
		{
		}


	}

}

