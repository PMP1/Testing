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
		
		public BiomeGenerator (String seed)
		{
			temperatureNoise = new BrownianNoise3D(new PerlinNoise(seed.GetHashCode() + 1));
			humidityNoise = new BrownianNoise3D(new PerlinNoise(seed.GetHashCode() + 2));
		}
		
		public float GetHumidityAt(int x, int z) {
			float result = humidityNoise.Noise(x * 0.0005f, 0f, 0.0005f * z);
			return (float) Mathf.Clamp((result + 1.0f) / 2.0f, 0, 1);
		}
		
		public float GetTemperatureAt(int x, int z) {
			float result = (float)temperatureNoise.Noise(x * 0.0005, 0, 0.0005 * z);
			return (float) Mathf.Clamp((result + 1.0f) / 2.0f, 0, 1);
		}
		
		
		public int GetBiomeAt(int x, int z) {
			double temp = GetTemperatureAt(x, z);
			double humidity = GetHumidityAt(x, z) * temp;
			
			if (temp >= 0.5 && humidity < 0.3) {
				return 1;//desert
			} else if (humidity >= 0.3 && humidity <= 0.6 && temp >= 0.5) {
				return 2;//plains
			} else if (temp <= 0.3 && humidity > 0.5) {
				return 3;//snow
			} else if (humidity >= 0.2 && humidity <= 0.6 && temp < 0.5) {
				return 4; //mountin shoudl really be based on height...
			}
			
			return 5;
		}
	}
}

