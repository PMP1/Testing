using UnityEngine;
using System.Collections;

public class ModifyTerrain : MonoBehaviour {

	World world;
	GameObject cameraGO;

	// Use this for initialization
	void Start () {

		//This is where the innitial load is done!
		System.DateTime start = System.DateTime.Now;

		world=gameObject.GetComponent("World") as World;
		cameraGO=GameObject.FindGameObjectWithTag("MainCamera");




		LoadChunks(GameObject.FindGameObjectWithTag("Player").transform.position,40,256, false); //needs to load at 256

		//Need to call an inittial set of chunk updates here so that the lighting is rendered
		/*for(int x = 0; x < world.chunks.GetLength(0); x++){ 
			for(int z = 0; z < world.chunks.GetLength(1); z++){ 
				if (world.chunks[x, z])
					world.chunks[x, z].DoUpdate();
			}
		}*/


		//this will then give us a true start up time
		//
		//update chunks internal light
		//then update light from other chunks? good idea phil!!
		world.startupTime = System.DateTime.Now.Subtract (start);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			ReplaceBlockCursor(0);
		}
		
		if(Input.GetMouseButtonDown(1)){
			AddBlockCursor(1);
		}

		//LoadChunks(GameObject.FindGameObjectWithTag("Player").transform.position,100,180, true);
	}

	public void LoadChunks(Vector3 playerPos, float distToLoad, float distToUnload, bool useSectionLoader){


		for(int x=0;x<world.chunks.GetLength(0);x++){
			for(int z=0;z<world.chunks.GetLength(1);z++){
				
				float dist=Vector2.Distance(new Vector2(x*world.sectionSize,
				                                        z*world.sectionSize),new Vector2(playerPos.x,playerPos.z));
				
				if(dist<distToLoad){

                    world.chunkManager.LoadChunk(x, z);
                    //world.chunkRenderer.RenderChunk(world.chunkManager.GetChunk(x, z));
					/*if(world.chunks[x,z]==null){
						world.GenColumn(x,z, dist, useSectionLoader);
						if (x - 1 > 0 && world.chunks[x - 1, z]) {
							world.chunks[x - 1, z].update = true;
						}
						if (z - 1 > 0 && world.chunks[x, z - 1]) {
							world.chunks[x, z - 1].update = true;
						}
						if (x + 1 >= world.worldX && world.chunks[x + 1, z]) {
							world.chunks[x + 1, z].update = true;
						}
						if (z + 1 >= world.worldZ && world.chunks[x, z + 1]) {
							world.chunks[x, z + 1].update = true;
						}
					}
					//if (dist <= 32 ) {
					//	world.chunks[x, z].useCollisionMatrix = true;
					//}
				} else if(dist>distToUnload){
					if(world.chunks[x,z]!=null){
						
						world.UnloadColumn(x,z);
					}*/
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
		int updateX= Mathf.FloorToInt( x/world.sectionSize);
		int updateY= Mathf.FloorToInt( y/world.sectionSize);
		int updateZ= Mathf.FloorToInt( z/world.sectionSize);

		int blockPosX = x % world.sectionSize;
		int blockPosZ = z % world.sectionSize;
        int sectionY = y / world.sectionSize;
        int sectionPosY = y % world.sectionSize;
		world.chunks [updateX, updateZ].sections[sectionY].data[blockPosX,sectionPosY,blockPosZ] = block;

		print("Updating: " + updateX + ", " + updateY + ", " + updateZ);

		world.chunks[updateX, updateZ].update=true;
        world.chunks [updateX, updateY].updateHeightMap = true;

		if(x-(world.sectionSize*updateX)==0 && updateX!=0){
			world.chunks[updateX-1, updateZ].update=true;
		}
		
		if(x-(world.sectionSize*updateX)==15 && updateX!=world.chunks.GetLength(0)-1){
			world.chunks[updateX+1, updateZ].update=true;
		}
		
		if(y-(world.sectionSize*updateY)==0 && updateY!=0){
			world.chunks[updateX, updateZ].update=true;
		}
		
		if(y-(world.sectionSize*updateY)==15 && updateY!=(world.worldZ/world.sectionSize)-1){
			world.chunks[updateX, updateZ].update=true;
		}
		
		if(z-(world.sectionSize*updateZ)==0 && updateZ!=0){
			world.chunks[updateX, updateZ-1].update=true;
		}
		
		if(z-(world.sectionSize*updateZ)==15 && updateZ!=world.chunks.GetLength(1)-1){
			world.chunks[updateX, updateZ+1].update=true;
		}
	}


}
