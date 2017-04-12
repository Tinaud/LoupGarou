using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRole : MonoBehaviour {

	public virtual void Start () {
        StartCoroutine(GetComponent<Player>().Vote());
	}

    public virtual void PlayTurn() {

    }
}
