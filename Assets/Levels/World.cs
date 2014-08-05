using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class World : MonoBehaviour {

	public byte[,,] data;
	public int worldX=16;
	public int worldY=16;
	public int worldZ=16;

	public GameObject section;
    public GameObject sectionGO;
	public GameObject chunk;
	//public Chunk[,] chunks; 
	public int sectionSize=16;

	//scripts
	public TimeManager time;
	public ModifyTerrain terrain;


	public WorldConfig configSettings;
	//public AbstractWorldGenerator worldGenerator;

	//public BlockManager BlockManager { get; set; }
	public SectionColliderGenerator SectionCollider { get; set; }

	public System.TimeSpan startupTime;
	public System.TimeSpan runningTime;
	private System.DateTime start;

    private Vector3 playerpos;
	// Use this for initialization

    public ChunkManager chunkManager;
   // public ChunkRenderer chunkRenderer;


	void Awake() {
	
		
		//Sytem starting
		start = System.DateTime.Now;
        this.SectionCollider = new SectionColliderGenerator ();
		                               
		configSettings = new WorldConfig("PMP");
		
        playerpos = GameObject.FindGameObjectWithTag("Player").transform.position;

        PerlinWorldGenerator.Init();
        PerlinWorldGenerator.SetSeed(configSettings.Seed);


        chunkManager = new ChunkManager(gameObject.GetComponent("World") as World);
        //chunkRenderer = new ChunkRenderer(chunkManager, gameObject.GetComponent("World") as World);
	}


	// Update is called once per frame
	void Update () {
        playerpos = GameObject.FindGameObjectWithTag("Player").transform.position;
		runningTime = System.DateTime.Now.Subtract (start);
        chunkManager.RenderMissingGOs();
	}

    private void OnApplicationQuit()
    {
        ChunkLoader.ShutDown();
    }
    	
	public void UnloadColumn(int x, int z){

		//Object.Destroy(chunks [x, z].gameObject);
	}

    public GameObject CreateSectionGO(Chunk2 chunk, Section2 sec) {
        GameObject go = Instantiate(sectionGO, 
                           new Vector3(chunk.xPosition * 16f - 0.5f, sec.posY * 16f + 0.5f, chunk.zPosition * 16f - 0.5f), 
                                     new Quaternion(0, 0, 0, 0)) as GameObject;

        return go;
    }


    public Vector3 GetPlayerPos() 
    {
        return playerpos;
    }
}
