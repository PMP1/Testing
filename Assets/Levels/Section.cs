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
	private Vector2 tStone = new Vector2 (0, 1);
	private Vector2 tGrass = new Vector2 (0, 2);
	private Vector2 tDirt = new Vector2 (0, 3);
	private Vector2 tSand = new Vector2 (1, 2);
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
	public bool updateDayLight;

	public bool useCollisionMatrix;
	private bool hasCollisionMatrix = false;


	// Use this for initialization
	void Start ()
	{ 
		mesh = GetComponent<MeshFilter> ().mesh;
		col = GetComponent<MeshCollider> ();
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
		} else if (updateDayLight) {
			GenerateDayLight();
			updateDayLight = false;
		}
	}



	public void GenerateMesh ()
	{
		if (useCollisionMatrix) {
			GenerateCollisionMesh ();
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

	/// <summary>
	/// Generates the collision mesh.
	/// </summary>
	private void GenerateCollisionMesh ()
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
		hasCollisionMatrix = true;
	}

	byte Block (int x, int y, int z)
	{
		return chunk.Block (x, y + sectionY, z);
	}

	byte LightBlock (int x, int y, int z)
	{
		byte l = chunk.LightBlock (x, y + sectionY, z);
		return (byte)Mathf.Max (l, 0);
	}
	
	private Vector2 GetTexture(int type) {
		switch (type) {
		
			case 1: 
				return tSand;
			case 2: 
				return tGrass;
			case 3: 
				return t3;
			case 4: 
				return tStone;
			default:
				return tDirt;		
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
		
		byte n  = LightBlock (x,   y + 1, z+1);
		byte ne = LightBlock (x+1, y + 1, z+1);
		byte e  = LightBlock (x+1, y + 1, z);
		byte se = LightBlock (x+1, y + 1, z-1);
		byte s  = LightBlock (x,   y + 1, z-1);
		byte sw = LightBlock (x-1, y + 1, z-1);
		byte w  = LightBlock (x-1, y + 1, z);
		byte nw = LightBlock (x-1, y + 1, z+1);
		byte c  = LightBlock (x,   y + 1, z);

		byte ne1 = (byte)((float)(c + n + ne + e) / 4f);
		byte se1 = (byte)((float)(c + s + se + e) / 4f);
		byte sw1 = (byte)((float)(c + s + sw + w) / 4f);
		byte nw1 = (byte)((float)(c + n + nw + w) / 4f);

		Cube (texturePos, ne1, se1, sw1, nw1);
	}

	void CubeNorth (int x, int y, int z, byte block)
	{
		newVertices.Add (new Vector3 (x + 1, y - 1, z + 1));
		newVertices.Add (new Vector3 (x + 1, y, z + 1));
		newVertices.Add (new Vector3 (x, y, z + 1));
		newVertices.Add (new Vector3 (x, y - 1, z + 1));

		Vector2 texturePos = new Vector2 (0, 0);
		texturePos = GetTexture(Block (x, y, z));

		//  |tw|t |te|
		//  |w |c |e |
		//  |bw|b |be|
		byte t  = LightBlock (x,   y+1, z+1); //top
		byte te = LightBlock (x+1, y+1, z+1); //top - east
		byte e  = LightBlock (x+1, y,   z+1);
		byte be = LightBlock (x+1, y-1, z+1);
		byte b  = LightBlock (x,   y-1, z+1);
		byte bw = LightBlock (x-1, y-1, z+1);
		byte w  = LightBlock (x-1, y,   z+1);
		byte tw = LightBlock (x-1, y+1, z+1);
		byte c  = LightBlock (x,   y,   z+1);
		
		byte te1 = (byte)((float)(c + t + te + e) / 4f);
		byte be1 = (byte)((float)(c + b + be + e) / 4f);
		byte bw1 = (byte)((float)(c + b + bw + w) / 4f);
		byte tw1 = (byte)((float)(c + t + tw + w) / 4f);

		Cube (texturePos, te1, tw1 , bw1, be1);
	}

	void CubeEast (int x, int y, int z, byte block)
	{
		newVertices.Add (new Vector3 (x + 1, y - 1, z));
		newVertices.Add (new Vector3 (x + 1, y, z));
		newVertices.Add (new Vector3 (x + 1, y, z + 1));
		newVertices.Add (new Vector3 (x + 1, y - 1, z + 1));

		Vector2 texturePos = new Vector2 (0, 0);
		texturePos = GetTexture(Block (x, y, z));

		//Cube (texturePos, LightBlock(x + 1, y, z));
		//  |ts|t |tn|
		//  |s |c |n |
		//  |bs|b |bn|
		byte t  = LightBlock (x+1,   y+1, z); //top
		byte tn = LightBlock (x+1, y+1, z+1); //top - east
		byte n  = LightBlock (x+1, y,   z+1);
		byte bn = LightBlock (x+1, y-1, z+1);
		byte b  = LightBlock (x+1,   y-1, z);
		byte bs = LightBlock (x+1, y-1, z-1);
		byte s  = LightBlock (x+1, y,   z-1);
		byte ts = LightBlock (x+1, y+1, z-1);
		byte c  = LightBlock (x+1,   y,   z);
		
		byte tn1 = (byte)((float)(c + t + tn + n) / 4f);
		byte bn1 = (byte)((float)(c + b + bn + n) / 4f);
		byte bs1 = (byte)((float)(c + b + bs + s) / 4f);
		byte ts1 = (byte)((float)(c + t + ts + s) / 4f);
		
		Cube (texturePos, ts1, tn1 , bn1, bs1);
	}

	void CubeSouth (int x, int y, int z, byte block)
	{
		newVertices.Add (new Vector3 (x, y - 1, z));
		newVertices.Add (new Vector3 (x, y, z));
		newVertices.Add (new Vector3 (x + 1, y, z));
		newVertices.Add (new Vector3 (x + 1, y - 1, z));

		Vector2 texturePos = new Vector2 (0, 0);
		texturePos = GetTexture(Block (x, y, z));

		//  |tw|t |te|
		//  |w |c |e |
		//  |bw|b |be|
		byte t  = LightBlock (x,   y+1, z-1); //top
		byte te = LightBlock (x+1, y+1, z-1); //top - east
		byte e  = LightBlock (x+1, y,   z-1);
		byte be = LightBlock (x+1, y-1, z-1);
		byte b  = LightBlock (x,   y-1, z-1);
		byte bw = LightBlock (x-1, y-1, z-1);
		byte w  = LightBlock (x-1, y,   z-1);
		byte tw = LightBlock (x-1, y+1, z-1);
		byte c  = LightBlock (x,   y,   z-1);
		
		byte te1 = (byte)((float)(c + t + te + e) / 4f);
		byte be1 = (byte)((float)(c + b + be + e) / 4f);
		byte bw1 = (byte)((float)(c + b + bw + w) / 4f);
		byte tw1 = (byte)((float)(c + t + tw + w) / 4f);
		
		Cube (texturePos, tw1, te1 , be1, bw1);
	}

	void CubeWest (int x, int y, int z, byte block)
	{
		newVertices.Add (new Vector3 (x, y - 1, z + 1));
		newVertices.Add (new Vector3 (x, y, z + 1));
		newVertices.Add (new Vector3 (x, y, z));
		newVertices.Add (new Vector3 (x, y - 1, z));

		Vector2 texturePos = new Vector2 (0, 0);
		texturePos = GetTexture(Block (x, y, z));

		//Cube (texturePos, LightBlock(x + 1, y, z));
		//  |ts|t |tn|
		//  |s |c |n |
		//  |bs|b |bn|
		byte t  = LightBlock (x-1, y+1, z); //top
		byte tn = LightBlock (x-1, y+1, z+1); //top - north
		byte n  = LightBlock (x-1, y,   z+1);
		byte bn = LightBlock (x-1, y-1, z+1);
		byte b  = LightBlock (x-1, y-1, z);
		byte bs = LightBlock (x-1, y-1, z-1);
		byte s  = LightBlock (x-1, y,   z-1);
		byte ts = LightBlock (x-1, y+1, z-1);
		byte c  = LightBlock (x-1, y,   z);
		
		byte tn1 = (byte)((float)(c + t + tn + n) / 4f);
		byte bn1 = (byte)((float)(c + b + bn + n) / 4f);
		byte bs1 = (byte)((float)(c + b + bs + s) / 4f);
		byte ts1 = (byte)((float)(c + t + ts + s) / 4f);
		
		Cube (texturePos, tn1, ts1, bs1, bn1);	
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

	void Cube (Vector2 texturePos, byte ne, byte se, byte sw, byte nw)
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
		
		CubeLight (ne, se, sw, nw);
		
		faceCount++; // Add this line
	}

	void CubeLight(byte ne, byte se, byte sw, byte nw) {

		newColor.Add(new Color(0f,0f,0f,nw/16f));
		newColor.Add(new Color(0f,0f,0f,ne/16f));
		newColor.Add(new Color(0f,0f,0f,se/16f));
		newColor.Add(new Color(0f,0f,0f,sw/16f));
	}

	void CubeLight(byte lightLevel) {

		newColor.Add(new Color(0f,0f,0f,lightLevel/16f));
		newColor.Add(new Color(0f,0f,0f,lightLevel/16f));
		newColor.Add(new Color(0f,0f,0f,lightLevel/16f));
		newColor.Add(new Color(0f,0f,0f,lightLevel/16f));
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


		//Update the collider
		col.sharedMesh = null;

		Mesh newMesh = new Mesh();
		newMesh.vertices  = newColliderVertices.ToArray();
		newMesh.triangles = newColliderTriangles.ToArray();
		newMesh.RecalculateBounds();
		col.sharedMesh = newMesh;
		colliderFaceCount = 0;
		newColliderVertices.Clear ();
		newColliderTriangles.Clear ();

		renderer.material.SetFloat ("_Sun", 0f);

		newVertices.Clear ();
		newUV.Clear ();
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

	/// <summary>
	/// Updates the vertex light colours, rgb is tourch light alpha is sunlight
	/// </summary>
	private void UpdateMeshLight ()
	{
		mesh.colors = newColor.ToArray ();
		newColor.Clear ();
	}

	/// <summary>
	/// Updates the global sun value on this mesh.
	/// </summary>
	private void GenerateDayLight()
	{
		byte w = world.time.GetDaylightLevel();
		renderer.material.SetFloat ("_Sun", (float)w /16f);
	}
	
	// top 0 bottom 1 N 2 E 3 S 4 W 5
	
	bool[,] CullCollisionMatrix(bool[,] mask, int dim, int type) {
		// this just sets up an example 2D plane for testing purposes
		int size = 16; // 7 x 7 plane 
		
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