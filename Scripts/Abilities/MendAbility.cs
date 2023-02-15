using System;
using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using StarCraftCore.Scripts.Data.Sigils;
using UnityEngine;

namespace StarCraftCore.Scripts.Abilities
{
    public class MendAbility : ACustomAbilityBehaviour<MendAbility, AbilityData>
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
	        if (ValidSlots().Count == 0)
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
		        yield return HealTarget(cardSlot);
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

        private IEnumerator HealTarget(CardSlot targetSlot)
        {
	        yield return base.PreSuccessfulTriggerSequence();
	        PlayableCard targetCard = targetSlot.Card;

	        Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
	        yield return new WaitForSeconds(0.15f);
	        targetCard.Anim.PlayTransformAnimation();
	        yield return new WaitForSeconds(0.15f);

	        int newHealth = Mathf.Max(0, targetCard.Status.damageTaken - 1); 
	        targetCard.Status.damageTaken = newHealth;
	        targetCard.UpdateStatsText();
	        yield return new WaitForSeconds(0.5f);
        }

        private List<CardSlot> ValidSlots()
        {
	        return Singleton<BoardManager>.Instance.playerSlots.FindAll((a) => a.Card != null && a.Card != Card && a.Card.Status.damageTaken > 0);
        }
    }
}