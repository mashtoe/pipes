using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterController : MonoBehaviour {

	//public GameObject cylinder;
	//public GameObject sphere;
	public GameObject pipe;
	public GameObject pipeParent;
	public GameObject cameraMain;
	public int boxLength;
	public int maxPipeCount;
	public int maxCylinderCount;

	private int[,,] box;
	private List<Vector3> availableSpaces;

	// Use this for initialization
	void Start () {
		ResetBox ();
		StartCoroutine (Routine());
		cameraMain.transform.position = new Vector3 (boxLength, boxLength, 0);
	}

	void ResetBox (){
		box = new int[boxLength, boxLength, boxLength];
		InitHashSet ();
	}

	void InitHashSet() {
		availableSpaces = new List<Vector3> ();
		for (int i = 0; i < boxLength; i++) {
			for (int j = 0; j < boxLength; j++) {
				for (int k = 0; k < boxLength; k++) {
					availableSpaces.Add (new Vector3(i,j,k));
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {
	}


	// called 50 times a second not regardless of what the framerate is
	void FixedUpdate () {
		
	}
		
	IEnumerator Routine() {
		int pipeAmount = 0;
		int cylinderAmount = 0;
		GameObject parent = Instantiate(pipeParent, Vector3.zero, Quaternion.identity) as GameObject;
		while (true) {
			if (pipeAmount < maxPipeCount && cylinderAmount < maxCylinderCount) {
				var pipeInstance = Instantiate (pipe, Vector3.zero, Quaternion.identity) as GameObject;
				pipeInstance.transform.parent = parent.transform;
				var pipeScript = pipeInstance.GetComponent<Pipe> ();
				Debug.Log ("Hey there: " + availableSpaces.Count);
				pipeScript.availableSpaces = availableSpaces;
				pipeScript.box = box;
				pipeScript.boxLength = boxLength;
				pipeScript.ResetPos ();
				yield return pipeScript.PipeRoutine ();
				cylinderAmount += pipeScript.cylinderAmt;
				Debug.Log (cylinderAmount + " " + pipeAmount);
				pipeAmount++;
			} else {
				yield return new WaitForSeconds (1f);
				Destroy (parent);
				pipeAmount = 0;
				cylinderAmount = 0;
				ResetBox ();
				parent = Instantiate(pipeParent, Vector3.zero, Quaternion.identity) as GameObject;
			}
		}
	}
}
