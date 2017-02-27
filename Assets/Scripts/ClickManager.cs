using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour {

	PlacementManager placementManager;
	Camera cam;

	public GameObject building;

	void Start () {
		cam = FindObjectOfType<Camera>();
		placementManager = FindObjectOfType<PlacementManager>();
	}
	
	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = cam.ScreenPointToRay (Input.mousePosition);
			Debug.DrawRay (ray.origin, ray.direction * 50, Color.yellow);

			RaycastHit hit; 

			if (Physics.Raycast (ray, out hit)) {
				placementManager.PlaceObject(new Vector2((int)hit.point.x, (int)hit.point.z));
			}
		}
	}
}