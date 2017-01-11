using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundOverDetector : MonoBehaviour {

	public static bool wasRoundOver = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	//On collision
	void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Target"){
			wasRoundOver = true;
		}
    }
}
