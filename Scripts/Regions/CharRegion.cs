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
    public class CharRegion
    {
        public static string RegionName = "Char";
        public static RegionData regionData = null;
        public static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Plugin.Directory, "AssetBundles/regionchar"));
        
        public static void Initialize()
        {
            RegionData wetLands = Resources.Load<RegionData>("data/map/regions/Wetlands");

            regionData = RegionManager.New(RegionName, 1, true);
            regionData.terrainCards = new List<CardInfo>()
            {
                CardLoader.GetCardByName("StarCraftCore_JSON_Minerals")
            };
            regionData.encounters = new List<EncounterBlueprintData>(wetLands.encounters);
            regionData.likelyCards = new List<CardInfo>(wetLands.likelyCards);
            regionData.dominantTribes = new List<Tribe>() { Tribe.Insect };
            regionData.bossPrepEncounter = wetLands.encounters[0];
            regionData.ambientLoopId = "ambient_wetlands";
            regionData.boardLightColor = new Color(255/255f, 129/255f, 37/255f);
            regionData.cardsLightColor = new Color(183/255f, 82/255f, 145/255f);
            Texture2D texture = StarCraftCore.Utils.GetTextureFromPath("Artwork/Regions/mapScroll_Albedo_Char.png", Plugin.Directory);
            regionData.mapAlbedo = texture;
            
            regionData.mapParticlesPrefabs = new List<GameObject>();
            regionData.mapParticlesPrefabs.Add(assetBundle.LoadAsset<GameObject>("MapAshParticles"));

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

            // Cliff
            /*ScarceSceneryEntry cliffs = new ScarceSceneryEntry()
            {
                minInstances = 40,
                maxInstances = 80,
                minDensity = 1f,
            };
            SceneryData cliffsData = ScriptableObject.CreateInstance<SceneryData>();
            cliffs.data = cliffsData;
            cliffsData.minScale = new Vector2(0.025f, 0.025f);
            cliffsData.maxScale = new Vector2(0.05f, 0.05f);
            cliffsData.prefabNames = ConvertNames(Cliffs);
            cliffsData.radius = 0.075f;*/

            // Crystals
            ScarceSceneryEntry crystals = new ScarceSceneryEntry()
            {
                minInstances = 40,
                maxInstances = 80,
                minDensity = 1f,
            };
            SceneryData crystalsData = ScriptableObject.CreateInstance<SceneryData>();
            crystals.data = crystalsData;
            crystals.data.minScale = new Vector2(0.55f, 0.55f);
            crystals.data.maxScale = new Vector2(0.85f, 0.85f);
            crystals.data.prefabNames = ConvertNames(Crystals);
            crystals.data.radius = 0.075f;
            
            // Trees
            ScarceSceneryEntry trees = new ScarceSceneryEntry()
            {
                minInstances = 40,
                maxInstances = 80,
                minDensity = 1f,
            };
            SceneryData treesData = ScriptableObject.CreateInstance<SceneryData>();
            trees.data = treesData;
            trees.data.minScale = new Vector2(0.55f, 0.55f);
            trees.data.maxScale = new Vector2(0.85f, 0.85f);
            trees.data.prefabNames = ConvertNames(Trees);
            trees.data.radius = 0.075f;
            
            //entries.Add(cliffs);
            entries.Add(crystals);
            entries.Add(trees);
            return entries;
        }

        private static List<FillerSceneryEntry> GetFillerScenery()
        {
            List<FillerSceneryEntry> entries = new List<FillerSceneryEntry>();
                
            FillerSceneryEntry cliffs = new FillerSceneryEntry();
            cliffs.data = ScriptableObject.CreateInstance<SceneryData>();
            cliffs.data.minScale = new Vector2(0.025f, 0.025f);
            cliffs.data.maxScale = new Vector2(0.05f, 0.05f);
            cliffs.data.prefabNames = ConvertNames(Cliffs);
            cliffs.data.radius = 0.1f;
                
            FillerSceneryEntry rocks = new FillerSceneryEntry();
            rocks.data = ScriptableObject.CreateInstance<SceneryData>();
            rocks.data.minScale = new Vector2(0.7f, 0.7f);
            rocks.data.maxScale = new Vector2(1.0f, 1.0f);
            rocks.data.prefabNames = ConvertNames(Rocks);
            rocks.data.radius = 0.05f;
            
            /*FillerSceneryEntry crystals = new FillerSceneryEntry();
            crystals.data = ScriptableObject.CreateInstance<SceneryData>();
            crystals.data.minScale = new Vector2(0.35f, 0.35f);
            crystals.data.maxScale = new Vector2(0.5f, 0.5f);
            crystals.data.prefabNames = ConvertNames(Crystals);
            crystals.data.radius = 0.03f;
            
            FillerSceneryEntry trees = new FillerSceneryEntry();
            trees.data = ScriptableObject.CreateInstance<SceneryData>();
            trees.data.minScale = new Vector2(0.7f, 0.7f);
            trees.data.maxScale = new Vector2(1.0f, 1.0f);
            trees.data.prefabNames = ConvertNames(Trees);
            trees.data.radius = 0.06f;*/

            entries.Add(cliffs);
            entries.Add(rocks);
            //entries.Add(crystals);
            //entries.Add(trees);
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
            "TreeA", "TreeB", "TreeC",
        };
        
        static List<string> Crystals = new List<string>()
        {
            "CrystalA",
        };
        
        static List<string> Cliffs = new List<string>()
        {
            "Cliff_1",
        };
        
        static List<string> Rocks = new List<string>()
        {
            "Rock_8", "Rock_9", "Rock_10", "Rock_11", "Rock_12", "Rock_13", "Rock_14",
        };

        public static void LoadGameObjects()
        {
            List<string> prefabs = new List<string>();
            prefabs.AddRange(Trees);
            prefabs.AddRange(Crystals);
            prefabs.AddRange(Cliffs);
            prefabs.AddRange(Rocks);

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
                            text = "A thick pile of ash is seen further than the eye can see.",
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
                            text = "One wrong step and you find yourself wading in a pool of hot lava.",
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
                            text = "An eerie red glow comes from the magma that seeps up through the surface of the planet.",
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
                                text = "Char Again?. Most would rather die than revisit this hellhole.",
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