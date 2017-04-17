using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour {

    static int nextId = 0;
    public int id;
    GameManager gm;
    Quaternion targetRotation;

    void Start() {
        id = nextId++;
        gm = Camera.main.GetComponent<GameManager>();
    }

    void Update() {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);
    }

    public IEnumerator Vote() {
        while(true) {
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
