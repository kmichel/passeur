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

	public IslandsNavigator navigator;
	public GridLayout gridLayout;
	public GameObject waterPrototype;
	public GameObject groundPrototype;

	public string currentSeed;
	[System.NonSerialized]
	private Island currentIsland;

	[System.NonSerialized]
	private Dictionary<string, Island> islands;

	public Islands() {
		currentSeed = "";
		islands = new Dictionary<string, Island>();
	}

	void Update () {
		currentSeed = navigator.seed;
		var newCurrentIsland = GetIsland(currentSeed);
		if (newCurrentIsland != currentIsland) {
			currentIsland = newCurrentIsland;
			gridLayout.size = islandsSize;
			gridLayout.Clear();
			for (var row = 0; row < islandsSize; ++row) {
				for (var column = 0; column < islandsSize; ++column) {
					var cellType = currentIsland.cells[row * islandsSize + column];
					var prototype = cellType == CellType.Water ? waterPrototype : groundPrototype;
					var instance = Object.Instantiate(prototype) as GameObject;
					instance.SetActive(true);
					instance.transform.localRotation = Quaternion.identity;
					gridLayout.AddItem(row, column, instance);
				}
			}
			gridLayout.Layout();
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
	
}
