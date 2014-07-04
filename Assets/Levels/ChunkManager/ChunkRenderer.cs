//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Collections.Generic;


namespace AssemblyCSharp
{
    public class ChunkRenderer: MonoBehaviour
    {
        public GameObject sectionGo;
        private World world;
        private ChunkManager chunkManager;

        private List<Vector3> newVertices = new List<Vector3> ();
        private List<int> newTriangles = new List<int> ();
        private List<Vector2> newUV = new List<Vector2> ();
        
        private List<Vector3> newColliderVertices = new List<Vector3> ();
        private List<int> newColliderTriangles = new List<int> ();
        
        private List<Color> newColor = new List<Color> ();
        private float tUnit = 0.25f;

        private int faceCount = 0;

        public ChunkRenderer(ChunkManager manager)
        {
            this.chunkManager = manager;
        }

        private bool smoothLighting = true;
        
        public void RenderChunk(Chunk2 chunk) 
        {
            int firstSection = chunk.firstSection;

            for (int secY = firstSection; secY >= 0; secY++) 
            {
                Section2 section = chunk.GetSection(secY);
                RenderSection(section, chunk);
            }
        }

        private void RenderSection(Section2 sec, Chunk2 chunk) 
        {
            GenerateMesh(sec, chunk);

            GameObject newSectionGO = Instantiate(sectionGo, 
                                                  new Vector3(chunk.xPosition * 16f - 0.5f, sec.posY * 16f + 0.5f, chunk.zPosition * 16f - 0.5f), 
                                                  new Quaternion(0, 0, 0, 0)) as GameObject;
            sec.sectionGO = newSectionGO.GetComponent("SectionGO") as SectionGO;
            sec.sectionGO.SetMesh(newUV, newVertices, newTriangles);

            newUV.Clear();
            newVertices.Clear();
            newTriangles.Clear();
            faceCount = 0;
        }

