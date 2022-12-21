using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DiskCardGame;
using InscryptionAPI.Card;
using InscryptionAPI.Helpers;
using UnityEngine;
using StarCraftCore.Scripts;
using StarCraftCore.Scripts.Data;
using StarCraftCore.Scripts.Data.Sigils;

namespace StarCraftCore
{
    public static class Utils
    {
        public static Dictionary<Type, AData> s_dataLookup = new Dictionary<Type, AData>();
        
        public static void InitializeAbility<T>(string pluginGUID, Type declaringType, string pluginDirectory, out AbilityInfo newAbility) where T : AbilityData
        {
            AbilityData loadedData = DataUtil.LoadFromFile<T>("Data/Sigils/" + declaringType.Name + ".customsigil", pluginDirectory);
            s_dataLookup[declaringType] = loadedData;

            Texture2D texture = GetTextureFromPath(loadedData.iconPath, pluginDirectory);
            newAbility = AbilityManager.New(pluginGUID, loadedData.ruleBookName, loadedData.ruleDescription, declaringType, texture);
            newAbility.powerLevel = loadedData.power;
            newAbility.metaCategories = loadedData.metaCategories;
            if (!string.IsNullOrEmpty(loadedData.pixelIconPath))
            {
                Texture2D pixelTexture = GetTextureFromPath(loadedData.pixelIconPath, pluginDirectory);
                newAbility.SetPixelAbilityIcon(pixelTexture);
            }
            
            if (!string.IsNullOrEmpty(loadedData.learnText))
            {
                List<DialogueEvent.Line> lines = new List<DialogueEvent.Line>();
                DialogueEvent.Line line = new DialogueEvent.Line();
                line.text = loadedData.learnText;
                lines.Add(line);
                newAbility.abilityLearnedDialogue = new DialogueEvent.LineSet(lines);
            }
            //Plugin.Log.LogInfo("Initialized Ability: " + loadedData.ruleBookName + " as " + newAbility.ability);
        }
        
        public static void InitializeSpecialAbility<T>(string pluginGUID, Type declaringType, string pluginDirectory, out T loadedData, out SpecialTriggeredAbilityManager.FullSpecialTriggeredAbility newSpecialAbility) 
            where T : SpecialAbilityData
        {
            loadedData = DataUtil.LoadFromFile<T>("Data/SpecialAbilities/" + declaringType.Name + ".customsigil", pluginDirectory);
            
            StatIconInfo iconInfo = new StatIconInfo();
            iconInfo.rulebookName = loadedData.ruleBookName;
            iconInfo.rulebookDescription = loadedData.ruleDescription;
            iconInfo.iconGraphic = GetTextureFromPath(loadedData.iconPath, pluginDirectory);
            iconInfo.metaCategories = loadedData.metaCategories;

            StatIconManager.FullStatIcon fullStatIcon = StatIconManager.Add(pluginGUID, iconInfo, declaringType);
            SpecialTriggeredAbility specialAbility = fullStatIcon.AbilityId;
            newSpecialAbility = SpecialTriggeredAbilityManager.AllSpecialTriggers.First((a) => a.Id == specialAbility);
            //Plugin.Log.LogInfo("Initialized Special Ability: " + iconInfo.name + " as " + specialAbility);
        }
        public static Texture2D GetTextureFromPath(string path, string pluginDirectory)
        {
            string fullPath = Path.Combine(pluginDirectory, path);
            if (IsPathADirectory(fullPath))
            {
                Plugin.Log.LogError("Cannot load texture at path because it's pointing to a directory! '" + fullPath + ".");
                return null;
            }
            
            //Plugin.Log.LogInfo("Loading texture at path '" + fullPath + ".");
            
            byte[] imgBytes = File.ReadAllBytes(fullPath);
            Texture2D tex = new Texture2D(2,2);
            tex.LoadImage(imgBytes);
            tex.filterMode = FilterMode.Point;

            return tex;
        }
        
