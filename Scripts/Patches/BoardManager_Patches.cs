using System.Collections.Generic;
using DiskCardGame;
using HarmonyLib;
using StarCraftCore.Scripts.Abilities;

namespace StarCraftCore.Scripts.Patches
{
    [HarmonyPatch(typeof (BoardManager), "GetValueOfSacrifices")]
    public class BoardManager_GetValueOfSacrifices
    {
        public static void Postfix(List<CardSlot> sacrifices, ref BoardManager __instance, ref int __result)
        {
            int num = __result;
            foreach (CardSlot cardSlot in sacrifices)
            {
                PlayableCard card = cardSlot.Card;
                if (cardSlot != null && card != null)
                {
                    if (card.HasAbility(BloodBankAbility.ability) && !card.HasAbility(Ability.Sacrificial))
                    {
                        ProgressionData.SetAbilityLearned(BloodBankAbility.ability);
                        if (card.HasAbility(Ability.TripleBlood))
                        {
                            // -3 because Prefix adds +3 for Triple blood
                            num += card.Health * 3 - 3; // Card has 5 health. So total 15 blood for a sacrifice
                        }
                        else
                        {
                            // -1 because Prefix adds +1 for non-triple blood cards 
                            num += card.Health - 1; // Card has 5 health. So total 5 Health for a sacrifice
                        }
                    }
                }
            }

            __result = num;
        }
    }
    
    [HarmonyPatch(typeof (BoardManager), "SacrificesCreateRoomForCard")]
    public class BoardManager_SacrificesCreateRoomForCard
    {
        public static bool Prefix(PlayableCard card, List<CardSlot> sacrifices, ref BoardManager __instance, ref bool __result)
        {
            bool result = false;
            foreach (CardSlot cardSlot in __instance.PlayerSlotsCopy)
            {
                if (cardSlot.Card == null)
                {
                    result = true;
                    break;
                }

                if (card.Info.BloodCost < 1) continue;
                if (!sacrifices.Contains(cardSlot)) continue;
                if (cardSlot.Card.HasAbility(Ability.Sacrificial)) continue;
                if (!cardSlot.Card.CanBeSacrificed) continue;
                if (cardSlot.Card.HasAbility(BloodBankAbility.ability) && cardSlot.Card.Health > card.Info.BloodCost) continue;
                
                result = true;
                break;
            }
            
            __result = result;
            return false;
        }
    }
}