//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18063
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class BiomeGenerator : IBiomeGenerator
	{
		private INoise3D temperatureNoise;
		private INoise3D humidityNoise;
        private INoise3D heightNoise;
        private INoise3D seaNoise;
		
		public BiomeGenerator (String seed)
		{
			temperatureNoise = new BrownianNoise3D(new PerlinNoise(seed.GetHashCode() + 1));
			humidityNoise = new BrownianNoise3D(new PerlinNoise(seed.GetHashCode() + 2));
            heightNoise = new BrownianNoise3D(new PerlinNoise(seed.GetHashCode() + 3));
            seaNoise = new BrownianNoise3D(new PerlinNoise(seed.GetHashCode() + 4), 1);
		}
		
		public float GetHumidityAt(int x, int z) {
			float result = humidityNoise.Noise(x * 0.005f, 0f, 0.005f * z);
			return (float) Mathf.Clamp((result + 1.0f) / 2.0f, 0, 1);
		}
		
		public float GetTemperatureAt(int x, int z) {
			float result = (float)temperatureNoise.Noise(x * 0.005, 0, 0.005 * z);
			return (float) Mathf.Clamp((result + 1.0f) / 2.0f, 0, 1);
		}
		
		public float GetHeightAt(int x, int z) {
			float result = heightNoise.Noise(x * 0.005, 0, 0.005 * z);
			//return Mathf.Clamp01(result * 8f);

            //need to clamp seas at xz +- 200
            //int plateauArea = (int) (256 * 0.10);
            //float flatten = Mathf.Clamp01(((256 - 16) - y) / plateauArea);


            return Mathf.Clamp01(result);
		}

        public float GetSeaAt(int x, int z)
        {
            //int absX = Math.Abs(x);
            //int absZ = Math.Abs(z);

            //float cX = Mathf.Clamp01(100f - absX / 20f);
            //float cz = Mathf.Clamp01(100 - absZ / 20);

            //float edge = Mathf.Clamp01(cX + cz);
            float result = heightNoise.Noise(x * 0.001, 0, 0.001 * z);
            //float result = Mathf.Clamp01(heightNoise.Noise(x * 0.001, 0, 0.001 * z));
            return (float) Mathf.Clamp01((result + 1.0f) / 2.0f);
            return result;
        }

		
		public int GetHeightBiomeAt(int x, int z) {
			float height = GetHeightAt(x, z);
			
            return (int)(height * 10);
            /*
			if (height < 0.4) {
				return 1; //water
			} else if (height >= 0.4 && height <= 0.5) {
				return 2;//beach
			} else if (height > 0.7) {
				return 3; //mountins
			}
			
			return 4;//plains*/
		}
		
		public BiomeType GetBiomeAt(int x, int z) {
			float temp = GetTemperatureAt(x, z);
			float humidity = GetHumidityAt(x, z) * temp;
			
			if (temp >= 0.5 && humidity < 0.3) {
				return BiomeType.Desert;
			} else if (humidity >= 0.3 && humidity <= 0.6 && temp >= 0.5) {
				return BiomeType.GrassLand;
			} else if (temp <= 0.3 && humidity > 0.5) {
				return BiomeType.Tundra;
			} else if (humidity >= 0.2 && humidity <= 0.6 && temp < 0.5) {
				return BiomeType.Mountain; //mountin shoudl really be based on height...
			}


			
			return BiomeType.SeasonalForest;
		}
	}
}

