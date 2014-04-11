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
	private List<Vector2> newUV2 = new List<Vector2> ();
	private float tUnit = 0.25f;
	private Vector2 tStone = new Vector2 (1, 0);
	private Vector2 tGrass = new Vector2 (0, 1);
	private Vector2 tGrassTop = new Vector2 (1, 1);
	private Mesh mesh;
	private MeshCollider col;
	private int faceCount;
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

	void CubeTop (int x, int y, int z, byte block)
	{

		newVertices.Add (new Vector3 (x, y, z + 1));
		newVertices.Add (new Vector3 (x + 1, y, z + 1));
		newVertices.Add (new Vector3 (x + 1, y, z));
		newVertices.Add (new Vector3 (x, y, z));

		Vector2 texturePos = new Vector2 (0, 0);

		if (Block (x, y, z) == 1) {
				texturePos = tStone;
		} else if (Block (x, y, z) == 2) {
				texturePos = tGrassTop;
		}

		Cube (texturePos, LightBlock(x, y + 1, z));

	}

	void CubeNorth (int x, int y, int z, byte block)
	{

		newVertices.Add (new Vector3 (x + 1, y - 1, z + 1));
		newVertices.Add (new Vector3 (x + 1, y, z + 1));
		newVertices.Add (new Vector3 (x, y, z + 1));
		newVertices.Add (new Vector3 (x, y - 1, z + 1));

		Vector2 texturePos = new Vector2 (0, 0);

		if (Block (x, y, z) == 1) {
				texturePos = tStone;
		} else if (Block (x, y, z) == 2) {
				texturePos = tGrass;
		}

		Cube (texturePos, LightBlock(x, y, z + 1));

	}

	void CubeEast (int x, int y, int z, byte block)
	{
		newVertices.Add (new Vector3 (x + 1, y - 1, z));
		newVertices.Add (new Vector3 (x + 1, y, z));
		newVertices.Add (new Vector3 (x + 1, y, z + 1));
		newVertices.Add (new Vector3 (x + 1, y - 1, z + 1));

		Vector2 texturePos = new Vector2 (0, 0);

		if (Block (x, y, z) == 1) {
				texturePos = tStone;
		} else if (Block (x, y, z) == 2) {
				texturePos = tGrass;
		}

		Cube (texturePos, LightBlock(x + 1, y, z));

	}

	void CubeSouth (int x, int y, int z, byte block)
	{

		newVertices.Add (new Vector3 (x, y - 1, z));
		newVertices.Add (new Vector3 (x, y, z));
		newVertices.Add (new Vector3 (x + 1, y, z));
		newVertices.Add (new Vector3 (x + 1, y - 1, z));

		Vector2 texturePos = new Vector2 (0, 0);

		if (Block (x, y, z) == 1) {
				texturePos = tStone;
		} else if (Block (x, y, z) == 2) {
				texturePos = tGrass;
		}

		Cube (texturePos, LightBlock(x, y, z - 1));

	}

	void CubeWest (int x, int y, int z, byte block)
	{

			newVertices.Add (new Vector3 (x, y - 1, z + 1));
			newVertices.Add (new Vector3 (x, y, z + 1));
			newVertices.Add (new Vector3 (x, y, z));
			newVertices.Add (new Vector3 (x, y - 1, z));

			Vector2 texturePos = new Vector2 (0, 0);

			if (Block (x, y, z) == 1) {
					texturePos = tStone;
			} else if (Block (x, y, z) == 2) {
					texturePos = tGrass;
			}

		Cube (texturePos,  LightBlock(x - 1, y, z));

	}

	void CubeBot (int x, int y, int z, byte block)
	{

		newVertices.Add (new Vector3 (x, y - 1, z));
		newVertices.Add (new Vector3 (x + 1, y - 1, z));
		newVertices.Add (new Vector3 (x + 1, y - 1, z + 1));
		newVertices.Add (new Vector3 (x, y - 1, z + 1));

		Vector2 texturePos = new Vector2 (0, 0);

		if (Block (x, y, z) == 1) {
				texturePos = tStone;
		} else if (Block (x, y, z) == 2) {
				texturePos = tGrass;
		}

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
		Vector2 lightUV = new Vector2(lightLevel / 4, lightLevel % 4	);
		//inversed y!
		newUV2.Add (new Vector2 (tUnit * lightUV.x + tUnit, tUnit * lightUV.y));
		newUV2.Add (new Vector2 (tUnit * lightUV.x + tUnit, tUnit * lightUV.y + tUnit));
		newUV2.Add (new Vector2 (tUnit * lightUV.x, tUnit * lightUV.y + tUnit));
		newUV2.Add (new Vector2 (tUnit * lightUV.x, tUnit * lightUV.y));
	}

	void UpdateMesh ()
	{
		mesh.Clear ();
		mesh.vertices = newVertices.ToArray ();
		mesh.uv = newUV.ToArray ();
		mesh.uv2 = newUV2.ToArray ();
		mesh.triangles = newTriangles.ToArray ();
		mesh.Optimize ();
		mesh.RecalculateNormals ();

		col.sharedMesh = null;
		col.sharedMesh = mesh;

		newVertices.Clear ();
		newUV.Clear ();
		newUV2.Clear ();
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
		mesh.uv2 = newUV2.ToArray ();
		newUV2.Clear ();
	}


}