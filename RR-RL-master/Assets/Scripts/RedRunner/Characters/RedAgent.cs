using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using RedRunner;
using RedRunner.Characters;
using RedRunner.TerrainGeneration;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityStandardAssets.CrossPlatformInput;

public class RedAgent : Agent
{
    //oggetto con tutti attributi del personaggio e alcuni metodi (forse alla fine per migliorare prestazioni del personaggio)
    private RedCharacter redrunner;	
    private TrackCheckpoints trackCheckpoints; //funzione che memorizza tutti i checkpoint
    private int currentBlockCheckpointsNumber;
    private TrackCheckpoints currentTrackCheckpoints;
    private bool firstCheckpointPassed;
    private bool agentDead;
    private int stepsSinceLastCheckpoint;
    Rigidbody2D redrunnerRigidbody;


    //----------------area definizione dei parametri
    //private Vector3 checkpoint_position;	//al momento dichiarato all'interno di 'collect_observation()'

    //----------------
    //private Vector3 checkpoint_position;


    //private Vector3 checkpoint_position2;
    
    private Vector3 checkpoint_position_ = new Vector3();
    private Vector3 checkpoint_position_2_ = new Vector3();
    private Vector3 appoggio_checkpoint_position;
    private bool subscribed;
    private bool subscribed2 = true; 
    private Vector3 accumulo_posizione_tracks = new Vector3(x:0f,y:0f,z:0f);
    private int currentBlockCheckpointsNumber2;
    private int dimTrackCheckPoint;
    private int dimTrackCheckPoint2;
    private int contatore = 0;	//per memorizzare il numero di tracce percorse
    //----------------

    [SerializeField] private int maxEnvironmentStep;
    
    

