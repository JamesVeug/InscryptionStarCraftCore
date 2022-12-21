using System;
using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using Pixelplacement;
using UnityEngine;
using StarCraftCore.Scripts.Data.Sigils;
using Object = UnityEngine.Object;

namespace StarCraftCore.Scripts.Abilities
{
    public class HookAbility : ACustomAbilityBehaviour<HookAbility, AbilityData>
    {
	    public override Ability Ability => ability;
	    public static Ability ability = Ability.None;
		
	    public static void Initialize(Type declaringType)
	    {
		    ability = InitializeBase(Plugin.PluginGuid, declaringType, Plugin.Directory);
	    }
	    
	    public override bool RespondsToResolveOnBoard()
        {
	        return StarCraftCore.Utils.GetSlot(Card).IsPlayerSlot;
        }

        public override IEnumerator OnResolveOnBoard()
        {
	        if (GetValidTargets().Count == 0)
	        {
		        Card.Anim.StrongNegationEffect();
		        yield return new WaitForSeconds(0.3f);
		        yield break;
	        }
	        
	        yield return ActivateSequence();

	        Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
	        yield return new WaitForSeconds(0.1f);
	        Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
	        Singleton<InteractionCursor>.Instance.InteractionDisabled = false;
        }
        
        private IEnumerator ActivateSequence()
		{
			yield return new WaitForSeconds(0.1f);
			Singleton<UIManager>.Instance.Effects.GetEffect<EyelidMaskEffect>().SetIntensity(0.6f, 0.2f);
			Singleton<ViewManager>.Instance.SwitchToView(View.OpponentQueue, false, false);
			yield return new WaitForSeconds(0.25f);
			Transform firstPersonItem = Singleton<FirstPersonController>.Instance.AnimController.SpawnFirstPersonAnimation("FirstPersonFishHook", null).transform;
			firstPersonItem.localPosition = new Vector3(0f, -1.25f, 4f) + Vector3.right * 3f;
			firstPersonItem.localEulerAngles = new Vector3(0f, 0f, 0f);
			Singleton<InteractionCursor>.Instance.InteractionDisabled = false;
			CardSlot target = null;
			List<CardSlot> validTargets = this.GetValidTargets();
			this.MoveItemToPosition(firstPersonItem, validTargets[validTargets.Count - 1].transform.position);
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield return Singleton<BoardManager>.Instance.ChooseTarget(this.GetAllTargets(), validTargets, delegate(CardSlot slot)
			{
				target = slot;
			}, new Action<CardSlot>(this.OnInvalidTargetSelected), delegate(CardSlot slot)
			{
				this.MoveItemToPosition(firstPersonItem, slot.transform.position);
			}, () => Singleton<ViewManager>.Instance.CurrentView != View.OpponentQueue || !Singleton<TurnManager>.Instance.IsPlayerMainPhase, CursorType.FishHook);
			if (target != null)
			{
				Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Locked;
				Singleton<InteractionCursor>.Instance.InteractionDisabled = true;
				yield return this.OnValidTargetSelected(target, firstPersonItem.gameObject);
			}
			
			Object.Destroy(firstPersonItem.gameObject);
			Singleton<UIManager>.Instance.Effects.GetEffect<EyelidMaskEffect>().SetIntensity(0f, 0.2f);
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield break;
		}
        
        private void MoveItemToPosition(Transform item, Vector3 targetPos)
        {
	        Tween.Position(item, new Vector3(targetPos.x, item.position.y, item.position.z), 0.2f, 0f, Tween.EaseOut, Tween.LoopType.None, null, null, true);
        }
        
        private IEnumerator OnValidTargetSelected(CardSlot target, GameObject firstPersonItem)
        {
	        AudioController.Instance.PlaySound3D("angler_use_hook", MixerGroup.TableObjectsSFX, target.transform.position, 1f, 0.1f, null, null, null, null, false);
	        firstPersonItem.GetComponentInChildren<Animator>().SetTrigger("hook");
	        yield return new WaitForSeconds(0.51f);
	        PlayableCard targetCard = target.Card;
	        targetCard.SetIsOpponentCard(false);
	        targetCard.transform.eulerAngles += new Vector3(0f, 0f, -180f);
	        yield return Singleton<BoardManager>.Instance.AssignCardToSlot(targetCard, target.opposingSlot, 0.33f, null, true);
	        if (targetCard.FaceDown)
	        {
		        targetCard.SetFaceDown(false, false);
		        targetCard.UpdateFaceUpOnBoardEffects();
	        }
	        yield return new WaitForSeconds(0.66f);
	        Tween.Position(firstPersonItem.transform, firstPersonItem.transform.position + Vector3.back * 4f, 0.2f, 0f, Tween.EaseOut, Tween.LoopType.None, null, null, true);
	        yield return new WaitForSeconds(0.15f);
	        yield break;
        }

        private List<CardSlot> GetValidTargets()
        {
	        List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
	        opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card == null || x.opposingSlot.Card != null || x.Card.Info.HasTrait(Trait.Uncuttable));
	        return opponentSlotsCopy;
        }
        
        private List<CardSlot> GetAllTargets()
        {
	        return Singleton<BoardManager>.Instance.OpponentSlotsCopy;
        }
        
        private void OnInvalidTargetSelected(CardSlot targetSlot)
        {
	        if (targetSlot.Card != null)
	        {
		        if (targetSlot.Card.Info.HasTrait(Trait.Uncuttable))
		        {
			        CustomCoroutine.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.ShowThenClear("You can't hook that one.", 2.5f, 0f, Emotion.Anger, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.Single, null));
			        return;
		        }
		        if (targetSlot.opposingSlot.Card != null)
		        {
			        CustomCoroutine.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.ShowThenClear("There's no space to pull that one into.", 3f, 0f, Emotion.Neutral, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.Single, null));
		        }
	        }
        }
    }
}