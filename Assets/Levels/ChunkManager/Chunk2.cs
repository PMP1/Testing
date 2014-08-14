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
namespace AssemblyCSharp
{
    public class Chunk2
    {

        public int xPosition;
        public int zPosition;
        public int yHeight;
        int sectionSize = 16;

        private BlockLightUpdate[] collection = new BlockLightUpdate[32*32*32];


        public ChunkManager manager;

        //16*16 = 256

        Section2[] sections;
        public int firstSection; 

        byte[] biomeMap;

        public byte[] heightMap;
        public int maxHeight;
        public int minHeight;


        bool[] daylightColumnUpdates;



        public bool containsWater = false;


        public Chunk2 ChunkNorth;
        public Chunk2 ChunkNorthEast;
        public Chunk2 ChunkEast;
        public Chunk2 ChunkSouthEast;
        public Chunk2 ChunkSouth;
        public Chunk2 ChunkSouthWest;
        public Chunk2 ChunkWest;
        public Chunk2 ChunkNorthWest;

        public int status = 0; //0=created, 1=dataloaded, 2=lightingupdated, 3=rendered
        public int pendingStatus = 0;

        public bool isDataLoaded = false; //Set when data is fully loaded and basic daylightadded

        public bool isNeighboursLoaded = false; //when true should spread daylight

        public bool isLightingUpdateRequired = true;
        //public bool isDaylightCalculated = false;
        //public bool isLightCalculated = false;
        //public bool isSectionsGenerated = false; // 

        public bool isQueuedForReRender = false;

        //requires light spread
        //basic generation?
        //requires regeneration

        public bool isSectionsRendered = false; // section completed loading and Go created

        //section = 4096

        //all 65536
        public Chunk2(ChunkManager manager, int x, int z)
        {
            xPosition = x;
            zPosition = z;
            yHeight = 256;

            this.manager = manager;

            sections = new Section2[sectionSize];
            biomeMap = new byte[sectionSize * sectionSize];
            heightMap = new byte[sectionSize * sectionSize];
            daylightColumnUpdates = new bool[sectionSize * sectionSize];



        }


        public void SetData(byte[] chunkData)
        {
            int height = chunkData.Length / 256; //one slice of 16*16 data

            //split data into sections
            for (int x = 0; x < sectionSize; x++)
            {
                for (int z = 0; z < sectionSize; z++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        byte block = chunkData[x + 16 * (z + 16 * y)];

                        if (block != 0) {

                            int section = y / 16;
                            if (sections[section] == null) {
                                sections[section] = new Section2(section, this);
                            }

                            sections[section].SetBlockId(x, y % 16, z, block);
                        }
                    }
                }
            }

            SetFirstSection();
            GenerateDaylight();
        }

        private void SetFirstSection()
        {
            for (int y = 15; y >= 0; y--)
            {
                if (this.sections[y] != null)
                {
                    this.firstSection = y;
                    return;
                }
            }
        }

        public Section2 GetSection(int y)
        {
            if (y > this.firstSection)
                return null;

            return this.sections [y];
        }


        public void CheckStatusUpdate(bool useQueue)
        {
            /* Status 0 = not loaded
             * Status 1 = data generated
             * Status 2 = Light generated
             * Status 3 = rendered */

            if (status == 0)
                return;

            if (status == 3 && pendingStatus == 3)
                return;

            if (status == 1 && pendingStatus == 1 && NeightboursMinStatus(1))
            {
                pendingStatus = 2;
                //status = 2;
                if (useQueue)
                {
                    ChunkLoader.GenerateLight(this);
                }else {
                    this.SpreadDaylight();
                    status = 2;
                }
            }

            if (status == 2 && pendingStatus == 2 && NeightboursMinStatus(2))
            {
                pendingStatus = 3;
                if (useQueue)
                {
                    ChunkLoader.RenderChunk(this);
                }else {
                    manager.renderer.RenderChunk(this);
                }

                return;
            }
        }



