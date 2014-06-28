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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{

	public enum BlockType 
	{
		Air = 0,
		Dirt = 1,
		Grass = 2,
		Stone = 3,
		Sand = 4,
		Snow = 5,
		Water = 6
	}

	public static class BlockManager
	{
		private static Block[] blockList = new Block[] {
				
			new Block (BlockType.Air, 3, 3),
			new Block (BlockType.Dirt,	0, 3),
			new Block (BlockType.Grass,0, 2),
			new Block (BlockType.Stone,0, 1),
			new Block (BlockType.Sand, 1, 3),
			new Block (BlockType.Snow, 3, 3),
			new Block (BlockType.Water, 0, 0),
			new Block (BlockType.Snow, 1, 2),
			new Block (BlockType.Snow, 1, 3),
			new Block (BlockType.Snow, 2, 0),
			new Block (BlockType.Snow, 2, 1)
		};

		public static Block GetBlock(byte id) 
		{
			return blockList [id];
		}

		public static Vector2 GetTexture(byte id)
		{
			return blockList [id].Texture;
		}
	}
}

