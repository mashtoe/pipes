using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour {

	private int maxLength;
	private int additionalSameDirPipesInPool;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public IEnumerator PipeRoutine(){
		yield return new WaitForSeconds (3f);
	}
}
