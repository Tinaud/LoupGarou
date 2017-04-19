using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    static int nextId = 0;
    public int id;
    GameManager gm;
    Quaternion targetRotation;

    void Start() {
		if (isLocalPlayer) {
			id = nextId++;
			gm = Camera.main.GetComponent<GameManager> ();
		}
    }

	public override void OnStartLocalPlayer() {
		changeColor (Color.red);
	}
		
	public void changeColor(Color c) {
		GetComponent<MeshRenderer> ().material.color = c;
	}

    void Update() {
		if (isLocalPlayer) {
			transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime * 2.0f);
		}
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
