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

namespace AssemblyCSharp
{
	public class PerlinTerrainGenerator : IFirstPassGenerator
	{
		private string _seed;
		private IBiomeGenerator _biomeGenerator;
        private BiomeType _biomeType;

		private int SAMPLE_RATE_XZ = 4;
		private int SAMPLE_RATE_Y = 4;

		private INoise3D terrainNoise;
		private INoise3D caveNoise;
		private INoise3D hillNoise;
		private INoise3D mouintainNoise;

		public PerlinTerrainGenerator ()
		{
            this._biomeType = new BiomeType();
		}

		public void SetSeed(String seed) {
			this._seed = seed;

			if (this._seed != null) {

				terrainNoise = new BrownianNoise3D(new PerlinNoise(this._seed.GetHashCode()), 5);
				caveNoise = new BrownianNoise3D(new PerlinNoise(this._seed.GetHashCode() + 3));
				hillNoise = new BrownianNoise3D(new PerlinNoise(this._seed.GetHashCode() + 4), 5);
				mouintainNoise = new BrownianNoise3D(new PerlinNoise(this._seed.GetHashCode() + 5), 8);
			}
		}

		public void SetBiomeGenerator(IBiomeGenerator generator) {
			this._biomeGenerator = generator;
		}



        public void GenerateChunk(Chunk2 chunk) 
        {
            int sectionSize = 16;
            int posX = chunk.xPosition * 16;
            int posZ = chunk.zPosition * 16;
            int heightY = chunk.yHeight;
            byte[] data = new byte[sectionSize * sectionSize * heightY];
            byte[] geometryData = new byte[sectionSize * sectionSize * heightY];


            float[,,] densityMap = new float[sectionSize + 1, heightY + 1, sectionSize + 1];
            
            for (int x = 0; x <= sectionSize; x += SAMPLE_RATE_XZ)
            {
                for (int z = 0; z <= sectionSize; z += SAMPLE_RATE_XZ)
                {

                    float temperature = this._biomeGenerator.GetTemperatureAt(posX + x, posZ + z);
                    float humidity = this._biomeGenerator.GetHumidityAt(posX + x, posZ +z);
                    float seaLevel = this._biomeGenerator.GetSeaAt(posX + x, posZ +z);
                    float terrainHeight = this._biomeGenerator.GetHeightBiomeAt(posX + x, posZ +z) / 10f;
                    float cz1 = Mathf.Clamp((1600f - z) / 1200f, 0, 2f);
                    temperature = Mathf.Clamp01(temperature * cz1);

                    for (int y = 0; y <= heightY; y += SAMPLE_RATE_Y)
                    {
                        densityMap [x, y, z] = CalculateDensity(posX + x, y, posZ + z, temperature, humidity, seaLevel, terrainHeight);
                    }
                }
            }
            
            triLerpDensityMap(densityMap);
            
            for (int x = 0; x < sectionSize; x++)
            {
                for (int z = 0; z < sectionSize; z++)
                {

                    float temperature = this._biomeGenerator.GetTemperatureAt(posX + x, posZ + z);
                    float humidity = this._biomeGenerator.GetHumidityAt(posX + x, posZ +z);
                    float seaLevel = this._biomeGenerator.GetSeaAt(posX + x, posZ +z);
                    float terrainHeight = this._biomeGenerator.GetHeightBiomeAt(posX + x, posZ +z) / 10f;
                    float cz1 = Mathf.Clamp((1600f - z) / 1200f, 0, 2f);
                    temperature = Mathf.Clamp01(temperature * cz1);
                    BiomeType.Biome bio;

                    if (seaLevel < 0.6)
                    {
                        bio = BiomeType.Biome.Ocean;
                    } 
                    else if (terrainHeight <= 0.2 && seaLevel <= 0.7)
                    {
                        bio = BiomeType.Biome.Desert;
                    }
                    else 
                    {
                        bio = this._biomeType.GetBiome((int)(temperature * 10), (int)(humidity * 10));
                    }
                   
                    chunk.SetBiomeMap(x, z, (byte)bio);


                    //BiomeType.Biome type = this._biomeGenerator.GetBiomeAt(posX + x, posZ + z);
                    //chunk.SetBiomeMap(x, z, (byte)type);

                    int firstBlockHeight = -1;
                    BlockType previousBlock = BlockType.Air;

                    for (int y = heightY - 1; y >= 0; y--)
                    {



                        if (y <= 1) {
                            SetData(data, x, y, z, BlockType.Stone);
                            geometryData[x + 16 * (z + 16 * y)] = (byte)BlockShape.Cube;
                            if (firstBlockHeight == -1)
                            {
                                firstBlockHeight = y;
                                chunk.SetHeightMap(x, z, (byte)(y + 1));
                            }
                            continue;
                        }
                            
                        if (y <= 32)
                        {
                            //ocean
                            //row + Self.width * (col + Self.height * layer)
                            //x + 16 * (z * 16 + y)
                            //16 + 16 *(256 + 16)
                            //16 + 16 * (512)
                            SetData(data, x, y, z, BlockType.Water);
                            geometryData[x + 16 * (z + 16 * y)] = (byte)BlockShape.Cube;
                            chunk.containsWater = true;
                            previousBlock = BlockType.Water;
                            //temp - removewhen we do water better
                           /* if (firstBlockHeight == -1)
                            {
                                firstBlockHeight = y;
                                chunk.SetHeightMap(x, z, (byte)(y + 1));
                            }
                            continue;*/
                        }
                        
                        float dens = densityMap [x, y, z];
                        
                        if ((dens >= 0 && dens < 32))
                        {
                            
                            // Some block was set...                       
                            if (calcCaveDensity(posX + x, y, posZ + z) > -0.7)
                            {
                                geometryData[x + 16 * (z + 16 * y)] = (byte)CalculateGeometry(firstBlockHeight, previousBlock, dens);

                                if (firstBlockHeight == -1)
                                {
                                    firstBlockHeight = y;
                                    chunk.SetHeightMap(x, z, (byte)(y + 1));
                                } 
                                SetBlock(x, y, z, firstBlockHeight, bio, data, previousBlock);
                            } else
                            {
                                SetData(data, x, y, z, BlockType.Air);
                                //data [x + 16 * (z + 16 * y)] = (byte)BlockType.Air;
                                previousBlock = BlockType.Air;
                            }
                            
                            continue;
                        } else if (dens >= 32)
                        {
                            // Some block was set...
                            if (calcCaveDensity(posX + x, y, posZ + z) > -0.4)
                            {
                                geometryData[x + 16 * (z + 16 * y)] = (byte)CalculateGeometry(firstBlockHeight, previousBlock, dens);

                                if (firstBlockHeight == -1)
                                {
                                    firstBlockHeight = y;
                                    chunk.SetHeightMap(x, z, (byte)(y + 1));
                                }
                                SetBlock(x, y, z, firstBlockHeight, bio, data, previousBlock);
                            } else
                            {
                                SetData(data, x, y, z, BlockType.Air);
                                previousBlock = BlockType.Air;
                            }
                            
                            continue;
                        }
                    }
                }
            }

            chunk.SetData(data, geometryData);
        }


