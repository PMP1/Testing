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

namespace AssemblyCSharp
{
    public static class StatsEngine
    {
        private static float timeLeft = 0f;
        private static float updateInterval = 0.5f;


        private static int chunkUpdates = 0;
        private static float chunkUpdateTime = 0;
        private static List<int> chunkUpdateHistory = new List<int>();
        private static List<float> chunkUpdateTimeHistory = new List<float>();

        //also want to record time spent lighting

        private static int sectionUpdates = 0;
        private static float sectionUpdateTime = 0;
        private static List<int> sectionUpdateHistory = new List<int>();
        private static List<float> sectionUpdateTimeHistory = new List<float>();


        public static float ChunksLoaded = 0;
        public static float TotalLoadTime = 0;
        public static float ChunkGenTime = 0;
        public static float ChunkRenderTime = 0;

        public static float ChunkSpreadLight = 0;

        public static float SectionMeshGen = 0;
        public static float SectionGoCreate = 0;
        public static float SectionSmoothLighting = 0;
        public static float SectionColliderGen = 0;

        public static float ChunkDaylightFill = 0;


        public static float PrevChunksLoaded = 0;
        public static float PrevChunkGenTime = 0;
        public static float PrevChunkDaylightFill = 0;


        public static int QueueLength = 0;
        public static int RenderQueueLength = 0;


        public static void UpdateTime (float next)
        {
            timeLeft -= next;
            //accum += Time.timeScale/Time.deltaTime;
            //++frames;
            
            // Interval ended - update GUI text and start new interval
            if( timeLeft <= 0.0 ) {

                chunkUpdateHistory.Add(chunkUpdates);
                chunkUpdateTimeHistory.Add(chunkUpdateTime);

                sectionUpdateHistory.Add(sectionUpdates);
                sectionUpdateTimeHistory.Add(sectionUpdateTime);


                PrevChunkGenTime = ChunkGenTime;
                PrevChunkDaylightFill = ChunkDaylightFill;
                PrevChunksLoaded = ChunksLoaded;
                //ChunkGenTime = 0;
                //ChunkDaylightFill = 0;
                //ChunksLoaded = 0;

                chunkUpdates = 0;
                chunkUpdateTime = 0;
                sectionUpdates = 0;
                sectionUpdateTime = 0;
                
                if (chunkUpdateHistory.Count > 20)
                {
                    chunkUpdateHistory.RemoveAt(0);
                }
                
                if (chunkUpdateTimeHistory.Count > 20)
                {
                    chunkUpdateTimeHistory.RemoveAt(0);
                }

                if (sectionUpdateHistory.Count > 20)
                {
                    sectionUpdateHistory.RemoveAt(0);
                }

                if (sectionUpdateTimeHistory.Count > 20)
                {
                    sectionUpdateTimeHistory.RemoveAt(0);
                }

                timeLeft = updateInterval;
                //accum = 0.0F;
                //frames = 0;
            }
        }

        public static void UpdateSection(float time)
        {
            sectionUpdates ++;
            sectionUpdateTime += time;
        }

        public static List<int> GetSectionHistory() 
        {
            return sectionUpdateHistory;
        }
        
        public static int GetSectionUpdate() 
        {
            return sectionUpdateHistory[sectionUpdateHistory.Count - 1];
        }

        public static List<float> GetSectionUpdateTimeHistory() 
        {
            return sectionUpdateTimeHistory;
        }
        
        public static float GetSectionUpdateTime() 
        {
            return sectionUpdateTimeHistory[sectionUpdateTimeHistory.Count - 1];
        }

     



    }
}

