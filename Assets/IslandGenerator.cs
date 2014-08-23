using UnityEngine;
using System.Collections;

public class IslandGenerator : MonoBehaviour {

	public Renderer target;
	[Range(1, 515)]
	public int textureSize;

	public Vector2 noiseCenter;
	public float noiseSize;
	[Range(0, 10)]
	public float falloffExponent;

	[Range(0, 1)]
	public float threshold;
	public Color waterColor;
	public Color earthColor;

	public bool debug;

	[System.NonSerialized]
	private Color[] islandPixels;
	[System.NonSerialized]
	private Texture2D islandTexture;

	void Update () {
		if (islandTexture == null || islandTexture.width != textureSize) {
			islandPixels = new Color[textureSize * textureSize];
			islandTexture = new Texture2D(textureSize, textureSize);
			islandTexture.hideFlags = HideFlags.DontSave;
			islandTexture.filterMode = FilterMode.Point;
			if (target != null)
				target.material.mainTexture = islandTexture;
		}
		var spacing = noiseSize / textureSize;
		var offset = (spacing - noiseSize) * 0.5f;
		var center = textureSize * 0.5f;
		var radius = textureSize * Mathf.Sqrt(0.5f);
		for (var column = 0; column < textureSize; ++column) {
			for (var row = 0; row < textureSize; ++row) {
				var x = noiseCenter.x + offset + row * spacing;
				var y = noiseCenter.y + offset + column * spacing;
				var distanceToCenter =
					Vector2.Distance(
						new Vector2(row, column),
						new Vector2(center, center))
					/ radius;
				var sample = Mathf.PerlinNoise(x, y) * (1.0f - Mathf.Pow(distanceToCenter, falloffExponent));
				var color = sample > threshold ? earthColor : waterColor;
				islandPixels[row * textureSize + column] = debug ? new Color(sample, sample, sample) : color;
			}
		}
		islandTexture.SetPixels(islandPixels);
		islandTexture.Apply();
	}
}
