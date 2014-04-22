using UnityEngine;
using System.Collections;

public class Chunk : MonoBehaviour {

	public GameObject worldGO;
	World world;
	public Section[] sections; 
	public int sectionSize=16;
	public GameObject section;
	public int chunkX;
	public int worldY; //world y
	public int chunkZ;

	public bool update = false;
	public bool updateLight = false;
	public bool changeDayLight = false;

	//need to save world data here
	public byte biome;
	public byte[,,] data;
	public byte[,,] lightData;
	public byte[,,] daylightData;
	public int[,] heightMap; // records the max height of a solid object (used for quick daylight)

	// Use this for initialization
	void Start () {
		world=worldGO.GetComponent("World") as World;

		sections=new Section[Mathf.FloorToInt(worldY/sectionSize)];
		daylightData = new byte[sectionSize, worldY, sectionSize];

		GenColumn ();
		update = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate () {


		if(update || updateLight || changeDayLight){

			string debugMessage = "";
			if (this.update) {
				debugMessage = "UPDATE called: " + chunkX + ", " + chunkZ;
			} 
			if (this.updateLight) {
				debugMessage = "UPDATE LIGHT called: " + chunkX + ", " + chunkZ;
			} 
			if (this.changeDayLight) {
				debugMessage = "CHANGE DAYLIGHT called: " + chunkX + ", " + chunkZ;
			} 
			//print(debugMessage );


			if (this.update) {
				//terrain has been updated

				SetHeightMap();
				GenerateDayLightData();

				for( int i = 0; i < sections.Length; i ++) {
					sections[i].update = true;
				}
				update=false;
				updateLight = false;
				changeDayLight = false;
			}
			if (this.updateLight) {

				GenerateDayLightData();
				
				for( int i = 0; i < sections.Length; i ++) {
					sections[i].lightUpdate = true;
				}
				updateLight = false;
				changeDayLight = false;
			}
			if (this.changeDayLight) {

				for( int i = 0; i < sections.Length; i ++) {
					sections[i].lightUpdate = true;
				}
				changeDayLight = false;
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

		for (int x=0; x<sectionSize; x++) 
		{
			for (int z=0; z<sectionSize; z++) 
			{
				for (int y=worldY - 1; y >= 0; y--) 
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

	/*public int GetDaylightHeight(int x, int z) 
	{
		return this.heightMap[x, z];
	}*/

	void GenerateDayLightData()
	{
		daylightData = new byte[sectionSize, worldY, sectionSize];
		for (int x = 0; x < sectionSize; x++) {
			for (int z = 0; z < sectionSize; z++) {

				int y = this.heightMap [x, z];
				daylightData [x, y + 1, z] = 15;

				SpreadDaylight (x, y + 2, z, 13); //up
				SpreadDaylight (x, y, z, 13); //down
				SpreadDaylight (x + 1, y + 1, z, 13);
				SpreadDaylight (x - 1, y + 1, z, 13);
				SpreadDaylight (x, y + 1, z + 1, 13);
				SpreadDaylight (x, y + 1, z - 1, 13);
			}
		}

		SpreadDaylightFromXChunk (chunkX + 1, 0, sectionSize - 1);
		SpreadDaylightFromXChunk (chunkX - 1, sectionSize - 1, 0);
		SpreadDaylightFromZChunk (chunkZ + 1, 0, sectionSize - 1);
		SpreadDaylightFromZChunk (chunkZ - 1, sectionSize - 1, 0);


	}

	public void SpreadDaylight(int x, int y, int z, byte level) {

		if (x < 0) {
			//SpreadDaylightToChunk(chunkX - 1, chunkZ, sectionSize + x, y, z, level);
			return; 
		}
		
		if (x >= sectionSize) {
			//SpreadDaylightToChunk(chunkX + 1, chunkZ, sectionSize - x, y, z, level);
			return; 
		}

		if (z < 0) {
			//SpreadDaylightToChunk(chunkX, chunkZ - 1, x, y, sectionSize + z, level);
			return; 
		}
		
		if (z >= sectionSize) {
			//SpreadDaylightToChunk(chunkX, chunkZ + 1, x, y, sectionSize - z, level);
			return; 
		}

		if (y < 0 || y >= worldY)
			return;


		if (daylightData [x, y, z] < level && level > 1 && Block(x,y,z) == 0) {

			daylightData [x, y, z] = level;
			updateLight = true;
			level -= 2;
			SpreadDaylight(x, y + 1, z, level); //up
			SpreadDaylight(x, y - 1, z, level); //down
			SpreadDaylight(x + 1, y, z, level);
			SpreadDaylight(x - 1, y, z, level);
			SpreadDaylight(x, y, z + 1, level);
			SpreadDaylight(x, y, z - 1, level);
		}
	}

	void SpreadDaylightFromXChunk(int chunk_X, int from_x, int to_x) {
		
		if (IsChunk (chunk_X, chunkZ)) {
			Chunk chunk = world.chunks[chunk_X, chunkZ];
			byte level = 0;
			for (int y = 0; y < worldY; y++) {
				for (int z = 0; z < sectionSize; z++) {
					if (chunk.daylightData != null) {
						level = chunk.daylightData[from_x,y,z];
						if (level >= 3) {
							level -= 2;
							SpreadDaylight(to_x, y, z, level);
						}
					}
				}
			}
		}
	}

	void SpreadDaylightFromZChunk(int chunk_Z, int from_z, int to_z) {
		
		if (IsChunk (chunkX, chunk_Z)) {
			Chunk chunk = world.chunks[chunkX, chunk_Z];
			byte level = 0;
			for (int y = 0; y < worldY; y++) {
				for (int x = 0; x < sectionSize; x++) {
					if (chunk.daylightData != null) {
						level = chunk.daylightData[x,y,from_z];
						if (level >= 3) {
							level -= 2;
							SpreadDaylight(x, y, to_z, level);
						}
					}
				}
			}
		}
	}

	void SpreadDaylightToChunk(int chunkX, int chunkZ, int x, int y, int z, byte level) {

		if (IsChunk (chunkX, chunkZ)) {
			world.chunks [chunkX, chunkZ].SpreadDaylight (x, y, z, level);
		}
	}

	/// <summary>
	/// Does a chunk exist and is loaded at x, z
	/// </summary>
	/// <returns></returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	bool IsChunk(int x, int z) {

		if ( x < 0 || z < 0 || x >= world.worldX || z >= world.worldZ)
		{
			return false;
		}
		if (!world.chunks [x, z]) {
				return false;
		}
		return true;
	}


	public byte Block (int x, int y, int z)
	{
		if( x>=sectionSize || x<0 || y>=64 || y<0 || z>=sectionSize || z<0)
		{
			return (byte)0;
		}
		return data[x,y,z];
	}

	public byte LightBlock (int x, int y, int z)
	{
		if (x >= sectionSize && (chunkX*sectionSize) + 1 < world.worldX && world.chunks[chunkX + 1, chunkZ]) {
			return world.chunks[chunkX + 1, chunkZ].LightBlock(sectionSize - x,y,z);
		}
		if (x < 0 && chunkX - 1 >= 0 && world.chunks[chunkX - 1, chunkZ]) {
			return world.chunks[chunkX - 1, chunkZ].LightBlock(sectionSize + x,y,z);
		}
		if (z >= sectionSize && (chunkZ*sectionSize) + 1 < world.worldZ && world.chunks[chunkX, chunkZ + 1]) {
			return world.chunks[chunkX, chunkZ + 1].LightBlock(x,y,sectionSize - z);
		}
		if (z < 0 && chunkZ -1 >= 0 && world.chunks[chunkX, chunkZ - 1]) {
			return world.chunks[chunkX, chunkZ - 1].LightBlock(x,y,sectionSize + z);
		}

		if( x>=sectionSize || x<0 || y>=64 || y<0 || z>=sectionSize || z<0)
		{
			return (byte)0;
		}
		return daylightData[x,y,z];
	}

	public void GenColumn(){
		

		for (int y= (worldY/sectionSize) - 1; y >= 0; y--){
			//Create a temporary Gameobject for the new chunk instead of using chunks[x,y,z]
			GameObject newChunk= Instantiate(section,new Vector3(chunkX*sectionSize-0.5f,
			                                                   y*sectionSize+0.5f,
			                                                     chunkZ*sectionSize-0.5f),new Quaternion(0,0,0,0)) as GameObject;

			sections[y]= newChunk.GetComponent("Section") as Section;
			sections[y].worldGO=worldGO;
			sections[y].chunkGO=this.gameObject;
			sections[y].world=worldGO.GetComponent ("World") as World;
			sections[y].chunk=this.gameObject.GetComponent ("Chunk") as Chunk;
			sections[y].sectionSize=sectionSize;
			sections[y].sectionX=chunkX*sectionSize;
			sections[y].sectionY=y*sectionSize;
			sections[y].sectionZ=chunkZ*sectionSize;

			
		}
	}
}
