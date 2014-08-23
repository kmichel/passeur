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
		if (Random.Range(0f, 1f) < movementProbability) {
			var direction = GetRandomDirection();
			switch (direction) {
			case Direction.Left:
				if (island.IsWalkable(row, column - 1))
					column -= 1;
				break;
			case Direction.Right:
				if (island.IsWalkable(row, column + 1))
					column += 1;
				break;
			case Direction.Top:
				if (island.IsWalkable(row + 1, column))
					row += 1;
				break;
			case Direction.Bottom:
				if (island.IsWalkable(row - 1, column))
					row -= 1;
				break;
			}
		}
	}

	public static Direction GetRandomDirection() {
		switch (Random.Range(0, 4)) {
		default:
		case 0:
			return Direction.Left;
		case 1:
			return Direction.Right;
		case 2:
			return Direction.Top;
		case 3:
			return Direction.Bottom;
		}
	}
}
