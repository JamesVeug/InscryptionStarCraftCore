using System;
using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using Pixelplacement;
using UnityEngine;
using StarCraftCore.Scripts.Data.Sigils;

namespace StarCraftCore.Scripts.Abilities
{
    public class AbductAbility : ACustomAbilityBehaviour<AbductAbility, AbilityData>
    {
	    public override Ability Ability => ability;
	    public static Ability ability = Ability.None;
	    
        private CardSlot m_targetedCardSlot = null;

        public static void Initialize(Type declaringType)
        {
	        ability = InitializeBase(Plugin.PluginGuid, declaringType, Plugin.Directory);
        }

        public override bool RespondsToResolveOnBoard()
        {
	        return Card.Slot.IsPlayerSlot;
        }
        
        public override IEnumerator OnResolveOnBoard()
        {
	        if (!CanAbduct())
	        {
		        yield return new WaitForSeconds(0.3f);
		        yield break;
	        }
	        
	        BoardManager boardManager = Singleton<BoardManager>.Instance;
	        Singleton<ViewManager>.Instance.Controller.SwitchToControlMode(boardManager.ChoosingSlotViewMode, false);
	        
	        // Find who we want to abduct
	        IEnumerator onResolveOnBoard = ChooseTarget();
	        yield return onResolveOnBoard;
	        
	        // Make sure its a valid target
	        CardSlot cardSlot = m_targetedCardSlot;
	        CardSlot slot = StarCraftCore.Utils.GetSlot(this.Card);
	        if (cardSlot != null && (cardSlot.Card != null && cardSlot.Card != this.Card && cardSlot.Index != slot.Index))
	        {
		        // Pull them to the correct slot
		        yield return PullTargetToNearestSlot(cardSlot);
	        }
	        else
	        {
		        Card.Anim.StrongNegationEffect();
		        yield return new WaitForSeconds(0.3f);
	        }

	        Singleton<ViewManager>.Instance.Controller.SwitchToControlMode(boardManager.DefaultViewMode, false);
	        Singleton<ViewManager>.Instance.SwitchToView(Singleton<BoardManager>.Instance.CombatView, false, false);
	        Singleton<CombatPhaseManager>.Instance.VisualizeClearSniperAbility();
        }

        private IEnumerator ChooseTarget()
        {
	        CombatPhaseManager combatPhaseManager = Singleton<CombatPhaseManager>.Instance;
	        BoardManager boardManager = Singleton<BoardManager>.Instance;
	        List<CardSlot> allSlots = new List<CardSlot>(boardManager.AllSlots);

	        Action<CardSlot> callback1 = null;
	        Action<CardSlot> callback2 = null;
	        
	        combatPhaseManager.VisualizeStartSniperAbility(Card.slot);
	        
	        CardSlot cardSlot = Singleton<InteractionCursor>.Instance.CurrentInteractable as CardSlot;
	        if (cardSlot != null && allSlots.Contains(cardSlot))
	        {
		        combatPhaseManager.VisualizeAimSniperAbility(Card.slot, cardSlot);
	        }

	        List<CardSlot> allTargetSlots = allSlots;
	        List<CardSlot> validTargetSlots = allSlots;

	        m_targetedCardSlot = null;
	        Action<CardSlot> targetSelectedCallback;
	        if ((targetSelectedCallback = callback1) == null)
	        {
		        targetSelectedCallback = (callback1 = delegate(CardSlot s)
		        {
			        m_targetedCardSlot = s;
			        combatPhaseManager.VisualizeConfirmSniperAbility(s);
		        });
	        }
	        
	        Action<CardSlot> invalidTargetCallback = null;
	        Action<CardSlot> slotCursorEnterCallback;
	        if ((slotCursorEnterCallback = callback2) == null)
	        {
		        slotCursorEnterCallback = (callback2 = delegate(CardSlot s)
		        {
			        combatPhaseManager.VisualizeAimSniperAbility(Card.slot, s);
		        });
	        }

	        yield return boardManager.ChooseTarget(allTargetSlots, validTargetSlots, targetSelectedCallback, invalidTargetCallback, slotCursorEnterCallback, () => false, CursorType.Target);
        }

