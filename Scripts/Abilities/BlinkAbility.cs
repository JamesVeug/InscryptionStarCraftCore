using System;
using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using Pixelplacement;
using UnityEngine;
using StarCraftCore.Scripts.Data.Sigils;

namespace StarCraftCore.Scripts.Abilities
{
    public class BlinkAbility : ACustomAbilityBehaviour<AbductAbility, AbilityData>
    {
	    public override Ability Ability => ability;
	    public static Ability ability = Ability.None;
	    
        private CardSlot m_targetedCardSlot = null;

        public static void Initialize(Type declaringType)
        {
	        ability = InitializeBase(Plugin.PluginGuid, declaringType, Plugin.Directory);
        }

        public override bool RespondsToTurnEnd(bool playerTurnEnd)
        {
	        BoardManager boardManager = Singleton<BoardManager>.Instance;
	        List<CardSlot> allSlots = new List<CardSlot>(boardManager.playerSlots);
	        int totalEmptySlots = allSlots.FindAll((a) => a.Card == null).Count;
	        return playerTurnEnd && totalEmptySlots > 0;
        }

        public override IEnumerator OnTurnEnd(bool playerTurnEnd)
        {
	        if (!CanBlink())
	        {
		        Card.Anim.StrongNegationEffect();
		        yield return new WaitForSeconds(0.3f);
		        yield break;
	        }
	        
	        BoardManager boardManager = Singleton<BoardManager>.Instance;
	        Singleton<ViewManager>.Instance.Controller.SwitchToControlMode(boardManager.ChoosingSlotViewMode, false);
	        
	        // Find who we want to abduct
	        IEnumerator choosingTarget = ChooseTarget();
	        yield return choosingTarget;
	        
	        // Make sure its a valid target
	        CardSlot targetSlot = m_targetedCardSlot;
	        CardSlot slot = StarCraftCore.Utils.GetSlot(this.Card);
	        if (targetSlot != null && targetSlot.Card == null)
	        {
		        // Move this card to the target slot
		        yield return BlinkToSlot(targetSlot);
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
	        List<CardSlot> allSlots = new List<CardSlot>(boardManager.playerSlots);

	        Action<CardSlot> callback1 = null;
	        Action<CardSlot> callback2 = null;
	        
	        combatPhaseManager.VisualizeStartSniperAbility(Card.slot);
	        
	        CardSlot cardSlot = Singleton<InteractionCursor>.Instance.CurrentInteractable as CardSlot;
	        if (cardSlot != null && allSlots.Contains(cardSlot))
	        {
		        combatPhaseManager.VisualizeAimSniperAbility(Card.slot, cardSlot);
	        }

	        List<CardSlot> allTargetSlots = allSlots;
	        List<CardSlot> validTargetSlots = allSlots.FindAll((a)=>a.Card == null || a.Card == this.Card);

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
        
        private IEnumerator BlinkToSlot(CardSlot targetSlot)
        {
	        yield return base.PreSuccessfulTriggerSequence();
	        
	        Vector3 a = (targetSlot.transform.position + targetSlot.transform.position) / 2f;
	        Tween.Position(Card.transform, a + Vector3.up * 0.5f, 0.05f, 0f, Tween.EaseIn, Tween.LoopType.None, null, null, true);
	        yield return Singleton<BoardManager>.Instance.AssignCardToSlot(Card, targetSlot, 0.05f, null, true);
	        yield return new WaitForSeconds(0.2f);
        }

        private bool CanBlink()
        {
	        return Singleton<BoardManager>.Instance.playerSlots.Find((a)=>a.Card == null);
        }
    }
}