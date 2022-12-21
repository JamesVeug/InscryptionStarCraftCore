using System;
using DiskCardGame;
using InscryptionAPI.Card;
using StarCraftCore.Scripts.Data.Sigils;

namespace StarCraftCore.Scripts.SpecialAbilities
{
    public abstract class ACustomSpecialAbilityBehaviour<T> : SpecialCardBehaviour where T : SpecialAbilityData
    {
        public static T LoadedData => m_loadedData;
        private static T m_loadedData = null;
        
        public override int Priority => LoadedData.priority;

        public static SpecialTriggeredAbility InitializeBase(string pluginGUID, Type declaringType, string pluginDirectory)
        {
            SpecialTriggeredAbilityManager.FullSpecialTriggeredAbility newSpecialAbility;
            StarCraftCore.Utils.InitializeSpecialAbility(pluginGUID, declaringType, pluginDirectory, out m_loadedData, out newSpecialAbility);
            return newSpecialAbility.Id;
        }
    }
}