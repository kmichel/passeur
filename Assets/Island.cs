﻿using UnityEngine;
using System.Collections.Generic;

public class Island {

	public int size;
	public string seed;
	public CellType[] cells;
	public List<Animal> animals;

	public Island(int size, string seed, CellType[] cells) {
		this.size = size;
		this.seed = seed;
		this.cells = cells;
		this.animals = new List<Animal>();
		if (IsWalkable(size / 2, size / 2))
			this.animals.Add(new Animal(AnimalType.Sheep, size / 2, size / 2));
	}

	public void CreateAnimals(AnimalType type, int count) {
		for (var animalCount = 0; animalCount < count;)
			if (TryCreateAnimal(type))
				++animalCount;
	}

	public bool TryCreateAnimal(AnimalType type) {
		var row = Random.Range(0, size);
		var column = Random.Range(0, size);
		if (IsWalkable(row, column)) {
			this.animals.Add(new Animal(type, row, column));
			return true;
		}
		return false;
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
