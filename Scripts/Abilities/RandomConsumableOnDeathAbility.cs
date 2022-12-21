using System;
using System.Collections;
using DiskCardGame;
using UnityEngine;
using StarCraftCore.Scripts.Data.Sigils;

namespace StarCraftCore.Scripts.Abilities
{
    public class RandomConsumableOnDeathAbility : ACustomAbilityBehaviour<RandomConsumableOnDeathAbility, AbilityData>
    {
        public override Ability Ability => ability;
        public static Ability ability = Ability.None;
		
        public static void Initialize(Type declaringType)
        {
            ability = InitializeBase(Plugin.PluginGuid, declaringType, Plugin.Directory);
        }

        public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
        {
            return true;
        }

        public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
        {
            yield return base.PreSuccessfulTriggerSequence();
            yield return new WaitForSeconds(0.2f);
            Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
            yield return new WaitForSeconds(0.25f);
            if (RunState.Run.consumables.Count < RunState.Run.MaxConsumables)
            {
                RunState.Run.consumables.Add(ItemsUtil.GetRandomUnlockedConsumable(base.GetRandomSeed()).name);
                Singleton<ItemsManager>.Instance.UpdateItems(false);
            }
            else
            {
                base.Card.Anim.StrongNegationEffect();
                yield return new WaitForSeconds(0.2f);
                Singleton<ItemsManager>.Instance.ShakeConsumableSlots(0f);
            }
            yield return new WaitForSeconds(0.2f);
            yield return base.LearnAbility(0f);
        }

    }
}