using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

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

    public int Id = 0;
	//private Vector2 t2 = new Vector2 (2, 1);
	//private Vector2 t3 = new Vector2 (2, 2);
	//private Vector2 t4 = new Vector2 (2, 3);


	public byte[,,] data;
	public byte[,,] lightData;
	public byte[,,] daylightData;


	//aspirational info
	private bool isSolid = false;
	private bool isAir = false;
	private bool isLoaded = false;
	private int hightestPoint = 0;
	private int lowestPoint = 0;

	
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
	public bool hasCollisionMatrix = false;


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

	public void SetBlock(int x, int y, int z, BlockType type)
	{
		this.data [x, y, z] = (byte)type;
	}

	public void SetCollisionMesh(List<Vector3> verts, List<int> tris)
	{
		newColliderTriangles = tris;
		newColliderVertices = verts;
	}


	public void GenerateMesh ()
	{
		if (useCollisionMatrix) {

			SectionColliderGenerator generator = new SectionColliderGenerator();
			//GenerateCollisionMesh ();

			generator.GenerateCollisionMatrix(this);
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

	public byte Block (int x, int y, int z)
	{
        if (x < 0 || x >= sectionSize || y < 0 || y >= sectionSize || z < 0 || z >= sectionSize)
        {
            return chunk.Block(x, y + sectionY, z);
        }

        return data [x, y, z];
	}

	byte LightBlock (int x, int y, int z)
	{
        byte l = 5;
        //byte l = daylightData [x, y, z];
		//byte l = chunk.LightBlock (x, y + sectionY, z);
		return (byte)Mathf.Max (l, 0);
	}
	
	private Vector2 GetTexture(int type) {
		return world.BlockManager.GetTexture ((byte) type);
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

		GenerateDayLight ();
		//renderer.material.SetFloat ("_Sun", 0f);

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
}