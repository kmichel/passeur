using UnityEngine;

public class GridLayout : MonoBehaviour {

	[Range(1, 100)]
	public int size;

	GridLayout() {
		size = 1;
	}

	public void AddItem(int row, int column, float rotation, GameObject gameObject, GameObject pool) {
		gameObject.transform.parent = transform;
		GridLayoutItem gridLayoutItem = gameObject.GetComponent<GridLayoutItem>();
		if (gridLayoutItem == null)
			gridLayoutItem = gameObject.AddComponent<GridLayoutItem>();
		gridLayoutItem.pool = pool;
		SetRotation(gameObject, rotation);
		SetScale(gameObject, size);
		SetPosition(gameObject, row, column, size);
		gameObject.SetActive(true);
	}

	public void MoveItem(int row, int column, float rotation, GameObject gameObject) {
		SetRotation(gameObject, rotation);
		SetPosition(gameObject, row, column, size);
	}

	public void SetRotation(GameObject gameObject, float rotation) {
		gameObject.transform.localRotation = Quaternion.AngleAxis(rotation, Vector3.forward);
	}

	public void SetScale(GameObject gameObject, int gridSize) {
		var spacing = 1.0f / gridSize;
		gameObject.transform.localScale = new Vector3(spacing, spacing, spacing);
	}

	public void SetPosition(GameObject gameObject, int row, int column, int gridSize) {
		var spacing = 1.0f / gridSize;
		var offset = (spacing - 1.0f) * 0.5f;
		gameObject.transform.localPosition =
			new Vector3(offset + spacing * column, offset + spacing * row, 0);
	}

	public void Clear() {
		for (var index = transform.childCount - 1; index >= 0; --index) {
			var item = transform.GetChild(index);
			item.gameObject.SetActive(false);
			item.gameObject.transform.parent = item.GetComponent<GridLayoutItem>().pool.transform;
		}
	}

	public void RemoveItemsFromPool(GameObject pool) {
		for (var index = transform.childCount - 1; index >= 0; --index) {
			var item = transform.GetChild(index);
			if (item.GetComponent<GridLayoutItem>().pool == pool) {
				item.gameObject.SetActive(false);
				item.gameObject.transform.parent = item.GetComponent<GridLayoutItem>().pool.transform;
			}
		}
	}
}
