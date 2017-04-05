using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    bool gameStarted;
    static int nextId = 0;
    int id;
    public string role;
    GameManager gm;
    Quaternion targetRotation;

    void Start() {
        gameStarted = false;
        id = nextId++;
    }

    void Update() {
        if(gameStarted)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);
    }

    public void StartGame(string r) {
        gameStarted = true;
        gm = Camera.main.GetComponent<GameManager>();
        role = r;
        StartCoroutine(Vote());
    }

    IEnumerator Vote() {
        while(gameStarted) {
            yield return new WaitForSeconds(Random.Range(2.0f, 4.0f));

            GameObject playerToVote;

            do
                playerToVote = gm.GetPlayers()[Random.Range(0, gm.GetPlayers().Count)];
            while (id == playerToVote.GetComponent<Player>().ID());

            Vector3 relativePos = playerToVote.transform.position - transform.position;
            targetRotation = Quaternion.LookRotation(relativePos);
        }
    }

    public int ID() {
        return id;
    }
}
