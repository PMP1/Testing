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
namespace AssemblyCSharp
{
    public class Section2
    {

        byte[] data;
        byte[] daylightData;
        byte[] lightData;

        public int posY; 

        public Chunk2 chunk;
        public SectionGO sectionGO;

        public Section2(int y, Chunk2 cnk)
        {
            this.posY = y;
            data = new byte[4096];
            daylightData = new byte[4096];
            lightData = new byte[4096];
            chunk = cnk;
        }

        public Section2(int y, byte[] data)
        {
            this.posY = y;
            this.data = data;
        }

        #region Block Access

        public void SetBlockId(int x, int y, int z, byte block) 
        {
            this.SetBlockId(x + 16 * (z + 16 * y), block);
        }

        public void SetBlockId(int xyz, byte block)
        {
            try 
            {
            this.data [xyz] = block;
            //this may be a good place to check to see if an update is needed?
            }
            catch
            {
                int i = 0;

            }
        }

        public byte GetBlockId(int x, int y, int z)
        {
            return this.GetBlockId(x + 16 * (z + 16 * y));
        }

        public byte GetBlockId(int xyz)
        {
            return this.data[xyz];
        }

        #endregion

        #region Light Access
        public void SetDatlightData(int x, int  y, int z, int level) 
        {
            this.SetDAylightData(x + 16 * (z + 16 * y), level);
        }

        public void SetDAylightData(int xyz, int level)
        {
            this.daylightData [xyz] = (byte)level;
        }

        #endregion
    }
}

