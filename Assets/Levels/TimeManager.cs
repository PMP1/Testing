using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class TimeManager : MonoBehaviour {
	
	public  float updateInterval = 0.05F;
	public  float fps = 0;
	public  int tick = 0;
	private World world;

	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval
	
	
	private int previousTick = -1;
	private byte previousDayLight = 0;
	
	private int startTick = 200;
	private float ticksInASecond = 10;
	private int ticksInAnHour = 50; // 1 hour = 10 seconds
	private int hoursInADay = 24; // 1 day = 240 seconds = 2400 ticks
	//months, seasons, moon phases, temperature
	


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
		
        StatsEngine.UpdateTime(Time.deltaTime);
		// Interval ended - update GUI text and start new interval
		if( timeleft <= 0.0 ) {
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

		tick = (int)(Time.timeSinceLevelLoad * ticksInASecond) + startTick;

		if (tick != previousTick) {

			CheckTickEvents();

			previousTick = tick;
		}
	}
	
	
	private void CheckTickEvents() {
        
        if (tick % ticksInAnHour == 0) {
            //Change of hour
        }
        
        if (tick % (ticksInAnHour * hoursInADay) == 0) {
            //change of day
        }
        
		CheckLightLevels ();


		if (tick > (ticksInAnHour * hoursInADay)) {
			//tick = 0;
		}

        world.chunkManager.PerformTick(true);
    }
	
	
    private void CheckLightLevels() {

		byte currentLight = previousDayLight;
		int dayTick = tick % (ticksInAnHour * hoursInADay);

        //daybreak
        if (dayTick > 6 * ticksInAnHour && dayTick <= 7 * ticksInAnHour) {
            float perc = ((float)dayTick - (6 * ticksInAnHour))/ (float)ticksInAnHour;
            currentLight = (byte)((perc * 15));
        }
        //sunset
        if (dayTick > 22 * ticksInAnHour && dayTick <= 23 * ticksInAnHour) {
            float perc = ((float)dayTick - (22 * ticksInAnHour))/ (float)ticksInAnHour;
            currentLight = (byte)(15 - (perc * 15));
        }

		if (previousDayLight != currentLight) {
			UpdateLight();
			previousDayLight = currentLight;
		}
    }

	private void UpdateLight() {

        world.chunkManager.UpdateDaylight(this.previousDayLight);
	}

	public byte GetDaylightLevel() {
		return this.previousDayLight;
	}
    
}
