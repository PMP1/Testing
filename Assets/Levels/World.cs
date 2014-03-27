using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {

	public byte[,,] data;
	public int worldX=16;
	public int worldY=16;
	public int worldZ=16;

	public GameObject chunk;
	public GameObject chunkColumn;
	public ChunkColumn[,] chunkColumns; 
	public int chunkSize=16;


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
		
		chunkColumns = new ChunkColumn[Mathf.FloorToInt(worldX/chunkSize),
		                       Mathf.FloorToInt(worldZ/chunkSize)];
	}


	
	// Update is called once per frame
	void Update () {
	
	}

	public byte[,,] GenData(int xpos, int zpos) {

		byte[,,] colData = new byte[worldX,worldY,worldZ];

		int newX = xpos * chunkSize;
		int newZ = zpos * chunkSize;

		for (int x=0; x<chunkSize; x++){
			for (int z=0; z<chunkSize; z++){
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

	/*public byte Block(int x, int y, int z){
		
		if( x>=worldX || x<0 || y>=worldY || y<0 || z>=worldZ || z<0)
		{
			return (byte)0;
		}
		
		return data[x,y,z];
	}*/

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
		GameObject newChunkColumn= Instantiate(chunkColumn,new Vector3(x*chunkSize-0.5f,
		                                                   0*chunkSize+0.5f,
		                                                   z*chunkSize-0.5f),new Quaternion(0,0,0,0)) as GameObject;

		chunkColumns [x, z] = newChunkColumn.GetComponent("ChunkColumn") as ChunkColumn;
		chunkColumns [x, z].chunkX=x;
		chunkColumns [x, z].chunkZ=z;
		chunkColumns [x, z].chunkY=worldY;
		chunkColumns [x, z].worldGO=gameObject;
		chunkColumns [x, z].data=GenData(x, z);


	}
	
	public void UnloadColumn(int x, int z){

		Object.Destroy(chunkColumns [x, z].gameObject);
	}

	public int SetDaylight() {
		//midday 12 am
		return 1;
	}
}
