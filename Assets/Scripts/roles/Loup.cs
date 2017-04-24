using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Loup : BaseRole {

    public override void Start() {
        base.Start();
    }

    public override void PlayTurn() {
        ready = false;
        selectedPlayer = null;

        //StartCoroutine(WaitForChoice());
        ready = true;
    }

	public override string ToString () {
		return string.Format ("[Loup]");
	}

    /*IEnumerator WaitForChoice() {
        while (selectedPlayer == null) {
            selectedPlayer = GameManager.instance.GetMostVote();
            yield return new WaitForSeconds(0.5f);
        }



        ready = true;
    }*/
}
