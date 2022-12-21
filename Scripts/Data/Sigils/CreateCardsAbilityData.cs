using System;

namespace StarCraftCore.Scripts.Data.Sigils
{
    [Serializable]
    public class CreateCardsAbilityData : AbilityData
    {
        public string spawnCardId = "Squirrel";
        public string cannotSpawnDialogue = "No room for Larva!";
    }
}