using UnityEngine;
using System.Collections;

public class Voyante : BaseRole {

	public override void Start () {
        base.Start();
	}

    public override void PlayTurn() {
        ready = false;
        selectedPlayer = null;

        StartCoroutine(WaitForChoice());
    }

    public override string ToString () {
		return string.Format ("[Voyante]");
	}

    IEnumerator WaitForChoice() {
        yield return new WaitWhile(() => selectedPlayer == null);

        CmdMsg("MJ : La voyante a sondé : " + selectedPlayer.GetComponent<BaseRole>().ToString(), true);

        ready = true;
    }
}
