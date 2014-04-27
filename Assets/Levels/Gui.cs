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

	void Start () {
		world=worldGO.GetComponent("World") as World;
		
		print( world.worldGenerator.GetTexturePixel("Temperature",0,0));
		
		tempTexture = new Texture2D(500, 500, TextureFormat.ARGB32, false);
		for(int i = 0; i < 500; i++) {
			for(int j = 0; j < 500; j++) {
				tempTexture.SetPixel(i, j, world.worldGenerator.GetTexturePixel("Temperature",i*16,j*16));
			}
		}
		tempTexture.Apply();
		
		humidTexture = new Texture2D(500, 500, TextureFormat.ARGB32, false);
		for(int i = 0; i < 500; i++) {
			for(int j = 0; j < 500; j++) {
				humidTexture.SetPixel(i, j, world.worldGenerator.GetTexturePixel("Humidity",i*16,j*16));
			}
		}
		humidTexture.Apply();

		terrainTexture = new Texture2D(500, 500, TextureFormat.ARGB32, false);
		for(int i = 0; i < 500; i++) {
			for(int j = 0; j < 500; j++) {
				terrainTexture.SetPixel(i, j, world.worldGenerator.GetTexturePixel("Terrain",i*16,j*16));
			}
		}
		terrainTexture.Apply();
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
