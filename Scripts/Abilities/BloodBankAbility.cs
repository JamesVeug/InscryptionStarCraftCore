using System;
using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using UnityEngine;
using StarCraftCore.Scripts.Data.Sigils;

namespace StarCraftCore.Scripts.Abilities
{
    public class BloodBankAbility : ACustomAbilityBehaviour<BloodBankAbility, AbilityData>
	{
		public override Ability Ability => ability;
		public static Ability ability = Ability.None;

		private List<CardSlot> m_cachedList = new List<CardSlot>(1) {null};
		private int m_totalBloodSacrificed = 0;
		
		public static void Initialize(Type declaringType)
		{
			ability = InitializeBase(Plugin.PluginGuid, declaringType, Plugin.Directory);
		}

		public override void ManagedUpdate()
		{
			base.ManagedUpdate();
			if (StarCraftCore.Utils.GetSlot(Card) == null)
			{
				return;
			}
			
			// This card is on the board
			PlayableCard currentSacrificeDemandingCard = Singleton<BoardManager>.Instance.currentSacrificeDemandingCard;
			if (currentSacrificeDemandingCard == null)
			{
				m_totalBloodSacrificed = 0;
			}
		}

		public bool RespondsToOnOtherCardSacrificed(CardSlot slot, PlayableCard card)
		{
			return !Card.HasAbility(Ability.Sacrificial);
		}

		public IEnumerator OnOtherCardSacrificed(CardSlot slot, PlayableCard card)
		{
			// Record how much blood has been sacrificed before use
			int bloodCost = GetValueOfSacrifice(slot);
			m_totalBloodSacrificed += bloodCost;
			yield return null;
		}

		public override bool RespondsToSacrifice()
		{
			// Too many edge cases to consider Sacrificial, Triple blood and Blood Bank. So nullify it
			return !Card.HasAbility(Ability.Sacrificial);
		}

		public override IEnumerator OnSacrifice()
		{
			yield return PreSuccessfulTriggerSequence();
			
			// Add cost of Triple blood ability and any others that someone might make
			PlayableCard currentSacrificeDemandingCard = Singleton<BoardManager>.Instance.currentSacrificeDemandingCard;
			int bloodRequired = currentSacrificeDemandingCard.Info.BloodCost;
			if (Card.HasAbility(Ability.TripleBlood))
			{
				bloodRequired = Mathf.Max(Mathf.CeilToInt(bloodRequired / 3.0f), 1);
			}
			
			
			int healthToTake = Mathf.Clamp(Card.Health, 1, bloodRequired - m_totalBloodSacrificed);
			if (healthToTake >= Card.Health)
			{
				// Kill card
				Card.Anim.DeactivateSacrificeHoverMarker();
				yield return Card.Die(true, null, true);
			}
			else
			{
				Card.Anim.SetSacrificeHoverMarkerShown(false);
				Card.Anim.SetMarkedForSacrifice(false);
				Card.Anim.PlaySacrificeParticles();
				
				// Reduce health
				if (healthToTake > 0)
				{
					Card.Status.damageTaken += healthToTake;
				}
			}
			
			yield return base.LearnAbility(0.25f);
		}

		private int GetValueOfSacrifice(CardSlot slot)
		{
			m_cachedList[0] = slot; // Reuse the same list
			
			BoardManager boardManager = Singleton<BoardManager>.Instance;
			int bloodCost = boardManager.GetValueOfSacrifices(m_cachedList);
			return bloodCost;
		}
	}
}