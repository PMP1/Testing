using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour {
	
	public  float updateInterval = 0.05F;
	public  float fps = 0;
	private World world;

	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval
	private int previousTick = -1;
	private byte previousDayLight = 0;


	// Use this for initialization
	void Start () {
		world=gameObject.GetComponent("World") as World;
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
		if (tick != previousTick ) {

			if (previousDayLight != GetDayLightLevel()) {

				for(int x = 0; x < world.chunks.GetLength(0); x++){ 
					for(int z = 0; z < world.chunks.GetLength(1); z++){ 
						if (world.chunks[x, z])
							world.chunks[x, z].changeDayLight = true;
					}
				}
				previousDayLight = GetDayLightLevel();
			}
			previousTick = tick;

		}
	}

	public byte GetDayLightLevel() {

		if (previousTick <= 6 || previousTick >= 18)  {
			return (byte)15;
		}
		if (previousTick == 7 || previousTick == 17) {
			return (byte)13;
		}
		if (previousTick == 8 || previousTick == 16) {
			return (byte)9;
		}
		if (previousTick == 9 || previousTick == 15) {
			return (byte)2;
		}
		return 0;

	}
}
