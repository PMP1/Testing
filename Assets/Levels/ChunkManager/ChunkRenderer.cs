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
    public class ChunkRenderer
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

        private SectionColliderGenerator colliderGenerator;
        private bool smoothLighting = true;


        private Chunk2 chunk;
        private bool neighboursLoaded = false; 



        public ChunkRenderer(ChunkManager manager, World world)
        {
            this.chunkManager = manager;
            this.world = world;
            this.colliderGenerator = new SectionColliderGenerator(this);
        }
                
        public void RenderChunk(Chunk2 chunk) 
        {
            int firstSection = chunk.firstSection;
            byte daylightLevel = world.time.GetDaylightLevel();

            this.chunk = chunk;


            for (int secY = firstSection; secY >= 0; secY--) 
            {
                Section2 section = chunk.GetSection(secY);
                RenderSection(section, chunk, daylightLevel);
                section.RequiresGORedraw = true;
            }

            if (chunk.xPosition == 8 && chunk.zPosition == 10)
            {
                int ii = 0;
            }
            chunkManager.requiresGOgeneration.Enqueue(chunk);

            chunk.isSectionsRendered = true;
        }

        private void RenderSection(Section2 sec, Chunk2 chunk, byte daylight) 
        {
            System.DateTime startGenMesh = System.DateTime.Now;
            GenerateMesh(sec, chunk);
            StatsEngine.SectionMeshGen += (float)System.DateTime.Now.Subtract(startGenMesh).TotalSeconds;

            if (faceCount > 0)
            {
                System.DateTime startCreateCollider = System.DateTime.Now;
                colliderGenerator.GenerateCollisionMatrix(sec, ref newColliderTriangles, ref newColliderVertices);
                StatsEngine.SectionColliderGen += (float)System.DateTime.Now.Subtract(startCreateCollider).TotalSeconds;

                sec.SetMeshData(newVertices, newTriangles, newUV);
                sec.SetColorData(newColor);
                sec.SetColliderData(newColliderVertices, newColliderTriangles);
            }
            newUV.Clear();
            newVertices.Clear();
            newTriangles.Clear();
            newColor.Clear();
            faceCount = 0;

            newColliderTriangles.Clear();
            newColliderVertices.Clear();
        }

        private Block GetBlock(int x, int y, int z)
        {
            return BlockManager.GetBlock(GetBlockId(x, y, z));
        }

        //local values
        public byte GetBlockId(int x, int y, int z)
        {
            byte blockId = 3;
            
            if (y >= 256 || y < 0)
            {
                return (byte)0;
            } else
            {
                
                if (x < 0) 
                {
                    if (z < 0) 
                    {
                        blockId = chunk.ChunkSouthWest.GetBlockId(x + 16, y, z + 16);
                    } 
                    else if (z >= 16)
                    { 
                        blockId = chunk.ChunkNorthWest.GetBlockId(x + 16, y, z - 16);
                    }
                    else 
                    {
                        blockId = chunk.ChunkWest.GetBlockId(x + 16, y, z);
                    }
                } 
                else if (x >= 16)
                {
                    if (z < 0) 
                    {
                        blockId = chunk.ChunkSouthEast.GetBlockId(x - 16, y, z + 16);
                    } 
                    else if (z >= 16)
                    {
                        blockId = chunk.ChunkNorthEast.GetBlockId(x - 16, y, z - 16);
                    }
                    else 
                    {
                        blockId = chunk.ChunkEast.GetBlockId(x - 16, y, z);
                    }
                }
                else
                {
                    //center
                    if (z < 0) 
                    {
                        blockId = chunk.ChunkSouth.GetBlockId(x, y, z + 16);
                    } 
                    else if (z >= 16)
                    {
                        blockId = chunk.ChunkNorth.GetBlockId(x, y, z - 16);
                    }
                    else 
                    {
                        blockId = chunk.GetBlockId(x, y, z);
                    }
                }
                return blockId;
            }
        }

        //local values
        public byte GetDaylightValue(int x, int y, int z)
        {
            byte value = 15;
            
            if (y >= 256 || y < 0)
            {
                return (byte)15;
            } else
            {
                if (x < 0) 
                {
                    if (z < 0) 
                    {
                        value = chunk.ChunkSouthWest.GetDaylightValue(x + 16, y, z + 16);
                    } 
                    else if (z >= 16)
                    {
                        value = chunk.ChunkNorthWest.GetDaylightValue(x + 16, y, z - 16);
                    }
                    else 
                    {
                        value = chunk.ChunkWest.GetDaylightValue(x + 16, y, z);
                    }
                } 
                else if (x >= 16)
                {
                    if (z < 0) 
                    {
                        value = chunk.ChunkSouthEast.GetDaylightValue(x - 16, y, z + 16);
                    } 
                    else if (z >= 16)
                    {
                        value = chunk.ChunkNorthEast.GetDaylightValue(x - 16, y, z - 16);
                    }
                    else 
                    {
                        value = chunk.ChunkEast.GetDaylightValue(x - 16, y, z);
                    }
                }
                else
                {
                    //center
                    if (z < 0) 
                    {
                        value = chunk.ChunkSouth.GetDaylightValue(x, y, z + 16);
                    } 
                    else if (z >= 16)
                    {
                        value = chunk.ChunkNorth.GetDaylightValue(x, y, z - 16);
                    }
                    else 
                    {
                        value = chunk.GetDaylightValue(x, y, z);
                    }
                }
                
                
                return value;
            }
        }

        private void GenerateMesh(Section2 section, Chunk2 chunk) 
        {
            int posy;

            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        //posx = x + (chunkx * 16);
                        posy = y + (section.posY * 16);
                        //posz = z + (chunkz * 16);

                        byte id = section.GetBlockId(x, y, z);

                        if (id != (byte)0)//|| block.LightOpacity < 16)
                        {
                            Block block = BlockManager.GetBlock((byte)id);

                            Block blockT = this.GetBlock(x, posy + 1, z);
                            Block blockB = this.GetBlock(x, posy - 1, z);
                            Block blockN = this.GetBlock(x, posy, z + 1);
                            Block blockE = this.GetBlock(x + 1, posy, z);
                            Block blockS = this.GetBlock(x, posy, z - 1);
                            Block blockW = this.GetBlock(x - 1, posy, z);

                            System.DateTime startCreateSmoothLight;
                            double currentlighting = 0;

                            if (blockT.BlkType == BlockType.Air) // || (blockT.LightOpacity < 16 && blockT.BlockType != block.BlockType)) 
                            {
                                CubeTop(x, y, z, block);
                                CubeTopLight(x, posy, z, block);
                            }
                            if (blockB.BlkType == BlockType.Air)
                            {
                                CubeBot(x, y, z, block);
                                CubeBottomLight(x, posy, z, block);
                            }
                            if (blockN.BlkType == BlockType.Air)
                            {
                                CubeNorth(x, y, z, block);
                                CubeNorthLight(x, posy, z, block);
                            }
                            if (blockE.BlkType == BlockType.Air)
                            {
                                CubeEast(x, y, z, block);
                                CubeEastLight(x, posy, z, block);
                            }
                            if (blockS.BlkType == BlockType.Air)
                            {
                                CubeSouth(x, y, z, block);
                                CubeSouthLight(x, posy, z, block);
                            }
                            if (blockW.BlkType == BlockType.Air)
                            {
                                CubeWest(x, y, z, block);
                                CubeWestLight(x, posy, z, block);
                            }

                            StatsEngine.SectionSmoothLighting += (float)currentlighting;
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

                byte blockN = this.GetDaylightValue(x, y + 1, z + 1);
                byte blockNE = this.GetDaylightValue(x + 1, y + 1, z + 1);
                byte blockE = this.GetDaylightValue(x + 1, y + 1, z);
                byte blockSE = this.GetDaylightValue(x + 1, y + 1, z - 1);
                byte blockS = this.GetDaylightValue(x, y + 1, z - 1);
                byte blockSW = this.GetDaylightValue(x - 1, y + 1, z - 1);
                byte blockW = this.GetDaylightValue(x - 1, y + 1, z);
                byte blockNW = this.GetDaylightValue(x - 1, y + 1, z + 1);
                byte blockC = this.GetDaylightValue(x, y + 1, z);
                float c1 = GetAverageLight(blockN, blockW, blockNW, blockC);
                float c2 = GetAverageLight(blockS, blockW, blockSW, blockC);
                float c3 = GetAverageLight(blockS, blockE, blockSE, blockC);
                float c4 = GetAverageLight(blockN, blockE, blockNE, blockC);
                    
                CubeLight(c1, c4, c3, c2);
            } else
            {
                CubeLight(15);
            }
        }

        
        private void CubeBottomLight(int x, int y, int z, Block block) 
        {
            if (smoothLighting)
            {
                
                byte blockN  = this.GetDaylightValue(x, y - 1, z + 1);
                byte blockNE = this.GetDaylightValue(x + 1, y - 1, z + 1);
                byte blockE  = this.GetDaylightValue(x + 1, y - 1, z);
                byte blockSE = this.GetDaylightValue(x + 1, y - 1, z - 1);
                byte blockS  = this.GetDaylightValue(x, y - 1, z - 1);
                byte blockSW = this.GetDaylightValue(x - 1, y - 1, z - 1);
                byte blockW  = this.GetDaylightValue(x - 1, y - 1, z);
                byte blockNW = this.GetDaylightValue(x - 1, y - 1, z + 1);
                byte blockC = this.GetDaylightValue(x, y - 1, z);
                
                float c1 = GetAverageLight(blockN, blockW, blockNW, blockC);
                float c2 = GetAverageLight(blockS, blockW, blockSW, blockC);
                float c3 = GetAverageLight(blockS, blockE, blockSE, blockC);
                float c4 = GetAverageLight(blockN, blockE, blockNE, blockC);
                
                CubeLight(c2, c3,c4, c1);
            } else
            {
                CubeLight(15);
            }
        }

        
        private void CubeNorthLight(int x, int y, int z, Block block) 
        {
            if (smoothLighting)
            {
                byte blockT  = this.GetDaylightValue(x,   y+1, z+1); //top
                byte blockTE = this.GetDaylightValue(x+1, y+1, z+1); //top - east
                byte blockE  = this.GetDaylightValue(x+1, y,   z+1);
                byte blockBE = this.GetDaylightValue(x+1, y-1, z+1);
                byte blockB  = this.GetDaylightValue(x,   y-1, z+1);
                byte blockBW = this.GetDaylightValue(x-1, y-1, z+1);
                byte blockW  = this.GetDaylightValue(x-1, y,   z+1);
                byte blockTW = this.GetDaylightValue(x-1, y+1, z+1);
                byte blockC  = this.GetDaylightValue(x,   y,   z+1);

                float c1 = GetAverageLight(blockT, blockW, blockTW, blockC);
                float c2 = GetAverageLight(blockB, blockW, blockBW, blockC);
                float c3 = GetAverageLight(blockB, blockE, blockBE, blockC);
                float c4 = GetAverageLight(blockT, blockE, blockTE, blockC);

                CubeLight(c3, c4, c1, c2);
            } else
            {
                CubeLight(15);
            }
        }
        
        private void CubeEastLight(int x, int y, int z, Block block) 
        {
            if (smoothLighting)
            {
                byte blockT = this.GetDaylightValue(x + 1, y + 1, z); //top
                byte blockTN = this.GetDaylightValue(x + 1, y + 1, z + 1); //top - east
                byte blockN = this.GetDaylightValue(x + 1, y, z + 1);
                byte blockBN = this.GetDaylightValue(x + 1, y - 1, z + 1);
                byte blockB = this.GetDaylightValue(x + 1, y - 1, z);
                byte blockBS = this.GetDaylightValue(x + 1, y - 1, z - 1);
                byte blockS = this.GetDaylightValue(x + 1, y, z - 1);
                byte blockTS = this.GetDaylightValue(x + 1, y + 1, z - 1);
                byte blockC = this.GetDaylightValue(x + 1, y, z);
            
                float c1 = GetAverageLight(blockT, blockN, blockTN, blockC);
                float c2 = GetAverageLight(blockB, blockN, blockBN, blockC);
                float c3 = GetAverageLight(blockB, blockS, blockBS, blockC);
                float c4 = GetAverageLight(blockT, blockS, blockTS, blockC);

                CubeLight(c3, c4, c1, c2);       
            } else
            {
                CubeLight(15);
            }
        }

        
        private void CubeSouthLight(int x, int y, int z, Block block) 
        {
            if (smoothLighting)
            {
                byte blockT = this.GetDaylightValue(x, y + 1, z - 1); //top
                byte blockTE = this.GetDaylightValue(x + 1, y + 1, z - 1); //top - east
                byte blockE = this.GetDaylightValue(x + 1, y, z - 1);
                byte blockBE = this.GetDaylightValue(x + 1, y - 1, z - 1);
                byte blockB = this.GetDaylightValue(x, y - 1, z - 1);
                byte blockBW = this.GetDaylightValue(x - 1, y - 1, z - 1);
                byte blockW = this.GetDaylightValue(x - 1, y, z - 1);
                byte blockTW = this.GetDaylightValue(x - 1, y + 1, z - 1);
                byte blockC = this.GetDaylightValue(x, y, z - 1);
            
                float c1 = GetAverageLight(blockT, blockE, blockTE, blockC);
                float c2 = GetAverageLight(blockB, blockE, blockBE, blockC);
                float c3 = GetAverageLight(blockB, blockW, blockBW, blockC);
                float c4 = GetAverageLight(blockT, blockW, blockTW, blockC);

                CubeLight(c3, c4, c1, c2);
            } else
            {
                CubeLight(15);
            }
        }

        private void CubeWestLight(int x, int y, int z, Block block) 
        {   
            if (smoothLighting)
            {
                byte blockT = this.GetDaylightValue(x - 1, y + 1, z); //top
                byte blockTN = this.GetDaylightValue(x - 1, y + 1, z + 1); //top - north
                byte blockN = this.GetDaylightValue(x - 1, y, z + 1);
                byte blockBN = this.GetDaylightValue(x - 1, y - 1, z + 1);
                byte blockB = this.GetDaylightValue(x - 1, y - 1, z);
                byte blockBS = this.GetDaylightValue(x - 1, y - 1, z - 1);
                byte blockS = this.GetDaylightValue(x - 1, y, z - 1);
                byte blockTS = this.GetDaylightValue(x - 1, y + 1, z - 1);
                byte blockC = this.GetDaylightValue(x - 1, y, z);
               
                float c1 = GetAverageLight(blockT, blockN, blockTN, blockC);
                float c2 = GetAverageLight(blockB, blockN, blockBN, blockC);
                float c3 = GetAverageLight(blockB, blockS, blockBS, blockC);
                float c4 = GetAverageLight(blockT, blockS, blockTS, blockC);

                CubeLight(c2, c1, c4, c3);       
            } else
            {
                CubeLight(15);
            }
        }


        private float GetAverageLight(byte side1, byte side2, byte side3, byte side4)
        {
            if (side1 + side2 + side3 + side4 < 60)
            {
                int i = 0; 
            }
            return ((float)(side1 + side2 + side3 + side4)) / (16f * 4f);
        }

        private int GetCornerLight(Block side1, Block side2, Block corner)
        {
            if ((int)side1.BlkType != 0 && (int)side2.BlkType != 0)
            {
                return 4;
            }

            int hasSide1 = (int)side1.BlkType != 0 ? 1 : 0;
            int hasSide2 = (int)side2.BlkType != 0 ? 1 : 0;
            int hasCorner = (int)corner.BlkType != 0 ? 1 : 0;
            return 1 + (hasSide1 + hasSide2 + hasCorner);
        }

        void CubeLight(int defaultLevel) {

            float level = defaultLevel / 15f;

            newColor.Add(new Color(0f,0f,0f,level));
            newColor.Add(new Color(0f,0f,0f,level));
            newColor.Add(new Color(0f,0f,0f,level));
            newColor.Add(new Color(0f,0f,0f,level));
        }

        void CubeLight(float c1, float c2, float c3, float c4) {
            
            newColor.Add(new Color(0f,0f,0f,c1));
            newColor.Add(new Color(0f,0f,0f,c2));
            newColor.Add(new Color(0f,0f,0f,c3));
            newColor.Add(new Color(0f,0f,0f,c4));
        }

        void CubeLight(int c1, int c2, int c3, int c4, byte light) {
            
            newColor.Add(new Color(0f,0f,0f,light/(c1*15f)));
            newColor.Add(new Color(0f,0f,0f,light/(c2*15f)));
            newColor.Add(new Color(0f,0f,0f,light/(c3*15f)));
            newColor.Add(new Color(0f,0f,0f,light/(c4*15f)));
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
                        
            faceCount++; // Add this line
        }



    }
}

