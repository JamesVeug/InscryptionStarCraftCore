using System;
using APIPlugin;
using DiskCardGame;
using StarCraftCore.Scripts.Data.Sigils;

namespace StarCraftCore.Scripts.Abilities
{
    public abstract class ACustomCreateCardsAdjacent<Y, T> : CreateCardsAdjacent where T : CreateCardsAbilityData where Y : CreateCardsAdjacent
    {
        public T LoadedData => m_loadedData ?? (m_loadedData = (T)StarCraftCore.Utils.s_dataLookup[typeof(Y)]);
        protected T m_loadedData = null;
        
        public override int Priority => LoadedData.priority;
        public override string SpawnedCardId
        {
            get
            {
                return LoadedData.spawnCardId;
            }
        }

        public override string CannotSpawnDialogue => LoadedData.cannotSpawnDialogue;

        protected static Ability InitializeBase(string pluginGUID, Type declaringType, string pluginDirectory)
        {
            StarCraftCore.Utils.InitializeAbility<T>(pluginGUID, declaringType, pluginDirectory, out AbilityInfo newAbility);
            return newAbility.ability;
        }
    }
}