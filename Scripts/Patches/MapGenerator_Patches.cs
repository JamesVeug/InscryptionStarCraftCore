using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using APIPlugin;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Regions;
using StarCraftCore;
using StarCraftCore.Scripts.Regions;
using UnityEngine;

namespace StarCraftCore.Scripts.Patches
{
    /*[HarmonyPatch(typeof (MapGenerator), "GenerateScenery", new System.Type[] {typeof(int), typeof(RegionData), typeof(List<NodeData>), typeof(float), typeof(PredefinedScenery)})]
    public class MapGenerator_GenerateScenery
    {
        public static bool Prefix(int randomSeed, RegionData region, List<NodeData> nodes, float mapLength, PredefinedScenery predefinedScenery = null)
        {
            if (RegionManager.AllRegionsCopy.Contains(region))
            {
                // Custom region!
                bool initialized = false;
                string dataPrefabName = predefinedScenery.scenery[0].data.prefabNames[0];
                List<ResourceBank.Resource> resources = ResourceBank.instance.resources;
                for (var i = 0; i < resources.Count; i++)
                {
                    string resourcePath = SceneryData.PREFABS_ROOT + resources[i].path; 
                    if (resourcePath == dataPrefabName)
                    {
                        initialized = true;
                        break;
                    }
                }

                if (!initialized)
                {
                    
                }
            }

            return true;
        }
    }*/
    
    /*[HarmonyPatch(typeof (MapElementData), "SetPrefabPath", new System.Type[] {typeof(string)})]
    public class MapElementData_SetPrefabPath
    {
        public static void Postfix(string path)
        {
            Plugin.Log.LogInfo("[MapElementData] " + path);
        }
    }
    
    [HarmonyPatch(typeof (SceneryData), "GetRandomPrefabId", new System.Type[] {})]
    public class SceneryData_GetRandomPrefabId
    {
        public static bool Prefix(SceneryData __instance)
        {
            Plugin.Log.LogInfo("[SceneryData_GetRandomPrefabId] " + __instance.GetType().FullName + " " + __instance.name + " " + __instance.prefabNames.Count);

            return true;
        }
    }
    
    [HarmonyPatch(typeof (MapGenerator), "CreateElementAtPosition", new System.Type[] {typeof(Vector2), typeof(SceneryData), typeof(Vector2), typeof(int)})]
    public class MapGenerator_CreateElementAtPosition
    {
        public static void Postfix(SceneryElementData __result)
        {
            Plugin.Log.LogInfo("[MapGenerator_CreateElementAtPosition] __result " + __result);
        }
    }*/
    
    [HarmonyPatch(typeof (DialogueDataUtil), "ReadDialogueData", new System.Type[] {})]
    public class DialogueDataUtil_ReadDialogueData
    {
        public static void Postfix()
        {
            DialogueDataUtil.data.events.Add(CharRegion.GetDialogue());
        }
    }
}

[HarmonyPatch(typeof(MapDataReader), "SpawnMapObjects", new Type[]{typeof(MapData), typeof(int), typeof(Vector2)})]
public class MapDataReader_SpawnMapObjects
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        // === We want to turn this
        
        // gameObject.GetComponent<Renderer>();
        
        // === Into this
        
        // GetSpawnedMapObjectRenderer(gameObject);
        
        // ===
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

        MethodInfo GetSpawnedMapObjectRendererInfo =  SymbolExtensions.GetMethodInfo(() => GetSpawnedMapObjectRenderer(null));

        for (int i = 0; i < codes.Count; i++)
        {
            if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand.ToString() == "UnityEngine.Renderer GetComponent[Renderer]()")
            {
                codes[i].operand = GetSpawnedMapObjectRendererInfo;
            }
        }

        return codes;
    }

    private static Renderer GetSpawnedMapObjectRenderer(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out Renderer renderer))
        {
            return renderer;
        }

        Renderer spawnedMapObjectRenderer = gameObject.GetComponentInChildren<Renderer>();
        return spawnedMapObjectRenderer;
    }
}