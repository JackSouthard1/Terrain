using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour {

	Landscape landscape;
	public Building[] buildingPool = new Building[3];
	public Building building;

	void Start () {
		landscape = FindObjectOfType<Landscape>();

		buildingPool[0] = new Building(6, Resources.Load("TownHall") 	as GameObject);
		buildingPool[1] = new Building(4, Resources.Load("House") 		as GameObject);
		buildingPool[2] = new Building(5, Resources.Load("Smiths") 		as GameObject);

		building = buildingPool[0];
	}

	void Update ()
	{
		if (Input.GetKey ("1")) {
			building = buildingPool[0];
		} else if (Input.GetKey ("2")) {
			building = buildingPool[1];
		} else if (Input.GetKey ("3")) {
			building = buildingPool[2];
		}
	}

	public void PlaceBuilding (Vector2 worldCoord) {
		landscape.FlattenTerrain(worldCoord, building.size);

		float height = landscape.GetHeightOfPoint(worldCoord);
		Vector3 pos = new Vector3(worldCoord.x, height, worldCoord.y);

		GameObject.Instantiate(building.go, pos, Quaternion.identity, transform);
	}

//	public void PlaceTree (Vector2 worldCoord) {
//		float height = landscape.GetHeightOfPoint(worldCoord);
//
//		Vector3 pos = new Vector3(worldCoord.x, height, worldCoord.y);
//		GameObject.Instantiate(building.go, pos, Quaternion.identity, transform);
//	}
}

public class Building {
	public int size;
	public GameObject go;

	public Building (int size, GameObject go)
	{
		this.size = size;
		this.go = go;
	}
}
