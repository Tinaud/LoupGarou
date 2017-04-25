using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyManager : MonoBehaviour {

	[SerializeField]
	List<Sun> dayNight = new List<Sun>(2);

	[SerializeField]
	FireLightScript fireCamp = null;

	public bool bMenu = false;

	public bool bDead = false;

	public FireLightScript FireCamp {
		get { return fireCamp; }
	}

	bool isDay = true;

	public IEnumerator SwitchTime() 
	{
		if (IsReady ()) 
		{
			foreach (Sun _s in dayNight)
				_s.switchCycle = true;

			if (bDead)
				yield return 1;
			
			yield return new WaitUntil (() => IsFire());
			{
				isDay = !isDay;

				if (!isDay) {
					fireCamp.run = false;
					if (!bMenu)
						fireCamp.ChangeColor (GameManager.TurnIssue.NO_TURN);
					else
						fireCamp.MenuFire ();
				} else {
					fireCamp.run = true;
					fireCamp.ChangeColor (GameManager.TurnIssue.VOTE);
				}
			}

			yield return new WaitUntil (() => IsReady());
			{
				if (!isDay) {
					RenderSettings.ambientIntensity = 0f;
				} else {
					RenderSettings.ambientIntensity = 0.5f;
				}
			}
		}
			
	}

	bool IsFire()
	{
		if (!isDay)
			return !(dayNight [0].fire) && !(dayNight [0].fire);
		else
			return dayNight [0].fire && dayNight [1].fire;
	}

	public bool IsReady()
	{
		return (!dayNight [0].switchCycle) && (!dayNight [1].switchCycle);
	}

}
