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

		private int SAMPLE_RATE_XZ = 1;
		private int SAMPLE_RATE_Y = 1;

		private INoise3D terrainNoise;
		private INoise3D caveNoise;

		public PerlinTerrainGenerator ()
		{

		}

		public void SetSeed(String seed) {
			this._seed = seed;

			if (this._seed != null) {

				terrainNoise = new BrownianNoise3D(new PerlinNoise(this._seed.GetHashCode()));
				caveNoise = new BrownianNoise3D(new PerlinNoise(this._seed.GetHashCode() + 3));
			}
		}


		
		public void SetBiomeGenerator(IBiomeGenerator generator) {
			this._biomeGenerator = generator;
		}

		public void GenerateChunk(Chunk chunk) {

			float[,,] densityMap = new float[chunk.sectionSize,chunk.worldY, chunk.sectionSize];

			for (int x = 0; x < chunk.sectionSize; x += SAMPLE_RATE_XZ) {
				for (int z = 0; z < chunk.sectionSize; z += SAMPLE_RATE_XZ) {
					for (int y = 0; y < chunk.worldY; y += SAMPLE_RATE_Y) {
						densityMap[x,y,z] = CalculateDensity((chunk.chunkX * chunk.sectionSize) + x, y, (chunk.chunkZ * chunk.sectionSize) + z);
					}
				}
			}







			for (int x = 0; x < chunk.sectionSize; x++) {
				for (int z = 0; z < chunk.sectionSize; z++) {
					int type = this._biomeGenerator.GetBiomeAt ((chunk.chunkX * chunk.sectionSize) + x, (chunk.chunkZ * chunk.sectionSize) + z);
					int firstBlockHeight = -1;

					for (int y = chunk.worldY - 1; y >= 0; y--) {


						if (y <= 32) {
							//ocean
							chunk.data[x,y,z] = 3;
							//continue;
						}

						float dens = densityMap[x,y,z];

						if ((dens >= 0 && dens < 32)) {
							
							// Some block was set...
							if (firstBlockHeight == -1) {
								firstBlockHeight = y;
							}
							
							if (calcCaveDensity((chunk.chunkX * chunk.sectionSize) + x, y, (chunk.chunkZ * chunk.sectionSize) + z) > -0.7) {//-0.7) {
								//c.setBlock(x, y, z, stone);
								chunk.data[x,y,z] = 0;
							} else {
								chunk.data[x,y,z] = 0;
							}
							
							continue;
						} else {
							// Some block was set...
							if (firstBlockHeight == -1) {
								firstBlockHeight = y;
							}
							
							if (calcCaveDensity((chunk.chunkX * chunk.sectionSize) + x, y, (chunk.chunkZ * chunk.sectionSize) + z) > -0.6) {
								chunk.data[x,y,z] = 1;
							} else {
								chunk.data[x,y,z] = 0;
							}
							
							continue;
						}
					}
				}
			}
		}

		private float CalculateDensity(int x, int y, int z) {

			float height = CalcBaseTerrain(x, z);
			//float ocean = calcOceanTerrain(x, z);
			//float river = calcRiverTerrain(x, z);
			
			float temp = this._biomeGenerator.GetTemperatureAt(x, z);
			float humidity = this._biomeGenerator.GetHumidityAt(x, z) * temp;
			
			//Vector2f distanceToMountainBiome = new Vector2f(temp - 0.25f, humidity - 0.35f);
			
			//double mIntens = TeraMath.clamp(1.0 - distanceToMountainBiome.length() * 3.0);
			//double densityMountains = calcMountainDensity(x, y, z) * mIntens;
			//double densityHills = calcHillDensity(x, y, z) * (1.0 - mIntens);
			
			int plateauArea = (int) (256 * 0.10);
			float flatten = Mathf.Clamp01(((256 - 128) - y) / plateauArea);
			
			//return -y + (((32.0 + height * 32.0) * TeraMath.clamp(river + 0.25) * TeraMath.clamp(ocean + 0.25)) + densityMountains * 1024.0 + densityHills * 128.0) * flatten;
			return -y + ((32.0f + height * 32.0f) * 0.2f ) * flatten;
		}

		private float CalcBaseTerrain(int x, int z)
		{
			return Mathf.Clamp((terrainNoise.Noise(0.004f * x, 0, 0.004f * z) + 1.0f) / 2.0f, 0, 1);
		}

		private float calcCaveDensity(double x, double y, double z) {

			float test = caveNoise.Noise(x * 0.02, y * 0.02, z * 0.02);
			return test;
		}

	}
}

