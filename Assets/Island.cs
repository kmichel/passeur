using UnityEngine;

public class Island {

	public string seed;
	public CellType[] cells;

	public Island(string seed, CellType[] cells) {
		this.seed = seed;
		this.cells = cells;
	}

	public bool IsNavigable(int row, int column, int size) {
		return row < 0 || row >= size || column < 0 || column >= size
			|| cells[row * size + column] == CellType.Water;
	}

	public static CellType[] Generate(string seed, int islandSize, int seedBase, float noiseSize, float falloffExponent, float threshold) {
		var cells = new CellType[islandSize * islandSize];
		var seedHash = seedBase + seed.GetHashCode();
		// These weird nubers are 'random' prime numbers
		var seedX = seedHash % 65521;
		var seedY = (seedHash * 45949) % 31147;
		var spacing = noiseSize / islandSize;
		var offset = (spacing - noiseSize) * 0.5f;
		var center = islandSize * 0.5f;
		var radius = islandSize * Mathf.Sqrt(0.5f);
		for (var row = 0; row < islandSize; ++row) {
			for (var column = 0; column < islandSize; ++column) {
				if (row == 0 || column == 0 || row == islandSize - 1 || column == islandSize - 1) {
					cells[row * islandSize + column] = CellType.Water;
				} else {
					var x = seedX + offset + row * spacing;
					var y = seedY + offset + column * spacing;
					var distanceToCenter =
						Vector2.Distance(
							new Vector2(row, column),
							new Vector2(center, center))
							/ radius;
					var sample = Mathf.PerlinNoise(x, y) * (1.0f - Mathf.Pow(distanceToCenter, falloffExponent));
					cells[row * islandSize + column] = sample > threshold ? CellType.Ground : CellType.Water;
				}
			}
		}
		return cells;
	}

}
