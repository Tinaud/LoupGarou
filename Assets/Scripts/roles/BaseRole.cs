using System.Collections.Generic;
using UnityEngine;

abstract public class BaseRole : MonoBehaviour {

    protected bool ready;
    protected GameObject selectedPlayer;
    public GameObject lover;
    public List<GameObject> players = new List<GameObject>();

    public virtual void Start () {
		players = GameManager.instance.GetPlayers();

        lover = null;
        selectedPlayer = null;
        //StartCoroutine(GetComponent<Player>().Vote());
	}

    public abstract void PlayTurn();
    public virtual void Die() {
        Debug.Log("{MORT} " + GetType() + " (id : " + GetComponent<Player>().ID() + ") est mouru!");

        if(lover != null) {
            Debug.Log("Son amour apporta quelqu'un dans la mort.");
            lover.GetComponent<BaseRole>().SetLover(null);
            lover.GetComponent<BaseRole>().Die();
        }

        if (GetComponent<Loup>())
            GameManager.instance.CmdRemoveWolf(gameObject);
        GetComponent<Player>().RpcChangeColor(Color.black);
        players.Remove(gameObject);
        /*Destroy(gameObject);*/
    }

    public bool IsReady() {
        return ready;
    }

    public void SetLover(GameObject l) {
        lover = l;
    }
}
