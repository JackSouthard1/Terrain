using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

	float moveSpeed = 7;
	float zoomSpeed = 30;

	void Start () {
		
	}

	void Update ()
	{
		float heightModifier = 0f;
		if (Input.GetKey (KeyCode.Equals)) {
			heightModifier = -zoomSpeed;
		} else if (Input.GetKey (KeyCode.Minus)) {
			heightModifier = zoomSpeed;
		}

		float speedModifier = transform.position.y / 10;

		Vector3 moveIncriment = new Vector3(Input.GetAxis("Horizontal") * moveSpeed * speedModifier, heightModifier, Input.GetAxis("Vertical") * moveSpeed * speedModifier); 
		transform.position = transform.position + moveIncriment * Time.deltaTime;
	}
}
