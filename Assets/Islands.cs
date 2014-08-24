using UnityEngine;
using System.Collections.Generic;

public class Islands : MonoBehaviour {

	public int islandsSize;

	public int initialHumanCount;
	public int minInitialSheepCount;
	public int maxInitialSheepCount;
	[Range(0, 1)]
	public float movementProbability;

	public int seedBase;
	public float noiseSize;
	[Range(0, 10)]
	public float falloffExponent;
	[Range(0, 1)]
	public float threshold;

	public GridLayout frontLayout;
	public GridLayout leftLayout;
	public GridLayout rightLayout;
	public GridLayout topLayout;
	public GridLayout bottomLayout;

	public WorldRotator worldRotator;
	public GameObject groundPrototype;
	public GameObject sheepPrototype;
	public GameObject humanPrototype;
	public GameObject harborPrototype;
	public GameObject ship;

	public GameObject groundPool;
	public GameObject sheepPool;
	public GameObject humanPool;
	public GameObject harborPool;
	public GameObject shipPool;

	public string frontSeed;
	public int shipRow;
	public int shipColumn;
	public Direction shipDirection;

	[System.NonSerialized]
	private Dictionary<string, Island> islands;

	[System.NonSerialized]
	private bool lockRotator;

	private Island frontIsland;

	public Islands() {
		frontSeed = "";
		islands = new Dictionary<string, Island>();
	}

	void Start() {
		frontIsland = GetIsland(frontSeed, isInitialIsland:true);
		FinishNavigation();
	}

	public void FinishNavigation() {
		lockRotator = false;

		frontIsland = GetIsland(frontSeed);
		ShowIslandOnGrid(frontIsland, frontLayout);
		ShowIslandOnGrid(GetIsland(GetNeighborSeed(frontSeed, "l", "r")), leftLayout);
		ShowIslandOnGrid(GetIsland(GetNeighborSeed(frontSeed, "r", "l")), rightLayout);
		ShowIslandOnGrid(GetIsland(GetNeighborSeed(frontSeed, "t", "b")), topLayout);
		ShowIslandOnGrid(GetIsland(GetNeighborSeed(frontSeed, "b", "t")), bottomLayout);

		frontLayout.AddItem(shipRow, shipColumn, DirectionToAngle(shipDirection), ship, shipPool);
		UpdateRotationHint();
		worldRotator.JumpToTarget();
	}

	void Update () {
		if (!lockRotator) {
			// TODO: cell accessor and bounds checking
			if (Input.GetKeyDown("left")) {
				shipDirection = Direction.Left;
				if (frontIsland.IsNavigable(shipRow, shipColumn - 1))
					shipColumn -= 1;
			}
			if (Input.GetKeyDown("right")) {
				shipDirection = Direction.Right;
				if (frontIsland.IsNavigable(shipRow, shipColumn + 1))
					shipColumn += 1;
			}
			if (Input.GetKeyDown("up")) {
				shipDirection = Direction.Top;
				if (frontIsland.IsNavigable(shipRow + 1, shipColumn))
					shipRow += 1;
			}
			if (Input.GetKeyDown("down")) {
				shipDirection = Direction.Bottom;
				if (frontIsland.IsNavigable(shipRow - 1, shipColumn))
					shipRow -= 1;
			}
		}

		if (shipColumn == -1) {
			shipColumn = islandsSize - 1;
			UpdateSeed("l", "r");
			lockRotator = true;
			leftLayout.AddItem(shipRow, shipColumn, DirectionToAngle(shipDirection), ship, shipPool);
			worldRotator.Navigate(Direction.Left, FinishNavigation);
		} else if (shipColumn == islandsSize) {
			shipColumn = 0;
			UpdateSeed("r", "l");
			lockRotator = true;
			rightLayout.AddItem(shipRow, shipColumn, DirectionToAngle(shipDirection), ship, shipPool);
			worldRotator.Navigate(Direction.Right, FinishNavigation);
		} else if (shipRow == -1) {
			shipRow = islandsSize - 1;
			UpdateSeed("b", "t");
			lockRotator = true;
			bottomLayout.AddItem(shipRow, shipColumn, DirectionToAngle(shipDirection), ship, shipPool);
			worldRotator.Navigate(Direction.Bottom, FinishNavigation);
		} else if (shipRow == islandsSize) {
			shipRow = 0;
			UpdateSeed("t", "b");
			lockRotator = true;
			topLayout.AddItem(shipRow, shipColumn, DirectionToAngle(shipDirection), ship, shipPool);
			worldRotator.Navigate(Direction.Top, FinishNavigation);
		} else {
			frontLayout.MoveItem(shipRow, shipColumn, DirectionToAngle(shipDirection), ship);
		}

		if (!lockRotator)
			UpdateRotationHint();

		foreach (var island in islands.Values)
			island.Update(movementProbability);
	}

