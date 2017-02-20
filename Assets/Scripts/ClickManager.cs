using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour {

	Camera camera;
	public GameObject clickedMarker;
	float mapChunkSize;
	float chunkSizeInWorldUnits;

	public GameObject building;

	void Start () {
		camera = FindObjectOfType<Camera>();
		mapChunkSize = MapGenerator.mapChunkSize - 1;
		chunkSizeInWorldUnits = (mapChunkSize) / 2;
	}
	
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = camera.ScreenPointToRay (Input.mousePosition);
			Debug.DrawRay (ray.origin, ray.direction * 50, Color.yellow);

			RaycastHit hit; 

			if (Physics.Raycast (ray, out hit)) {
				clickedMarker.transform.position = hit.point;

				Vector2 tile = TileFromWorldPoint(hit.point);
				Vector2 chunkCord = ChunkCordFromWorldPoint(hit.point);
				GameObject chunk = ChunkFromChunkCord(chunkCord);
//				print("PIC: " + TileFromChunkCordAndWorldPoint (hit.point));

				PlaceGameObjectAtTileInChunk(building, tile, chunk, chunkCord);
			}
		}
	}

	public Vector2 ChunkCordFromWorldPoint (Vector3 point) {
		Vector2 point2D = new Vector2(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.z));
		print("Rounded Point: " + point2D);
		print("World Point: " + point);
		Vector2 chunkCord = new Vector2(Mathf.FloorToInt(point2D.x / chunkSizeInWorldUnits), Mathf.FloorToInt(point2D.y / chunkSizeInWorldUnits));
		return chunkCord;
	}

	public GameObject ChunkFromChunkCord (Vector2 chunkCord) {
		GameObject[] chunks = GameObject.FindGameObjectsWithTag("Chunk");

		GameObject closest = null;
        float distance = Mathf.Infinity;

		Vector3 pointToTestFrom = new Vector3(chunkCord.x * chunkSizeInWorldUnits, 0f, chunkCord.y * chunkSizeInWorldUnits);
		pointToTestFrom = new Vector3(pointToTestFrom.x + (chunkSizeInWorldUnits / 2), 0f, pointToTestFrom.z + (chunkSizeInWorldUnits / 2));
		print("PTTF: " + pointToTestFrom);

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
		float pointX = point2D.x % chunkSizeInWorldUnits;
		float pointY = point2D.y % chunkSizeInWorldUnits;

		if (pointX < 0) {
			pointX = chunkSizeInWorldUnits + pointX;
		}
		if (pointY < 0) {
			pointY = chunkSizeInWorldUnits + pointY;
		}

		Vector2 pointInChunk = new Vector2(pointX, pointY);
		return pointInChunk;
	}

	public void PlaceGameObjectAtTileInChunk (GameObject gameObject, Vector2 tile, GameObject chunk, Vector2 chunkCord) {
		Vector2 position2D = new Vector2(chunkCord.x * chunkSizeInWorldUnits + tile.x, chunkCord.y * chunkSizeInWorldUnits + tile.y);

		RaycastHit hit;
		Physics.Raycast(Vector3.down, new Vector3(position2D.x, 100f, position2D.y), out hit);

		float height = hit.point.y;
		Vector3 position = new Vector3(position2D.x, height, position2D.y);
		print ("Spawn Position: " + position);
		GameObject placedObject = GameObject.Instantiate(gameObject, position, Quaternion.identity, chunk.transform);
	}
}
