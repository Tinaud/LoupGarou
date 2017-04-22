using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    bool gameStarted;
    GameObject player,
               refVoyante,
               refChasseur,
               refCupidon,
               refSorciere;

    public List<GameObject> victims = new List<GameObject>();

    int nbrPlayers;
    public List<GameObject> playerList = new List<GameObject>();
    public List<GameObject> refLoups = new List<GameObject>();
    List<string> roles = new List<string>();

	public static GameManager instance = null;
	void Awake()
    {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject); 
    }

    void Start () {
        gameStarted = false;
        player = (GameObject)Resources.Load("Player");
        nbrPlayers = 0;

        refVoyante = null;
        refChasseur = null;
        refSorciere = null;
        refCupidon = null;

        AddRoles();
	}

    void Update () {
        if (Input.GetKeyDown(KeyCode.Space) && nbrPlayers < 20 && !gameStarted)
            NewPlayer();

        if(Input.GetKeyDown(KeyCode.Return) && nbrPlayers > 5 && !gameStarted)
            StartGame();
	}

    void StartGame() {
        gameStarted = true;

        float wolfNumber = Mathf.Floor(nbrPlayers / 3.0f);

        for (int i = 0; i < wolfNumber; i++)
            roles.Add("Loup-Garou");
        for (int i = roles.Count; i < nbrPlayers; i++)
            roles.Add("Villageois");

        foreach (GameObject g in playerList) {
            int r = Random.Range(0, roles.Count);

            switch (roles[r]) {
                case "Villageois":
                    g.AddComponent<Villageois>();
                    break;
                case "Loup-Garou":
                    g.AddComponent<Loup>();
                    refLoups.Add(g);
                    break;
                case "Sorcière":
                    g.AddComponent<Sorciere>();
                    refSorciere = g;
                    break;
                case "Cupidon":
                    g.AddComponent<Cupidon>();
                    refCupidon = g;
                    break;
                case "Chasseur":
                    g.AddComponent<Chasseur>();
                    refChasseur = g;
                    break;
                case "Voyante":
                    g.AddComponent<Voyante>();
                    refVoyante = g;
                    break;
                default:
                    Debug.Log("Unknown role :(");
                    break;
            }

            roles.RemoveAt(r);
        }

        StartCoroutine(GameTurn());
    }

    public void NewPlayer() {
        nbrPlayers++;
        GameObject g = Instantiate(player, new Vector3(0, 0, 0), Quaternion.identity);
        playerList.Add(g);
        RearrangePlayers();
    }

	public void AddPlayer(GameObject p) {
		nbrPlayers++;
		playerList.Add(p);
		RearrangePlayers();
	}

    void RearrangePlayers() {
        float angle = 0.0f;

        foreach(GameObject g in playerList) {
            Vector3 pos = new Vector3(10 * Mathf.Cos(angle), 0, 10 * Mathf.Sin(angle));
            Vector3 relativePos = Vector3.zero - pos;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            
            g.transform.position = pos;
            g.transform.rotation = rotation;

            angle += (2 * Mathf.PI) / playerList.Count;
        }
    }

    void AddRoles() {
        roles.Add("Sorcière");
        roles.Add("Voyante");
        roles.Add("Cupidon");
        roles.Add("Chasseur");
    }

    public List<GameObject> GetPlayers() {
        return playerList;
    }

    public bool IsStarted() {
        return gameStarted;
    }

    public void RemoveWolf(GameObject o) {
        refLoups.Remove(o);
    }

    public void AddVictim(GameObject o) {
        victims.Add(o);
    }

    public void SaveVictim() {
        victims.Clear();
    }

    IEnumerator GameTurn() {
        //First Turn only
        yield return new WaitForSeconds(2f);

        //CUPIDON
        if(refCupidon != null) {
            refCupidon.GetComponent<BaseRole>().PlayTurn();
            yield return new WaitUntil(() => refCupidon.GetComponent<BaseRole>().IsReady());
        }

        while(gameStarted) {

            //VOYANTE
            if(refVoyante != null) {
                refVoyante.GetComponent<BaseRole>().PlayTurn();
                yield return new WaitUntil(() => refVoyante.GetComponent<BaseRole>().IsReady());
            }

            //LOUPS
            if(refLoups.Count > 0) {
                refLoups[0].GetComponent<BaseRole>().PlayTurn();
                yield return new WaitUntil(() => refLoups[0].GetComponent<BaseRole>().IsReady());
            }

            //SORCIÈRE
            if(refSorciere != null) {
                refSorciere.GetComponent<BaseRole>().PlayTurn();
                yield return new WaitUntil(() => refSorciere.GetComponent<BaseRole>().IsReady());
            }

            //FIN DE LA NUIT
            if(victims.Count > 0) {
                foreach(GameObject o in victims)
                    o.GetComponent<BaseRole>().Die();

                victims.Clear();
            }
            else
                Debug.Log("{MORT} Il n'y a aucun mort cette nuit! gg wp");

            Debug.Log(playerList.Count + " " + refLoups.Count);

            yield return new WaitForSeconds(4f);
            
            if (refLoups.Count <= 0) {
                Debug.Log("VILLAGEOIS GAGNENT!");
                gameStarted = false;
            }
            else if(playerList.Count == refLoups.Count) {
                Debug.Log("LOUPS GAGNENT!");
                gameStarted = false;
            } 
        }
    }

    public void MessageToPlayers(string Msg)
    {
        foreach (GameObject pla in playerList)
        {
            pla.GetComponent<Player>().AddMsg(Msg);
        }
    }
}