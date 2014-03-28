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
	public bool updateLight = true;

	//need to save world data here
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

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate () {
		if(update || updateLight){

			update=false;

			updateLight = false;
			SetHeightMap();
			GenerateDayLightData();

			for( int i = 0; i < sections.Length; i ++) {
				sections[i].update = true;
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

	public int GetDaylightHeight(int x, int z) 
	{
		return this.heightMap[x, z];
	}

	void GenerateDayLightData()
	{
		daylightData = new byte[sectionSize, worldY, sectionSize];
		for (int x = 0; x < sectionSize; x++) {
			for (int z = 0; z < sectionSize; z++) {

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

		if (x < 0 && chunkX - 1 > 0 && world.chunks [chunkX - 1, chunkZ]) {
			world.chunks [chunkX - 1, chunkZ].SpreadDaylight (sectionSize + x, y, z, level);
		}
		
		if (x >= sectionSize && chunkX + 1 < world.worldX && world.chunks [chunkX + 1, chunkZ]) {
			world.chunks [chunkX + 1, chunkZ].SpreadDaylight (sectionSize - x, y, z, level);
		}

		if (z < 0 && chunkZ - 1 > 0 && world.chunks [chunkX, chunkZ - 1]) {
			world.chunks [chunkX, chunkZ - 1].SpreadDaylight (x, y, sectionSize + z, level);
		}
		
		if (z >= sectionSize && chunkZ + 1 < world.worldX && world.chunks [chunkX, chunkZ + 1]) {
			world.chunks [chunkX, chunkZ + 1].SpreadDaylight (x, y, sectionSize - z, level);
		}
		if (y < 0 || y >= worldY)
			return;

		if (x < 0 || y < 0 || z < 0 || x >= sectionSize || y >= worldY || z >= sectionSize)
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
