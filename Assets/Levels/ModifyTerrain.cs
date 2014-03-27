using UnityEngine;
using System.Collections;

public class ModifyTerrain : MonoBehaviour {

	World world;
	GameObject cameraGO;

	// Use this for initialization
	void Start () {
		world=gameObject.GetComponent("World") as World;
		cameraGO=GameObject.FindGameObjectWithTag("MainCamera");
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			ReplaceBlockCursor(0);
		}
		
		if(Input.GetMouseButtonDown(1)){
			AddBlockCursor(1);
		}

		LoadChunks(GameObject.FindGameObjectWithTag("Player").transform.position,32,48);
	}

	public void LoadChunks(Vector3 playerPos, float distToLoad, float distToUnload){


		for(int x=0;x<world.chunkColumns.GetLength(0);x++){
			for(int z=0;z<world.chunkColumns.GetLength(1);z++){
				
				float dist=Vector2.Distance(new Vector2(x*world.chunkSize,
				                                        z*world.chunkSize),new Vector2(playerPos.x,playerPos.z));
				
				if(dist<distToLoad){
					if(world.chunkColumns[x,z]==null){
						world.GenColumn(x,z);
						if (x - 1 > 0 && world.chunkColumns[x - 1, z]) {
							world.chunkColumns[x - 1, z].update = true;
						}
						if (z - 1 > 0 && world.chunkColumns[x, z - 1]) {
							world.chunkColumns[x, z - 1].update = true;
						}
						if (x + 1 >= world.worldX && world.chunkColumns[x + 1, z]) {
							world.chunkColumns[x + 1, z].update = true;
						}
						if (z + 1 >= world.worldZ && world.chunkColumns[x, z + 1]) {
							world.chunkColumns[x, z + 1].update = true;
						}
					}
				} else if(dist>distToUnload){
					if(world.chunkColumns[x,z]!=null){
						
						world.UnloadColumn(x,z);
					}
				}
			}
		}
	}

	public void ReplaceBlockCenter(float range, byte block){
		//Replaces the block directly in front of the player
		Ray ray = new Ray(cameraGO.transform.position, cameraGO.transform.forward);
		RaycastHit hit;
		
		if (Physics.Raycast (ray, out hit)) {
			
			if(hit.distance<range){
				ReplaceBlockAt(hit, block);
			}
		}
	}
	
	public void AddBlockCenter(float range, byte block){
		//Adds the block specified directly in front of the player
		Ray ray = new Ray(cameraGO.transform.position, cameraGO.transform.forward);
		RaycastHit hit;
		
		if (Physics.Raycast (ray, out hit)) {
			
			if(hit.distance<range){
				AddBlockAt(hit,block);
			}
			Debug.DrawLine(ray.origin,ray.origin+( ray.direction*hit.distance),Color.green,2);
		}
	}
	
	public void ReplaceBlockCursor(byte block){
		//Replaces the block specified where the mouse cursor is pointing
		
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		
		if (Physics.Raycast (ray, out hit)) {
			
			ReplaceBlockAt(hit, block);
			Debug.DrawLine(ray.origin,ray.origin+( ray.direction*hit.distance),
			               Color.green,2);
			
		}
	}
	
	public void AddBlockCursor( byte block){
		///Adds the block specified where the mouse cursor is pointing
		
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		
		if (Physics.Raycast (ray, out hit)) {
			
			AddBlockAt(hit, block);
			Debug.DrawLine(ray.origin,ray.origin+( ray.direction*hit.distance),
			               Color.green,2);
		}
	}
	
	public void ReplaceBlockAt(RaycastHit hit, byte block) {
		//removes a block at these impact coordinates, you can raycast against the terrain and call this with the hit.point
		Vector3 position = hit.point;
		position+=(hit.normal*-0.5f);
		
		SetBlockAt(position, block);
	}
	
	public void AddBlockAt(RaycastHit hit, byte block) {
		//adds the specified block at these impact coordinates, you can raycast against the terrain and call this with the hit.point
		Vector3 position = hit.point;
		position+=(hit.normal*0.5f);
		
		SetBlockAt(position,block);
	}
	
	public void SetBlockAt(Vector3 position, byte block) {
		//sets the specified block at these coordinates
		int x= Mathf.RoundToInt( position.x );
		int y= Mathf.RoundToInt( position.y );
		int z= Mathf.RoundToInt( position.z );
		
		SetBlockAt(x,y,z,block);
	}
	
	public void SetBlockAt(int x, int y, int z, byte block) {
		//adds the specified block at these coordinates
		print("Adding: " + x + ", " + y + ", " + z);

		//world.data[x,y,z]=block;
		UpdateChunkAt(x,y,z, block);

	}
	
	public void UpdateChunkAt(int x, int y, int z, byte block){
		//Updates the chunk containing this block
		int updateX= Mathf.FloorToInt( x/world.chunkSize);
		int updateY= Mathf.FloorToInt( y/world.chunkSize);
		int updateZ= Mathf.FloorToInt( z/world.chunkSize);

		int blockPosX = x % world.chunkSize;
		int blockPosZ = z % world.chunkSize;
		world.chunkColumns [updateX, updateZ].data[blockPosX,y,blockPosZ] = block;

		print("Updating: " + updateX + ", " + updateY + ", " + updateZ);

		world.chunkColumns[updateX, updateZ].update=true;

		if(x-(world.chunkSize*updateX)==0 && updateX!=0){
			world.chunkColumns[updateX-1, updateZ].update=true;
		}
		
		if(x-(world.chunkSize*updateX)==15 && updateX!=world.chunkColumns.GetLength(0)-1){
			world.chunkColumns[updateX+1, updateZ].update=true;
		}
		
		if(y-(world.chunkSize*updateY)==0 && updateY!=0){
			world.chunkColumns[updateX, updateZ].update=true;
		}
		
		if(y-(world.chunkSize*updateY)==15 && updateY!=(world.worldZ/world.chunkSize)-1){
			world.chunkColumns[updateX, updateZ].update=true;
		}
		
		if(z-(world.chunkSize*updateZ)==0 && updateZ!=0){
			world.chunkColumns[updateX, updateZ-1].update=true;
		}
		
		if(z-(world.chunkSize*updateZ)==15 && updateZ!=world.chunkColumns.GetLength(1)-1){
			world.chunkColumns[updateX, updateZ+1].update=true;
		}
	}


}
