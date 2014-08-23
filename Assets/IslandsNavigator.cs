using UnityEngine;
using System.Collections;

public class IslandsNavigator : MonoBehaviour {

	public IslandGenerator generator;

	[System.NonSerialized]
	private string seed;

	void Update () {
		if (seed == null)
			seed = "";

		if (Input.GetKeyDown("left"))
			UpdateSeed("l", "r");
		if (Input.GetKeyDown("right"))
			UpdateSeed("r", "l");
		if (Input.GetKeyDown("up"))
			UpdateSeed("t", "b");
		if (Input.GetKeyDown("down"))
			UpdateSeed("b", "t");

		if (generator != null)
			generator.seed = seed;
	}

	public void UpdateSeed(string suffix, string oppositeSuffix) {
		if (seed == null)
			seed = suffix;
		else if (seed.EndsWith(oppositeSuffix))
			seed = seed.Substring(0, seed.Length - oppositeSuffix.Length);
		else
			seed = seed + suffix;
	}
}
