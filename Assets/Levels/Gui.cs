using UnityEngine;
using System.Collections;

public class Gui : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI () {

		//full day = 24 seconds
		//1 second = 1 tick



		int tick = (int)Time.timeSinceLevelLoad % 24;



		GUI.Label (new Rect (10, 10, 100, 20), Time.timeSinceLevelLoad.ToString() );

		GUI.Label (new Rect (10, 30, 100, 20), tick.ToString() + ":00" );

	}

}
