using System.Collections.Generic;
using System.IO;
using DiskCardGame;
using InscryptionAPI.Dialogue;
using InscryptionAPI.Helpers;
using InscryptionAPI.Regions;
using InscryptionAPI.Resource;
using InscryptionAPI.Sound;
using UnityEngine;

namespace StarCraftCore.Scripts.Regions
{
    public class AuirRegion
    {
        public static string RegionName = "Auir";
        public static RegionData regionData = null;
        public static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Plugin.Directory, "AssetBundles/auir"));
        
        public static void Initialize()
        {
            RegionData regionTemplate = Resources.Load<RegionData>("data/map/regions/Forest");

            regionData = RegionManager.New(RegionName, 0, true);
            regionData.terrainCards = new List<CardInfo>()
            {
                CardLoader.GetCardByName("StarCraftCore_JSON_Minerals")
            };
            regionData.encounters = new List<EncounterBlueprintData>(regionTemplate.encounters);
            regionData.likelyCards = new List<CardInfo>(regionTemplate.likelyCards);
            regionData.dominantTribes = new List<Tribe>() { Tribe.Bird };
            regionData.bossPrepEncounter = regionTemplate.encounters[0];
            regionData.ambientLoopId = "";
            regionData.boardLightColor = new Color((float)256/256, (float)146/256, (float)38/256);
            regionData.cardsLightColor = regionTemplate.cardsLightColor;
            Texture2D texture = StarCraftCore.Utils.GetTextureFromPath("Artwork/Regions/mapScroll_Albedo_Auir.png", Plugin.Directory);
            regionData.mapAlbedo = texture;
            
            regionData.mapParticlesPrefabs = new List<GameObject>();
            //regionData.mapParticlesPrefabs.Add(assetBundle.LoadAsset<GameObject>("MapAshParticles"));

            regionData.fillerScenery = GetFillerScenery();
            regionData.scarceScenery = GetScarceScenery();
            regionData.predefinedScenery = null;
            
                
            regionData.bosses = new List<Opponent.Type>()
            {
                Opponent.Type.ProspectorBoss, Opponent.Type.AnglerBoss, Opponent.Type.TrapperTraderBoss
            };
            
            LoadGameObjects();
            LoadDialogue();
        }

        private static List<ScarceSceneryEntry> GetScarceScenery()
        {
            List<ScarceSceneryEntry> entries = new List<ScarceSceneryEntry>();

            // GasGysers
            ScarceSceneryEntry gasGysers = new ScarceSceneryEntry()
            {
                minInstances = 10,
                maxInstances = 15,
                minDensity = 1f,
            };
            SceneryData gasGysersData = ScriptableObject.CreateInstance<SceneryData>();
            gasGysers.data = gasGysersData;
            gasGysers.data.minScale = new Vector2(0.55f, 0.55f);
            gasGysers.data.maxScale = new Vector2(0.85f, 0.85f);
            gasGysers.data.prefabNames = ConvertNames(GasGysers);
            gasGysers.data.radius = 0.2f;

            // GemTower
            ScarceSceneryEntry gemTower = new ScarceSceneryEntry()
            {
                minInstances = 10,
                maxInstances = 20,
                minDensity = 1f,
            };
            SceneryData gemTowerData = ScriptableObject.CreateInstance<SceneryData>();
            gemTower.data = gemTowerData;
            gemTower.data.minScale = new Vector2(1.0f, 1.0f);
            gemTower.data.maxScale = new Vector2(1.5f, 1.5f);
            gemTower.data.prefabNames = ConvertNames(GemTowers);
            gemTower.data.radius = 0.2f;
            
            //entries.Add(trees);
            entries.Add(gasGysers);
            entries.Add(gemTower);
            return entries;
        }

        private static List<FillerSceneryEntry> GetFillerScenery()
        {
            List<FillerSceneryEntry> entries = new List<FillerSceneryEntry>();
                
            FillerSceneryEntry trees = new FillerSceneryEntry();
            trees.data = ScriptableObject.CreateInstance<SceneryData>();
            trees.data.minScale = new Vector2(0.55f, 0.55f);
            trees.data.maxScale = new Vector2(0.75f, 0.75f);
            trees.data.prefabNames = ConvertNames(Trees);
            trees.data.radius = 0.1f;
            trees.data.perlinNoiseHeight = true;

            entries.Add(trees);
            return entries;
        }

        static List<string> ConvertNames(List<string> s)
        {
            List<string> names = new List<string>(s.Count);
            foreach (string s1 in s)
            {
                names.Add(RegionName + "/" + s1);
            }

            return names;
        }
        
        static List<string> Trees = new List<string>()
        {
            "Tree",
        };
        
        static List<string> GemTowers = new List<string>()
        {
            "GemTower",
        };
        
        static List<string> GasGysers = new List<string>()
        {
            "GasGyser",
        };

        public static void LoadGameObjects()
        {
            List<string> prefabs = new List<string>();
            prefabs.AddRange(Trees);
            prefabs.AddRange(GasGysers);
            prefabs.AddRange(GemTowers);

            foreach (string prefab in prefabs)
            {
                GameObject gameObject = assetBundle.LoadAsset<GameObject>(prefab);
                if (gameObject == null)
                {
                    continue;
                }
                
                if (gameObject.GetComponent<MapElement>() == null)
                {
                    gameObject.AddComponent<MapElement>();
                }

                ResourceBankManager.Add(Plugin.PluginGuid,
                    path: "Prefabs/Map/MapScenery/" + RegionName + "/" + prefab,  // Prefabs/Map/MapScenery/Char/TreeA
                    unityObject: gameObject);
            }
        }

        public static void LoadDialogue()
        {
            DialogueManager.Add(Plugin.PluginGuid, new DialogueEvent()
            {
                id = "Region" + RegionName,
                groupId = "Game Flow",
                speakers = new List<DialogueEvent.Speaker>() { DialogueEvent.Speaker.Single },
                maxRepeatsBehaviour = DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat,
                mainLines = new DialogueEvent.LineSet()
                {
                    lines = new List<DialogueEvent.Line>()
                    {
                        new DialogueEvent.Line()
                        {
                            p03Face = P03AnimationController.Face.Default,
                            emotion = Emotion.Neutral,
                            letterAnimation = TextDisplayer.LetterAnimation.None,
                            speakerIndex = 0,
                            text = "Auir, the homeworld of the Protoss long ago engineered.",
                            specialInstruction = "",
                            storyCondition = StoryEvent.BasicTutorialCompleted,
                            storyConditionMustBeMet = false,
                        },
                        new DialogueEvent.Line()
                        {
                            p03Face = P03AnimationController.Face.Default,
                            emotion = Emotion.Neutral,
                            letterAnimation = TextDisplayer.LetterAnimation.None,
                            speakerIndex = 0,
                            text = "The planet beats with psionic energy.",
                            specialInstruction = "",
                            storyCondition = StoryEvent.BasicTutorialCompleted,
                            storyConditionMustBeMet = false,
                        }
                    }
                },
                repeatLines = new List<DialogueEvent.LineSet>()
                {
                    new DialogueEvent.LineSet()
                    {
                        lines = new List<DialogueEvent.Line>()
                        {
                            new DialogueEvent.Line()
                            {
                                p03Face = P03AnimationController.Face.Default,
                                emotion = Emotion.Neutral,
                                letterAnimation = TextDisplayer.LetterAnimation.None,
                                speakerIndex = 0,
                                text = "Auir. It's good to be home",
                                specialInstruction = "",
                                storyCondition = StoryEvent.BasicTutorialCompleted,
                                storyConditionMustBeMet = false,
                            }
                        }
                    }
                }
            });
        }
    }
}