        public void GenerateSecGO()
        {

            if (xPosition == 8 && zPosition == 10)
            {
                int ii = 0;
            }
            for (int i = 0; i < 16; i++)
            {
                if (sections[i] != null)
                {
                    Section2 sec = sections[i];
                    if (!sec.sectionGO) {
                        sec.GenerateGO();
                        //sec.ClearGOTempData();
                    }
                    else 
                    {
                        sec.updateMesh = true;
                        //sec.SetNewGOMesh();
                        //sec.SetNewGOCollider();
                        //sec.ClearGOTempData();
                    }
                }
            }
            this.isSectionsRendered = true;
        }

        /// <summary>
        /// A quickway to update a height, used during generation
        /// </summary>
        /// <param name="x">The x coordinate.</pforaram>
        /// <param name="z">The z coordinate.</param>
        /// <param name="height">Height.</param>
        public void SetHeightMap(int x, int z, byte height) 
        {
            this.heightMap[ x + 16 * z] = height;
        }

        public void SetBiomeMap(int x, int z, byte biome) 
        {
            this.biomeMap[ x + 16 * z] = biome;
        }

        public void SetBlockId(int x, int y, int z, byte id)
        {
            int secY = y / 16;
            if (sections.Length < secY)
                return;
            
            Section2 sec = this.sections [secY];
            
            if (sec == null)
            {
                return;
            } else
            {
                int secYPos = y - (secY * 16); //TODO check this for speed issues
                sec.SetBlockId(x, secYPos, z, id);
            }
        }

        public byte GetBlockId(int x, int y, int z)
        {
            int secY = y / 16;
            if (sections.Length < secY)
                return 0;

            Section2 sec = this.sections [secY];

            if (sec == null)
            {
                return 0;
            } else
            {
                int secYPos = y - (secY * 16); //TODO check this for speed issues
                return sec.GetBlockId(x, secYPos, z);
            }
        }

        public byte GetHeightMap(int x, int z)
        {
            return this.heightMap [x + 16 * z];
        }

        public int PosGetHeightMap(int posx, int posz)
        {
            int chunkX = posx >> 4;
            int chunkZ = posz >> 4;
            int x = posx - chunkX * 16;
            int z = posz - chunkZ * 16;
            
            Chunk2 chunk = GetChunk(chunkX - xPosition, chunkZ - zPosition);
            return (int)chunk.GetHeightMap(x, z);
        }

        public bool NeightboursMinStatus(int status)
        {
            if (ChunkNorth == null || ChunkNorth.status < status)
                return false;
            if (ChunkNorthEast == null || ChunkNorthEast.status < status)
                return false;
            if (ChunkEast == null || ChunkEast.status < status)
                return false;
            if (ChunkSouthEast == null || ChunkSouthEast.status < status)
                return false;
            if (ChunkSouth == null || ChunkSouth.status < status)
                return false;
            if (ChunkSouthWest == null || ChunkSouthWest.status < status)
                return false;
            if (ChunkWest == null || ChunkWest.status < status)
                return false;
            if (ChunkNorthWest == null || ChunkNorthWest.status < status)
                return false;

            return true;
        }


        public bool HasNeighbours(int x, int y, int z, int blockDist)
        {
            return manager.DoChunksExist(x, y, z, blockDist);
        }

        #region light operators

        public byte GetDaylightValue(int x, int y, int z)
        {
            int secY = y / 16;
            if (sections.Length < secY)
                return 0;
            
            Section2 sec = this.sections [secY];
            
            if (sec == null)
            {
                return 0;
            } else
            {
                int secYPos = y - (secY * 16); //TODO check this for speed issues
                return sec.GetDaylightValue(x, secYPos, z);
            }
        }

        public void SetDaylightValue(int x, int y, int z, byte level)
        {
            int secY = y / 16;
            if (sections.Length < secY)
                return;
            
            Section2 sec = this.sections [secY];
            
            if (sec == null)
            {
                return;
            } else
            {
                int secYPos = y - (secY * 16); //TODO check this for speed issues
                sec.SetDatlightData(x, secYPos, z, level);
            }
        }

