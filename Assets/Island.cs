using UnityEngine;
using System.Collections.Generic;

public class Island {

	public class Area {
		public int startRow;
		public int startColumn;
		public int number;
		public int size;

		public Area(int startRow, int startColumn, int number, int size) {
			this.startRow = startRow;
			this.startColumn = startColumn;
			this.number = number;
			this.size = size;
		}
	}

	public int size;
	public string seed;
	public CellType[] cells;

	public int[] areaNumbers;
	public List<Area> waterAreas;
	public List<Area> groundAreas;

	public List<Animal> animals;
	public List<Harbor> harbors;
	public List<Grass> grasses;

	private int randomSeed;

	public Island(int size, string seed, CellType[] cells) {
		this.size = size;
		this.seed = seed;
		this.cells = cells;
		areaNumbers = new int[cells.Length];
		waterAreas = new List<Area>();
		groundAreas = new List<Area>();
		ComputeAreas();
		this.animals = new List<Animal>();
		this.harbors = new List<Harbor>();
		this.grasses = new List<Grass>();
		randomSeed = seed.GetHashCode();
	}

	public Area GetLargestGroundArea() {
		Area largestArea = null;
		foreach (var area in groundAreas)
			if (largestArea == null || area.size > largestArea.size)
				largestArea = area;
		return largestArea;
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
			animals.Add(new Animal(type, row, column));
			return true;
		}
		return false;
	}

	public void CreateGrassPatches(int count) {
		for (var grassCount = 0; grassCount < count;)
			if (TryCreateGrassPatch(RandInt(0, size), RandInt(0, size), 2))
				++grassCount;
	}

	public bool TryCreateGrassPatch(int row, int column, int radius) {
		if (IsWalkableAndPlantable(row, column)) {
			grasses.Add(new Grass(row, column, RandFloat(0, 10)));
			if (radius > 0) {
				TryCreateGrassPatch(row - 1, column, radius - 1);
				TryCreateGrassPatch(row + 1, column, radius - 1);
				TryCreateGrassPatch(row, column - 1, radius - 1);
				TryCreateGrassPatch(row, column + 1, radius - 1);
			}
			return true;
		}
		return false;
	}

	public void CreateGrass(int row, int column) {
		grasses.Add(new Grass(row, column, 0));
	}

	public void CreateHarbor(int row, int column) {
		harbors.Add(new Harbor(row, column));
	}

	public void Update(float movementProbability, float eatGrassProbability, float grassGrowProbability) {
		for (int index = 0; index < animals.Count; ++index)
			animals[index].Update(this, movementProbability, eatGrassProbability);
		for (int index = 0; index < grasses.Count; ++index)
			grasses[index].Update(this, grassGrowProbability);
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

	public bool IsPlantable(int row, int column) {
		for (var index = 0; index < grasses.Count; ++index) {
			var grass = grasses[index];
			if (grass.row == row && grass.column == column)
				return false;
		}
		return true;
	}

	public bool IsWalkableAndAvailable(int row, int column) {
		return IsWalkable(row, column) && IsAvailable(row, column);
	}

	public bool IsWalkableAndPlantable(int row, int column) {
		return IsWalkable(row, column) && IsPlantable(row, column);
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

	public void ComputeAreas() {
		int row;
		int column;
		int areaNumber = 1;
		while (FindFirstNonNumberedCell(CellType.Ground, out row, out column)) {
			int areaSize = 0;
			FillArea(CellType.Ground, row, column, areaNumber, ref areaSize);
			groundAreas.Add(new Area(row, column, areaNumber, areaSize));
			++areaNumber;
		}
		areaNumber = -1;
		while (FindFirstNonNumberedCell(CellType.Water, out row, out column)) {
			int areaSize = 0;
			FillArea(CellType.Water, row, column, areaNumber, ref areaSize);
			waterAreas.Add(new Area(row, column, areaNumber, areaSize));
			--areaNumber;
		}
	}

	public bool FindFirstNonNumberedCell(CellType type, out int outRow, out int outColumn) {
		for (var row = 0; row < size; ++row) {
			for (var column = 0; column < size; ++column) {
				var index = row * size + column;
				if (cells[index] == type && areaNumbers[index] == 0) {
					outRow = row;
					outColumn = column;
					return true;
				}
			}
		}
		outRow = -1;
		outColumn = -1;
		return false;
	}

	public void FillArea(CellType type, int row, int column, int areaNumber, ref int areaSize) {
		var index = row * size + column;
		if (cells[index] == type && areaNumbers[index] == 0) {
			areaNumbers[index] = areaNumber;
			++areaSize;
			if (row > 0)
				FillArea(type, row - 1, column, areaNumber, ref areaSize);
			if (row < size - 1)
				FillArea(type, row + 1, column, areaNumber, ref areaSize);
			if (column > 0)
				FillArea(type, row, column - 1, areaNumber, ref areaSize);
			if (column < size - 1)
				FillArea(type, row, column + 1, areaNumber, ref areaSize);
		}
	}

}