        private void GenerateMesh(Section2 section, Chunk2 chunk) 
        {
            int chunkx = chunk.xPosition;
            int chunkz = chunk.zPosition;

            int posx;
            int posy;
            int posz;


            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        posx = x + chunkx;
                        posy = y + (section.posY * 16);
                        posz = z + chunkz;

                        Block block = this.chunkManager.GetBlock(posx, posy, posz);

                        if (block.BlockType == BlockType.Air || block.LightOpacity < 16)
                        {
                            Block blockT = this.chunkManager.GetBlock(posx, posy + 1, posz);
                            Block blockB = this.chunkManager.GetBlock(posx, posy - 1, posz);
                            Block blockN = this.chunkManager.GetBlock(posx, posy, posz + 1);
                            Block blockE = this.chunkManager.GetBlock(posx + 1, posy, posz);
                            Block blockS = this.chunkManager.GetBlock(posx, posy, posz - 1);
                            Block blockW = this.chunkManager.GetBlock(posx - 1, posy, posz);

                            if (blockT.BlockType == BlockType.Air || (blockT.LightOpacity < 16 && blockT.BlockType != block.BlockType)) 
                            {
                                CubeTop(posx,posy, posz, block);
                            }
                        }
                    }
                }
            }


        }

        private void CubeTop (int x, int y, int z, Block block)
        {
            newVertices.Add(new Vector3(x, y, z + 1));
            newVertices.Add(new Vector3(x + 1, y, z + 1));
            newVertices.Add(new Vector3(x + 1, y, z));
            newVertices.Add(new Vector3(x, y, z));
            
            Vector2 texturePos = block.Texture;
            Cube(texturePos);
        }

        private void CubeNorth (int x, int y, int z, Block block)
        {
            newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
            newVertices.Add(new Vector3(x + 1, y, z + 1));
            newVertices.Add(new Vector3(x, y, z + 1));
            newVertices.Add(new Vector3(x, y - 1, z + 1));
            
            Vector2 texturePos = block.Texture;
            Cube(texturePos);
        }

        private void CubeEast (int x, int y, int z, Block block)
        {
            newVertices.Add(new Vector3(x + 1, y - 1, z));
            newVertices.Add(new Vector3(x + 1, y, z));
            newVertices.Add(new Vector3(x + 1, y, z + 1));
            newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
            
            Vector2 texturePos = block.Texture;
            Cube(texturePos);
        }

        
        private void CubeSouth (int x, int y, int z, Block block)
        {
            newVertices.Add(new Vector3(x, y - 1, z));
            newVertices.Add(new Vector3(x, y, z));
            newVertices.Add(new Vector3(x + 1, y, z));
            newVertices.Add(new Vector3(x + 1, y - 1, z));
            
            Vector2 texturePos = block.Texture;
            Cube(texturePos);
        }


        
        private void CubeWest (int x, int y, int z, Block block)
        {
            newVertices.Add(new Vector3(x, y - 1, z + 1));
            newVertices.Add(new Vector3(x, y, z + 1));
            newVertices.Add(new Vector3(x, y, z));
            newVertices.Add(new Vector3(x, y - 1, z));

            Vector2 texturePos = block.Texture;
            Cube(texturePos);
        }

        private void CubeBot (int x, int y, int z, Block block)
        {
            
            newVertices.Add(new Vector3(x, y - 1, z));
            newVertices.Add(new Vector3(x + 1, y - 1, z));
            newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
            newVertices.Add(new Vector3(x, y - 1, z + 1));
            
            Vector2 texturePos = block.Texture;
            Cube(texturePos);
            
        }

        
        private void CubeTopLight(int x, int y, int z, Block block) 
        {
            if (smoothLighting)
            {
                Block blockN = this.chunkManager.GetBlock(x, y + 1, z + 1);
                Block blockNE = this.chunkManager.GetBlock(x + 1, y + 1, z + 1);
                Block blockE = this.chunkManager.GetBlock(x + 1, y + 1, z);
                Block blockSE = this.chunkManager.GetBlock(x + 1, y + 1, z - 1);
                Block blockS = this.chunkManager.GetBlock(x, y + 1, z - 1);
                Block blockSW = this.chunkManager.GetBlock(x - 1, y + 1, z - 1);
                Block blockW = this.chunkManager.GetBlock(x - 1, y + 1, z);
                Block blockNW = this.chunkManager.GetBlock(x - 1, y + 1, z + 1);
            
                int c1 = GetCornerLight(blockN, blockW, blockNW);
                int c2 = GetCornerLight(blockS, blockW, blockSW);
                int c3 = GetCornerLight(blockS, blockE, blockSE);
                int c4 = GetCornerLight(blockN, blockE, blockNE);
            
                CubeLight(c1, c2, c3, c4);
            } else
            {
                CubeLight(0, 0, 0, 0);
            }
        }

        
        private void CubeNorthLight(int x, int y, int z, Block block) 
        {
            /*Block blockN = this.chunkManager.GetBlock(x, y + 1, z + 1);
            Block blockNE = this.chunkManager.GetBlock(x + 1, y + 1, z + 1);
            Block blockE = this.chunkManager.GetBlock(x+ 1, y + 1, z);
            Block blockSE = this.chunkManager.GetBlock(x + 1, y + 1, z - 1);
            Block blockS = this.chunkManager.GetBlock(x, y + 1, z - 1);
            Block blockSW = this.chunkManager.GetBlock(x - 1, y + 1, z - 1);
            Block blockW = this.chunkManager.GetBlock(x - 1, y + 1, z);
            Block blockNW = this.chunkManager.GetBlock(x - 1, y + 1, z + 1);
            
            int c1 = GetCornerLight(blockN, blockW, blockNW);
            int c2 = GetCornerLight(blockS, blockW, blockSW);
            int c3 = GetCornerLight(blockS, blockE, blockSE);
            int c4 = GetCornerLight(blockN, blockE, blockNE);
            
            CubeLight(c1, c2, c3, c4);
            */


        }

        private int GetCornerLight(Block side1, Block side2, Block corner)
        {
            if (side1.BlockType != BlockType.Air && side2.BlockType != BlockType.Air)
            {
                return 0;
            }

            int hasSide1 = side1.BlockType != BlockType.Air ? 1 : 0;
            int hasSide2 = side2.BlockType != BlockType.Air ? 1 : 0;
            int hasCorner = corner.BlockType != BlockType.Air ? 1 : 0;
            return 3 - (hasSide1 + hasSide2 + hasCorner);
        }

        void CubeLight(int c1, int c2, int c3, int c4) {
            
            newColor.Add(new Color(0f,0f,0f,c1/3f));
            newColor.Add(new Color(0f,0f,0f,c2/3f));
            newColor.Add(new Color(0f,0f,0f,c3/3f));
            newColor.Add(new Color(0f,0f,0f,c4/3f));
        }

        void Cube (Vector2 texturePos)
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
            
            //CubeLight (lightLevel);
            
            faceCount++; // Add this line
        }



    }
}