        private IEnumerator WiggleEffect()
        {
	        Card.Anim.StrongNegationEffect();
	        yield return new WaitForSeconds(0.3f);
        }
        
        private IEnumerator PullTargetToNearestSlot(CardSlot targetSlot)
        {
	        yield return base.PreSuccessfulTriggerSequence();

	        PlayableCard targetCard = targetSlot.Card;
	        List<CardSlot> slots = new List<CardSlot>(Singleton<BoardManager>.Instance.GetSlots(targetSlot.IsPlayerSlot));
	        slots.RemoveAll((slot) => slot.Card != null);

	        CardSlot currentSlot = StarCraftCore.Utils.GetSlot(this.Card);

	        CardSlot destinationSlot = null;
	        bool pullFromLeft = targetSlot.Index < currentSlot.Index;
	        foreach (CardSlot slot in slots)
	        {
		        if (pullFromLeft)
		        {
			        if (slot.Index <= currentSlot.Index && (destinationSlot == null || slot.Index > destinationSlot.Index))
			        {
				        destinationSlot = slot;
			        }
		        }
		        else
		        {
			        if (slot.Index >= currentSlot.Index && (destinationSlot == null || slot.Index < destinationSlot.Index))
			        {
				        destinationSlot = slot;
			        }
		        }
	        }

	        if (destinationSlot == null)
	        {
		        Plugin.Log.LogInfo("No destination slot found! " + pullFromLeft);
		        yield return WiggleEffect();
		        yield break;
	        }

	        Vector3 a = (targetSlot.transform.position + destinationSlot.transform.position) / 2f;
	        Tween.Position(targetCard.transform, a + Vector3.up * 0.5f, 0.05f, 0f, Tween.EaseIn, Tween.LoopType.None, null, null, true);
	        yield return Singleton<BoardManager>.Instance.AssignCardToSlot(targetCard, destinationSlot, 0.05f, null, true);
	        yield return new WaitForSeconds(0.2f);
        }

        private bool CanAbduct()
        {
	        return CanAbductSlots(Singleton<BoardManager>.Instance.opponentSlots) ||
	               CanAbductSlots(Singleton<BoardManager>.Instance.playerSlots);
        }

        private bool CanAbductSlots(List<CardSlot> slots)
        {
	        int currentSlotIndex = StarCraftCore.Utils.GetSlot(Card).Index;


	        CardSlot furthestLeftSlot = slots.Find((a) => a.Card != null && a.Card != this.Card);
	        CardSlot furthestRightSlot = slots.FindLast((a) => a.Card != null && a.Card != this.Card);

	        bool hasCardsToAbduct = furthestLeftSlot != null;
	        if (!hasCardsToAbduct)
	        {
		        return false;
	        }

	        int closestLeftEmptySlot = -1;
	        int closestRightEmptySlot = -1;
	        int furthestCardOnRight = int.MinValue;
	        int furthestCardOnLeft = int.MaxValue;
	        foreach (CardSlot slot in slots)
	        {
		        int slotIndex = slot.Index;
		        if (slot.Card == null)
		        {
			        // Empty slot
			        if (slotIndex > closestLeftEmptySlot && slotIndex <= currentSlotIndex)
			        {
				        // Pull from left
				        closestLeftEmptySlot = slotIndex;
			        }
			        if (slotIndex < closestRightEmptySlot && slotIndex >= currentSlotIndex)
			        {
				        // Pull from Right
				        closestRightEmptySlot = slotIndex;
			        }
		        }
		        else if(slot.Card != this.Card && slotIndex != currentSlotIndex)
		        {
			        // Card in slot
			        furthestCardOnRight = Mathf.Max(furthestCardOnRight, slotIndex);
			        furthestCardOnLeft = Mathf.Min(furthestCardOnLeft, slotIndex);
		        }
	        }

			// Can pull from left
	        if (closestLeftEmptySlot > furthestCardOnLeft)
	        {
		        return true;
	        }
	        
	        // Can pull from Right
	        if (closestRightEmptySlot < furthestCardOnRight)
	        {
		        return true;
	        }
	        
	        return false;
        }

    }
}