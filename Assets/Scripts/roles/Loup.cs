using UnityEngine;

public class Loup : BaseRole {

    public override void Start() {
        base.Start();
    }

    public override void PlayTurn() {
        ready = false;
        int rand;
        do
            rand = Random.Range(0, players.Count);
        while (players[rand].GetComponent<Loup>());

        selectedPlayer = players[rand];

        Debug.Log("[LOUP] Le joueur mangé est " + selectedPlayer.GetComponent<BaseRole>().GetType() + " (id : " + selectedPlayer.GetComponent<Player>().ID() + ")");

        selectedPlayer.GetComponent<BaseRole>().Die();

        ready = true;
    }
}