        private BlockShape CalculateGeometry(int firstBlockHeight, BlockType previousYBlock, float density)
        {
            if ((firstBlockHeight == -1 || previousYBlock == BlockType.Air) &&  density < 16)
            {
                return BlockShape.Slab;
            }
            return BlockShape.Cube;
        }

		private float CalculateDensity(int x, int y, int z, float temp, float humidity, float sea, float height) {


            float baseHeight = CalcBaseTerrain(x, z);

            float densityMountains = 0;
            float densityHills = 0;
            float densitySea = 1;

            if (sea <= 0.6)
            {
                if (height <= 0.2)
                {
            //        height = 0;
                }
                else 
                {
                    densitySea = 0;
                }
            } 

            if (sea <= 0.6)
            {
                densitySea = Mathf.Clamp01(((sea - 0.2f) * 2));
            }

            baseHeight = baseHeight * height;
            densityMountains = calcMountainDensity(x, y, z) * Mathf.Clamp01(height - 0.3f);
            densityHills = calcHillDensity(x, y, z) * Mathf.Clamp01(height - 0.3f); //( 1f - height);
            			
			int plateauArea = (int) (256 * 0.10);
			float flatten = Mathf.Clamp01(((256 - 16) - y) / plateauArea);
			
            return -y + ((((32.0f + baseHeight * 256.0f)) * densitySea + densityMountains * 1024.0f + densityHills * 1024.0f) * flatten) * densitySea; 
		}

		private float CalcBaseTerrain(int x, int z)
		{
			return Mathf.Clamp((terrainNoise.Noise(0.004f * x, 0, 0.004f * z) + 1.0f) / 2.0f, 0, 1);
		}

		private float calcCaveDensity(double x, double y, double z) {

			float test = caveNoise.Noise(x * 0.02, y * 0.02, z * 0.02);
			return test;
		}

		private float calcMountainDensity(double x, double y, double z) {
			double x1 = x * 0.002;
			double y1 = y * 0.001;
			double z1 = z * 0.002;
			
			float result = mouintainNoise.Noise(x1, y1, z1);
			return result > 0.0 ? result : 0;
		}
		
