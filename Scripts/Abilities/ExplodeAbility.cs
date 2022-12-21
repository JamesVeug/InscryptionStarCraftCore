using System;
using System.Collections;
using DiskCardGame;

namespace StarCraftCore.Scripts.Abilities
{
    public class ExplodeAbility : SplashDamageAbility
	{
		public override Ability Ability => ability;
		public new static Ability ability = Ability.None;
		
		private bool m_startedAttack = false;
		
		public new static void Initialize(Type declaringType)
		{
			ability = InitializeBase(Plugin.PluginGuid, declaringType, Plugin.Directory);
		}
		
		public override IEnumerator OnDealDamage(int amount, PlayableCard target)
		{
			m_startedAttack = true;
			yield return base.OnDealDamage(amount, target);
		}

		public override bool RespondsToDealDamageDirectly(int amount)
		{
			return true;
		}

		public override IEnumerator OnDealDamageDirectly(int amount)
		{
			m_startedAttack = true;
			yield return base.OnDealDamageDirectly(amount);
		}

		public override bool RespondsToAttackEnded()
		{
			return m_startedAttack;
		}

		public override IEnumerator OnAttackEnded()
		{
			yield return base.PreSuccessfulTriggerSequence();
			if (base.Card != null && !base.Card.Dead)
			{
				yield return base.Card.Die(false, null, true);
				yield return base.LearnAbility(0.25f);
			}
			yield break;
		}
	}
}