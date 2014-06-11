using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class Chunk : MonoBehaviour {

	public GameObject worldGO;
	public World world;
	public Section[] sections; 
	public int sectionSize=16;
	public GameObject section;
	public int chunkX;
	public int worldY; //world y
	public int chunkZ;

	public bool update = false;
	public bool updateHeightMap = false;
	public bool updateLight = false;
	public bool changeDayLight = false;

	public bool useCollisionMatrix = false;

	//need to save world data here
	public byte biome;
	public byte[,,] data;
	public byte[,,] lightData;
	public byte[,,] daylightData;
	public int[,] heightMap; // records the max height of a solid object (used for quick daylight)



	//some debug info
	public byte updateType = 0; 
	public System.TimeSpan updateTime = System.TimeSpan.MinValue;
	
	// Use this for initialization
	void Start () {

	}

	public void Init() {
		sections=new Section[Mathf.FloorToInt(worldY/sectionSize)];
		daylightData = new byte[sectionSize, worldY, sectionSize];
		GenColumn ();
		update = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate () {

		DoUpdate ();
	}

	public void SetBlock(int x, int y, int z, BlockType type) 
	{
		data [x, y, z] = (byte)type;
	}

	public void DoUpdate () {
		
		System.DateTime start = System.DateTime.Now;

		if(update || updateLight || changeDayLight){

			string debugMessage = "";
			if (this.update) {
				updateType = 1;
				debugMessage = "UPDATE called: " + chunkX + ", " + chunkZ;
			} 
			if (this.updateLight) {
				updateType = 2;
				debugMessage = "UPDATE LIGHT called: " + chunkX + ", " + chunkZ;
			} 
			if (this.changeDayLight) {
				updateType = 3;
				debugMessage = "CHANGE DAYLIGHT called: " + chunkX + ", " + chunkZ;
			} 
			//print(debugMessage );

			if (updateHeightMap) {
				SetHeightMap();
				updateHeightMap = false;
			}

			if (this.update) {
				//terrain has been updated

				//SetHeightMap();
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
					sections[i].updateDayLight = true;
				}
				changeDayLight = false;
			}
		} else {
			updateType = 0;
		}
		
		System.DateTime end = System.DateTime.Now;
		updateTime = end.Subtract(start);
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

	/// <summary>
	/// Fills the sky to heightmap with light and then spreads out from where the light hits
	/// </summary>
	void GenerateDayLightData()
	{
		daylightData = new byte[sectionSize, worldY, sectionSize];

		//floodfill initial daylight
		for (int x = 0; x < sectionSize; x++) {
			for (int z = 0; z < sectionSize; z++) {
				for (int y = worldY - 1; y > this.heightMap [x, z] + 1; y--) {
					daylightData [x, y, z] = 15;
				}
			}
		}

		//spread daylightwhen we hit a floor
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

		//Get the daylight from neighbouring chunks
		SpreadDaylightFromXChunk (chunkX + 1, 0, sectionSize - 1);
		SpreadDaylightFromXChunk (chunkX - 1, sectionSize - 1, 0);
		SpreadDaylightFromZChunk (chunkZ + 1, 0, sectionSize - 1);
		SpreadDaylightFromZChunk (chunkZ - 1, sectionSize - 1, 0);
	}


	public void SpreadDaylight(int x, int y, int z, byte level) {

		if (!IsInChunk(x, y, z)) return;

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

	//TODO remove this funtion???
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
	private bool IsChunk(int x, int z) {

		if ( x < 0 || z < 0 || x >= world.worldX || z >= world.worldZ)
		{
			return false;
		}
		if (!world.chunks [x, z]) {
				return false;
		}
		return true;
	}

	/// <summary>
	/// Does x, y, z fall within the chunk bounds
	/// </summary>
	/// <returns></returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	private bool IsInChunk (int x, int y, int z)
	{
		if( x>=sectionSize || x<0 || y>=world.worldY || y<0 || z>=sectionSize || z<0)
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// Gets the neighbouring chunk based on the local x, y, y.
	/// </summary>
	/// <returns>The neighbouring chunk.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	private Chunk GetNeighbouringChunk(int x, int y, int z) {

		if (y >= world.worldY || y < 0) return null;

		int offsetX = 0;
		int offsetZ = 0;

		if (x < 0) offsetX = -1;
		if (x >= sectionSize) offsetX = 1;
		if (z < 0) offsetZ = -1;
		if (z >= sectionSize) offsetZ = 1;

		if (IsChunk(this.chunkX + offsetX, this.chunkZ + offsetZ))
			return world.chunks[this.chunkX + offsetX, this.chunkZ + offsetZ];

		return null; 
	}

	/// <summary>
	/// Gets the offset x, y, z for a neighhbour
	/// </summary>
	/// <returns>The offset local.</returns>
	/// <param name="i">The index.</param>
	private int GetNeighbourLocal(int i) {
		if (i < 0) return sectionSize + i;
		if (i >= sectionSize) return sectionSize - i;
		return i;
	}

	public byte Block (int x, int y, int z)
	{
		if ( !IsInChunk(x, y, z)) {

			Chunk neighbour = GetNeighbouringChunk(x, y, z);
			if (neighbour == null) return 1;
			return neighbour.data[GetNeighbourLocal(x), y, GetNeighbourLocal(z)];
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

		if( x>=sectionSize || x<0 || y>=worldY || y<0 || z>=sectionSize || z<0)
		{
			return (byte)0;
		}
		return daylightData[x,y,z];
	}

	public void GenColumn(){
		

		for (int y = (worldY/sectionSize) - 1; y >= 0; y--){
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
			sections[y].useCollisionMatrix = useCollisionMatrix;


			
		}
	}
}
