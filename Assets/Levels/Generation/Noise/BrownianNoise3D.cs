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
namespace AssemblyCSharp
{
	public class BrownianNoise3D : BrownianNoise, INoise3D
	{
		private INoise3D noiseGen; //we use another noise gen to do the work

		public BrownianNoise3D(INoise3D noiseGen) {
			this.noiseGen = noiseGen; 
		}

		public BrownianNoise3D(INoise3D noiseGen, int octaves) {
			this.noiseGen = noiseGen;
			SetOctaves(octaves);
		}

		public float Noise(double x, double y, double z) {
			double result = 0.0;
			
			double workingX = x;
			double workingY = y;
			double workingZ = z;
			for (int i = 0; i < GetOctaves(); i++) {
				result += noiseGen.Noise(workingX, workingY, workingZ) * GetSpectralWeight(i);
				
				workingX *= GetLacunarity();
				workingY *= GetLacunarity();
				workingZ *= GetLacunarity();
			}
			
			return (float)result;
		}
	}
}

