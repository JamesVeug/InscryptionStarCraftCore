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
    public class MarSaraRegion
    {
        public static string RegionName = "MarSara";
        public static RegionData regionData = null;
        public static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Plugin.Directory, "AssetBundles/marsara"));
        
        public static void Initialize()
        {
            RegionData wetLands = Resources.Load<RegionData>("data/map/regions/Wetlands");

            regionData = RegionManager.New(RegionName, 2, true);
            regionData.terrainCards = new List<CardInfo>()
            {
                CardLoader.GetCardByName("StarCraftCore_JSON_Minerals")
            };
            regionData.encounters = new List<EncounterBlueprintData>(wetLands.encounters);
            regionData.likelyCards = new List<CardInfo>(wetLands.likelyCards);
            regionData.dominantTribes = new List<Tribe>() { Tribe.Canine };
            regionData.bossPrepEncounter = wetLands.encounters[0];
            regionData.ambientLoopId = "ambient_wetlands";
            regionData.boardLightColor = new Color(255/255f, 197/255f, 139/255f);
            regionData.cardsLightColor = new Color(255/255f, 197/255f, 139/255f);
            Texture2D texture = StarCraftCore.Utils.GetTextureFromPath("Artwork/Regions/mapScroll_Albedo_MarSara.png", Plugin.Directory);
            regionData.mapAlbedo = texture;
            
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

            // Rocks
            ScarceSceneryEntry rocks = new ScarceSceneryEntry()
            {
                minInstances = 10,
                maxInstances = 25,
                minDensity = 0.1f,
            };
            rocks.data = ScriptableObject.CreateInstance<SceneryData>();
            rocks.data.minScale = new Vector2(2.0f, 2.0f);
            rocks.data.maxScale = new Vector2(2.0f, 2.0f);
            rocks.data.prefabNames = ConvertNames(Rocks);
            rocks.data.radius = 0.125f;

            // Trees
            /*ScarceSceneryEntry trees = new ScarceSceneryEntry()
            {
                minInstances = 12,
                maxInstances = 25,
                minDensity = 0.1f,
            };
            trees.data = ScriptableObject.CreateInstance<SceneryData>();
            trees.data.baseEulers = new Vector3(-90, 0, 0);
            trees.data.minScale = new Vector2(50f, 50f);
            trees.data.maxScale = new Vector2(50f, 50f);
            trees.data.prefabNames = new List<string>(Trees);
            trees.data.perlinNoiseHeight = true;
            trees.data.radius = 0.05f;*/

            // Stones
            ScarceSceneryEntry stones = new ScarceSceneryEntry()
            {
                minInstances = 12,
                maxInstances = 25,
                minDensity = 0.1f,
            };
            stones.data = ScriptableObject.CreateInstance<SceneryData>();
            stones.data.minScale = new Vector2(2.0f, 2.0f);
            stones.data.maxScale = new Vector2(2.0f, 2.0f);
            stones.data.prefabNames = ConvertNames(Stones);
            stones.data.radius = 0.15f;
            
            //entries.Add(trees);
            entries.Add(rocks);
            //entries.Add(stones);
            return entries;
        }

        private static List<FillerSceneryEntry> GetFillerScenery()
        {
            List<FillerSceneryEntry> entries = new List<FillerSceneryEntry>();
            
            // Tree
            FillerSceneryEntry trees = new FillerSceneryEntry();
            trees.data = ScriptableObject.CreateInstance<SceneryData>();
            trees.data.minScale = new Vector2(50f, 50f);
            trees.data.maxScale = new Vector2(50f, 50f);
            trees.data.prefabNames = Trees;
            trees.data.radius = 0.125f;
            trees.data.baseEulers = new Vector3(-90, 0, 0);
            
            // Tumbleweed
            FillerSceneryEntry tumbleweed = new FillerSceneryEntry();
            tumbleweed.data = ScriptableObject.CreateInstance<SceneryData>();
            tumbleweed.data.minScale = new Vector2(1, 1);
            tumbleweed.data.maxScale = new Vector2(1, 1);
            tumbleweed.data.prefabNames = ConvertNames(Tumbleweed);
            tumbleweed.data.radius = 0.1f;
            
            // Stones
            FillerSceneryEntry stones = new FillerSceneryEntry();
            stones.data = ScriptableObject.CreateInstance<SceneryData>();
            stones.data.minScale = new Vector2(1.5f, 1.5f);
            stones.data.maxScale = new Vector2(2.0f, 2.0f);
            stones.data.prefabNames = ConvertNames(Stones);
            stones.data.radius = 0.15f;
                
            entries.Add(trees);
            entries.Add(tumbleweed);
            entries.Add(stones);
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
            "CommonTree_Dead_1",
            "CommonTree_Dead_2",
            "CommonTree_Dead_3",
            "CommonTree_Dead_4",
            "CommonTree_Dead_5",
        };
        
        static List<string> Rocks = new List<string>()
        {
            "Rock_8",
            "Rock_9",
            "Rock_10",
            "Rock_11",
            "Rock_12",
            "Rock_13",
            "Rock_14",
        };
        
        static List<string> Stones = new List<string>()
        {
            "stones",
        };
        
        static List<string> Tumbleweed = new List<string>()
        {
            "Tumbleweed",
        };


        public static void LoadGameObjects()
        {
            List<string> prefabs = new List<string>();
            prefabs.AddRange(Rocks);
            prefabs.AddRange(Stones);
            prefabs.AddRange(Tumbleweed);

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
                            letterAnimation = TextDisplayer.LetterAnimation.WavyJitter,
                            speakerIndex = 0,
                            text = "Mar Sara. A home planet for a Terran colony in the Koprulu sector.",
                            specialInstruction = "",
                            storyCondition = StoryEvent.BasicTutorialCompleted,
                            storyConditionMustBeMet = false,
                        },
                        new DialogueEvent.Line()
                        {
                            p03Face = P03AnimationController.Face.Default,
                            emotion = Emotion.Neutral,
                            letterAnimation = TextDisplayer.LetterAnimation.WavyJitter,
                            speakerIndex = 0,
                            text = "Many factions live here in conflict, fighting over the resource rich planet.",
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
                                letterAnimation = TextDisplayer.LetterAnimation.WavyJitter,
                                speakerIndex = 0,
                                text = "Your actions on Mar Sara will shape the fate of the planet and have far-reaching consequences, so choose your allies wisely.",
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