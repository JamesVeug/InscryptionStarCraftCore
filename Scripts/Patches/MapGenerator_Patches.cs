using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Helpers.Extensions;
using StarCraftCore.Scripts.Regions;
using UnityEngine;

namespace StarCraftCore.Scripts.Patches
{
    [HarmonyPatch(typeof (MapGenerator), "GenerateScenery")]
    public class MapGenerator_GenerateScenery
    {
        private static float X_OFFSET = 0.45f;
        
        public static bool Prefix(ref List<SceneryElementData> __result, int randomSeed, RegionData region, List<NodeData> nodes, float mapLength, PredefinedScenery predefinedScenery)
        {
            if (region != MarSaraRegion.regionData)
            {
                return true;
            }
            List<SceneryElementData> list = new List<SceneryElementData>();
            if (predefinedScenery != null)
            {
                foreach (SceneryElementData sceneryElementData in predefinedScenery.scenery)
                {
                    list.Add(MapGenerator.InitializeSceneryElement(sceneryElementData, sceneryElementData.data));
                }
            }
            
            List<MapGenerator.SceneryAvoidPoint> nodeAndPathPoints = MapGenerator.GetNodeAndPathPoints(nodes);
            
            // Generate cliffs on edges of map
            GenerateCliffSceneryData(list, region, ref randomSeed, nodeAndPathPoints, mapLength);
            
            // Generate props in the middle of the map
            GenerateFillerSceneryData(list, region, ref randomSeed, nodeAndPathPoints, mapLength + 1);


            __result = list;
            return false;
        }

        private static void GenerateFillerSceneryData(List<SceneryElementData> list, RegionData region, ref int randomSeed, List<MapGenerator.SceneryAvoidPoint> nodePoints, float mapLength)
        {
            float size = region.scarceScenery.Max((a) => a.data.radius);

            List<FillerSceneryEntry> fillerSceneryEntries = new List<FillerSceneryEntry>(region.fillerScenery);
            fillerSceneryEntries.Randomize();
            
            foreach (FillerSceneryEntry sceneryEntry in fillerSceneryEntries)
            {
                float radius = sceneryEntry.data.radius;
                int num = Mathf.RoundToInt(1f / radius - size);
                int num2 = Mathf.RoundToInt(mapLength / radius);
                for (int i = 0; i < num; i++)
                {
                    for (int j = 0; j < num2; j++)
                    {
                        Vector2 position = new Vector2(size + radius * (float)i, radius * (float)j);
                        position += SeededRandom.InsideUnitCircle(randomSeed++) * radius * 0.5f;
                        if (!nodePoints.Exists(p => Vector2.Distance(p.position, position) < p.radius) &&
                            !list.Exists(s => Vector2.Distance(s.position, position) < s.data.radius))
                        {
                            list.Add(MapGenerator.CreateElementAtPosition(position, sceneryEntry.data,
                                new Vector2(radius * (float)i, radius * (float)j), randomSeed++));
                        }
                    }
                }
            }
        }

        private static void GenerateCliffSceneryData(List<SceneryElementData> list, RegionData regionData,
            ref int randomSeed, List<MapGenerator.SceneryAvoidPoint> nodePoints, float mapLength)
        {
            list.AddRange(GenerateSceneryDataVertically(regionData.scarceScenery, ref randomSeed, nodePoints, mapLength, -X_OFFSET));
                
            list.AddRange(GenerateSceneryDataVertically(regionData.scarceScenery, ref randomSeed, nodePoints, mapLength, X_OFFSET));
        }
        
        private static List<SceneryElementData> GenerateSceneryDataVertically(List<ScarceSceneryEntry> entries,
            ref int randomSeed, List<MapGenerator.SceneryAvoidPoint> nodePoints, float mapLength, float xOffset)
        {
            List<SceneryElementData> list = new List<SceneryElementData>();
            
            float y = 0;
            while (y < mapLength)
            {
                ScarceSceneryEntry entry = entries.GetSeededRandom(randomSeed++);
                y += entry.data.radius / 2;
                
                Vector2 randomPos = nodePoints[0].position;
                randomPos.x += xOffset;
                randomPos.y = y;
                
                list.Add(MapGenerator.CreateElementAtPosition(randomPos, entry.data, Vector2.zero,
                    randomSeed++));
                
                y += entry.data.radius / 2;
            }

            return list;
        }
    }
}