using UnityEngine;
public class Chasseur : BaseRole {

    public override void Start() {
        base.Start();
    }

    public override void PlayTurn() {
        
    }

    public override void Die() {
        if (selectedPlayer != null) {
            Debug.Log("Chasseur a emmené quelqu'un dans la mort.");
            selectedPlayer.GetComponent<BaseRole>().Die();
        }

        base.Die();
    }
}
