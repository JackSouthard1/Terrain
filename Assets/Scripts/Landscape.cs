using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landscape : MonoBehaviour {

	MapGenerator mapGen;
	EndlessTerrain endlessTerrainScript;

	public int mapSize;
	int chunkSize;

	// the height of each vertex in the landscape
	float[,] heights;

	void Awake () {
		mapGen = GameObject.Find("Map Generator").GetComponent<MapGenerator>();
		endlessTerrainScript = GameObject.Find("Map Generator").GetComponent<EndlessTerrain>();
		chunkSize = MapGenerator.mapChunkSize;
		heights = Noise.GenerateNoiseMap (chunkSize * mapSize, chunkSize * mapSize, mapGen.seed, mapGen.noiseScale, mapGen.octaves, mapGen.persistance, mapGen.lacunarity, Vector2.zero + mapGen.offset, mapGen.normalizeMode);
		ScaleHeights();
	}

	public void FlattenTerrain (Vector2 bottomLeft, int size)
	{
		bottomLeft = new Vector2 (Mathf.RoundToInt (bottomLeft.x), -Mathf.RoundToInt (bottomLeft.y));

		// calculate average height of all points
		float averageHeight = 0;
		float sum = 0;

		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				Vector2 coord = new Vector2 (bottomLeft.x + x, bottomLeft.y + y);
				sum += heights [(int)coord.x, (int)coord.y];
			}
		}
		averageHeight = sum / (size * size);

		// set heights of points
		List<Vector2> modifiedChunkCoords = new List<Vector2> ();

		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				Vector2 heightCoord = new Vector2 (bottomLeft.x + x, bottomLeft.y + y);
				heights [(int)heightCoord.x, (int)heightCoord.y] = averageHeight;
//				print("Modified Terrain Point at " + heightCoord + " to " + averageHeight);

				Vector2 chunkCoord = GetChunkCord (bottomLeft);
				EdgeData edgeData = PointOnChunkEdge (heightCoord, chunkCoord);

				if (edgeData.onEdge) {
					List<Vector2> borderingChunks = GetBorderingChunks (edgeData, chunkCoord);

					foreach (Vector2 coord in borderingChunks) {
						if (!modifiedChunkCoords.Contains (coord)) {
							modifiedChunkCoords.Add (coord);
						}
					}
				} else if (!modifiedChunkCoords.Contains (chunkCoord)) {
					modifiedChunkCoords.Add (chunkCoord);
				}
			}
		}

		// update modified chunks
		foreach (Vector2 coord in modifiedChunkCoords) {
			EndlessTerrain.TerrainChunk terrainChunk = endlessTerrainScript.terrainChunkDictionary [new Vector2(coord.x, -coord.y)];
			terrainChunk.UpdateMesh();
		}
	}

	public float[,] GetHeightsInChunk (Vector2 center) {
		int size = chunkSize + 2;
		float[,] chunkHeights = new float[size, size];

		for (int y = 0; y < size; y++) {
			for (int x = 0; x < size; x++) {
				chunkHeights [x, y] = heights[x + Mathf.RoundToInt(center.x), y - Mathf.RoundToInt(center.y)];
			}
		}

		return chunkHeights;
	}

	public float GetHeightOfPoint (Vector2 worldCoord)
	{
		worldCoord = new Vector2 (worldCoord.x, -worldCoord.y);
		return heights[(int)worldCoord.x, (int)worldCoord.y];
	}

	Vector2 GetChunkCord (Vector2 point) {
		Vector2 rPoint2D = new Vector2(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y));
		Vector2 chunkCord = new Vector2(Mathf.FloorToInt(rPoint2D.x / chunkSize), Mathf.FloorToInt(rPoint2D.y / chunkSize));

		return chunkCord;
	}

	// scale values from 0 - 1 to real world coordinate values
	void ScaleHeights () {
		int sizeX = heights.GetLength(0);
		int sizeY = heights.GetLength(1);

		for (int y = 0; y < sizeY; y++) {
			for (int x = 0; x < sizeX; x++) {
				heights[x, y] = heights[x, y] * mapGen.GetScaleValue(heights[x, y]);
			}
		}
	}

	EdgeData PointOnChunkEdge (Vector2 bottomLeft, Vector2 chunkCoords)
	{
		Vector2 worldPoint = new Vector2 (bottomLeft.x, -bottomLeft.y);
		int worldChunkSize = chunkSize - 1;
		bool onEdge = false;
		bool onEdgeX = false;
		bool onEdgeZ = false;

		bool onEdgeTop = false;
		bool onEdgeBottom = false;
		bool onEdgeLeft = false;
		bool onEdgeRight = false;

		if (worldPoint.x % worldChunkSize == 0) {
			onEdgeZ = true;
		}
		if (worldPoint.y % worldChunkSize == 0) {
			onEdgeX = true;
		}

		if (onEdgeX) {
			if ((chunkCoords.y * worldChunkSize) + (worldChunkSize / 2) < worldPoint.y) {
				onEdgeBottom = true;
			} else {
				onEdgeTop = true;
			}
		}
		if (onEdgeZ) {
			if ((chunkCoords.x * worldChunkSize) + (worldChunkSize / 2) < worldPoint.x) {
				onEdgeRight = true;
			} else {
				onEdgeLeft = true;
			}
		}
		if (onEdgeX || onEdgeZ) {
			onEdge = true;
//			print ("Point: " + worldPoint + ", Chunk Cords: " + chunkCords + ", Top: " + onEdgeTop + ", Bottom: " + onEdgeBottom + ", Left: " + onEdgeLeft + ", Right: " + onEdgeRight);
		}

		return new EdgeData(onEdge, onEdgeTop, onEdgeBottom, onEdgeLeft, onEdgeRight);
	}

	List<Vector2> GetBorderingChunks (EdgeData edgeData, Vector2 chunkCoords)
	{
		List<Vector2> borderingChunkCoords = new List<Vector2>();

		Vector2 newChunkCords = new Vector2();
		Vector2 newVertex2D = new Vector2();

		if (edgeData.onEdgeTop) {
			borderingChunkCoords.Add(new Vector2 (chunkCoords.x, chunkCoords.y + 1));
		} else if (edgeData.onEdgeBottom) {
			borderingChunkCoords.Add(new Vector2 (chunkCoords.x, chunkCoords.y - 1));
		} else if (edgeData.onEdgeRight) {
			borderingChunkCoords.Add(new Vector2 (chunkCoords.x + 1, chunkCoords.y));
		} else if (edgeData.onEdgeLeft) {
			borderingChunkCoords.Add(new Vector2 (chunkCoords.x - 1, chunkCoords.y));
		}

		return borderingChunkCoords;
	}
}

public struct EdgeData {
		public bool onEdge;
		public bool onEdgeTop;
		public bool onEdgeBottom;
		public bool onEdgeLeft;
		public bool onEdgeRight;

		public EdgeData (bool onE, bool top, bool bottom, bool left, bool right) {
			onEdge = onE;
			onEdgeTop = top;
			onEdgeBottom = bottom;
			onEdgeLeft = left;
			onEdgeRight = right;
		}
}
