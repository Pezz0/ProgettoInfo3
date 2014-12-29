using System;
using Android.OS;
using ChiamataLibrary;

namespace BTLibrary
{
	public class BTHandler:Handler
	{
		/*TODO: scrivo qua sperando che la gente lo legga
		 * 	nei ragionamenti che abbiamo fatot ci sono delle falle, ma prima voglio discutere altro.
		 * 	
		 * 	l'inizializzazione viene eseguita alla fine dell'interfaccia che fa bono, ma devo spiegare come si esegue sulla Board
		 * 	Dal master:
		 * 		- requisiti:
		 * 			i nomi dei 5 player
		 * 			chi è il mazziere
		 * 		- procedura
		 * 			si chiama Board.Instance.InitializeMaster passando i due parametri
		 * 			si estrae la sequenza di byte da Board.Instance.toBytesArray()
		 * 			inviare a tutti gli slave la sequenza di byte
		 * 			(ora qua ho un dubbio, la Board da per scontato che la sequenza sia stata ricevuta è safe il protocollo del BT?)
		 * 
		 *	Dallo slave:
		 *		- requisiti:
		 *			il tuo nome
		 *		- procedura
		 *			si invia il proprio nome al master(cosi lui può fare il primo punto della sua procedura)
		 *			si aspetta di ricevere l'array inviato dal master nell'ultimo punto della sua procedura
		 *			si chiama il metodo Board.Instance.ricreateFromByteArray (o quello che è) passando l'array preso al passo prima
		 *				quel metodo ritorna qualcosa, ma in sto caso non serve a un cazzo
		 *			si chiama il metodo Boad.Instance.InitializeSlave passando il tuo nome
		 *			
		 *	finite queste procedure tutti gli slave e il master dovrebbero avere le board uguali.
		 *
		 *	la falla consiste nel fatto che dalla ChiamataLibrary non si ha l'accesso alle figate del BT e quindi non posso inviare il messaggio gli altri
		 *	però ci sono gli eventi quindi la mia idea è che da qualche parte qualcuno si iscrive a quegli eventi mettendo l'invio
		 *	quel da qualche parte dovrebbe essere praticamente un singleton quindi ad esempio nella BTPlayService, ma li mi sembra fuori posto
		 *	nella gamescene è altrettanto fuori posto, qualcuno sa dove piazzare sti metodi?
		 *
		 *	in concreto questi metodi devono fare cose molto semplici:
		 *		Dal master:
		 *			si invia a tutti gli slave (se la mossa la fa il master va inviata a tutti)
		 *		Dallo slave:
		 *			si invia al master(pota può inviare solo a lui)
		 *
		 *	ricordando che quando il master riceve una giocata(o bid) la invia a tutti gli altri slave(da implementare in sta classe)
		 *	il tutto dovrebbe quadrare e funzionare.
		 *
		 *	Per quanto riguarda la posizione forse il posto migliore è proprio qua dentro l'handler.
		 *	che diventerebbe la classe che gestisce tutti gli scambi BT e facendo in modo che nel resto del mondo si possa dimenticare da dove arrivino
		 *	i dati, seguendo sto ragionamento sta classe potremmo chiamara BTManager (ma è solo una idea) e si occuperebbe di implementare anche i passaggi di inizializzazione.
		 *	
		 *	inoltre usando questo metodo quando l'IA fa una mossa fa scattare sti eventi mandando la sua mossa a tutti e dato che a sto evento è iscritta anche l'animazione di pezzo
		 *	scatta pure quella e in questo modo pezzo può anche dimenticarsi del BT, come è sacrosanto e giusto che sia 
		 *	( da scrivere nella documentazione: usando il modello di stabilità di Myers garantendo disaccopiamento e coesione tra le parti usando un connettore selective broadcast)
		 *	
		 *	ultima cosa questa classe BTManager probabilmente dovrebbe essere un singleton dato che c'è solo una istanza.
		 *
		 *	commentate qua sotto e se avete un modo migliore per comunicare sti testi infiniti proponete
		 *

		*/



		public BTHandler ()
		{

		}

		public override void HandleMessage (Message msg)
		{
			switch (msg.What) {

				case (int)MessageType.MESSAGE_READ:

					if (Board.Instance.isAuctionPhase)
						Board.Instance.auctionPlaceABid ((byte []) msg.Obj);

					if (Board.Instance.isPlayTime)
						Board.Instance.PlayACard ((byte []) msg.Obj);

					Player sender = Board.Instance.getPlayer ((byte []) msg.Obj);

				break;

			}
		}

	}
}

