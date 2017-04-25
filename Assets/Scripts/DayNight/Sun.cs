using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour {

	float angle = 0, maxangle = 180;
	public bool switchCycle = false;
	public bool fire = false;

	bool once = false;

	void Update () {

		if (switchCycle)
		{
			transform.RotateAround (new Vector3 (5f, 0, 13f), Vector3.right, 20f * Time.deltaTime); 
			transform.LookAt (new Vector3 (5f, 0, 13f));

			angle += 20f * Time.deltaTime;

			if (angle >= maxangle / 2f && !once) 
			{
				fire = !fire;
				once = true;
			}

			if (angle >= maxangle) {
				switchCycle = false;
				angle = 0;
				once = false;
			}
		}
	}
}
