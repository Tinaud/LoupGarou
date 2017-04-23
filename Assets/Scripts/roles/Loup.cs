using UnityEngine;

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
        GetComponent<Player>().CmdVictims(selectedPlayer);

        ready = true;
    }
}
