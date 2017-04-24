using UnityEngine;
using UnityEngine.Networking;

public class FireLightScript : NetworkBehaviour
{
	public float minIntensity = 0.25f;
	public float maxIntensity = 0.5f;

	public Light fireLight;

	float random;

	void Update()
	{
		random = Random.Range(0.0f, 150.0f);
		float noise = Mathf.PerlinNoise(random, Time.time);
		fireLight.GetComponent<Light>().intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
	}

    [ClientRpc]
    public void RpcChangeColor()
    {
        switch(GameManager.instance.turnIssue)
        {
            case GameManager.TurnIssue.NO_DEATH:
                fireLight.color = new Color(1, 135f / 255f, 43f / 255f, 1);
                break;

            case GameManager.TurnIssue.DEATH:
                fireLight.color = new Color(1, 20f / 255f, 0, 1);
                break;

            case GameManager.TurnIssue.WITCH:
                fireLight.color = new Color(80f / 255f, 1, 0, 1);
                break;
        }
    }
}