using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour {

	public float angle = 0, lastangle = 180;
	public bool switchCycle = false;

	void Update () {

		if (switchCycle)
		{
			transform.RotateAround (new Vector3 (5f, 0, 13f), Vector3.right, 10f * Time.deltaTime); 
			transform.LookAt (new Vector3 (5f, 0, 13f));

			angle += 10f * Time.deltaTime;

			if (angle >= lastangle) {
				switchCycle = false;
				angle = 0;
			}
		}
	}
}
