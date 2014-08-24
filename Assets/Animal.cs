using UnityEngine;

public class Animal {

	public AnimalType type;

	public int row;
	public int column;

	public float age;

	public Animal(AnimalType type, int row, int column) {
		this.type = type;
		this.row = row;
		this.column = column;
		age = 0;
	}

	public void Update(Island island, float movementProbability, float eatGrassProbability) {
		age += Time.deltaTime;
		Move(island, movementProbability);
		if (type == AnimalType.Sheep)
			EatGrass(island, eatGrassProbability);
	}

	public void Move(Island island, float movementProbability) {
		if (island.RandFloat(0f, 1f) < movementProbability) {
			var direction = island.RandInt(0, 4);
			switch (direction) {
			case 0:
				if (island.IsWalkableAndAvailable(row, column - 1))
					column -= 1;
				break;
			case 1:
				if (island.IsWalkableAndAvailable(row, column + 1))
					column += 1;
				break;
			case 2:
				if (island.IsWalkableAndAvailable(row + 1, column))
					row += 1;
				break;
			case 3:
				if (island.IsWalkableAndAvailable(row - 1, column))
					row -= 1;
				break;
			}
		}
	}

	public void EatGrass(Island island, float eatGrassProbability) {
		if (island.RandFloat(0f, 1f) < eatGrassProbability) {
			foreach (var grass in island.grasses) {
				if (grass.row == row && grass.column == column) {
					island.grasses.Remove(grass);
					break;
				}
			}
		}
	}
}
