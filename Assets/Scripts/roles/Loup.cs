using UnityEngine;
using UnityEngine.Networking;


public class Loup : BaseRole {

    public override void Start() {
        base.Start();
    }

    public override void PlayTurn() {
        Debug.Log("[LOUP] Les loups font leur choix.");

        ready = false;
        int rand;

        do
            rand = Random.Range(0, players.Count);
        while (players[rand].GetComponent<Loup>());

        selectedPlayer = players[rand];
		CmdAddVictim(selectedPlayer);

        ready = true;
    }

	[Command]
	void CmdAddVictim(GameObject _v) {
		GameManager.instance.AddVictim (_v);
	}

	public override string ToString ()
	{
		return string.Format ("[Loup]");
	}
}
