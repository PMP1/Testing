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
	public interface IBiomeGenerator
	{
		float getHumidityAt(int x, int z);

		float getTemperatureAt(int x, int z);

		//should we also care about base height?

		float getBiomeAt(int x, int z);
	}
}
