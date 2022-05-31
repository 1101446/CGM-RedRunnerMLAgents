# CGM-RedRunnerMLAgents

Progetto Computer Graphics e Multimedia A.A. 2021/2022

## Group Members
- Alex Giacomini
- Giacomo Licci
- Fiorenza Bocchini

## COMMAND WINDOW AND UNITY USEFUL COMMANDS/OPERATIONS
### COMANDI ML-AGENTS TERMINALE
	•	mlagents-learn --run-id=test3-run1 (esegue un test con run id specificato)
	•	mlagents-learn --run-id=test3-run1 --force(sovrascrive il file e riparte da 0 l'allenamento)
	•	mlagents-learn --run-id=test3-run1 --resume(continua stesso allenamento da dove si è fermato)
	•	mlagents-learn config/FILE.yaml  --run-id=test3-run1 (per utilizzare uno specifico file di configurazione di iperparametri)
	•	mlagents-learn config/FILE.yaml  --run-id=test3-run2 --initialize-from=test3-run1(per far partire un addestramento da un training precedente)
	•	tensorboard --logdir results --port 6006
	  ⁃	localhost:6006/#scalars (da barra di ricerca google)


### COMANDI PER UNITY
	•	importare rete: 
	  ⁃	Assets -> import new asset (e lo selezioni) -> trascinare la rete in Red_Agent (Behaviour parameters)


### REPOSITORY CON DESCRIZIONE IPERPARAMETRI (E RANGE TIPICI DI LAVORO DEGLI STESSI)
  - https://github.com/Unity-Technologies/ml-agents/blob/main/docs/Training-Configuration-File.md



  
