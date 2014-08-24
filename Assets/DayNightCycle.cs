using UnityEngine;

public class DayNightCycle : MonoBehaviour {

	public float dayLengthInSeconds;
	[Range(0, 1)]
	public float dayPosition;

	public Camera targetCamera;
	public Gradient cameraGradient;

	public Light targetLight;
	public Gradient lightGradient;

	public Transform sunGlow;
	public Gradient sunGlowGradient;
	public Vector2 sunGlowEllipsis;

	public Transform shadows;
	public Light shadowsLight;

	public ParticleSystem stars;

	void Update () {
		dayPosition += Time.deltaTime / dayLengthInSeconds;
		if (dayPosition > 1.0f)
			dayPosition -= 1.0f;
		var gradientPosition = dayPosition > 0.5f ? 2 - dayPosition * 2 : dayPosition * 2;
		targetCamera.backgroundColor = cameraGradient.Evaluate(gradientPosition);
		targetLight.color = lightGradient.Evaluate(gradientPosition);
		shadowsLight.color = lightGradient.Evaluate(gradientPosition);
		sunGlow.renderer.material.SetColor("_TintColor", sunGlowGradient.Evaluate(gradientPosition));
		sunGlow.transform.localPosition = new Vector3(
			Mathf.Sin(2*Mathf.PI * dayPosition) * sunGlowEllipsis.x,
			Mathf.Cos(2*Mathf.PI * dayPosition) * sunGlowEllipsis.y,
			2);
		shadows.transform.localRotation = Quaternion.AngleAxis(dayPosition * 360, Vector3.back);
		shadowsLight.shadowStrength = 1 - gradientPosition;
		stars.renderer.material.color = new Color(1, 1, 1, Mathf.Pow(gradientPosition, 3));
	}
}
