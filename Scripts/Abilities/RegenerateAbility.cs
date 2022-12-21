using System;
using System.Collections;
using DiskCardGame;
using UnityEngine;
using StarCraftCore.Scripts.Data.Sigils;

namespace StarCraftCore.Scripts.Abilities
{
    public class RegenerateAbility : ACustomAbilityBehaviour<RegenerateAbility, RegenerateAbilityData>
    {
        public override Ability Ability => ability;
        public static Ability ability = Ability.None;
		
        public static void Initialize(Type declaringType)
        {
            ability = InitializeBase(Plugin.PluginGuid, declaringType, Plugin.Directory);
        }
        
        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return !Card.Dead && Card.Status.damageTaken > 0 && base.Card.OpponentCard != playerUpkeep;
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

            Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
            yield return new WaitForSeconds(0.15f);
            base.Card.Anim.PlayTransformAnimation();
            yield return new WaitForSeconds(0.15f);

            PlayableCard playableCard = this.Card;
            int newHealth = Mathf.Max(0, playableCard.Status.damageTaken - LoadedData.health); 
            playableCard.Status.damageTaken = newHealth;
            playableCard.UpdateStatsText();
            yield return new WaitForSeconds(0.5f);
            
            yield return base.LearnAbility(0f);
            yield break;
        }
    }
}