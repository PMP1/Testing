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
using System.Collections.Generic;
using UnityEngine;

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

        public bool RequiresGORedraw = false;

        public bool updateMesh = false; //rebuild all
        public bool updateDayLight = false; //update daylight level
        public bool updateLight = false; //update colours


        public List<Vector3> vertices = new List<Vector3> ();
        public List<int> triangles = new List<int> ();
        public List<Vector2> uvs = new List<Vector2> ();
        public List<Vector3> colliderVertices = new List<Vector3> ();
        public List<int> colliderTriangles = new List<int> ();
        public List<Color> colors = new List<Color> ();

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

        public void SetMeshData(List<Vector3> verts, List<int> tri, List<Vector2> uv)
        {
            this.vertices = new List<Vector3>(verts);
            this.triangles = new List<int>(tri);
            this.uvs = new List<Vector2>(uv);
        }

        public void SetColliderData(List<Vector3> verts, List<int> tri)
        {
            this.colliderTriangles = new List<int>(tri);
            this.colliderVertices = new List<Vector3>(verts);
        }

        public void SetColorData(List<Color> color)
        {
            this.colors = new List<Color>(color);
        }

        /*public void ClearGOTempData() 
        {
            this.vertices.Clear();
            this.triangles.Clear();
            this.uvs.Clear();
            this.colliderTriangles.Clear();
            this.colliderVertices.Clear();
            this.colors.Clear();
        }*/

        public void GenerateGO() 
        {
            System.DateTime startCreateGO = System.DateTime.Now;
            GameObject newSectionGO = chunk.manager.world.CreateSectionGO(chunk, this);

            
            this.sectionGO = newSectionGO.GetComponent("SectionGO") as SectionGO;
            this.sectionGO.world = chunk.manager.world;
            this.sectionGO.section = this;
            //this.sectionGO.updateMesh = true;
            //this.sectionGO.SetCollider(colliderVertices, colliderTriangles);
            this.sectionGO.SetDaylight(chunk.manager.world.time.GetDaylightLevel());
            this.sectionGO.name = chunk.xPosition.ToString() + ":" + chunk.zPosition.ToString() + ":" + (this.posY << 4).ToString();

            this.updateMesh = true;


            StatsEngine.SectionGoCreate += (float)System.DateTime.Now.Subtract(startCreateGO).TotalSeconds;
        }

        /*public void SetNewGOMesh()
        {
            this.sectionGO.SetMesh(uvs, vertices, triangles, colors);
        }

        public void SetNewGOCollider()
        {
            this.sectionGO.SetCollider(colliderVertices, colliderTriangles);
        }*/


        
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
            this.SetDaylightData(x + 16 * (z + 16 * y), level);
        }

        public void SetDaylightData(int xyz, int level)
        {
            this.daylightData [xyz] = (byte)level;
        }

        public byte GetDaylightValue(int x, int y, int z)
        {
            return this.GetDaylightValue(x + 16 * (z + 16 * y));
        }

        private byte GetDaylightValue(int xyz)
        {
            return this.daylightData[xyz];
        }

        #endregion


    }
}

