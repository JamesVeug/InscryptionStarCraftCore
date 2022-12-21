using System;
using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using HarmonyLib;
using StarCraftCore.Scripts.Abilities;
using StarCraftCore.Scripts.SpecialAbilities;
using StarCraftCore.Scripts.Utils;

namespace StarCraftCore.Scripts.Patches
{
    [HarmonyPatch(typeof (PlayableCard), "Sacrifice")]
    public class PlayableCard_Sacrifice
    {
        public static IEnumerator Postfix(IEnumerator result, PlayableCard __instance)
        {
            PlayableCard demandingCard = Singleton<BoardManager>.Instance.currentSacrificeDemandingCard;
            if (demandingCard == null)
            {
                yield return result;
                yield break;
            }

            IMoreTriggers moreTriggers = demandingCard.GetComponent<IMoreTriggers>();
            if (moreTriggers != null && moreTriggers.RespondsToOtherSacrificed(__instance))
            {
                yield return moreTriggers.OnOtherSacrificed(__instance);
            }
            
            List<CardSlot> allSlots = Singleton<BoardManager>.Instance.AllSlots;
            foreach (CardSlot slot in allSlots)
            {
                if (slot.Card != null && slot.Card != __instance && slot.Card.HasAbility(BloodBankAbility.ability))
                {
                    CardSlot sacrificingCardSlot = StarCraftCore.Utils.GetSlot(__instance);
                    BloodBankAbility bloodBankAbility = slot.Card.GetComponent<BloodBankAbility>();
                    if (bloodBankAbility.RespondsToOnOtherCardSacrificed(sacrificingCardSlot, __instance))
                    {
                        yield return bloodBankAbility.OnOtherCardSacrificed(sacrificingCardSlot, __instance);
                    }
                }
            }

            if (__instance.HasAbility(BloodBankAbility.ability) && !__instance.HasAbility(Ability.Sacrificial))
            {
                __instance.Anim.PlaySacrificeSound();
                if (__instance.TriggerHandler.RespondsToTrigger(Trigger.Sacrifice, Array.Empty<object>()))
                {
                    yield return __instance.TriggerHandler.OnTrigger(Trigger.Sacrifice, Array.Empty<object>());
                }
            }
            else
            {
                yield return result;
            }
        }
    }
    
    [HarmonyPatch(typeof (PlayableCard), "TransformIntoCard", new System.Type[] {typeof(CardInfo), typeof(Action), typeof(Action)})]
    public class PlayableCard_TransformIntoCard
    {
        public static IEnumerator Postfix(IEnumerator result, PlayableCard __instance, CardInfo evolvedInfo, Action onTransformedCallback = null, Action preTransformCallback = null)
        {
            yield return result;
            if (__instance.HasAbility(DetectorAbility.ability))
            {
                DetectorAbility detectorAbility = __instance.GetComponent<DetectorAbility>();
                if (detectorAbility.ShouldRevealCards())
                {
                    yield return detectorAbility.RevealSubmurgedCards();
                }
            }
        }
    }
}