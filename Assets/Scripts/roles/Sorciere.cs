using UnityEngine;

public class Sorciere : BaseRole {

    public override void Start() {
        base.Start();
    }

    public override void PlayTurn() {
        ready = false;

        Debug.Log("Witch turn");

        ready = true;
    }
}
