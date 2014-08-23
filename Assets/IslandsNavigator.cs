using UnityEngine;
using System.Collections;

public class IslandsNavigator : MonoBehaviour {

	[System.NonSerialized]
	public string seed;

	IslandsNavigator() {
		seed = "";
	}

	void Update () {
		if (Input.GetKeyDown("left"))
			UpdateSeed("l", "r");
		if (Input.GetKeyDown("right"))
			UpdateSeed("r", "l");
		if (Input.GetKeyDown("up"))
			UpdateSeed("t", "b");
		if (Input.GetKeyDown("down"))
			UpdateSeed("b", "t");
	}

	public void UpdateSeed(string suffix, string oppositeSuffix) {
		if (seed.EndsWith(oppositeSuffix))
			seed = seed.Substring(0, seed.Length - oppositeSuffix.Length);
		else
			seed = seed + suffix;
	}
}
