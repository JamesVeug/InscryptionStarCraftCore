using System;
using System.Collections;
using DiskCardGame;
using UnityEngine;
using StarCraftCore.Scripts.Data.Sigils;

namespace StarCraftCore.Scripts.Abilities
{
	/// <summary>
	/// Credits to Cyantist for ThickShell ability from SigilADay
	/// </summary>
	public class ArmoredAbility : ACustomAbilityBehaviour<ArmoredAbility, ArmoredAbilityData>
	{
		public override Ability Ability => ability;
		public static Ability ability = Ability.None;
	    
		private bool attacked;
		private CardModificationInfo mod;

		public static void Initialize(Type declaringType)
		{
			ability = InitializeBase(Plugin.PluginGuid, declaringType, Plugin.Directory);
		}
		
		private void Start()
		{
			this.mod = new CardModificationInfo();
			this.mod.nonCopyable = true;
			this.mod.singletonId = "ArmouredHP";
			this.mod.healthAdjustment = 0;
			base.Card.AddTemporaryMod(this.mod);
		}

		public override bool RespondsToCardGettingAttacked(PlayableCard source)
		{
			return source == base.Card;
		}

		public override bool RespondsToAttackEnded()
		{
			return this.attacked;
		}

		public override IEnumerator OnCardGettingAttacked(PlayableCard source)
		{
			this.attacked = true;
			yield return base.PreSuccessfulTriggerSequence();
			this.mod.healthAdjustment = 1;
			yield break;
		}

		public override IEnumerator OnAttackEnded()
		{
			this.attacked = false;
			yield return new WaitForSeconds(0.1f);
			this.mod.healthAdjustment = 0;
			base.Card.HealDamage(LoadedData.damageBlocked);
			base.Card.Anim.LightNegationEffect();
			yield return new WaitForSeconds(0.1f);
			yield return base.LearnAbility(0.25f);
			yield break;
		}
	}
}