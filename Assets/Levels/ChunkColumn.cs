using UnityEngine;
using System.Collections;

public class ChunkColumn : MonoBehaviour {

	public GameObject worldGO;
	World world;
	public Chunk[] chunks; 
	public int chunkSize=16;
	public GameObject chunk;
	public int chunkX;
	public int chunkY; //world y
	public int chunkZ;

	public bool update = false;
	public bool updateLight = true;

	//need to save world data here
	public byte[,,] data;
	public byte[,,] lightData;
	public byte[,,] daylightData;
	public int[,] heightMap; // records the max height of a solid object (used for quick daylight)

	// Use this for initialization
	void Start () {
		world=worldGO.GetComponent("World") as World;

		chunks=new Chunk[Mathf.FloorToInt(chunkY/chunkSize)];
		daylightData = new byte[chunkSize, chunkY, chunkSize];


		/*SetHeightMap ();
		GenerateDayLightData ();*/
		GenColumn ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate () {
		if(update || updateLight){
			//GenerateMesh();
			update=false;
		//}

		//if (updateLight) {
			updateLight = false;
			SetHeightMap();
			GenerateDayLightData();

			for( int i = 0; i < chunks.Length; i ++) {
				//if (chunks[i]) 
					chunks[i].update = true;
			}
		}
	}

	public void SetHeightMap() {
		heightMap = new int[16,16];

		for (int i = 0; i < 0; i++) {
			for (int j = 0; j < 0; j++) {
				heightMap[i,j] = 0;
			}
		}

		for (int x=0; x<chunkSize; x++) 
		{
			for (int z=0; z<chunkSize; z++) 
			{
				for (int y=chunkY - 1; y >= 0; y--) 
				{
					if (Block (x, y, z) != 0)
					{
						heightMap[x,z] = y;
						break;
					}
				}
			}
		}
	}

	public int GetDaylightHeight(int x, int z) 
	{
		return this.heightMap[x, z];
	}

	void GenerateDayLightData()
	{
		daylightData = new byte[chunkSize, chunkY, chunkSize];
		for (int x = 0; x < chunkSize; x++) {
			for (int z = 0; z < chunkSize; z++) {

				int y = this.heightMap[x, z];
				daylightData[x, y + 1, z] = 15;

				SpreadDaylight(x, y + 2, z, 13); //up
				SpreadDaylight(x, y, z, 13); //down
				SpreadDaylight(x + 1, y + 1, z, 13);
				SpreadDaylight(x - 1, y + 1, z, 13);
				SpreadDaylight(x, y + 1, z + 1, 13);
				SpreadDaylight(x, y + 1, z - 1, 13);
			}
		}


	}
	public void SpreadDaylight(int x, int y, int z, byte level) {

		if (x < 0 && chunkX - 1 > 0 && world.chunkColumns [chunkX - 1, chunkZ]) {
			world.chunkColumns [chunkX - 1, chunkZ].SpreadDaylight (chunkSize + x, y, z, level);
		}
		
		if (x >= chunkSize && chunkX + 1 < world.worldX && world.chunkColumns [chunkX + 1, chunkZ]) {
			world.chunkColumns [chunkX + 1, chunkZ].SpreadDaylight (chunkSize - x, y, z, level);
		}

		if (z < 0 && chunkZ - 1 > 0 && world.chunkColumns [chunkX, chunkZ - 1]) {
			world.chunkColumns [chunkX, chunkZ - 1].SpreadDaylight (x, y, chunkSize + z, level);
		}
		
		if (z >= chunkSize && chunkZ + 1 < world.worldX && world.chunkColumns [chunkX, chunkZ + 1]) {
			world.chunkColumns [chunkX, chunkZ + 1].SpreadDaylight (x, y, chunkSize - z, level);
		}
		if (y < 0 || y >= chunkY)
			return;

		if (x < 0 || y < 0 || z < 0 || x >= chunkSize || y >= chunkY || z >= chunkSize)
			return; //TODO SPREAD OVER CHUNKS



		if (daylightData [x, y, z] < level && level > 1 && Block(x,y,z) == 0) {

			daylightData [x, y, z] = level;
			level -= 2;
			SpreadDaylight(x, y + 1, z, level); //up
			SpreadDaylight(x, y - 1, z, level); //down
			SpreadDaylight(x + 1, y, z, level);
			SpreadDaylight(x - 1, y, z, level);
			SpreadDaylight(x, y, z + 1, level);
			SpreadDaylight(x, y, z - 1, level);
		}
	}


	public byte Block (int x, int y, int z)
	{
		if( x>=chunkSize || x<0 || y>=64 || y<0 || z>=chunkSize || z<0)
		{
			return (byte)0;
		}
		return data[x,y,z];
	}

	public byte LightBlock (int x, int y, int z)
	{
		if (x >= chunkSize && (chunkX*chunkSize) + 1 < world.worldX && world.chunkColumns[chunkX + 1, chunkZ]) {
			return world.chunkColumns[chunkX + 1, chunkZ].LightBlock(chunkSize - x,y,z);
		}
		if (x < 0 && chunkX - 1 >= 0 && world.chunkColumns[chunkX - 1, chunkZ]) {
			return world.chunkColumns[chunkX - 1, chunkZ].LightBlock(chunkSize + x,y,z);
		}
		if (z >= chunkSize && (chunkZ*chunkSize) + 1 < world.worldZ && world.chunkColumns[chunkX, chunkZ + 1]) {
			return world.chunkColumns[chunkX, chunkZ + 1].LightBlock(x,y,chunkSize - z);
		}
		if (z < 0 && chunkZ -1 >= 0 && world.chunkColumns[chunkX, chunkZ - 1]) {
			return world.chunkColumns[chunkX, chunkZ - 1].LightBlock(x,y,chunkSize + z);
		}

		if( x>=chunkSize || x<0 || y>=64 || y<0 || z>=chunkSize || z<0)
		{
			return (byte)0;
		}
		return daylightData[x,y,z];
	}

	public void GenColumn(){
		

		for (int y= (chunkY/chunkSize) - 1; y >= 0; y--){
			//Create a temporary Gameobject for the new chunk instead of using chunks[x,y,z]
			GameObject newChunk= Instantiate(chunk,new Vector3(chunkX*chunkSize-0.5f,
		                                                   y*chunkSize+0.5f,
			                                                   chunkZ*chunkSize-0.5f),new Quaternion(0,0,0,0)) as GameObject;

			chunks[y]= newChunk.GetComponent("Chunk") as Chunk;
			chunks[y].worldGO=worldGO;
			chunks[y].chunkColumnGO=this.gameObject;
			chunks[y].world=worldGO.GetComponent ("World") as World;
			chunks[y].chunkColumn=this.gameObject.GetComponent ("ChunkColumn") as ChunkColumn;
			chunks[y].chunkSize=chunkSize;
			chunks[y].chunkX=chunkX*chunkSize;
			chunks[y].chunkY=y*chunkSize;
			chunks[y].chunkZ=chunkZ*chunkSize;

			
		}
	}
}
