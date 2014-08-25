using UnityEngine;
using System.Collections.Generic;

public class Islands : MonoBehaviour {

	public int islandsSize;

	public int initialHumanCount;
	public int minInitialSheepCount;
	public int maxInitialSheepCount;
	public int initialGrassPatches;
	[Range(0, 1)]
	public float movementProbability;
	[Range(0, 1)]
	public float eatGrassProbability;
	[Range(0, 1)]
	public float grassGrowProbability;

	public int seedBase;
	public float noiseSize;
	[Range(0, 10)]
	public float falloffExponent;
	[Range(0, 1)]
	public float threshold;

	public MeshFilter frontGround;
	public MeshFilter leftGround;
	public MeshFilter rightGround;
	public MeshFilter topGround;
	public MeshFilter bottomGround;

	public GridLayout frontLayout;
	public GridLayout leftLayout;
	public GridLayout rightLayout;
	public GridLayout topLayout;
	public GridLayout bottomLayout;

	public WorldRotator worldRotator;
	public GameObject sheepPrototype;
	public GameObject humanPrototype;
	public GameObject harborPrototype;
	public GameObject grassPrototype;
	public GameObject ship;

	public GameObject sheepPool;
	public GameObject humanPool;
	public GameObject harborPool;
	public GameObject grassPool;
	public GameObject shipPool;

	public string frontSeed;
	public int shipRow;
	public int shipColumn;
	public Direction shipDirection;

	[System.NonSerialized]
	private Dictionary<string, Island> islands;

	[System.NonSerialized]
	private bool lockRotator;

	[System.NonSerialized]
	private Island frontIsland;
	[System.NonSerialized]
	private Island leftIsland;
	[System.NonSerialized]
	private Island rightIsland;
	[System.NonSerialized]
	private Island topIsland;
	[System.NonSerialized]
	private Island bottomIsland;

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
		leftIsland = GetIsland(GetNeighborSeed(frontSeed, "l", "r"));
		rightIsland = GetIsland(GetNeighborSeed(frontSeed, "r", "l"));
		topIsland = GetIsland(GetNeighborSeed(frontSeed, "t", "b"));
		bottomIsland = GetIsland(GetNeighborSeed(frontSeed, "b", "t"));

		ShowIslandOnGrid(frontIsland, frontLayout, frontGround);
		ShowIslandOnGrid(leftIsland, leftLayout, leftGround);
		ShowIslandOnGrid(rightIsland, rightLayout, rightGround);
		ShowIslandOnGrid(topIsland, topLayout, topGround);
		ShowIslandOnGrid(bottomIsland, bottomLayout, bottomGround);

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
			island.Update(movementProbability, eatGrassProbability, grassGrowProbability);

		frontLayout.RemoveItemsFromPool(grassPool);
		foreach (var grass in frontIsland.grasses)
			AddGrassView(grass, frontLayout);
		leftLayout.RemoveItemsFromPool(grassPool);
		foreach (var grass in leftIsland.grasses)
			AddGrassView(grass, leftLayout);
		rightLayout.RemoveItemsFromPool(grassPool);
		foreach (var grass in rightIsland.grasses)
			AddGrassView(grass, rightLayout);
		topLayout.RemoveItemsFromPool(grassPool);
		foreach (var grass in topIsland.grasses)
			AddGrassView(grass, topLayout);
		bottomLayout.RemoveItemsFromPool(grassPool);
		foreach (var grass in bottomIsland.grasses)
			AddGrassView(grass, bottomLayout);
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

	public void ShowIslandOnGrid(Island island, GridLayout gridLayout, MeshFilter meshFilter) {
		gridLayout.size = islandsSize;
		gridLayout.Clear();
		meshFilter.transform.localScale = Vector3.one;
		meshFilter.mesh = IslandBuilder.Build(island);
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
			if (harborPool.transform.childCount > 0)
				instance = harborPool.transform.GetChild(0).gameObject;
			else
				instance = Object.Instantiate(harborPrototype) as GameObject;
			gridLayout.AddItem(harbor.row, harbor.column, 0, instance, harborPool);
		}
		foreach (var grass in island.grasses)
			AddGrassView(grass, gridLayout);
	}

	public void AddGrassView(Grass grass, GridLayout gridLayout) {
		GameObject instance;
		if (grassPool.transform.childCount > 0)
			instance = grassPool.transform.GetChild(0).gameObject;
		else
			instance = Object.Instantiate(grassPrototype) as GameObject;
		Random.seed = (grass.row * 7457) ^ (grass.column * 89);
		var grassTransform = instance.transform.GetChild(0).transform;
		grassTransform.localRotation =
			Quaternion.Euler(0f, 180f, Random.Range(0f, 360f) + grass.row * 97);
		var grassSize = Mathf.Clamp(grass.age * 0.2f, 0, 1);
		grassTransform.localScale = new Vector3(grassSize, grassSize, grassSize);
		gridLayout.AddItem(grass.row, grass.column, 0, instance, grassPool);
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
			island.CreateGrassPatches(initialGrassPatches);
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
