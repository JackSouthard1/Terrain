using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour {

	Camera camera;
	public GameObject clickedMarker;
	float mapChunkSize;
	Landscape landscape;

	MapGenerator mapGenerator;
	EndlessTerrain endlessTerrainScript;

	public GameObject building;

	void Start () {
		camera = FindObjectOfType<Camera>();
		mapChunkSize = MapGenerator.mapChunkSize;
		mapGenerator = GameObject.Find("Map Generator").GetComponent<MapGenerator>();
		landscape = GameObject.Find("Map Generator").GetComponent<Landscape>();
		endlessTerrainScript = GameObject.Find ("Map Generator").GetComponent<EndlessTerrain> ();
	}
	
	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = camera.ScreenPointToRay (Input.mousePosition);
			Debug.DrawRay (ray.origin, ray.direction * 50, Color.yellow);

			RaycastHit hit; 

			if (Physics.Raycast (ray, out hit)) {
				clickedMarker.transform.position = hit.point;

//				Vector2 tile = TileFromWorldPoint (hit.point);
//				Vector2 chunkCord = ChunkCordFromWorldPoint (hit.point);
//				GameObject chunk = ChunkFromChunkCord (chunkCord);

//				FlattenPointsFromWorldPointArray(GetWorldPointsFromWorldPointAndSize(hit.point, 20));
				landscape.FlattenTerrain(new Vector2(hit.point.x, hit.point.z), 10);

//				PlaceGameObjectAtTileInChunk (building, tile, chunk, chunkCord);
			}
		}
	}

