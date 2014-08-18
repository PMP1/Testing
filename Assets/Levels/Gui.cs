using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System.Collections.Generic;

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
		
		print( PerlinWorldGenerator.GetTexturePixel("Temperature",0,0));
        Vector3 player = world.GetPlayerPos();

		tempTexture = new Texture2D(200, 200, TextureFormat.ARGB32, false);
		for(int i = 0; i < 200; i++) {
			for(int j = 0; j < 200; j++) {
                tempTexture.SetPixel(i, j, PerlinWorldGenerator.GetTexturePixel("TempHumid",i*2,j*2));
			}
		}
        tempTexture.SetPixel((int)player.x / 2, (int)player.z / 2, new Color(1f,1f,1f));
        tempTexture.Apply();
		
		heightTexture = new Texture2D(200, 200, TextureFormat.ARGB32, false);
		for(int i = 0; i < 200; i++) {
			for(int j = 0; j < 200; j++) {
                heightTexture.SetPixel(i, j, PerlinWorldGenerator.GetTexturePixel("Sea",-1600 + (i*16),-1600 + (j*16)));
			}
		}
        heightTexture.SetPixel((int)player.x / 16, (int)player.z / 16, new Color(1f,1f,1f));
		heightTexture.Apply();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI () {

        //full day = 24 seconds
        //1 second = 1 tick

        GUI.DrawTexture(new Rect(10, 10, 200, 200), tempTexture, ScaleMode.ScaleToFit, true, 1.0f);
        //GUI.DrawTexture(new Rect(10, 110, 100, 100), humidTexture, ScaleMode.ScaleToFit, true, 1.0f);
        GUI.DrawTexture(new Rect(10, 210, 200, 200), heightTexture, ScaleMode.ScaleToFit, true, 1.0f);
        //GUI.DrawTexture(new Rect(10, 310, 100, 100), heightTexture, ScaleMode.ScaleToFit, true, 1.0f);
		
		
        System.TimeSpan test = new System.TimeSpan();
        int totalChunks = 0;
        int totalUpdate1 = 0;
        int totalUpdate2 = 0;
        int totalUpdate3 = 0;
		
        /*for (int x=0; x<world.worldX/world.sectionSize; x++)
        {
            for (int z=0; z<world.worldZ/world.sectionSize; z++)
            {
                if (world.chunks [x, z])
                {
                    test.Add(world.chunks [x, z].updateTime);
                    totalChunks ++;
                    if (world.chunks [x, z].updateType == 1)
                        totalUpdate1 ++;
                    if (world.chunks [x, z].updateType == 2)
                        totalUpdate2 ++;
                    if (world.chunks [x, z].updateType == 3)
                        totalUpdate3 ++;
                }
            }
        }*/
		

        int loadTime = (int)Time.realtimeSinceStartup - (int)Time.timeSinceLevelLoad;
        int loadTest = (int)(((float)loadTime / 77f) * 100);


        GUI.Label(new Rect(300, 20, 300, 20), "Actual load time : ");
        GUI.Label(new Rect(300, 10, 300, 20), "Total Generation time : ");
        GUI.Label(new Rect(300, 30, 300, 20), " - Chunk Gen time : ");
        GUI.Label(new Rect(300, 40, 300, 20), " - Chunk Render time : ");
        GUI.Label(new Rect(300, 50, 300, 20), " - Chunk light time : ");
        GUI.Label(new Rect(300, 60, 300, 20), " - - Section Mesh Gen : ");
        GUI.Label(new Rect(300, 70, 300, 20), " - - Section Colllider : ");
        GUI.Label(new Rect(300, 80, 300, 20), " - - Section Smooth light: ");
        GUI.Label(new Rect(300, 90, 300, 20), " - - Section GO create : ");
        GUI.Label(new Rect(300, 100, 300, 20), "QueueLength : ");


        GUI.Label(new Rect(500, 20, 300, 20), loadTime.ToString());
        GUI.Label(new Rect(500, 10, 300, 20), world.startupTime.Seconds.ToString() + "(" + ((world.startupTime.Seconds / loadTime) * 100).ToString() + "%)");
        GUI.Label(new Rect(500, 30, 300, 20), StatsEngine.PrevChunkGenTime.ToString());
        GUI.Label(new Rect(500, 40, 300, 20), StatsEngine.ChunkRenderTime.ToString());
        GUI.Label(new Rect(500, 50, 300, 20), StatsEngine.ChunkSpreadLight.ToString());
        GUI.Label(new Rect(500, 60, 300, 20), StatsEngine.SectionMeshGen.ToString());
        GUI.Label(new Rect(500, 70, 300, 20), StatsEngine.SectionColliderGen.ToString());
        GUI.Label(new Rect(500, 80, 300, 20), StatsEngine.SectionSmoothLighting.ToString());
        GUI.Label(new Rect(500, 90, 300, 20), StatsEngine.SectionGoCreate.ToString());
        GUI.Label(new Rect(500, 100, 300, 20), StatsEngine.QueueLength  .ToString());


        GUI.Label(new Rect(10, 30, 100, 20), "Tick: " + time.tick.ToString() + " Seconds: " + (time.tick / 10).ToString());

        GUI.Label(new Rect(10, 50, 100, 20), "FPS:  " + time.fps.ToString());


        int hist = StatsEngine.GetSectionUpdate();
        GUI.Label(new Rect(10, 70, 200, 20), "Updated Sections: " + hist.ToString());

        float t1 = StatsEngine.GetSectionUpdateTime();
        GUI.Label(new Rect(10, 80, 200, 20), "Updated Section Time: " + t1.ToString("0.00000"));


    }
}
