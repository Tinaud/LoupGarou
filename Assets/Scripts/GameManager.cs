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
        nbrPlayers = 0;
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
                    break;
                case "Sorcière":
                    g.AddComponent<Sorciere>();
                    break;
                case "Cupidon":
                    g.AddComponent<Cupidon>();
                    break;
                case "Chasseur":
                    g.AddComponent<Chasseur>();
                    break;
                case "Voyante":
                    g.AddComponent<Voyante>();
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

    IEnumerator GameTurn() {
        while(gameStarted) {
            yield return new WaitForSeconds(4f);
        }
    }
}