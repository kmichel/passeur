using UnityEngine;
using System.Collections;

public class GridFiller : MonoBehaviour {

	public GameObject prototype;
	public GridLayout layout;

	void Start () {
		if (layout != null && prototype != null) {
			for (var column = 0; column < layout.size; ++column) {
				for (var row = 0; row < layout.size; ++row) {
					var instance = Object.Instantiate(prototype) as GameObject;
					instance.SetActive(true);
					instance.transform.localRotation = Quaternion.identity;
					layout.AddItem(row, column, instance);
               }
			}
		}
	}
}
