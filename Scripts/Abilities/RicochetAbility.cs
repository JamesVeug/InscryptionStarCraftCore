using System;
using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using UnityEngine;
using StarCraftCore.Scripts.Data.Sigils;

namespace StarCraftCore.Scripts.Abilities
{
    public class RicochetAbility : ACustomAbilityBehaviour<RicochetAbility, RicochetAbilityData>
    {
        public override Ability Ability => ability;
        public static Ability ability = Ability.None;
		
        private bool activated = false;
        
        public static void Initialize(Type declaringType)
        {
            ability = InitializeBase(Plugin.PluginGuid, declaringType, Plugin.Directory);
        }
        
        public override bool RespondsToDealDamage(int amount, PlayableCard target)
        {
            return !activated && !Card.Dead;
        }

        public override IEnumerator OnDealDamage(int amount, PlayableCard target)
        {
            activated = true;
            
            // Wait for them to finish the attack
            yield return new WaitForSeconds(0.125f);
            
            // Switch to Combat view
            if (Singleton<ViewManager>.Instance.CurrentView != Singleton<BoardManager>.Instance.CombatView)
            {
                yield return new WaitForSeconds(0.2f);
                Singleton<ViewManager>.Instance.SwitchToView(Singleton<BoardManager>.Instance.CombatView, false, false);
                yield return new WaitForSeconds(0.2f);
            }

            // Adjust settings
            CardModificationInfo flyingModifier = new CardModificationInfo
            {
                attackAdjustment = -Card.Attack + LoadedData.ricochetDamage, // Set the damage to 1
                abilities = new List<Ability> { Ability.Flying } // Allow flying over targets
            };
            Card.AddTemporaryMod(flyingModifier);
            
            // Attack
            yield return new WaitForSeconds(0.2f);
            yield return Singleton<CombatPhaseManager>.Instance.SlotAttackSlot(Card.Slot, StarCraftCore.Utils.GetSlot(target));
            yield return new WaitForSeconds(0.2f);
            
            // Revert settings
            Card.RemoveTemporaryMod(flyingModifier);
            
            // Learn
            yield return base.LearnAbility(0f);
            activated = false;
        }
    }
}