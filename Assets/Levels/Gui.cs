using UnityEngine;
using System.Collections;

public class Gui : MonoBehaviour {
	

	public TimeManager time;
	public GameObject worldGO;
	World world;
	int lastTick = -1;

	void Start () {
		//world=worldGO.GetComponent("World") as World;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI () {

		//full day = 24 seconds
		//1 second = 1 tick





		//int tick = (int)Time.timeSinceLevelLoad % 24;

		/*if (lastTick != tick) {
			for (int x=0; x<world.worldX; x++){
				for (int z=0; z<world.worldZ; z++){
					world.chunks[x, z].updateLight = true;
				}
			}
		}*/

		GUI.Label (new Rect (10, 10, 100, 20), Time.timeSinceLevelLoad.ToString() );

		GUI.Label (new Rect (10, 30, 100, 20), "Tick: " + time.tick.ToString() + " Seconds: " + (time.tick / 10).ToString());

		GUI.Label (new Rect (10, 50, 100, 20), time.fps.ToString());

	}

}
