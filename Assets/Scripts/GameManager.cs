using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    bool gameStarted;
    GameObject player;
    int nbrPlayers;
    List<GameObject> playerList = new List<GameObject>();
    List<string> roles = new List<string>();

	void Start () {
        gameStarted = false;
        player = (GameObject)Resources.Load("Player");
        nbrPlayers = 1;
        AddRoles();
        NewPlayer();
	}

	void Update () {
        if (Input.GetKeyDown(KeyCode.Space) && nbrPlayers < 20 && !gameStarted) {
            nbrPlayers++;
            NewPlayer();
        }

        if(Input.GetKeyDown(KeyCode.Return) && nbrPlayers > 5 && !gameStarted) {
            float wolfNumber = Mathf.Floor(nbrPlayers / 3);

            for(int i = 0; i < wolfNumber; i++)
                roles.Add("Loup-Garou");
            for(int i = roles.Count; i < nbrPlayers; i++)
                roles.Add("Villageois");

            StartGame();
        }
	}

    void StartGame() {
        gameStarted = true;

        foreach (GameObject g in playerList) {
            int r = Random.Range(0, roles.Count);
            g.GetComponent<Player>().StartGame(roles[r]);
            roles.RemoveAt(r);
        }
    }

    void NewPlayer() {
        GameObject g = Instantiate(player, new Vector3(0, 0, 0), Quaternion.identity);
        playerList.Add(g);
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
}