        public int GetLightOpacity(int x, int y, int z) 
        {
            return BlockManager.GetLightOpacity(GetBlockId(x, y, z));
        }

        public void GenerateDaylight() 
        {
            //bool secChanged[] = false;
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    int lightLevel = 15;

                    for (int y = (this.firstSection * sectionSize) + sectionSize - 1; y > 0; y --)
                    {
                        lightLevel -= this.GetLightOpacity(x, y, z);
                        if (lightLevel > 0) {
                            Section2 sec = this.sections[y /16];
                            sec.SetDatlightData(x, y % 16, z, lightLevel);
                        } else {
                            break;
                        }

                    }

                    this.daylightColumnUpdates[x + 16 * z] = true;
                }
            }
        }

        /*public void SpreadDaylight_tick()
        {
            if (isLightingUpdateRequired)
            {
                if (SpreadDaylight())
                {
                    ChunkLoader.RequestLightRegeneration(manager, this);
                }
            }
        }*/

        public bool SpreadDaylight()
        {
            //if (manager.DoChunksExist(this.xPosition << 4, 0, this.zPosition << 4, 16))
            {
                int chunkX = xPosition << 4;
                int chunkZ = zPosition << 4;




                for (int x = 0; x < 16; x++)
                {
                    for (int z = 0; z < 16; z++)
                    {

                        int posx = xPosition * 16 + x;
                        int posz = zPosition * 16 + z;

                        int c = PosGetHeightMap(posx, posz);
                        int n = PosGetHeightMap(posx, posz + 1);
                        int e = PosGetHeightMap(posx + 1, posz);
                        int s = PosGetHeightMap(posx, posz - 1);
                        int w = PosGetHeightMap(posx - 1, posz);

                        //need to get the heighest neighbour
                        if (n < e)
                            n = e;
                        if (n < s)
                            n = s;
                        if (n < w)
                            n = w;

                        for (int y = c; y < n; y++)
                        {
                            if (xPosition == 9 && zPosition == 8 && x == 0  && z == 0 && y == 208)
                            {
                                var i = 0;
                            }

                            UpdateLightBlock(x, y, z, 15);
                            UpdateLightBlock(x, y, z + 1, 15);
                            UpdateLightBlock(x, y, z - 1, 15);
                            UpdateLightBlock(x + 1, y, z, 15);
                            UpdateLightBlock(x - 1, y, z, 15);
                        }
                    }
                }
                isLightingUpdateRequired = false;
                return true;
            }
            return false;
        }



        public void UpdateDaylight(byte level)
        {

            for (int i = 0; i < 16; i++)
            {
                if (sections[i] != null)
                {
                    Section2 sec = sections[i];
                    if (sec.sectionGO) {
                        sec.sectionGO.SetDaylight(level);
                    }
                }
            }
        }

        public int GetChunkLight(int chunkX, int y, int chunkZ)
        {
            if (xPosition == 9 && zPosition == 8 && chunkX == 0  && chunkZ == 0 && y >=  192 && y <= 195)
            {
                var i = 0;
            }

            int secY = y >> 4;
            if (sections.Length < secY)
                return 0;
            
            Section2 sec = this.sections [secY];
            
            if (sec == null)
            {
                return 0;
            } else
            {
                int secYPos = y - (secY * 16); //TODO check this for speed issues
                return sec.GetDaylightValue(chunkX, secYPos, chunkZ);
            }
        }

        public void SetChunkLight(int chunkX, int y, int chunkZ, int value)
        {

            if (xPosition == 9 && zPosition == 8 && chunkX == 0  && chunkZ == 0 && y >=  192 && y <= 194)
            {
                var i = 0;
            }

            int secY = y >> 4;
            if (sections.Length < secY)
                return;
            
            Section2 sec = this.sections [secY];
            
            if (sec == null)
            {
                return;
            } else
            {
                int secYPos = y - (secY * 16); //TODO check this for speed issues
                sec.SetDatlightData(chunkX, secYPos, chunkZ, value);
            }
        }



        private Chunk2 GetChunk(int chunkX, int chunkZ)
        {
            if (chunkX < 0) //west
            {
                if (chunkZ < 0) //south
                {
                    return ChunkSouthWest;
                } else if (chunkZ >= 1) //north
                {
                    return ChunkNorthWest;
                } else //center
                {
                    return ChunkWest;
                }
            } else if (chunkX >= 1) //east
            {
                if (chunkZ < 0) //south
                {
                    return ChunkSouthEast;
                } else if (chunkZ >= 1) //north
                {
                    return ChunkNorthEast;
                } else //center
                {
                    return ChunkEast;
                }
            } else //center
            {
                if (chunkZ < 0) //south
                {
                    return ChunkSouth;
                } else if (chunkZ >= 1) //north
                {
                    return ChunkNorth;
                } else //center
                {
                    return this;
                }
            }
        }


        private int PosGetLight(int posx, int y, int posz)
        {
            if (y >= 256)
                return 0;
            
            if (y <= 0)
                return 0;
            
            int chunkX = posx >> 4;
            int chunkZ = posz >> 4;
            int x = posx - chunkX * 16;
            int z = posz - chunkZ * 16;
            
            Chunk2 chunk = GetChunk(chunkX - xPosition, chunkZ - zPosition);

            return chunk.GetChunkLight(x, y, z);
        }

        private void PosSetLight(int posx, int y, int posz, int level)
        {

            if (y >= 256)
                return;
            
            if (y <= 0)
                return;
            
            int chunkX = posx >> 4;
            int chunkZ = posz >> 4;
            int x = posx - chunkX * 16;
            int z = posz - chunkZ * 16;
            
            Chunk2 chunk = GetChunk(chunkX - xPosition, chunkZ - zPosition);

            chunk.SetChunkLight(x, y, z, level);
        }

        
        private bool FacesTheSky(int x, int y, int z)
        {
            int height = GetHeightMap(x, z);

            if (y >= height)
                return true;
            else 
                return false;
        }

        private bool PosFacesTheSky(int posx, int y, int posz)
        {
            if (y >= 256)
                return true;
            
            if (y <= 0)
                return false;

            int chunkX = posx >> 4;
            int chunkZ = posz >> 4;
            int x = posx - chunkX * 16;
            int z = posz - chunkZ * 16;

            Chunk2 chunk = GetChunk(chunkX - xPosition, chunkZ - zPosition);
              
            return chunk.FacesTheSky(x, y, z);

        }

        private Block GetBlock(int chunkX, int y, int chunkZ)
        {
            int blockId = GetBlockId(chunkX, y, chunkZ);
            return BlockManager.GetBlock(blockId);
        }

        private Block PosGetBlock(int posx, int y, int posz)
        {
            if (y >= 256 || y <= 0)
                return null;

            int chunkX = posx >> 4;
            int chunkZ = posz >> 4;
            int x = posx - chunkX * 16;
            int z = posz - chunkZ * 16;

            Chunk2 chunk = GetChunk(chunkX - xPosition, chunkZ - zPosition);

            return chunk.GetBlock(x, y, z);
        }

        public void UpdateLightBlock(int x, int y, int z, byte level)
        {
            int capacity = 0;
            int current = 0;
                       
            collection [capacity++] = new BlockLightUpdate(x, y, z, level);

            while (capacity > current)
            {
                int n = 0;
                int s = 0;
                int t = 0;
                int b = 0;
                int e = 0;
                int w = 0;
                bool neightboursLoaded = false;
                
                BlockLightUpdate block = collection [current++];
                
                int posX = block.posX;
                int posY = block.posY;
                int posZ = block.posZ;

                int savedValue = PosGetLight(posX + (xPosition * 16), posY, posZ + (zPosition * 16));

                int calcValue = 0;
                
                if (PosFacesTheSky(posX + (xPosition * 16), posY, posZ + (zPosition * 16)))
                {
                    calcValue = 15;
                } else
                {
                    n = PosGetLight(posX + (xPosition * 16), posY, posZ + (zPosition * 16) + 1);
                    s = PosGetLight(posX + (xPosition * 16), posY, posZ + (zPosition * 16) - 1);
                    t = PosGetLight(posX + (xPosition * 16), posY + 1, posZ + (zPosition * 16));
                    b = PosGetLight(posX + (xPosition * 16), posY - 1, posZ + (zPosition * 16));
                    e = PosGetLight(posX + (xPosition * 16) + 1, posY, posZ + (zPosition * 16));
                    w = PosGetLight(posX + (xPosition * 16) - 1, posY, posZ + (zPosition * 16));

                    neightboursLoaded = true;
                    calcValue = CalcLightValue(posX, posY, posZ, n, s, e, w, t, b);
                }
                
                if (calcValue != savedValue)
                {
                    PosSetLight(posX + xPosition * 16, posY, posZ + zPosition * 16, calcValue);
                    
                    if (capacity < 32762) 
                    {
                        if (calcValue > savedValue)
                        {
                            if (!neightboursLoaded)
                            {
                                n = PosGetLight(posX + (xPosition * 16), posY, posZ + (zPosition * 16) + 1);
                                s = PosGetLight(posX + (xPosition * 16), posY, posZ + (zPosition * 16) - 1);
                                t = PosGetLight(posX + (xPosition * 16), posY + 1, posZ + (zPosition * 16));
                                b = PosGetLight(posX + (xPosition * 16), posY - 1, posZ + (zPosition * 16));
                                e = PosGetLight(posX + (xPosition * 16) + 1, posY, posZ + (zPosition * 16));
                                w = PosGetLight(posX + (xPosition * 16) - 1, posY, posZ + (zPosition * 16));

                            }
                            
                            //calc distance from staret if (
                            int diffX = Math.Abs(posX - x);
                            int diffY = Math.Abs(posY - y);
                            int diffZ = Math.Abs(posZ - z); 
                            
                            if (diffX + diffY + diffZ < 17)
                            {
                                if (n < calcValue)
                                {
                                    collection [capacity++] = new BlockLightUpdate(posX, posY, posZ + 1, calcValue);
                                }
                                if (s < calcValue)
                                {
                                    collection [capacity++] = new BlockLightUpdate(posX, posY, posZ - 1, calcValue);
                                }
                                if (e < calcValue)
                                {
                                    collection [capacity++] = new BlockLightUpdate(posX + 1, posY, posZ, calcValue);
                                }
                                if (w < calcValue)
                                {
                                    collection [capacity++] = new BlockLightUpdate(posX - 1, posY, posZ, calcValue);
                                }
                                if (t < calcValue)
                                {
                                    collection [capacity++] = new BlockLightUpdate(posX, posY + 1, posZ, calcValue);
                                }
                                if (b < calcValue)
                                {
                                    collection [capacity++] = new BlockLightUpdate(posX, posY - 1, posZ, calcValue);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        private int CalcLightValue(int x, int y, int z, int n, int s, int e, int w, int t, int b)
        {
            int level = 0;
            
            int opacity = PosGetBlock(x + xPosition * 16, y, z + zPosition * 16).LightOpacity;

            if (xPosition == 9 && zPosition == 8 && x == 0  && z == 0 && y >= 192 && y <= 194)
            {
                var i = 0;
            }


            if (opacity >= 15)
            {
                return 0;
            }
            
            n-=2;
            s-=2;
            e-=2;
            w-=2;
            t-=2;
            b-=2;
            
            if (n > level)
                level = n;
            if (s > level)
                level = s;
            if (t > level)
                level = t;
            if (b > level)
                level = b;
            if (e > level)
                level = e;
            if (w > level)
                level = w;
            
            return level;
        }

        #endregion
    }
}

