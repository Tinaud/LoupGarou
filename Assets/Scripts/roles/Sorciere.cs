using UnityEngine;
using System.Collections;

public class Sorciere : BaseRole {

    bool lifePotion;
    bool victimPotion;

    public override void Start() {
        lifePotion = true;
        victimPotion = true;

        base.Start();
    }

    public override void PlayTurn() {
		GetComponent<Player> ().cursor.color = Color.white;
        ready = false;
        selectedPlayer = null;

        if(lifePotion || victimPotion)
            StartCoroutine(WaitForChoice());
        else {
            CmdMsg("La sorcière n'a plus de potions :(", true);
            ready = true;
        }       
    }

	public override string ToString () {
		return string.Format ("[Sorcière]");
	}

    IEnumerator WaitForChoice() {
        yield return new WaitWhile(() => selectedPlayer == null);

        if (lifePotion && selectedPlayer == GameManager.instance.GetVictim()) {
            GameManager.instance.SaveVictim();
            lifePotion = false;
        }
        else if (victimPotion)
            GameManager.instance.AddVictim(selectedPlayer);
       
        ready = true;
    }
}
