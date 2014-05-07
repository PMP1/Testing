using UnityEngine;
using System.Collections;

public class Gui : MonoBehaviour {
	

	public TimeManager time;
	public GameObject worldGO;
	World world;
	int lastTick = -1;
	
	private Texture2D tempTexture;
	private Texture2D humidTexture;
	private Texture2D terrainTexture;
	private Texture2D heightTexture;

	void Start () {
		world=worldGO.GetComponent("World") as World;
		
		print( world.worldGenerator.GetTexturePixel("Temperature",0,0));
		
		tempTexture = new Texture2D(200, 200, TextureFormat.ARGB32, false);
		for(int i = 0; i < 200; i++) {
			for(int j = 0; j < 200; j++) {
				tempTexture.SetPixel(i, j, world.worldGenerator.GetTexturePixel("Temperature",i*1,j*1));
			}
		}
		tempTexture.Apply();
		
		humidTexture = new Texture2D(200, 200, TextureFormat.ARGB32, false);
		for(int i = 0; i < 200; i++) {
			for(int j = 0; j < 200; j++) {
				humidTexture.SetPixel(i, j, world.worldGenerator.GetTexturePixel("Humidity",i*1,j*1));
			}
		}
		humidTexture.Apply();

		terrainTexture = new Texture2D(200, 200, TextureFormat.ARGB32, false);
		for(int i = 0; i < 200; i++) {
			for(int j = 0; j < 200; j++) {
				terrainTexture.SetPixel(i, j, world.worldGenerator.GetTexturePixel("Terrain",i*1,j*1));
			}
		}
		terrainTexture.Apply();
		
		heightTexture = new Texture2D(500, 500, TextureFormat.ARGB32, false);
		for(int i = 0; i < 500; i++) {
			for(int j = 0; j < 500; j++) {
				heightTexture.SetPixel(i, j, world.worldGenerator.GetTexturePixel("Height",i*16,j*16));
			}
		}
		heightTexture.Apply();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI () {

		//full day = 24 seconds
		//1 second = 1 tick

		GUI.DrawTexture(new Rect(10,10,100,100), tempTexture, ScaleMode.ScaleToFit, true, 1.0f);
		GUI.DrawTexture(new Rect(10,110,100,100), humidTexture, ScaleMode.ScaleToFit, true, 1.0f);
		GUI.DrawTexture(new Rect(10,210,100,100), terrainTexture, ScaleMode.ScaleToFit, true, 1.0f);
		GUI.DrawTexture(new Rect(10,310,100,100), heightTexture, ScaleMode.ScaleToFit, true, 1.0f);
		
		
		System.TimeSpan test = new System.TimeSpan();
		int totalChunks = 0;
		int totalUpdate1 = 0;
		int totalUpdate2 = 0;
		int totalUpdate3 = 0;
		
		for (int x=0; x<world.worldX/world.sectionSize; x++){
			for (int z=0; z<world.worldZ/world.sectionSize; z++){
				if (world.chunks[x, z]) {
					test.Add(world.chunks[x, z].updateTime);
					totalChunks ++;
					if (world.chunks[x, z].updateType == 1) totalUpdate1 ++;
					if (world.chunks[x, z].updateType == 2) totalUpdate2 ++;
					if (world.chunks[x, z].updateType == 3) totalUpdate3 ++;
				}
			}
		}
		
		GUI.Label (new Rect (300, 10, 100, 20), test.ToString() );
		GUI.Label (new Rect (300, 20, 100, 20), totalUpdate1.ToString() );
		GUI.Label (new Rect (300, 30, 100, 20), totalUpdate2.ToString() );
		GUI.Label (new Rect (300, 40, 100, 20), totalUpdate3.ToString() );
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
