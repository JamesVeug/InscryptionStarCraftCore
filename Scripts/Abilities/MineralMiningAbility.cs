using System;
using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using InscryptionAPI.Card;
using Pixelplacement;
using UnityEngine;
using StarCraftCore.Scripts.Data.Sigils;

namespace StarCraftCore.Scripts.Abilities
{
    public class MineralMiningAbility : ACustomAbilityBehaviour<MineralMiningAbility, AbilityData>
    {
	    public override Ability Ability => ability;
	    public static Ability ability = Ability.None;
	    
        public static void Initialize(Type declaringType)
        {
	        ability = InitializeBase(Plugin.PluginGuid, declaringType, Plugin.Directory);
        }

        public override bool RespondsToTakeDamage(PlayableCard source)
        {
	        return source.IsPlayerCard();
        }

        public override IEnumerator OnTakeDamage(PlayableCard source)
        {
	        yield return base.OnTakeDamage(source);

	        Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
	        yield return new WaitForSeconds(0.1f);
	        base.Card.Anim.LightNegationEffect();
	        yield return base.PreSuccessfulTriggerSequence();
	        yield return Singleton<ResourcesManager>.Instance.AddBones(1, base.Card.Slot);
	        yield return new WaitForSeconds(0.1f);
	        yield return base.LearnAbility(0.1f);
	        Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
	        yield break;
        }
    }
}