    public override void OnEpisodeBegin()		//parametri di inizio episodio, non fa altro che richiamare FirstTrackCheckpoint() di importante
    {
		//Debug.Log("ENTRATO IN OnEpisodeBegin() ");
        currentBlockCheckpointsNumber = 0;
        //-----
        currentBlockCheckpointsNumber2 = 0;
        //-----
        firstCheckpointPassed = false;
        StartCoroutine(FirstTrackCheckpoint());
        agentDead = false;
        stepsSinceLastCheckpoint = 0;
        redrunnerRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void setAgentDead(bool agentDead)	//Personaggio muore e attiva questo
    {
        this.agentDead = agentDead;
    }
    
    IEnumerator FirstTrackCheckpoint()
    {
		//Debug.Log("ENTRATO IN FirstTrackCheckpoint() ");
        yield return new WaitForSeconds (1f * Time.timeScale);
        //Debug.Log("-------------------------------------------------------------------------------------------------Terrain Generator");
        trackCheckpoints = TerrainGenerator.Singleton.GetCharacterBlock().GetComponent<TrackCheckpoints>();		//genera delle aree con checkpoints (NUMERO VARIABILE A SECONDA DELLA DISTANZA A CUI PUò ARRIVARE) - e le prendo in formato oggetto 'trackcheckpoints'
        currentTrackCheckpoints = trackCheckpoints;		//metto riferimento al primo checkpoint di una lista
        Subscribe(trackCheckpoints);
        redrunnerRigidbody.constraints &= ~RigidbodyConstraints2D.FreezePosition;
    }

    private void Subscribe(TrackCheckpoints tc)	//mi allaccio ad un certo blocco di percorso e monitoro i checkpoints se ci passo in ordine coerente
    {
        Debug.Log("-------------------------------------------------------------SUBSCRIBED TO " + tc);
        tc.OnPlayerCorrectCheckpoint += TrackCheckpoints_OnPlayerCorrectCheckpoint;	//assegno questi 2 metodi a degli attirbuti della classe, che faranno delle operazioni mentre l'agente si muove
        tc.OnPlayerWrongCheckpoint += TrackCheckpoints_OnPlayerWrongCheckpoint;
        //---------variabile sotto per passare ai CHECKPOINT VETTORIALI-------
        subscribed = true;
        //----------------
    }



//viene richiamato da Trackcheckpoints in 'PlayerThroughCheckpoint'
    private void TrackCheckpoints_OnPlayerCorrectCheckpoint(object sender, System.EventArgs e)		//cosa succede se prendo il giusto checkpoint
    {
		//Debug.Log("ENTRATO IN OnPlayerCorrectCheckpoint ");
        stepsSinceLastCheckpoint = 0;
        firstCheckpointPassed = true;
        Debug.Log("reward added");
        
        AddReward(1f);
        currentBlockCheckpointsNumber++;
        //Debug.Log("-------------------------------------------------indice");
        //Debug.Log(currentBlockCheckpointsNumber);
        if (currentBlockCheckpointsNumber == currentTrackCheckpoints.getCheckpointsNumber())
        {
			//----------
			//Debug.Log("---------------------------------sono arrivato alla fine di questa area di gioco");
			

			appoggio_checkpoint_position = currentTrackCheckpoints.getCheckpoints(currentTrackCheckpoints.getCheckpointsNumber()-1).transform.localPosition;//
			accumulo_posizione_tracks += appoggio_checkpoint_position;//
			appoggio_checkpoint_position = currentTrackCheckpoints.getCheckpoints(currentTrackCheckpoints.getCheckpointsNumber()-1).transform.localPosition;
       	    accumulo_posizione_tracks += appoggio_checkpoint_position;
            currentBlockCheckpointsNumber = 1;	//perchè ci servirà di partire di passargli il secondo checkpoint al runner, dopo che si sarà iscritto alla traccia avendo già attraversato il primo checkpoint
			//----------

            //currentBlockCheckpointsNumber = 0;
        }
    }
    
    private void TrackCheckpoints_OnPlayerWrongCheckpoint(object sender, System.EventArgs e)		//cosa succede se prendo il checkpoint sbagliato
    {
		//Debug.Log("ENTRATO IN OnPlayerWrongCheckpoint ");
        stepsSinceLastCheckpoint = 0;
        Debug.Log("penalty added");
        AddReward(-1f);
        Unsubscribe(trackCheckpoints);
        redrunner.Die();
        EndEpisode();
    }

    private void Awake()		//Use Awake to initialize variables or states before the application starts 
    {
		//Debug.Log("AWAKE() - RED_AGENT");
        redrunner = GetComponent<RedCharacter>();
        Academy.Instance.AutomaticSteppingEnabled = false;
        redrunnerRigidbody = GetComponent<Rigidbody2D>();
    }
    
    //NON DA MODIFICARE, serve solo per terminare il gioco se pupino sta sempre fermo ì
    void Update()							//si attiva da solo di continuo e verifica se non ha ancora trovato un nuovo checkpoint dopo un TOT. di iterazioni ed eventualmente uccide il personaggio
    {
        Academy.Instance.EnvironmentStep();
        
        stepsSinceLastCheckpoint++;
        if (stepsSinceLastCheckpoint >= maxEnvironmentStep)
        {
            redrunner.Die(false);
            EndEpisode();
        }
    }

    private void FixedUpdate()		//va sempre da sola, si attiva volta per volta
    {
		//Debug.Log("FIXED_UPDATE");
        if (firstCheckpointPassed){	//si attiva da subito appena incontra il primo checkpoint
			//Debug.Log("------------------------------------------------------First Trackcheckpoint passed");
            trackCheckpoints = redrunner.getTrackCheckpointsRed();		//DEFINITO DA METODO AWAKE()

            if ((trackCheckpoints != currentTrackCheckpoints))		//si attiva quando una nuova porzione di percorso viene attraversata (quindi le tracce non corrispondono e assegno la nuova traccia)
            {
				//CI ENTRA NEL MOMENTO STESSO IN CUI IL PERSONAGGIO INCROCIA UN CHECKPOINT DI UNA NUOVA AREA
				//Debug.Log("ENTRATO IN FixedUpdate() - trackCheckpoints != currentTrackCheckpoints ");
                currentTrackCheckpoints = trackCheckpoints;	//SE SONO ARRIVATO AD UN NUOVO TRACKCHECKPOINT, ALLORA LO ASSEGNO ALLO STATO CORRENTE
                Subscribe(trackCheckpoints);	//POI SOTTOSCRIVO IL MONITORAGGIO DELL'AGENT AL NUOVO BLOCCO DI CHECKPOINT
                //Debug.Log("rew fixedupdate");
                //Debug.Log("reward added");
                AddReward(1f);
            }
        }
        
        if (agentDead)			//si attiva quando il personaggio muore
        {
            AddReward(-1f);
            //accumulo_posizione_tracks = new Vector3(x:0f,y:0f,z:0f);
            Debug.Log("character dead");
            Unsubscribe(trackCheckpoints);
            EndEpisode();
            agentDead = false;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {

        Vector3 red_position = redrunner.transform.localPosition;
        sensor.AddObservation(red_position);
        
        //sensor.AddObservation(redrunner.transform.localPosition);
        sensor.AddObservation(redrunner.GetComponent<Rigidbody2D>().velocity.x);
        //PROBABILMENTE DEVI PASSARE L'INFORMAZIONE DELLA POSIZIONE DEI CHECKPOINT SUL SENSORE.
 
		
        //--------------------PARTE DI CODICE DOVE IMPLEMENTO------------------------------------------------------------------
        
        if (subscribed)
        {
			
			if (currentTrackCheckpoints)
			{
				checkpoint_position_ = currentTrackCheckpoints.getCheckpoints(currentBlockCheckpointsNumber).transform.position;
				
				
				if (red_position[0] > checkpoint_position_[0])	//ci entra quando ho superato l'ultimo checkpoint di ogni traccia
                {
					checkpoint_position_ = currentTrackCheckpoints.getCheckpoints(currentTrackCheckpoints.getCheckpointsNumber()-1).transform.position;
					checkpoint_position_2_ = checkpoint_position_;
					
					checkpoint_position_[0] += 10;
					checkpoint_position_2_[0] += 20;
					subscribed = false;
					
		
               	}
				else
                {				
					//parte per gestire 2 checkpoint da passare al sensore insieme, altrimenti toglierla, se si vuole provare con 1 checkpoint

					if (currentBlockCheckpointsNumber + 1 >= currentTrackCheckpoints.getCheckpointsNumber())
					{
						checkpoint_position_2_ = checkpoint_position_;
						checkpoint_position_2_[0] += 10;
					}
					else
					{
						checkpoint_position_2_ = currentTrackCheckpoints.getCheckpoints(currentBlockCheckpointsNumber+1).transform.position;
					}

				}
					
			}
		
		}				

		//Debug.Log("----------------------------");
		//Debug.Log("check_red");
		//Debug.Log(red_position);
		//Debug.Log("check_p1");//
		//Debug.Log(checkpoint_position_);//
		//Debug.Log("check_p2");
		//Debug.Log(checkpoint_position_2_);
			
		//--------------sensor adding the vector position of the checkpoint
		sensor.AddObservation(checkpoint_position_);
		sensor.AddObservation(checkpoint_position_2_);	
	}
		
		
		
		
		/*	//ALCUNE PROVE
		if (currentTrackCheckpoints != null)	//------------------versione L
        {
            //Passaggio dei valori vettoriale
            foreach (CheckpointSingle checkpoint in currentTrackCheckpoints.getCheckpoints())
            {
                int index = currentTrackCheckpoints.getCheckpoints().IndexOf(checkpoint);
                //Debug.Log(checkpoint.transform.localPosition);
                if (checkpoint.transform.localPosition.x > redrunner.transform.localPosition.x & index > currentBlockCheckpointsNumber)
                {
                    sensor.AddObservation(checkpoint.transform.localPosition.x);
                    //sensor.AddObservation(checkpoint.transform.localPosition.y);
                }
            }
        }
		*/
		
		/*
			if (red_position[0] >= (checkpoint_position[0]+ accumulo_posizione_tracks))		//-----------versione A
			{
				Debug.Log(red_position[0]);
				Debug.Log(checkpoint_position[0] + accumulo_posizione_tracks);
				
				currentBlockCheckpointsNumber2++;
				if (currentBlockCheckpointsNumber2 == dimTrackCheckPoint)
				{
					Debug.Log("---------------------sono arrivato alla fine di questa area di gioco - VERSIONE MIA");
					
					subscribed = false;
					//subscribed2 = true;
					accumulo_posizione_tracks = accumulo_posizione_tracks + checkpoint_position[0]; //per memorizzare la distanza che sta percorrendo il personaggio
					currentBlockCheckpointsNumber2 = 0;
					contatore = contatore +1;
				}
				
			}
			*/
        //---------------------------------------------------------------------------------------------------------------------   

    
    //non credo che questa sia da modificare, perchè a partire dall'input Buffer poi attiva i metodi per fargli fare movimenti
    public override void OnActionReceived(ActionBuffers actionBuffers)		//metodo che modifica alcuni parametri dopo aver ricevuto il buffer per far fare le osservazioni effettive all'Agent
    {
        redrunner.directionFloat = actionBuffers.ContinuousActions[0];
        var jump = actionBuffers.DiscreteActions[0];	//contiene un valore tra 0-1-2, e solo con 1 salta
        //Debug.Log("----VAR JUMP----");
        //Debug.Log(jump);

        switch (jump)			//può erroneamente valere 3 valori (0,1,2), ma lui salta solo quando avrà 1 nella variabile (VERIFICA SE IL VALORE 2 SERVE A LUI DA QUALCHE PARTE)
        {
            case 0:
				//Debug.Log("----NON SALTO----");
                redrunner.jumping = 0; // not jumping
                break;
            case 1:
				//Debug.Log("----SALTO----");
                redrunner.jumping = 1; // jump button pressed
                AddReward(-0.1f);
                break;
        }
        AddReward(-0.1f / maxEnvironmentStep);
    }

    //Euristica per settare i movimenti effettivi da far fare al personaggio/Agent (((SE USO COMANDI IN INPUT)))
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Time.timeScale = 1f;
        var discreteActionsOut = actionsOut.DiscreteActions;
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = CrossPlatformInputManager.GetAxis("Horizontal");
        
        if (CrossPlatformInputManager.GetButtonDown ("Jump"))
        {
            discreteActionsOut[0] = 1;
        }
    }

	//per dare il reward negativo quando muore
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Instakill"))
        {
            redrunner.Die();
            AddReward(-1f);
            EndEpisode();
        }
        /*
        if (other.CompareTag("Checkpoint"))
        {
            Debug.Log("----------------------------------------------------------------------------------Checkpoint Collider trovato");
        }
        */
    }

    private void Unsubscribe(TrackCheckpoints tc)		//mi stacco da una certa porzione di gioco per collegarmi alle successive
    {
        //Debug.Log("---------------------------------------------------------------UNSUBSCRIBED TO " + tc);
        tc.OnPlayerCorrectCheckpoint -= TrackCheckpoints_OnPlayerCorrectCheckpoint;
        tc.OnPlayerWrongCheckpoint -= TrackCheckpoints_OnPlayerWrongCheckpoint;
    }
}
