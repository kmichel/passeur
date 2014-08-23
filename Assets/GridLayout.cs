﻿using UnityEngine;
using System.Collections.Generic;

public class GridLayout : MonoBehaviour {

	private class GridItem {
		public int row;
		public int column;
		public GameObject gameObject;
		public GameObject pool;

		public void SetScale(int gridSize) {
			var spacing = 1.0f / gridSize;
			gameObject.transform.localScale = new Vector3(spacing, spacing, spacing);
		}

		public void SetPosition(int gridSize) {
			var spacing = 1.0f / gridSize;
			var offset = (spacing - 1.0f) * 0.5f;
			gameObject.transform.localPosition =
				new Vector3(offset + spacing * column, offset + spacing * row, 0);
		}
	}

	[Range(1, 100)]
	public int size;

	[System.NonSerialized]
	private Dictionary<GameObject, GridItem> items;

	GridLayout() {
		size = 1;
		items = new Dictionary<GameObject, GridItem>();
	}

	public void AddItem(int row, int column, GameObject gameObject, GameObject pool) {
		var item = new GridItem();
		item.row = row;
		item.column = column;
		item.gameObject = gameObject;
		item.pool = pool;
		items.Add(gameObject, item);

		gameObject.transform.parent = transform;
		item.SetScale(size);
		item.SetPosition(size);
	}

	public void MoveItem(int row, int column, GameObject gameObject) {
		var item = items[gameObject];
		item.row = row;
		item.column = column;
		item.SetPosition(size);
	}

	public void Clear() {
		foreach (var item in items.Values)
			item.gameObject.transform.parent = item.pool.transform;
		items.Clear();
	}
}
