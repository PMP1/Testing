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

		data = new byte[worldX,worldY,worldZ];

		for (int x=0; x<worldX; x++){
			for (int z=0; z<worldZ; z++){
				int stone=PerlinNoise(x,0,z,10,3,1.2f);
				stone+= PerlinNoise(x,300,z,20,4,0)+10;
				int dirt=PerlinNoise(x,100,z,50,2,0) +0; //Added +1 to make sure minimum grass height is 1
				
				for (int y=0; y<worldY; y++){
					if(y<=stone){
						data[x,y,z]=1;
					} else if(y<=dirt+stone){ //Changed this line thanks to a comment
						data[x,y,z]=2;
					}
					
				}
			}
		}
		
		chunks = new Chunk[Mathf.FloorToInt(worldX/sectionSize),
		                               Mathf.FloorToInt(worldZ/sectionSize)];
	}


	
	// Update is called once per frame
	void Update () {
	
	}

	public byte[,,] GenData(int xpos, int zpos) {

		byte[,,] colData = new byte[worldX,worldY,worldZ];

		int newX = xpos * sectionSize;
		int newZ = zpos * sectionSize;

		for (int x=0; x<sectionSize; x++){
			for (int z=0; z<sectionSize; z++){
				int stone=PerlinNoise(x + newX,0,z + newZ,10,3,1.2f);
				stone+= PerlinNoise(x + newX,300,z + newZ,20,4,0)+10;
				int dirt=PerlinNoise(x + newX,100,z + newZ,50,2,0) +0; //Added +1 to make sure minimum grass height is 1
				
				for (int y=0; y<worldY; y++){
					if(y<=stone){
						colData[x,y,z]=1;
					} else if(y<=dirt+stone){ //Changed this line thanks to a comment
						colData[x,y,z]=2;
					}
					
				}
			}
		}
		return colData;
	}

	int PerlinNoise(int x,int y, int z, float scale, float height, float power){
		float rValue;


		rValue=Noise.Noise.GetNoise (((double)x) / scale, ((double)y)/ scale, ((double)z) / scale);

		rValue*=height;
		
		if(power!=0){
			rValue=Mathf.Pow( rValue, power);
		}
		
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
		chunks [x, z].data=GenData(x, z);
	}
	
	public void UnloadColumn(int x, int z){

		Object.Destroy(chunks [x, z].gameObject);
	}

	public int SetDaylight() {
		//midday 12 am
		return 1;
	}
}
