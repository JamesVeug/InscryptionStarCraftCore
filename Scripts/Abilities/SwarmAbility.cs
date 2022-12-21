using System;
using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using UnityEngine;
using StarCraftCore.Scripts.Data.Sigils;

namespace StarCraftCore.Scripts.Abilities
{
    public class SwarmAbility : ACustomAbilityBehaviour<SwarmAbility, SwarmAbilityData>
    {
        public override Ability Ability => SwarmAbility.ability;
        public static Ability ability;
        
        private List<int> attackedSlots = new List<int>();
        
        public static void Initialize(Type declaringType)
        {
            ability = InitializeBase(Plugin.PluginGuid, declaringType, Plugin.Directory);
        }

        public override bool RespondsToAttackEnded()
        {
            return attackedSlots.Count > 0;
        }

        public override IEnumerator OnAttackEnded()
        {
            attackedSlots.Clear();
            yield return null;
        }

        public override bool RespondsToDealDamage(int amount, PlayableCard target)
        {
            return target != null;
        }

        public override IEnumerator OnDealDamage(int amount, PlayableCard target)
        {
            attackedSlots.Add(StarCraftCore.Utils.GetSlot(target).Index);
            yield return null;
        }

        public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
        {
            return attackedSlots.Contains(deathSlot.Index);
        }

        public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
        {
            attackedSlots.Remove(deathSlot.Index);
            yield return null;
        }

        public override bool RespondsToOtherCardDealtDamage(PlayableCard attacker, int amount, PlayableCard target)
        {
            if (target.Dead || attacker.Dead)
            {
                return false;
            }
            
            if (attacker.OpponentCard != this.Card.OpponentCard)
            {
                return false;
            }

            int slotDifference = Mathf.Abs(attacker.slot.Index - this.Card.slot.Index);
            if (slotDifference != 1)
            {
                return false;
            }

            if (attackedSlots.Contains(target.slot.Index))
            {
                return false;
            }
            
            return true;
        }

        public override IEnumerator OnOtherCardDealtDamage(PlayableCard attacker, int amount, PlayableCard target)
        {
            yield return new WaitForSeconds(0.25f);
            CardSlot targetSlot = StarCraftCore.Utils.GetSlot(target);
            attackedSlots.Add(targetSlot.Index);
            yield return base.PreSuccessfulTriggerSequence();
            yield return Singleton<CombatPhaseManager>.Instance.SlotAttackSlot(Card.slot, targetSlot, 0f);
            yield return base.LearnAbility(0.1f);
        }
    }
}