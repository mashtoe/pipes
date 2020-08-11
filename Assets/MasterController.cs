using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterController : MonoBehaviour {

	public GameObject pipe;
	public GameObject cylinder;
	public GameObject sphere;
	public GameObject pipeParent;
	public GameObject cameraMain;
	public int boxLength;
	public int additionalSameDirCylinders;
	public int pipeLength;
	public int maxPipeCount;


	public bool run;


	private int[,,] box;
	private Vector3 realPos;
	private Vector3 boxPos; //for position in multi dimensional array. doesnt match pos since length of one pipe is 2
	private string dirValue;
	// private string[] directions = { "up", "down", "forward", "back", "left", "right" };

	// Use this for initialization
	void Start () {
		dirValue = "";
		ResetBox ();
		StartCoroutine (Routine());
		cameraMain.transform.position = new Vector3 (boxLength, boxLength, 0);
		//Resources.LoadAll??
	}

	void ResetBox (){
		box = new int[boxLength, boxLength, boxLength];
		ResetPos ();
	}

	void ResetPos () {
		int randoX = Random.Range (0, boxLength);
		int randoY = Random.Range (0, boxLength);
		int randoZ = Random.Range (0, boxLength);
		realPos = new Vector3 (randoX, randoY, randoZ) * 2;
		boxPos = new Vector3 (randoX, randoY, randoZ);
	}

	
	// Update is called once per frame
	void Update () {
	}


	// called 50 times a second not affected by framerate
	void FixedUpdate () {
		
	}

	IEnumerator SpawnCylinder(string dir, GameObject parent) {
		var oldPos = realPos;
		var oldDirValue = dirValue;
		GameObject createdObjcet = null;

		switch (dir) {
		case "up":
			createdObjcet = Instantiate (cylinder, oldPos, Quaternion.Euler (0, 0, 0)) as GameObject;
			realPos = realPos + new Vector3 (0, 2, 0);
			dirValue = "up";
			boxPos = boxPos + new Vector3 (0,1,0);

			break;
		case "down":
			createdObjcet = Instantiate (cylinder, oldPos, Quaternion.Euler(180,0,0)) as GameObject;
			realPos = realPos + new Vector3 (0, -2, 0);
			dirValue = "down";
			boxPos = boxPos + new Vector3 (0,-1,0);
			break;
		case "forward":
			createdObjcet = Instantiate (cylinder, oldPos, Quaternion.Euler(90,0,0)) as GameObject;
			realPos = realPos + new Vector3 (0, 0, 2);
			dirValue = "forward";
			boxPos = boxPos + new Vector3 (0,0,1);
			break;
		case "back":
			createdObjcet = Instantiate (cylinder, oldPos, Quaternion.Euler(270,0,0)) as GameObject;
			realPos = realPos + new Vector3 (0, 0, -2);
			dirValue = "back";
			boxPos = boxPos + new Vector3 (0,0,-1);
			break;
		case "right":
			createdObjcet = Instantiate (cylinder, oldPos, Quaternion.Euler(0,0,270)) as GameObject;
			realPos = realPos + new Vector3 (2, 0, 0);
			dirValue = "right";
			boxPos = boxPos + new Vector3 (1,0,0);
			break;
		case "left":
			createdObjcet = Instantiate (cylinder, oldPos, Quaternion.Euler(0,0,90)) as GameObject;
			realPos = realPos + new Vector3 (-2, 0, 0);
			dirValue = "left";
			boxPos = boxPos + new Vector3 (-1,0,0);
			break;
		
		default:
			createdObjcet = null;
			break;
		}
		box [(int)boxPos.x, (int)boxPos.y, (int)boxPos.z] = 1;

		createdObjcet.transform.parent = parent.transform;
		if (oldDirValue != dirValue) {
			//spawn sphere since the pipe is turning
			var sphereInstance = Instantiate (sphere, oldPos, Quaternion.Euler(0,0,0)) as GameObject;
			sphereInstance.transform.parent = parent.transform;
		}
		yield return new WaitForSeconds (0.02f);
	}

	IEnumerator Routine() {

		var parent = Instantiate (pipeParent, Vector3.zero, Quaternion.identity);
		int counter = 0;
		int pipeCounter = 0;
		while (true) {
			var availableDirs = AvailableDirections (dirValue);
			if (availableDirs.Count > 0 && counter < pipeLength) {
				var rando = Random.Range (0, availableDirs.Count);
				yield return SpawnCylinder (availableDirs [rando], parent);
				counter++;
			} else if (pipeCounter < maxPipeCount) {
				//spawn new pipe
				pipeCounter++;
				counter = 0;
				ResetPos ();
				yield return new WaitForSeconds (1);

			} else {
				yield return new WaitForSeconds (1);
				ResetBox ();
				counter = 0;
				pipeCounter = 0;
				Destroy (parent);
				parent = Instantiate (pipeParent, Vector3.zero, Quaternion.identity);
			}
		}
	}

	List<string> AvailableDirections(string currentDirection) {
		//check up
		List<string> options = new List<string>();
		var up = (int)boxPos.y + 1;
		if (up < boxLength - 1 && box [(int)boxPos.x, up, (int)boxPos.z] != 1 && !(currentDirection.Equals("down"))) {
			options.Add ("up");
			if (currentDirection.Equals("up")) {
				for (int i = 0; i < additionalSameDirCylinders; i++) {
					options.Add ("up");
				}
			}
		}
		var down = (int)boxPos.y - 1;
		if (down > 0 && box [(int)boxPos.x, down, (int)boxPos.z] != 1 && !(currentDirection.Equals("up"))) {
			options.Add ("down");
			if (currentDirection.Equals("down")) {
				for (int i = 0; i < additionalSameDirCylinders; i++) {
					options.Add ("down");
				}
			}
		}
		var forward = (int)boxPos.z + 1;
		if (forward < boxLength - 1 && box [(int)boxPos.x, (int)boxPos.y, forward] != 1 && !(currentDirection.Equals("back"))) {
			options.Add ("forward");
			if (currentDirection.Equals("forward")) {
				for (int i = 0; i < additionalSameDirCylinders; i++) {
					options.Add ("forward");
				}
			}
		}
		var back = (int)boxPos.z - 1;
		if (back > 0 && box [(int)boxPos.x, (int)boxPos.y, back] != 1 && !(currentDirection.Equals("forward"))) {
			options.Add ("back");
			if (currentDirection.Equals("back")) {
				for (int i = 0; i < additionalSameDirCylinders; i++) {
					options.Add ("back");
				}
			}
		}
		var right = (int)boxPos.x + 1;
		if (right < boxLength - 1 && box [right, (int)boxPos.y, (int)boxPos.z] != 1 && !(currentDirection.Equals("left"))) {
			options.Add ("right");
			if (currentDirection.Equals("right")) {
				for (int i = 0; i < additionalSameDirCylinders; i++) {
					options.Add ("right");
				}
			}
		}
		var left = (int)boxPos.x - 1;	
		if (left > 0 && box [left, (int)boxPos.y, (int)boxPos.z] != 1 && !(currentDirection.Equals("right"))) {
			options.Add ("left");
			if (currentDirection.Equals("left")) {
				for (int i = 0; i < additionalSameDirCylinders; i++) {
					options.Add ("left");
				}
			}
		}
		return options;
	}
}
