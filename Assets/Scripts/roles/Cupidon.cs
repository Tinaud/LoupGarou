using UnityEngine;
using System.Collections;

public class Cupidon : BaseRole {

    GameObject secondSelected;

    public override void Start() {
        secondSelected = null;
        base.Start();
    }

    public override void PlayTurn() {
		GetComponent<Player> ().cursor.color = Color.white;
        ready = false;

        players = GameManager.instance.GetPlayers();
        StartCoroutine(WaitForChoice());
    }

    public override string ToString () {
		return string.Format ("[Cupidon]");
	}

    public void SetSecondLover(GameObject o) {
        secondSelected = o;
    }

    IEnumerator WaitForChoice() {
        yield return new WaitWhile(() => selectedPlayer == null || secondSelected == null);

        selectedPlayer.GetComponent<BaseRole>().SetLover(secondSelected);
        secondSelected.GetComponent<BaseRole>().SetLover(selectedPlayer);

        ready = true;
    }
}
