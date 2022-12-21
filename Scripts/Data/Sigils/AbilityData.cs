using System;
using System.Collections.Generic;
using DiskCardGame;

namespace StarCraftCore.Scripts.Data.Sigils
{
    [Serializable]
    public class AbilityData : AData
    {
        public int power = 0;
        public int priority;
        public string learnText = "";
        public string ruleBookName = "";
        public string ruleDescription = "";
        public string iconPath = "Artwork/";
        public string pixelIconPath = "Artwork/Pixel/";
        public List<AbilityMetaCategory> metaCategories = new List<AbilityMetaCategory>{ AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular };
    }
}