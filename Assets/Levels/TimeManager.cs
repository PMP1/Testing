using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour {
	
	public  float updateInterval = 0.5F;
	public  float fps = 0;
	public World world;

	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval
	private int previousTick = -1;



	// Use this for initialization
	void Start () {
		//world=worldGO.GetComponent("World") as World;
		timeleft = updateInterval; 

	}
	
	// Update is called once per frame
	void Update () {
		FrameRate ();
		DayTimeTick ();
	}
	
	void FrameRate() {
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;
		
		// Interval ended - update GUI text and start new interval
		if( timeleft <= 0.0 )
		{
			// display two fractional digits (f2 format)
			fps = accum/frames;
			//string format = System.String.Format("{0:F2} FPS",fps);
			//guiText.text = format;
			
			//if(fps < 30)
			//	guiText.material.color = Color.yellow;
			//else if(fps < 10)
			//	guiText.material.color = Color.red;
			//else
			//	guiText.material.color = Color.green;
			//	DebugConsole.Log(format,level);*/
			timeleft = updateInterval;
			accum = 0.0F;
			frames = 0;
		}
	}
	
	void DayTimeTick() {
		int tick = (int)Time.timeSinceLevelLoad % 24;
		if (tick != previousTick) {
			for(int x = 0; x < world.worldX; x++){ 
				for(int z = 0; z < world.worldZ; z++){ 
					world.chunks[x, z].updateLight = true;
				}
			}
			previousTick = tick;
		}
	}

	byte GetDayLightLevel(int tick) {

		if (tick < 6 || tick > 18)  {
			return (byte)0;
		}
		if (tick == 7 || tick == 15) {
			return (byte)2;
		}
		if (tick == 8 || tick == 14) {
			return (byte)5;
		}
		if (tick == 9 || tick == 13) {
			return (byte)10;
		}
		return 15;

	}
}
