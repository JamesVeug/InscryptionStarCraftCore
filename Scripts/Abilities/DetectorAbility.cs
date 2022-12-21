using System;
using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using UnityEngine;
using StarCraftCore.Scripts.Data.Sigils;

namespace StarCraftCore.Scripts.Abilities
{
    public class DetectorAbility : ACustomAbilityBehaviour<DetectorAbility, AbilityData>
    {
        public override Ability Ability => ability;
        public static Ability ability = Ability.None;
		
        public static void Initialize(Type declaringType)
        {
            ability = InitializeBase(Plugin.PluginGuid, declaringType, Plugin.Directory);
        }
        
        public override int Priority => this.triggerPriority;
        private int triggerPriority = 0;

        private bool activated = false;

        private void Awake()
        {
            triggerPriority = LoadedData.power;
        }

        public override bool RespondsToResolveOnBoard()
        {
            return ShouldRevealCards();
        }

        public override IEnumerator OnResolveOnBoard()
        {
            yield return RevealSubmurgedCards();
        }

        public IEnumerator RevealSubmurgedCards()
        {
            activated = true;

            Singleton<ViewManager>.Instance.SwitchToView(View.Board, false);
            yield return new WaitForSeconds(0.15f);
            yield return base.PreSuccessfulTriggerSequence();

            List<CardSlot> opposingSlots = GetValidCardSlotsToFlip(false);
            for (int i = 0; i < opposingSlots.Count; i++)
            {
                PlayableCard card = opposingSlots[i].Card;
                yield return base.LearnAbility(0f);

                // Our targeted
                card.SetFaceDown(false, false);
                card.UpdateFaceUpOnBoardEffects();
                yield return new WaitForSeconds(0.3f);
            }

            this.triggerPriority = int.MinValue + 1;
            yield return new WaitForSeconds(0.3f); // Because its happening too fast maybe?
        }

        public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
        {
            return activated;
        }

        public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
        {
            activated = false;
            Singleton<ViewManager>.Instance.SwitchToView(View.Board, false);
            yield return new WaitForSeconds(0.15f);
            
            List<CardSlot> opposingSlots = GetValidCardSlotsToFlip(true);
            for (int i = 0; i < opposingSlots.Count; i++)
            {
                PlayableCard card = opposingSlots[i].Card;
                yield return base.LearnAbility(0f);
                        
                // Our targeted
                card.SetCardbackSubmerged();
                card.SetFaceDown(true, false);
                yield return new WaitForSeconds(0.3f);
            }
            
            this.triggerPriority = int.MaxValue;
            yield return new WaitForSeconds(0.3f); // Because its happening too fast maybe?
        }

        private List<CardSlot> GetOpponentSlots(bool isPlayer)
        {
            return isPlayer ? BoardManager.Instance.opponentSlots : BoardManager.Instance.playerSlots;
        }

        private List<CardSlot> GetValidCardSlotsToFlip(bool facingDown)
        {
            List<CardSlot> slots = new List<CardSlot>();
            
            List<CardSlot> opposingSlots = GetOpponentSlots(Singleton<TurnManager>.Instance.IsPlayerTurn);
            for (int i = 0; i < opposingSlots.Count; i++)
            {
                CardSlot slot = opposingSlots[i];
                if (slot.Card != null && !slot.Card.Dead)
                {
                    if (slot.Card.HasAbility(Ability.Submerge) || slot.Card.HasAbility(Ability.SubmergeSquid) && (slot.Card.FaceDown == facingDown))
                    {
                        slots.Add(slot);
                    }
                }
            }

            return slots;
        }

        public bool ShouldRevealCards()
        {
            return Singleton<TurnManager>.Instance.IsPlayerTurn && GetValidCardSlotsToFlip(false).Count > 0;
        }
    }
}