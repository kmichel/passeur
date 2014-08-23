using UnityEngine;
using System.Collections.Generic;

public class GridLayout : MonoBehaviour {

	private class GridItem {
		public int row;
		public int column;
		public GameObject gameObject;
	}

	[Range(1, 100)]
	public int size;

	[System.NonSerialized]
	private List<GridItem> items;

	GridLayout() {
		size = 1;
		items = new List<GridItem>();
	}

	void Update () {
		var scaling = 1.0f / size;
		var spacing = scaling;
		var offset = (spacing - 1.0f) * 0.5f;
		for (var index = 0; index < items.Count; ++index) {
			var item = items[index];
			item.gameObject.transform.localScale =
				new Vector3(scaling, scaling, scaling);
			item.gameObject.transform.localPosition =
				new Vector3(offset + spacing * item.row, offset + spacing * item.column, 0);
		}
	}

	public void AddItem(int row, int column, GameObject gameObject) {
		var item = new GridItem();
		item.row = row;
		item.column = column;
		item.gameObject = gameObject;
		gameObject.transform.parent = transform;
		items.Add(item);
	}
}
