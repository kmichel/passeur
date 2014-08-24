using UnityEngine;

public class Grass {

	public int row;
	public int column;
	public float age;
	public Grass(int row, int column, float age) {
		this.row = row;
		this.column = column;
		this.age = age;
	}

	public void Update(Island island, float grassGrowProbability) {
		age += Time.deltaTime;
		if (island.RandFloat(0f, 1f) < grassGrowProbability) {
			var direction = island.RandInt(0, 4);
			switch (direction) {
			case 0:
				if (island.IsWalkableAndPlantable(row, column - 1))
					island.CreateGrass(row, column - 1);
				break;
			case 1:
				if (island.IsWalkableAndPlantable(row, column + 1))
					island.CreateGrass(row, column + 1);
				break;
			case 2:
				if (island.IsWalkableAndPlantable(row + 1, column))
					island.CreateGrass(row + 1, column);
				break;
			case 3:
				if (island.IsWalkableAndPlantable(row - 1, column))
					island.CreateGrass(row - 1, column);
				break;
			}
		}
	}
}
