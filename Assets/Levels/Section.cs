using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Section : MonoBehaviour
{

	public GameObject worldGO;
	public World world;

	public GameObject chunkGO;
	public Chunk chunk;

	private List<Vector3> newVertices = new List<Vector3> ();
	private List<int> newTriangles = new List<int> ();
	private List<Vector2> newUV = new List<Vector2> ();

	private List<Vector3> newColliderVertices = new List<Vector3> ();
	private List<int> newColliderTriangles = new List<int> ();
	
	//private List<Vector2> newUV2 = new List<Vector2> ();
	private List<Color> newColor = new List<Color> ();
	private float tUnit = 0.25f;
	private Vector2 tStone = new Vector2 (1, 0);
	private Vector2 tGrass = new Vector2 (0, 1);
	private Vector2 tGrassTop = new Vector2 (1, 1);
	private Vector2 t1 = new Vector2 (2, 0);
	private Vector2 t2 = new Vector2 (2, 1);
	private Vector2 t3 = new Vector2 (2, 2);
	private Vector2 t4 = new Vector2 (2, 3);
	
	private Mesh mesh;
	private MeshCollider col;
	private int faceCount;
	private int colliderFaceCount;
	public int sectionSize = 16;
	public int sectionX;
	public int sectionY;
	public int sectionZ;
		
	public bool update;
	public bool lightUpdate;

	// Use this for initialization
	void Start ()
	{ 
		mesh = GetComponent<MeshFilter> ().mesh;
		col = GetComponent<MeshCollider> ();

		GenerateMesh ();
	}

	// Update is called once per frame
	void Update ()
	{

	}

	void LateUpdate () {
		if (update) {
			GenerateMesh ();
			update = false;
		} else if (lightUpdate) {
			GenerateLight ();
			lightUpdate = false;
		}
	}

	public void GenerateMesh ()
	{
		bool [,] meshTop = new bool[sectionSize,sectionSize];
		bool [,] meshBottom = new bool[sectionSize,sectionSize];
		bool [,] meshEast = new bool[sectionSize,sectionSize];
		bool [,] meshWest = new bool[sectionSize,sectionSize];
		bool [,] meshNorth = new bool[sectionSize,sectionSize];
		bool [,] meshSouth = new bool[sectionSize,sectionSize];

		for (int y=0; y<sectionSize; y++) {
			for (int x=0; x<sectionSize; x++) {
				for (int z=0; z<sectionSize; z++) {
					if (Block (x, y, z) != 0) {
						if (Block (x, y + 1, z) <= 0) {
							meshTop[x, z] = true;
						}
						if (Block (x, y - 1, z) <= 0) {
							meshBottom[x, z] = true;
						}
					}
				}
			}
			CullCollisionMatrix(meshTop, y, 0);
			CullCollisionMatrix(meshBottom, y, 1);
		}

		for (int x=0; x<sectionSize; x++) {
			for (int y=0; y<sectionSize; y++) {
				for (int z=0; z<sectionSize; z++) {
					if (Block (x, y, z) != 0) {
						if (Block (x + 1, y, z) <= 0) {
							meshEast[y, z] = true;
						}
						if (Block (x - 1, y, z) <= 0) {
							meshBottom[y, z] = true;
						}
					}
				}
			}
			CullCollisionMatrix(meshEast, x, 3);
			CullCollisionMatrix(meshBottom, x, 5);
		}

		for (int z=0; z<sectionSize; z++) {
			for (int x=0; x<sectionSize; x++) {
				for (int y=0; y<sectionSize; y++) {
					if (Block (x, y, z) != 0) {
						if (Block (x, y, z + 1) <= 0) {
							meshNorth[y, x] = true;
						}
						if (Block (x, y, z - 1) <= 0) {
							meshSouth[y, x] = true;
						}
					}
				}
			}
			CullCollisionMatrix(meshNorth, z, 2);
			CullCollisionMatrix(meshSouth, z, 4);
		}
		
		
		
	
	
		for (int x=0; x<sectionSize; x++) {
			for (int y=0; y<sectionSize; y++) {
				for (int z=0; z<sectionSize; z++) {
					//This code will run for every block in the section
					if (Block (x, y, z) != 0) {
						if (Block (x, y + 1, z) <= 0) {
							//Block above is air
							CubeTop (x, y, z, Block (x, y, z));
						}

						if (Block (x, y - 1, z) <= 0) {
							//Block below is air
							CubeBot (x, y, z, Block (x, y, z));
						}

						if (Block (x + 1, y, z) <= 0 ) {
							//Block east is air
							CubeEast (x, y, z, Block (x, y, z));
						}

						if (Block (x - 1, y, z) <= 0) {
							//Block west is air
							CubeWest (x, y, z, Block (x, y, z));
						}

						if (Block (x, y, z + 1) <= 0) {
							//Block north is air
							CubeNorth (x, y, z, Block (x, y, z));
						}

						if (Block (x, y, z - 1) <= 0) {
							//Block south is air
							CubeSouth (x, y, z, Block (x, y, z));
						}
					}
				}
			}
		}
		UpdateMesh ();
	}

	byte Block (int x, int y, int z)
	{
		return chunk.Block (x, y + sectionY, z);
	}

	byte LightBlock (int x, int y, int z)
	{
		byte l = chunk.LightBlock (x, y + sectionY, z);
		byte w = world.time.GetDaylightLevel();
		return (byte)Mathf.Max (l - w, 0);
	}
	
	private Vector2 GetTexture(int type) {
		switch (type) {
		
			case 1: 
				return t1;
			case 2: 
				return t2;
			case 3: 
				return t3;
			case 4: 
				return t4;
			default:
				return tStone;		
		}
	}

	void CubeTop (int x, int y, int z, byte block)
	{

		newVertices.Add (new Vector3 (x, y, z + 1));
		newVertices.Add (new Vector3 (x + 1, y, z + 1));
		newVertices.Add (new Vector3 (x + 1, y, z));
		newVertices.Add (new Vector3 (x, y, z));

		Vector2 texturePos = new Vector2 (0, 0);
		texturePos = GetTexture(Block (x, y, z));
		

		Cube (texturePos, LightBlock(x, y + 1, z));

	}

	void CubeNorth (int x, int y, int z, byte block)
	{

		newVertices.Add (new Vector3 (x + 1, y - 1, z + 1));
		newVertices.Add (new Vector3 (x + 1, y, z + 1));
		newVertices.Add (new Vector3 (x, y, z + 1));
		newVertices.Add (new Vector3 (x, y - 1, z + 1));

		Vector2 texturePos = new Vector2 (0, 0);
		texturePos = GetTexture(Block (x, y, z));

		Cube (texturePos, LightBlock(x, y, z + 1));

	}

	void CubeEast (int x, int y, int z, byte block)
	{
		newVertices.Add (new Vector3 (x + 1, y - 1, z));
		newVertices.Add (new Vector3 (x + 1, y, z));
		newVertices.Add (new Vector3 (x + 1, y, z + 1));
		newVertices.Add (new Vector3 (x + 1, y - 1, z + 1));

		Vector2 texturePos = new Vector2 (0, 0);
		texturePos = GetTexture(Block (x, y, z));

		Cube (texturePos, LightBlock(x + 1, y, z));

	}

	void CubeSouth (int x, int y, int z, byte block)
	{

		newVertices.Add (new Vector3 (x, y - 1, z));
		newVertices.Add (new Vector3 (x, y, z));
		newVertices.Add (new Vector3 (x + 1, y, z));
		newVertices.Add (new Vector3 (x + 1, y - 1, z));

		Vector2 texturePos = new Vector2 (0, 0);
		texturePos = GetTexture(Block (x, y, z));

		Cube (texturePos, LightBlock(x, y, z - 1));

	}

	void CubeWest (int x, int y, int z, byte block)
	{

		newVertices.Add (new Vector3 (x, y - 1, z + 1));
		newVertices.Add (new Vector3 (x, y, z + 1));
		newVertices.Add (new Vector3 (x, y, z));
		newVertices.Add (new Vector3 (x, y - 1, z));

		Vector2 texturePos = new Vector2 (0, 0);
		texturePos = GetTexture(Block (x, y, z));

		Cube (texturePos,  LightBlock(x - 1, y, z));

	}

	void CubeBot (int x, int y, int z, byte block)
	{

		newVertices.Add (new Vector3 (x, y - 1, z));
		newVertices.Add (new Vector3 (x + 1, y - 1, z));
		newVertices.Add (new Vector3 (x + 1, y - 1, z + 1));
		newVertices.Add (new Vector3 (x, y - 1, z + 1));

		Vector2 texturePos = new Vector2 (0, 0);
		texturePos = GetTexture(Block (x, y, z));

		Cube (texturePos,  LightBlock(x, y - 1, z));

	}

	void Cube (Vector2 texturePos, byte lightLevel)
	{
		newTriangles.Add (faceCount * 4); //1
		newTriangles.Add (faceCount * 4 + 1); //2
		newTriangles.Add (faceCount * 4 + 2); //3
		newTriangles.Add (faceCount * 4); //1
		newTriangles.Add (faceCount * 4 + 2); //3
		newTriangles.Add (faceCount * 4 + 3); //4

		newUV.Add (new Vector2 (tUnit * texturePos.x + tUnit, tUnit * texturePos.y));
		newUV.Add (new Vector2 (tUnit * texturePos.x + tUnit, tUnit * texturePos.y + tUnit));
		newUV.Add (new Vector2 (tUnit * texturePos.x, tUnit * texturePos.y + tUnit));
		newUV.Add (new Vector2 (tUnit * texturePos.x, tUnit * texturePos.y));

		CubeLight (lightLevel);

		faceCount++; // Add this line
	}

	void CubeLight(byte lightLevel) {
		//Vector2 lightUV = new Vector2(lightLevel / 4, lightLevel % 4	);
		//inversed y!
		/*newUV2.Add (new Vector2 (tUnit * lightUV.x + tUnit, tUnit * lightUV.y));
		newUV2.Add (new Vector2 (tUnit * lightUV.x + tUnit, tUnit * lightUV.y + tUnit));
		newUV2.Add (new Vector2 (tUnit * lightUV.x, tUnit * lightUV.y + tUnit));
		newUV2.Add (new Vector2 (tUnit * lightUV.x, tUnit * lightUV.y));
		*/
		newColor.Add(new Color(lightLevel/16f,lightLevel/16f,lightLevel/16f,0.5f));
		newColor.Add(new Color(lightLevel/16f,lightLevel/16f,lightLevel/16f,0.5f));
		newColor.Add(new Color(lightLevel/16f,lightLevel/16f,lightLevel/16f,0.5f));
		newColor.Add(new Color(lightLevel/16f,lightLevel/16f,lightLevel/16f,0.5f));
	}

	void UpdateMesh ()
	{
		mesh.Clear ();
		mesh.vertices = newVertices.ToArray ();
		mesh.uv = newUV.ToArray ();
		//mesh.uv2 = newUV2.ToArray ();
		mesh.colors = newColor.ToArray();
		mesh.triangles = newTriangles.ToArray ();
		mesh.Optimize ();
		mesh.RecalculateNormals ();

		col.sharedMesh = null;
		//col.sharedMesh = mesh;

		//MeshCollider myMC = GetComponent<MeshCollider>();
		Mesh newMesh = new Mesh();
		//newMesh  = new Mesh();
		newMesh.vertices  = newColliderVertices.ToArray();
		newMesh.triangles = newColliderTriangles.ToArray();
		newMesh.RecalculateBounds();
		col.sharedMesh = newMesh;
		colliderFaceCount = 0;



		newVertices.Clear ();
		newUV.Clear ();
		//newUV2.Clear ();
		newColor.Clear();
		newTriangles.Clear ();
		faceCount = 0;
		
		
	}

	//TODO finish generating light update, need to tests agianst non light update
	public void GenerateLight ()
	{
		byte lightLevel;
		for (int x=0; x<sectionSize; x++) {
			for (int y=0; y<sectionSize; y++) {
				for (int z=0; z<sectionSize; z++) {
					if (Block (x, y, z) != 0) {
						if (Block (x, y + 1, z) <= 0) {
							//Block above is air
							CubeLight (LightBlock(x, y + 1, z));
						}
						
						if (Block (x, y - 1, z) <= 0) {
							//Block below is air
							CubeLight (LightBlock(x, y - 1, z));
						}
						
						if (Block (x + 1, y, z) <= 0 ) {
							//Block east is air
							CubeLight (LightBlock(x + 1, y, z));
						}
						
						if (Block (x - 1, y, z) <= 0) {
							//Block west is air
							CubeLight (LightBlock(x - 1, y, z));
						}
						
						if (Block (x, y, z + 1) <= 0) {
							//Block north is air
							CubeLight (LightBlock(x, y, z + 1));
						}
						
						if (Block (x, y, z - 1) <= 0) {
							//Block south is air
							CubeLight (LightBlock(x, y, z - 1));
						}
					}
				}
			}
		}
		UpdateMeshLight ();
	}

	void UpdateMeshLight ()
	{
		//mesh.uv2 = newUV2.ToArray ();
		//newUV2.Clear ();
		
		mesh.colors = newColor.ToArray ();
		newColor.Clear ();
	}
	
	// top 0 bottom 1 N 2 E 3 S 4 W 5
	
	bool[,] CullCollisionMatrix(bool[,] mask, int dim, int type) {
		// this just sets up an example 2D plane for testing purposes
		int size = 16; // 7 x 7 plane 
		
		
		/* =new bool[,] {
			{true, false, false,false, true, false,true, false, false,false, false, false,false, false, false, false},
			{true, false, false,false, true, false,false, false, false,false, false, false,false, false, false, false},
			{false, false, false,false, true, false,false, false, false,false, false, false,false, false, false, false},
			{false, false, false,false, false, false,false, false, false,false, false, false,false, false, false, false},
			{false, false, false,false, false, false,false, false, false,false, false, false,false, false, false, false},
			{false, false, false,false, false, false,false, false, false,false, false, false,false, false, false, false},
			{false, false, false,false, false, false,false, false, false,false, false, false,false, false, false, false},
			{false, false, false,false, false, false,false, false, false,false, false, false,false, false, false, false},
			{false, false, false,false, false, false,false, false, false,false, false, false,false, false, false, false},
			{false, false, false,false, false, false,false, false, false,false, false, false,false, false, false, false},
			{false, false, false,false, false, false,false, false, false,false, false, false,false, false, false, false},
			{false, false, false,false, false, false,false, false, false,false, false, false,false, false, false, false},
			{false, false, false,false, false, false,false, false, false,false, false, false,false, false, false, false},
			{false, false, false,false, false, false,false, false, false,false, false, false,false, false, false, false},
			{false, false, false,false, false, false,false, false, false,false, false, false,false, false, false, false},
			{true, false, false,false, false, false,false, false, false,false, false, false,false, false, false, false}
		};*/

		int i = 0; //start z
		int j = 0; //end z
		int h = 0;
		bool building;
		bool done = false;
		
		for (int x = 0; x < size; x ++) 
		{
			building = false;
			for (int z = 0; z < size; z ++) 
			{
				if (mask[x, z] == true && !building) // start recording a new rectangle
				{
					building = true;
					i = z; //start Z pos
				}
				// if you reach a block that needs no face, or you reach the end of the row, you're done
				if ((mask[x, z] == false && building) || (z == size-1 && building))
				{
					if (mask[x, z]==false && building) // -1 because you've already passed the last block
					{
						j = z - 1;
					}
					else
					{
						j = z;
					}
					building = false;
					done = false;
					h = 1; // height
					
					// look upwards to see if the row above this one needs faces in the exact same spots (width wise)
					// this loop is a little wonky but it works
					
					
					for (int x2 = x+1; x2 < size; x2 ++) 
					{
						for (int z2 = i; z2 <= j; z2 ++) 
						{
							if (mask[x2, z2] == false)
							{
								// cannot expand. get out of the loop.
								done = true;
								break;
							}
						}
						if (done)
							break;
						// we got to the end of the range so we're good! increase height and let's try the next row
						h += 1;
					}
					//# all done with this rectangle
					//# lower left coordinate of the RECTANGLE = x, i
					//# upper right coordinate of the RECTANGLE = x+h, j+1
					//print x, i, j, h	

					switch(type) {

					case 0://Top
						newColliderVertices.Add (new Vector3 (x, dim, j+1));
						newColliderVertices.Add (new Vector3 (x+h , dim, j+1));
						newColliderVertices.Add (new Vector3 (x+h, dim, i));
						newColliderVertices.Add (new Vector3 (x, dim, i));
						break;
					case 1://Bottom
						newColliderVertices.Add (new Vector3 (x, dim - 1, i));
						newColliderVertices.Add (new Vector3 (x+h , dim - 1, i));
						newColliderVertices.Add (new Vector3 (x+h, dim - 1, j+1));
						newColliderVertices.Add (new Vector3 (x, dim - 1, j+1));
						break;
					case 2://North

						newColliderVertices.Add (new Vector3 (j+1, x - 1,  dim + 1));
						newColliderVertices.Add (new Vector3 (j+1, x+h - 1, dim + 1));
						newColliderVertices.Add (new Vector3 (i,  x+h - 1, dim + 1));
						newColliderVertices.Add (new Vector3 (i, x - 1,  dim + 1));


						break;
					case 3: //East
						newColliderVertices.Add (new Vector3 (dim + 1, x - 1, i));
						newColliderVertices.Add (new Vector3 (dim + 1, x+h - 1, i));
						newColliderVertices.Add (new Vector3 (dim + 1, x+h - 1, j+1));
						newColliderVertices.Add (new Vector3 (dim + 1, x - 1, j+1));
						break;
					case 4: //South
						newColliderVertices.Add (new Vector3 (i, x - 1,  dim));
						newColliderVertices.Add (new Vector3 (i,  x+h - 1, dim));
						newColliderVertices.Add (new Vector3 (j+1, x+h - 1, dim));
						newColliderVertices.Add (new Vector3 (j+1, x - 1,  dim));
						break;
					case 5: //West
						newColliderVertices.Add (new Vector3 (dim, x - 1, j+1));
						newColliderVertices.Add (new Vector3 (dim, x+h - 1, j+1));
						newColliderVertices.Add (new Vector3 (dim, x+h - 1, i));
						newColliderVertices.Add (new Vector3 (dim, x - 1, i));

						break;

					}
					newColliderTriangles.Add (colliderFaceCount * 4); //1
					newColliderTriangles.Add (colliderFaceCount * 4 + 1); //2
					newColliderTriangles.Add (colliderFaceCount * 4 + 2); //3
					newColliderTriangles.Add (colliderFaceCount * 4); //1
					newColliderTriangles.Add (colliderFaceCount * 4 + 2); //3
					newColliderTriangles.Add (colliderFaceCount * 4 + 3); //4
					colliderFaceCount ++;				
					// update the mask to show that the spots covered by your rectangle no longer need faces
					for (int x3 = x; x3 < h+x; x3 ++) 
					{
						for (int z3 = i; z3 < j+1; z3 ++) 
						{
							mask[x3, z3] = false;
						}
					}
				}
			}
		}
		return mask;
	}


}