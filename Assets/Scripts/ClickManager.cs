using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour {

	Camera camera;
	public GameObject clickedMarker;
	float mapChunkSize;

	MapGenerator mapGenerator;

	public GameObject building;

	void Start () {
		camera = FindObjectOfType<Camera>();
		mapChunkSize = MapGenerator.mapChunkSize - 1;
		mapGenerator = GameObject.Find("Map Generator").GetComponent<MapGenerator>();
	}
	
	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = camera.ScreenPointToRay (Input.mousePosition);
			Debug.DrawRay (ray.origin, ray.direction * 50, Color.yellow);

			RaycastHit hit; 

			if (Physics.Raycast (ray, out hit)) {
				clickedMarker.transform.position = hit.point;

				Vector2 tile = TileFromWorldPoint (hit.point);
				Vector2 chunkCord = ChunkCordFromWorldPoint (hit.point);
				GameObject chunk = ChunkFromChunkCord (chunkCord);

				ModifyPointsFromWorldPointArray(WorldPointsFromWorldPointAndSize(hit.point, 2));

//				PlaceGameObjectAtTileInChunk (building, tile, chunk, chunkCord);
			}
		}
	}

	Vector3[] WorldPointsFromWorldPointAndSize (Vector3 initialWorldPoint, int size)
	{
		Vector3[] worldPoints = new Vector3[size * size];

		Vector3 rInitialWorldPoint = new Vector3(Mathf.RoundToInt(initialWorldPoint.x), initialWorldPoint.y, Mathf.RoundToInt(initialWorldPoint.z));

		worldPoints[0] = rInitialWorldPoint;
		worldPoints[1] = new Vector3(rInitialWorldPoint.x + 1, rInitialWorldPoint.y, rInitialWorldPoint.z);
		worldPoints[2] = new Vector3(rInitialWorldPoint.x, rInitialWorldPoint.y, rInitialWorldPoint.z + 1);
		worldPoints[3] = new Vector3(rInitialWorldPoint.x + 1, rInitialWorldPoint.y, rInitialWorldPoint.z + 1);

		return worldPoints;


//		// Get all points on x axis
//		int arrayIndex = 0;
//
//		for (int i = 0; i < size; i++) {
//			
//		}

	}

	void ModifyPointsFromWorldPointArray (Vector3[] worldPoints)
	{
		EndlessTerrain endlessTerrainScript = GameObject.Find ("Map Generator").GetComponent<EndlessTerrain> ();

		List<EndlessTerrain.TerrainChunk> totalTerrainChunks = new List<EndlessTerrain.TerrainChunk> ();
		EndlessTerrain.TerrainChunk[] terrainChunks = new EndlessTerrain.TerrainChunk[worldPoints.Length];

		Vector2[] vertexCords2D = new Vector2[worldPoints.Length];
		Vector3[] vertexCords3D = new Vector3[worldPoints.Length];
		float[] vertexCordHeights = new float[worldPoints.Length];
		float averageVertexHeight = 0;

		for (int i = 0; i < worldPoints.Length; i++) {
			Vector3 worldPoint = worldPoints [i];
			Vector2 tile = TileFromWorldPoint (worldPoint);
			Vector2 chunkCord = ChunkCordFromWorldPoint (worldPoint);

			EndlessTerrain.TerrainChunk terrainChunk = endlessTerrainScript.terrainChunkDictionary [chunkCord];
			terrainChunks[i] = terrainChunk;
			if (!totalTerrainChunks.Contains (terrainChunk)) {
				totalTerrainChunks.Add (terrainChunk);
			}

			vertexCords2D[i] = VertexCordFromTileCord(tile);

			vertexCordHeights[i] = VertexHeightFromChunkAndIndex(terrainChunk, IndexFromTile(tile));
		}

		// calculate average height of vertexes
		float averageHeight = AverageHeight(vertexCordHeights);
		float heightRatio = averageHeight / (mapGenerator.meshHeightMultiplier * 1f); // TODO MAGIC NUMBER ##

		for (int n = 0; n < worldPoints.Length; n++) {
			vertexCords3D[n] = new Vector3 (vertexCords2D[n].x, heightRatio, vertexCords2D[n].y);
		}

		// move verticies
		for (int o = 0; o < worldPoints.Length; o++) {
			terrainChunks[o].modifiedTerrainPoints.Add(vertexCords3D[o]);
		}

		// Update all terrain chunks that are effected
		for (int e = 0; e < totalTerrainChunks.Count; e++) {
			totalTerrainChunks[e].UpdateModifiedVerticies();
		}
	}

	float AverageHeight (float[] heights)
	{
		float average = 0;
		float sum = 0;

		for(int i = 0; i < heights.Length; i++) {
    		sum += heights[i];
		}
		average = sum / heights.Length;

		return average;
	}

	Vector2 VertexCordFromTileCord (Vector2 tileCord)
	{
		Vector2 newCords = new Vector2(tileCord.x, mapChunkSize - tileCord.y);
		return newCords;
	}

	float VertexHeightFromChunkAndIndex (EndlessTerrain.TerrainChunk terrainChunk, int index) {
		return terrainChunk.GetVertexHeight(index);
	}
