using UnityEngine;
using System.Collections;

public class Chasseur : BaseRole {

    public override void Start() {
        base.Start();
    }

    public override void PlayTurn() {
		GetComponent<Player> ().cursor.color = Color.white;
        ready = false;
        selectedPlayer = null;

        StartCoroutine(WaitForChoice());
    }

	public override string ToString () {
		return string.Format ("[Chasseur]");
	}

    IEnumerator WaitForChoice() {
        yield return new WaitWhile(() => selectedPlayer == null);

        ready = true;
    }
}
