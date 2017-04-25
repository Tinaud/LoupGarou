using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

abstract public class BaseRole : NetworkBehaviour {

    protected bool ready;

    [SyncVar]
    protected GameObject selectedPlayer;

	[SyncVar]
    public GameObject lover;

    public List<GameObject> players = new List<GameObject>();

    public virtual void Start () {
		players =  GameManager.instance.GetPlayers();

        lover = null;
        selectedPlayer = null;
	}

    public abstract void PlayTurn();

    public virtual void Die() {
		CmdRemovePlayer (gameObject);
        GetComponent<Player>().death = true;
    }

    [Command]
    public void CmdSendMsgLover(GameObject l1, GameObject l2)
    {
        l2.GetComponent<Player>().RpcAddMsg("Vous avez un amoureux " + l1.GetComponent<Player>().pseudo);
        l1.GetComponent<Player>().RpcAddMsg("Vous avez un amoureux " + l2.GetComponent<Player>().pseudo);

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

    public GameObject GetSelectedPlayer() {
        return selectedPlayer;
    }

    public void SetSelectedPlayer(GameObject g) {
        if(GetComponent<Cupidon>()) {
            if (selectedPlayer != null && selectedPlayer != g)
            { 
                Debug.Log(g.GetComponent<Player>().pseudo + " est amoureux ");
                GetComponent<Cupidon>().SetSecondLover(g);
            }       
            else if (selectedPlayer == null)
            {
                 Debug.Log(g.GetComponent<Player>().pseudo + " est amoureux ");
                            selectedPlayer = g;
            }                  
        }
        else
            selectedPlayer = g;
    }

    [Command]
    public void CmdMsg(string msg, bool pV) {
        GameManager.instance.MessageToPlayers(msg, pV);
    }
}
