using UnityEngine;
using System.Collections.Generic;

public class Islands : MonoBehaviour {

	public int islandsSize;

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
	public GameObject waterPrototype;
	public GameObject groundPrototype;
	public GameObject ship;

	public GameObject waterPool;
	public GameObject groundPool;
	public GameObject shipPool;

	public string frontSeed;
	public int shipRow;
	public int shipColumn;
	public Direction shipDirection;

	[System.NonSerialized]
	private Dictionary<string, Island> islands;

	[System.NonSerialized]
	private bool lockRotator;

	public Islands() {
		frontSeed = "";
		islands = new Dictionary<string, Island>();
		shipRow = 3;
		shipColumn = 3;
		shipDirection = Direction.Left;
	}

	void Start() {
		FinishNavigation();
	}

	public void FinishNavigation() {
		lockRotator = false;

		ShowIslandOnGrid(GetIsland(frontSeed), frontLayout);
		ShowIslandOnGrid(GetIsland(GetNeighborSeed(frontSeed, "l", "r")), leftLayout);
		ShowIslandOnGrid(GetIsland(GetNeighborSeed(frontSeed, "r", "l")), rightLayout);
		ShowIslandOnGrid(GetIsland(GetNeighborSeed(frontSeed, "t", "b")), topLayout);
		ShowIslandOnGrid(GetIsland(GetNeighborSeed(frontSeed, "b", "t")), bottomLayout);

		frontLayout.AddItem(shipRow, shipColumn, ship, shipPool);
		UpdateRotationHint();
		worldRotator.JumpToTarget();
	}

	void Update () {
		if (!lockRotator) {
			if (Input.GetKeyDown("left")) {
				shipColumn -= 1;
				shipDirection = Direction.Left;
			}
			if (Input.GetKeyDown("right")) {
				shipColumn += 1;
				shipDirection = Direction.Right;
			}
			if (Input.GetKeyDown("up")) {
				shipRow += 1;
				shipDirection = Direction.Top;
			}
			if (Input.GetKeyDown("down")) {
				shipRow -= 1;
				shipDirection = Direction.Bottom;
			}
		}

		if (shipColumn == -1) {
			shipColumn = islandsSize - 1;
			UpdateSeed("l", "r");
			lockRotator = true;
			leftLayout.AddItem(shipRow, shipColumn, ship, shipPool);
			worldRotator.Navigate(Direction.Left, FinishNavigation);
		} else if (shipColumn == islandsSize) {
			shipColumn = 0;
			UpdateSeed("r", "l");
			lockRotator = true;
			rightLayout.AddItem(shipRow, shipColumn, ship, shipPool);
			worldRotator.Navigate(Direction.Right, FinishNavigation);
		} else if (shipRow == -1) {
			shipRow = islandsSize - 1;
			UpdateSeed("b", "t");
			lockRotator = true;
			bottomLayout.AddItem(shipRow, shipColumn, ship, shipPool);
			worldRotator.Navigate(Direction.Bottom, FinishNavigation);
		} else if (shipRow == islandsSize) {
			shipRow = 0;
			UpdateSeed("t", "b");
			lockRotator = true;
			topLayout.AddItem(shipRow, shipColumn, ship, shipPool);
			worldRotator.Navigate(Direction.Top, FinishNavigation);
		} else {
			frontLayout.MoveItem(shipRow, shipColumn, ship);
		}

		if (!lockRotator)
			UpdateRotationHint();
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
		for (var row = 0; row < islandsSize; ++row) {
			for (var column = 0; column < islandsSize; ++column) {
				var cellType = island.cells[row * islandsSize + column];
				var pool = cellType == CellType.Water ? waterPool : groundPool;
				if (pool.transform.childCount > 0) {
					var instance = pool.transform.GetChild(0).gameObject;
					gridLayout.AddItem(row, column, instance, pool);
				} else {
					var prototype = cellType == CellType.Water ? waterPrototype : groundPrototype;
					var instance = Object.Instantiate(prototype) as GameObject;
					instance.SetActive(true);
					instance.transform.localRotation = Quaternion.identity;
					gridLayout.AddItem(row, column, instance, pool);
				}
			}
		}
	}

	public Island GetIsland(string seed) {
		Island island;
		if (!islands.TryGetValue(seed, out island)) {
			island = CreateIsland(seed);
			islands[seed] = island;
		}
		return island;
	}

	public Island CreateIsland(string seed) {
		return new Island(
			seed,
			Island.Generate(seed, islandsSize, seedBase, noiseSize, falloffExponent, threshold));
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
	
}
