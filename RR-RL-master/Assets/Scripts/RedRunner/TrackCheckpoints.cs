using System;
using System.Collections;
using System.Collections.Generic;
using RedRunner.Characters;
using UnityEngine;
using UnityEngine.Events;

public class TrackCheckpoints : MonoBehaviour
{
    public event EventHandler OnPlayerCorrectCheckpoint;	//gestisco l'evento "checkpoint corretto"
    public event EventHandler OnPlayerWrongCheckpoint;

    private List<CheckpointSingle> checkpointSingleList;	//lista per memorizzare ogni singolo checkpoint e ognuno 
    private int nextCheckpointSingleIndex;
    
    
    //-----------------------------------------
    private Vector3 xPosition;
    private Vector3 xPosition_2;
    private List<Vector3> positionsList;			//metti private e fai metodo
    //-----------------------------------------
    

    private String checkpointCorrect;
    private void Awake()	//risveglia questa classe da RedAgent, caricando la lista dei checkpoint che trova nella mappa, poi crea una lista ordinata composta da ogni SingleCheckpoint in ordine
    {
		//Debug.Log("--------------------------------------------------------------------------------ENTRATO IN TRACK_CHECKPOINT - AWAKE()");
        //Transform è la libreria/classe che gestisce gli oggetti nello spazio (posizioni)
        Transform checkpointTransform = transform.Find("Checkpoints");	//Find cerca tutti i blocchi e i figli che hanno stringa 'Checkpoints' in 'Hierarchy'

        checkpointSingleList = new List<CheckpointSingle>();	//creo una lista per memorizzare i checkpoints
        
        //----------
        positionsList = new List<Vector3>();	//lista per memorizzare i valori delle posizioni vettoriali (x,y,z)
        //----------
        
        foreach (Transform checkpointSingleTransform in checkpointTransform)
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();	//estra ogni oggetto in formato oggetto 'CheckpointSingle'
            checkpointSingle.SetTrackCheckpoint(this);	//ASSEGNO AD OGNI SINGOLO CHECKPOINT ALLO STESSO MODO il 'track' A CUI APPARTIENE
            

            //-----------------------------------------
            xPosition_2 = checkpointSingleTransform.position;
            //Debug.Log("TRACK_CHECKPOINT - elemento ");
            //Debug.Log(xPosition_2);
            
            positionsList.Add(xPosition_2);
            //-----------------------------------------

            checkpointSingleList.Add(checkpointSingle);		//lista in cui carico tutti i checkpointSingle in ordine di indice
        }
        
 
        /*
        foreach (Vector3 elemento in positionsList)
        {
            
            //-----------------------------------------
            Debug.Log("elemento posizione");
            Debug.Log(elemento);
            //-----------------------------------------
        }
        */


        nextCheckpointSingleIndex = 0;	//inizializzo il contatore attuale in cui mi trovo nella lista

        checkpointCorrect = "not set";
    }
    
    //-----------------------------da rimuovere------------

	public Vector3 getCheckpointPosition(int indice)
	{
		//this.checkpointSingleList.Find()
		return this.positionsList[indice];
	}
	
	
	public CheckpointSingle getCheckpoints(int indice)	//versione L
	{    
		return checkpointSingleList[indice];
	}

    //-----------------------------------------
    
 

    public void PlayerThroughCheckpoint(CheckpointSingle checkpointSingle)	//
    {
		//Debug.Log("ENTRATO IN PLAYER_THROUGH_CHECKPOINT() - trackcheckpoints");
		//Debug.Log(checkpointSingle);
        if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex)	//quando trovo contatto con uno dei checkpoint singoli, verifico se la sua posizione si trova effettivamente nell'indice attuale che dovrebbe trovare
        {
			//Debug.Log(checkpointSingleList.IndexOf(checkpointSingle));
            checkpointCorrect = "correct";
            OnPlayerCorrectCheckpoint?.Invoke(this, EventArgs.Empty);
            Debug.Log("correct");
            nextCheckpointSingleIndex++;
            //Debug.Log(nextCheckpointSingleIndex);
        }
        else
        {
            checkpointCorrect = "wrong";
            OnPlayerWrongCheckpoint?.Invoke(this, EventArgs.Empty);
            Debug.Log("wrong");
        }
    }

    public int getCheckpointsNumber()
    {
        return checkpointSingleList.Count;
    }

    public void setCheckpointState(String checkpointCorrect)
    {
        this.checkpointCorrect = checkpointCorrect;
    }
    
    public String getCheckpointState()
    {
        return checkpointCorrect;
    }
}
