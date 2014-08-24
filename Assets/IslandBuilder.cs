using UnityEngine;
using System.Collections.Generic;

public class IslandBuilder {

	public static Mesh Build(Island island) {
		var size = island.size;
		var mesh = new Mesh();
		var sliceSize = (size + 1) * (size + 1);
		var vertices = new Vector3[sliceSize * 5];
		var uv = new Vector2[vertices.Length];
		for (var row = 0; row < size + 1; ++row) {
			for (var column = 0; column < size + 1; ++column) {
				var frontIndex = row * (size + 1) + column;
				var secondFrontIndex = frontIndex + sliceSize;
				var thirdFrontIndex = secondFrontIndex + sliceSize;
				var backIndex = thirdFrontIndex + sliceSize;
				var secondBackIndex = backIndex + sliceSize;
				var x = 1f / (size) * column;
				var y = 1f / (size) * row;
				vertices[frontIndex] = new Vector3(x - 0.5f, y - 0.5f, -0.5f / size);
				vertices[secondFrontIndex] = vertices[frontIndex];
				vertices[thirdFrontIndex] = vertices[frontIndex];
				vertices[backIndex] = new Vector3(x - 0.5f, y - 0.5f, 0.5f / size);
				vertices[secondBackIndex] = vertices[backIndex];
				uv[frontIndex] = new Vector2(x, y);
				uv[secondFrontIndex] = uv[frontIndex];
				uv[thirdFrontIndex] = uv[frontIndex];
				uv[backIndex] = uv[frontIndex];
				uv[secondBackIndex] = uv[frontIndex];
			}
		}
		var triangles = new List<int>();
		for (var row = 1; row < size; ++row) {
			for (var column = 1; column < size; ++column) {
				var isGround = island.cells[row * size + column] == CellType.Ground;
				var downIsGround = island.cells[(row - 1) * size + column] == CellType.Ground;
				var leftIsGround = island.cells[row * size + column - 1] == CellType.Ground;
				if (isGround) {
					triangles.Add(row * (size + 1) + column);
					triangles.Add((row + 1) * (size + 1) + column);
					triangles.Add(row * (size + 1) + column + 1);

					triangles.Add(row * (size + 1) + column + 1);
					triangles.Add((row + 1) * (size + 1) + column);
					triangles.Add((row + 1) * (size + 1) + column + 1);
				}
				if (isGround != leftIsGround) {
					if (isGround) {
						triangles.Add(sliceSize + row * (size + 1) + column);
						triangles.Add(sliceSize + row * (size + 1) + column + sliceSize * 2);
						triangles.Add(sliceSize + (row + 1) * (size + 1) + column);

						triangles.Add(sliceSize + row * (size + 1) + column + sliceSize * 2);
						triangles.Add(sliceSize + (row + 1) * (size + 1) + column + sliceSize * 2);
						triangles.Add(sliceSize + (row + 1) * (size + 1) + column);
					} else {
						triangles.Add(sliceSize + row * (size + 1) + column);
						triangles.Add(sliceSize + (row + 1) * (size + 1) + column);
						triangles.Add(sliceSize + row * (size + 1) + column + sliceSize * 2);

						triangles.Add(sliceSize + row * (size + 1) + column + sliceSize * 2);
						triangles.Add(sliceSize + (row + 1) * (size + 1) + column);
						triangles.Add(sliceSize + (row + 1) * (size + 1) + column + sliceSize * 2);
					}
				}
				if (isGround != downIsGround) {
					if (isGround) {
						triangles.Add(sliceSize * 2 + row * (size + 1) + column);
						triangles.Add(sliceSize * 2 + row * (size + 1) + column + 1);
						triangles.Add(sliceSize * 2 + row * (size + 1) + column + sliceSize * 2);

						triangles.Add(sliceSize * 2 + row * (size + 1) + column + 1);
						triangles.Add(sliceSize * 2 + row * (size + 1) + column + 1 + sliceSize * 2);
						triangles.Add(sliceSize * 2 + row * (size + 1) + column + sliceSize * 2);
					} else {
						triangles.Add(sliceSize * 2 + row * (size + 1) + column);
						triangles.Add(sliceSize * 2 + row * (size + 1) + column + sliceSize * 2);
						triangles.Add(sliceSize * 2 + row * (size + 1) + column + 1);

						triangles.Add(sliceSize * 2 + row * (size + 1) + column + 1);
						triangles.Add(sliceSize * 2 + row * (size + 1) + column + sliceSize * 2);
						triangles.Add(sliceSize * 2 + row * (size + 1) + column + 1 + sliceSize * 2);
					}
				}
			}
		}
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
		mesh.name = "Island " + island.seed;
		mesh.Optimize();
		return mesh;
	}
}
