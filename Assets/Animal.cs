using UnityEngine;
using System.Collections;

public class Animal {

	public AnimalType type;

	public int row;
	public int column;

	public Animal(AnimalType type, int row, int column) {
		this.type = type;
		this.row = row;
		this.column = column;
	}

	public void Update(Island island, float movementProbability) {
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
}