	public void UpdateRotationHint() {
		worldRotator.Center();

		if (shipDirection == Direction.Left || shipDirection == Direction.Right) {
			if (shipColumn == 0)
				worldRotator.Hint(Direction.Left);
			else if (shipColumn == islandsSize - 1)
				worldRotator.Hint(Direction.Right);
		}

		if (shipDirection == Direction.Top || shipDirection == Direction.Bottom) {
			if (shipRow == 0)
				worldRotator.Hint(Direction.Bottom);
			else if (shipRow == islandsSize - 1)
				worldRotator.Hint(Direction.Top);
		}
	}

	public void ShowIslandOnGrid(Island island, GridLayout gridLayout) {
		gridLayout.size = islandsSize;
		gridLayout.Clear();
		// TODO: deduplicate pooling code
		for (var row = 0; row < islandsSize; ++row) {
			for (var column = 0; column < islandsSize; ++column) {
				var index = row * islandsSize + column;
				var cellType = island.cells[index];
				if (cellType == CellType.Ground) {
					GameObject instance;
					if (groundPool.transform.childCount > 0) {
						instance = groundPool.transform.GetChild(0).gameObject;
					} else {
						instance = Object.Instantiate(groundPrototype) as GameObject;
					}
					gridLayout.AddItem(row, column, 0, instance, groundPool);
				}
			}
		}
		foreach (var animal in island.animals) {
			var pool = animal.type == AnimalType.Sheep ? sheepPool : humanPool;
			GameObject instance;
			if (pool.transform.childCount > 0) {
				instance = pool.transform.GetChild(0).gameObject;
			} else {
				var prototype = animal.type == AnimalType.Sheep ? sheepPrototype : humanPrototype;
				instance = Object.Instantiate(prototype) as GameObject;
			}
			var animalView = instance.GetComponent<AnimalView>();
			animalView.gridLayout = gridLayout;
			animalView.animal = animal;
			gridLayout.AddItem(animal.row, animal.column, 0, instance, pool);
		}
		foreach (var harbor in island.harbors) {
			GameObject instance;
			if (harborPool.transform.childCount > 0) {
				instance = harborPool.transform.GetChild(0).gameObject;
			} else {
				instance = Object.Instantiate(harborPrototype) as GameObject;
			}
			gridLayout.AddItem(harbor.row, harbor.column, 0, instance, harborPool);
		}
	}

	public Island GetIsland(string seed, bool isInitialIsland=false) {
		Island island;
		if (!islands.TryGetValue(seed, out island)) {
			island = CreateIsland(seed);
			if (isInitialIsland) {
				var largestArea = island.GetLargestGroundArea();
				island.CreateHarbor(largestArea.startRow, largestArea.startColumn);
				// The fact that this cell is always water relies on the scan order of
				// the algorithm used for area detection and on the all-water border
				shipRow = largestArea.startRow;
				shipColumn = largestArea.startColumn - 1;
				shipDirection = Direction.Bottom;
				island.CreateAnimals(AnimalType.Human, initialHumanCount);
			}
			island.CreateAnimals(AnimalType.Sheep, island.RandInt(minInitialSheepCount, maxInitialSheepCount));
			islands[seed] = island;
		}
		return island;
	}

	public Island CreateIsland(string seed) {
		return new Island(
			islandsSize,
			seed,
			Island.GenerateGround(seed, islandsSize, seedBase, noiseSize, falloffExponent, threshold));
	}

	public void UpdateSeed(string suffix, string oppositeSuffix) {
		frontSeed = GetNeighborSeed(frontSeed, suffix, oppositeSuffix);
	}

	public string GetNeighborSeed(string seed, string suffix, string oppositeSuffix) {
		if (frontSeed.EndsWith(oppositeSuffix))
			return seed.Substring(0, frontSeed.Length - oppositeSuffix.Length);
		else
			return seed + suffix;
	}

	public float DirectionToAngle(Direction direction) {
		switch (direction) {
		default:
		case Direction.Left:
			return 0f;
		case Direction.Right:
			return 180f;
		case Direction.Top:
			return 270f;
		case Direction.Bottom:
			return 90f;
		}
	}
	
}
