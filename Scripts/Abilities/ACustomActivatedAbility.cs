using System;
using APIPlugin;
using DiskCardGame;
using StarCraftCore.Scripts.Data.Sigils;

namespace StarCraftCore.Scripts.Abilities
{
    public abstract class ACustomActivatedAbility<Y, T> : ActivatedAbilityBehaviour where T : ActivatedAbilityData where Y : ActivatedAbilityBehaviour
    {
        public T LoadedData => m_loadedData ?? (m_loadedData = (T)StarCraftCore.Utils.s_dataLookup[typeof(Y)]);
        protected T m_loadedData = null;
        
        public override int Priority => LoadedData.priority;
        protected override int BonesCost => LoadedData.boneCost;
        protected override int EnergyCost => LoadedData.energyCost;

        protected static Ability InitializeBase(string pluginGUID, Type declaringType, string pluginDirectory)
        {
            StarCraftCore.Utils.InitializeAbility<T>(pluginGUID, declaringType, pluginDirectory, out AbilityInfo newAbility);
            newAbility.activated = true;
            return newAbility.ability;
        }
    }
}