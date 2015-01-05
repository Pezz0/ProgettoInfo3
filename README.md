Spiegazione new Board

La nuova board è pronta, funzionante, ma con cose diverse.

La principale modifica riguarda come si inviano i comandi (place a bid, choose seme e play a card) alla board infatti ora i rispettivi metodi non esistono più.

Nella classe player ci sono due nuovi metodi:
  - setControAuction: questo metodo richiede due delegati nei parametri:
    - il primo deve restituire una bid
    - il seconde deve restituire un seme
  - seControlPlaytime: questo metodo richiede un delegato nei parametri che deve restituire una carta-

questi 3 delegati saranno chiamati dalla board a tempo debito per mettere una determinata bid, card o scelta del seme di quel player.

Ad esempio ora il BTManager deve settare i player comandati dal BT assegnando questi 3 metodi a dovere.

Per l'ui è uguale i metodi conterrano l'animazione(la board si ferma ad aspettare il return del metodo quindi si può mettere tutte le attese del mondo dentro) e restituranno l'azione fatta (bid piazzata, seme scelto o carta giocata).
E si potranno assegnare con Board.Instance.Me.

A questo sistema di controllo si affiancano ancora gli eventi di prima:
  - someonePlaceABid: qualcuno che non il proprietario del device ha messo una bid
  - IPlaceABid: il proprietario del device ha fatto una bid
  - someonePlayACard: qualcuno che non il proprietario del device ha giocato una carta
  - IPlayACard: il proprietario del device ha giocato una carta
  - AuctionStart: inizia l'asta
  - GameStart: iniza il playtime
  - GameFinish: fine partita

A cui ci si potrà iscrivere sia per inviare i dati agli altri(BTManager) sia per fare le animazioni(Gamescene).

Anche il modo in cui le AI è cambiato e ve le spiegherò alle prossima riunione per vedere come si inizializza si può vedere il program.cs e se volete crearne alcune guardate come ho fatto quelle che ci sono.

L'inizializzazione rimane invariata, mentre sparisce il metodo startGame che viene sostituito dal metodo .run che svolgono la stessa funzione praticamente.

Io ho inserito la mia nuova board e tutto ciò che ci sta intorno adattando tutto e sostituisco il consoleProject con uno di esempio funzionante col nuovo metodo (vanno anche le AI, non so se intelligentemente, ma vanno).

Però ci sono tanti errori dato che le altre parti usano ancora la vecchia board e dato che non so fare le vostre parti(e non ne ho la minima voglia dopo tutto il giorno che preparo la dannata nuova board) li lascio li e li sistemerete voi.

(io le warning le ho ancora... dannate)

-------------------------------------
Inizializzazione

Nella Board ho inserito i metodi per renderla IBTSendable e scrivo qua la procedura per ottenere le board nello stesso stato su tutti i device.
Questi metodi si basano sul fatto che il BT sia safe(non ho messo ack o cose cosi)

  - Dal master:
    - requisiti:
      - i nomi dei 5 player
      - chi è il mazziere
    - procedura
      - si chiama Board.Instance.InitializeMaster passando i due parametri
      - si estrae la sequenza di byte da Board.Instance.toBytesArray()
      - inviare a tutti gli slave la sequenza di byte
  - Dallo slave:
    - requisiti:
      - il tuo nome
    - procedura
      - si invia il proprio nome al master(cosi lui può fare il primo punto della sua procedura)
      - si aspetta di ricevere l'array inviato dal master nell'ultimo punto della sua procedura
      -	si chiama il metodo Board.Instance.ricreateFromByteArray (o quello che è) passando l'array preso al passo prima
        quel metodo ritorna qualcosa, ma in sto caso non serve a un cazzo
      - 	si chiama il metodo Boad.Instance.InitializeSlave passando il tuo nome  
      
finite queste procedure tutti gli slave e il master dovrebbero avere le board uguali.

Queste procedure vanno eseguite a inizio partita e quindi quando si è ancora nei menu.
