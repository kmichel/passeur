using UnityEngine;
using System.Collections;

public class AnimalView : MonoBehaviour {

	[System.NonSerialized]
	public GridLayout gridLayout;
	[System.NonSerialized]
	public Animal animal;

	public void Update() {
		if (gridLayout != null && animal != null)
			gridLayout.MoveItem(animal.row, animal.column, 0, this.gameObject);
	}
}
