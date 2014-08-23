using UnityEngine;
using System.Collections.Generic;

public class Island {

	public int size;
	public string seed;
	public CellType[] cells;
	public List<Animal> animals;
	public List<Harbor> harbors;

	private int randomSeed;

	public Island(int size, string seed, CellType[] cells) {
		this.size = size;
		this.seed = seed;
		this.cells = cells;
		this.animals = new List<Animal>();
		this.harbors = new List<Harbor>();
		this.randomSeed = seed.GetHashCode();
	}

	public int RandInt(int min, int max) {
		Random.seed = randomSeed;
		var value = Random.Range(min, max);
		randomSeed = Random.Range(int.MinValue, int.MaxValue);
		return value;
	}

	public float RandFloat(float min, float max) {
		Random.seed = randomSeed;
		var value = Random.Range(min, max);
		randomSeed = Random.Range(int.MinValue, int.MaxValue);
		return value;
	}

	public void CreateAnimals(AnimalType type, int count) {
		for (var animalCount = 0; animalCount < count;)
			if (TryCreateAnimal(type))
				++animalCount;
	}

	public bool TryCreateAnimal(AnimalType type) {
		var row = RandInt(0, size);
		var column = RandInt(0, size);
		if (IsWalkableAndAvailable(row, column)) {
			this.animals.Add(new Animal(type, row, column));
			return true;
		}
		return false;
	}

	public void CreateHarbor(int row, int column) {
		this.harbors.Add(new Harbor(row, column));
	}

	public void Update(float movementProbability) {
		for (int index = 0; index < animals.Count; ++index)
			animals[index].Update(this, movementProbability);
	}

	public bool IsNavigable(int row, int column) {
		return row < 0 || row >= size || column < 0 || column >= size
			|| cells[row * size + column] == CellType.Water;
	}

	public bool IsWalkable(int row, int column) {
		return row >= 0 && row < size && column >= 0 && column < size
			&& cells[row * size + column] == CellType.Ground;
	}

	public bool IsAvailable(int row, int column) {
		for (var index = 0; index < animals.Count; ++index) {
			var animal = animals[index];
			if (animal.row == row && animal.column == column)
				return false;
		}
		for (var index = 0; index < harbors.Count; ++index) {
			var harbor = harbors[index];
			if (harbor.row == row && harbor.column == column)
				return false;
		}
		return true;
	}

	public bool IsWalkableAndAvailable(int row, int column) {
		return IsWalkable(row, column) && IsAvailable(row, column);
	}

	public static CellType[] GenerateGround(string seed, int islandSize, int seedBase, float noiseSize, float falloffExponent, float threshold) {
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
