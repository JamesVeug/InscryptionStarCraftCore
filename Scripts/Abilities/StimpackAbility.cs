using System;
using System.Collections;
using DiskCardGame;
using UnityEngine;
using StarCraftCore.Scripts.Data.Sigils;

namespace StarCraftCore.Scripts.Abilities
{
	public class StimpackAbility : ACustomActivatedAbility<StimpackAbility, StimpackAbilityData>
	{
		public override Ability Ability => ability;
		public static Ability ability = Ability.None;
		private CardModificationInfo tempMod;

		public static void Initialize(Type declaringType)
		{
			ability = InitializeBase(Plugin.PluginGuid, declaringType, Plugin.Directory);
		}

		public override bool CanActivate()
		{
			return Card.Health > LoadedData.damage;
		}

		public override IEnumerator Activate()
		{
			Card.Anim.PlayHitAnimation();
			//Take damage
			yield return Card.TakeDamage(LoadedData.damage, Card);
			
			//Add temporary mod for increased attack
			tempMod = new CardModificationInfo();
			tempMod.attackAdjustment = LoadedData.attackBoost;
			
			Card.TemporaryMods.Add(tempMod);
			Card.UpdateStatsText();
		}

		public override IEnumerator OnUpkeep(bool playerUpkeep)
		{
			//Remove temporary mod
			Card.TemporaryMods.Remove(tempMod);
			Card.UpdateStatsText();
			yield break;
		}
	}
}