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
	public static class PMath
	{
		public static int FastFloor(double x)
		{
			int xi = (int)x;
			return x < xi ? xi - 1 : xi;
		}

		/*public static float fastFloor(float d) {
			int i = (int) d;
			return (d < 0 && d != i) ? i - 1 : i;
		}*/
		
		/// <summary>
		/// Linear interpolation returning a value based from position coordinates
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="x1">The first x value.</param>
		/// <param name="x2">The second x value.</param>
		/// <param name="v00">value at x1</param>
		/// <param name="v01">value at x2</param>
		public static double lerp(double x, double x1, double x2, double v00, double v01) {
			return ((x2 - x) / (x2 - x1)) * v00 + ((x - x1) / (x2 - x1)) * v01;
		}
		
		public static double lerp(double x1, double x2, double p) {
			return x1 * (1.0 - p) + x2 * p;
		}
		
		public static double lerpf(double x1, double x2, double p) {
			return x1 * (1.0f - p) + x2 * p;
		}
		
		
		/// <summary>
		/// Trilinear interpolation returning value at given point
		/// </summary>
		/// <returns>Value at x,y,z</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="z">The z coordinate.</param>
		/// <param name="v000">Value at x1, y1, z1</param>
		/// <param name="v001">Value at x1, y2, z1.</param>
		/// <param name="v010">Value at x1, y1, z2.</param>
		/// <param name="v011">Value at x1, y2, z2.</param>
		/// <param name="v100">Value at x2, y1, z1.</param>
		/// <param name="v101">Value at x2, y2, z1.</param>
		/// <param name="v110">Value at x2, y1, z2.</param>
		/// <param name="v111">Value at x2, y2, z2.</param>
		/// <param name="x1">The first x value.</param>
		/// <param name="x2">The second x value.</param>
		/// <param name="y1">The first y value.</param>
		/// <param name="y2">The second y value.</param>
		/// <param name="z1">The first z value.</param>
		/// <param name="z2">The second z value.</param>
		public static double triLerp(double x, double y, double z, double v000, double v001, double v010, double v011, double v100, double v101, double v110, double v111,
		                            double x1, double x2, double y1, double y2, double z1, double z2) {
			//do x's
			double x00 = lerp(x, x1, x2, v000, v100);
			double x10 = lerp(x, x1, x2, v010, v110);
			double x01 = lerp(x, x1, x2, v001, v101);
			double x11 = lerp(x, x1, x2, v011, v111);

			//do y's
			double r0 = lerp(y, y1, y2, x00, x01);
			double r1 = lerp(y, y1, y2, x10, x11);

			//do z's
			return lerp(z, z1, z2, r0, r1);
		}
	}
}