        public static void PrintHierarchy(GameObject go, bool printParents)
        {
            string prefix = "";
            if (printParents)
            {
                List<Transform> hierarchy = new List<Transform>();
                
                Transform t = go.transform.parent;
                while (t != null)
                {
                    hierarchy.Add(t);
                    t = t.parent;
                }

                for (int i = hierarchy.Count - 1; i >= 0; i--)
                {
                    Transform tran = hierarchy[i];
                    string text = prefix + tran.gameObject.name + "(" + tran.gameObject.GetInstanceID() + ")";
                    Plugin.Log.LogInfo(prefix + text);

                    prefix += "\t";
                }
            }

            PrintGameObject(go, prefix);
        }

        private static void PrintGameObject(GameObject go, string prefix = "")
        {
            string text = prefix + go.name + "(" + go.GetInstanceID() + ")";
            Plugin.Log.LogInfo(prefix + text);
            Plugin.Log.LogInfo(prefix + "- Components: " + go.transform.childCount);
            foreach (Component component in go.GetComponents<Component>())
            {
                Plugin.Log.LogInfo(prefix + "-- " + component.GetType());
                if (component is SpriteRenderer spriteRenderer)
                {
                    Plugin.Log.LogInfo(prefix + "-- Name: " + spriteRenderer.name);
                    Plugin.Log.LogInfo(prefix + "-- Sprite Name: " + spriteRenderer.sprite.name);
                }
            }

            Plugin.Log.LogInfo(prefix + "- Children: " + go.transform.childCount);
            for (int i = 0; i < go.transform.childCount; i++)
            {
                PrintGameObject(go.transform.GetChild(i).gameObject, prefix + "- -\t");
            }
        }
        
        /// <summary>
        /// Some cards do not have Card.Slot assigned. So this is a work around
        /// </summary>
        public static CardSlot GetSlot(PlayableCard cardToGetSlot)
        {
            if (cardToGetSlot.Slot != null)
            {
                return cardToGetSlot.Slot;
            }

            CardSlot cardSlot = cardToGetSlot.transform.parent.GetComponent<CardSlot>();
            if (cardSlot != null)
            {
                return cardSlot;
            }

            List<CardSlot> allSlots = new List<CardSlot>();
            allSlots.AddRange(Singleton<BoardManager>.Instance.GetSlots(false));
            allSlots.AddRange(Singleton<BoardManager>.Instance.GetSlots(true));

            for (int i = 0; i < allSlots.Count; i++)
            {
                CardSlot slot = allSlots[i];
                if (slot.Index != 2)
                {
                    continue;
                }
                
                PlayableCard card = slot.Card;
                if (card == null)
                    continue;
                
                if (card.gameObject == cardToGetSlot.gameObject)
                {
                    return slot;
                }
            }

            return null;
        }
        
        public static List<CardSlot> GetAdjacentSlots(CardSlot slot)
        {
            int slotIndex = slot.Index;
            List<CardSlot> allSlots = Singleton<BoardManager>.Instance.GetSlots(!slot.opposingSlot);
            List<CardSlot> slots = new List<CardSlot>();
            for (int i = 0; i < allSlots.Count; i++)
            {
                CardSlot cardSlot = allSlots[i];
                if (cardSlot == null || cardSlot.Card == null || cardSlot.Card.Dead || cardSlot.Card.FaceDown)
                {
                    continue;
                }
                
                if (cardSlot.Index == slotIndex - 1 || cardSlot.Index == slotIndex + 1)
                {
                    slots.Add(cardSlot);
                }
            }

            return slots;
        }

        public static int GetRandomWeight(int totalWeight)
        {
            int randomSeed = SaveManager.SaveFile.GetCurrentRandomSeed() + Singleton<GlobalTriggerHandler>.Instance.NumTriggersThisBattle + 1;
            return SeededRandom.Range(0, totalWeight, randomSeed);
        }

