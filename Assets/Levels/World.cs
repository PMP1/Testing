using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class World : MonoBehaviour {

	public byte[,,] data;
	public int worldX=16;
	public int worldY=16;
	public int worldZ=16;

	public GameObject section;
	public GameObject chunk;
	public Chunk[,] chunks; 
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
	// Use this for initialization

    public ChunkManager chunkManager;


	void Awake() {
	
		
		//Sytem starting
		start = System.DateTime.Now;
        this.SectionCollider = new SectionColliderGenerator ();


		chunks = new Chunk[Mathf.FloorToInt(worldX/sectionSize),
		                               Mathf.FloorToInt(worldZ/sectionSize)];
		                               
		configSettings = new WorldConfig("PMP");
		
        PerlinWorldGenerator.Init();
        PerlinWorldGenerator.SetSeed(configSettings.Seed);


        chunkManager = new ChunkManager(gameObject.GetComponent("World") as World);
	}


	// Update is called once per frame
	void Update () {
		runningTime = System.DateTime.Now.Subtract (start);
	}

	/// <summary>
	/// Gnerates the Chunk for a given x, z
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	/// <param name="dist">Dist.</param>
	public void GenColumn(int x, int z, float dist, bool useSectionLoader){

		GameObject newChunkColumn= Instantiate(chunk,new Vector3(x*sectionSize-0.5f,
		                                                               0*sectionSize+0.5f,
		                                                               z*sectionSize-0.5f),new Quaternion(0,0,0,0)) as GameObject;

		chunks [x, z] = newChunkColumn.GetComponent("Chunk") as Chunk;
		chunks [x, z].chunkX=x;
		chunks [x, z].chunkZ=z;
		chunks [x, z].worldY=worldY;
		chunks [x, z].worldGO=gameObject;
		chunks [x, z].world = gameObject.GetComponent ("World") as World;
		//chunks [x, z].data = new byte[sectionSize,worldY,sectionSize];
		chunks [x, z].heightMap = new int[sectionSize, sectionSize];
		chunks [x, z].useCollisionMatrix = dist < 132 ? true : false;
		//worldGenerator.CreateChunk(chunks [x, z]);
        chunks [x, z].Init (useSectionLoader);

	}
	
	public void UnloadColumn(int x, int z){

		Object.Destroy(chunks [x, z].gameObject);
	}
}
