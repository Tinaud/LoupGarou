using UnityEngine;

public class Sorciere : BaseRole {

    bool lifePotion;
    bool deathPotion;

    public override void Start() {
        lifePotion = true;
        deathPotion = true;

        base.Start();
    }

    public override void PlayTurn() {
        ready = false;

        if(lifePotion) {

        }

        if(deathPotion) {

        }

        ready = true;
    }

	public override string ToString ()
	{
		return string.Format ("[Sorcière]");
	}
}