        public static CardInfo GetRandomWeightedCard(List<WeightData> weights, int totalWeight)
        {
            int expectedWeight = StarCraftCore.Utils.GetRandomWeight(totalWeight);
            int currentWeight = 0;
            for (var i = 0; i < weights.Count; i++)
            {
                WeightData data = weights[i];
                currentWeight += data.weight;
                if (currentWeight >= expectedWeight)
                {
                    return CardLoader.GetCardByName(data.cardName);
                }
            }

            string cardName = weights[weights.Count - 1].cardName;
            return CardLoader.GetCardByName(cardName);
        }
        
        /// <summary>
        /// Returns a _private_ Property Value from a given Object. Uses Reflection.
        /// Throws a ArgumentOutOfRangeException if the Property is not found.
        /// </summary>
        /// <typeparam name="T">Type of the Property</typeparam>
        /// <param name="obj">Object from where the Property Value is returned</param>
        /// <param name="propName">Propertyname as string.</param>
        /// <returns>PropertyValue</returns>
        public static T GetPrivatePropertyValue<T>(this object obj, string propName)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            PropertyInfo pi = obj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (pi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Property {0} was not found in Type {1}", propName, obj.GetType().FullName));
            return (T)pi.GetValue(obj, null);
        }

        /// <summary>
        /// Returns a private Property Value from a given Object. Uses Reflection.
        /// Throws a ArgumentOutOfRangeException if the Property is not found.
        /// </summary>
        /// <typeparam name="T">Type of the Property</typeparam>
        /// <param name="obj">Object from where the Property Value is returned</param>
        /// <param name="propName">Propertyname as string.</param>
        /// <returns>PropertyValue</returns>
        public static T GetPrivateFieldValue<T>(this object obj, string propName)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            Type t = obj.GetType();
            FieldInfo fi = null;
            while (fi == null && t != null)
            {
                fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                t = t.BaseType;
            }
            if (fi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));
            return (T)fi.GetValue(obj);
        }

        /// <summary>
        /// Sets a _private_ Property Value from a given Object. Uses Reflection.
        /// Throws a ArgumentOutOfRangeException if the Property is not found.
        /// </summary>
        /// <typeparam name="T">Type of the Property</typeparam>
        /// <param name="obj">Object from where the Property Value is set</param>
        /// <param name="propName">Propertyname as string.</param>
        /// <param name="val">Value to set.</param>
        /// <returns>PropertyValue</returns>
        public static void SetPrivatePropertyValue<T>(this object obj, string propName, T val)
        {
            Type t = obj.GetType();
            if (t.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) == null)
                throw new ArgumentOutOfRangeException("propName", string.Format("Property {0} was not found in Type {1}", propName, obj.GetType().FullName));
            t.InvokeMember(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, obj, new object[] { val });
        }

        /// <summary>
        /// Set a private Property Value on a given Object. Uses Reflection.
        /// </summary>
        /// <typeparam name="T">Type of the Property</typeparam>
        /// <param name="obj">Object from where the Property Value is returned</param>
        /// <param name="propName">Propertyname as string.</param>
        /// <param name="val">the value to set</param>
        /// <exception cref="ArgumentOutOfRangeException">if the Property is not found</exception>
        public static void SetPrivateFieldValue<T>(this object obj, string propName, T val)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            Type t = obj.GetType();
            FieldInfo fi = null;
            while (fi == null && t != null)
            {
                fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                t = t.BaseType;
            }
            if (fi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));
            fi.SetValue(obj, val);
        }
        
        public static GameObject FindObjectInChildren(GameObject parent, string name)
        {
            foreach (Transform child in parent.transform)
            {
                if (child.gameObject.name == name)
                {
                    return child.gameObject;
                }
            }
            
            foreach (Transform child in parent.transform)
            {
                GameObject foundChild = FindObjectInChildren(child.gameObject, name);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }

            return null;
        }

        public static bool IsPathADirectory(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            return (attr & FileAttributes.Directory) == FileAttributes.Directory;
        }
    }
}