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

		private int SAMPLE_RATE_XZ = 4;
		private int SAMPLE_RATE_Y = 4;

		private INoise3D terrainNoise;
		private INoise3D caveNoise;
		private INoise3D hillNoise;
		private INoise3D mouintainNoise;

		public PerlinTerrainGenerator ()
		{

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
            
            float[,,] densityMap = new float[sectionSize + 1, heightY + 1, sectionSize + 1];
            
            for (int x = 0; x <= sectionSize; x += SAMPLE_RATE_XZ)
            {
                for (int z = 0; z <= sectionSize; z += SAMPLE_RATE_XZ)
                {
                    for (int y = 0; y <= heightY; y += SAMPLE_RATE_Y)
                    {
                        densityMap [x, y, z] = CalculateDensity(posX + x, y, posZ + z);
                    }
                }
            }
            
            triLerpDensityMap(densityMap);
            
            for (int x = 0; x < sectionSize; x++)
            {
                for (int z = 0; z < sectionSize; z++)
                {
                    BiomeType type = this._biomeGenerator.GetBiomeAt(posX + x, posZ + z);
                    chunk.SetBiomeMap(x, z, (byte)type);

                    int firstBlockHeight = -1;
                    
                    for (int y = heightY - 1; y >= 0; y--)
                    {
                            
                        if (y <= 32)
                        {
                            //ocean
                            //row + Self.width * (col + Self.height * layer)
                            //x + 16 * (z * 16 + y)
                            //16 + 16 *(256 + 16)
                            //16 + 16 * (512)
                            data [x + 16 * (z + 16 * y)] = (byte)BlockType.Water;
                            chunk.containsWater = true;

                            //temp - removewhen we do water better
                            if (firstBlockHeight == -1)
                            {
                                firstBlockHeight = y;
                                chunk.SetHeightMap(x, z, (byte)(y + 1));
                            }
                            continue;
                        }
                        
                        float dens = densityMap [x, y, z];
                        
                        if ((dens >= 0 && dens < 32))
                        {
                            
                            // Some block was set...

                            
                            if (calcCaveDensity(posX + x, y, posZ + z) > -0.7)
                            {
                                if (firstBlockHeight == -1)
                                {
                                    firstBlockHeight = y;
                                    chunk.SetHeightMap(x, z, (byte)(y + 1));
                                }
                                SetBlock(x, y, z, firstBlockHeight, type, data);
                            } else
                            {
                                data [x + 16 * (z + 16 * y)] = (byte)BlockType.Air;
                            }
                            
                            continue;
                        } else if (dens >= 32)
                        {
                            // Some block was set...

                            
                            if (calcCaveDensity(posX + x, y, posZ + z) > -0.4)
                            {
                                if (firstBlockHeight == -1)
                                {
                                    firstBlockHeight = y;
                                    chunk.SetHeightMap(x, z, (byte)(y + 1));
                                }
                                SetBlock(x, y, z, firstBlockHeight, type, data);
                            } else
                            {
                                data [x + 16 * (z + 16 * y)] = (byte)BlockType.Air;
                            }
                            
                            continue;
                        }
                    }
                }
            }

            chunk.SetData(data);
        }

		private float CalculateDensity(int x, int y, int z) {

			float height = CalcBaseTerrain(x, z);
			//float ocean = calcOceanTerrain(x, z);
			//float river = calcRiverTerrain(x, z);
			
			float temp = this._biomeGenerator.GetTemperatureAt(x, z);
			float humidity = this._biomeGenerator.GetHumidityAt(x, z) * temp;
			
			Vector2 distanceToMountainBiome = new Vector2(temp - 0.25f, humidity - 0.35f);
			
			float mIntens = Mathf.Clamp01(1.0f - distanceToMountainBiome.magnitude * 3.0f);
			float densityMountains = calcMountainDensity(x, y, z) * mIntens;
			float densityHills = calcHillDensity(x, y, z) * (1.0f - mIntens);
			
			int plateauArea = (int) (256 * 0.10);
			float flatten = Mathf.Clamp01(((256 - 16) - y) / plateauArea);
			
			return -y + (((32.0f + height * 32.0f))  + densityMountains * 1024.0f + densityHills * 128.0f) * flatten;
            //-256 + (((32 + 32)) + 0 + 
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

		private void SetBlock(int x, int y, int z, int firstBlock, BiomeType biome, byte[] data) {
			
            int depth = y - firstBlock;
			
            switch (biome)
            {	
                case BiomeType.GrassLand:
                case BiomeType.Mountain:
                case BiomeType.SeasonalForest:
                case BiomeType.Woodland:
                    if (depth <= 3 && y > 28 && y <= 32)
                    {
                        data [x + 16 * (z + 16 * y)] = (byte)BlockType.Sand;
                    } else if (depth == 0 && y > 32 && y < 170)
                    {
                        data [x + 16 * (z + 16 * y)] = (byte)BlockType.Grass;
                    } else if (depth == 0 && y >= 240)
                    {
                        data [x + 16 * (z + 16 * y)] = (byte)BlockType.Snow;
                    } else
                    {
                        data [x + 16 * (z + 16 * y)] = (byte)BlockType.Stone;
                    } 
                    break;
                case BiomeType.Desert:
                    data [x + 16 * (z + 16 * y)] = (byte)BlockType.Sand;
                    break;
                case BiomeType.Tundra:
                    data [x + 16 * (z + 16 * y)] = (byte)BlockType.Snow;
                    break;
                default: 
                    data [x + 16 * (z + 16 * y)] = (byte)BlockType.Snow;
                    break;
            }
        }
    }
}


