using System;
using System.Collections;
using System.Collections.Generic;
using RedRunner.Characters;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    private TrackCheckpoints trackCheckpoints;	//
    private void OnTriggerEnter2D(Collider2D other)
    {
		Debug.Log("-----------------------------------------------ho incrociato un checkpoint - OnTriggerEnter2D - CheckpointSingle "); 
        //TryGetComponent ritorna TRUE/FALSE se ha trovato collisione
        if (other.TryGetComponent<RedCharacter>(out RedCharacter redCharacter))	//cerca di vedere se il collider di questo oggetto trova contatto con il red_character
        {
			Debug.Log("--------------------------------------------intersezione con checkpoint");
            
            //QUESTA RIGA SERVE PER CONTROLLARE GLI INDICI DEI SINGOLI CHECKPOINT SE SONO COERENTI
            trackCheckpoints.PlayerThroughCheckpoint(this);		//richiama un metodo dell'altra classe che verifica se dentro la lista di check_point che ha, l'indice si trova nella posizione corretta incrementale del contatore che il personaggio incrementa ogni volta che ne tocca 1 
            
            //
            redCharacter.setTrackCheckpointsRed(trackCheckpoints); //ASSEGNO ALL'OGGETTO DEL PERSONAGGIO L'ATTUALE TRACCIA DI PERCORSO IN CUI SI TROVA, COSI CHE QUANDO SARà ENTRATO IN UNA NUOVA PORZIONE DI PERCORSO
        }
    }

    public void SetTrackCheckpoint(TrackCheckpoints trackCheckpoints)
    {
        this.trackCheckpoints = trackCheckpoints;
    }
    
    public string getBlockName()
    {
        return transform.parent.parent.name;
    }

    public TrackCheckpoints getBlockTrackCheckpoints()
    {
        return transform.parent.parent.GetComponent<TrackCheckpoints>();
    }
}
