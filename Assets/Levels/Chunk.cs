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

	public int[,] heightMap; // records the max height of a solid object (used for quick daylight)
    public int maxHeight;
    public int minHeight;

	//some debug info
	public byte updateType = 0; 
	public System.TimeSpan updateTime = System.TimeSpan.MinValue;
	
	// Use this for initialization
	void Start () {

	}

	public void Init(bool useSectionLoader) {
		sections=new Section[Mathf.FloorToInt(worldY/sectionSize)];
		//daylightData = new byte[sectionSize, worldY, sectionSize];
		DefaultHeightMap();// sets the heightmap to be all -1 - used for section generation
        GenColumn (useSectionLoader);
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
		int section = (y / sectionSize) - 1;
		sections [section].SetBlock (x, y, z, type);
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
                LightGenerator lightGen = new LightGenerator();
                lightGen.GenerateDaylight(this);
                lightGen.SpreadDaylight(this);
                //SetHeightMap();
				//GenerateDayLightData();

				for( int i = 0; i < sections.Length; i ++) {
					sections[i].update = true;
				}
				update=false;
				updateLight = false;
				changeDayLight = false;
			}
			if (this.updateLight) {

                LightGenerator lightGen = new LightGenerator();
                lightGen.GenerateDaylight(this);
                lightGen.SpreadDaylight(this);
                //GenerateDayLightData();
				
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
        heightMap = new int[sectionSize,sectionSize];

        for (int i = 0; i < sectionSize; i++) {
            for (int j = 0; j < sectionSize; j++) {
                heightMap[i,j] = 0;
            }
        }
        maxHeight = 0;
        minHeight = 0;
        
        int len = sections.Length;
        
        for (int x = 0; x < sectionSize; x++) 
        {
            for (int z = 0; z < sectionSize; z++) 
            {
                for (int s = len - 1; s >= 0; s--)
                {
                    Section section = sections[s];
                    for (int y = sectionSize - 1; y >= 0; y--) 
                    {
                        if (section.data[x, y, z] != 0)
                        {
                            int posY =  y + (sectionSize * len);
                            heightMap[x, z] = posY;
                            if (posY > maxHeight) maxHeight = posY;
                            if (posY < minHeight) minHeight = posY;
                            break;
                        }
                    }
                }
            }
		}
	}

    public void SetHeightMapMaxMin()
    {
        maxHeight = 0;
        minHeight = 0;

        for (int x=0; x<sectionSize; x++)
        {
            for (int z=0; z<sectionSize; z++)
            {
                var h = heightMap [x, z];
                if (h > maxHeight) 
                    maxHeight = h;
                if (h < minHeight) 
                    minHeight = h;
            }
        }
    }

    public void DefaultHeightMap() {
    
        heightMap = new int[sectionSize,sectionSize];
        
        for (int i = 0; i < sectionSize; i++) {
            for (int j = 0; j < sectionSize; j++) {
            	heightMap[i,j] = -1;
            }
        }
    }


		
	/*void SpreadDaylightFromXSection(int chunk_X, int from_x, int to_x, int sectionY) {
		if (IsSection (chunkX, chunkZ, sectionY)) {
			Section section = world.chunks[chunkX, chunkZ].sections[sectionY];
			byte level = 0;

			for (int y = 0; y < sectionSize; y++) {
				for (int z = 0; z < sectionSize; z++) {
					if (section.daylightData != null) {
						level = section.daylightData[from_x,y,z];
						if (level >= 3) {
							level -= 2;
							SpreadDaylight(to_x, y, z, level, section);
						}
					}
				}
			}
		}
	}

	
	void SpreadDaylightFromZSection(int chunk_Z, int from_z, int to_z, int sectionY) {
		
		if (IsSection (chunkX, chunk_Z, sectionY)) {
			Section section = world.chunks[chunkX, chunkZ].sections[sectionY];
			byte level = 0;

			for (int y = 0; y < worldY; y++) {
				for (int x = 0; x < sectionSize; x++) {
					if (section.daylightData != null) {
						level = section.daylightData[x,y,from_z];
						if (level >= 3) {
							level -= 2;
							SpreadDaylight(x, y, to_z, level, section);
						}
					}
				}
			}
		}
	}*/

	public byte Block (int x, int y, int z)
	{
        int sectionIndex = y / sectionSize;
        int sectionY = y % sectionSize;

		if ( !IsInChunk(x, y, z)) {

			Chunk neighbour = GetNeighbouringChunk(x, y, z);
			if (neighbour == null) return 1;
            return neighbour.sections[sectionIndex].data[GetNeighbourLocal(x), sectionY, GetNeighbourLocal(z)];
		}

        return sections[sectionIndex].data[x,sectionY,z];
	}


    public byte LightBLock (int x, int y, int z)
    {
        int sectionIndex = y / sectionSize;
        int sectionY = y % sectionSize;
        
        if ( !IsInChunk(x, y, z)) {
            
            Chunk neighbour = GetNeighbouringChunk(x, y, z);
            if (neighbour == null) return 1;
            return neighbour.sections[sectionIndex].daylightData[GetNeighbourLocal(x), sectionY, GetNeighbourLocal(z)];
        }
        
        return sections[sectionIndex].daylightData[x,sectionY,z];
    }

    /// <summary>
    /// Does x, y, z fall within the chunk bounds
    /// </summary>
    /// <returns></returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="z">The z coordinate.</param>
    public bool IsInChunk (int x, int y, int z)
    {
        if( x>=sectionSize || x<0 || y>=world.worldY || y<0 || z>=sectionSize || z<0)
        {
            return false;
        }
        return true;
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


	public void GenColumn(bool useSectionLoader){
		

		
	}
}
