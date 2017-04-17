using UnityEngine;

public class Voyante : BaseRole {

	public override void Start () {
        base.Start();
	}

    public override void PlayTurn() {
        ready = false;

        selectedPlayer = players[Random.Range(0, players.Count)];

        Debug.Log("[VOYANTE] Le joueur " + selectedPlayer.GetComponent<Player>().ID() + " est : " + selectedPlayer.GetComponent<BaseRole>().GetType());

        ready = true;
    }
}