		private float calcHillDensity(double x, double y, double z) {
			double x1 = x * 0.008;
			double y1 = y * 0.006;
			double z1 = z * 0.008;
			
			float result = hillNoise.Noise(x1, y1, z1) - 0.1f;
			return result > 0.0 ? result : 0;
		}
		
        private void triLerpDensityMap(float[,,] densityMap) {
            int sectionSize = 16;
            
            for (int x = 0; x < 16; x++) {
                for (int y = 0; y < 256; y++) {
                    for (int z = 0; z < 16; z++) {
                        if (!(x % SAMPLE_RATE_XZ == 0 && y % SAMPLE_RATE_Y == 0 && z % SAMPLE_RATE_XZ == 0)) {
                            int offsetX = (x / SAMPLE_RATE_XZ) * SAMPLE_RATE_XZ;
                            int offsetY = (y / SAMPLE_RATE_Y) * SAMPLE_RATE_Y;
                            int offsetZ = (z / SAMPLE_RATE_XZ) * SAMPLE_RATE_XZ;
                            densityMap[x, y, z] = (float)PMath.TriLerp(x, y, z,
                               densityMap[offsetX, offsetY, offsetZ],
                               densityMap[offsetX, SAMPLE_RATE_Y + offsetY, offsetZ],
                               densityMap[offsetX, offsetY, offsetZ + SAMPLE_RATE_XZ],
                               densityMap[offsetX, offsetY + SAMPLE_RATE_Y, offsetZ + SAMPLE_RATE_XZ],
                               densityMap[SAMPLE_RATE_XZ + offsetX, offsetY, offsetZ],
                               densityMap[SAMPLE_RATE_XZ + offsetX, offsetY + SAMPLE_RATE_Y, offsetZ],
                               densityMap[SAMPLE_RATE_XZ + offsetX, offsetY, offsetZ + SAMPLE_RATE_XZ],
                               densityMap[SAMPLE_RATE_XZ + offsetX, offsetY + SAMPLE_RATE_Y, offsetZ + SAMPLE_RATE_XZ],
                               offsetX, SAMPLE_RATE_XZ + offsetX, offsetY, SAMPLE_RATE_Y + offsetY, offsetZ, offsetZ + SAMPLE_RATE_XZ);
                        }
                    }
                }
            }
        }

		private void SetBlock(int x, int y, int z, int firstBlock, BiomeType.Biome biome, byte[] data, BlockType previousBlock) {
			
            int depth = y - firstBlock;
			
            BlockType block = BlockType.Air;

            switch (biome)
            {	
                case BiomeType.Biome.Desert:
                    block = BlockType.Sand;
                    break;
                case BiomeType.Biome.GrassLand:
                    if (depth == 1)
                        block = BlockType.Grass;
                    else if (depth < 3)
                        block = BlockType.Dirt;
                    else 
                        block = BlockType.Stone;
                    break;
                case BiomeType.Biome.Ice:
                    if (depth > 3)
                        block = BlockType.Stone;
                    else 
                        block = BlockType.Snow; //TODO NEED ICE
                    break;
                case BiomeType.Biome.Ocean:
                    block = BlockType.Sand;
                    break;
                case BiomeType.Biome.RainForest:
                    block = BlockType.Dirt; //TODO 
                    break;
                case BiomeType.Biome.Savana:
                    block = BlockType.Sand; //TODO
                    break;
                case BiomeType.Biome.SeasonalForest:
                    block = BlockType.Dirt; //TODO
                    break;
                case BiomeType.Biome.ShrubLand:
                    block = BlockType.Sand; //TODO
                    break;
                case BiomeType.Biome.Swamp:
                    block = BlockType.Dirt; //TODO
                    break;
                case BiomeType.Biome.TemperateForest:
                    block = BlockType.Grass;
                    break;
                case BiomeType.Biome.Tiaga:
                    block = BlockType.Snow;
                    break;
                case BiomeType.Biome.TropicalForest:
                    block = BlockType.Grass; //TODO
                    break;
                case BiomeType.Biome.Tundra:
                    block = BlockType.Snow;
                    break;
                case BiomeType.Biome.Woodland:
                    block = BlockType.Grass;
                    break;
                default: 
                    block = BlockType.Snow;
                    break;
            }

            previousBlock = block;
            SetData(data, x, y, z, block);
        }

        private void SetData(byte [] data, int x, int y, int z, BlockType block)
        {
            //y << 8 | z<< 4 | x
            data [y << 8 | z << 4 | x] = (byte)block;//* (z + 16 * y)] = (byte)block;
            //data [x + 16 * (z + 16 * y)] = (byte)block;
        }
    }

}


