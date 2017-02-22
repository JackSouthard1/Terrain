using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour {

	Camera camera;
	public GameObject clickedMarker;
	float mapChunkSize;

	public GameObject building;

	void Start () {
		camera = FindObjectOfType<Camera>();
		mapChunkSize = MapGenerator.mapChunkSize - 1;
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

				PlaceGameObjectAtTileInChunk (building, tile, chunk, chunkCord);
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
		for (int i = 0; i < worldPoints.Length; i++) {
			Vector3 hitPoint = worldPoints[i];
			Vector2 tile = TileFromWorldPoint (hitPoint);
			Vector2 chunkCord = ChunkCordFromWorldPoint (hitPoint);
			GameObject chunk = ChunkFromChunkCord (chunkCord);
			EndlessTerrain.TerrainChunk terrainChunk = GameObject.Find ("Map Generator").GetComponent<EndlessTerrain> ().terrainChunkDictionary [chunkCord];
			Vector2 vertexCords = VertexCordFromTileCord (tile);
			terrainChunk.modifiedTerrainPoints.Add (vertexCords);
			terrainChunk.UpdateModifiedVerticies();
		}
	}

	Vector2 VertexCordFromTileCord (Vector2 tileCord)
	{
		Vector2 newCords = new Vector2(tileCord.x, mapChunkSize - tileCord.y);
		return newCords;
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
		GameObject placedObject = GameObject.Instantiate(gameObject, position, Quaternion.identity, chunk.transform);
	}
}
