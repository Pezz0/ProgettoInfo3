Spiegazione new Board

La nuova board è pronta, funzionante, ma con cose diverse.

La principale modifica riguarda come si inviano i comandi (place a bid, choose seme e play a card) alla board infatti ora i rispettivi metodi non esistono più.

Nella classe player ci sono due nuovi metodi:
  - setControAuction




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
