using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour {

	private GameObject cylinder;
	private GameObject sphere;

	// reference passed
	public int[,,] box;
	public List<Vector3> availableSpaces;
	public int boxLength;

	// generated 
	private int maxLength;
	private int additionalSameDirPipesInPool;
	private float R, G, B; //color


	// other
	private string dirValue;
	private Vector3 realPos;
	private Vector3 boxPos; //for position in multi dimensional array. doesnt match pos since length of one pipe is 2
	public int cylinderAmt;

	// private string[] directions = { "up", "down", "forward", "back", "left", "right" };

	void Awake() {
		cylinderAmt = 0;
		//ResetPos ();
		dirValue = "";
		GeneratePipeValues ();
		cylinder = Resources.Load<GameObject>("CylinderGO") as GameObject;
		sphere = Resources.Load<GameObject>("Sphere") as GameObject;
	}

	// Use this for initialization
	void Start () {
	}

	void GeneratePipeValues() {
		maxLength = Random.Range (300,801);
		int[] values = { 0, 1, 5, 10, 20, 30 };
		additionalSameDirPipesInPool = values[Random.Range (0, values.Length)];

		R = Random.Range (0.0f, 1f);
		G = Random.Range (0.0f, 1f);
		B = Random.Range (0.0f, 1f);

		var cylinders = Resources.Load<GameObject>("") as GameObject;

	}

	// Update is called once per frame
	void Update () {
		
	}

	public IEnumerator PipeRoutine(){
		yield return new WaitForSeconds (0.1f);
		bool stop = false;
		while (stop == false) {
			var availableDirs = AvailableDirections (dirValue);
			if (availableDirs.Count > 0 && cylinderAmt < maxLength) {
				var rando = Random.Range (0, availableDirs.Count);
				yield return SpawnCylinder (availableDirs [rando], gameObject);
				cylinderAmt++;
			} else {
				stop = true;
			}
		}
	}

	public void ResetPos () {
		boxPos = availableSpaces [Random.Range (0, availableSpaces.Count)];
		realPos = boxPos * 2;
		//int randoX = Random.Range (0, boxLength);
		//int randoY = Random.Range (0, boxLength);
		//int randoZ = Random.Range (0, boxLength);
		//realPos = new Vector3 (randoX, randoY, randoZ) * 2;
		//boxPos = new Vector3 (randoX, randoY, randoZ);
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
		var countSpace = availableSpaces.Count;
		var index = availableSpaces.FindIndex (c => c.Equals (new Vector3 (boxPos.x, boxPos.y, boxPos.z)));
		availableSpaces.RemoveAt (index);

		createdObjcet.transform.Find("Cylinder").GetComponent<MeshRenderer> ().material.color = new Color (R,G,B);
		createdObjcet.transform.parent = parent.transform;
		if (oldDirValue != dirValue) {
			//spawn sphere since the pipe is turning
			var sphereInstance = Instantiate (sphere, oldPos, Quaternion.Euler(0,0,0)) as GameObject;
			sphereInstance.GetComponent<MeshRenderer> ().material.color = new Color (R,G,B);
			sphereInstance.transform.parent = parent.transform;

		}
		yield return new WaitForSeconds (0.02f);
	}

	List<string> AvailableDirections(string currentDirection) {
		//check up
		List<string> options = new List<string>();
		var up = (int)boxPos.y + 1;
		if (up < boxLength - 1 && box [(int)boxPos.x, up, (int)boxPos.z] != 1 && !(currentDirection.Equals("down"))) {
			options.Add ("up");
			if (currentDirection.Equals("up")) {
				for (int i = 0; i < additionalSameDirPipesInPool; i++) {
					options.Add ("up");
				}
			}
		}
		var down = (int)boxPos.y - 1;
		if (down > 0 && box [(int)boxPos.x, down, (int)boxPos.z] != 1 && !(currentDirection.Equals("up"))) {
			options.Add ("down");
			if (currentDirection.Equals("down")) {
				for (int i = 0; i < additionalSameDirPipesInPool; i++) {
					options.Add ("down");
				}
			}
		}
		var forward = (int)boxPos.z + 1;
		if (forward < boxLength - 1 && box [(int)boxPos.x, (int)boxPos.y, forward] != 1 && !(currentDirection.Equals("back"))) {
			options.Add ("forward");
			if (currentDirection.Equals("forward")) {
				for (int i = 0; i < additionalSameDirPipesInPool; i++) {
					options.Add ("forward");
				}
			}
		}
		var back = (int)boxPos.z - 1;
		if (back > 0 && box [(int)boxPos.x, (int)boxPos.y, back] != 1 && !(currentDirection.Equals("forward"))) {
			options.Add ("back");
			if (currentDirection.Equals("back")) {
				for (int i = 0; i < additionalSameDirPipesInPool; i++) {
					options.Add ("back");
				}
			}
		}
		var right = (int)boxPos.x + 1;
		if (right < boxLength - 1 && box [right, (int)boxPos.y, (int)boxPos.z] != 1 && !(currentDirection.Equals("left"))) {
			options.Add ("right");
			if (currentDirection.Equals("right")) {
				for (int i = 0; i < additionalSameDirPipesInPool; i++) {
					options.Add ("right");
				}
			}
		}
		var left = (int)boxPos.x - 1;	
		if (left > 0 && box [left, (int)boxPos.y, (int)boxPos.z] != 1 && !(currentDirection.Equals("right"))) {
			options.Add ("left");
			if (currentDirection.Equals("left")) {
				for (int i = 0; i < additionalSameDirPipesInPool; i++) {
					options.Add ("left");
				}
			}
		}
		return options;
	}
}
