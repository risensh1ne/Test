using UnityEngine;
using System.Collections;

public class PinchZoom : TouchLogic {

	public float zoomSpeed = 20.0f;

	private Vector2 currTouch1 = Vector2.zero,
	lastTouch1 = Vector2.zero,
	currTouch2 = Vector2.zero,
	lastTouch2 = Vector2.zero;

	private float currDist = 0.0f,
	lastDist = 0.0f;
	private float zoomFactor = 0.0f;

	void OnTouchBeganAnywhere()
	{
	}
	
	void OnTouchMovedAnywhere()
	{
		Zoom ();
	}

	void OnTouchStayedAnywhere()
	{
		Zoom ();
	}

	void OnTouchEndedAnywhere()
	{

	}

	void Zoom()
	{
		//GUIStyle style = new GUIStyle();
		//style.normal.textColor = Color.red;
		//GUI.Label (new Rect (100, 100, 50, 20), "Zoom invoked!!", style);

		switch (TouchLogic.currTouch) 
		{
		case 0:
			currTouch1 = Input.GetTouch (0).position;
			lastTouch1 = currTouch1 - Input.GetTouch (0).deltaPosition;
			break;
		case 1:
			currTouch2 = Input.GetTouch (1).position;
			lastTouch2 = currTouch2 - Input.GetTouch (1).deltaPosition;
			break;
		}


		if (TouchLogic.currTouch >= 1) {
			currDist = Vector2.Distance (currTouch1, currTouch2);
			lastDist = Vector2.Distance (lastTouch1, lastTouch2);
		} else {
			currDist = 0.0f;
			lastDist = 0.0f;
		}

		zoomFactor = Mathf.Clamp (lastDist - currDist, -50.0f, 50.0f); 
		Debug.Log ("zf" + zoomFactor + " c:" + currDist + " l:" + lastDist);

		//Vector3 PlayerPOS = GameObject.Find("Player").transform.transform.position;
		//transform.position = new Vector3(PlayerPOS.x, 5, PlayerPOS.z - DistanceAway);


		Camera.main.transform.Translate (Vector3.forward * zoomFactor * zoomSpeed * Time.deltaTime);
		
	}
}
