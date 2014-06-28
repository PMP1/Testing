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

        //16*16 = 256

        Section2[] sections;

        byte[] biomeMap;

        byte[] heightMap;
        int maxHeight;
        int minHeight;

        bool[] daylightColumnUpdates;


        public bool containsWater = false;


        //section = 4096

        //all 65536
        public Chunk2(int x, int z)
        {
            zPosition = x;
            zPosition = z;

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
                        int xyz = x + 16 * (y + 256 * z);
                        byte block = chunkData[xyz];//TODO CHECK THIS

                        if (block != 0) {

                            int section = height / 16;
                            if (sections[section] == null) {
                                sections[section] = new Section2();
                            }

                            sections[section].SetBlockId(xyz, block);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A quickway to update a height, used during generation
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        /// <param name="height">Height.</param>
        public void setHeightMap(int x, int z, byte height) {
            this.heightMap[ x + 16 * z] = height;
        }

        public void setBiomeMap(int x, int z, byte biome) {
            this.biomeMap[ x + 16 * z] = biome;
        }
    }
}

