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
	public class Block
	{
		public BlockType BlockType { get; set; }
		public Vector2 Texture {get; set; }
		public Vector2 TextureTop {get; set; }

		public Block(BlockType type, int texX, int texY)
		{
			BlockType = type;
			Texture = new Vector2 (texX, texY);
		}

		public Block(BlockType type, int texX, int texY, int texXTop, int texYTop)
		{
			BlockType = type;
			Texture = new Vector2 (texX, texY);
			Texture = new Vector2 (texXTop, texYTop);
		}
	}
}

