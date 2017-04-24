using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

abstract public class BaseRole : NetworkBehaviour {

    protected bool ready;
    protected GameObject selectedPlayer;

	[SyncVar]
    public GameObject lover;

    public List<GameObject> players = new List<GameObject>();

    public virtual void Start () {
		players =  GameManager.instance.GetPlayers();

        lover = null;
        selectedPlayer = null;
        //StartCoroutine(GetComponent<Player>().Vote());
	}

    public abstract void PlayTurn();
    public virtual void Die() {
        Debug.Log("{MORT} " + GetType() + " (id : " + GetComponent<Player>().ID() + ") est mouru!");

		CmdRemovePlayer (gameObject);
        /*Destroy(gameObject);*/
    }

	[Command]
	void CmdRemovePlayer(GameObject _p) {
		if (GetComponent<Loup>())
			GameManager.instance.RemoveWolf (_p);

		GameManager.instance.RemovePlayer (_p);
	}

    public bool IsReady() {
        return ready;
    }

    public void SetLover(GameObject l) {
        lover = l;
    }

}
