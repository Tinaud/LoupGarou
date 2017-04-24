using UnityEngine;

public class FireLightScript : MonoBehaviour
{
	public float minIntensity = 0.25f;
	public float maxIntensity = 0.5f;

	public Light fireLight;
	public ParticleSystem flameParticles;

	ParticleSystem.MainModule settings;

	Color flameColor, witchFlame, victimFlame;

	float random;

	void Start() {
		settings = flameParticles.main;
		flameColor = new Color(62f / 255f, 62f / 255f, 62f / 255f, 1);
		witchFlame = new Color(62f / 255f, 255f / 255f, 62f / 255f, 1);
		victimFlame = new Color(124f / 255f, 62f / 255f, 62f / 255f, 1);
	}

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
			case GameManager.TurnIssue.NO_TURN:
				fireLight.range = 5f;
				goto case GameManager.TurnIssue.NO_VICTIMS;

			case GameManager.TurnIssue.TURN:
				goto case GameManager.TurnIssue.NO_VICTIMS;
			
            case GameManager.TurnIssue.NO_VICTIMS:
                fireLight.color = new Color(1, 135f / 255f, 43f / 255f, 1);
				fireLight.range = 30f;
				settings.startColor = new ParticleSystem.MinMaxGradient (flameColor);
                break;

            case GameManager.TurnIssue.VICTIMS:
                fireLight.color = new Color(1, 80f / 255f, 25f / 255f, 1);
				fireLight.range = 30f;
				settings.startColor = new ParticleSystem.MinMaxGradient (victimFlame);
                break;

            case GameManager.TurnIssue.WITCH:
				fireLight.color = new Color(125f / 255f, 125f / 255f, 60f / 255f, 1);
				fireLight.range = 30f;
				settings.startColor = new ParticleSystem.MinMaxGradient (witchFlame);
                break;

			case GameManager.TurnIssue.DEAD:
				fireLight.color = new Color (100f / 255f, 100 / 255f, 100f / 255f, 1);
				fireLight.range = 30f;
				settings.startColor = new ParticleSystem.MinMaxGradient (witchFlame);
				break;
        }
    }
}