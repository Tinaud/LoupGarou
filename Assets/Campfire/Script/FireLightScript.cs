using UnityEngine;

public class FireLightScript : MonoBehaviour
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
		
	public void ChangeColor(GameManager.TurnIssue issue)
    {
		switch(issue)
        {
            case GameManager.TurnIssue.NO_VICTIMS:
                fireLight.color = new Color(1, 135f / 255f, 43f / 255f, 1);
                break;

            case GameManager.TurnIssue.VICTIMS:
                fireLight.color = new Color(1, 70f / 255f, 20f / 255f, 1);
                break;

            case GameManager.TurnIssue.WITCH:
				fireLight.color = new Color(150f / 255f, 180f / 255f, 0, 1);
                break;

			case GameManager.TurnIssue.DEAD:
				fireLight.color = new Color(1, 225f / 255f, 225f / 255f, 1);
				break;
        }
    }
}