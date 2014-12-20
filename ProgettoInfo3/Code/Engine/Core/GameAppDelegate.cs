using System;
using CocosSharp;


namespace ProgettoInfo3
{

	public class GameAppDelegate : CCApplicationDelegate
	{
		public override void ApplicationDidFinishLaunching (CCApplication application, CCWindow mainWindow)
		{
			application.PreferMultiSampling = false;
			application.ContentRootDirectory = "Content";

			// TODO : Qui ci va l'inizializzazione della GameScene e di tutte le robe che servono (matrice carte, setup connessione)
			//	  Per ora è solamente una scena con un solo layer			
			GameScene gameScene = new GameScene (mainWindow);
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

