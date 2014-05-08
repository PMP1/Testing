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
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
	public abstract class AbstractWorldGenerator
	{
	
		private String seed;
		private IBiomeGenerator biomeGenerator;
		private List<IFirstPassGenerator> firstPassGenerators = new List<IFirstPassGenerator>();
		
		public AbstractWorldGenerator ()
		{
		}
		
		public void SetSeed(string newSeed) {
			seed = newSeed;
			
			biomeGenerator = new BiomeGenerator(seed);
			
			foreach (IFirstPassGenerator generator in firstPassGenerators) {
				generator.SetBiomeGenerator(biomeGenerator);
				generator.SetSeed(newSeed);
			}
		}
		
		/// <summary>
		/// Register the specified generator.
		/// </summary>
		/// <param name="generator">Generator.</param>
		protected void Register(IFirstPassGenerator generator) {
			generator.SetBiomeGenerator(biomeGenerator);
			generator.SetSeed(seed);
			firstPassGenerators.Add(generator);
		} 
		
		/// <summary>
		/// Creates the chunk.
		/// </summary>
		/// <param name="chunk">Chunk.</param>
		public void CreateChunk(Chunk chunk) {
			foreach (IFirstPassGenerator generator in firstPassGenerators) {
				generator.GenerateChunk(chunk);
			}
		}
		
		//TODO add texture generation for debug
		public Color GetTexturePixel(string layerName, int x, int z) {
		
			switch (layerName) {
				case "Temperature":
					float temp = biomeGenerator.GetTemperatureAt(x, z);
					return new Color(temp, temp * 0.2f, temp * 0.2f);
					break;
				case "Humidity":
					float hum = biomeGenerator.GetHumidityAt(x, z);
					return new Color(hum * 0.2f, hum * 0.2f, hum);
					break;
			case "Terrain":
				int biome = biomeGenerator.GetBiomeAt(x, z);
				switch (biome) {
					
				case 1:
					return Color.yellow;
					break;
				case 2:
					return Color.green;
					break;
				case 3:
					return Color.white;
					break;
				case 4:
					return Color.gray;
					break;
				default:
					return new Color(0.2f,0.8f,0.2f);
					break;
				}
				return new Color(hum * 0.2f, hum * 0.2f, hum);
				break;
			case "Height":
				int height = biomeGenerator.GetHeightBiomeAt(x, z);
				switch (height) {
					
				case 1:
					return Color.blue;
					break;
				case 2:
					return Color.yellow;
					break;
				case 3:
					return Color.gray;
					break;
				case 4:
					return Color.green;
					break;
				default:
					return new Color(0.2f,0.8f,0.2f);
					break;
				}
				return new Color(biome * 0.2f, biome * 0.2f, biome);
				break;

			}
			return Color.black;
		}
	}
}
