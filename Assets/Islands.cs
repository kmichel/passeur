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

	public GridLayout gridLayout;
	public GameObject waterPrototype;
	public GameObject groundPrototype;
	public GameObject ship;

	public GameObject waterPool;
	public GameObject groundPool;
	public GameObject shipPool;

	public string currentSeed;
	[System.NonSerialized]
	private Island currentIsland;
	public int shipRow;
	public int shipColumn;

	[System.NonSerialized]
	private Dictionary<string, Island> islands;

	public Islands() {
		currentSeed = "";
		islands = new Dictionary<string, Island>();
		shipRow = 3;
		shipColumn = 3;
	}

	void Update () {
		if (Input.GetKeyDown("left"))
			shipColumn -= 1;
		if (Input.GetKeyDown("right"))
			shipColumn += 1;
		if (Input.GetKeyDown("up"))
			shipRow += 1;
		if (Input.GetKeyDown("down"))
			shipRow -= 1;

		if (shipColumn == -1) {
			shipColumn = islandsSize - 1;
			UpdateSeed("l", "r");
		} else if (shipColumn == islandsSize) {
			shipColumn = 0;
			UpdateSeed("r", "l");
		}

		if (shipRow == -1) {
			shipRow = islandsSize - 1;
			UpdateSeed("b", "t");
		} else if (shipRow == islandsSize) {
			shipRow = 0;
			UpdateSeed("t", "b");
		}

		var newCurrentIsland = GetIsland(currentSeed);
		if (newCurrentIsland != currentIsland) {
			currentIsland = newCurrentIsland;
			gridLayout.size = islandsSize;
			gridLayout.Clear();
			for (var row = 0; row < islandsSize; ++row) {
				for (var column = 0; column < islandsSize; ++column) {
					var cellType = currentIsland.cells[row * islandsSize + column];
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
			gridLayout.AddItem(shipRow, shipColumn, ship, shipPool);
		} else {
			gridLayout.MoveItem(shipRow, shipColumn, ship);
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
		if (currentSeed.EndsWith(oppositeSuffix))
			currentSeed = currentSeed.Substring(0, currentSeed.Length - oppositeSuffix.Length);
		else
			currentSeed = currentSeed + suffix;
	}
	
}
