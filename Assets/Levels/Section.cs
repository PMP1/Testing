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
		bool [,] meshXZ = new bool[sectionSize,sectionSize];
		
		for (int y=0; y<sectionSize; y++) {
			for (int x=0; x<sectionSize; x++) {
			
				for (int z=0; z<sectionSize; z++) {
					if (Block (x, y, z) != 0) {
						//if (Block (x, 80 + 1, z) <= 0) {
							meshXZ[x, z] = true;
							
						//}
					}
				}
			}
			CullCollisionMatrix(meshXZ, y);
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
		col.sharedMesh = mesh;

		//MeshCollider myMC = GetComponent<MeshCollider>();
		/*Mesh newMesh = new Mesh();
		//newMesh  = new Mesh();
		newMesh.vertices  = newColliderVertices.ToArray();
		newMesh.triangles = newColliderTriangles.ToArray();
		newMesh.RecalculateBounds();
		col.sharedMesh = newMesh;
		colliderFaceCount = 0;*/



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
	
	
	
	bool[,] CullCollisionMatrix(bool[,] mask, int yy) {
		// this just sets up an example 2D plane for testing purposes
		int size = 16; // 7 x 7 plane 
		
		// mask is just a 2D array of bools, where true means that square needs a face drawn
		/*bool[,] mask = new bool[16, 16];
		for (int y = 0; y < 16; y ++)
		{
			for (int x = 0; x < 16; x ++)
			{
				mask[y, x] = true;
			}
		}

		mask[0, 0] = false;
		mask[0, 1] = false;
		mask[0, 2] = false;
		mask[0, 3] = false;
		mask[2, 4] = false;
		mask[2, 5] = false;
		mask[3, 1] = false;
		mask[4, 1] = false;
		mask[5, 1] = false;
		mask[6, 1] = false;*/
		
		int i = 0;
		int j = 0;
		int h = 0;
		bool building;
		bool done = false;
		
		for (int y = 0; y < 16; y ++) 
		{
			building = false;
			for (int x = 0; x < 16; x ++) 
			{
				if (mask[y, x] && !building) // start recording a new rectangle
				{
					building = true;
					i = x;
				}
				// if you reach a block that needs no face, or you reach the end of the row, you're done
				if ((!mask[y, x] && building) || (x == size-1 && building))
				{
					if (!mask[y, x] && building) // -1 because you've already passed the last block
					{
						j = x - 1;
					}
					else
					{
						j = x;
					}
					building = false;
					done = false;
					h = 1; // height
					
					// look upwards to see if the row above this one needs faces in the exact same spots (width wise)
					// this loop is a little wonky but it works
					for (int y2 = y+1; y2 < 16; y2 ++) 
					{
						for (int x2 = i; x2 < j; x2 ++) 
						{
							if (!mask[y2, x2])
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
					//# lower left coordinate of the RECTANGLE = i, y
					//# upper right coordinate of the RECTANGLE = j+1, y+h
					//print y, i, j, h	
					newColliderVertices.Add (new Vector3 (i, yy, y+h));
					newColliderVertices.Add (new Vector3 (j+1 + 1, yy, y+h));
					newColliderVertices.Add (new Vector3 (j+1 + 1, yy, y));
					newColliderVertices.Add (new Vector3 (i, yy, y));
					
					newColliderTriangles.Add (colliderFaceCount * 4); //1
					newColliderTriangles.Add (colliderFaceCount * 4 + 1); //2
					newColliderTriangles.Add (colliderFaceCount * 4 + 2); //3
					newColliderTriangles.Add (colliderFaceCount * 4); //1
					newColliderTriangles.Add (colliderFaceCount * 4 + 2); //3
					newColliderTriangles.Add (colliderFaceCount * 4 + 3); //4
					colliderFaceCount ++;				
					// update the mask to show that the spots covered by your rectangle no longer need faces
					for (int y3 = y; y3 < h+y; y3 ++) 
					{
						for (int x3 = i; x3 < j+1; x3 ++) 
						{
							mask[y3, x3] = false;
						}
					}
				}
			}
		}
		return mask;
	}


}