//	Vector3[] GetWorldPointsFromWorldPointAndSize (Vector3 initialWorldPoint, int size)
//	{
//		Vector3[] worldPoints = new Vector3[size * size];
//
//		Vector3 rInitialWorldPoint = new Vector3(Mathf.RoundToInt(initialWorldPoint.x), initialWorldPoint.y, Mathf.RoundToInt(initialWorldPoint.z));
//
//		int index = 0;
//
//		for (int collumn = 0; collumn < size; collumn++) {
//			for (int row = 0; row < size; row++) {
//				worldPoints[index] = new Vector3(rInitialWorldPoint.x + collumn, rInitialWorldPoint.y, rInitialWorldPoint.z + row);
//				index++;
//			}
//		}
//
//		return worldPoints;
//	}
//
//	void FlattenPointsFromWorldPointArray (Vector3[] worldPoints)
//	{
//		List<EndlessTerrain.TerrainChunk> totalTerrainChunks = new List<EndlessTerrain.TerrainChunk> ();
//		List<VertexData> vertexDatas = new List<VertexData> ();
//		List<float> vertexCordHeights = new List<float> ();
//		float averageVertexHeight = 0;
//
//		// Add vertices
//		for (int i = 0; i < worldPoints.Length; i++) {
//			Vector3 worldPoint = worldPoints [i];
//			Vector2 tile = GetTileFromWorldPoint (worldPoint);
//			Vector2 chunkCord = GetChunkCordFromWorldPoint (worldPoint);
//
//			EdgeData edgeData = VertexOnChunkEdge (worldPoint, chunkCord);
//			if (edgeData.onEdge) {
//				vertexDatas.Add (GetEquivilentVertex (worldPoint, edgeData, chunkCord));
//			}
//
//			EndlessTerrain.TerrainChunk terrainChunk = endlessTerrainScript.terrainChunkDictionary [chunkCord];
////			vertexDatas[i].chunk = terrainChunk;
//			if (!totalTerrainChunks.Contains (terrainChunk)) {
//				totalTerrainChunks.Add (terrainChunk);
//			}
//
////			vertexDatas[i].vertex2D = VertexCordFromTileCord(tile);
//			Vector2 vertex2D = GetVertexCordFromTileCord (tile);
//
//
////			float worldHeight = GetVertexHeightFromChunkAndIndex (terrainChunk, GetIndexFromTile (tile));
//
//			vertexCordHeights.Add (worldHeight);
//			vertexDatas.Add (new VertexData (vertex2D, terrainChunk, worldHeight));
//
//			// debug
////			if (GetEquivilentVertex (worldPoint, edgeData, chunkCord) == new VertexData (vertex2D, terrainChunk, worldHeight)) {
////				print ("same");
////			}
//		}
//
//		// calculate average height of vertexes
//		float averageHeight = AverageHeight(vertexCordHeights.ToArray());
//		float heightRatio = averageHeight; // TODO MAGIC NUMBER  / (mapGenerator.meshHeightMultiplier)
//
//		for (int n = 0; n < vertexDatas.Count; n++) {
//			vertexDatas[n].SetVertex3D (new Vector3 (vertexDatas[n].vertex2D.x, heightRatio, vertexDatas[n].vertex2D.y));
//		}
//
//		// move verticies
//		for (int o = 0; o < vertexDatas.Count; o++) {
////			vertexDatas[o].chunk.modifiedTerrainPoints.Add(vertexDatas[o].vertex3D);
//		}
//
//		// Update all terrain chunks that are effected
//		for (int e = 0; e < totalTerrainChunks.Count; e++) {
//			totalTerrainChunks[e].UpdateMesh();
//		}
//	}
//
//	float AverageHeight (float[] heights)
//	{
//		float average = 0;
//		float sum = 0;
//
//		for(int i = 0; i < heights.Length; i++) {
//    		sum += heights[i];
//		}
//		average = sum / heights.Length;
//
//		return average;
//	}
//
//	// Not sure if this works
//	EdgeData VertexOnChunkEdge (Vector3 worldPoint, Vector2 chunkCords)
//	{
//		worldPoint = new Vector3 (Mathf.RoundToInt (worldPoint.x), worldPoint.y, Mathf.RoundToInt (worldPoint.z));
//		bool onEdge = false;
//		bool onEdgeX = false;
//		bool onEdgeZ = false;
//
//		bool onEdgeTop = false;
//		bool onEdgeBottom = false;
//		bool onEdgeLeft = false;
//		bool onEdgeRight = false;
//
//		if (worldPoint.x % mapChunkSize == 0) {
//			onEdgeZ = true;
//		}
//		if (worldPoint.z % mapChunkSize == 0) {
//			onEdgeX = true;
//		}
//
//		if (onEdgeX) {
//			if ((chunkCords.y * mapChunkSize) + (mapChunkSize / 2) < worldPoint.z) {
//				onEdgeBottom = true;
//			} else {
//				onEdgeTop = true;
//			}
//		}
//		if (onEdgeZ) {
//			if ((chunkCords.x * mapChunkSize) + (mapChunkSize / 2) < worldPoint.x) {
//				onEdgeRight = true;
//			} else {
//				onEdgeLeft = true;
//			}
//		}
//
//		if (onEdgeX || onEdgeZ) {
//			onEdge = true;
////			print ("Point: " + worldPoint + ", Chunk Cords: " + chunkCords + ", Top: " + onEdgeTop + ", Bottom: " + onEdgeBottom + ", Left: " + onEdgeLeft + ", Right: " + onEdgeRight);
//		}
//
//		return new EdgeData(onEdge, onEdgeTop, onEdgeBottom, onEdgeLeft, onEdgeRight);
//	}
//
//	VertexData GetEquivilentVertex (Vector3 worldPoint, EdgeData edgeData, Vector2 chunkCords)
//	{
//		Vector2 newChunkCords = new Vector2();
//		Vector2 vertex2D = new Vector2 (Mathf.RoundToInt (worldPoint.x), Mathf.RoundToInt (worldPoint.z));
//		Vector2 newVertex2D = new Vector2();
//
//		if (edgeData.onEdgeTop) {
//			newChunkCords = new Vector2 (chunkCords.x, chunkCords.y + 1);
//			newVertex2D = new Vector2 (vertex2D.x, mapChunkSize);
//		} else if (edgeData.onEdgeBottom) {
//			newChunkCords = new Vector2 (chunkCords.x, chunkCords.y - 1);
//			newVertex2D = new Vector2 (vertex2D.x, 0f);
//		} else if (edgeData.onEdgeRight) {
//			newChunkCords = new Vector2 (chunkCords.x + 1, chunkCords.y);
//			newVertex2D = new Vector2 (0f, mapChunkSize - vertex2D.y);
//		} else if (edgeData.onEdgeLeft) {
//			newChunkCords = new Vector2 (chunkCords.x - 1, chunkCords.y);
//			newVertex2D = new Vector2 (mapChunkSize, mapChunkSize - vertex2D.y);
//		}
//
//		EndlessTerrain.TerrainChunk chunk = endlessTerrainScript.terrainChunkDictionary [newChunkCords];
//		Vector2 tile = new Vector2(newVertex2D.x, mapChunkSize - newVertex2D.y);
//
////		float height = GetVertexHeightFromChunkAndIndex(chunk, GetIndexFromTile(tile));
//		VertexData vertexData = new VertexData(newVertex2D, chunk, height);
//
//		print("New Vertex: " + newVertex2D + " New Chunk Cords: " + newChunkCords + " Height: " + height + " Tile: " + tile + " Index: " + GetIndexFromTile(tile));
//
//		return vertexData;
//	}
//
//	Vector2 GetVertexCordFromTileCord (Vector2 tileCord)
//	{
//		Vector2 newCords = new Vector2(tileCord.x, mapChunkSize - tileCord.y);
//		return newCords;
//	}
//
////
//	int GetIndexFromTile (Vector2 tile) {
//		Vector2 invertedTile = new Vector2 ((int)tile.x, (int)(mapChunkSize - tile.y));
//		int index = Mathf.RoundToInt(((invertedTile.y - 1) * mapChunkSize) + invertedTile.x);
//
//		return index * 6; // bc there are 6 vertices per slot
//	}
//
//	public Vector2 GetChunkCordFromWorldPoint (Vector3 point) {
//		Vector2 point2D = new Vector2(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.z));
////		print("Rounded Point: " + point2D);
////		print("World Point: " + point);
//		Vector2 chunkCord = new Vector2(Mathf.FloorToInt(point2D.x / mapChunkSize), Mathf.FloorToInt(point2D.y / mapChunkSize));
//		return chunkCord;
//	}
//
//	public GameObject GetChunkFromChunkCord (Vector2 chunkCord) {
//		GameObject[] chunks = GameObject.FindGameObjectsWithTag("Chunk");
//
//		GameObject closest = null;
//        float distance = Mathf.Infinity;
//
//		Vector3 pointToTestFrom = new Vector3(chunkCord.x * mapChunkSize, 0f, chunkCord.y * mapChunkSize);
//		pointToTestFrom = new Vector3(pointToTestFrom.x + (mapChunkSize / 2), 0f, pointToTestFrom.z + (mapChunkSize / 2));
////		print("PTTF: " + pointToTestFrom);
//
//        foreach (GameObject go in chunks) {
//            Vector3 diff = go.transform.position - pointToTestFrom;
//            float curDistance = diff.sqrMagnitude;
//            if (curDistance < distance) {
//                closest = go;
//                distance = curDistance;
//            }
//        }
//        return closest;
//	}
//
//	public Vector2 GetTileFromWorldPoint (Vector3 point)
//	{
//		Vector2 point2D = new Vector2 (Mathf.RoundToInt (point.x), Mathf.RoundToInt (point.z));
//		float pointX = point2D.x % mapChunkSize;
//		float pointY = point2D.y % mapChunkSize;
//
//		if (pointX < 0) {
//			pointX = mapChunkSize + pointX;
//		}
//		if (pointY < 0) {
//			pointY = mapChunkSize + pointY;
//		}
//
//		Vector2 pointInChunk = new Vector2(pointX, pointY);
//		return pointInChunk;
//	}
//
//	public void GetPlaceGameObjectAtTileInChunk (GameObject gameObject, Vector2 tile, GameObject chunk, Vector2 chunkCord) {
//		Vector2 position2D = new Vector2(chunkCord.x * mapChunkSize + tile.x, chunkCord.y * mapChunkSize + tile.y);
//
//		RaycastHit hit;
//		Physics.Raycast(Vector3.down, new Vector3(position2D.x, 100f, position2D.y), out hit);
//
//		float height = hit.point.y;
//		Vector3 position = new Vector3(position2D.x, height, position2D.y);
////		print ("Spawn Position: " + position);
//		GameObject.Instantiate(gameObject, position, Quaternion.identity, chunk.transform);
//	}
//}
//
//public class VertexData {
//	public Vector2 vertex2D;
//	public Vector3 vertex3D;
//
//	public EndlessTerrain.TerrainChunk chunk;
//	public float worldHeight;
//
//	public VertexData (Vector2 vertex2D, EndlessTerrain.TerrainChunk chunk, float worldHeight)
//	{
//		this.vertex2D = vertex2D;
//		this.chunk = chunk;
//		this.worldHeight = worldHeight;
//	}
//
//	public void SetVertex3D (Vector3 vertex3D)
//	{
//		this.vertex3D = vertex3D;
//	}
//}
//
//public struct EdgeData {
//	public bool onEdge;
//	public bool onEdgeTop;
//	public bool onEdgeBottom;
//	public bool onEdgeLeft;
//	public bool onEdgeRight;
//
//	public EdgeData (bool onE, bool top, bool bottom, bool left, bool right) {
//		onEdge = onE;
//		onEdgeTop = top;
//		onEdgeBottom = bottom;
//		onEdgeLeft = left;
//		onEdgeRight = right;
//	}
}
