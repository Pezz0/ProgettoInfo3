ProgettoInfo3
=============

Progetto di informatica 3, (Bonomi,Pezzoli,Scandella) A.S 2014/2015

Uso questo spazio per comunicare con voi di due argomenti: inizializzazione della board e una falla sui nostri ragionamenti.

Inizializzazione

Nella Board ho inserito i metodi per renderla IBTSendable e scrivo qua la procedura per ottenere le board nello stesso stato su tutti i device.
Questi metodi si basano sul fatto che il BT sia safe(non ho messo ack o cose cosi)

  Dal master:
    requisiti:
      - i nomi dei 5 player
      - chi è il mazziere
    procedura
      - si chiama Board.Instance.InitializeMaster passando i due parametri
      - si estrae la sequenza di byte da Board.Instance.toBytesArray()
      - inviare a tutti gli slave la sequenza di byte
Dallo slave:
    requisiti:
      - il tuo nome
    procedura
      - si invia il proprio nome al master(cosi lui può fare il primo punto della sua procedura)
      - si aspetta di ricevere l'array inviato dal master nell'ultimo punto della sua procedura
      -	si chiama il metodo Board.Instance.ricreateFromByteArray (o quello che è) passando l'array preso al passo prima
        quel metodo ritorna qualcosa, ma in sto caso non serve a un cazzo
      - 	si chiama il metodo Boad.Instance.InitializeSlave passando il tuo nome  
      
finite queste procedure tutti gli slave e il master dovrebbero avere le board uguali.

Queste procedure vanno eseguite a inizio partita e quindi quando si è ancora nei menu.

Falla nei nostri ragionamenti

la falla consiste nel fatto che dalla ChiamataLibrary non si ha l'accesso alle figate del BT 
quindi da li non si può inviare i messaggi di "placeABid" e "PlayACard".

I due metodi però hanno già degli eventi attaccati per l'animazione di pezzo quindi la mia idea è di attaccare a questi eventi
anche un metodo che invia i messaggi agli altri.

questi metodi che inviano dovrebbero fare una cosa molto semplice:
  Dal master:
    - si invia a tutti gli slave (se la mossa la fa il master va inviata a tutti)
  Dallo slave:
    - si invia al master(pota può inviare solo a lui)

dato che quando il master riceve un messaggio da uno slave lo inoltra agli altri slave tutto dovrebbe funzionare.
    
Questi metodi vanno messi in un singleton e quindi le opzioni sono:
  - BTPlayService: qua mi sembra fuori posto e fuori luogo
  - GameScene: non è proprio un singleton, ma vabbè e comunque non va bene qua c'è solo roba gragica
quindi io trasformerei il BTHandler in un singleton chiamato BTManager che oltre a fare l'handler contiene anche questi metodi 
che permettono l'invio.

questa soluzione comporta alcuni vantaggi:

  - le IA non rompono le palle, infatti le IA chiameranno i metodi PlayACard e PlaceABid che fanno scattare gli eventi e quindi
    sia l'invio a tutti gli altri sia l'animazione
  - A pezzo non gliene frega nulla, infatti in questo modo a pezzo non interessa da dove viene il comando
    (da una IA, dal bluetooth o da un cinese che ha hackerato la nostra app)
  - quando pezzo da un comando chiama gli eventi in automatico e invia a tutti gli altri  

se avete commenti modificate pure sto readme aggiungendo roba e almeno confermate che vi quadri tutto.
