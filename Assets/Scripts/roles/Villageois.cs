using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villageois : BaseRole {

    public override void Start() {
        base.Start();
    }

    public override void PlayTurn() {
		GetComponent<Player> ().cursor.color = Color.white;
    }

	public override string ToString ()
	{
		return string.Format ("[Villageois]");
	}
}
