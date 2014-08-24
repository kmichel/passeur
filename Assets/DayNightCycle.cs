using UnityEngine;

public class DayNightCycle : MonoBehaviour {

	public float dayLengthInSeconds;
	[Range(0, 1)]
	public float dayPosition;

	public Camera targetCamera;
	public Gradient cameraGradient;

	public Light targetLight;
	public Gradient lightGradient;

	public ParticleSystem stars;

	void Update () {
		dayPosition += Time.deltaTime / dayLengthInSeconds;
		if (dayPosition > 1.0f)
			dayPosition -= 1.0f;
		var gradientPosition = dayPosition > 0.5f ? 2 - dayPosition * 2 : dayPosition * 2;
		targetCamera.backgroundColor = cameraGradient.Evaluate(gradientPosition);
		targetLight.color = lightGradient.Evaluate(gradientPosition);
		stars.renderer.material.color = new Color(1, 1, 1, Mathf.Pow(gradientPosition, 3));
	}
}
