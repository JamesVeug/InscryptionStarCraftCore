using System;
using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using UnityEngine;
using StarCraftCore.Scripts.Data.Sigils;

namespace StarCraftCore.Scripts.Abilities
{
    public class HealingAbility : ACustomAbilityBehaviour<HealingAbility, HealingAbilityData>
    {
        public override Ability Ability => ability;
        public static Ability ability = Ability.None;
		
        public static void Initialize(Type declaringType)
        {
            ability = InitializeBase(Plugin.PluginGuid, declaringType, Plugin.Directory);
        }
        
        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return !Card.Dead && Card.OpponentCard != playerUpkeep && GetValidSlots().Count > 0;
        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            yield return base.PreSuccessfulTriggerSequence();
		        
            if (Singleton<ViewManager>.Instance.CurrentView != View.Board)
            {
                yield return new WaitForSeconds(0.2f);
                Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
                yield return new WaitForSeconds(0.2f);
            }


            List<CardSlot> cardSlots = GetValidSlots();
            foreach (CardSlot slot in cardSlots)
            {
                slot.Card.Anim.PlayTransformAnimation();
                yield return new WaitForSeconds(0.15f);

                PlayableCard adjacentCard = slot.Card;
                int newHealth = Mathf.Max(0, adjacentCard.Status.damageTaken - LoadedData.health); 
                adjacentCard.Status.damageTaken = newHealth;
                adjacentCard.UpdateStatsText();
                yield return new WaitForSeconds(0.5f);
            }

            yield return base.LearnAbility(0f);
        }

        private List<CardSlot> GetValidSlots()
        {
            List<CardSlot> cardSlots = StarCraftCore.Utils.GetAdjacentSlots(Card.slot);
            cardSlots.RemoveAll((a) => a.Card.Status.damageTaken <= 0);
            return cardSlots;
        }
    }
}