//
	int IndexFromTile (Vector2 tile) {
		Vector2 invertedTile = new Vector2 ((int)tile.x, (int)(mapChunkSize - tile.y));
		int index = Mathf.RoundToInt(((invertedTile.y - 1) * mapChunkSize) + invertedTile.x);

		return index * 6; // bc there are 6 vertices per slot
	}

	public Vector2 ChunkCordFromWorldPoint (Vector3 point) {
		Vector2 point2D = new Vector2(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.z));
//		print("Rounded Point: " + point2D);
//		print("World Point: " + point);
		Vector2 chunkCord = new Vector2(Mathf.FloorToInt(point2D.x / mapChunkSize), Mathf.FloorToInt(point2D.y / mapChunkSize));
		return chunkCord;
	}

	public GameObject ChunkFromChunkCord (Vector2 chunkCord) {
		GameObject[] chunks = GameObject.FindGameObjectsWithTag("Chunk");

		GameObject closest = null;
        float distance = Mathf.Infinity;

		Vector3 pointToTestFrom = new Vector3(chunkCord.x * mapChunkSize, 0f, chunkCord.y * mapChunkSize);
		pointToTestFrom = new Vector3(pointToTestFrom.x + (mapChunkSize / 2), 0f, pointToTestFrom.z + (mapChunkSize / 2));
//		print("PTTF: " + pointToTestFrom);

        foreach (GameObject go in chunks) {
            Vector3 diff = go.transform.position - pointToTestFrom;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance) {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
	}

	public Vector2 TileFromWorldPoint (Vector3 point)
	{
		Vector2 point2D = new Vector2 (Mathf.RoundToInt (point.x), Mathf.RoundToInt (point.z));
		float pointX = point2D.x % mapChunkSize;
		float pointY = point2D.y % mapChunkSize;

		if (pointX < 0) {
			pointX = mapChunkSize + pointX;
		}
		if (pointY < 0) {
			pointY = mapChunkSize + pointY;
		}

		Vector2 pointInChunk = new Vector2(pointX, pointY);
		return pointInChunk;
	}

	public void PlaceGameObjectAtTileInChunk (GameObject gameObject, Vector2 tile, GameObject chunk, Vector2 chunkCord) {
		Vector2 position2D = new Vector2(chunkCord.x * mapChunkSize + tile.x, chunkCord.y * mapChunkSize + tile.y);

		RaycastHit hit;
		Physics.Raycast(Vector3.down, new Vector3(position2D.x, 100f, position2D.y), out hit);

		float height = hit.point.y;
		Vector3 position = new Vector3(position2D.x, height, position2D.y);
//		print ("Spawn Position: " + position);
		GameObject.Instantiate(gameObject, position, Quaternion.identity, chunk.transform);
	}
}
