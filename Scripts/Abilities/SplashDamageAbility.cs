using System;
using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using InscryptionAPI.Card;
using UnityEngine;
using StarCraftCore.Scripts.Data.Sigils;

namespace StarCraftCore.Scripts.Abilities
{
    public class SplashDamageAbility : ACustomAbilityBehaviour<SplashDamageAbility, AbilityData>
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
            CardSlot slot = StarCraftCore.Utils.GetSlot(target);
            return !activated && slot != null;
        }

        public override IEnumerator OnDealDamage(int amount, PlayableCard target)
        {
            yield return this.PreSuccessfulTriggerSequence();

            if (Singleton<ViewManager>.Instance.CurrentView != View.Board)
            {
                yield return new WaitForSeconds(0.2f);
                Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
                yield return new WaitForSeconds(0.2f);
            }
            
            activated = true;
            
            CardSlot cardSlot = StarCraftCore.Utils.GetSlot(target);
            List<CardSlot> adjacentSlots = StarCraftCore.Utils.GetAdjacentSlots(cardSlot);
            foreach (CardSlot slot in adjacentSlots)
            {
                if (slot.Card != null && !slot.Card.Dead && !slot.Card.FaceDown && slot.opposingSlot != this.Card.slot.opposingSlot)
                {
                    yield return slot.Card.TakeDamage(amount, Card);
                }
            }

            activated = false;

            yield return base.LearnAbility(0.0f);
        }
    }
}