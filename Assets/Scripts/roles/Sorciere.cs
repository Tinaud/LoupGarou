using UnityEngine;

public class Sorciere : BaseRole {

    bool lifePotion;
    bool VICTIMSPotion;

    public override void Start() {
        lifePotion = true;
        VICTIMSPotion = true;

        base.Start();
    }

    public override void PlayTurn() {
        ready = false;



        if(lifePotion) {

        }

        if(VICTIMSPotion) {

        }

        ready = true;
    }

	public override string ToString ()
	{
		return string.Format ("[Sorcière]");
	}
}
