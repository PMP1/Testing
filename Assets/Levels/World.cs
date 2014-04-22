using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {

	public byte[,,] data;
	public int worldX=16;
	public int worldY=16;
	public int worldZ=16;

	public GameObject section;
	public GameObject chunk;
	public Chunk[,] chunks; 
	public int sectionSize=16;

	public TimeManager time;


	// Use this for initialization
	void Awake() {

		chunks = new Chunk[Mathf.FloorToInt(worldX/sectionSize),
		                               Mathf.FloorToInt(worldZ/sectionSize)];
	}


	
	// Update is called once per frame
	void Update () {
	
	}

	public byte GenBiome(int xpos, int zpos) {
		int scale = 10;
		int result = PerlinNoise(xpos,10,zpos,scale,10);

		//1 = mountin
		if (result >= 5) {
			return (byte)1;
		}

		//0 = plain
		return (byte)0;
	}

	public byte[,,] GenData(int xpos, int zpos, byte biome) {

		byte[,,] colData = new byte[sectionSize,worldY,sectionSize];

		byte[,] baseHeightData = new byte[sectionSize,sectionSize];
		byte[,] heightData = new byte[sectionSize,sectionSize];

		int newX = xpos * sectionSize;
		int newZ = zpos * sectionSize;

		//set heightmap
		for (int x=0; x<sectionSize; x++) {
			for (int z=0; z<sectionSize; z++) {
				baseHeightData[x,z]=(byte)(PerlinNoise(x + newX,0,z + newZ,30,10) + 20);
				heightData[x,z]=(byte)(PerlinNoise(x + newX,0,z + newZ,60,40) + 20);
			}
		}
		

		for (int x=0; x<sectionSize; x++) {
			for (int z=0; z<sectionSize; z++) {
				for (int y=0; y<worldY; y++) {
					if (heightData[x,z] > y) {
						colData[x,y,z]= (byte)(biome + 1);
						continue;
					}
				}
			}
		}

		for (int x=0; x<sectionSize; x++) {
			for (int z=0; z<sectionSize; z++) {
				for (int y=baseHeightData[x,z]; y<worldY; y++) {

					int stone=PerlinNoise(x + newX,y,z + newZ,50,20) + 20;
					
					if (stone > 31)
						colData[x,y,z]=0;
				}
			}
		}



		/*for (int x=0; x<sectionSize; x++){
			for (int z=0; z<sectionSize; z++){

				//flat and nice
				//int stone=PerlinNoise(x + newX,10,z + newZ,50,10,1);

				int stone=PerlinNoise(x + newX+ 40,10,z + newZ,50,40,0);
				//stone-=PerlinNoise(x + newX,14,z + newZ,10,50,0);
					/*if (stone > 5)
					colData[x,y,z]=2;*/
				/*int stone=PerlinNoise(x + newX,0,z + newZ,80,20,1.2f);
				stone+= PerlinNoise(x + newX,200,z + newZ,20,30,0.5f)+0;
				int dirt=PerlinNoise(x + newX,300,z + newZ,50,2,0) +1; //Added +1 to make sure minimum grass height is 1
				*/

				/*for (int y=0; y<worldY; y++){
					if(y<=stone){
						colData[x,y,z]=2;
					} 
				}
			}
		}*/
		return colData;
	}

	int PerlinNoise(int x,int y, int z, float scale, float height){
		float rValue;

		rValue=Noise.Noise.GetNoise (((double)x) / scale, ((double)y)/ scale, ((double)z) / scale);
		rValue*=height;

		return (int) rValue;
	}

	public void GenColumn(int x, int z){

		//generate a chunk column
		GameObject newChunkColumn= Instantiate(chunk,new Vector3(x*sectionSize-0.5f,
		                                                               0*sectionSize+0.5f,
		                                                               z*sectionSize-0.5f),new Quaternion(0,0,0,0)) as GameObject;

		chunks [x, z] = newChunkColumn.GetComponent("Chunk") as Chunk;
		chunks [x, z].chunkX=x;
		chunks [x, z].chunkZ=z;
		chunks [x, z].worldY=worldY;
		chunks [x, z].worldGO=gameObject;
		chunks [x, z].biome = GenBiome (x, z);
		chunks [x, z].data = GenData(x, z, chunks [x, z].biome);
	}
	
	public void UnloadColumn(int x, int z){

		Object.Destroy(chunks [x, z].gameObject);
	}

	public int SetDaylight() {
		//midday 12 am
		return 1;
	}
}
